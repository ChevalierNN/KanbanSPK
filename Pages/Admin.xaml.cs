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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FTSControl.Data;

namespace FTSControl.Pages
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Page
    {
        public Admin()
        {
            InitializeComponent();

            DGridEmployees.ItemsSource = ConnectObject.GetConnect().Users.ToList();
        }

        private void ButtonAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.Navigate(new AddEdithEmployee());
        }
        private void BackAutorization_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.GoBack();
        }
        // Переход на страницу AddEdithUser с заполненными полями для удобного изменения
        private void ButtonEditEmployee_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedUser = button?.DataContext as Users;

            if (selectedUser != null)
            {
                FrameObject.frameMain.Navigate(new AddEdithEmployee(selectedUser));
            }
        }
    }
}
