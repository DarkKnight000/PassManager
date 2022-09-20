using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
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

namespace PassManager
{
    /// <summary>
    /// Логика взаимодействия для Passes.xaml
    /// </summary>
    public partial class Passes : Window
    {
        Data data = new Data();
        Crypt crypto = new Crypt();
        private static bool check_acc = true;
        private static bool check_card = true;
        private static string paySystem;
        public static bool block = false;
        public Passes()
        {
            InitializeComponent();

            ShowAll();

        }

        #region Аккаунты
        // Кнопка Добавить/Изменить:
        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                data.CheckIntConn();
                if (Data.check_con == true)
                {
                    Data.title = crypto.Encode(textBox.Text, Crypt.key);
                    Data.link = crypto.Encode(textBox1.Text, Crypt.key);
                    Data.nickname = crypto.Encode(textBox2.Text, Crypt.key);
                    Data.email = crypto.Encode(textBox3.Text, Crypt.key);
                    Data.pass = crypto.Encode(passwordBox.Password, Crypt.key);
                    Data.accDescription = crypto.Encode(textBox4.Text, Crypt.key);

                    if (check_acc == true)
                    {
                        Data.sqlcmd = $@"INSERT INTO Passwords
                                     (`user_id`, `Код`, `Заголовок`, `Ссылка`, `Логин(Ник)`, 
                                      `Электронная почта`, `Пароль`, `Описание`)
                                     VALUES ({Data.userId}, {++Data.accLastCode}, '{Data.title}', '{Data.link}', 
                                            '{Data.nickname}', '{Data.email}', '{Data.pass}', '{Data.accDescription}')"
                        ;

                        data.Connect(Data.sqlcmd);
                        if (Data.check_con == true)
                        {
                            data.show_accounts(dataGrid1);

                            MessageBox.Show("Запись успешно добавлена!");
                        }

                    }
                    if (check_acc == false)
                    {
                        Data.sqlcmd = $@"UPDATE Passwords
                                     SET `Заголовок` = '{Data.title}', `Ссылка` = '{Data.link}', 
                                         `Логин(Ник)` = '{Data.nickname}', `Электронная почта` = '{Data.email}', 
                                         `Пароль` = '{Data.pass}', `Описание` = '{Data.accDescription}'
                                     WHERE user_id = {Data.userId} AND `Код` = {Data.idAcc}"
                        ;
                        data.Connect(Data.sqlcmd);
                        if (Data.check_con == true)
                        {
                            data.show_accounts(dataGrid1);

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
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
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
                DataRowView row = dataGrid1.SelectedItem as DataRowView;
                if (row != null)
                {
                    selectAcc_row();
                    textBox.Text = Data.title;
                    textBox1.Text = Data.link;
                    textBox2.Text = Data.nickname;
                    textBox3.Text = Data.email;
                    passwordBox.Password = Data.pass;
                    textBox4.Text = Data.accDescription;

                    AddAccButton.Content = "Изменить";
                    buttonAccClear.Visibility = Visibility.Visible;
                    check_acc = false;
                }
            }
            catch { }
        }

        // Удалить:
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = dataGrid1.SelectedItem as DataRowView;
            if (row != null)
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
                            selectAcc_row();
                            Data.sqlcmd = $@"DELETE FROM Passwords 
                                         WHERE user_id = {Data.userId} AND Код = {Data.idAcc}"
                            ;
                            data.Connect(Data.sqlcmd);
                            data.show_accounts(dataGrid1);
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

            selectAcc_row();
            accinfo.textBoxCode.Text = Data.idAcc;
            accinfo.textBoxTitle.Text = Data.title;
            accinfo.textBoxLogin.Text = Data.nickname;
            accinfo.textBoxEmail.Text = Data.email;
            accinfo.textBoxPass.Text = Data.pass;
            accinfo.textBoxDesc.Text = Data.accDescription;

            accinfo.ShowDialog();
        }

