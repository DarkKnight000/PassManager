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
using System.Windows.Markup;

namespace PassManager
{
    public class Data
    {

        public static DataTable dt_acc;
        public static int accountsCount { get; set; }
        public class DataPass
        {
            // Аккаунты:
            public string idPass { get; set; }
            public string idUser { get; set; }
            public string title { get; set; }
            public string link { get; set; }
            public string nickname { get; set; }
            public string email { get; set; }
            public string pass { get; set; }
            public string accDescription { get; set; }
            public string isPrivate { get; set; }

        }

        // Карты:
        public static int cardsCount { get; set; }
        public static int cardsLastCode { get; set; }
        public static DataTable dt_cards;
        public class DataCard
        {
            public string idCard { get; set; }
            public string idUser { get; set; }
            public string owner { get; set; }
            public string bank { get; set; }
            public string cardType { get; set; }
            public string cardNumber { get; set; }
            public string date { get; set; }
            public string cvc { get; set; }
            public string pin { get; set; }
            public string cardDescription { get; set; }
            public string isPrivate { get; set; }
        }

        // Пользователь:
        public static int userId { get; set; }
        public static string userNick { get; set; }
        public static string userName { get; set; }
        public static string userSurname { get; set; }
        public static string userPatronymic { get; set; }
        public static string userEmail { get; set; }
        public static string userPass { get; set; }
        public static string userPosition { get; set; }

        // Прочее:
        public static MySqlConnectionStringBuilder connection;

        public static string sqlcmd;
        public static bool check_con = false;
        public static DataTable dt_user;
        public static DataTable dt_banks;
        public static DataTable dt_positions;
        public static bool dir = false;
        public static int myInd;
        public static string myPos;
        public static string myName;
        public static string mySurname;
        public static string myPatronymic;
        Crypt crypto = new Crypt();

        // Строка соединения:
        private void strconnection()
        {
            connection = new MySqlConnectionStringBuilder()
            {
                Server = "mysql.j74236113.myjino.ru",
                Port = 3306,
                Database = "j74236113",
                UserID = "j74236113",
                Password = "deuzatihza"
            };
        }
        public void strcon()
        {
            strconnection();
        }

        // Соединение с БД:
        public DataTable Connect(string selectSQL)
        {
            dt_user = new DataTable();

            try
            {
                MySqlConnection conn = new MySqlConnection(connection.ConnectionString);
                conn.Open();
                MySqlCommand sqlCommand = new MySqlCommand(sqlcmd, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(sqlCommand);
                da.Fill(dt_user);
                conn.Close();
                check_con = true;
            }
            catch
            {
                check_con = false;
                MessageBox.Show("Ошибка подключения!");
            }

            return dt_user;
        }

        // Вывод аккаунтов:
        public void show_accounts(DataGrid dataGridWork, DataGrid dataGridPrivate)
        {
            Data.dt_user = new DataTable();

            sqlcmd = $@"SELECT *
                        FROM passwords
                        WHERE user_id = {userId}"
            ;
            Data.dt_user = Connect(sqlcmd);

            if (check_con == true)
            {
                dataGridWork.Items.Clear();
                dataGridPrivate.Items.Clear();
                Data.accountsCount = Data.dt_user.Rows.Count;

                //crypto.Encrypt(Data.accountsCount, 7);
                crypto.Encrypt(Data.dt_user, Data.accountsCount, 7);

                for (int i = 0; i < Data.dt_user.Rows.Count; i++)
                {
                    DataPass data = new DataPass()
                    {
                        idPass = Convert.ToString(Data.dt_user.Rows[i][0]),
                        idUser = Convert.ToString(Data.dt_user.Rows[i][1]),
                        title = Convert.ToString(Data.dt_user.Rows[i][2]),
                        link = Convert.ToString(Data.dt_user.Rows[i][3]),
                        nickname = Convert.ToString(Data.dt_user.Rows[i][4]),
                        email = Convert.ToString(Data.dt_user.Rows[i][5]),
                        pass = Convert.ToString(Data.dt_user.Rows[i][6]),
                        accDescription = Convert.ToString(Data.dt_user.Rows[i][7]),
                        isPrivate = Convert.ToString(Data.dt_user.Rows[i][8])
                    };

                    if (data.isPrivate == "False")
                    {
                        dataGridWork.Items.Add(data);
                    }
                    else
                    {
                        dataGridPrivate.Items.Add(data);
                    }
                }

                //dataGrid1.ItemsSource = dt_acc.DefaultView;    // Сам вывод данных
            }

        }

        // Вывод карт:
        public void show_cards(DataGrid dataGridWork, DataGrid dataGridPrivate)
        {
            dt_cards = new DataTable();

            sqlcmd = $@"SELECT `cards`.`card_id`, `cards`.`user_id`, `cards`.owner, `banks`.`Банк`, `card type`.`Тип карты`, 
                               `cards`.number, `cards`.date, `cards`.cvc, `cards`.pin, `cards`.description, `cards`.private
                        FROM `cards` 
	                    LEFT JOIN `banks` ON `cards`.bank = `banks`.`id_bank` 
	                    LEFT JOIN `card type` ON `cards`.type = `card type`.`id_type`
                        WHERE user_id = {userId};"
            ;

            dt_cards = Connect(sqlcmd);

            if (check_con == true)
            {
                dataGridWork.Items.Clear();
                dataGridPrivate.Items.Clear();
                if (dt_cards.Rows.Count == 0)
                {
                    cardsLastCode = 0;
                }
                else
                {
                    cardsLastCode = Convert.ToInt32(dt_cards.Rows[dt_cards.Rows.Count - 1][0]);
                }
                cardsCount = dt_cards.Rows.Count;

                //crypto.Encrypt(cardsCount, 8);
                crypto.Encrypt(Data.dt_user, cardsCount, 8);

                for (int i = 0; i < Data.dt_user.Rows.Count; i++)
                {
                    DataCard dataCard = new DataCard()
                    {
                        idCard = Convert.ToString(Data.dt_user.Rows[i][0]),
                        idUser = Convert.ToString(Data.dt_user.Rows[i][1]),
                        owner = Convert.ToString(Data.dt_user.Rows[i][2]),
                        bank = Convert.ToString(Data.dt_user.Rows[i][3]),
                        cardType = Convert.ToString(Data.dt_user.Rows[i][4]),
                        cardNumber = Convert.ToString(Data.dt_user.Rows[i][5]),
                        date = Convert.ToString(Data.dt_user.Rows[i][6]),
                        cvc = Convert.ToString(Data.dt_user.Rows[i][7]),
                        pin = Convert.ToString(Data.dt_user.Rows[i][8]),
                        cardDescription = Convert.ToString(Data.dt_user.Rows[i][9]),
                        isPrivate = Convert.ToString(Data.dt_user.Rows[i][10])
                    };

                    if (dataCard.isPrivate == "False")
                    {
                        dataGridWork.Items.Add(dataCard);
                    }
                    else
                    {
                        dataGridPrivate.Items.Add(dataCard);
                    }
                }
            }

        }

        // Список банков:
        public void GetBanks()
        {
            dt_banks = new DataTable();

            sqlcmd = $@"SELECT * 
                             FROM Banks"
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


        public void GetPositions()
        {
            sqlcmd = $@"SELECT *
                        FROM position"
            ;
            dt_positions = Connect(sqlcmd);
        }
        // Проверка соединения:
        public void CheckIntConn()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.yandex.ru"))
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
