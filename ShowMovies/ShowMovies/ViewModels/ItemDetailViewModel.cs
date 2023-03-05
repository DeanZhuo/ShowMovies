using ShowMovies.Models;
using ShowMovies.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShowMovies.ViewModels
{
    [QueryProperty(nameof(SelectedGenre), nameof(SelectedGenre))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private int selectedGenre;

        public int SelectedGenre
        {
            get { return selectedGenre; }
            set
            {
                selectedGenre = value;
                GetGenreName(value);
            }
        }

        private void GetGenreName(int value)
        {
            Genre genre = DataStore.Genres.Find(x => x.id == value);
            GenreName = genre.name;
        }

        private string genreName;

        public string GenreName
        {
            get { return genreName; }
            set { SetProperty(ref genreName, value); }
        }

        public Command LoadItemsCommand { get; }
        public Command<Movie> ItemTapped { get; }
        public Command LoadMoreCommand { get; }
        public ObservableCollection<Movie> Movies { get; }
        private int page;
        private int lastPage;

        public ItemDetailViewModel()
        {
            Movies = new ObservableCollection<Movie>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Movie>(OnItemSelected);
            LoadMoreCommand = new Command(async () => await LoadMoreMovie());
        }

        private async Task LoadMoreMovie()
        {
            try
            {
                page += 1;
                if (page > lastPage)
                    return;

                var result = await DataStore.GetMovieByGenreAsync(SelectedGenre, page);
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

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Movies.Clear();
                page = 1;
                var result = await DataStore.GetMovieByGenreAsync(SelectedGenre, page);
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
            IsBusy = true;
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