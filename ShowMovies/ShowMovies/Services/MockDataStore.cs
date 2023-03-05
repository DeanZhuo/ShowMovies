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

        public MockDataStore()
        {
            client = new HttpClient();
        }

        public async Task<List<Genre>> GetItemAsync()
        {
            List<Genre> Items = new();

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
                        Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Items;
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

        public async Task<List<UserReviews>> GetMovieReviewsAsync(int id)
        {
            List<UserReviews> Items = new();

            int page = 1;
            Uri baseUri = new(APIURL + "/movie/" + id.ToString() + "/reviews" + APIHEADER + "&page=");
            Uri uri = new(baseUri + page.ToString());
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    ReviewResult result = JsonSerializer.Deserialize<ReviewResult>(content);
                    int totalPage = result.total_pages;
                    for (int i = 1; i <= totalPage; i++)
                    {
                        page = i;
                        uri = new Uri(baseUri + page.ToString());
                        response = await client.GetAsync(uri);
                        if (response.IsSuccessStatusCode)
                        {
                            content = await response.Content.ReadAsStringAsync();
                            result = JsonSerializer.Deserialize<ReviewResult>(content);
                            foreach (var item in result.results)
                            {
                                Items.Add(item);
                            }
                        }
                        Debug.WriteLine($"finished loading page {page} out of {totalPage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Items;
        }

        public async Task<List<Movie>> GetMovieAsync(string searchkey)
        {
            List<Movie> Items = new();
            int page = 1;

            Uri baseUri = new(APIURL + "/search/movie" + APIHEADER + "&query=" + searchkey + "&page=");
            Uri uri = new(baseUri + page.ToString());
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    MovieResult result = JsonSerializer.Deserialize<MovieResult>(content);
                    int totalPage = result.total_pages;
                    for (int i = 1; i <= totalPage; i++)
                    {
                        page = i;
                        uri = new Uri(baseUri + page.ToString());
                        response = await client.GetAsync(uri);
                        if (response.IsSuccessStatusCode)
                        {
                            content = await response.Content.ReadAsStringAsync();
                            result = JsonSerializer.Deserialize<MovieResult>(content);
                            foreach (var item in result.results)
                            {
                                item.poster_path = "https://image.tmdb.org/t/p/w200" + item.poster_path;
                                Items.Add(item);
                            }
                        }
                        Debug.WriteLine($"finished loading page {page} out of {totalPage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Items;
        }

        // not used. API problem, it keeps loading all movies instead of the search key
        public async Task<List<Movie>> GetMovieByGenreAsync(string genrekey)
        {
            List<Movie> Items = new();
            int page = 1;
            Uri baseUri = new(APIURL + "/discover/movie" + APIHEADER + "&sort_by=vote_average.desc&include_video=true&page=");
            Uri uri = new(baseUri + page.ToString() + "&with_genres=" + genrekey);
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    MovieResult result = JsonSerializer.Deserialize<MovieResult>(content);
                    int totalPage = (result.total_pages > 5) ? 5 : result.total_pages; // takes too much time, lazy load later
                    for (int i = 1; i <= totalPage; i++)
                    {
                        page = i;
                        uri = new Uri(baseUri + page.ToString() + "&with_genres=" + genrekey);
                        response = await client.GetAsync(uri);
                        if (response.IsSuccessStatusCode)
                        {
                            content = await response.Content.ReadAsStringAsync();
                            result = JsonSerializer.Deserialize<MovieResult>(content);
                            foreach (var item in result.results)
                            {
                                Items.Add(item);
                            }
                        }
                        Debug.WriteLine($"finished loading page {page} out of {totalPage}");
                    }
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