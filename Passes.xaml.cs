using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static PassManager.Data;

namespace PassManager
{
    /// <summary>
    /// Логика взаимодействия для Passes.xaml
    /// </summary>
    public partial class Passes : Window
    {
        Data data = new Data();
        DataPass dataPass = new DataPass();
        DataCard dataCard = new DataCard();
        Crypt crypto = new Crypt();
        private static bool check_acc = true;
        private static bool check_card = true;
        private static string paySystem;
        private static bool block;
        private static bool isAccPrivate = false;
        private static bool isCardPrivate = false;
        int _ind;   // Индекс строки

        int[] staff;    // Сотрудники

        public static DataTable dt;
        public Passes()
        {
            InitializeComponent();

            ShowAll();

        }

        #region Аккаунты
        // Кнопка Добавить/Изменить:
        private void button_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            data.CheckIntConn();
            if (Data.check_con == true)
            {

                if (check_acc == true)
                {
                    dataPass = new DataPass()
                    {
                        title = crypto.Encode(textBox.Text, Crypt.key),
                        link = crypto.Encode(textBox1.Text, Crypt.key),
                        nickname = crypto.Encode(textBox2.Text, Crypt.key),
                        email = crypto.Encode(textBox3.Text, Crypt.key),
                        pass = crypto.Encode(passwordBox.Password, Crypt.key),
                        accDescription = crypto.Encode(textBox4.Text, Crypt.key)
                    };

                    if (checkBoxAccPrivate.IsChecked == true)
                    {
                        Data.sqlcmd = $@"INSERT INTO Passwords
                                     (`user_id`, `title`, `link`, `nick`, 
                                      `Email`, `password`, `description`, `private`)
                                       VALUES ({Data.userId}, '{dataPass.title}', '{dataPass.link}', 
                                            '{dataPass.nickname}', '{dataPass.email}', '{dataPass.pass}', '{dataPass.accDescription}', 1);
                                       SELECT LAST_INSERT_ID()"
                        ;
                    }
                    else
                    {
                        Data.sqlcmd = $@"INSERT INTO Passwords
                                     (`user_id`, `title`, `link`, `nick`, 
                                      `Email`, `password`, `description`, `private`)
                                       VALUES ({Data.userId}, '{dataPass.title}', '{dataPass.link}', 
                                            '{dataPass.nickname}', '{dataPass.email}', '{dataPass.pass}', '{dataPass.accDescription}', 0);
                                       SELECT LAST_INSERT_ID()"
                        ;
                    }

                    data.Connect(Data.sqlcmd);

                    if (Data.check_con == true)
                    {
                        dataPass = new DataPass()
                        {
                            idPass = Data.dt_user.Rows[0][0].ToString(),
                            title = textBox.Text,
                            link = textBox1.Text,
                            nickname = textBox2.Text,
                            email = textBox3.Text,
                            pass = passwordBox.Password,
                            accDescription = textBox4.Text
                        };

                        if (checkBoxAccPrivate.IsChecked == true)
                        {
                            dataPass.isPrivate = "True";
                            dataGridAccPrivate.Items.Add(dataPass);
                        }
                        else
                        {
                            dataPass.isPrivate = "False";
                            dataGridAccWork.Items.Add(dataPass);
                        }
                        MessageBox.Show("Запись успешно добавлена!");
                    }

                }
                if (check_acc == false)
                {
                    string ind = dataPass.idPass;
                    dataPass = new DataPass()
                    {
                        idPass = ind,
                        title = crypto.Encode(textBox.Text, Crypt.key),
                        link = crypto.Encode(textBox1.Text, Crypt.key),
                        nickname = crypto.Encode(textBox2.Text, Crypt.key),
                        email = crypto.Encode(textBox3.Text, Crypt.key),
                        pass = crypto.Encode(passwordBox.Password, Crypt.key),
                        accDescription = crypto.Encode(textBox4.Text, Crypt.key),
                        isPrivate = checkBoxAccPrivate.ToString()
                    };

                    Data.sqlcmd = $@"UPDATE Passwords
                                     SET `title` = '{dataPass.title}', `link` = '{dataPass.link}', 
                                         `nick` = '{dataPass.nickname}', `Email` = '{dataPass.email}', 
                                         `password` = '{dataPass.pass}', `description` = '{dataPass.accDescription}',
                                         `private` = {checkBoxAccPrivate.IsChecked}
                                     WHERE `user_id` = {Data.userId} AND `pass_id` = {dataPass.idPass}"
                    ;

                    MessageBox.Show(Data.sqlcmd);

                    data.Connect(Data.sqlcmd);
                    if (Data.check_con == true)
                    {
                        dataPass = new DataPass()
                        {
                            idPass = ind,
                            title = textBox.Text,
                            link = textBox1.Text,
                            nickname = textBox2.Text,
                            email = textBox3.Text,
                            pass = passwordBox.Password,
                            accDescription = textBox4.Text
                        };


                        if (checkBoxAccPrivate.IsChecked == true)
                        {
                            dataPass.isPrivate = "True";
                            try
                            {
                                dataGridAccPrivate.Items[_ind] = dataPass;
                            }
                            catch { }
                        }
                        else
                        {
                            dataPass.isPrivate = "False";
                            try
                            {
                                dataGridAccWork.Items[_ind] = dataPass;
                            }
                            catch { }
                        }

                        MessageBox.Show("Запись успешно изменена!");


                        check_acc = true;
                        AddAccButton.Content = "Добавить";
                        buttonAccClear.Visibility = Visibility.Hidden;
                    }
                }

                textBox.Text = "";
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                passwordBox.Password = "";
                textBox4.Text = "";
                checkBoxAccPrivate.IsChecked = false;
            }
            /*}
            catch
            {
                MessageBox.Show("Ошибка");
            }*/
        }

