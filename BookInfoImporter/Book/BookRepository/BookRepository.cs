using System.Collections.Generic;

interface BookRepository
{
    void InsertAll(List<Book> books, List<BookRepositoryFailedOperation> failedInserts);
    List<Book> QueryTop100MostRecentlyPublishedBooksReport();
}
