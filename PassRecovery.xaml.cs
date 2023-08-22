using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
    /// Логика взаимодействия для PassRecovery.xaml
    /// </summary>
    public partial class PassRecovery : Window
    {
        public PassRecovery()
        {
            InitializeComponent();

            GridCode.Visibility = Visibility.Hidden;
            GridPass.Visibility = Visibility.Hidden;
        }

        private int code;
        private string userMail;
        private string userMail_enc;
        private string userNewPass;
        SolidColorBrush brush = new SolidColorBrush();
        Data data = new Data();
        Crypt crypto = new Crypt();


        // Кнопка Получить код:
        private void RecButton_Click(object sender, RoutedEventArgs e)
        {
            userMail = textBoxLogMail.Text;
            userMail_enc = crypto.Encode(userMail, Crypt.key);
            Data.sqlcmd = $@"SELECT * FROM Users 
                             WHERE BINARY login = '{userMail_enc}' OR BINARY email = '{userMail_enc}'"
            ;
            Data.dt_user = data.Connect(Data.sqlcmd);

            if (Data.check_con == true)
            {
                if (Data.dt_user.Rows.Count > 0)     // Если такая запись существует
                {
                    userMail = Data.dt_user.Rows[0][5].ToString();
                    userMail = crypto.Decode(userMail, Crypt.key);
                    //crypto.Encrypt(1, 3);
                    //crypto.Encrypt(Data.dt_user, 1, 5);

                    //userMail = Data.dt_user.Rows[0][5].ToString();

                    //MessageBox.Show($"{userMail}");
                    SendRecovery();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким логином или эл.почтой не существует");
                }
            }
        }

        // Отправка кода восстановления на эл.почту:
        private void SendRecovery()
        {
            /*string myMail = "passman.recovery@bk.ru";
            string myPass = "nVRx25iwfiqp1pYYktRZ";
            string name = "Менеджер паролей";
            string theme = "Восстановление пароля";*/

            string myMail = "passmanager@bk.ru";
            string myPass = "c06DXxXuHZYS7GuLXsXJ";
            string name = "Менеджер паролей";
            string theme = "Восстановление пароля";


            Random rnd = new Random();
            code = rnd.Next(100000, 999999);
            try
            {
                // отправитель - устанавливаем адрес и отображаемое в письме имя
                MailAddress from = new MailAddress(myMail, name);
                // кому отправляем
                MailAddress to = new MailAddress(userMail);
                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = theme;
                // текст письма
                m.Body = $"Код восстановления: {code}";
                // письмо представляет код html
                //m.IsBodyHtml = true;

                // адрес smtp-сервера и порт, с которого будем отправлять письмо
                SmtpClient smtp = new SmtpClient("smtp.bk.ru", 587);
                // логин и пароль
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(myMail, myPass);
                smtp.Send(m);

                //Cursor = Cursors.Arrow;

                //MessageBox.Show(userMail);
                MessageBox.Show("Сообщение успешно отправлено\nПроверьте свою почту");
                GridCode.Visibility = Visibility.Visible;
            }
            catch
            {
                //Cursor = Cursors.Arrow;
                MessageBox.Show("Ошибка отправки сообщения\nПроверьте введённые данные\nили интернет соединение");
            }
        }

        // Кнопка Проверить (код):
        private void CodeButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxCode.Text == code.ToString())
            {
                GridPass.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Неверный код!");
            }
        }

        // Кнопка Восстановить пароль:
        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            userNewPass = crypto.Encode(passBox.Password, Crypt.key);
            Data.sqlcmd = $@"UPDATE Users
                             SET `pass` = '{userNewPass}'
                             WHERE `user_id` = '{Data.dt_user.Rows[0][0]}';"
            ;
            try
            {
                data.Connect(Data.sqlcmd);

                if (Data.check_con == true)
                {
                    MessageBox.Show("Ваш пароль изменён");
                    textBoxLogMail.Text = "";
                    GridCode.Visibility = Visibility.Hidden;
                    GridPass.Visibility = Visibility.Hidden;
                }
            }
            catch { }

        }

        // Проверка введённого пароля:
        private void passwordBoxRec_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passBox.Password != passwordBoxRec.Password)
            {
                passCheck.Visibility = Visibility.Visible;
                brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                passwordBoxRec.BorderBrush = brush;
            }
            else
            {
                passCheck.Visibility = Visibility.Hidden;
                brush = new SolidColorBrush(Color.FromRgb(0, 120, 215));
                passwordBoxRec.BorderBrush = brush;
            }
        }
        private void passBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordBoxRec_PasswordChanged(null, null);
        }


        // Закрыть:
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
