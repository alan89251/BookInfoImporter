namespace BookInfoImporter
{
    internal class BookRepositoryFailedOperation
    {
        public Book book { get; set; }
        public string message { get; set; }

        public BookRepositoryFailedOperation(string message, Book book)
        {
            this.message = message;
            this.book = book;
        }
    }
}
