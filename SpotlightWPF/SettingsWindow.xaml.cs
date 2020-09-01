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
            themesBox.SelectedItem = "dark";

            themesBox.SelectionChanged += (s, e) => OnThemeChange();
        }

        private void OnThemeChange()
        {
            string style = themesBox.SelectedItem as string;
            // определяем путь к файлу ресурсов
            var uri = new Uri("Themes/" + style + ".xaml", UriKind.Relative);
            // загружаем словарь ресурсов
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            // очищаем коллекцию ресурсов приложения
            Application.Current.Resources.Clear();
            // добавляем загруженный словарь ресурсов
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }
}
