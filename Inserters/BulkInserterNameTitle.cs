using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.Inserters
{
    public class BulkInserterNameTitle
    {
        public void InsertNamesTitlesBatch(DataTable table, SqlConnection conn)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
            {
                // Vigtigt! Peger nu på Names_Titles tabellen
                bulkCopy.DestinationTableName = "Names_Titles";

                // Vigtigt! Mapper nu NConst og TConst
                bulkCopy.ColumnMappings.Add("FK_NConst", "FK_NConst");
                bulkCopy.ColumnMappings.Add("FK_TConst", "FK_TConst");

                bulkCopy.BulkCopyTimeout = 600;

                bulkCopy.WriteToServer(table);
            }
        }
    }
}
