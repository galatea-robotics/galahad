using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Galahad
{
    static class Extension
    {
        public static void ScrollToBottom(this TextBox textBox)
        {
            try
            {
                var grid = (Grid)VisualTreeHelper.GetChild(textBox, 0);
                for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
                {
                    ScrollViewer obj = VisualTreeHelper.GetChild(grid, i) as ScrollViewer;
                    if (!(obj is ScrollViewer)) continue;
                    obj.ChangeView(0.0f, obj.ExtentHeight, 1.0f, true);
                    break;
                }
                textBox.Select(textBox.Text.Length, 0);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}
