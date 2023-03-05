using Xamarin.Forms;

namespace ShowMovies.ViewModels
{
    [QueryProperty(nameof(MovieId), nameof(MovieId))]
    public class MovieDetailViewModel : BaseViewModel
    {
        private int movieId;

        public int MovieId
        {
            get
            {
                return movieId;
            }
            set
            {
                movieId = value;
            }
        }
    }
}