using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.Inserters
{
    public class BulkInserterPrimaryProfession
    {
        public void InsertPP(HashSet<string> primaryProfession, SqlConnection sqlConn)
        {
            DataTable PrimaryProfessionTable = new DataTable();
            PrimaryProfessionTable.Columns.Add("PrimaryProfession", typeof(string));

            foreach (string pp in primaryProfession)
            {
                PrimaryProfessionTable.Rows.Add(pp);
            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn))
            {
                bulkCopy.DestinationTableName = "PrimaryProfessions";

                bulkCopy.ColumnMappings.Add("PrimaryProfession", "PrimaryProfession");

                bulkCopy.WriteToServer(PrimaryProfessionTable);
            }
        }
    }
}
