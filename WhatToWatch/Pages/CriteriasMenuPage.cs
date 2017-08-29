using System;
using System.Collections.Generic;
using System.Linq;
using WhatToWatch.Service;
using WhatToWatch.Views.Cells;
using Xamarin.Forms;

namespace WhatToWatch.Pages
{
    public class CriteriasMenuPage : ContentPage
    {
        public event EventHandler<int[]> recommendationEvent;

        private List<CriteriaView> criteriaViews;
        private Button recommendButton;

        public CriteriasMenuPage(int[] lastCriterias)
        {
            var label = new Label
            {
                Text = AppResources.RecommendationCriterias,
                Style = (Style)Application.Current.Resources["centeredHeaderLabelStyle"],
                VerticalOptions = LayoutOptions.Start
            };

            var grid1 = new Grid();
            grid1.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            var grid2 = new Grid();
            grid2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            int i1 = 0, i2 = 0;

            criteriaViews = new List<CriteriaView>();
            foreach (var criteria in DataService.Instance().Criterias)
            {
                var view = new CriteriaView(criteria.CriteriaId, lastCriterias.Contains(criteria.CriteriaId), criteria.CriteriaName);
                criteriaViews.Add(view);

                if (criteria.CriteriaType == 1)
                {
                    grid1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    grid1.Children.Add(view, 0, i1++);
                }
                else
                {
                    grid2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    grid2.Children.Add(view, 0, i2++);
                }
            }

            recommendButton = new Button
            {
                Text = AppResources.GetRecommendations,
                Style = (Style)Application.Current.Resources["buttonStyle"]
            };
            recommendButton.Clicked += RecommendButton_Clicked;

            BackgroundColor = Color.White;

            Content = new StackLayout
            {
                Children = {
                    label,
                    new ScrollView
                    {
                        Content = new StackLayout
                        {
                            Children = { grid1, grid2 },
                            VerticalOptions = LayoutOptions.Fill
                        }
                    },
                    recommendButton
                }
            };
        }

        async private void RecommendButton_Clicked(object sender, EventArgs e)
        {
            List<int> criterias = new List<int>();
            foreach (var view in criteriaViews)
            {
                if (view.IsOn)
                    criterias.Add(view.CriteriaId);
            }

            recommendationEvent.Invoke(this, criterias.ToArray());
            await Navigation.PopModalAsync();
        }
    }
}
