using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using WhatToWatch.Model;
using WhatToWatch.Pages;
using WhatToWatch.Views.Cells;
using Xamarin.Forms;

namespace WhatToWatch.Views.ListViews
{
    public class FavoriteMovieListView : MovieListView
    {
        public FavoriteMovieListView(List<Movie> mov) : base(mov)
        {
        }

        public override void UpdateMovies(List<Movie> mov)
        {
            if (mov?.Count == 0)
                mov = null;

            this.HasUnevenRows = true;
            this.movies = mov;
            this.ItemsSource = movies;
            if (movies != null)
            {
                this.ItemTemplate = new DataTemplate(typeof(FavoriteMovieCell));

                this.ItemTemplate.SetBinding(FavoriteMovieCell.MovieIdProperty, "MovieId");
                this.ItemTemplate.SetBinding(FavoriteMovieCell.MovieNameProperty, "Name");
                this.ItemTemplate.SetBinding(FavoriteMovieCell.MovieImageProperty, "FullPosterURL");
                this.ItemTemplate.SetBinding(FavoriteMovieCell.InFavoriteProperty, "InFavorite");

                ScrollTo(movies[0], ScrollToPosition.Start, false);
            }
        }
    }
}
