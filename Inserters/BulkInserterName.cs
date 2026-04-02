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
    public class BulkInserterName
    {
        public void InsertNames(List<Name_Model> names, SqlConnection sqlConn)
        {
            DataTable nameTable = new DataTable();
            nameTable.Columns.Add("NConst", typeof(string));
            nameTable.Columns.Add("PrimaryName", typeof(string));
            nameTable.Columns.Add("BirthYear", typeof(int));
            nameTable.Columns.Add("DeathYear", typeof(int));

            foreach (Name_Model name in names)
            {
                nameTable.Rows.Add(name.NConst, name.PrimaryName, name.BirthYear ?? (object)DBNull.Value, name.DeathYear ?? (object)DBNull.Value);
            }
            SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn)
            {
                DestinationTableName = "Names"
            };

            bulkCopy.WriteToServer(nameTable);
        }
    }
}