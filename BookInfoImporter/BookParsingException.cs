using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookInfoImporter
{
    internal class BookParsingException : Exception
    {
        public string line { get; set; } = "";
        public BookParsingException(string message, string line, Exception e) : base(message, e)
        {
            this.line = line;
        }
    }
}
