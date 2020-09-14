using Winspotlight.Indexing;
using Winspotlight.Settings;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Winspotlight.Utils;

namespace Winspotlight
{
    public partial class SettingsWindow : Window
    {
        private readonly Searcher searcher;
        private bool isRefreshing;

        public SettingsWindow(Searcher searcher)
        {
            InitializeComponent();
            this.searcher = searcher;

            isRefreshing = true;

            RefreshThemeBox();
            RefreshIndexIntervalsBox();
            RefreshIgnoreList();
            RefreshAutorun();

            isRefreshing = false;
        }

        private void RefreshThemeBox()
        {
            List<string> styles = new List<string> { "light", "dark" };
            themesBox.ItemsSource = styles;
            themesBox.SelectedItem = SettingsWrapper.Settings.SelectedTheme;

            themesBox.SelectionChanged += (s, e) =>
            {
                string style = themesBox.SelectedItem as string;
                ThemeManager.ApplyTheme(style);

                SettingsWrapper.Settings.SelectedTheme = style;
                SettingsWrapper.Save();

                RefreshIgnoreList();
            };
        }

        private void RefreshIndexIntervalsBox()
        {
            List<IntervalSelectionModel> intervalModels = new List<IntervalSelectionModel>()
            {
                new IntervalSelectionModel("1 minute", 1),
                new IntervalSelectionModel("2 minutes", 2),
                new IntervalSelectionModel("5 minutes", 5),
                new IntervalSelectionModel("10 minutes", 10),
                new IntervalSelectionModel("15 minutes", 15),
                new IntervalSelectionModel("30 minutes", 30),
                new IntervalSelectionModel("1 hour", 60),
                new IntervalSelectionModel("2 hours", 120),
                new IntervalSelectionModel("12 hours", 720),
                new IntervalSelectionModel("24 hours", 1440)
            };
            intevalBox.ItemsSource = intervalModels;
            intevalBox.SelectedItem = intervalModels.FirstOrDefault(c => c.Minutes == SettingsWrapper.Settings.IndexInterval);
            intevalBox.SelectionChanged += (s, e) =>
            {
                IntervalSelectionModel selectedInterval = intevalBox.SelectedItem as IntervalSelectionModel;

                SettingsWrapper.Settings.IndexInterval = selectedInterval.Minutes;
                SettingsWrapper.Save();

                searcher.UpdateTimerInterval();
            };
        }

        private void RefreshIgnoreList()
        {
            IgnoreStackPanel.Children.Clear();

            int i = -1;
            SolidColorBrush evenColor = new SolidColorBrush(Color.FromArgb(30, 100, 100, 100));
            SolidColorBrush oddColor = new SolidColorBrush(Color.FromArgb(30, 0, 0, 0));
            foreach (string item in SettingsWrapper.Settings.IgnoreList)
            {
                // Ignore syntax is {displayName}#{displaySubName}
                string displayName = item.Split('#')[0];
                string displaySubName = item.Split('#')[1];
                i++;




                // Create presenter item
                Grid grid = new Grid
                {
                    Height = 28
                };
                // Background rect
                Rectangle rect = new Rectangle()
                {
                    Height = 28,
                    Fill = i % 2 == 0 ? evenColor : oddColor
                };



                TextBlock nameText = new TextBlock()
                {
                    Text = displayName,
                    FontSize = 11,
                    Margin = new Thickness(14, 0, 0, 0),
                    Style = Application.Current.FindResource("TextBlockStyle") as Style,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Button button = new Button()
                {
                    Content = "Return",
                    FontSize = 10,
                    Height = 22,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Padding = new Thickness(10, 0, 10, 0),
                    Margin = new Thickness(0, 0, 18, 0)
                };
                button.Click += (s, e) =>
                {
                    searcher.RemoveFromIgnoreList(displayName, displaySubName);
                    RefreshIgnoreList();
                };


                grid.Children.Add(rect);
                grid.Children.Add(nameText);
                grid.Children.Add(button);


                IgnoreStackPanel.Children.Add(grid);
            }
        }


        private void RefreshAutorun()
        {
            bool isEnabled = ShortcutCreator.IsInAutorun();
            AutorunCheckbox.IsChecked = isEnabled;
        }

        private void AutorunCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (isRefreshing) return;
            ShortcutCreator.AddToAutorun();
        }

        private void AutorunCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (isRefreshing) return;
            ShortcutCreator.RemoveFromAutorun();
        }
    }
}
