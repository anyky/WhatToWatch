using System;
using System.Collections.Generic;
using WhatToWatch.Handlers;
using WhatToWatch.Model;
using WhatToWatch.Service;
using WhatToWatch.Views.ListViews;
using Xamarin.Forms;

namespace WhatToWatch.Pages
{
    public class RecommendationPage : ContentPage, IMyContentPage
    {
        private Button recommendButton;
        private List<Movie> movies;
        private MovieListView listView;
        private int[] lastCriterias;

        public RecommendationPage()
        {
            recommendButton = new Button
            {
                Text = AppResources.ChooseCriterias,
                Style = (Style)Application.Current.Resources["buttonStyle"]
            };
            recommendButton.Clicked += RecommendButton_Clicked;

            lastCriterias = new int[0];
            movies = null;
            listView = new MovieListView(movies);

            Content = new StackLayout
            {
                Children = {
                    recommendButton,
                    listView
                }
            };
        }

        private async void RecommendButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (DataService.Instance().LoggedIn)
                {
                    DataService.Instance().StartRecommendations();

                    if (DataService.Instance().NeedQuiz)
                    {
                        bool startQuiz = await ExceptionHandler.HandleDialog(this, AppResources.VoteMoviesBeforeRecommendations, AppResources.VoteRandomMovie, AppResources.Later);
                        if (startQuiz)
                            await Navigation.PushModalAsync(new MoviePage());
                    }
                    else
                    {
                        var criteriasPage = new CriteriasMenuPage(lastCriterias);
                        criteriasPage.recommendationEvent += Page_recommendationEvent;
                        await Navigation.PushModalAsync(criteriasPage);
                    }
                }
                else
                {
                    ExceptionHandler.HandleWarning(this, AppResources.LogInForRecommendations);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }

        private void Page_recommendationEvent(object sender, int[] criterias)
        {
            lastCriterias = criterias;
            LoadRecommedMovies();
        }

        private void UpdateMovies(List<Movie> mov)
        {
            movies = mov;
            listView.UpdateMovies(movies);
        }

        private void LoadRecommedMovies()
        {
            List<Movie> mov = null;
            try
            {
                DataService.Instance().GetRecommendations(lastCriterias);
                mov = new List<Movie>(DataService.Instance().Movies);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
            UpdateMovies(mov);
        }

        public void UpdateLanguage()
        {
            recommendButton.Text = AppResources.ChooseCriterias;
            if(movies != null)
                LoadRecommedMovies();
        }
    }
}
