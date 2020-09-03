using Winspotlight.Extensions;
using Winspotlight.Hotkey;
using Winspotlight.Indexing;
using Winspotlight.Models;
using Winspotlight.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace Winspotlight
{
    /// <summary>
    /// Spotlight main window
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<SearchPresentUIItem> presentModels = new List<SearchPresentUIItem>();


        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = MathUtils.Clamp(value, 0, Math.Min(searcher.searchResults.Count, presentModels.Count) - 1); }
        }
        private int _selectedIndex;



        private readonly Searcher searcher;
        private readonly NotifyIcon trayIcon;



        public MainWindow()
        {
            InitializeComponent();

            ThemeManager.ApplyTheme(SettingsWrapper.Settings.SelectedTheme);
            searcher = new Searcher(this);

            // Create search slots
            for (int i = 0; i < 6; i++)
            {
                CreateItem();
            }
            presentModels[0].Select();


            HotkeyManager.Bind((s, e) => ShowWindow());
            ShowWindow();



            Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Winspotlight;component/Images/spotlight.ico")).Stream;
            trayIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon(iconStream),
                Visible = true
            };
            trayIcon.Click += (s, e) => ShowWindow();
        }

        public void CloseWindow()
        {
            trayIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }
        public void OpenSettings()
        {
            SettingsWindow window = new SettingsWindow(searcher);
            window.Show();
        }






        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searcher == null) return;

            searcher.Search(searchBox.Text);

            UpdateItems();
        }


        private void CreateItem()
        {
            presentModels.Add(new SearchPresentUIItem(ui, searcher));
        }
        private void UpdateItems()
        {
            float height = 60;
            for (int i = 0; i < Math.Min(searcher.searchResults.Count, presentModels.Count); i++)
            {
                height += 60;
                presentModels[i].Refresh(searcher.searchResults[i]);
                if(i == 0)
                {
                    presentModels[i].Select();
                }
            }
            Height = height;


            SelectedIndex = 0;
        }


        private void Window_Deactivated(object sender, EventArgs e)
        {
            HideWindow();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                if(presentModels.Count > 0)
                {
                    presentModels[SelectedIndex].Deselect();

                    SelectedIndex += e.Key == Key.Up ? -1 : 1;

                    presentModels[SelectedIndex].Select();
                }
            }

            if (e.Key == Key.Enter)
            {
                bool isShiftPressed = System.Windows.Input.Keyboard.IsKeyDown(Key.LeftShift) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightShift);

                // Needed before hiding
                int selected = SelectedIndex;
                HideWindow();

                presentModels[selected].item.Execute(isShiftPressed);
            }
        }

        private void ShowWindow()
        {
            searchBox.Text = "";
            Height = 60;
            Show();
            Activate();

            searchBox.Focus();

            // Change input language to english
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("en-US"));


            searcher.OnWindowShown();
        }

        private void HideWindow()
        {
            searchBox.Text = "";
            Hide();
        }
    }
}
