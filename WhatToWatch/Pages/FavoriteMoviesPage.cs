using System;
using System.Collections.Generic;
using WhatToWatch.Handlers;
using WhatToWatch.Model;
using WhatToWatch.Service;
using WhatToWatch.Views.ListViews;
using Xamarin.Forms;

namespace WhatToWatch.Pages
{
    public class FavoriteMoviesPage : ContentPage, IMyContentPage
    {
        private FavoriteMovieListView listView;
        private List<Movie> movies;
        private Label resultLabel;

        public FavoriteMoviesPage()
        {
            movies = null;
            listView = new FavoriteMovieListView(movies);
            listView.HorizontalOptions = LayoutOptions.FillAndExpand;
            resultLabel = new Label { Style = (Style)Application.Current.Resources["centeredLabelStyle"] };
            resultLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;

            Content = new StackLayout
            {
                Children = {
                    resultLabel,
                    listView
                },
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
        }

        public void UpdateLanguage()
        {
            try
            {
                resultLabel.Text = "";
                if (DataService.Instance().LoggedIn)
                    LoadFavoriteMovies();
                else
                    resultLabel.Text = AppResources.LogInForFavoritesList;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                resultLabel.Text = "";
                if (DataService.Instance().LoggedIn)
                    LoadFavoriteMovies();
                else
                {
                    resultLabel.Text = AppResources.LogInForFavoritesList;
                    movies = null;
                    listView.UpdateMovies(movies);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }

        private void LoadFavoriteMovies()
        {
            movies = new List<Movie>(DataService.Instance().GetFavoriteMovies());
            resultLabel.Text = AppResources.NoFavorites;
            if (movies != null && movies.Count > 0)
                resultLabel.Text = AppResources.FavoritesCount + ": " + movies.Count;
            listView.UpdateMovies(movies);
        }
    }
}