        // Выбор строки:
        public void selectAcc_row()
        {
            try
            {
                DataRowView row = dataGrid1.SelectedItem as DataRowView;
                if (row != null)
                {
                    Data.idAcc = row.Row.ItemArray[0].ToString();
                    Data.title = row.Row.ItemArray[1].ToString();
                    Data.link = row.Row.ItemArray[2].ToString();
                    Data.nickname = row.Row.ItemArray[3].ToString();
                    Data.email = row.Row.ItemArray[4].ToString();
                    Data.pass = row.Row.ItemArray[5].ToString();
                    Data.accDescription = row.Row.ItemArray[6].ToString();
                }
            }
            catch
            { }
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
            if (textBoxNumber.Text.Contains('_'))
            {
                error += "номер карты, ";
            }
            if (Date.Text.Contains('_'))
            {
                error += "дату, ";
            }
            if (textBoxCVC.Text.Contains('_'))
            {
                error += "CVC/CVV, ";
            }
            error = error.Remove(error.Length - 2);

            if (textBox5.Text == "" || textBoxNumber.Text.Contains('_') || Date.Text.Contains('_') || textBoxCVC.Text.Contains('_'))
            {
                MessageBox.Show(error);
            }
            else
            {
                try
                {
                    data.CheckIntConn();
                    if (Data.check_con == true)
                    {
                        Data.owner = crypto.Encode(textBox5.Text, Crypt.key);
                        Data.bank = crypto.Encode(comboBoxBank.Text, Crypt.key);
                        Data.cardType = crypto.Encode(comboBoxType.Text, Crypt.key);
                        Data.cardNumber = crypto.Encode(textBoxNumber.Text, Crypt.key);
                        Data.date = crypto.Encode(Date.Text, Crypt.key);
                        Data.cvc = crypto.Encode(textBoxCVC.Text, Crypt.key);
                        Data.pin = crypto.Encode(textBoxPin.Text, Crypt.key);
                        Data.cardDescription = crypto.Encode(DescriptionCard.Text, Crypt.key);

                        if (check_card == true)
                        {
                            Data.sqlcmd = $@"INSERT INTO Cards (user_id, Код, `Владелец карты`, Банк, `Тип карты`, 
                                                            `Номер карты`, `Срок действия`, `CVC/CVV`, `Пин-код`, Описание)
                                         SELECT {Data.userId}, {++Data.cardsLastCode}, '{Data.owner}', Banks.id_bank, 
                                                `Card type`.id_type, '{Data.cardNumber}', '{Data.date}', `CVC/CVV`.id_cvc, 
                                                Pin.id_pin, '{Data.cardDescription}'
                                         FROM Banks, Pin, `Card type`, `CVC/CVV`
                                         WHERE Banks.Банк='{Data.bank}' AND `Card type`.`Тип карты`='{Data.cardType}' 
                                               AND `CVC/CVV`.`CVC/CVV`='{Data.cvc}' And Pin.`Пин-код`='{Data.pin}';"
                            ;
                            data.Connect(Data.sqlcmd);
                            if (Data.check_con == true)
                            {
                                data.show_cards(dataGrid2);
                            }
                            MessageBox.Show("Запись успешно добавлена!");
                            buttonCardClear.Visibility = Visibility.Hidden;

                        }
                        if (check_card == false)
                        {
                            Data.sqlcmd = $@"UPDATE Cards, Banks, `Card type`, `CVC/CVV`, Pin
                                         SET `Владелец карты` = '{Data.owner}', Cards.Банк = Banks.id_bank, 
                                             Cards.`Тип карты` = `Card type`.id_type, `Номер карты` = '{Data.cardNumber}', 
                                             `Срок действия` = '{Data.date}', Cards.`CVC/CVV` = `CVC/CVV`.id_cvc, 
                                             Cards.`Пин-код` = Pin.id_pin, Описание = '{Data.cardDescription}'
                                         WHERE Banks.Банк='{Data.bank}' AND `Card type`.`Тип карты`='{Data.cardType}' 
                                               AND `CVC/CVV`.`CVC/CVV`='{Data.cvc}' And Pin.`Пин-код`='{Data.pin}' 
                                               AND user_id = {Data.userId} AND Cards.Код = {Data.idCard};"
                            ;
                            data.Connect(Data.sqlcmd);
                            if (Data.check_con == true)
                            {
                                data.show_cards(dataGrid2);
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
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка");
                }
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
                DataRowView row = dataGrid2.SelectedItem as DataRowView;
                if (row != null)
                {
                    selectCard_row();
                    textBox5.Text = Data.owner;
                    comboBoxBank.Text = Data.bank;
                    comboBoxType.Text = Data.cardType;
                    textBoxNumber.Text = Data.cardNumber;
                    Date.Text = Data.date;
                    textBoxCVC.Text = Data.cvc;
                    textBoxPin.Text = Data.pin;
                    DescriptionCard.Text = Data.cardDescription;

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
            DataRowView row = dataGrid2.SelectedItem as DataRowView;
            if (row != null)
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
                            selectCard_row();
                            Data.sqlcmd = $@"DELETE FROM Cards 
                                         WHERE user_id = {Data.userId} AND Код =  {Data.idCard}"
                            ;
                            data.Connect(Data.sqlcmd);
                            if (Data.check_con == true)
                            {
                                data.show_cards(dataGrid2);
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
            selectCard_row();
            CardInfo cardinfo = new CardInfo();
            cardinfo.CVC.Text = Data.cvc;
            cardinfo.Pin.Text = Data.pin;
            cardinfo.textBoxNumberShow.Text = Data.cardNumber;
            cardinfo.textBoxDateShow.Text = Data.date;
            cardinfo.textBoxOwnerShow.Text = Data.owner;
            cardinfo.textBoxBank.Text = Data.bank;
            cardinfo.labelCardType.Content = Data.cardType;

            paySystem = "";
            if (Data.cardNumber[0] == '2')
            {
                paySystem = "/Resources/Cards/mir.png";
            }
            if (Data.cardNumber[0] == '4')
            {
                paySystem = "/Resources/Cards/visa.png";
            }
            if (Data.cardNumber[0] == '3' || (Data.cardNumber[0] == '6' && (Data.cardNumber[1] == '3' || Data.cardNumber[1] == '7')) || (Data.cardNumber[0] == '5' && (Data.cardNumber[1] == '0' || Data.cardNumber[1] == '6' || Data.cardNumber[1] == '7' || Data.cardNumber[1] == '8')))
            {
                paySystem = "/Resources/Cards/maestro.png";
            }
            if (Data.cardNumber[0] == '5' && (Data.cardNumber[0] == '5' || Data.cardNumber[1] == '1' || Data.cardNumber[1] == '2' || Data.cardNumber[1] == '3' || Data.cardNumber[1] == '4' || Data.cardNumber[1] == '5'))
            {
                paySystem = "/Resources/Cards/mastercard.png";
            }

            cardinfo.imageCard.Source = new BitmapImage(new Uri(paySystem, UriKind.Relative));
            cardinfo.ShowDialog();
        }

        // Выбор строки:
        public void selectCard_row()
        {
            try
            {
                DataRowView row = dataGrid2.SelectedItem as DataRowView;
                if (row != null)
                {
                    Data.idCard = row.Row.ItemArray[0].ToString();
                    Data.owner = row.Row.ItemArray[1].ToString();
                    Data.bank = row.Row.ItemArray[2].ToString();
                    Data.cardType = row.Row.ItemArray[3].ToString();
                    Data.cardNumber = row.Row.ItemArray[4].ToString();
                    Data.date = row.Row.ItemArray[5].ToString();
                    Data.cvc = row.Row.ItemArray[6].ToString();
                    Data.pin = row.Row.ItemArray[7].ToString();
                    Data.cardDescription = row.Row.ItemArray[8].ToString();
                }
            }
            catch
            {
            }
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
            if (Data.check_con == true)
            {
                data.show_accounts(dataGrid1);
                data.show_cards(dataGrid2);
            }

            expander.Header = "Пользователь:   " + Data.userName;

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
                Data.sqlcmd = $@"UPDATE Users
                                 SET last_access = NOW(),
                                     online = 0
                                 WHERE BINARY user_id = {Data.userId}"
                ;
                data.Connect(Data.sqlcmd);

                Environment.Exit(0);
            }
            
        }

        // Закрытие окна сторонними процессами:
        private void Window_Closed(object sender, EventArgs e)
        {
            Data.sqlcmd = $@"UPDATE Users
                             SET last_access = NOW(),
                                 online = 0
                             WHERE BINARY user_id = {Data.userId}"
            ;
            data.Connect(Data.sqlcmd);

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
    }

}
