using IMDBopgave.Inserters;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.DataInserting
{
    public class BulkTitleCrewDataInsert
    {
        public static void TitleCrewInserting()
        {
            SqlConnection sqlConn = new SqlConnection(
            "Server=localhost;Database=MovieDB;Integrated security=True;" +
            "Trusted_Connection=True;TrustServerCertificate=True;");
            sqlConn.Open();

            BulkInserterTitleCrew titleCrewInserter = new BulkInserterTitleCrew();

            // 1. Læs den RIGTIGE fil med navne
            var allLines = File.ReadLines("");

            Console.WriteLine("Henter alle gyldige TConst (Film-ID'er) fra databasen...");

            // 2. Vi bruger et HashSet til lynhurtigt at tjekke, om en film findes i databasen
            HashSet<int> validTitles = new HashSet<int>();

            using (SqlCommand cmd = new SqlCommand("SELECT TConst FROM Titles", sqlConn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    validTitles.Add(reader.GetInt32(0));
                }
            }
            Console.WriteLine($"Fandt {validTitles.Count} gyldige film i databasen.");

            // 3. Klargør en tabel til at holde relationerne (FK_NConst og FK_TConst)
            DataTable namesTitlesTable = new DataTable();
            namesTitlesTable.Columns.Add("FK_NConst", typeof(int));
            namesTitlesTable.Columns.Add("FK_TConst", typeof(int));

            Console.WriteLine("Læser Names og bygger relationer...");

            int relationCount = 0;

            foreach (string line in allLines.Skip(1))
            {
                string[] parts = line.Split('\t');

                // name.basics har 6 kolonner. Kolonne index 5 er "knownForTitles"
                if (parts.Length == 6)
                {
                    // Hent NConst og fjern "nm"
                    string rawNConst = parts[0].Substring(2);

                    if (int.TryParse(rawNConst, out int nconstInt))
                    {
                        string knownForString = parts[5];

                        if (knownForString != "\\N")
                        {
                            // Split på komma, da en person kan have flere kendte titler
                            string[] individualTitles = knownForString.Split(',');

                            foreach (string t in individualTitles)
                            {
                                // Hent TConst og fjern "tt"
                                string cleanTitle = t.Trim().Substring(2);

                                if (int.TryParse(cleanTitle, out int tconstInt))
                                {
                                    // TJEK: Findes denne film overhovedet i vores SQL database?
                                    if (validTitles.Contains(tconstInt))
                                    {
                                        namesTitlesTable.Rows.Add(nconstInt, tconstInt);
                                        relationCount++;
                                    }
                                }
                            }
                        }
                    }
                }

                // Batching: Skyd afsted pr. 500.000 rækker
                if (namesTitlesTable.Rows.Count >= 500000)
                {
                    Console.WriteLine($"Indsætter batch... (Har fundet {relationCount} relationer indtil videre)");
                    titleCrewInserter.InsertNamesTitlesBatch(namesTitlesTable, sqlConn);
                    namesTitlesTable.Clear();
                }
            }

            // Indsæt resterne
            if (namesTitlesTable.Rows.Count > 0)
            {
                Console.WriteLine("Indsætter sidste batch...");
                titleCrewInserter.InsertNamesTitlesBatch(namesTitlesTable, sqlConn);
            }

            Console.WriteLine($"Færdig! Indsatte i alt {relationCount} Names_Titles relationer.");

            sqlConn.Close();
        }
    }
}
