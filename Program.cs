using IMDBopgave;
using IMDBopgave.DataInserting;
using IMDBopgave.Inserters;
using IMDBopgave.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System;

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
            Console.WriteLine("2. Søg efter person");
            Console.WriteLine("3. Tilføj en person");
            Console.WriteLine("4. Tilføj en film");
            Console.WriteLine("5. Opdatér en film"); 
            Console.WriteLine("0. Afslut");
            Console.Write("\nVælg en handling: ");

            string valg = Console.ReadLine();

            switch (valg)
            {
                case "1":
                    Console.Write("Indtast filmtitel du vil søge efter: ");
                    string movieSearch = Console.ReadLine();
                    SearchMovie(movieSearch);
                    break;

                case "2":
                    Console.Write("Indtast navnet på personen du vil søge efter: ");
                    string personSearch = Console.ReadLine();
                    SearchPerson(personSearch);
                    break;

                case "3":
                    Console.WriteLine("\n--- Tilføj ny person ---");
                    Console.Write("Navn: ");
                    string navn = Console.ReadLine();

                    Console.Write("Fødselsår (tryk enter hvis ukendt): ");
                    int? fødselsÅr = int.TryParse(Console.ReadLine(), out int bYear) ? bYear : null;

                    Console.Write("Dødsår (tryk enter hvis stadig i live): ");
                    int? dødsÅr = int.TryParse(Console.ReadLine(), out int dYear) ? dYear : null;

                    Console.Write("Professioner (f.eks. actor,producer): ");
                    string profs = Console.ReadLine();

                    Console.Write("Kendt for film (kun numre, f.eks. 72308, 50419): ");
                    string personTitler = Console.ReadLine();

                    AddPersonToDb(navn, fødselsÅr, dødsÅr, profs, personTitler);
                    break;

                case "4":
                    Console.WriteLine("\n--- Tilføj ny film ---");

                    Console.Write("Titel Type (f.eks. movie, short, tvSeries): ");
                    string tType = Console.ReadLine();

                    Console.Write("Primary Title: ");
                    string pTitle = Console.ReadLine();

                    Console.Write("Original Title: ");
                    string oTitle = Console.ReadLine();

                    Console.Write("Er det en voksenfilm (18+)? (Skriv 'ja' eller 'nej'): ");
                    bool isAdult = Console.ReadLine()?.Trim().ToLower() == "ja";

                    Console.Write("Start-år (tryk enter hvis ukendt): ");
                    int? sYear = int.TryParse(Console.ReadLine(), out int sy) ? sy : null;

                    Console.Write("Slut-år (tryk enter hvis ukendt): ");
                    int? eYear = int.TryParse(Console.ReadLine(), out int ey) ? ey : null;

                    Console.Write("Spilletid i minutter (tryk enter hvis ukendt): ");
                    int? runtime = int.TryParse(Console.ReadLine(), out int rt) ? rt : null;

                    Console.Write("Genrer (adskilt af komma, f.eks. Action,Comedy): ");
                    string genres = Console.ReadLine();

                    AddMovieToDb(tType, pTitle, oTitle, isAdult, sYear, eYear, runtime, genres);
                    break;

                case "5":
                    Console.WriteLine("\n--- Opdatér eksisterende film ---");

                    Console.Write("Indtast ID på den film der skal opdateres (KUN TALLET, f.eks. 1 for tt0000001): ");
                    if (!int.TryParse(Console.ReadLine(), out int updateTConst))
                    {
                        Console.WriteLine("Ugyldigt ID. Du skal indtaste et tal. Afbryder...");
                        break;
                    }

                    Console.Write("Ny Titel Type (f.eks. movie, short): ");
                    string upType = Console.ReadLine();

                    Console.Write("Ny Primary Title: ");
                    string upPTitle = Console.ReadLine();

                    Console.Write("Ny Original Title: ");
                    string upOTitle = Console.ReadLine();

                    Console.Write("Er det en voksenfilm (18+)? (Skriv 'ja' eller 'nej'): ");
                    bool upIsAdult = Console.ReadLine()?.Trim().ToLower() == "ja";

                    Console.Write("Nyt Start-år (tryk enter hvis ukendt): ");
                    int? upSYear = int.TryParse(Console.ReadLine(), out int usy) ? usy : null;

                    Console.Write("Nyt Slut-år (tryk enter hvis ukendt): ");
                    int? upEYear = int.TryParse(Console.ReadLine(), out int uey) ? uey : null;

                    Console.Write("Ny Spilletid i min. (tryk enter hvis ukendt): ");
                    int? upRuntime = int.TryParse(Console.ReadLine(), out int urt) ? urt : null;

                    Console.Write("Nye Genrer (adskilt af komma, f.eks. Action,Comedy): ");
                    string upGenres = Console.ReadLine();

                    UpdateMovieInDb(updateTConst, upType, upPTitle, upOTitle, upIsAdult, upSYear, upEYear, upRuntime, upGenres);
                    break;

                case "0":
                    kører = false;
                    Console.WriteLine("Programmet afsluttes...");
                    break;

                default:
                    Console.WriteLine("Ugyldigt valg. Prøv igen.");
                    break;
            }

            if (kører)
            {
                Console.WriteLine("\nTryk på en tast for at vende tilbage til menuen...");
                Console.ReadKey();
            }
        }
    }

    static void SearchMovie(string fragment)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            SqlCommand cmd = new SqlCommand("SearchMovieTitle", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 120;
            cmd.Parameters.AddWithValue("@TitleFragment", fragment);

            try
            {
                conn.Open();
                Console.WriteLine("\nSender forespørgsel til databasen... Vent venligst...");
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine($"\n--- FILMSØGNING FOR: {fragment} ---");

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Ingen film fundet.");
                        return;
                    }

                    while (reader.Read())
                    {
                        string tconst = reader["TConst"].ToString();
                        string type = reader["TitleType"].ToString();
                        string primary = reader["PrimaryTitle"].ToString();
                        string original = reader["OriginalTitle"].ToString();
                        string adult = (bool)reader["IsAdult"] ? "Ja" : "Nej";
                        string startYear = reader["StartYear"].ToString();
                        string endYear = reader["EndYear"].ToString();
                        string runtime = reader["RuntimeMinutes"].ToString();
                        string genre = reader["Genre"].ToString();

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
            catch (Exception ex)
            {
                Console.WriteLine($"\nFEJL UNDER SØGNING: {ex.Message}");
            }
        }
    }

    static void SearchPerson(string fragment)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            SqlCommand cmd = new SqlCommand("SearchName", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 120;
            cmd.Parameters.AddWithValue("@NameFragment", fragment);

            try
            {
                conn.Open();
                Console.WriteLine("\nSender forespørgsel til databasen... Vent venligst...");
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine($"\n--- PERSONSØGNING FOR: {fragment} ---");

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Ingen personer fundet.");
                        return;
                    }

                    while (reader.Read())
                    {
                        string nconst = reader["NConst"].ToString();
                        string name = reader["PrimaryName"].ToString();
                        string birthYear = reader["BirthYear"].ToString();
                        string deathYear = reader["DeathYear"].ToString();

                        string profession = reader["PrimaryProfession"]?.ToString();
                        if (string.IsNullOrEmpty(profession)) profession = "Ukendt profession";

                        string title = reader["PrimaryTitle"]?.ToString();
                        if (string.IsNullOrEmpty(title)) title = "Ingen kendte titler";

                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine($"> ID:           nm{nconst.PadLeft(7, '0')}");
                        Console.WriteLine($"> NAVN:         {name}");

                        if (!string.IsNullOrEmpty(birthYear))
                        {
                            string levetid = string.IsNullOrEmpty(deathYear) ? $"{birthYear} - Nu" : $"{birthYear} - {deathYear}";
                            Console.WriteLine($"> LEVETID:      {levetid}");
                        }

                        Console.WriteLine($"> PROFESSION:   {profession.ToUpper()}");
                        Console.WriteLine($"> KENDT FRA:    {title}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFEJL UNDER PERSONSØGNING: {ex.Message}");
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

            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    Console.WriteLine($"\nSucces! {name} er nu tilføjet til databasen.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFEJL VED OPRETTELSE: {ex.Message}");
            }
        }
    }

    static void AddMovieToDb(string titleType, string primaryTitle, string originalTitle, bool isAdult, int? startYear, int? endYear, int? runtime, string genres)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            SqlCommand cmd = new SqlCommand("AddMovie", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@TitleType", titleType);
            cmd.Parameters.AddWithValue("@PrimaryTitle", primaryTitle);
            cmd.Parameters.AddWithValue("@OriginalTitle", originalTitle);
            cmd.Parameters.AddWithValue("@IsAdult", isAdult);

            cmd.Parameters.AddWithValue("@StartYear", (object)startYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EndYear", (object)endYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RuntimeMinutes", (object)runtime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Genres", string.IsNullOrEmpty(genres) ? DBNull.Value : genres);

            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    Console.WriteLine($"\nSucces! Filmen '{primaryTitle}' er nu tilføjet til databasen.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFEJL VED OPRETTELSE AF FILM: {ex.Message}");
            }
        }
    }

    static void UpdateMovieInDb(int tconst, string titleType, string primaryTitle, string originalTitle, bool isAdult, int? startYear, int? endYear, int? runtime, string genres)
    {
        using (SqlConnection conn = new SqlConnection(connString))
        {
            SqlCommand cmd = new SqlCommand("UpdateMovie", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@TConst", tconst);
            cmd.Parameters.AddWithValue("@TitleType", titleType);
            cmd.Parameters.AddWithValue("@PrimaryTitle", primaryTitle);
            cmd.Parameters.AddWithValue("@OriginalTitle", originalTitle);
            cmd.Parameters.AddWithValue("@IsAdult", isAdult);
            cmd.Parameters.AddWithValue("@StartYear", (object)startYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EndYear", (object)endYear ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RuntimeMinutes", (object)runtime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Genres", string.IsNullOrEmpty(genres) ? DBNull.Value : genres);
            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    Console.WriteLine($"\nSucces! Filmen med ID tt{tconst.ToString().PadLeft(7, '0')} er nu opdateret.");
                else
                    Console.WriteLine($"\nIngen ændringer foretaget. Fandt databasen overhovedet en film med ID'et {tconst}?");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFEJL VED OPDATERING: {ex.Message}");
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




