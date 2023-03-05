using System;

namespace ShowMovies.Models
{
    public class MovieReview
    {
        public int id { get; set; }
        public int page { get; set; }
        public Reviews[] results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }

    public class Reviews
    {
        public string author { get; set; }
        public Author_Details author_details { get; set; }
        public string content { get; set; }
        public DateTime created_at { get; set; }
        public string id { get; set; }
        public DateTime updated_at { get; set; }
        public string url { get; set; }
    }

    public class Author_Details
    {
        public string name { get; set; }
        public string username { get; set; }
        public string avatar_path { get; set; }
        public int? rating { get; set; }
    }
}