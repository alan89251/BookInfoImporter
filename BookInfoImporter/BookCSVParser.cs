using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace BookInfoImporter
{
    internal class BookCSVParser : BookParser
    {
        private StreamReader file;

        public BookCSVParser(StreamReader file) {
            try
            {
                this.file = file;
                // Skip the titles
                this.file.ReadLine();
            }
            catch(Exception e)
            {
                throw new Exception("Error in opening the csv file.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>null if reach EOF</returns>
        /// <exception cref="BookParsingException"></exception>
        public Book? ParseSingleBook()
        {
            string? line = null;
            try
            {
                line = file.ReadLine();
                if (line == null)
                {
                    return null;
                }

                string[] fields = line.Split(",");
                Book book = new Book();
                book.bookID = Convert.ToInt32(fields[0]);
                book.title = fields[1];
                book.authors = fields[2];
                book.average_rating = Convert.ToDouble(fields[3]);
                book.isbn = fields[4];
                book.isbn13 = fields[5];
                book.language_code = fields[6];
                book.num_pages = Convert.ToInt32(fields[7]);
                book.ratings_count = Convert.ToInt32(fields[8]);
                book.text_reviews_count = Convert.ToInt32(fields[9]);
                book.publication_date = DateTime.ParseExact(
                    fields[10], 
                    new string[] { "MM/dd/yyyy", "M/dd/yyyy" , "MM/d/yyyy", "M/d/yyyy"},
                    CultureInfo.InvariantCulture);
                book.publisher = fields[11];
                return book;
            }
            catch (Exception e)
            {
                throw new BookParsingException($"Error in parsing a line in the csv file.", line, e);
            }
        }

        public List<Book> ParseAllBooks(List<string> invalidLines)
        {
            List<Book> books = new List<Book>();
            Book? book = null;
            while (true)
            {
                try
                {
                    book = ParseSingleBook();
                    if (book != null)
                    {
                        books.Add(book);
                    }
                    else
                    {
                        break;
                    }
                }
                catch (BookParsingException e)
                {
                    invalidLines.Add(e.line);
                }
            }
            return books;
        }
    }
}
