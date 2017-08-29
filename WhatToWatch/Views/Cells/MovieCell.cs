using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using WhatToWatch.Views.ImageViews;
using Xamarin.Forms;

namespace WhatToWatch.Views.Cells
{
    public class MovieCell : ViewCell
    {
        protected StackLayout stack;
        protected Label nameLabel;
        protected PosterImageView image;

        public static readonly BindableProperty MovieIdProperty =
            BindableProperty.Create("MovieId", typeof(int), typeof(MovieCell), 0);
        public static readonly BindableProperty MovieNameProperty =
            BindableProperty.Create("MovieName", typeof(string), typeof(MovieCell), "Name");
        public static readonly BindableProperty MovieImageProperty =
            BindableProperty.Create("MovieImage", typeof(ImageSource), typeof(MovieCell));

        public int MovieId
        {
            get { return (int)GetValue(MovieIdProperty); }
            set { SetValue(MovieIdProperty, value); }
        }

        public string MovieName
        {
            get { return (string)GetValue(MovieNameProperty); }
            set { SetValue(MovieNameProperty, value); }
        }

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource MovieImage
        {
            get { return (ImageSource)GetValue(MovieImageProperty); }
            set { SetValue(MovieImageProperty, value); }
        }


        public MovieCell()
        {
            stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;
            stack.BackgroundColor = Color.White;

            image = new PosterImageView(stack, 0.16, MovieImage);

            nameLabel = new Label
            {
                Style = (Style)Application.Current.Resources["centeredLabelStyle"]
            };

            stack.Children.Add(image);
            stack.Children.Add(nameLabel);

            View = new Frame { Content = stack, CornerRadius = 5, OutlineColor = Color.LightGray };
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                nameLabel.Text = MovieName;
                image.Source = MovieImage;
            }
        }
    }
}
