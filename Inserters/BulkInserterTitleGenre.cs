using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.Inserters
{
    public class BulkInserterTitleGenre
    {
        public void InsertTitlesGenresBatch(DataTable table, SqlConnection conn)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.DestinationTableName = "Titles_Genres";

                bulkCopy.ColumnMappings.Add("FK_TConst", "FK_TConst");
                bulkCopy.ColumnMappings.Add("FK_GenreID", "FK_GenreID");

                bulkCopy.BulkCopyTimeout = 600;

                bulkCopy.WriteToServer(table);
            }
        }
    }
}
