using System;
using WhatToWatch.Handlers;
using WhatToWatch.Model;
using WhatToWatch.MyEventArgs;
using WhatToWatch.Service;
using WhatToWatch.Views;
using WhatToWatch.Views.ImageViews;
using Xamarin.Forms;

namespace WhatToWatch.Pages
{
    public class MoviePage : ContentPage
    {
        private bool isQuiz;
        private bool isQuizOver = false;
        private Movie movie;
        public event EventHandler<ChangeMarkEventArgs> changeMarkEvent;
        public event EventHandler<EventArgs> changeFavoriteEvent;
        private Label quizTitle;
        private Button quizNextButton;
        private Button quizCancelButton;
        public MoviePage(int id)
        {
            isQuiz = false;
            changeMarkEvent += MoviePage_changeMarkEvent;
            changeFavoriteEvent += MoviePage_changeFavoriteEvent;

            movie = DataService.Instance().GetMovieById(id);
            UpdateMovie();
        }

        public MoviePage()
        {
            isQuiz = true;
            changeMarkEvent += MoviePage_changeMarkEvent;
            changeFavoriteEvent += MoviePage_changeFavoriteEvent;

            movie = DataService.Instance().GetQuizMovie();
            UpdateMovie();
        }

        private void MoviePage_changeMarkEvent(object sender, ChangeMarkEventArgs e)
        {
            RatingView ratingView = (RatingView)sender;
            try
            {
                if(DataService.Instance().LoggedIn)
                {
                    DataService.Instance().ChangeMark(e.markId, e.markVal);
                    ratingView.UpdateMark();
                    
                    if (isQuiz && DataService.Instance().TheMovie.GetMarks().FindAll(i => i.Value == 0).Count == 0)
                    {
                        quizNextButton.Text = AppResources.Next;
                        quizTitle.Text = AppResources.Voted + ": " + Math.Min(DataService.Instance().GradedCount+1, 10) + "/" + DataService.Instance().RequireGraded;

                        if (DataService.Instance().GradedCount + 1 >= 10 && isQuizOver == false)
                        {
                            isQuizOver = true;
                            ExceptionHandler.HandleMessage(this, AppResources.VotingFinished);
                            Navigation.PopModalAsync();
                        }
                    }
                }
                else
                {
                    ExceptionHandler.HandleWarning(this, AppResources.LogInForVoting);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }

        private void MoviePage_changeFavoriteEvent(object sender, EventArgs e)
        {
            var favoriteView = (FavoriteImageView)sender;
            try
            {
                if (DataService.Instance().LoggedIn)
                {
                    DataService.Instance().ChangeFavorite(movie.MovieId, !movie.InFavorite);
                    favoriteView.isOn = movie.InFavorite;
                }
                else
                {
                    ExceptionHandler.HandleWarning(this, AppResources.LogInForAddingToFavorites);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }

        private void UpdateMovie()
        {
            var fullContent = new StackLayout();
            fullContent.HorizontalOptions = LayoutOptions.StartAndExpand;
            fullContent.VerticalOptions = LayoutOptions.FillAndExpand;

            if(isQuiz)
            {
                quizTitle = new Label();
                quizTitle.Margin = new Thickness { Top = 10 };
                quizTitle.Text = AppResources.Voted + ": " + Math.Min(DataService.Instance().GradedCount, 10) + "/" + DataService.Instance().RequireGraded;
                quizTitle.TextColor = Color.Black;
                quizTitle.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
                quizTitle.FontAttributes = FontAttributes.Bold;
                quizTitle.HorizontalOptions = LayoutOptions.Fill;
                quizTitle.HorizontalTextAlignment = TextAlignment.Center;

                quizNextButton = new Button
                {
                    Text = AppResources.Skip,
                    Style = (Style)Application.Current.Resources["buttonStyle"]
                };
                quizCancelButton = new Button
                {
                    Text = AppResources.StopVoting,
                    Style = (Style)Application.Current.Resources["buttonStyle"]
                };


                quizNextButton.Clicked += async (object sender, EventArgs e) =>
                {
                    try
                    {
                        movie = DataService.Instance().GetQuizMovie();

                        if (!DataService.Instance().NeedQuiz && isQuizOver == false)
                        {
                            isQuizOver = true;
                            ExceptionHandler.HandleMessage(this, AppResources.VotingFinished);
                            await Navigation.PopModalAsync();
                        }
                        UpdateMovie();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.HandleError(this, ex.Message);
                    }
                };

                quizCancelButton.Clicked += async (object sender, EventArgs e) =>
                {
                    if(isQuizOver ||
                        await ExceptionHandler.HandleDialog(this, AppResources.VotingNotFinished, AppResources.StopVoting, AppResources.ContinueVoting))
                        await Navigation.PopModalAsync();
                };

                var quizButtons = new StackLayout
                {
                    Children =
                    {
                        quizCancelButton,
                        quizNextButton
                    },
                    Orientation = StackOrientation.Horizontal
                };

                fullContent.Children.Add(quizTitle);
                fullContent.Children.Add(quizButtons);
            }

            var movieTitle = new Label();
            movieTitle.Text = movie.Name;
            movieTitle.TextColor = Color.Black;
            movieTitle.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
            movieTitle.FontAttributes = FontAttributes.Bold;
            movieTitle.HorizontalOptions = LayoutOptions.FillAndExpand;
            movieTitle.HorizontalTextAlignment = TextAlignment.Center;
            movieTitle.VerticalTextAlignment = TextAlignment.Center;

            if (DataService.Instance().LoggedIn)
            {
                var movieTitleStack = new StackLayout();
                movieTitleStack.Orientation = StackOrientation.Horizontal;
                movieTitleStack.HorizontalOptions = LayoutOptions.FillAndExpand;
                movieTitleStack.VerticalOptions = LayoutOptions.FillAndExpand;

                var movieFavImage = new FavoriteImageView(movieTitleStack, movie.InFavorite, changeFavoriteEvent);

                movieTitleStack.Children.Add(movieFavImage);
                movieTitleStack.Children.Add(movieTitle);

                movieTitleStack.Margin = new Thickness { Top = 10 };
                fullContent.Children.Add(movieTitleStack);
            }
            else
            {
                movieTitle.Margin = new Thickness { Top = 10 };
                fullContent.Children.Add(movieTitle);
            }

            var movieInfo = new StackLayout();
            PosterImageView moviePoster;
            if (Device.Idiom == TargetIdiom.Phone)
            {
                moviePoster = new PosterImageView(movieInfo, 0.4, movie.FullPosterURL);
            }
            else
            {
                moviePoster = new PosterImageView(movieInfo, 0.27, movie.FullPosterURL);
                movieInfo.Orientation = StackOrientation.Horizontal;
            }
            moviePoster.HorizontalOptions = LayoutOptions.Center;
            moviePoster.VerticalOptions = LayoutOptions.Center;

            var movieShort = new StackLayout();
            movieShort.Children.Add(new Label { FormattedText = GetFormattedString(AppResources.Year, movie.Year.ToString()) });
            movieShort.Children.Add(new Label { FormattedText = GetFormattedString(AppResources.Director, movie.Directors.ToString()) });
            movieShort.Children.Add(new Label { FormattedText = GetFormattedString(AppResources.Genres, movie.Genres.ToString()) });
            movieShort.Children.Add(new Label { FormattedText = GetFormattedString(AppResources.Duration, movie.Duration.ToString()) });
            movieShort.Children.Add(new Label { FormattedText = GetFormattedString(AppResources.Actors, movie.Actors.ToString()) });
            if(!isQuiz)
                movieShort.Children.Add(new Label { FormattedText = GetFormattedString(AppResources.Description, movie.Description.ToString()) });
            movieShort.HorizontalOptions = LayoutOptions.Fill;
            movieShort.VerticalOptions = LayoutOptions.FillAndExpand;

            movieInfo.Children.Add(moviePoster);
            movieInfo.Children.Add(movieShort);

            fullContent.Children.Add(movieInfo);

            var mainButton = new Button
            {
                Text = AppResources.Main,
                Style = (Style)Application.Current.Resources["buttonStyle"]
            };
            var emotButton = new Button
            {
                Text = AppResources.Emotions,
                Style = (Style)Application.Current.Resources["buttonStyle"]
            };

            var buttonLayout = new RelativeLayout();
            buttonLayout.BackgroundColor = Color.Orange;
            buttonLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            buttonLayout.VerticalOptions = LayoutOptions.Fill;
            buttonLayout.Children.Add(mainButton,
                Constraint.Constant(0),
                Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => parent.Width / 2),
                Constraint.RelativeToParent((parent) => Math.Max(parent.Width / 15, 50)));
            buttonLayout.Children.Add(emotButton,
                Constraint.RelativeToParent((parent) => parent.Width / 2),
                Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => parent.Width / 2),
                Constraint.RelativeToParent((parent) => Math.Max(parent.Width / 15, 50)));

            var markLayout = new StackLayout();
            mainButton.Clicked += (object sender, EventArgs e) =>
            {
                if (mainButton.BackgroundColor != Color.White)
                {
                    mainButton.Style = (Style)Application.Current.Resources["pressedButtonStyle"];
                    emotButton.Style = (Style)Application.Current.Resources["buttonStyle"];

                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    int i = 0;
                    foreach (var mark in movie.GetMainMarks())
                    {
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        grid.Children.Add(new RatingView(mark, changeMarkEvent), 0, i++);
                    }
                    markLayout.Children.Clear();
                    markLayout.Children.Add(grid);
                }
            };

            emotButton.Clicked += (object sender, EventArgs e) =>
            {
                if (emotButton.BackgroundColor != Color.White)
                {
                    emotButton.Style = (Style)Application.Current.Resources["pressedButtonStyle"];
                    mainButton.Style = (Style)Application.Current.Resources["buttonStyle"];

                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    int i = 0;
                    foreach (var mark in movie.GetEmotMarks())
                    {
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        grid.Children.Add(new RatingView(mark, changeMarkEvent), 0, i++);
                    }
                    markLayout.Children.Clear();
                    markLayout.Children.Add(grid);
                }
            };

            fullContent.Children.Add(buttonLayout);
            fullContent.Children.Add(markLayout);

            var commentsButton = new Button
            {
                Text = AppResources.Comments,
                Style = (Style)Application.Current.Resources["buttonStyle"]
            };
            commentsButton.Clicked += async (object sender, EventArgs e) => await Navigation.PushModalAsync(new CommentsPage());
            fullContent.Children.Add(commentsButton);

            Content = new ScrollView() { Content = fullContent, Padding = new Thickness { Left = 10, Bottom = 10, Right = 10 }, BackgroundColor = Color.White };
        }

        private FormattedString GetFormattedString(string header, string text)
        {
            var fs = new FormattedString();
            fs.Spans.Add(new Span { Text = header, FontAttributes = FontAttributes.Bold, ForegroundColor = Color.Black });
            fs.Spans.Add(new Span { Text = ": " + text, FontAttributes = FontAttributes.None, ForegroundColor = Color.Black });

            return fs;
        }
    }
}
