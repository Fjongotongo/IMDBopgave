using IMDBopgave.Inserters;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

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

            // Title.crew.tsv
            var allLines = File.ReadLines("C:/Users/felsd/Desktop/SQLDatabaser/SQLObligatorisk/Filer/title.crew.tsv");

            Console.WriteLine("Henter gyldige TConst (Film-ID'er) fra databasen...");
            HashSet<int> validTitles = new HashSet<int>();
            using (SqlCommand cmd = new SqlCommand("SELECT TConst FROM Titles", sqlConn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read()) { validTitles.Add(reader.GetInt32(0)); }
            }
            Console.WriteLine($"Fandt {validTitles.Count} film.");

            Console.WriteLine("Henter gyldige NConst (Navne-ID'er) fra databasen...");
            HashSet<int> validNames = new HashSet<int>();
            using (SqlCommand cmd = new SqlCommand("SELECT NConst FROM Names", sqlConn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read()) { validNames.Add(reader.GetInt32(0)); }
            }
            Console.WriteLine($"Fandt {validNames.Count} navne.");

            Console.WriteLine("Henter RoleID'er fra databasen...");
            Dictionary<string, int> roleMap = new Dictionary<string, int>();
            using (SqlCommand cmd = new SqlCommand("SELECT RoleID, RoleName FROM CrewRoles", sqlConn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    roleMap.Add(reader.GetString(1), reader.GetInt32(0));
                }
            }

            if (!roleMap.ContainsKey("Director") || !roleMap.ContainsKey("Writer"))
            {
                Console.WriteLine("FEJL: Mangler 'director' eller 'writer' i CrewRoles tabellen i SQL!");
                return;
            }

            DataTable titlesCrewTable = new DataTable();
            titlesCrewTable.Columns.Add("FK_TConst", typeof(int));
            titlesCrewTable.Columns.Add("FK_NConst", typeof(int));
            titlesCrewTable.Columns.Add("FK_RoleID", typeof(int));

            Console.WriteLine("Læser title.crew og bygger relationer...");

            int relationCount = 0;

            foreach (string line in allLines.Skip(1))
            {
                string[] parts = line.Split('\t');

                if (parts.Length == 3)
                {
                    string rawTConst = parts[0].Substring(2);

                    if (int.TryParse(rawTConst, out int tconstInt))
                    {
                        if (validTitles.Contains(tconstInt))
                        {
                            if (parts[1] != "\\N")
                            {
                                string[] directors = parts[1].Split(',');
                                foreach (string d in directors)
                                {
                                    string cleanDirector = d.Trim().Substring(2); 
                                    if (int.TryParse(cleanDirector, out int nconstInt) && validNames.Contains(nconstInt))
                                    {
                                        titlesCrewTable.Rows.Add(tconstInt, nconstInt, roleMap["Director"]);
                                        relationCount++;
                                    }
                                }
                            }

                            if (parts[2] != "\\N")
                            {
                                string[] writers = parts[2].Split(',');
                                foreach (string w in writers)
                                {
                                    string cleanWriter = w.Trim().Substring(2); 
                                    if (int.TryParse(cleanWriter, out int nconstInt) && validNames.Contains(nconstInt))
                                    {
                                        titlesCrewTable.Rows.Add(tconstInt, nconstInt, roleMap["Writer"]);
                                        relationCount++;
                                    }
                                }
                            }
                        }
                    }
                }

                if (titlesCrewTable.Rows.Count >= 500000)
                {
                    Console.WriteLine($"Indsætter batch... (Har fundet {relationCount} relationer indtil videre)");
                    titleCrewInserter.InsertTitlesCrewBatch(titlesCrewTable, sqlConn);
                    titlesCrewTable.Clear();
                }
            }

            if (titlesCrewTable.Rows.Count > 0)
            {
                Console.WriteLine("Indsætter sidste batch...");
                titleCrewInserter.InsertTitlesCrewBatch(titlesCrewTable, sqlConn);
            }

            Console.WriteLine($"Færdig! Indsatte i alt {relationCount} Titles_Crew relationer.");

            sqlConn.Close();
        }
    }
}