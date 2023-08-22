using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static PassManager.Data;

namespace PassManager
{
    /// <summary>
    /// Логика взаимодействия для AddPosition.xaml
    /// </summary>
    public partial class AddPosition : Window
    {
        public AddPosition()
        {
            InitializeComponent();

            button1.Visibility = Visibility.Hidden;
            buttonCancel.Visibility = Visibility.Hidden;
            data.GetPositions();
            FillPos();
        }

        bool check = false;
        Data data = new Data();
        int N;

        private void FillPos()
        {
            listBoxPosition.Items.Clear();
            N = Data.dt_positions.Rows.Count;
            for (int i = 0; i < N; i++)
            {
                listBoxPosition.Items.Add(Data.dt_positions.Rows[i][1].ToString());
            }
        }

        // Кнопка Добавить/Изменить
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (check == false)
            {
                for (int i = 0; i < N; i++)
                {
                    if (textBox.Text == Data.dt_positions.Rows[i][1].ToString())
                    {
                        MessageBox.Show("Запись уже существует!");
                        break;
                    }
                    else if (i == N-1)
                    {
                        Data.sqlcmd = $@"INSERT INTO position
                                         (position_name)
                                         VALUES ('{textBox.Text}');

                                         SELECT LAST_INSERT_ID()"
                        ;

                        data.Connect(Data.sqlcmd);
                        string n = dt_user.Rows[0][0].ToString();

                        MessageBox.Show("Запись успешно добавлена!");

                        Data.dt_positions.Rows.Add(n, textBox.Text);
                        FillPos();

                        textBox.Text = "";
                        check = false;
                        break;
                    }
                }

            }
            else
            {
                Data.sqlcmd = $@"UPDATE position
                                 SET position_name = {textBox.Text}
                                 WHERE id_position = {Data.dt_positions.Rows[listBoxPosition.SelectedIndex][0]}"
                ;
                data.Connect(Data.sqlcmd);
                MessageBox.Show("Запись успешно изменена!");

                Data.dt_positions.Rows[listBoxPosition.SelectedIndex][1] = textBox.Text;
                listBoxPosition.Items[listBoxPosition.SelectedIndex] = textBox.Text;

                button1.Visibility = Visibility.Hidden;
                buttonCancel.Visibility = Visibility.Hidden;

                buttonAdd.Content = "Добавить";
                textBox.Text = "";
                listBoxPosition.SelectedIndex = -1;
                check = false;

            }
        }

        private void listBoxPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //textBox.Text = Data.dt_positions.Rows[int.Parse(listBoxPosition.SelectedIndex.ToString())][0].ToString();
                textBox.Text = Data.dt_positions.Rows[int.Parse(listBoxPosition.SelectedIndex.ToString())][1].ToString();

                button1.Visibility = Visibility.Visible;
                buttonCancel.Visibility = Visibility.Visible;

                buttonAdd.Content = "Изменить";
                check = true;
            }
            catch {}

        }

        // Кнопка Отмена
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            button1.Visibility = Visibility.Hidden;
            buttonCancel.Visibility = Visibility.Hidden;

            buttonAdd.Content = "Добавить";
            textBox.Text = "";
            listBoxPosition.SelectedIndex = -1;
            check = false;
        }

        // Кнопка Удалить
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Вы действительно хотите удалить запись?";
            string caption = "";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result;
            result = MessageBox.Show(messageBoxText, caption, button, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)         // Если выбрано Да
            {
                try
                {
                    Data.sqlcmd = $@"DELETE FROM position 
                                     WHERE id_position = {Data.dt_positions.Rows[int.Parse(listBoxPosition.SelectedIndex.ToString())][0]}"
                    ;

                    data.Connect(Data.sqlcmd);


                    MessageBox.Show("Запись успешно удалена!");

                    listBoxPosition.Items.Remove(listBoxPosition.SelectedItem); 
                    button1.Visibility = Visibility.Hidden;
                    buttonCancel.Visibility = Visibility.Hidden;
                    buttonAdd.Content = "Добавить";
                    textBox.Text = "";
                    check = true;
                }
                catch
                {
                    MessageBox.Show("Ошибка при удалении!");
                }
            }

        }
    }
}
