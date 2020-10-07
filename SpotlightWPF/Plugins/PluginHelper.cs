using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace Winspotlight.Plugins
{
    public static class PluginHelper
    {
        /// <summary>
        /// Load image from application pack
        /// </summary>
        /// <param name="relativePath">Plugin folder relative path (<b>example:</b> /Images/icon.png)</param>
        public static Image LoadImage(string pluginName, string relativePath)
        {
            return Image.FromStream(Application.GetResourceStream(new System.Uri("pack://application:,,,/Plugins/Embedded/" + pluginName + "/" + relativePath)).Stream);
        }
    }
}
