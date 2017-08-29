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
    public class SignUpPage : ContentPage, IMyContentPage
    {
        private Entry loginEntry;
        private Entry passwordEntry;
        private Entry password2Entry;
        private Entry emailEntry;
        private Button okButton;

        public SignUpPage()
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
                if (newtext.CompareTo(text) != 0)
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
            password2Entry = new Entry { Placeholder = AppResources.ConfirmPassword, IsPassword = true };
            password2Entry.TextChanged += (sender, args) =>
            {
                var entry = (Entry)sender;
                string text = args.NewTextValue;
                string newtext = text;

                if (newtext.Length > 50)
                    newtext = text.Remove(newtext.Length - 1);
                if (newtext.CompareTo(text) != 0)
                    entry.Text = newtext;
            };
            emailEntry = new Entry { Placeholder = AppResources.Email };
            emailEntry.TextChanged += (sender, args) =>
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
                Text = AppResources.SignUp,
                Style = (Style)Application.Current.Resources["buttonStyle"]
            };

            okButton.Clicked += OkButton_Clicked;

            BackgroundColor = Color.White;

            Content = new StackLayout
            {
                Children = {
                    loginEntry,
                    passwordEntry,
                    password2Entry,
                    emailEntry,
                    okButton
                },
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
        }

        private void OkButton_Clicked(object sender, EventArgs e)
        {
            if (loginEntry.Text == null || passwordEntry.Text == null || password2Entry.Text == null || emailEntry.Text == null)
                ExceptionHandler.HandleWarning(this, AppResources.AllFieldsAreRequired);
            else
            {
                try
                {
                    string response = DataService.Instance().RegisterUser(loginEntry.Text, passwordEntry.Text, password2Entry.Text, emailEntry.Text);

                    if (response == "OK")
                    {
                        ExceptionHandler.HandleMessage(this, AppResources.MessageSentToEmail);
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
            password2Entry.Placeholder = AppResources.ConfirmPassword;
            emailEntry.Placeholder = AppResources.Email;
            okButton.Text = AppResources.SignUp;
        }
    }
}
