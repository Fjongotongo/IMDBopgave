using IMDBopgave;
using IMDBopgave.DataInserting;
using IMDBopgave.Inserters;
using IMDBopgave.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

class Program
{
    // Din Connection String
    static string connString = ("Server=localhost;Database=MovieDB;Integrated security=True;" +
                                "Trusted_Connection=True;TrustServerCertificate=True;");

    static void Main(string[] args)
    {
        bool kører = true;

        while (kører)
        {
            Console.Clear();
            Console.WriteLine("--- IMDB System Aktiveret ---");
            Console.WriteLine("1. Søg efter film");
            Console.WriteLine("2. Tilføj en person");
            Console.WriteLine("0. Afslut");
            Console.Write("\nVælg en handling: ");

            string valg = Console.ReadLine();

            switch (valg)
            {
                case "1":
                    Console.Write("Indtast filmtitel du vil søge efter: ");
                    string søgeord = Console.ReadLine();
                    SearchMovie(søgeord);
                    break;

                case "2":
                    Console.WriteLine("\n--- Tilføj ny person ---");
                    Console.Write("Navn: ");
                    string navn = Console.ReadLine();

                    Console.Write("Fødselsår (tryk enter hvis ukendt): ");
                    string fødselsInput = Console.ReadLine();
                    int? fødselsÅr = string.IsNullOrEmpty(fødselsInput) ? null : int.Parse(fødselsInput);

                    Console.Write("Dødsår (tryk enter hvis stadig i live): ");
                    string dødsInput = Console.ReadLine();
                    int? dødsÅr = string.IsNullOrEmpty(dødsInput) ? null : int.Parse(dødsInput);

                    Console.Write("Professioner (f.eks. actor,producer): ");
                    string profs = Console.ReadLine();

                    Console.Write("Kendt for film (kun numre, f.eks. 72308, 50419): ");
                    string titler = Console.ReadLine();

                    AddPersonToDb(navn, fødselsÅr, dødsÅr, profs, titler);
                    break;

                case "0":
                    kører = false;
                    break;

                default:
                    Console.WriteLine("Ugyldigt valg.");
                    break;
            }

            if (kører)
            {
                Console.WriteLine("\nTryk på en tast for at vende tilbage til menuen...");
                Console.ReadKey();
            }
        }
    }

    // --- METODER ---

    static void SearchMovie(string fragment)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            SqlCommand cmd = new SqlCommand("SearchMovieTitle", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 120; // Giver SQL tid til at lede i de mange millioner film
            cmd.Parameters.AddWithValue("@TitleFragment", fragment);

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine($"\n--- RESULTATER FOR: {fragment} ---");

                if (!reader.HasRows)
                {
                    Console.WriteLine("Ingen film fundet.");
                    return;
                }

                while (reader.Read())
                {
                    // Her henter vi ALLE kolonnerne, så vi viser alt data tilhørende en titel
                    string tconst = reader["TConst"].ToString();
                    string type = reader["TitleType"].ToString();
                    string primary = reader["PrimaryTitle"].ToString();
                    string original = reader["OriginalTitle"].ToString();
                    string adult = (bool)reader["IsAdult"] ? "Ja" : "Nej";
                    string startYear = reader["StartYear"].ToString();
                    string endYear = reader["EndYear"].ToString();
                    string runtime = reader["RuntimeMinutes"].ToString();
                    string genre = reader["Genre"].ToString();

                    // Udskriver hver film/genre kombination med alle detaljer
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine($"> ID:       tt{tconst.PadLeft(7, '0')}");
                    Console.WriteLine($"> TITEL:    {primary} ({startYear})");
                    Console.WriteLine($"> GENRE:    {genre.ToUpper()}");
                    Console.WriteLine($"> TYPE:     {type} | Runtime: {runtime} min.");
                    Console.WriteLine($"> ORIGINAL: {original}");
                    Console.WriteLine($"> VOKSEN:   {adult}");

                    if (!string.IsNullOrEmpty(endYear))
                        Console.WriteLine($"> SLUT-ÅR:  {endYear}");
                }
            }
        }
    }

    static void AddPersonToDb(string name, int? birth, int? death, string professions, string titles)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            SqlCommand cmd = new SqlCommand("AddPerson", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PrimaryName", name);
            cmd.Parameters.AddWithValue("@BirthYear", (object)birth ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DeathYear", (object)death ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Professions", string.IsNullOrEmpty(professions) ? DBNull.Value : professions);
            cmd.Parameters.AddWithValue("@KnownForTitles", string.IsNullOrEmpty(titles) ? DBNull.Value : titles);

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




