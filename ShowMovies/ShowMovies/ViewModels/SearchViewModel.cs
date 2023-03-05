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
        public ObservableCollection<Movie> Movies { get; }

        public SearchViewModel()
        {
            Title = "Search";
            Movies = new ObservableCollection<Movie>();
            ItemTapped = new Command<Movie>(OnItemSelected);
            SearchCommand = new Command<string>(async (p) => await PerformSearch(p));
        }

        private async Task PerformSearch(string keyword)
        {
            IsBusy = true;

            try
            {
                Movies.Clear();
                var items = await DataStore.GetMovieAsync(keyword);
                foreach (var item in items)
                {
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