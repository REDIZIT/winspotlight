using SpotlightWPF.Settings;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SpotlightWPF
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            

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
    }
}
