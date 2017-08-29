using Plugin.Connectivity;
using System;
using System.Globalization;
using WhatToWatch.Handlers;
using WhatToWatch.Pages;
using Xamarin.Forms;

namespace WhatToWatch
{
    public class App : Application
    {
        private event EventHandler<EventArgs> changeLanguageEvent;

        public App()
        {
            AppResources.Culture = new CultureInfo("uk-UA");
            Application.Current.Resources = new ResourceDictionary();
            LoadStyles();
            LoadImageSources();

            // The root page of your application
            var mainPage = new MainTabbedPage();
            changeLanguageEvent += (object sender, EventArgs e) => mainPage.UpdateLanguage();
            MainPage = mainPage;

            CrossConnectivity.Current.ConnectivityChanged += (object sender,
                    Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e) =>
                {
                    if(!e.IsConnected)
                        ExceptionHandler.HandleError(mainPage, AppResources.NoInternet);
                };
        }

        private void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void UpdateLanguage()
        {
            changeLanguageEvent?.Invoke(this, new EventArgs());
        }

        private void LoadImageSources()
        {
            Application.Current.Resources.Add("tabSearchImageSource", FileImageSource.FromFile("tab_magnifier48.png"));
            Application.Current.Resources.Add("tabRecommendImageSource", FileImageSource.FromFile("tab_recommend48.png"));
            Application.Current.Resources.Add("tabFavoritesImageSource", FileImageSource.FromFile("tab_heart48.png"));
            Application.Current.Resources.Add("tabAuthenticationImageSource", FileImageSource.FromFile("tab_man48.png"));

            Application.Current.Resources.Add("reelFilledImageSource", ImageSource.FromFile("reel_filled.png"));
            Application.Current.Resources.Add("reelEmptyImageSource", ImageSource.FromFile("reel_empty.png"));

            Application.Current.Resources.Add("heartOnImageSource", ImageSource.FromFile("heart64on.png"));
            Application.Current.Resources.Add("heartOffImageSource", ImageSource.FromFile("heart64off.png"));

            Application.Current.Resources.Add("fingerOnImageSource", ImageSource.FromFile("finger64on.png"));
            Application.Current.Resources.Add("fingerOffImageSource", ImageSource.FromFile("finger64off.png"));

            Application.Current.Resources.Add("crossImageSource", ImageSource.FromFile("cross64.png"));
        }

        private void LoadStyles()
        {
            Application.Current.Resources.Add("buttonStyle",
                new Style(typeof(Button))
                {
                    Setters =
                    {
                        new Setter { Property = Button.BackgroundColorProperty, Value = Color.Orange },
                        new Setter { Property = Button.TextColorProperty, Value = Color.White },
                        new Setter { Property = Button.BorderColorProperty, Value = Color.Orange },
                        new Setter { Property = Button.HorizontalOptionsProperty, Value = LayoutOptions.FillAndExpand },
                        new Setter { Property = Button.VerticalOptionsProperty, Value = LayoutOptions.Fill },
                        new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold }
                    }
                });

            Application.Current.Resources.Add("pressedButtonStyle",
                new Style(typeof(Button))
                {
                    Setters =
                    {
                        new Setter { Property = Button.BackgroundColorProperty, Value = Color.White },
                        new Setter { Property = Button.TextColorProperty, Value = Color.Orange },
                        new Setter { Property = Button.BorderColorProperty, Value = Color.Orange },
                        new Setter { Property = Button.HorizontalOptionsProperty, Value = LayoutOptions.FillAndExpand },
                        new Setter { Property = Button.VerticalOptionsProperty, Value = LayoutOptions.Fill },
                        new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold }
                    }
                });

            Application.Current.Resources.Add("centeredLabelStyle",
                new Style(typeof(Label))
                {
                    Setters =
                    {
                        new Setter { Property = Label.TextColorProperty, Value = Color.Black },
                        new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.None },
                        new Setter { Property = Label.FontSizeProperty, Value = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) },
                        new Setter { Property = Label.HorizontalOptionsProperty, Value = LayoutOptions.FillAndExpand },
                        new Setter { Property = Label.VerticalOptionsProperty, Value = LayoutOptions.Center },
                        new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center }
                    }
                });

            Application.Current.Resources.Add("centeredHeaderLabelStyle",
                new Style(typeof(Label))
                {
                    Setters =
                    {
                        new Setter { Property = Label.TextColorProperty, Value = Color.Black },
                        new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold },
                        new Setter { Property = Label.FontSizeProperty, Value = Device.GetNamedSize(NamedSize.Large, typeof(Label)) },
                        new Setter { Property = Label.HorizontalOptionsProperty, Value = LayoutOptions.FillAndExpand },
                        new Setter { Property = Label.VerticalOptionsProperty, Value = LayoutOptions.CenterAndExpand },
                        new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center }
                    }
                });
        }
    }
}
