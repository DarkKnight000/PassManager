using System;
using System.Collections.Generic;
using System.Data;
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
using System.Threading;
using System.Net;

namespace PassManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            data.strcon();

            //data.CallCheck();
        }

        Data data = new Data();
        Crypt crypto = new Crypt();

        private ThreadStart ts;
        private Thread thread;

        // Регистрация:
        private void textBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Reg reg = new Reg();
            reg.ShowDialog();
        }

        // Восстановление пароля:
        private void textBlockRecov_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PassRecovery passrec = new PassRecovery();
            passrec.ShowDialog();
        }

        // Кнопка Войти:
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cursorLoad();

            Data.dt_user = new DataTable();

            if (passBoxLog.Text != "" && passBoxPass.Password != "")
            {
                check_authentification();
            }
            else
            {
                MessageBox.Show("Введите логин и/или пароль!");
            }

            cursorUnLoad();
        }

        // Проверка наличия пользователя:
        private void check_authentification()
        {
            Data.userName = crypto.Encode(passBoxLog.Text, Crypt.key);
            Data.userPass = crypto.Encode(passBoxPass.Password, Crypt.key);

            if (passBoxLog.Text.Contains('@'))
            {
                Data.sqlcmd = $@"SELECT * 
                                 FROM Users123 
                                 WHERE BINARY email = '{Data.userName}' 
                                       AND BINARY pass = '{Data.userPass}';"
                ;
            }
            else
            {
                Data.sqlcmd = $@"SELECT * 
                                 FROM Users123 
                                 WHERE BINARY login = '{Data.userName}' 
                                       AND BINARY pass = '{Data.userPass}';"
                ;
            }

            // Ищем в базе данных пользователя с такими данными:
            Data.dt_user = data.Connect(Data.sqlcmd);
            if (Data.check_con == true)
            {
                if (Data.dt_user.Rows.Count > 0)     // Если такая запись существует       
                {
                    Data.userId = Convert.ToInt32(Data.dt_user.Rows[0][0]);
                    Data.userName = crypto.Decode(Data.dt_user.Rows[0][1].ToString(), Crypt.key);
                    Data.userPass = crypto.Decode(Data.dt_user.Rows[0][3].ToString(), Crypt.key);

                    Passes passes = new Passes();
                    this.Close();
                    passes.Show();

                    /*
                    Data.sqlcmd = $@"SELECT * 
                                     FROM `Users` 
                                     WHERE user_id = {Data.userId} AND `last_access` > NOW() - 60"
                    ;
                    Data.dt_user = data.Connect(Data.sqlcmd);

                    if (Data.dt_user.Rows.Count < 1)
                    {
                        Data.sqlcmd = $@"UPDATE Users
                                         SET last_access = NOW(),
                                             online = 1
                                         WHERE user_id = {Data.userId}"
                        ;
                        Data.dt_user = data.Connect(Data.sqlcmd);

                        Passes passes = new Passes();
                        this.Close();
                        passes.Show();
                    }
                    else
                    {
                        MessageBox.Show("Аккаунт используется");
                    }
                    */
                }
                else
                {
                    MessageBox.Show("Неверный логин и/или пароль!");
                }
            }
        }

        // Смена курсора мыши при загрузке:
        private void cursorLoad()
        {
            Cursor = Cursors.Wait;
            buttonEnter.Cursor = Cursors.Wait;
            passBoxLog.Cursor = Cursors.Wait;
            passBoxPass.Cursor = Cursors.Wait;
            textBlock.Cursor = Cursors.Wait;
        }

        // Смена курсора мыши при работе:
        private void cursorUnLoad()
        {
            Cursor = Cursors.Arrow;
            buttonEnter.Cursor = Cursors.Hand;
            passBoxLog.Cursor = Cursors.IBeam;
            passBoxPass.Cursor = Cursors.IBeam;
            textBlock.Cursor = Cursors.Hand;
        }

        // Проверка интернет соединения:
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ts = new ThreadStart(Load);
            thread = new Thread(ts);

            thread.Start();
            
        }

        // Загрузка списка банков:
        private void Load()
        {
            data.CheckIntConn();
            while (true)
            {
                if (Data.check_con == true)
                {
                    data.GetBanks();
                    break;
                }
                else
                {
                    data.CheckIntConn();
                }
            }
        }

        // Закрытие окна, если не произошло подключение:
        private void Window_Closed(object sender, EventArgs e)
        {
            //thread.Abort();
            if (Data.check_con == false)
            {
                Environment.Exit(0);
            }
        }
    }
}