        // Кнопка Отмена:
        private void buttonAccClear_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            passwordBox.Password = "";
            textBox4.Text = "";

            check_acc = true;
            AddAccButton.Content = "Добавить";
            buttonAccClear.Visibility = Visibility.Hidden;
        }

        // Изменить:
        private void MenuItemChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isAccPrivate == false)
                {
                    dataPass = dataGridAccWork.SelectedItem as DataPass;
                    _ind = dataGridAccWork.SelectedIndex;
                }
                else
                {
                    dataPass = dataGridAccPrivate.SelectedItem as DataPass;
                    _ind = dataGridAccPrivate.SelectedIndex;
                }

                if (dataPass != null)
                {
                    textBox.Text = dataPass.title;
                    textBox1.Text = dataPass.link;
                    textBox2.Text = dataPass.nickname;
                    textBox3.Text = dataPass.email;
                    passwordBox.Password = dataPass.pass;
                    textBox4.Text = dataPass.accDescription;
                    checkBoxAccPrivate.IsChecked = isAccPrivate;

                    AddAccButton.Content = "Изменить";
                    buttonAccClear.Visibility = Visibility.Visible;
                    check_acc = false;
                }
                else
                {
                    MessageBox.Show("error");
                }
            }
            catch { MessageBox.Show("error"); }
        }

        // Удалить:
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (isAccPrivate == false)
            {
                dataPass = dataGridAccWork.SelectedItem as DataPass;
                _ind = dataGridAccWork.SelectedIndex;
            }
            else
            {
                dataPass = dataGridAccPrivate.SelectedItem as DataPass;
                _ind = dataGridAccPrivate.SelectedIndex;
            }

            if (dataPass != null)
            {
                string messageBoxText = "Вы действительно хотите удалить запись?";
                string caption = "";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result;
                result = MessageBox.Show(messageBoxText, caption, button, MessageBoxImage.Question);

                data.CheckIntConn();
                if (Data.check_con == true)
                {
                    if (result == MessageBoxResult.Yes)         // Если выбрано Да
                    {
                        try
                        {
                            Data.sqlcmd = $@"DELETE FROM Passwords 
                                             WHERE user_id = {Data.userId} AND pass_id = {dataPass.idPass}"
                            ;
                            data.Connect(Data.sqlcmd);

                            if (isAccPrivate == false)
                            {
                                dataGridAccWork.Items.Remove(dataPass);
                            }
                            else
                            {
                                dataGridAccPrivate.Items.Remove(dataPass);
                            }

                            MessageBox.Show("Запись успешно удалена!");
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка при удалении!");
                        }
                    }
                }
            }
        }

        // Информация:
        private void MenuItemInfo_Click(object sender, RoutedEventArgs e)
        {
            AccInfo accinfo = new AccInfo();

            dataPass = dataGridAccWork.SelectedItem as DataPass;
            accinfo.textBoxCode.Text = dataPass.idPass;
            accinfo.textBoxTitle.Text = dataPass.title;
            accinfo.textBlockLink.Text = dataPass.link;
            accinfo.textBoxLogin.Text = dataPass.nickname;
            accinfo.textBoxEmail.Text = dataPass.email;
            accinfo.textBoxPass.Text = dataPass.pass;
            accinfo.textBoxDesc.Text = dataPass.accDescription;

            accinfo.ShowDialog();
        }


        #endregion


                
        #region Карты
        // Кнопка Добавить/Изменить карту:
        private void CardAddButton_Click(object sender, RoutedEventArgs e)
        {
            string error = "Введите ";
            if (textBox5.Text == "")
            {
                error += "имя владельца, ";
            }
            if (comboBoxBank.Text == "")
            {
                error += "банк, ";
            }
            if (comboBoxType.Text == "")
            {
                error += "тип карты, ";
            }
            if (textBoxNumber.Text.Contains('_'))
            {
                error += "номер карты, ";
            }
            if (Date.Text.Contains('_'))
            {
                error += "срок действия, ";
            }
            if (textBoxCVC.Text.Length < 3)
            {
                error += "CVC/CVV, ";
            }

            if (textBoxPin.Text.Length < 4)
            {
                error += "пин-код, ";
            }
            error = error.Remove(error.Length - 2);

            if (textBox5.Text == "" ||
                textBoxNumber.Text.Contains('_') ||
                comboBoxBank.Text == "" ||
                comboBoxType.Text == "" ||
                Date.Text.Contains('_') ||
                textBoxCVC.Text.Contains('_') ||
                textBoxPin.Text == "")
            {
                MessageBox.Show(error);
            }
            else
            {
                //try
                //{
                data.CheckIntConn();
                if (Data.check_con == true)
                {
                    string ind = dataCard.idCard;

                    TextBlock tb1 = (TextBlock)comboBoxType.SelectedItem;
                    dataCard = new DataCard()
                    {
                        idCard = ind,
                        owner = crypto.Encode(textBox5.Text, Crypt.key),
                        bank = crypto.Encode(comboBoxBank.SelectedItem.ToString(), Crypt.key),
                        cardType = crypto.Encode(tb1.Text, Crypt.key),
                        cardNumber = crypto.Encode(textBoxNumber.Text, Crypt.key),
                        date = crypto.Encode(Date.Text, Crypt.key),
                        cvc = crypto.Encode(textBoxCVC.Text, Crypt.key),
                        pin = crypto.Encode(textBoxPin.Text, Crypt.key),
                        cardDescription = crypto.Encode(DescriptionCard.Text, Crypt.key),
                        isPrivate = checkBoxCardPrivate.ToString()
                    };

                    if (check_card == true)
                    {
                        if (checkBoxCardPrivate.IsChecked == true)
                        {
                            Data.sqlcmd = $@"INSERT INTO Cards (user_id, owner, bank, type, 
                                                                    number, date, cvc, pin, description, private)
                                                 SELECT {Data.userId}, '{dataCard.owner}', Banks.id_bank, 
                                                        `Card type`.id_type, '{dataCard.cardNumber}', '{dataCard.date}', '{dataCard.cvc}', 
                                                        '{dataCard.pin}', '{dataCard.cardDescription}', 1
                                                 FROM Banks, `Card type`
                                                 WHERE Banks.Банк='{dataCard.bank}' AND `Card type`.`Тип карты`='{dataCard.cardType}';
                                             
                                                 SELECT LAST_INSERT_ID();"
                            ;
                        }
                        else
                        {
                            Data.sqlcmd = $@"INSERT INTO Cards (user_id, owner, bank, type, 
                                                                    number, date, cvc, pin, description, private)
                                                 SELECT {Data.userId}, '{dataCard.owner}', Banks.id_bank, 
                                                        `Card type`.id_type, '{dataCard.cardNumber}', '{dataCard.date}', '{dataCard.cvc}', 
                                                        '{dataCard.pin}', '{dataCard.cardDescription}', 0
                                                 FROM Banks, `Card type`
                                                 WHERE Banks.Банк='{dataCard.bank}' AND `Card type`.`Тип карты`='{dataCard.cardType}';
                                             
                                                 SELECT LAST_INSERT_ID();"
                            ;
                        }

                        data.Connect(Data.sqlcmd);

                        if (Data.dt_user.Rows[0][0].ToString() != "0")
                        {
                            dataCard = new DataCard()
                            {
                                idCard = Data.dt_user.Rows[0][0].ToString(),
                                owner = textBox5.Text,
                                bank = comboBoxBank.SelectedItem.ToString(),
                                cardType = tb1.Text,
                                cardNumber = textBoxNumber.Text,
                                date = Date.Text,
                                cvc = textBoxCVC.Text,
                                pin = textBoxPin.Text,
                                cardDescription = DescriptionCard.Text
                            };

                            if (checkBoxCardPrivate.IsChecked == true)
                            {
                                dataGridCardsPrivate.Items.Add(dataCard);
                            }
                            else
                            {
                                dataGridCardsWork.Items.Add(dataCard);
                            }

                            MessageBox.Show("Запись успешно добавлена!");
                        }
                        else
                        {
                            MessageBox.Show("Ошибка добавления!");
                        }
                        buttonCardClear.Visibility = Visibility.Hidden;

                    }

                    if (check_card == false)
                    {
                        Data.sqlcmd = $@"UPDATE Cards, Banks, `Card type`
                                             SET owner = '{dataCard.owner}', Cards.bank = Banks.id_bank, 
                                                 Cards.type = `Card type`.id_type, number = '{dataCard.cardNumber}', 
                                                 date = '{dataCard.date}', Cards.cvc = '{dataCard.cvc}', 
                                                 Cards.pin = '{dataCard.pin}', description = '{dataCard.cardDescription}',
                                                 `private` = {checkBoxCardPrivate.IsChecked}
                                             WHERE Banks.Банк='{dataCard.bank}' AND `Card type`.`Тип карты`='{dataCard.cardType}'
                                                   AND user_id = {Data.userId} AND card_id = {dataCard.idCard}"
                        ;
                        data.Connect(Data.sqlcmd);

                        if (Data.check_con == true)
                        {
                            dataCard = new DataCard()
                            {
                                idCard = ind,
                                owner = textBox5.Text,
                                bank = comboBoxBank.SelectedItem.ToString(),
                                cardType = tb1.Text,
                                cardNumber = textBoxNumber.Text,
                                date = Date.Text,
                                cvc = textBoxCVC.Text,
                                pin = textBoxPin.Text,
                                cardDescription = DescriptionCard.Text
                            };

                            if (checkBoxCardPrivate.IsChecked == true)
                            {
                                dataCard.isPrivate = "True";
                                try
                                {
                                    dataGridCardsPrivate.Items[_ind] = dataCard;
                                }
                                catch { }
                            }
                            else
                            {
                                dataCard.isPrivate = "False";
                                try
                                {
                                    dataGridCardsPrivate.Items[_ind] = dataCard;
                                }
                                catch { }
                            }

                            MessageBox.Show("Запись успешно изменена!");


                            check_card = true;
                            CardAddButton.Content = "Добавить";
                            buttonCardClear.Visibility = Visibility.Hidden;
                        }
                    }


                    textBox5.Text = "";
                    comboBoxBank.Text = "";
                    comboBoxType.Text = "";
                    textBoxNumber.Text = "";
                    Date.Text = "";
                    textBoxCVC.Text = "";
                    textBoxPin.Text = "";
                    DescriptionCard.Text = "";
                    checkBoxCardPrivate.IsChecked = false;
                }
                /*}
                catch
                {
                    MessageBox.Show("Ошибка");
                }*/
            }
        }

        // Кнопка отмена:
        private void buttonCardClear_Click(object sender, RoutedEventArgs e)
        {
            textBox5.Text = "";
            comboBoxBank.Text = "";
            comboBoxType.Text = "";
            textBoxNumber.Text = "";
            Date.Text = "";
            textBoxCVC.Text = "";
            textBoxPin.Text = "";
            DescriptionCard.Text = "";

            check_card = true;
            CardAddButton.Content = "Добавить";
            buttonCardClear.Visibility = Visibility.Hidden;
            
        }

        // Изменить:
        private void MenuItemChange2_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (isCardPrivate == false)
                {
                    dataCard = dataGridCardsWork.SelectedItem as DataCard;
                    _ind = dataGridCardsWork.SelectedIndex;
                }
                else
                {
                    dataCard = dataGridCardsPrivate.SelectedItem as DataCard;
                    _ind = dataGridCardsPrivate.SelectedIndex;
                }

                if (dataCard != null)
                {
                    textBox5.Text = dataCard.owner;
                    comboBoxBank.Text = dataCard.bank;
                    comboBoxType.Text = dataCard.cardType;
                    textBoxNumber.Text = dataCard.cardNumber;
                    Date.Text = dataCard.date;
                    textBoxCVC.Text = dataCard.cvc;
                    textBoxPin.Text = dataCard.pin;
                    DescriptionCard.Text = dataCard.cardDescription;
                    checkBoxCardPrivate.IsChecked = isCardPrivate;

                    CardAddButton.Content = "Изменить";
                    buttonCardClear.Visibility = Visibility.Visible;
                    check_card = false;
                }
            }
            catch { }
            
        }

        // Удалить:
        private void MenuItemDelete2_Click(object sender, RoutedEventArgs e)
        {
            if (isCardPrivate == false)
            {
                dataCard = dataGridCardsWork.SelectedItem as DataCard;
                _ind = dataGridCardsWork.SelectedIndex;
            }
            else
            {
                dataCard = dataGridCardsPrivate.SelectedItem as DataCard;
                _ind = dataGridCardsPrivate.SelectedIndex;
            }

            if (dataCard != null)
            {
                string messageBoxText = "Вы действительно хотите удалить запись?";
                string caption = "";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result;
                result = MessageBox.Show(messageBoxText, caption, button, MessageBoxImage.Question);

                data.CheckIntConn();
                if (Data.check_con == true)
                {
                    if (result == MessageBoxResult.Yes)         // Если выбрано Да
                    {
                        try
                        {
                            Data.sqlcmd = $@"DELETE FROM Cards 
                                         WHERE user_id = {Data.userId} AND card_id =  {dataCard.idCard}"
                            ;
                            data.Connect(Data.sqlcmd);
                            if (Data.check_con == true)
                            {
                                if (isCardPrivate == false)
                                {
                                    dataGridCardsWork.Items.Remove(dataCard);
                                }
                                else
                                {
                                    dataGridCardsPrivate.Items.Remove(dataCard);
                                }

                                MessageBox.Show("Запись успешно удалена!");
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка при удалении!");
                        }
                    }
                }
            }
            
        }

        // Информация:
        private void MenuItemInfo2_Click(object sender, RoutedEventArgs e)
        {
            dataCard = dataGridCardsWork.SelectedItem as DataCard;
            CardInfo cardinfo = new CardInfo();
            cardinfo.CVC.Text = dataCard.cvc;
            cardinfo.Pin.Text = dataCard.pin;
            cardinfo.textBoxNumberShow.Text = dataCard.cardNumber;
            cardinfo.textBoxDateShow.Text = dataCard.date;
            cardinfo.textBoxOwnerShow.Text = dataCard.owner;
            cardinfo.textBoxBank.Text = dataCard.bank;
            cardinfo.labelCardType.Content = dataCard.cardType;

            paySystem = "";
            if (dataCard.cardNumber[0] == '2')
            {
                paySystem = "/Resources/Cards/mir.png";
            }
            if (dataCard.cardNumber[0] == '4')
            {
                paySystem = "/Resources/Cards/visa.png";
            }
            if (dataCard.cardNumber[0] == '3' || (dataCard.cardNumber[0] == '6' && (dataCard.cardNumber[1] == '3' || dataCard.cardNumber[1] == '7')) || (dataCard.cardNumber[0] == '5' && (dataCard.cardNumber[1] == '0' || dataCard.cardNumber[1] == '6' || dataCard.cardNumber[1] == '7' || dataCard.cardNumber[1] == '8')))
            {
                paySystem = "/Resources/Cards/maestro.png";
            }
            if (dataCard.cardNumber[0] == '5' && (dataCard.cardNumber[1] == '1' || dataCard.cardNumber[1] == '2' || dataCard.cardNumber[1] == '3' || dataCard.cardNumber[1] == '4' || dataCard.cardNumber[1] == '5'))
            {
                paySystem = "/Resources/Cards/mastercard.png";
            }

            cardinfo.imageCard.Source = new BitmapImage(new Uri(paySystem, UriKind.Relative));
            cardinfo.ShowDialog();
            
        }

        // Запрет пробела для номера карты:
        private void textBoxNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        // Запрет пробела для номера CVC/CVV:
        private void textBoxCVC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        // Максимум 3 числа CVC:
        private void textBoxCVC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (textBoxCVC.Text.Length > 2 || !Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        // Запрет пробела для Пин-кода:
        private void Pin_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        // Максимум 4 числа Пин-кода:
        private void Pin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (textBoxPin.Text.Length > 3 || !Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        
        #endregion
        


        #region Генератор паролей
        // Генерация паролей:
        private void buttonGen_Click(object sender, RoutedEventArgs e)
        {
            textBoxPas.Text = null;

            List<string> array = new List<string>();

            if (checkNumber.IsChecked == true)
            {
                array.AddRange(PassGenData.num);
                array.AddRange(PassGenData.num);
            }
            if (checkUp.IsChecked == true)
            {
                array.AddRange(PassGenData.up);
            }
            if (checkLow.IsChecked == true)
            {
                array.AddRange(PassGenData.low);
            }
            if (checkSymb.IsChecked == true)
            {
                array.AddRange(PassGenData.symb);
            }

            string a = slider.Value.ToString();
            int b = int.Parse(a);

            Random rnd = new Random();
            try
            {
                for (int i = 0; i < b; i++)
                {
                    textBoxPas.Text += array[rnd.Next(array.Count)];
                }
            }
            catch
            { }
        }
        #endregion



        #region Директор

        private void getStaff()
        {
            listBoxEmp.Items.Clear();
            stackPanelDir.Visibility = Visibility.Visible;
            getStaff2();
            data.GetPositions();
        }

        public void getStaff2()
        {

            Data.sqlcmd = $@"SELECT `users`.`user_id`, `users`.`name`, `users`.`surname`, `users`.`patronymic`, 
                                    `position`.`position_name`, `users`.`head`
                             FROM `users` 
	                         LEFT JOIN `position` ON `users`.`id_position` = `position`.`id_position`
                             WHERE `users`.`user_id` != {Data.myInd}"
            ;
            //DataTable dt;
            dt = data.Connect(Data.sqlcmd);

            int dt_count = dt.Rows.Count;

            string str;
            staff = new int[dt_count + 1];
            staff[0] = Data.myInd;

            str = $"(Я) {Data.userSurname} {Data.userName} {Data.userPatronymic} \nДиректор";
            listBoxEmp.Items.Add(str);

            for (int i = 0; i < dt_count; i++)
            {
                staff[i + 1] = int.Parse(dt.Rows[i][0].ToString());
                for (int j = 1; j <= 3; j++)
                {
                    dt.Rows[i][j] = crypto.Decode(dt.Rows[i][j].ToString(), Crypt.key);
                }
                str = dt.Rows[i][2].ToString() + " " + dt.Rows[i][1] + " " + dt.Rows[i][3] + "\n" + dt.Rows[i][4];
                listBoxEmp.Items.Add(str);
            }
        }
        // Выбор сотрудника и показ его данных:
        private void listBoxEmp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataGridAccWork.Items.Clear();
            dataGridAccPrivate.Items.Clear();

            //MessageBox.Show(listBoxEmp.SelectedIndex.ToString());

            tabItemAccPrivate.Visibility = Visibility.Visible;
            tabItemCardsPrivate.Visibility = Visibility.Visible;

            try
            {
                if (Data.myInd == staff[listBoxEmp.SelectedIndex])
                {
                    tabItemAccPrivate.Visibility = Visibility.Visible;
                    tabItemCardsPrivate.Visibility = Visibility.Visible;
                }
                else
                {
                    tabItemAccPrivate.Visibility = Visibility.Hidden;
                    tabItemCardsPrivate.Visibility = Visibility.Hidden;
                }

                Data.userId = staff[listBoxEmp.SelectedIndex];
            }
            catch { }

            if (listBoxEmp.SelectedIndex == 0)
            {
                Data.sqlcmd = $@"SELECT *
                             FROM passwords
                             WHERE user_id = {staff[listBoxEmp.SelectedIndex]}"
                ;
                checkBoxAccPrivate.IsEnabled = true;
                checkBoxCardPrivate.IsEnabled = true;
            }
            else if (listBoxEmp.SelectedIndex != -1)
            {
                Data.sqlcmd = $@"SELECT *
                             FROM passwords
                             WHERE user_id = {staff[listBoxEmp.SelectedIndex]} AND private = 0"
                ;
                checkBoxAccPrivate.IsChecked = false;
                checkBoxAccPrivate.IsEnabled = false;
                checkBoxCardPrivate.IsChecked = false;
                checkBoxCardPrivate.IsEnabled = false;
            }

            Data.dt_user = data.Connect(Data.sqlcmd);

            try
            {
                crypto.Encrypt(Data.dt_user, Data.accountsCount, 7);
            }
            catch { }

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
                    dataGridAccWork.Items.Add(data);
                }
                else
                {
                    dataGridAccPrivate.Items.Add(data);
                }
            }



            dataGridCardsWork.Items.Clear();
            dataGridCardsPrivate.Items.Clear();
            if (listBoxEmp.SelectedIndex == 0)
            {
                Data.sqlcmd = $@"SELECT `cards`.`card_id`, `cards`.`user_id`, `cards`.owner, `banks`.`Банк`, `card type`.`Тип карты`, 
                                        `cards`.number, `cards`.date, `cards`.cvc, `cards`.pin, `cards`.description, `cards`.private
                             FROM `cards` 
	                         LEFT JOIN `banks` ON `cards`.bank = `banks`.`id_bank` 
	                         LEFT JOIN `card type` ON `cards`.type = `card type`.`id_type`
                             WHERE user_id = {staff[listBoxEmp.SelectedIndex]}"
                ;
            }
            else if (listBoxEmp.SelectedIndex != -1)
            {
                Data.sqlcmd = $@"SELECT `cards`.`card_id`, `cards`.`user_id`, `cards`.owner, `banks`.`Банк`, `card type`.`Тип карты`, 
                                        `cards`.number, `cards`.date, `cards`.cvc, `cards`.pin, `cards`.description, `cards`.private
                             FROM `cards` 
	                         LEFT JOIN `banks` ON `cards`.bank = `banks`.`id_bank` 
	                         LEFT JOIN `card type` ON `cards`.type = `card type`.`id_type`
                             WHERE user_id = {staff[listBoxEmp.SelectedIndex]} AND private = 0"
                ;
            }
            Data.dt_user = data.Connect(Data.sqlcmd);

            try
            {
                crypto.Encrypt(Data.dt_user, Data.cardsCount, 8);
            }
            catch { }

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
                    dataGridCardsWork.Items.Add(dataCard);
                }
                else
                {
                    dataGridCardsPrivate.Items.Add(dataCard);
                }
            }

        }

        #endregion



        #region Руководитель

        private void getHead()
        {
            listBoxEmp.Items.Clear();

            Data.sqlcmd = $@"SELECT `users`.`user_id`, `users`.`name`, `users`.`surname`, `users`.`patronymic`, `position`.`position_name`
                             FROM `users` 
	                         LEFT JOIN `position` ON `users`.`id_position` = `position`.`id_position`
                             WHERE `users`.`head` = {Data.userId};"
            ;
            DataTable dt;
            dt = data.Connect(Data.sqlcmd);

            int dt_count = dt.Rows.Count;
            string str;
            staff = new int[dt_count + 1];
            staff[0] = Data.myInd;

            str = $"(Я) {Data.userName} {Data.userSurname} {Data.userPatronymic} \nРуководитель";
            listBoxEmp.Items.Add(str);


            for (int i = 0; i < dt_count; i++)
            {
                staff[i + 1] = int.Parse(dt.Rows[i][0].ToString());
                for (int j = 1; j <= 3; j++)
                {
                    dt.Rows[i][j] = crypto.Decode(dt.Rows[i][j].ToString(), Crypt.key);
                }
                str = dt.Rows[i][2].ToString() + " " + dt.Rows[i][1] + " " + dt.Rows[i][3] + "\n" + dt.Rows[i][4];
                listBoxEmp.Items.Add(str);
            }
        }

        #endregion



        private void Refresh()
        {
            Data.userName = Data.myName;
            Data.userSurname = Data.mySurname;
            Data.userPatronymic = Data.myPatronymic;
            Data.userPosition = Data.myPos;

            if (Data.check_con == true)
            {
                listBoxEmp.SelectedIndex = -1;
                listBoxEmp.Items.Clear();
                staff = new int[0];

                if (Data.myPos == "Директор")
                {
                    getStaff();
                    Data.dir = true;
                }
                else if (Data.myPos == "Руководитель")
                {
                    getHead();
                }
                else
                {
                    string str = $"(Я) {Data.userSurname} {Data.userName} {Data.userPatronymic} \n{Data.myPos}";
                    listBoxEmp.Items.Add(str);
                    listBoxEmp.IsHitTestVisible = false;
                }
                data.show_accounts(dataGridAccWork, dataGridAccPrivate);
                data.show_cards(dataGridCardsWork, dataGridCardsPrivate);
            }
        }

        // Кнопка блокировки:
        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            PassBlock passBlock = new PassBlock();
            BlurEffect blur = new BlurEffect();
            if (block == false)
            {

                blur.Radius = 30;
                Effect = blur;
                passBlock.ShowDialog();
                block = true;
            }
            if (PassBlock.close_check == true)      // Закрыть приложение, если не ввели пароль разблокировки
            {
                Close();
            }
            else
            {
                blur.Radius = 0;
                Effect = blur;
                block = false;
            }
        }

        // Горячая клавиша блокировки CTRL+B:
        public void Executed_New(object sender, ExecutedRoutedEventArgs e)
        {
            button_Click_1(null, null);
        }

        // Загрузка данных:
        private void ShowAll()
        {
            //Data.myInd = Data.userId;
            if (Data.check_con == true)
            {
                Data.myPos = Data.userPosition;
                if (Data.userPosition == "Директор")
                {
                    Data.dir = true;
                    getStaff();
                }
                else if (Data.userPosition == "Руководитель")
                {
                    getHead();
                }
                else
                {
                    string str = $"(Я) {Data.userSurname} {Data.userName} {Data.userPatronymic} \n{Data.myPos}";
                    listBoxEmp.Items.Add(str);
                    listBoxEmp.IsHitTestVisible = false;
                }
                data.show_accounts(dataGridAccWork, dataGridAccPrivate);
                data.show_cards(dataGridCardsWork, dataGridCardsPrivate);
            }

            expander.Header = "Пользователь:   " + Data.userNick;

            for (int i = 0; i < Data.dt_banks.Rows.Count; i++)
            {
                comboBoxBank.Items.Add(Data.dt_banks.Rows[i][1].ToString());
            }

        }

        // Минимизация окна:
        private void image_min_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //ShowInTaskbar = true;
            WindowState = WindowState.Minimized;
            //ShowInTaskbar = true;
        }

        // Кнопка закрытия окна:
        private void image_close_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            string messageBoxText = "Вы действительно хотите выйти?";
            string caption = "";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result;
            result = MessageBox.Show(messageBoxText, caption, button, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)         // Если выбрано Да
            {
                Environment.Exit(0);
            }
            
        }

        // Закрытие окна сторонними процессами:
        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        // Сворачивание экспандера при нажитии в другое место:
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            expander.IsExpanded = false;
        }

        // Кнопка Сменить пользователя:
        private void ButtonChangeUser_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Вы действительно хотите выйти?";
            string caption = "";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result;
            result = MessageBox.Show(messageBoxText, caption, button, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)         // Если выбрано Да
            {
                MainWindow auth = new MainWindow();
                this.Hide();
                auth.Show();
            }
            
        }

        // Кнопка Выйти:
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            image_close_PreviewMouseUp(null, null);
        }

        // Профиль
        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile();
            profile.ShowDialog();
        }

        // Обновить:
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(Data.myInd.ToString());
            Data.userId = Data.myInd;
            //ShowAll();
            Refresh();
        }

        // Регистрация нового сотрудника:
        private void textBlockReg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Reg reg = new Reg();
            reg.ShowDialog();
        }

        // Новая должность:
        private void textBlockPos_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AddPosition ap = new AddPosition();
            ap.ShowDialog();
        }

        private void tabItemAccPrivate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            checkBoxAccPrivate.IsChecked = true;
            isAccPrivate = true;
        }

        private void tabItemAccWork_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            checkBoxAccPrivate.IsChecked = false;
            isAccPrivate = false;
        }

        private void tabItemCardsWork_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            checkBoxAccPrivate.IsChecked = false;
            isCardPrivate = false;
        }

        private void tabItemCardsPrivate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            checkBoxCardPrivate.IsChecked = true;
            isCardPrivate = true;
        }
    }

}
