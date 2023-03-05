using ShowMovies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowMovies.Services
{
    public interface IDataStore
    {
        Task<List<Genre>> GetItemAsync();

        Task<Movie> GetMovie(int id);

        Task<List<Movie>> GetMovieAsync(string searchkey);

        Task<List<UserReviews>> GetMovieReviewsAsync(int id);

        Task<List<Movie>> GetMovieByGenreAsync(string genrekey);
    }
}