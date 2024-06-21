using System.Collections.Generic;

namespace BookInfoImporter
{
    internal interface BookRepository
    {
        void InsertAll(List<Book> books, List<BookRepositoryFailedOperation> failedInserts);
        List<Book> QueryTop100MostRecentlyPublishedBooksReport();
    }
}
