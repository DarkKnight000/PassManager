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
            //CodeConfirmGrid.Visibility = Visibility.Hidden;

            int N = Data.dt_positions.Rows.Count;
            for (int i = 1; i < N; i++)
            {
                comboBoxPosition.Items.Add(Data.dt_positions.Rows[i][1].ToString());
            }
        }

        private static int code;
        SolidColorBrush brush = new SolidColorBrush();
        Data data = new Data();
        Crypt crypto = new Crypt();

        // Регистрация:
        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            string error = "Введите: ";
            if (textBoxSur.Text == "")
            {
                error += "фамилию, ";
            }
            if (textBoxName.Text == "")
            {
                error += "имя, ";
            }
            if (comboBoxPosition.Text == "")
            {
                error += "должность, ";
            }
            if (textBoxLogin.Text == "")
            {
                error += "логин/никнейм, ";
            }
            if (textBoxEmail.Text == "")
            {
                error += "электронную почту, ";
            }
            if (PassTextBox.Password == "")
            {
                error += "пароль, ";
            }
            error = error.Remove(error.Length - 2);

            if (textBoxLogin.Text == "" || textBoxEmail.Text == "" || PassTextBox.Password == "")
            {
                MessageBox.Show(error);
            }
            else
            {
                if (PassTextBox.Password == passConfirmText.Password)
                {
                    string check_name = textBoxLogin.Text;
                    check_name = check_name.ToLower();
                    if (check_name == "admin" ||
                        check_name == "nimda" ||
                        check_name == "root" ||
                        check_name == "toor" ||
                        check_name == "qwerty")
                    {
                        MessageBox.Show($"Логин {textBoxLogin.Text} недоступен для регистрации");
                    }
                    else
                    {
                        Random rnd = new Random();
                        code = rnd.Next(100000, 999999);
                        code = 1;

                        Data.userNick = crypto.Encode(textBoxLogin.Text, Crypt.key);
                        Data.userEmail = crypto.Encode(textBoxEmail.Text, Crypt.key);
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
                             FROM users 
                             WHERE BINARY login='{Data.userNick}' OR BINARY email='{Data.userEmail}' LIMIT 1)";

            dt_user = data.Connect(Data.sqlcmd);
            if (Data.check_con == true)
            {
                if (dt_user.Rows[0][0].ToString() == "1")     // Если такая запись существует       
                {
                    //CodeConfirmGrid.Visibility = Visibility.Hidden;
                    MessageBox.Show("Пользователь с таким логином и/или эл.почтой уже существует");
                }
                else
                {
                    Data.userName = crypto.Encode(textBoxName.Text, Crypt.key);
                    Data.userSurname = crypto.Encode(textBoxSur.Text, Crypt.key);
                    Data.userPatronymic = crypto.Encode(textBoxPatr.Text, Crypt.key);
                    Data.userNick = crypto.Encode(textBoxLogin.Text, Crypt.key);
                    Data.userEmail = crypto.Encode(textBoxEmail.Text, Crypt.key);
                    Data.userPass = crypto.Encode(PassTextBox.Password, Crypt.key);

                    endReg();
                }
            }
        }

        // Оотправка сообщения на эл.почту:
        //private async Task SendEmail()
        private void SendEmail()
        {
            string myMail = "passmanager@bk.ru";
            string myPass = "BiqtT9LF9Wq28XYHsD5C";
            string name = "Менеджер паролей";
            string theme = "Код регистрации";

            Cursor = Cursors.Wait;
            try
            {
                // отправитель - устанавливаем адрес и отображаемое в письме имя
                MailAddress from = new MailAddress(myMail, name);
                // кому отправляем
                MailAddress to = new MailAddress(textBoxEmail.Text);
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

            }
            catch
            {
                Cursor = Cursors.Arrow;
                MessageBox.Show("Ошибка отправки сообщения\nПроверьте введённые данные\nили интернет соединение");
            }
        }

        private void endReg()
        {
            Data.sqlcmd = $@"INSERT INTO users
                                         (name, surname, patronymic, login, email, pass, id_position, last_access)
                                  SELECT '{Data.userName}', '{Data.userSurname}', '{Data.userPatronymic}', 
                                         '{Data.userNick}', '{Data.userEmail}', '{Data.userPass}', 
                                         `position`.id_position, NOW()
                                  FROM position
                                  WHERE position.position_name = '{comboBoxPosition.Text}'"
            ;
            MessageBox.Show(Data.sqlcmd);
            data.Connect(Data.sqlcmd);
            try
            {
                data.CheckIntConn();
                if (Data.check_con == true)
                {
                    //data.Connect(Data.sqlcmd);
                    MessageBox.Show("Поздравляем с успешной регистрацией");
                    //CodeConfirmGrid.Visibility = Visibility.Hidden;
                }
            }
            catch { }
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
            textBoxLogin.SelectionStart = textBoxLogin.Text.Length;
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
