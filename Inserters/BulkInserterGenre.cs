using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace IMDBopgave.Inserters
{
    public class BulkInserterGenre
    {
        public void InsertGenres(HashSet<string> genres, SqlConnection sqlConn)
        {
            DataTable genreTable = new DataTable();
            genreTable.Columns.Add("Genre", typeof(string));

            foreach (string genre in genres)
            {
                genreTable.Rows.Add(genre);
            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn))
            {
                bulkCopy.DestinationTableName = "Genres";

                // VIGTIGT: Her fortæller vi præcis, at "Genre"-kolonnen i vores DataTable 
                // skal overføres til "Genre"-kolonnen i databasen. 
                // Så ignorerer den automatisk GenreID, som SQL selv styrer.
                bulkCopy.ColumnMappings.Add("Genre", "Genre");

                bulkCopy.WriteToServer(genreTable);
            }
        }
    }
}