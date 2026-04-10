using IMDBopgave.Inserters;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace IMDBopgave.DataInserting
{
    public static class BulkNameTitleDataInsert
    {
        public static void NameTitleInserting()
        {
            SqlConnection sqlConn = new SqlConnection(
            "Server=localhost;Database=MovieDB;Integrated security=True;" +
            "Trusted_Connection=True;TrustServerCertificate=True;");
            sqlConn.Open();

            BulkInserterNameTitle nameTitleInserter = new BulkInserterNameTitle();

            // Name.basics.tsv
            var allLines = File.ReadLines("");

            Console.WriteLine("Henter alle gyldige TConst (Film-ID'er) fra databasen...");

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

            DataTable namesTitlesTable = new DataTable();
            namesTitlesTable.Columns.Add("FK_NConst", typeof(int));
            namesTitlesTable.Columns.Add("FK_TConst", typeof(int));

            Console.WriteLine("Læser Names og bygger relationer...");

            int relationCount = 0;

            foreach (string line in allLines.Skip(1))
            {
                string[] parts = line.Split('\t');

                if (parts.Length == 6)
                {
                    string rawNConst = parts[0].Substring(2);

                    if (int.TryParse(rawNConst, out int nconstInt))
                    {
                        string knownForString = parts[5];

                        if (knownForString != "\\N")
                        {
                            string[] individualTitles = knownForString.Split(',');

                            foreach (string t in individualTitles)
                            {
                                string cleanTitle = t.Trim().Substring(2);

                                if (int.TryParse(cleanTitle, out int tconstInt))
                                {
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

                if (namesTitlesTable.Rows.Count >= 500000)
                {
                    Console.WriteLine($"Indsætter batch... (Har fundet {relationCount} relationer indtil videre)");
                    nameTitleInserter.InsertNamesTitlesBatch(namesTitlesTable, sqlConn);
                    namesTitlesTable.Clear();
                }
            }

            if (namesTitlesTable.Rows.Count > 0)
            {
                Console.WriteLine("Indsætter sidste batch...");
                nameTitleInserter.InsertNamesTitlesBatch(namesTitlesTable, sqlConn);
            }

            Console.WriteLine($"Færdig! Indsatte i alt {relationCount} Names_Titles relationer.");

            sqlConn.Close();
        }
    }
}