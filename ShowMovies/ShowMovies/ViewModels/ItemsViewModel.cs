using ShowMovies.Models;
using ShowMovies.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShowMovies.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Genre _selectedGenre;

        public ObservableCollection<Genre> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command<Genre> ItemTapped { get; }

        public ItemsViewModel()
        {
            Title = "Browse Genre";
            Items = new ObservableCollection<Genre>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Genre>(OnItemSelected);
        }

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemAsync();
                foreach (var item in items)
                {
                    Items.Add(item);
                    Debug.WriteLine(item.name);
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

        public Genre SelectedItem
        {
            get => _selectedGenre;
            set
            {
                SetProperty(ref _selectedGenre, value);
                OnItemSelected(value);
            }
        }

        private async void OnItemSelected(Genre item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.GenreName)}={item.name}");
        }
    }
}