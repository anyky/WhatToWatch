using System;
using System.Collections.Generic;
using WhatToWatch.Handlers;
using WhatToWatch.Model;
using WhatToWatch.Service;
using WhatToWatch.Views.ListViews;
using Xamarin.Forms;

namespace WhatToWatch.Pages
{
    public class SearchMoviePage : ContentPage, IMyContentPage
    {
        private SearchBar searchBar;
        private Label resultLabel;
        private MovieListView listView;
        private List<Movie> movies;
        private string searched = null;
        public SearchMoviePage()
        {
            searchBar = new SearchBar();
            searchBar.SearchButtonPressed += SearchBar_SearchButtonPressed;
            resultLabel = new Label { Style = (Style)Application.Current.Resources["centeredLabelStyle"] };
            resultLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;

            movies = null;
            listView = new MovieListView(movies);

            Content = new StackLayout
            {
                Children = {
                    searchBar,
                    resultLabel,
                    listView
                },
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(movies == null)
            {
                LoadNewMovies();
            }
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (searchBar.Text == null)
                return;

            try
            {
                movies = new List<Movie>(DataService.Instance().GetMoviesBySearch(searchBar.Text));

                searched = searchBar.Text;
                if (movies.Count == 0)
                {
                    resultLabel.Text = AppResources.MoviesNotFound;
                    listView.UpdateMovies(null);
                }
                else
                {
                    resultLabel.Text = AppResources.MoviesFound + ": " + movies.Count;
                    listView.UpdateMovies(movies);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }

        public void UpdateLanguage()
        {
            if (searched != null && movies.Count == 0)
            {
                resultLabel.Text = AppResources.MoviesNotFound;
                listView.UpdateMovies(null);
            }
            else
            {
                LoadNewMovies();
            }
        }

        private void LoadNewMovies()
        {
            try
            {
                resultLabel.Text = AppResources.NewMovies;
                movies = new List<Movie>(DataService.Instance().GetNewMovies());
                listView.UpdateMovies(movies);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }
    }
}
