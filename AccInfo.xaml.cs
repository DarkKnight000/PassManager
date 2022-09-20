using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PassManager
{
    /// <summary>
    /// Логика взаимодействия для AccInfo.xaml
    /// </summary>
    public partial class AccInfo : Window
    {
        public AccInfo()
        {
            InitializeComponent();

        }

        /*
        [System.ComponentModel.Bindable(true)]
        [Localizability(LocalizationCategory.Hyperlink)]
        public Uri NavigateUri { get; set; }
        */
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
            catch
            {
                Process.Start(new ProcessStartInfo(e.Uri.OriginalString));
            }
            //Process.Start("iexplore.exe", e.Uri.OriginalString);
            //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            
            //e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Hyperlink hyperl = new Hyperlink();
            //hyperl.NavigateUri = new Uri($"{Data.link}");

            hyperLinkText.Inlines.Add(Data.link);

            if (Data.link.Contains("http://") || Data.link.Contains("https://"))
            { }
            else
            {
                Data.link = $"http://{Data.link}";
            }

            try
            {
                hyperLinkText.NavigateUri = new Uri($"{Data.link}");
            }
            catch
            { }
        }
    }
}
