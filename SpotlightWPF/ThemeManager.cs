using System;
using System.Windows;

namespace Winspotlight
{
    public static class ThemeManager
    {
        public static void ApplyTheme(string styleName)
        {
            // определяем путь к файлу ресурсов
            var uri = new Uri("Themes/" + styleName + ".xaml", UriKind.Relative);
            // загружаем словарь ресурсов
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            // очищаем коллекцию ресурсов приложения
            Application.Current.Resources.Clear();
            // добавляем загруженный словарь ресурсов
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }
}
