using System.Collections.Generic; 
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace BookInfoImporter
{
    public class Program
    {
        private static string RecordsImportReportName = "RecordsImportReport.txt";
        private static string Top100MostRecentlyPublishedBooksReportName = "Top100MostRecentlyPublishedBooksReport.csv";
        private static string connectionString = "";
        private static ILogger logger;
        private static List<Book> books;
        private static List<Book> top100MostRecentlyPublishedBooks;
        private static List<string> invalidLines = new List<string>();
        private static List<BookRepositoryFailedOperation> bookRepositoryFailedOperation = new List<BookRepositoryFailedOperation>();
        private static BookRepository bookRepo;

        public static void Main(string[] args)
        {
            Init();
            ReadBooks(args[0]);
            InsertDB();
            GenRecordsImportReport();
            QueryTop100MostRecentlyPublishedBooksReport();
            GenTop100MostRecentlyPublishedBooksReport();
            logger.LogInformation($"Generated 2 reports:\n\t{RecordsImportReportName}\n\t{Top100MostRecentlyPublishedBooksReportName}");
        }

        private static void ReadConfig()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);
            var config = builder.Build();
            connectionString = config["ConnectionString"];
            if (!string.IsNullOrEmpty(config["RecordsImportReportName"]))
                RecordsImportReportName = config["RecordsImportReportName"];
            if (!string.IsNullOrEmpty(config["Top100MostRecentlyPublishedBooksReportName"]))
                Top100MostRecentlyPublishedBooksReportName = config["Top100MostRecentlyPublishedBooksReportName"];
        }
        private static void Init()
        {
            ReadConfig();
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            logger = factory.CreateLogger("BookInfoImporter");
            bookRepo = new BookRepoImpSQLServer(connectionString);
        }

        private static void ReadBooks(string fileName)
        {
            try
            {
                logger.LogInformation($"Start importing from {fileName}.");
                using (StreamReader sr = new StreamReader(fileName))
                {
                    BookParser bookCSVParser = new BookCSVParser(sr);
                    books = bookCSVParser.ParseAllBooks(invalidLines);
                }
                logger.LogInformation($"Imported {books.Count} records.");
                if (invalidLines.Count > 0)
                    logger.LogError($"Skip {invalidLines.Count} invalid lines. Please check the report for details.");
            }
            catch (Exception e)
            {
                logger.LogError($"Error in reading {fileName}. Reason: {e.Message}");
                Environment.Exit(1);
            }
            
        }

        private static void InsertDB()
        {
            try
            {
                logger.LogInformation("Start inserting to database.");
                bookRepo.InsertAll(books, bookRepositoryFailedOperation);
                logger.LogInformation($"Inserted {books.Count - bookRepositoryFailedOperation.Count} records to database.");
                if (bookRepositoryFailedOperation.Count > 0)
                    logger.LogError($"Fail to insert {bookRepositoryFailedOperation.Count} records. Please check the report for details.");
            }
            catch (Exception e)
            {
                logger.LogError($"Error in inserting records to database. Reason: {e.Message}");
                Environment.Exit(1);
            }
        }

        private static void QueryTop100MostRecentlyPublishedBooksReport()
        {
            try
            {
                logger.LogInformation("Start querying top 100 most recently published books.");
                top100MostRecentlyPublishedBooks = bookRepo.QueryTop100MostRecentlyPublishedBooksReport();
                logger.LogInformation("Finished querying top 100 most recently published books.");
            }
            catch (Exception e)
            {
                logger.LogError($"Error in querying the top 100 most recently published books from database. Reason: {e.Message}");
                Environment.Exit(1);
            }
        }

        private static void GenTop100MostRecentlyPublishedBooksReport()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Top100MostRecentlyPublishedBooksReportName))
                {
                    writer.WriteLine("bookID,title,authors,average_rating,isbn,isbn13,language_code,num_pages,ratings_count,text_reviews_count,publication_date,publisher");
                    foreach (Book book in top100MostRecentlyPublishedBooks)
                    {
                        writer.WriteLine($"{book.book_id}," +
                            $"{book.title}," +
                            $"{book.authors}," +
                            $"{book.average_rating}," +
                            $"{book.isbn}," +
                            $"{book.isbn13}," +
                            $"{book.language_code}," +
                            $"{book.num_pages}," +
                            $"{book.ratings_count}," +
                            $"{book.text_reviews_count}," +
                            $"{book.publication_date.ToShortDateString()}," +
                            $"{book.publisher}");
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Error in generating the report of the top 100 most recently published books. Reason: {e.Message}");
                Environment.Exit(1);
            }
        }

        private static void GenRecordsImportReport()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(RecordsImportReportName))
                {
                    writer.WriteLine($"Inserted {books.Count - bookRepositoryFailedOperation.Count} records to database.");
                    writer.WriteLine($"Skip {invalidLines.Count} invalid lines from the CSV file.");
                    writer.WriteLine($"Failed to insert {bookRepositoryFailedOperation.Count} records.");
                    if (invalidLines.Count > 0)
                    {
                        writer.WriteLine("List of the invalid lines:");
                        foreach (string line in invalidLines)
                        {
                            writer.WriteLine(line);
                            writer.WriteLine("-------------------------------------------------------------");
                        }
                    }
                    if (bookRepositoryFailedOperation.Count > 0)
                    {
                        writer.WriteLine("List of id of the records which were failed to insert to database:");
                        foreach (BookRepositoryFailedOperation e in bookRepositoryFailedOperation)
                        {
                            writer.WriteLine(e.book.book_id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Error in generating the report of the book records import. Reason: {e.Message}");
                Environment.Exit(1);
            }
        }
    }
}