using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace PassManager
{
    /// <summary>
    /// Логика взаимодействия для Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        public Profile()
        {
            InitializeComponent();

            Launch();
        }

        Data data = new Data();
        Crypt crypto = new Crypt();

        private void GetData()
        {
            Data.sqlcmd = $@"SELECT `users`.`user_id`, `users`.`name`, `users`.`surname`, `users`.`patronymic`, 
                                    `users`.`login`, `users`.`email`, `position`.`position_name`
                             FROM `users` 
	                         LEFT JOIN `position` ON `users`.`id_position` = `position`.`id_position`
                             WHERE `users`.`user_id` = {Data.userId}"
            ;


            DataTable dt = data.Connect(Data.sqlcmd);
            for (int i = 1; i <= 5; i++)
            {
                dt.Rows[0][i] = crypto.Decode(dt.Rows[0][i].ToString(), Crypt.key);
            }

            labelSurname.Text = dt.Rows[0][2].ToString();
            labelName.Text = dt.Rows[0][1].ToString();
            labelPatronymic.Text = dt.Rows[0][3].ToString();
            labelNick.Text = dt.Rows[0][4].ToString();
            labelEmail.Text = dt.Rows[0][5].ToString();
            comboBoxPosition.Text = dt.Rows[0][6].ToString();

            if (Data.myPos != "Директор" && Data.myPos != "Руководитель")
            {
                comboBoxPosition.Text = Data.myPos;
            }
        }

        private void FillPosition()
        {
            if (Data.myPos == "Директор" || Data.myPos == "Руководитель")
            {
                int N = Data.dt_positions.Rows.Count;
                for (int i = 0; i < N; i++)
                {
                    comboBoxPosition.Items.Add(Data.dt_positions.Rows[i][1].ToString());
                }
            }
            else
            {
                comboBoxPosition.Items.Add(Data.myPos.ToString());
            }
        }

        private void DataChangeButton_Click(object sender, RoutedEventArgs e)
        {
            Data.userSurname = crypto.Encode(labelSurname.Text, Crypt.key);
            Data.userName = crypto.Encode(labelName.Text, Crypt.key);
            Data.userPatronymic = crypto.Encode(labelPatronymic.Text, Crypt.key);
            Data.userNick = crypto.Encode(labelNick.Text, Crypt.key);
            Data.userEmail = crypto.Encode(labelEmail.Text, Crypt.key);
            Data.userPosition = comboBoxPosition.Text;

            Data.sqlcmd = $@"UPDATE users, position
                             SET name = '{Data.userName}', surname = '{Data.userSurname}', patronymic = '{Data.userPatronymic}', 
                                 login = '{Data.userNick}', email = '{Data.userEmail}', `users`.`id_position` = `position`.`id_position`
                             WHERE `position`.`position_name` = '{comboBoxPosition.SelectedItem}' 
                                   AND user_id = {Data.userId}"
            ;

            data.Connect(Data.sqlcmd);
        }

        private void getUsers()
        {
            if (Data.myPos == "Директор" || Data.myPos == "Руководитель")
            {
                int N = Passes.dt.Rows.Count;
                List<Item> items1 = new List<Item>();
                List<Item> items2 = new List<Item>();

                for (int i = 0; i < N; i++)
                {
                    if (Passes.dt.Rows[i][5].ToString() == Data.userId.ToString())
                    {
                        //listBox.Items.Add($"{Passes.dt.Rows[i][2]} {Passes.dt.Rows[i][1]} {Passes.dt.Rows[i][3]} \n{Passes.dt.Rows[i][4]}");
                        items1.Add
                        (
                            new Item()
                            {
                                id = int.Parse(Passes.dt.Rows[i][0].ToString()),
                                Name = $"{Passes.dt.Rows[i][2]} {Passes.dt.Rows[i][1]} {Passes.dt.Rows[i][3]} \n{Passes.dt.Rows[i][4]}"
                            }
                         );
                    }
                }
                listBox.ItemsSource = items1;

                for (int i = 0; i < N; i++)
                {
                    if (Passes.dt.Rows[i][5].ToString() != Data.userId.ToString() && Passes.dt.Rows[i][0].ToString() != Data.userId.ToString())
                    {
                        items2.Add
                        (
                            new Item()
                            {
                                id = int.Parse(Passes.dt.Rows[i][0].ToString()),
                                Name = $"{Passes.dt.Rows[i][2]} {Passes.dt.Rows[i][1]} {Passes.dt.Rows[i][3]} \n{Passes.dt.Rows[i][4]}"
                            }
                         );
                    }
                }
                listBox2.ItemsSource = items2;
            }

        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            foreach (var tObj in (listBox2.ItemsSource as List<Item>).Where(myObj => myObj.IsChecked))
            {
                MessageBox.Show(tObj.id.ToString());

                Data.sqlcmd = $@"UPDATE users
                                 SET head = {Data.userId}
                                 WHERE user_id = {tObj.id};"
                ;
            }
            data.Connect(Data.sqlcmd);

            Passes passes = new Passes();
            passes.getStaff2();
            getUsers();
            listBox.UpdateLayout();
            listBox2.UpdateLayout();


        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            foreach (var tObj in (listBox.ItemsSource as List<Item>).Where(myObj => myObj.IsChecked))
            {
                //MessageBox.Show(tObj.id.ToString());

                Data.sqlcmd = $@"UPDATE users
                                 SET head = NULL
                                 WHERE user_id = {tObj.id};"
                ;
            }
            data.Connect(Data.sqlcmd);

            Passes passes = new Passes();
            passes.getStaff2();

            getUsers();
            listBox.UpdateLayout();
            listBox2.UpdateLayout();
        }

        private void Launch()
        {
            FillPosition();
            GetData();
            getUsers();

            if (Data.myPos != "Директор" && Data.myPos != "Руководитель")
            {
                buttonDelete.Visibility = Visibility.Hidden;
                stackPanelStaff1.Visibility = Visibility.Hidden;
                stackPanelStaff2.Visibility = Visibility.Hidden;
            }

            if (Data.userId == Data.myInd)
            {
                comboBoxPosition.IsEnabled = false;
                labelNick.IsEnabled = true;
            }
            else
            {
                comboBoxPosition.IsEnabled = true;
                labelNick.IsEnabled = false;
            }
        }
    }

    class Item
    {
        public int id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }

    }
}
