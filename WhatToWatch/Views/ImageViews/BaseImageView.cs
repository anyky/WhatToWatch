using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using WhatToWatch.MyEventArgs;
using Xamarin.Forms;

namespace WhatToWatch.Views.ImageViews
{
    public class BaseImageView : ContentView
    {
        protected double aspect = 1;
        protected Image image;
        private double aWidth = 1;

        public static readonly BindableProperty ImageHeightProperty =
            BindableProperty.Create("ImageHeight", typeof(double), typeof(BaseImageView), (double)1);
        public static readonly BindableProperty ImageWidthProperty =
            BindableProperty.Create("ImageWidth", typeof(double), typeof(BaseImageView), (double)1);

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty) * aWidth * aspect; }
            set { SetValue(ImageHeightProperty, value); }
        }

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty) * aWidth; }
            set { SetValue(ImageWidthProperty, value); }
        }

        public ImageSource Source
        {
            get { return image.Source; }
            set { image.Source = value; }
        }


        public BaseImageView(object context, double aW, ImageSource src = null)
        {
            aWidth = aW;
            image = new Image();
            image.Source = src;
            image.Aspect = Aspect.AspectFill;

            image.BindingContext = this;
            image.SetBinding(Image.HeightRequestProperty, "ImageHeight");
            image.SetBinding(Image.WidthRequestProperty, "ImageWidth");

            this.BindingContext = context;
            this.SetBinding(BaseImageView.ImageWidthProperty, "Width");
            this.SetBinding(BaseImageView.ImageHeightProperty, "Width");

            Content = new StackLayout
            {
                Children = { image },
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center
            };
        }
    }
}
