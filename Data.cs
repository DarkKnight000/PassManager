using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PassManager
{
    public class Data
    {
        // Аккаунты:
        public static string idAcc { get; set; }
        public static string title { get; set; }
        public static string link { get; set; }
        public static string nickname { get; set; }
        public static string email { get; set; }
        public static string pass { get; set; }
        public static string accDescription { get; set; }
        public static int accountsCount { get; set; }
        public static int accLastCode { get; set; }
        public static DataTable dt_acc;


        // Карты:
        public static string idCard { get; set; }
        public static string owner { get; set; }
        public static string bank { get; set; }
        public static string cardType { get; set; }
        public static string cardNumber { get; set; }
        public static string date { get; set; }
        public static string cvc { get; set; }
        public static string pin { get; set; }
        public static string cardDescription { get; set; }
        public static int cardsCount { get; set; }
        public static int cardsLastCode { get; set; }
        public static DataTable dt_cards;

        // Пользователь:
        public static int userId { get; set; }
        public static string userName { get; set; }
        public static string userEmail { get; set; }
        public static string userPass { get; set; }

        // Прочее:
        public static MySqlConnectionStringBuilder connection;

        public static string sqlcmd;
        public static bool check_con = false;
        public static DataTable dt_user;
        public static DataTable dt_banks;
        Crypt crypto = new Crypt();

        // Строка соединения:
        public void strcon()
        {
            connection = new MySqlConnectionStringBuilder()
            {
                Server = "f0586228.xsph.ru",
                Port = 3306,
                Database = "f0586228_test",
                UserID = "f0586228",
                Password = "deuzatihza"
            };
        }

        // Соединение с БД:
        public DataTable Connect(string selectSQL)
        {
            dt_user = new DataTable();

            //try
            //{
                MySqlConnection conn = new MySqlConnection(connection.ConnectionString);
                conn.Open();
                MySqlCommand sqlCommand = new MySqlCommand(sqlcmd, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(sqlCommand);
                da.Fill(dt_user);
                conn.Close();
                check_con = true;
            /*}
            catch
            {
                check_con = false;
                MessageBox.Show("Ошибка подключения!");
            }*/

            return dt_user;
        }

        // Вывод аккаунтов:
        public void show_accounts(DataGrid dataGrid1)
        {
            dt_acc = new DataTable();

            sqlcmd = $@"SELECT `Код`, `Заголовок`, `Ссылка`, `Логин(Ник)`, `Электронная почта`, `Пароль`, `Описание`
                        FROM Passwords
                        WHERE user_id = {userId}"
            ;
            dt_acc = Connect(sqlcmd);

            if (check_con == true)
            {
                if (dt_acc.Rows.Count == 0)
                {
                    accLastCode = 0;
                }
                else
                {
                    accLastCode = Convert.ToInt32(dt_acc.Rows[dt_acc.Rows.Count - 1][0]);
                }
                accountsCount = dt_acc.Rows.Count;

                crypto.Encrypt(accountsCount, 6);

                dataGrid1.ItemsSource = dt_acc.DefaultView;    // Сам вывод данных
            }

        }

        // Вывод карт:
        public void show_cards(DataGrid dataGrid2)
        {
            dt_cards = new DataTable();

            sqlcmd = $@"SELECT Cards.Код, Cards.`Владелец карты`, 
                        Banks.Банк, `Card type`.`Тип карты`, Cards.`Номер карты`, 
                        Cards.`Срок действия`, `CVC/CVV`.`CVC/CVV`, 
                        Pin.`Пин-код`, Cards.Описание
                        FROM Pin INNER JOIN 
                        (`CVC/CVV` INNER JOIN 
                        (`Card type` INNER JOIN 
                        (Banks INNER JOIN 
                        Cards ON Banks.id_bank = Cards.Банк) 
                        ON `Card type`.id_type = Cards.`Тип карты`) 
                        ON `CVC/CVV`.id_CVC = Cards.`CVC/CVV`) 
                        ON Pin.id_pin = Cards.`Пин-код`

                        WHERE user_id = {userId}"
            ;
            dt_cards = Connect(sqlcmd);

            if (check_con == true)
            {
                if (dt_cards.Rows.Count == 0)
                {
                    cardsLastCode = 0;
                }
                else
                {
                    cardsLastCode = Convert.ToInt32(dt_cards.Rows[dt_cards.Rows.Count - 1][0]);
                }
                cardsCount = dt_cards.Rows.Count;

                crypto.Encrypt(cardsCount, 8);

                dataGrid2.ItemsSource = dt_cards.DefaultView;    // Сам вывод данных
            }

        }

        // Список банков:
        public void GetBanks()
        {
            dt_banks = new DataTable();

            sqlcmd = $@"SELECT * 
                             FROM `Banks`"
            ;

            for (int i = 0; i < 1; i++)
            {
                if (check_con == true)
                {
                    dt_banks = Connect(sqlcmd);
                    break;
                }
                else
                {
                    CheckIntConn();
                }
                i--;
            }

            if (dt_banks.Rows.Count > 0)
            {
                check_con = true;

                Crypt crypt = new Crypt();
                for (int i = 0; i < dt_banks.Rows.Count; i++)
                {
                    dt_banks.Rows[i][1] = crypt.Decode(dt_banks.Rows[i][1].ToString(), Crypt.key);

                }
            }
        }

        // Проверка соединения:
        public void CheckIntConn()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.ru"))
                    {
                        check_con = true;
                    }
                }
            }
            catch
            {
                check_con = false;
                MessageBox.Show("Проверьте интернет подключение!");
            }

            //Thread.Sleep(1000);
            
        }


        // Проверка интернет подключения:
        /*public void CallCheck()
        {
            ThreadStart ts = new ThreadStart(show_cards);
            Thread thread = new Thread(ts);
            thread.Start();
        }*/
    }
}
