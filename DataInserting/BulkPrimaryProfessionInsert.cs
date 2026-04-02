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
    public class BulkPrimaryProfessionInsert
    {
        public static void InsertingPrimaryProfessions()
        {
            BulkInserter_PrimaryProfession PPInserter = new BulkInserter_PrimaryProfession();
            List<Primary_Profession_Model> PPS = new List<Primary_Profession_Model>();

            SqlConnection sqlConn = new SqlConnection(
                "Server=localhost;Database=MovieDB;Integrated security=True;" +
                "Trusted_Connection=True;TrustServerCertificate=True;");
            sqlConn.Open();

            var allLines = File.ReadLines("C:\\Users\\leo\\Downloads\\name.basics.tsv\\name.basics.tsv");

            HashSet<string> uniquePP = new HashSet<string>();

            foreach (string pp in allLines.Skip(1))
            {
                string[] parts = pp.Split('\t');

                if (parts.Length == 6)
                {
                    string PPString = parts[4];

                    if (PPString != "\\N")
                    {
                        string[] individualPPS = PPString.Split(',');

                        foreach (string p in individualPPS)
                        {
                            uniquePP.Add(p.Trim());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid line: " + pp);
                }
            }

            PPInserter.InsertPP(uniquePP, sqlConn);

            sqlConn.Close();
        }
    }
}
