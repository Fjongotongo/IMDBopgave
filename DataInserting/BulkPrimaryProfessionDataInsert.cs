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
    public class BulkPrimaryProfessionDataInsert
    {
        public static void InsertingPrimaryProfessions()
        {
            BulkInserterPrimaryProfession PrimaryProfessionInserter = new BulkInserterPrimaryProfession();
            List<Primary_Profession_Model> PrimaryProfessions = new List<Primary_Profession_Model>();

            SqlConnection sqlConn = new SqlConnection(
                "Server=localhost;Database=MovieDB;Integrated security=True;" +
                "Trusted_Connection=True;TrustServerCertificate=True;");
            sqlConn.Open();

            // Name.basics.tsv
            var allLines = File.ReadLines("C:\\Users\\leo\\Downloads\\name.basics.tsv\\name.basics.tsv");

            HashSet<string> uniquePrimaryProfession = new HashSet<string>();

            foreach (string primaryProfession in allLines.Skip(1))
            {
                string[] parts = primaryProfession.Split('\t');

                if (parts.Length == 6)
                {
                    string primaryProfessionString = parts[4];

                    if (primaryProfessionString != "\\N")
                    {
                        string[] individualPrimaryProfessions = primaryProfessionString.Split(',');

                        foreach (string p in individualPrimaryProfessions)
                        {
                            uniquePrimaryProfession.Add(p.Trim());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid line: " + primaryProfession);
                }
            }

            PrimaryProfessionInserter.InsertPP(uniquePrimaryProfession, sqlConn);

            sqlConn.Close();
        }
    }
}
