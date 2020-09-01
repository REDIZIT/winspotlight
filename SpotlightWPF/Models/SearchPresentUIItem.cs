using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotlightWPF.Models
{
    public class SearchPresentUIItem
    {
        public SearchItem item;

        private Rectangle panel;
        private TextBlock nameText, subNameText;
        private Image image, infoImage;

        //private readonly Brush defaultNameBrush;


        public SearchPresentUIItem(StackPanel parent)
        {
            Grid grid = new Grid();
            grid.Height = 60;


            // Background rect
            Rectangle rect = new Rectangle()
            {
                //Width = Width,
                Height = 60,
                Fill = Brushes.Transparent
            };
            panel = rect;




            TextBlock nameText = new TextBlock()
            {
                Text = "Name",
                Margin = new Thickness(70, 8, 0, 0),
                FontSize = 20,
                Style = Application.Current.FindResource("TextBlockStyle") as Style
            };
            this.nameText = nameText;
            //defaultNameBrush = nameText.Foreground;

            TextBlock subText = new TextBlock()
            {
                Text = "Window app",
                Margin = new Thickness(70, 32, 0, 0),
                FontSize = 12,
                Style = Application.Current.FindResource("TextBlockStyle") as Style
            };
            subNameText = subText;



            Image image = new Image()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 40,
                Height = 40,
                Margin = new Thickness(18, 5, 5, 5)
            };
            this.image = image;


            // Cover image
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(@"Images/info.png", UriKind.Relative);
            bitmap.EndInit();


            infoImage = new Image()
            {
                Source = bitmap,
                Width = 20,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 20, 0)
            };
            infoImage.AddHandler(Image.MouseDownEvent, new RoutedEventHandler(ShowInfo));


            grid.Children.Add(rect);
            grid.Children.Add(nameText);
            grid.Children.Add(subText);
            grid.Children.Add(image);
            grid.Children.Add(infoImage);


            parent.Children.Add(grid);
        }

        public void Refresh(SearchItem item)
        {
            this.item = item;

            panel.Fill = Brushes.Transparent;
            nameText.Text = item.displayName;
            subNameText.Text = item.displaySubName;

            nameText.Style = Application.Current.FindResource("TextBlockStyle") as Style;
            subNameText.Style = Application.Current.FindResource("TextBlockStyle") as Style;

            BitmapImage bitmapImage = BitmapToImageSource(item.iconBitmap);
            image.Source = bitmapImage;
        }


        public void Select()
        {
            panel.Fill = new SolidColorBrush(Color.FromRgb(0, 128, 255));
            //subNameText.Foreground = new SolidColorBrush(Color.FromRgb(45, 45, 128));
            subNameText.Style = Application.Current.FindResource("TextBlockSelectedStyle") as Style;
            nameText.Style = Application.Current.FindResource("TextBlockSelectedStyle") as Style;
        }
        public void Deselect()
        {
            panel.Fill = Brushes.Transparent;
            nameText.Style = Application.Current.FindResource("TextBlockStyle") as Style;
            subNameText.Style = Application.Current.FindResource("TextBlockStyle") as Style;
            //subNameText.Foreground = Brushes.Gray;
        }

        public void ShowInfo(object s, RoutedEventArgs e)
        {
            MessageBox.Show("Show info");
        }


        BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            if (bitmap == null) return null;

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
