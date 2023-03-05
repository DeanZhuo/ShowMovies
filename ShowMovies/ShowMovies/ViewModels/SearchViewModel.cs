using ShowMovies.Models;
using ShowMovies.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShowMovies.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public Command<Movie> ItemTapped { get; }
        public Command<string> SearchCommand { get; }
        public Command LoadMoreCommand { get; }
        private int page;
        private int lastPage;
        private string keyword;
        public ObservableCollection<Movie> Movies { get; }

        public SearchViewModel()
        {
            Title = "Search";
            Movies = new ObservableCollection<Movie>();
            ItemTapped = new Command<Movie>(OnItemSelected);
            SearchCommand = new Command<string>(async (p) => await PerformSearch(p));
            LoadMoreCommand = new Command(async () => await LoadMoreMovie());
        }

        private async Task LoadMoreMovie()
        {
            try
            {
                page += 1;
                if (page > lastPage)
                    return;

                var result = await DataStore.GetMovieAsync(keyword, page);
                foreach (var item in result.results)
                {
                    if (!string.IsNullOrEmpty(item.poster_path))
                    {
                        item.poster_path = "https://image.tmdb.org/t/p/w200" + item.poster_path;
                    }
                    Movies.Add(item);
                }
                Debug.WriteLine($"finished loading page {page} out of {lastPage}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task PerformSearch(string searchKey)
        {
            IsBusy = true;

            try
            {
                Movies.Clear();
                page = 1;
                keyword = searchKey;
                var result = await DataStore.GetMovieAsync(keyword, page);
                lastPage = result.total_pages;
                foreach (var item in result.results)
                {
                    if (!string.IsNullOrEmpty(item.poster_path))
                    {
                        item.poster_path = "https://image.tmdb.org/t/p/w200" + item.poster_path;
                    }
                    Movies.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = false;
            SelectedItem = null;
        }

        private Movie _selectedMovie;

        public Movie SelectedItem
        {
            get => _selectedMovie;
            set
            {
                SetProperty(ref _selectedMovie, value);
                OnItemSelected(value);
            }
        }

        private async void OnItemSelected(Movie item)
        {
            if (item == null)
                return;

            // This will push the MovieDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(MovieDetailPage)}?{nameof(MovieDetailViewModel.MovieId)}={item.id}");
        }
    }
}