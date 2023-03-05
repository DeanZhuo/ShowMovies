namespace ShowMovies.Models
{
    public class MovieResult
    {
        public int page { get; set; }
        public Movie[] results { get; set; }
        public int total_results { get; set; }
        public int total_pages { get; set; }
    }

    public class GenreResult
    {
        public Genre[] genres { get; set; }
    }

    public class ReviewResult
    {
        public int id { get; set; }
        public int page { get; set; }
        public UserReviews[] results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
}