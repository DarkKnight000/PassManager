using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Net.Mail;
using System.Data;

namespace PassManager
{
    /// <summary>
    /// Логика взаимодействия для Reg.xaml
    /// </summary>
    public partial class Reg : Window
    {
        public Reg()
        {
            InitializeComponent();
            CodeConfirmGrid.Visibility = Visibility.Hidden;
        }

        private static int code;
        SolidColorBrush brush = new SolidColorBrush();
        Data data = new Data();
        Crypt crypto = new Crypt();

        // Регистрация:
        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            string error = "Введите: ";
            if (textBox.Text == "")
            {
                error += "логин/никнейм, ";
            }
            if (textBox2.Text == "")
            {
                error += "электронную почту, ";
            }
            if (PassTextBox.Password == "")
            {
                error += "пароль, ";
            }
            error = error.Remove(error.Length - 2);

            if (textBox.Text == "" || textBox2.Text == "" || PassTextBox.Password == "")
            {
                MessageBox.Show(error);
            }
            else
            {
                if (PassTextBox.Password == passConfirmText.Password)
                {
                    string check_name = textBox.Text;
                    check_name = check_name.ToLower();
                    if (check_name == "admin" ||
                        check_name == "nimda" ||
                        check_name == "root" ||
                        check_name == "toor" ||
                        check_name == "qwerty")
                    {
                        MessageBox.Show($"Логин {textBox.Text} недоступен для регистрации");
                    }
                    else
                    {
                        Random rnd = new Random();
                        code = rnd.Next(100000, 999999);

                        Data.userName = crypto.Encode(textBox.Text, Crypt.key);
                        Data.userEmail = crypto.Encode(textBox2.Text, Crypt.key);
                        Data.userPass = crypto.Encode(PassTextBox.Password, Crypt.key);

                        check_user();       // Проверка наличия пользователя в бд

                        //SendEmail().GetAwaiter();   // Отправка сообщения
                    }
                }
                else
                {
                    MessageBox.Show("Пароли не совпадают!");
                }

            }
        }

        // Проверка зарегистрированного пользователя:
        private void check_user()
        {
            DataTable dt_user;
            Data.sqlcmd = $@"SELECT EXISTS 
                            (SELECT * 
                             FROM f0586228_test.Users 
                             WHERE BINARY login='{Data.userName}' OR BINARY email='{Data.userEmail}' LIMIT 1)";

            dt_user = data.Connect(Data.sqlcmd);
            if (Data.check_con == true)
            {
                if (dt_user.Rows[0][0].ToString() == "1")     // Если такая запись существует       
                {
                    CodeConfirmGrid.Visibility = Visibility.Hidden;
                    MessageBox.Show("Пользователь с таким логином и/или эл.почтой уже существует");
                }
                else
                {
                    //SendEmail().GetAwaiter();
                    SendEmail();
                }
            }
        }

        // Оотправка сообщения на эл.почту:
        //private async Task SendEmail()
        private void SendEmail()
        {
            string myMail = "passmanager@bk.ru";
            string myPass = "CdwxNc97K3AdQNNkTF38";
            string name = "Менеджер паролей";
            string theme = "Код регистрации";

            Cursor = Cursors.Wait;
            //try
            //{
                // отправитель - устанавливаем адрес и отображаемое в письме имя
                MailAddress from = new MailAddress(myMail, name);
                // кому отправляем
                MailAddress to = new MailAddress(textBox2.Text);
                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = theme;
                // текст письма
                m.Body = $"Код регистрации: {code}";
                // письмо представляет код html
                //m.IsBodyHtml = true;

                // адрес smtp-сервера и порт, с которого будем отправлять письмо
                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 25);
                // логин и пароль
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(myMail, myPass);
                smtp.Send(m);
                //await smtp.SendMailAsync(m);

                Cursor = Cursors.Arrow;

                MessageBox.Show("Сообщение успешно отправлено\nПроверьте свою почту");

                CodeConfirmGrid.Visibility = Visibility.Visible;
            /*}
            catch
            {
                Cursor = Cursors.Arrow;
                MessageBox.Show("Ошибка отправки сообщения\nПроверьте введённые данные\nили интернет соединение");
            }*/
        }

        // Завершение регистрации:
        private void EndRegButton_Click(object sender, RoutedEventArgs e)
        {
            if (textBox5.Text == code.ToString())
            {
                Data.sqlcmd = $@"INSERT INTO Users
                                         (login, email, pass, online)
                                  VALUES ('{Data.userName}', '{Data.userEmail}', 
                                         '{Data.userPass}', 0)"
                ;
                try
                {
                    data.CheckIntConn();
                    if (Data.check_con == true)
                    {
                        data.Connect(Data.sqlcmd);
                        MessageBox.Show("Поздравляем с успешной регистрацией");
                        CodeConfirmGrid.Visibility = Visibility.Hidden;
                    }
                }
                catch { }

            }
            else
            {
                MessageBox.Show("Неверный код доступа");
            }
        }

        // Кнопка Назад:
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Запрет пробела в логине:
        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).Text = Regex.Replace((sender as TextBox).Text, @"\s+", "");
            textBox.SelectionStart = textBox.Text.Length;
        }

        // Проверка подтверждения пароля:
        private void passConfirmText_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PassTextBox.Password != passConfirmText.Password)
            {
                passCheck.Visibility = Visibility.Visible;
                brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                passConfirmText.BorderBrush = brush;
            }
            else
            {
                passCheck.Visibility = Visibility.Hidden;
                brush = new SolidColorBrush(Color.FromRgb(0, 120, 215));
                passConfirmText.BorderBrush = brush;
            }
        }
        private void PassTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passConfirmText_PasswordChanged(null, null);
        }

    }
}
