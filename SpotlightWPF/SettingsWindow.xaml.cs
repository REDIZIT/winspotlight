using SpotlightWPF.Indexing;
using SpotlightWPF.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SpotlightWPF
{
    public partial class SettingsWindow : Window
    {
        private readonly Searcher searcher;

        public SettingsWindow(Searcher searcher)
        {
            InitializeComponent();
            this.searcher = searcher;

            RefreshThemeBox();
            RefreshIndexIntervalsBox();
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
    }
}
