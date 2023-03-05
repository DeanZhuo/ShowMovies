using ShowMovies.ViewModels;
using Xamarin.Forms;

namespace ShowMovies.Views
{
    public partial class MovieDetailPage : ContentPage
    {
        private MovieDetailViewModel _viewModel;

        public MovieDetailPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new MovieDetailViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}