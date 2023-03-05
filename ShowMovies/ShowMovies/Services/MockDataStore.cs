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
        private const string APIURL = "https://api.themoviedb.org/3/";

        private const string APIHEADER = "?api_key=" + APIKEY + "&language=en-US";

        private HttpClient client;

        public MockDataStore()
        {
            client = new HttpClient();
        }

        public async Task<List<Genre>> GetItemAsync()
        {
            List<Genre> Items = new List<Genre>();

            Uri uri = new Uri(APIURL + "/genre/movie/list" + APIHEADER + "/");
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

        public Task<Movie> GetMovie(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Movie>> GetMovieAsync(string searchkey)
        {
            List<Movie> Items = new List<Movie>();
            int page = 1;
            Uri uri = new Uri(APIURL + "/search/movie" + APIHEADER + "&query=" + searchkey + "&page=" + page);
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
                        uri = new Uri(APIURL + "/search/movie" + APIHEADER + "&query=" + searchkey + "&page=" + page);
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

        public async Task<List<Movie>> GetMovieByGenreAsync(string genrekey)
        {
            List<Movie> Items = new List<Movie>();
            int page = 1;
            //https://api.themoviedb.org/3/discover/movie?api_key=1227a9062ce42533a66af3933fd9237c&language=en-US&sort_by=vote_average.desc&include_video=true&page=1&with_genres=action
            Uri baseUri = new Uri(APIURL + "/discover/movie" + APIHEADER + "&sort_by=vote_average.desc&include_video=true&page=");
            Uri uri = new Uri(baseUri, page + "&with_genres=" + genrekey);
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
                        uri = uri = new Uri(baseUri, page + "&with_genres=" + genrekey);
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