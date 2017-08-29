using System;
using WhatToWatch.Handlers;
using WhatToWatch.Service;
using Xamarin.Forms;

namespace WhatToWatch.Pages
{
    public class AuthenticationPage : ContentPage, IMyContentPage
    {
        private Label header;
        private Button authButton;
        private Button regButton;
        private Button langButton;

        public LogInPage loginPage;
        public SignUpPage signupPage;
        public LanguagesPage langPage;

        public AuthenticationPage()
        {
            header = new Label { Style = (Style)Application.Current.Resources["centeredLabelStyle"] };


            authButton = new Button
            {
                Text = AppResources.LogIn,
                Style = (Style)Application.Current.Resources["buttonStyle"]
            };
            regButton = new Button
            {
                Text = AppResources.SignUp,
                Style = (Style)Application.Current.Resources["buttonStyle"]
            };
            langButton = new Button
            {
                Text = AppResources.Language,
                Style = (Style)Application.Current.Resources["buttonStyle"]
            };

            loginPage = new LogInPage();
            UpdateAuthorizationButton();
            loginPage.changeLogStateEvent += AuthorizationPage_changeLogStateEvent;
            signupPage = new SignUpPage();
            langPage = new LanguagesPage();

            regButton.Clicked += async (sender, args) =>
                await Navigation.PushModalAsync(signupPage);
            langButton.Clicked += async (sender, args) =>
                await Navigation.PushModalAsync(langPage);

            Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    header,
                    authButton,
                    regButton,
                    langButton
                },
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
        }

        private void AuthorizationPage_changeLogStateEvent(object sender, EventArgs e)
        {
            UpdateAuthorizationButton();
        }

        private void UpdateAuthorizationButton()
        {
            try
            {
                if (DataService.Instance().LoggedIn)
                {
                    authButton.Text = AppResources.LogOut;
                    authButton.Clicked += Unauthorize;
                    authButton.Clicked -= OpenAuthorizationPage;
                    header.Text = AppResources.User + ": " + DataService.Instance().Username;
                }
                else
                {
                    authButton.Text = AppResources.LogIn;
                    authButton.Clicked += OpenAuthorizationPage;
                    authButton.Clicked -= Unauthorize;
                    header.Text = "";
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }

        async private void OpenAuthorizationPage(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(loginPage);
        }

        private void Unauthorize(object sender, EventArgs e)
        {
            loginPage.Unautorize();
        }

        public void UpdateLanguage()
        {
            bool logged = false;
            try
            {
                logged = DataService.Instance().LoggedIn;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message + "logged");
            }

            if (logged)
            {
                authButton.Text = AppResources.LogOut;
                header.Text = AppResources.User + ": " + DataService.Instance().Username;
            }
            else
            {
                authButton.Text = AppResources.LogIn;
                header.Text = "";
            }

            regButton.Text = AppResources.SignUp;
            langButton.Text = AppResources.Language;

            loginPage.UpdateLanguage();
            signupPage.UpdateLanguage();
        }
    }
}
