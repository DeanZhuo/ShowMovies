using ShowMovies.ViewModels;
using Xamarin.Forms;

namespace ShowMovies.Views
{
    public partial class SearchPage : ContentPage
    {
        private SearchViewModel _viewModel;

        public SearchPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new SearchViewModel();
        }
    }
}