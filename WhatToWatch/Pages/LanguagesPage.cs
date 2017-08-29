using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using WhatToWatch.Handlers;
using WhatToWatch.Model;
using WhatToWatch.Service;
using WhatToWatch.Views;
using WhatToWatch.Views.Cells;
using WhatToWatch.Views.ListViews;
using System.Globalization;
using Xamarin.Forms;

namespace WhatToWatch.Pages
{
    public class LanguagesPage : ContentPage
    {
        private Dictionary<string, CultureInfo> languages;

        public LanguagesPage()
        {
            languages = new Dictionary<string, CultureInfo>
            {
                { "English", new CultureInfo("en-US") },
                { "Українська", new CultureInfo("uk-UA") }
            };

            List<string> langs = new List<string>();
            foreach (string langName in languages.Keys)
                langs.Add(langName);

            var listView = new ListView();
            listView.ItemsSource = langs;
            listView.ItemTemplate = new DataTemplate(typeof(TextCell));
            listView.ItemTemplate.SetBinding(TextCell.TextProperty, ".");
            listView.ItemTemplate.SetValue(TextCell.TextColorProperty, Color.Black);

            listView.ItemSelected += ListView_ItemSelected;

            Content = new ScrollView
            {
                Content = listView,
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            AppResources.Culture = languages[(string)e.SelectedItem];
            DataService.LanguagePrefix = (string)e.SelectedItem;
            ((App)Application.Current).UpdateLanguage();

            ((ListView)sender).SelectedItem = null;
            Navigation.PopModalAsync();
        }
    }
}
