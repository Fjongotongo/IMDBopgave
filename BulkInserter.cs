using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave
{
    public class BulkInserter : IInserter
    {
        public void InsertTitles(List<Title_Model> titles, SqlConnection sqlConn)
        {
            
        }
    }
}
