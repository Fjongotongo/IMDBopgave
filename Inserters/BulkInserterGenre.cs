using IMDBopgave.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.Inserters
{
    public class BulkInserterGenre
    {
        public void InsertGenres(List<Genre_Model> genres, SqlConnection sqlConn)
        {
            DataTable genreTable = new DataTable();
            genreTable.Columns.Add("Genre", typeof(string));

            foreach (Genre_Model genre in genres)
            {
                if (genreTable.Rows.Contains(genre.Genre))
                {
                    continue;
                }
                else
                {
                    genreTable.Rows.Add(genre.Genre);
                }

            }

            SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn)
            {
                DestinationTableName = "Genres"
            };

            bulkCopy.WriteToServer(genreTable);
        }
    }
}
