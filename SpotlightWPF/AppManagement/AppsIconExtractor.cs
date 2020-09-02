using System.Drawing;

namespace Winspotlight.AppManagement
{
    public static class AppsIconExtractor
    {
        public static Icon GetAppIcon(string path)
        {
            return Icon.ExtractAssociatedIcon(path);
        }

        public static Bitmap GetAppIconBitmap(string path)
        {
            return Icon.ExtractAssociatedIcon(path).ToBitmap();
        }
    }
}
