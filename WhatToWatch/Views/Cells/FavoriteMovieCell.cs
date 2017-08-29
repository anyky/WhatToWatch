using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using WhatToWatch.Handlers;
using WhatToWatch.Service;
using WhatToWatch.Views.ImageViews;
using Xamarin.Forms;

namespace WhatToWatch.Views.Cells
{
    public class FavoriteMovieCell : MovieCell
    {
        protected event EventHandler<EventArgs> changeFavoriteEvent;
        protected FavoriteImageView movieFavImage;

        public static readonly BindableProperty InFavoriteProperty =
            BindableProperty.Create("InFavorite", typeof(bool), typeof(FavoriteMovieCell), true);

        public bool InFavorite
        {
            get { return (bool)GetValue(InFavoriteProperty); }
            set { SetValue(InFavoriteProperty, value); }
        }

        public FavoriteMovieCell()
        {
            changeFavoriteEvent += FavoriteMovieCell_changeFavoriteEvent;

            movieFavImage = new FavoriteImageView(stack, true, changeFavoriteEvent);

            stack.Children.Add(movieFavImage);
        }

        private void FavoriteMovieCell_changeFavoriteEvent(object sender, EventArgs e)
        {
            if (DataService.Instance().LoggedIn)
            {
                DataService.Instance().ChangeFavorite(MovieId, !InFavorite);
                movieFavImage.isOn = InFavorite = !InFavorite;
            }
            else
            {
                throw new Exception("Для додавання в обране необхідна авторизація");
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                movieFavImage.isOn = InFavorite;
            }
        }
    }
}
