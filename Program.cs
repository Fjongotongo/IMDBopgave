using IMDBopgave;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

Console.WriteLine("IMDB Import");

Stopwatch stopwatch = Stopwatch.StartNew();
List<Title_Model> movies = new List<Title_Model>();

foreach (string movie in File.ReadLines("").Skip(1).Take(10000))
{
    string[] parts = movie.Split('\t');
    if (parts.Length == 9)
    {
        movies.Add(new Title_Model(parts));
    }
    else
    {
        Console.WriteLine("Invalid line: " + movie);
    }
}

//foreach (Title_Model movie in movies)
//{
//    Console.WriteLine(movie);
//}

stopwatch.Stop();
Console.WriteLine("Elapsed milliseconds to read from file: " + stopwatch.ElapsedMilliseconds);

stopwatch.Start();

//IInserter inserter = new NormalInserter();
//IInserter inserter = new PreparedInserter();
IInserter inserter = new BulkInserter();

SqlConnection sqlConn = new SqlConnection(
    "Server=localhost;Database=MovieDB;Integrated security=True;" +
    "Trusted_Connection=True;TrustServerCertificate=True;");
sqlConn.Open();
inserter.InsertTitles(movies, sqlConn);

sqlConn.Close();
stopwatch.Stop(); // 3800 milconds for 10000 records, 4.4 seconds
// 2800 for prepared
Console.WriteLine("Elapsed milliseconds to Insert Data: " + stopwatch.ElapsedMilliseconds);