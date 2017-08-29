using System;
using System.Collections.Generic;
using WhatToWatch.Handlers;
using WhatToWatch.Model;
using WhatToWatch.Pages;
using WhatToWatch.Views.Cells;
using Xamarin.Forms;

namespace WhatToWatch.Views.ListViews
{
    public class MovieListView : ListView
    {
        protected List<Movie> movies;
        private bool loadingMovie = false;

        public MovieListView(List<Movie> mov)
        {
            UpdateMovies(mov);

            this.ItemSelected +=
                async (object sender, SelectedItemChangedEventArgs e) =>
                {
                    if (e.SelectedItem == null)
                        return;
                    if (!loadingMovie)
                    {
                        loadingMovie = true;
                        try
                        {
                            await Navigation.PushModalAsync(new MoviePage(((Movie)e.SelectedItem).MovieId));
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.HandleError(App.Current.MainPage, ex.Message);
                        }
                        ((ListView)sender).SelectedItem = null;
                        loadingMovie = false;
                    }
                };
        }

        public virtual void UpdateMovies(List<Movie> mov)
        {
            if (mov?.Count == 0)
                mov = null;

            this.HasUnevenRows = true;
            this.movies = mov;
            this.ItemsSource = movies;
            if (movies != null)
            {
                this.ItemTemplate = new DataTemplate(typeof(MovieCell));
                this.ItemTemplate.SetBinding(MovieCell.MovieIdProperty, "MovieId");
                this.ItemTemplate.SetBinding(MovieCell.MovieNameProperty, "Name");
                this.ItemTemplate.SetBinding(MovieCell.MovieImageProperty, "FullPosterURL");

                ScrollTo(movies[0], ScrollToPosition.Start, false);
            }
        }
    }
}
