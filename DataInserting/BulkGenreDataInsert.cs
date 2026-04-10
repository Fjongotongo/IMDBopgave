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
    public static class BulkGenreDataInsert
    {
        public static void InsertingGenres()
        {
            BulkInserterGenre genreInserter = new BulkInserterGenre();
            List<Genre_Model> genres = new List<Genre_Model>();

            SqlConnection sqlConn = new SqlConnection(
                "Server=localhost;Database=MovieDB;Integrated security=True;" +
                "Trusted_Connection=True;TrustServerCertificate=True;");
            sqlConn.Open();

            // Title.basics.tsv
            var allLines = File.ReadLines("C:\\Users\\leo\\Downloads\\title.basics.tsv\\title.basics.tsv");

            HashSet<string> uniqueGenres = new HashSet<string>();

            foreach (string movie in allLines.Skip(1))
            {
                string[] parts = movie.Split('\t');

                if (parts.Length == 9)
                {
                    string genresString = parts[8];

                    if (genresString != "\\N")
                    {
                        string[] individualGenres = genresString.Split(',');

                        foreach (string g in individualGenres)
                        {
                            uniqueGenres.Add(g.Trim());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid line: " + movie);
                }
            }

            genreInserter.InsertGenres(uniqueGenres, sqlConn);

            sqlConn.Close();
        }
    }
}
