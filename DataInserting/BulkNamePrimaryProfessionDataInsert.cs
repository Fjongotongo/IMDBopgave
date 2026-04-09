using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace IMDBopgave.DataInserting
{
    public static class BulkNamePrimaryProfessionDataInsert
    {
        public static void NameProfessionInserting()
        {
            SqlConnection sqlConn = new SqlConnection(
            "Server=localhost;Database=MovieDB;Integrated security=True;" +
            "Trusted_Connection=True;TrustServerCertificate=True;");
            sqlConn.Open();

            // 1. Læs filen med navne
            var allLines = File.ReadLines("C:/Users/marti/OneDrive/Skrivebord/SQL-databaser/name.basics.tsv");

            Console.WriteLine("Henter Profession-ID'er fra databasen...");

            // 2. Vi bruger en Dictionary ("actor" -> 1, "producer" -> 2 osv.)
            Dictionary<string, int> professionMap = new Dictionary<string, int>();

            using (SqlCommand cmd = new SqlCommand("SELECT PPID, PrimaryProfession FROM PrimaryProfessions", sqlConn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string professionName = reader.GetString(1);
                    professionMap.Add(professionName, id);
                }
            }
            Console.WriteLine($"Fandt {professionMap.Count} professioner i databasen.");

            // 3. Klargør en tabel til at holde relationerne (FK_NConst og FK_PPID)
            DataTable namesProfessionsTable = new DataTable();
            namesProfessionsTable.Columns.Add("FK_NConst", typeof(int));
            namesProfessionsTable.Columns.Add("FK_PPID", typeof(int));

            Console.WriteLine("Læser Names og bygger professions-relationer...");

            int relationCount = 0;

            foreach (string line in allLines.Skip(1))
            {
                string[] parts = line.Split('\t');

                // name.basics har 6 kolonner
                if (parts.Length == 6)
                {
                    // Hent NConst og fjern "nm"
                    string rawNConst = parts[0].Substring(2);

                    if (int.TryParse(rawNConst, out int nconstInt))
                    {
                        // primaryProfession ligger på index 4
                        string professionsString = parts[4];

                        if (professionsString != "\\N")
                        {
                            // Split på komma, da en person kan have flere professioner
                            string[] individualProfessions = professionsString.Split(',');

                            foreach (string p in individualProfessions)
                            {
                                string cleanProfession = p.Trim();

                                // TJEK: Slå professionen op i vores Dictionary for at få ID'et
                                if (professionMap.ContainsKey(cleanProfession))
                                {
                                    int ppid = professionMap[cleanProfession];
                                    namesProfessionsTable.Rows.Add(nconstInt, ppid);
                                    relationCount++;
                                }
                            }
                        }
                    }
                }

                // Batching: Skyd afsted pr. 500.000 rækker for at spare RAM
                if (namesProfessionsTable.Rows.Count >= 500000)
                {
                    Console.WriteLine($"Indsætter batch... (Har fundet {relationCount} relationer indtil videre)");
                    InsertNamesProfessionsBatch(namesProfessionsTable, sqlConn);
                    namesProfessionsTable.Clear();
                }
            }

            // Indsæt resterne
            if (namesProfessionsTable.Rows.Count > 0)
            {
                Console.WriteLine("Indsætter sidste batch...");
                InsertNamesProfessionsBatch(namesProfessionsTable, sqlConn);
            }

            Console.WriteLine($"Færdig! Indsatte i alt {relationCount} Names_PrimaryProfessions relationer.");

            sqlConn.Close();
        }

        // Hjælpemetode til Names_PrimaryProfessions
        static void InsertNamesProfessionsBatch(DataTable table, SqlConnection conn)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.DestinationTableName = "Names_PrimaryProfessions";

                // Map C# kolonner til SQL kolonner
                bulkCopy.ColumnMappings.Add("FK_NConst", "FK_NConst");
                bulkCopy.ColumnMappings.Add("FK_PPID", "FK_PPID");

                bulkCopy.BulkCopyTimeout = 600;

                bulkCopy.WriteToServer(table);
            }
        }
    }
}