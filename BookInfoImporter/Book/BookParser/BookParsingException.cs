using System.Collections.Generic;

class BookParsingException : Exception
{
    public string line { get; set; } = "";
    public BookParsingException(string message, string line, Exception e) : base(message, e)
    {
        this.line = line;
    }
}
