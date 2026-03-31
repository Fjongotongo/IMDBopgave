using IMDBopgave;
using IMDBopgave.Inserters;
using IMDBopgave.Models;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

Console.WriteLine("IMDB Import");

//Stopwatch stopwatch = Stopwatch.StartNew();
List<Title_Model> movies = new List<Title_Model>();

IInserter inserter = new BulkInserter();

BulkInserterGenre genreInserter = new BulkInserterGenre();
List<Genre_Model> genres = new List<Genre_Model>();

SqlConnection sqlConn = new SqlConnection(
    "Server=localhost;Database=MovieDB;Integrated security=True;" +
    "Trusted_Connection=True;TrustServerCertificate=True;");
sqlConn.Open();


var allLines = File.ReadLines("C:/Users/leo/Downloads/title.basics.tsv/title.basics.tsv").ToArray();

foreach (string movie in allLines.Skip(1))
{
    string[] parts = movie.Split('\t');
    if (parts.Length == 9)
    {
        string[] genresInRow = movie.Split(",");

        genres.Add(new Genre_Model(parts[8]));
    }
    else
    {
        Console.WriteLine("Invalid line: " + movie);
    }
    
}

//foreach (string movie in allLines.Skip(1))
//{
//    string[] parts = movie.Split('\t');
//    if (parts.Length == 9)
//    {
//        movies.Add(new Title_Model(parts));
//    }
//    else
//    {
//        Console.WriteLine("Invalid line: " + movie);
//    }

//    if (movies.Count >= 1000000)
//    {
//        inserter.InsertTitles(movies, sqlConn);
//        movies.Clear();
//    }
//}

//if (movies.Count > 0) 
//{
//    inserter.InsertTitles(movies, sqlConn);
//}


sqlConn.Close();

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