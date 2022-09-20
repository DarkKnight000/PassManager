using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PassManager
{
    /// <summary>
    /// Логика взаимодействия для PassBlock.xaml
    /// </summary>
    public partial class PassBlock : Window
    {
        public PassBlock()
        {
            InitializeComponent();

        }
        public static bool close_check = false;
        private static bool close_any;

        private void buttonPassBlock_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password == Data.userPass)
            {
                close_any = true;
                Close();
                close_any = false;
            }
            else 
            if (passwordBox.Password == "")
            {
                MessageBox.Show("Введите пароль!");
            }
            else
            {
                MessageBox.Show("Неверный пароль!");
            }
        }

        // Закрытие приложения через 3 секунды
        public void button_Click(object sender, RoutedEventArgs e)
        {
            close_check = true;
            close_any = true;

            DispatcherTimer timer = new DispatcherTimer();
            TimeSpan time = TimeSpan.FromMilliseconds(4000);

            /*
            DispatcherTimer timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(10000);
            timer1.Tick += TimerTick;
            timer1.Start();
            */

            timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 1), DispatcherPriority.Normal, delegate
            {
                int t = int.Parse(time.ToString("ss"));
                button.Content = $"Закрытие приложения через {t-1}";
                if (time == TimeSpan.Zero)
                {
                    timer.Stop();
                    Close();
                }
                time = time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);

            //Close();
            //timer.Start();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (close_any == false)
            {
                e.Cancel = true;
            }
        }

        /*
        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;

            //Close();
        }
        */
    }
}
