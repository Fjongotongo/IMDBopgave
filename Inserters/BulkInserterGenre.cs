using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace IMDBopgave.Inserters
{
    public class BulkInserterGenre
    {
        // Vi ændrer inputtet til at tage imod vores HashSet
        public void InsertGenres(HashSet<string> genres, SqlConnection sqlConn)
        {
            DataTable genreTable = new DataTable();
            genreTable.Columns.Add("Genre", typeof(string));

            // Nu behøver vi slet ikke at tjekke for dubletter, 
            // for HashSet'et har allerede sorteret dem fra!
            foreach (string genre in genres)
            {
                genreTable.Rows.Add(genre);
            }

            // Brug "using" for at sikre at SqlBulkCopy rydder pænt op efter sig
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn))
            {
                bulkCopy.DestinationTableName = "Genres";
                bulkCopy.WriteToServer(genreTable);
            }
        }
    }
}