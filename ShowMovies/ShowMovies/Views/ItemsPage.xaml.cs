﻿using ShowMovies.ViewModels;
using Xamarin.Forms;

namespace ShowMovies.Views
{
    public partial class ItemsPage : ContentPage
    {
        private ItemsViewModel _viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new ItemsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}