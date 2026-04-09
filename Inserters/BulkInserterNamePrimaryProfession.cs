using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.Inserters
{
    public class BulkInserterNamePrimaryProfession
    {
        public void InsertNamesProfessionsBatch(DataTable table, SqlConnection conn)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.DestinationTableName = "Names_PrimaryProfessions";

                // Map C# kolonner til SQL kolonner
                bulkCopy.ColumnMappings.Add("FK_NConst", "FK_NConst");
                bulkCopy.ColumnMappings.Add("FK_PPID", "FK_PPID");

                bulkCopy.BulkCopyTimeout = 600;

                bulkCopy.WriteToServer(table);
            }
        }
    }
}
