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

                bulkCopy.ColumnMappings.Add("Genre", "Genre");

                bulkCopy.WriteToServer(genreTable);
            }
        }
    }
}