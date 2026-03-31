using IMDBopgave.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave
{
    public interface IInserter
    {
        void InsertTitles(List<Title_Model> titles, SqlConnection sqlConn);
    }
}
