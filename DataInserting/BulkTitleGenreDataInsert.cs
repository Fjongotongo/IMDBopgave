using IMDBopgave.Inserters;
using IMDBopgave.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.DataInserting
{
    public static class BulkTitleGenreDataInsert
    {
        public static void TitleGenreInserting()
        {
            SqlConnection sqlConn = new SqlConnection(
                "Server=localhost;Database=MovieDB;Integrated security=True;" +
                "Trusted_Connection=True;TrustServerCertificate=True;");
            sqlConn.Open();

            BulkInserterTitleGenre titleGenreInserter = new BulkInserterTitleGenre();

            var allLines = File.ReadLines("C:\\Users\\leo\\Downloads\\title.basics.tsv\\title.basics.tsv");


            Console.WriteLine("Henter Genre-ID'er fra databasen...");

            // 1. Opret en Dictionary (vores lynhurtige opslagsbog: "GenreNavn" -> GenreID)
            Dictionary<string, int> genreMap = new Dictionary<string, int>();

            using (SqlCommand cmd = new SqlCommand("SELECT GenreID, Genre FROM Genres", sqlConn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string genreName = reader.GetString(1);
                    genreMap.Add(genreName, id);
                }
            }
            Console.WriteLine($"Fandt {genreMap.Count} genrer i databasen.");

            // 2. Klargør en tabel til at holde relationerne (FK_TConst og FK_GenreID)
            DataTable titlesGenresTable = new DataTable();
            titlesGenresTable.Columns.Add("FK_TConst", typeof(int));
            titlesGenresTable.Columns.Add("FK_GenreID", typeof(int));

            Console.WriteLine("Læser film og bygger relationer...");

            // Vi bruger en tæller til at holde styr på, hvor mange vi har
            int relationCount = 0;

            foreach (string movie in allLines.Skip(1))
            {
                string[] parts = movie.Split('\t');

                if (parts.Length == 9)
                {
                    // IMDb ID'er starter med "tt". Vi fjerner "tt" for at få en ren INT til din database
                    string rawTConst = parts[0].Substring(2);

                    if (int.TryParse(rawTConst, out int tconstInt))
                    {
                        string genresString = parts[8];

                        if (genresString != "\\N")
                        {
                            string[] individualGenres = genresString.Split(',');

                            foreach (string g in individualGenres)
                            {
                                string cleanGenre = g.Trim();

                                // Slå genren op i vores Dictionary for at få det rigtige ID
                                if (genreMap.ContainsKey(cleanGenre))
                                {
                                    int genreId = genreMap[cleanGenre];
                                    titlesGenresTable.Rows.Add(tconstInt, genreId);
                                    relationCount++;
                                }
                            }
                        }
                    }
                }

                // VIGTIGT: Fordi der er over 12 mio film (og måske 25 mio relationer), 
                // sender vi dem til SQL i "bidder" af 500.000 for ikke at crashe din RAM.
                if (titlesGenresTable.Rows.Count >= 500000)
                {
                    Console.WriteLine($"Indsætter batch... (Har fundet {relationCount} relationer indtil videre)");
                    titleGenreInserter.InsertTitlesGenresBatch(titlesGenresTable, sqlConn);
                    titlesGenresTable.Clear(); // Tøm tabellen så vi er klar til de næste 500.000
                }
            }

            // Indsæt de allersidste der er tilbage i tabellen (hvis der er nogen)
            if (titlesGenresTable.Rows.Count > 0)
            {
                Console.WriteLine("Indsætter sidste batch...");
                titleGenreInserter.InsertTitlesGenresBatch(titlesGenresTable, sqlConn);
            }

            Console.WriteLine($"Færdig! Indsatte i alt {relationCount} genre-relationer.");

            sqlConn.Close();
        }
    }
}
