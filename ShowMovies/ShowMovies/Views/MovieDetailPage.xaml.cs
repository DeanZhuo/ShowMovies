using ShowMovies.ViewModels;
using Xamarin.Forms;

namespace ShowMovies.Views
{
    public partial class MovieDetailPage : ContentPage
    {
        public MovieDetailPage()
        {
            InitializeComponent();
            BindingContext = new MovieDetailViewModel();
        }
    }
}