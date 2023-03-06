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
        private const string APIKEY = "1227a9062ce42533a66af3933fd9237c";

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