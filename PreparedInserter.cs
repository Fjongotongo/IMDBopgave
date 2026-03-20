using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave
{
    public class PreparedInserter : IInserter
    {
        public void InsertTitles(List<Title_Model> titles, SqlConnection sqlConn)
        {
            string query = "INSERT INTO Titles (" +
                    "TConst, " +
                    "TitleType, " +
                    "PrimaryTitle, " +
                    "OriginalTitle, " +
                    "IsAdult, " +
                    "StartYear, " +
                    "EndYear, " +
                    "RuntimeMinutes) " +
                               "VALUES (@TConst, @TitleType, @PrimaryTitle," +
                               "@OriginalTitle, @IsAdult," +
                               "@StartYear, @EndYear, @RuntimeMinutes)";
            SqlCommand sqlComm = new SqlCommand(query, sqlConn);
            sqlComm.Prepare();
            foreach (Title_Model movie in titles)
            {
                sqlComm.Parameters.Clear();
                sqlComm.Parameters.AddWithValue("@TConst", movie.TConst);
                sqlComm.Parameters.AddWithValue("@TitleType", movie.TitleType);
                sqlComm.Parameters.AddWithValue("@PrimaryTitle", movie.PrimaryTitle);
                sqlComm.Parameters.AddWithValue("@OriginalTitle", movie.OriginalTitle);
                sqlComm.Parameters.AddWithValue("@IsAdult", movie.IsAdult);
                sqlComm.Parameters.AddWithValue("@StartYear", (object)movie.StartYear ?? DBNull.Value);
                sqlComm.Parameters.AddWithValue("@EndYear", (object)movie.EndYear ?? DBNull.Value);
                sqlComm.Parameters.AddWithValue("@RuntimeMinutes", (object)movie.RuntimeMinutes ?? DBNull.Value);
                try
                {
                    sqlComm.ExecuteNonQuery();
                }
                catch (SqlException sqlex)
                {
                    Console.WriteLine("Error inserting query:\r\n" + query);
                }
            }
        }
    }
}
