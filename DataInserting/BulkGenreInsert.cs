using IMDBopgave.Inserters;
using IMDBopgave.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.DataInserting
{
    public static class BulkGenreInsert
    {
        public static void InsertingGenres()
        {
            BulkInserterGenre genreInserter = new BulkInserterGenre();
            List<Genre_Model> genres = new List<Genre_Model>();

            SqlConnection sqlConn = new SqlConnection(
                "Server=localhost;Database=MovieDB;Integrated security=True;" +
                "Trusted_Connection=True;TrustServerCertificate=True;");
            sqlConn.Open();

            var allLines = File.ReadLines("C:\\Users\\leo\\Downloads\\title.basics.tsv\\title.basics.tsv");

            // Brug et HashSet i stedet for en List til at samle unikke genrer
            HashSet<string> uniqueGenres = new HashSet<string>();

            foreach (string movie in allLines.Skip(1))
            {
                string[] parts = movie.Split('\t');

                if (parts.Length == 9)
                {
                    string genresString = parts[8];

                    // IMDb bruger "\N" for tomme felter. Dem vil vi ikke have ind i databasen.
                    if (genresString != "\\N")
                    {
                        // Nu splitter vi KUN genre-teksten på komma
                        string[] individualGenres = genresString.Split(',');

                        foreach (string g in individualGenres)
                        {
                            // HashSet sørger automatisk for, at der ikke kommer dubletter ind
                            uniqueGenres.Add(g.Trim());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid line: " + movie);
                }
            }

            // Kald din inserter og send dit HashSet med
            // BulkInserterGenre genreInserter = new BulkInserterGenre();
            genreInserter.InsertGenres(uniqueGenres, sqlConn);

            sqlConn.Close();
        }
    }
}
