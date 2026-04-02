using IMDBopgave;
using IMDBopgave.Inserters;
using IMDBopgave.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBopgave.DataInserting
{
    public static class BulkNameInsert
    {
        public static void BulkInsertingName()
        {
            List<Name_Model> names = new List<Name_Model>();

            BulkInserterName inserter = new BulkInserterName();

            SqlConnection sqlConn = new SqlConnection(
            "Server=localhost;Database=MovieDB;Integrated security=True;" +
            "Trusted_Connection=True;TrustServerCertificate=True;");
            sqlConn.Open();

            var allLines = File.ReadLines("C:/Users/marti/OneDrive/Skrivebord/SQL-databaser/name.basics.tsv");

            foreach (string name in allLines.Skip(1))
            {
                string[] parts = name.Split('\t');
                if (parts.Length == 6)
                {
                    names.Add(new Name_Model(parts));
                }
                else
                {
                    Console.WriteLine("Invalid line: " + name);
                }

                if (names.Count >= 1000000)
                {
                    inserter.InsertNames(names, sqlConn);
                    names.Clear();
                }
            }

            if (names.Count > 0)
            {
                inserter.InsertNames(names, sqlConn);
            }

            sqlConn.Close();
        }
    }
}
