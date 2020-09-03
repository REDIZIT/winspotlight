using System.IO;
using System.Windows;
using Winspotlight.Apps;
using Winspotlight.Indexing;
using Winspotlight.Models;

namespace Winspotlight
{
    /// <summary>
    /// Логика взаимодействия для InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        private SearchPresentUIItem presenter;
        private Searcher searcher;

        public InfoWindow(SearchPresentUIItem presenter, Searcher searcher)
        {
            InitializeComponent();

            this.presenter = presenter;
            this.searcher = searcher;

            IconImage.Source = presenter.image.Source;
            DisplayNameText.Text = presenter.item.displayName;

            SubNameText.Text = presenter.item.displaySubName + (presenter.item.sourcePlugin != null ? $" (from {presenter.item.sourcePlugin.GetName()} plugin)" : "");


            if (presenter.item is SearchFileItem)
            {
                SearchFileItem item = presenter.item as SearchFileItem;
                bool isShortcut = Path.GetExtension(item.path) == ".lnk";

                ShortcutFilePathText.Visibility = isShortcut ? Visibility.Visible : Visibility.Collapsed;
                ShortcutFilePathLabel.Visibility = isShortcut ? Visibility.Visible : Visibility.Collapsed;

                ExeFilePathText.Visibility = Visibility.Visible;
                ExeFilePathLabel.Visibility = Visibility.Visible;
                

                if (isShortcut)
                {
                    ShortcutFilePathText.Text = item.path;

                    string targetPath = AppsLauncher.GetLinkTargetPath(item.path); ;
                    ExeFilePathText.Text = targetPath;
                    ExeFilePathLabel.Text = "Target file path";
                }
                else
                {
                    ExeFilePathLabel.Text = Path.GetExtension(item.path) + " file path";
                    ExeFilePathText.Text = item.path;
                }
            }
            else
            {
                ShortcutFilePathText.Visibility = Visibility.Collapsed;
                ShortcutFilePathLabel.Visibility = Visibility.Collapsed;

                ExeFilePathText.Visibility = Visibility.Collapsed;
                ExeFilePathLabel.Visibility = Visibility.Collapsed;
            }
            
        }

        private void AddToIgnoreListBtn_Click(object sender, RoutedEventArgs e)
        {
            searcher.AddToIgnoreList(presenter.item);
        }

        private void RemoveFromIgnoreListBtn_Click(object sender, RoutedEventArgs e)
        {
            searcher.RemoveFromIgnoreList(presenter.item);
        }
    }
}
