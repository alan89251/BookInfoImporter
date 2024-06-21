using System.Collections.Generic;

interface BookParser
{
    Book? ParseSingleBook();
    List<Book> ParseAllBooks(List<string> invalidLines);
}