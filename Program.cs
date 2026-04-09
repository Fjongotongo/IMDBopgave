using IMDBopgave;
using IMDBopgave.DataInserting;
using IMDBopgave.Inserters;
using IMDBopgave.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

class Program
{
    // Husk at rette din Connection String så den passer til din SQL Server
    static string connString = ("Server=localhost;Database=MovieDB;Integrated security=True;" +
            "Trusted_Connection=True;TrustServerCertificate=True;");

    static void Main(string[] args)
    {
        Console.WriteLine("--- IMDB System Aktiveret ---");

        // 1. Eksempel: Søg efter film (SearchMovieTitle)
        SearchMovie("Min SSeje");

        // 2. Eksempel: Tilføj en person (AddPerson)
        AddPersonToDb("Mads Mikkelsen", 1965, null, "actor", "1,2");

        Console.WriteLine("\nTryk på en tast for at lukke...");
        Console.ReadKey();
    }

    // METODE TIL AT SØGE OG VISE RESULTATER
    static void SearchMovie(string fragment)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            // Navnet på din procedure
            SqlCommand cmd = new SqlCommand("SearchMovieTitle", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Tilføj parameteren
            cmd.Parameters.AddWithValue("@TitleFragment", fragment);

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine($"\nSøgeresultater for: {fragment}");
                Console.WriteLine("--------------------------------------");

                if (!reader.HasRows) Console.WriteLine("Ingen film fundet.");

                while (reader.Read())
                {
                    // Her henter vi kolonnerne. Du kan bruge navne eller index.
                    string title = reader["PrimaryTitle"].ToString();
                    string genre = reader["Genre"].ToString();
                    string year = reader["StartYear"].ToString();

                    Console.WriteLine($"- {title} ({year}) | Genre: {genre}");
                }
            }
        }
    }

    // METODE TIL AT TILFØJE (Add/Update/Delete)
    static void AddPersonToDb(string name, int? birth, int? death, string professions, string titles)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            SqlCommand cmd = new SqlCommand("AddPerson", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Parametre - Husk DBNull.Value hvis noget er null!
            cmd.Parameters.AddWithValue("@PrimaryName", name);
            cmd.Parameters.AddWithValue("@BirthYear", (object)birth ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DeathYear", (object)death ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Professions", (object)professions ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@KnownForTitles", (object)titles ?? DBNull.Value);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
                Console.WriteLine($"\nSucces! {name} er nu tilføjet til databasen.");
        }
    }
}

//Console.WriteLine("IMDB Import");

//BulkTitleDataInsert.BulkInsertingTitle();
//BulkGenreDataInsert.InsertingGenres();
//BulkTitleGenreDataInsert.TitleGenreInserting();
//BulkNameDataInsert.BulkInsertingName();
//BulkPrimaryProfessionDataInsert.InsertingPrimaryProfessions();
//BulkNameTitleDataInsert.NameTitleInserting();
//BulkNamePrimaryProfessionDataInsert.NameProfessionInserting();
//BulkTitleCrewDataInsert.TitleCrewInserting();

//Stopwatch stopwatch = Stopwatch.StartNew();

////foreach (Title_Model movie in movies)
////{
////    Console.WriteLine(movie);
////}

//stopwatch.Stop();
//Console.WriteLine("Elapsed milliseconds to read from file: " + stopwatch.ElapsedMilliseconds);

//stopwatch.Start();

////IInserter inserter = new NormalInserter();
////IInserter inserter = new PreparedInserter();
//IInserter inserter = new BulkInserter();

//SqlConnection sqlConn = new SqlConnection(
//    "Server=localhost;Database=MovieDB;Integrated security=True;" +
//    "Trusted_Connection=True;TrustServerCertificate=True;");
//sqlConn.Open();
//inserter.InsertTitles(movies, sqlConn);

//sqlConn.Close();
//stopwatch.Stop(); // 3800 milconds for 10000 records, 4.4 seconds
//// 2800 for prepared
//Console.WriteLine("Elapsed milliseconds to Insert Data: " + stopwatch.ElapsedMilliseconds);




