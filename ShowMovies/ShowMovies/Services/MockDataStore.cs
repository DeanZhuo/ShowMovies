using ShowMovies.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShowMovies.Services
{
    public class MockDataStore : IDataStore
    {
        private const string APIKEY = "Your API KEY here";

        //url: APIURL + route + APIHEADER + query
        private const string APIURL = "https://api.themoviedb.org/3";

        private const string APIHEADER = "?api_key=" + APIKEY + "&language=en-US";

        private HttpClient client;
        public List<Genre> Genres { get; set; } = new();

        public MockDataStore()
        {
            client = new HttpClient();
        }

        public async Task<List<Genre>> GetItemAsync()
        {
            // get all genre. only call API if the genre list is empty
            // https://developers.themoviedb.org/3/genres/get-movie-list
            if (Genres.Count > 0)
                return Genres;
            else
                Genres.Clear();

            Uri uri = new(APIURL + "/genre/movie/list" + APIHEADER);
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    GenreResult result = JsonSerializer.Deserialize<GenreResult>(content);
                    foreach (var item in result.genres)
                    {
                        Genres.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Genres;
        }

        public async Task<Movie> GetMovie(int id)
        {
            // get a movie item detail by movie id
            // https://developers.themoviedb.org/3/movies/get-movie-details
            Movie Item = new();

            Uri uri = new(APIURL + "/movie/" + id.ToString() + APIHEADER);
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Item = JsonSerializer.Deserialize<Movie>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Item;
        }

        public async Task<string> GetTrailerUrl(int id)
        {
            // get youtube links of movie trailer. return if the video is available in youtube and is a trailer
            // https://developers.themoviedb.org/3/movies/get-movie-videos
            string Item = string.Empty;

            Uri uri = new(APIURL + "/movie/" + id.ToString() + "/videos" + APIHEADER);
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    VideoResult result = JsonSerializer.Deserialize<VideoResult>(content);
                    foreach (var item in result.results)
                    {
                        if (item.site.Equals("YouTube") && item.type.Equals("Trailer"))
                        {
                            Item = "https://www.youtube.com/watch?v=" + item.key;
                            return Item;
                        }
                    }
                    Debug.WriteLine("Video link: " + Item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Item;
        }

        public async Task<ReviewResult> GetMovieReviewsAsync(int id, int page)
        {
            // get a movie review by id. page parameter used for lazy load
            // https://developers.themoviedb.org/3/movies/get-movie-reviews
            ReviewResult Items = new();
            Uri baseUri = new(APIURL + "/movie/" + id.ToString() + "/reviews" + APIHEADER + "&page=");
            Uri uri = new(baseUri + page.ToString());
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Items = JsonSerializer.Deserialize<ReviewResult>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Items;
        }

        public async Task<MovieResult> GetMovieAsync(string searchkey, int page)
        {
            // get movies for search page. page parameter for lazy load
            // https://developers.themoviedb.org/3/search/search-movies
            MovieResult Items = new();

            Uri baseUri = new(APIURL + "/search/movie" + APIHEADER + "&query=" + searchkey + "&page=");
            Uri uri = new(baseUri + page.ToString());
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Items = JsonSerializer.Deserialize<MovieResult>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Items;
        }

        public async Task<MovieResult> GetMovieByGenreAsync(int genrekey, int page)
        {
            // get movies for genre page. page parameter for lazy load
            // https://developers.themoviedb.org/3/discover/movie-discover
            MovieResult Items = new();
            Uri baseUri = new(APIURL + "/discover/movie" + APIHEADER + "&sort_by=popularity.desc&include_video=true&page=");
            Uri uri = new(baseUri + page.ToString() + "&with_genres=" + genrekey.ToString());
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Items = JsonSerializer.Deserialize<MovieResult>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Items;
        }
    }
}
