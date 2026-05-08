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
    /// Логика взаимодействия для AddEdithEmployee.xaml
    /// </summary>
    public partial class AddEdithEmployee : Page
    {
        private Users tempEmployee = new Users();
        public AddEdithEmployee(Users selectedEmployee = null)
        {
            InitializeComponent();

            if (selectedEmployee != null)
            {
                tempEmployee = selectedEmployee;
            }

            DataContext = tempEmployee;

            CBRole.ItemsSource = ConnectObject.GetConnect().Roles.ToList();
            CBStatus.ItemsSource = ConnectObject.GetConnect().UserStatuses.ToList();
        }
        private void ButtonOKAddEdit_Click(object sender, RoutedEventArgs e)
        {
            bool isNew = tempEmployee.UserID == 0;

            if (string.IsNullOrWhiteSpace(TBLastName.Text) ||
                string.IsNullOrWhiteSpace(TBFirstName.Text) ||
                string.IsNullOrWhiteSpace(TBPatronymic.Text) ||
                string.IsNullOrWhiteSpace(TBLoginNew.Text) ||
                (isNew && string.IsNullOrWhiteSpace(TBPasswordNew.Text)) ||
                CBRole.SelectedItem == null ||
                CBStatus.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            tempEmployee.RoleID = (CBRole.SelectedItem as Roles)?.RoleID ?? 0;
            tempEmployee.StatusID = (CBStatus.SelectedItem as UserStatuses)?.StatusID ?? 0;

            if (!string.IsNullOrWhiteSpace(TBPasswordNew.Text))
            {
                tempEmployee.Password = TBPasswordNew.Text; 
            }

            var context = ConnectObject.GetConnect();

            if (isNew)
            {
                context.Users.Add(tempEmployee);
            }

            context.SaveChanges();
            MessageBox.Show("Изменения сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            FrameObject.frameMain.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.GoBack();
        }
    }
}
