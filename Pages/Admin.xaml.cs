using FTSControl.Data;
using Microsoft.Win32;
using Newtonsoft.Json;        
using Newtonsoft.Json.Linq;   
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
        // Метод для экспорта таблицы пользователей в Json
        private void Export(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    FileName = $"SPKanban_Users_{DateTime.Now:yyyyMMdd_HHmmss}.json",
                    Title = "Экспорт списка сотрудников"
                };

                if (saveFileDialog.ShowDialog() != true) return; 

                var context = ConnectObject.GetConnect();
                var employees = context.Users.Select(u => new
                    {u.UserID,u.LastName,u.FirstName,u.Patronymic,u.Login,Role = u.Roles != null ? u.Roles.RoleName : "Не указана",
                     Department = u.Departments != null ? u.Departments.DepartmentName : "Не указан",Email = u.Email,
                     Phone = u.Phone,Status = u.UserStatuses != null ? u.UserStatuses.StatusName : "Неизвестен",
                     u.CreatedAt,u.UpdatedAt}).ToList();

                var exportData = employees;

                string jsonString = JsonConvert.SerializeObject(exportData, Formatting.Indented,new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include
                    });

                File.WriteAllText(saveFileDialog.FileName, jsonString, new UTF8Encoding(false));

                MessageBox.Show( $"Экспорт завершён!\nСотрудников: {employees.Count}","Успех",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта:\n{ex.Message}\n\n{ex.InnerException?.Message}","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
    }
}
