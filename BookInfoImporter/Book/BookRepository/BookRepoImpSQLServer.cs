using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

class BookRepoImpSQLServer : BookRepository
{
    private string connectionString;

    public BookRepoImpSQLServer(string connectionString)
    {
        this.connectionString = connectionString;
    }

    private DataTable CreateDataTable()
    {
        DataTable dataTable = new DataTable("books");
        DataColumn bookID = new DataColumn("book_id", typeof(int));
        dataTable.Columns.Add(bookID);
        dataTable.Columns.Add(new DataColumn("title", typeof(string)));
        dataTable.Columns.Add(new DataColumn("authors", typeof(string)));
        dataTable.Columns.Add(new DataColumn("average_rating", typeof(double)));
        dataTable.Columns.Add(new DataColumn("isbn", typeof(string)));
        dataTable.Columns.Add(new DataColumn("isbn13", typeof(string)));
        dataTable.Columns.Add(new DataColumn("language_code", typeof(string)));
        dataTable.Columns.Add(new DataColumn("num_pages", typeof(int)));
        dataTable.Columns.Add(new DataColumn("ratings_count", typeof(int)));
        dataTable.Columns.Add(new DataColumn("text_reviews_count", typeof(int)));
        dataTable.Columns.Add(new DataColumn("publication_date", typeof(DateTime)));
        dataTable.Columns.Add(new DataColumn("publisher", typeof(string)));
        dataTable.PrimaryKey = new DataColumn[] { bookID };
        return dataTable;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="books">source</param>
    /// <param name="dataTable">destination</param>
    /// <param name="failedInserts"></param>
    private void InsertToDataTable(List<Book> books, DataTable dataTable, List<BookRepositoryFailedOperation> failedInserts)
    {
        foreach (Book book in books)
        {
            try
            {
                DataRow row = dataTable.NewRow();
                row["book_id"] = book.book_id;
                row["title"] = book.title;
                row["authors"] = book.authors;
                row["average_rating"] = book.average_rating;
                row["isbn"] = book.isbn;
                row["isbn13"] = book.isbn13;
                row["language_code"] = book.language_code;
                row["num_pages"] = book.num_pages;
                row["ratings_count"] = book.ratings_count;
                row["text_reviews_count"] = book.text_reviews_count;
                row["publication_date"] = book.publication_date;
                row["publisher"] = book.publisher;
                dataTable.Rows.Add(row);
            }
            catch (Exception e)
            {
                failedInserts.Add(new BookRepositoryFailedOperation($"Failed to insert record {book.book_id}. Reason: {e.Message}", book));
            }
        }
    }

    private List<int> QueryIDsOfRecordsAlreadyExist(List<Book> books)
    {
        string ids = "";
        foreach (Book book in books)
        {
            ids += $"{book.book_id},";
        }
        ids = ids.TrimEnd(',');
        string sql = $"SELECT book_id FROM Books WHERE book_id in ({ids});";
        List<int> bookIds = new List<int>();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int bookId = reader.GetInt32(0);
                        bookIds.Add(bookId);
                    }
                }
            }
        }
        return bookIds;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="books"></param>
    /// <param name="failedInserts">Info of failed insertions of book record</param>
    public void InsertAll(List<Book> books, List<BookRepositoryFailedOperation> failedInserts)
    {
        // Ignore the book records that already exist in the database to prevent bulk insert failure
        List<int> idsAlreadyExistInDB = QueryIDsOfRecordsAlreadyExist(books);
        List<Book> booksToInsert = new List<Book>();
        foreach (Book book in books)
        {
            if (!idsAlreadyExistInDB.Contains(book.book_id))
            {
                booksToInsert.Add(book);
            }
            else
            {
                failedInserts.Add(new BookRepositoryFailedOperation($"{book.book_id} is dulplicate in the DB", book));
            }
        }

        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
        {
            DataTable dataTable = CreateDataTable();
            InsertToDataTable(booksToInsert, dataTable, failedInserts);
            bulkCopy.DestinationTableName = "books";
            bulkCopy.ColumnMappings.Add(nameof(Book.book_id), "book_id");
            bulkCopy.ColumnMappings.Add(nameof(Book.title), "title");
            bulkCopy.ColumnMappings.Add(nameof(Book.authors), "authors");
            bulkCopy.ColumnMappings.Add(nameof(Book.average_rating), "average_rating");
            bulkCopy.ColumnMappings.Add(nameof(Book.isbn), "isbn");
            bulkCopy.ColumnMappings.Add(nameof(Book.isbn13), "isbn13");
            bulkCopy.ColumnMappings.Add(nameof(Book.language_code), "language_code");
            bulkCopy.ColumnMappings.Add(nameof(Book.num_pages), "num_pages");
            bulkCopy.ColumnMappings.Add(nameof(Book.ratings_count), "ratings_count");
            bulkCopy.ColumnMappings.Add(nameof(Book.text_reviews_count), "text_reviews_count");
            bulkCopy.ColumnMappings.Add(nameof(Book.publication_date), "publication_date");
            bulkCopy.ColumnMappings.Add(nameof(Book.publisher), "publisher");

            bulkCopy.WriteToServer(dataTable);
        }
    }

    public List<Book> QueryTop100MostRecentlyPublishedBooksReport()
    {
        const string sql = "WITH MOST_RECENT_BOOK AS (SELECT TOP 100 * FROM Books ORDER BY publication_date DESC) SELECT * FROM MOST_RECENT_BOOK ORDER BY publication_date DESC, publisher ASC;";
        List<Book> result = new List<Book>();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Book book = new Book();
                        book.book_id = reader.GetInt32(0);
                        book.title = reader.GetString(1);
                        book.authors = reader.GetString(2);
                        book.average_rating = reader.GetDouble(3);
                        book.isbn = reader.GetString(4);
                        book.isbn13 = reader.GetString(5);
                        book.language_code = reader.GetString(6);
                        book.num_pages = reader.GetInt32(7);
                        book.ratings_count = reader.GetInt32(8);
                        book.text_reviews_count = reader.GetInt32(9);
                        book.publication_date = reader.GetDateTime(10);
                        book.publisher = reader.GetString(11);
                        result.Add(book);
                    }
                }
            }
        }
        return result;
    }
}
