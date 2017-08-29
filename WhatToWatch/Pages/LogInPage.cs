using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using WhatToWatch.Handlers;
using WhatToWatch.Service;
using Xamarin.Forms;

namespace WhatToWatch.Pages
{
    public class LogInPage : ContentPage, IMyContentPage
    {
        public event EventHandler<EventArgs> changeLogStateEvent;

        private Entry loginEntry;
        private Entry passwordEntry;
        private Button okButton;

        public LogInPage()
        {
            loginEntry = new Entry { Placeholder = AppResources.LoginName };
            loginEntry.TextChanged += (sender, args) =>
            {
                var entry = (Entry)sender;
                string text = args.NewTextValue;
                Regex rgx = new Regex("[^a-zA-Z0-9-]");
                string newtext = rgx.Replace(text, "");

                if (newtext.Length > 50)
                    newtext = text.Remove(newtext.Length - 1);
                if(newtext.CompareTo(text) != 0)
                    entry.Text = newtext;
            };
            passwordEntry = new Entry { Placeholder = AppResources.Password, IsPassword = true };
            passwordEntry.TextChanged += (sender, args) =>
            {
                var entry = (Entry)sender;
                string text = args.NewTextValue;
                string newtext = text;

                if (newtext.Length > 50)
                    newtext = text.Remove(newtext.Length - 1);
                if (newtext.CompareTo(text) != 0)
                    entry.Text = newtext;
            };
            okButton = new Button
            {
                Text = AppResources.LogIn,
                Style = (Style)Application.Current.Resources["buttonStyle"]
            };

            okButton.Clicked += OkButton_Clicked;

            BackgroundColor = Color.White;

            Content = new StackLayout
            {
                Children = {
                    loginEntry,
                    passwordEntry,
                    okButton
                },
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
        }

        public void Unautorize()
        {
            try
            {
                DataService.Instance().LogOutUser();
                changeLogStateEvent.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleError(this, ex.Message);
            }
        }

        private void OkButton_Clicked(object sender, EventArgs e)
        {
            if (loginEntry.Text == null || passwordEntry.Text == null)
                ExceptionHandler.HandleWarning(this, AppResources.NoLoginOrPassword);
            else
            {
                try
                {
                    string response = DataService.Instance().LogInUser(loginEntry.Text, passwordEntry.Text);

                    if (DataService.Instance().LoggedIn)
                    {
                        changeLogStateEvent.Invoke(this, new EventArgs());
                        Navigation.PopModalAsync();
                    }
                    else
                    {
                        ExceptionHandler.HandleWarning(this, response);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleError(this, ex.Message);
                }
            }
        }

        public void UpdateLanguage()
        {
            loginEntry.Placeholder = AppResources.LoginName;
            passwordEntry.Placeholder = AppResources.Password;
            okButton.Text = AppResources.LogIn;
        }
    }
}
