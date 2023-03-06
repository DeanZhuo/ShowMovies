using ShowMovies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowMovies.Services
{
    public interface IDataStore
    {
        List<Genre> Genres { get; set; }
        Task<List<Genre>> GetItemAsync();
        Task<Movie> GetMovie(int id);
        Task<string> GetTrailerUrl(int id);
        Task<ReviewResult> GetMovieReviewsAsync(int id, int page);
        Task<MovieResult> GetMovieAsync(string searchkey, int page);
        Task<MovieResult> GetMovieByGenreAsync(int genrekey, int page);
    }
}