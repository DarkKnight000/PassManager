using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PassManager
{
    /// <summary>
    /// Логика взаимодействия для CardInfo.xaml
    /// </summary>
    public partial class CardInfo : Window
    {
        public CardInfo()
        {
            InitializeComponent();
        }

        // Перемещение:
        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        // Перемещение:
        private void Card_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        // Закрытие окна:
        private void image2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
