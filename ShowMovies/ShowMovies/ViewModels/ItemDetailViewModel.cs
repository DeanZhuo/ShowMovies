using ShowMovies.Models;
using ShowMovies.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShowMovies.ViewModels
{
    [QueryProperty(nameof(GenreName), nameof(GenreName))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string genreName;

        public string GenreName
        {
            get
            {
                return genreName;
            }
            set
            {
                genreName = value;
            }
        }

        public Command LoadItemsCommand { get; }
        public Command<Movie> ItemTapped { get; }
        public ObservableCollection<Movie> Movies { get; }

        public ItemDetailViewModel()
        {
            Movies = new ObservableCollection<Movie>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Movie>(OnItemSelected);
        }

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Movies.Clear();
                var items = await DataStore.GetMovieByGenreAsync(GenreName);
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