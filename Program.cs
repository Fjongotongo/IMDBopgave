using IMDBopgave;
using IMDBopgave.DataInserting;
using IMDBopgave.Inserters;
using IMDBopgave.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

class Program
{
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

                    Console.Write("Kendt for film (TConst ID'er med komma): ");
                    string titler = Console.ReadLine();

                    AddPersonToDb(navn, fødselsÅr, dødsÅr, profs, titler);
                    break;

                case "0":
                    kører = false;
                    Console.WriteLine("Programmet afsluttes...");
                    break;

                default:
                    Console.WriteLine("Ugyldigt valg, prøv igen.");
                    break;
            }

            if (kører)
            {
                Console.WriteLine("\nTryk på en tast for at vende tilbage til menuen...");
                Console.ReadKey();
            }
        }
    }

    // --- DINE METODER ---

    static void SearchMovie(string fragment)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            SqlCommand cmd = new SqlCommand("SearchMovieTitle", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 120;
            cmd.Parameters.AddWithValue("@TitleFragment", fragment);
        Console.WriteLine("/help for commands");



        switch (Console.ReadLine())
        {
            case "/help":
                Console.WriteLine("Tilgængelige kommandoer:");
                Console.WriteLine("/search - Søg efter film");
                Console.WriteLine("/addperson - Tilføj en person");
                // Tilføj flere kommandoer her
                break;
            case "/search":
                Console.WriteLine("Search for title");
                string titleFragment = Console.ReadLine();

                // 1. Eksempel: Søg efter film (SearchMovieTitle)
                SearchMovie(titleFragment);
                break;

            //case "/addperson":
            //    Console.WriteLine("Add person in the format");

            //    Console.WriteLine("Enter name");
            //    string nameInput = Console.ReadLine();

            //    Console.WriteLine("Enter birthinput (press enter to skip)");
            //    int? birthInput = Console.ReadLine();

            //    if (string.IsNullOrWhiteSpace(birthInput)) { birthInput = null; }

            //    Console.WriteLine("Enter deathinput (press enter to skip)");
            //    string? deathInput = Console.ReadLine();

            //    if (string.IsNullOrWhiteSpace(deathInput)) { deathInput = null; }

            //    Console.WriteLine("Enter professions (comma separated)");
            //    string professionsInput = Console.ReadLine();

            //    Console.WriteLine("Enter known for titles (comma separated)");
            //    string knownForTitlesInput = Console.ReadLine();

            //    AddPersonToDb(nameInput, birthInput, deathInput, professionsInput, knownForTitlesInput);
            //    break;
            default:
                Console.WriteLine("Command not available, /help for commands");
                break;


                //Console.WriteLine("\nTryk på en tast for at lukke...");
                //Console.ReadKey();
        }

        // METODE TIL AT SØGE OG VISE RESULTATER
        static void SearchMovie(string fragment)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                Console.WriteLine($"\n--- RESULTATER FOR: {fragment} ---");

                if (!reader.HasRows)
                {
                    Console.WriteLine("Ingen film fundet.");
                }
                // Navnet på din procedure
                SqlCommand cmd = new SqlCommand("SearchMovieTitle", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Tilføj parameteren
                cmd.Parameters.AddWithValue("@TitleFragment", fragment);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Her henter vi ALLE kolonnerne fra Titles.* og Genres.Genre
                    string tconst = reader["TConst"].ToString();
                    string type = reader["TitleType"].ToString();
                    string primary = reader["PrimaryTitle"].ToString();
                    string original = reader["OriginalTitle"].ToString();
                    string adult = (bool)reader["IsAdult"] ? "Ja" : "Nej";
                    string startYear = reader["StartYear"].ToString();
                    string endYear = reader["EndYear"].ToString();
                    string runtime = reader["RuntimeMinutes"].ToString();
                    string genre = reader["Genre"].ToString(); // Den enkelte genre for denne række

                    // Vi laver en pæn blok for hver række/genre
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine($"> ID:       tt{tconst.PadLeft(7, '0')}"); // Formaterer ID pænt
                    Console.WriteLine($"> TITEL:    {primary} ({startYear})");
                    Console.WriteLine($"> GENRE:    {genre.ToUpper()}"); // Viser den specifikke genre
                    Console.WriteLine($"> TYPE:     {type} | Runtime: {runtime} min.");
                    Console.WriteLine($"> ORIGINAL: {original}");
                    Console.WriteLine($"> VOKSEN:   {adult}");

                    if (!string.IsNullOrEmpty(endYear))
                        Console.WriteLine($"> SLUT-ÅR:  {endYear}");
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

    static void AddPersonToDb(string name, int? birth, int? death, string professions, string titles)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        // METODE TIL AT TILFØJE (Add/Update/Delete)
        static void AddPersonToDb(string name, int? birth, int? death, string professions, string titles)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand("AddPerson", conn);
                cmd.CommandType = CommandType.StoredProcedure;

            // Her håndterer vi både null fra C# og tomme strenge fra konsollen
            cmd.Parameters.AddWithValue("@PrimaryName", name);
            cmd.Parameters.AddWithValue("@BirthYear", (object)birth ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DeathYear", (object)death ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Professions", string.IsNullOrEmpty(professions) ? DBNull.Value : professions);
            cmd.Parameters.AddWithValue("@KnownForTitles", string.IsNullOrEmpty(titles) ? DBNull.Value : titles);
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




