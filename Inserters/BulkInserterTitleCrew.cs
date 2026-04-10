using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.Inserters
{
    public class BulkInserterTitleCrew
    {
        public void InsertTitlesCrewBatch(DataTable table, SqlConnection conn)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.DestinationTableName = "Titles_Crew";

                bulkCopy.ColumnMappings.Add("FK_TConst", "FK_TConst");
                bulkCopy.ColumnMappings.Add("FK_NConst", "FK_NConst");
                bulkCopy.ColumnMappings.Add("FK_RoleID", "FK_RoleID");

                bulkCopy.BulkCopyTimeout = 600;

                bulkCopy.WriteToServer(table);
            }
        }
    }
}
