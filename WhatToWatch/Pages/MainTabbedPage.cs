using Xamarin.Forms;

namespace WhatToWatch.Pages
{
    public class MainTabbedPage : TabbedPage, IMyContentPage
    {
        SearchMoviePage searchPage;
        RecommendationPage recommendPage;
        FavoriteMoviesPage favoritePage;
        AuthenticationPage authenticationPage;

        public MainTabbedPage()
        {
            BarTextColor = Color.White;
            BarBackgroundColor = Color.Orange;
            BackgroundColor = Color.White;

            searchPage = new SearchMoviePage { Title = AppResources.SearchTab, Icon = (FileImageSource)Application.Current.Resources["tabSearchImageSource"] };
            recommendPage = new RecommendationPage { Title = AppResources.RecommendationTab, Icon = (FileImageSource)Application.Current.Resources["tabRecommendImageSource"] };
            favoritePage = new FavoriteMoviesPage { Title = AppResources.FavoritesTab, Icon = (FileImageSource)Application.Current.Resources["tabFavoritesImageSource"] };
            authenticationPage = new AuthenticationPage { Title = AppResources.LoginTab, Icon = (FileImageSource)Application.Current.Resources["tabAuthenticationImageSource"] };

            Children.Add(searchPage);
            Children.Add(recommendPage);
            Children.Add(favoritePage);
            Children.Add(authenticationPage);
        }

        public void UpdateLanguage()
        {
            searchPage.Title = AppResources.SearchTab;
            recommendPage.Title = AppResources.RecommendationTab;
            favoritePage.Title = AppResources.FavoritesTab;
            authenticationPage.Title = AppResources.LoginTab;

            searchPage.UpdateLanguage();
            recommendPage.UpdateLanguage();
            favoritePage.UpdateLanguage();
            authenticationPage.UpdateLanguage();
        }
    }
}
