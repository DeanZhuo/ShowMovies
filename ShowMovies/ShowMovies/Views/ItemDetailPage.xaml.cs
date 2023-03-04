using ShowMovies.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace ShowMovies.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}