using System.Collections.Generic;

namespace BookInfoImporter
{
    internal interface BookParser
    {
        Book? ParseSingleBook();
        List<Book> ParseAllBooks(List<string> invalidLines);
    }
}
