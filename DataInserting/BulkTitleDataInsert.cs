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
    public static class BulkTitleDataInsert
    {
        public static void BulkInsertingTitle()
        {
            List<Title_Model> movies = new List<Title_Model>();

            IInserter inserter = new BulkInserterTitle();

            SqlConnection sqlConn = new SqlConnection(
            "Server=localhost;Database=MovieDB;Integrated security=True;" +
            "Trusted_Connection=True;TrustServerCertificate=True;");
            sqlConn.Open();


            var allLines = File.ReadLines("C:\\Users\\leo\\Downloads\\title.basics.tsv\\title.basics.tsv");

            foreach (string movie in allLines.Skip(1))
            {
                string[] parts = movie.Split('\t');
                if (parts.Length == 9)
                {
                    movies.Add(new Title_Model(parts));
                }
                else
                {
                    Console.WriteLine("Invalid line: " + movie);
                }

                if (movies.Count >= 1000000)
                {
                    inserter.InsertTitles(movies, sqlConn);
                    movies.Clear();
                }
            }

            if (movies.Count > 0)
            {
                inserter.InsertTitles(movies, sqlConn);
            }

            sqlConn.Close();
        }
    }
}
