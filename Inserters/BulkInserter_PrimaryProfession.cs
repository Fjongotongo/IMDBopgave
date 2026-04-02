using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.Inserters
{
    public class BulkInserter_PrimaryProfession
    {
        public void InsertPP(HashSet<string> PP, SqlConnection sqlConn)
        {
            DataTable PPTable = new DataTable();
            PPTable.Columns.Add("PrimaryProfession", typeof(string));

            foreach (string pp in PP)
            {
                PPTable.Rows.Add(pp);
            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn))
            {
                bulkCopy.DestinationTableName = "PrimaryProfessions";

                // VIGTIGT: Her fortæller vi præcis, at "Genre"-kolonnen i vores DataTable 
                // skal overføres til "Genre"-kolonnen i databasen. 
                // Så ignorerer den automatisk GenreID, som SQL selv styrer.
                bulkCopy.ColumnMappings.Add("PrimaryProfession", "PrimaryProfession");

                bulkCopy.WriteToServer(PPTable);
            }
        }
    }
}
