using IMDBopgave.Inserters;
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

            BulkInserterNamePrimaryProfession namePrimaryProfessionInserter = new BulkInserterNamePrimaryProfession();

            var allLines = File.ReadLines("C:/Users/marti/OneDrive/Skrivebord/SQL-databaser/name.basics.tsv");

            Console.WriteLine("Henter Profession-ID'er fra databasen...");

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

            DataTable namesProfessionsTable = new DataTable();
            namesProfessionsTable.Columns.Add("FK_NConst", typeof(int));
            namesProfessionsTable.Columns.Add("FK_PPID", typeof(int));

            Console.WriteLine("Læser Names og bygger professions-relationer...");

            int relationCount = 0;

            foreach (string line in allLines.Skip(1))
            {
                string[] parts = line.Split('\t');

                if (parts.Length == 6)
                {
                    string rawNConst = parts[0].Substring(2);

                    if (int.TryParse(rawNConst, out int nconstInt))
                    {
                        string professionsString = parts[4];

                        if (professionsString != "\\N")
                        {
                            string[] individualProfessions = professionsString.Split(',');

                            foreach (string p in individualProfessions)
                            {
                                string cleanProfession = p.Trim();

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

                if (namesProfessionsTable.Rows.Count >= 500000)
                {
                    Console.WriteLine($"Indsætter batch... (Har fundet {relationCount} relationer indtil videre)");
                    namePrimaryProfessionInserter.InsertNamesProfessionsBatch(namesProfessionsTable, sqlConn);
                    namesProfessionsTable.Clear();
                }
            }

            if (namesProfessionsTable.Rows.Count > 0)
            {
                Console.WriteLine("Indsætter sidste batch...");
                namePrimaryProfessionInserter.InsertNamesProfessionsBatch(namesProfessionsTable, sqlConn);
            }

            Console.WriteLine($"Færdig! Indsatte i alt {relationCount} Names_PrimaryProfessions relationer.");

            sqlConn.Close();
        }
    }
}