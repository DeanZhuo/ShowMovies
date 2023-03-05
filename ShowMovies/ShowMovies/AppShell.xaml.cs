using ShowMovies.Views;
using Xamarin.Forms;

namespace ShowMovies
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(MovieDetailPage), typeof(MovieDetailPage));
        }
    }
}