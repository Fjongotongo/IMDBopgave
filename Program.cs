using IMDBopgave;
using IMDBopgave.DataInserting;
using IMDBopgave.Inserters;
using IMDBopgave.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

Console.WriteLine("IMDB Import");

//BulkTitleDataInsert.BulkInsertingTitle();
//BulkGenreDataInsert.InsertingGenres();
//BulkTitleGenreDataInsert.TitleGenreInserting();
//BulkNameDataInsert.BulkInsertingName();
//BulkPrimaryProfessionDataInsert.InsertingPrimaryProfessions();
//BulkNameTitleDataInsert.NameTitleInserting();
//BulkNamePrimaryProfessionDataInsert.NameProfessionInserting();
BulkTitleCrewDataInsert.TitleCrewInserting();

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