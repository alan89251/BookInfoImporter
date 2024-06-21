namespace BookInfoImporter
{
    internal class Book
    {
        public int bookID { get; set; } = 0;
        public string title { get; set; } = "";
        public string authors { get; set; } = "";
        public double average_rating { get; set; } = 0;
        public string isbn { get; set; } = "";
        public string isbn13 { get; set; } = "";
        public string language_code { get; set; } = "";
        public int num_pages { get; set; } = 0;
        public int ratings_count { get; set; } = 0;
        public int text_reviews_count { get; set; } = 0;
        public DateTime publication_date { get; set; }
        public string publisher { get; set; } = "";
    }
}
