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
    /// Логика взаимодействия для AddEditTask.xaml
    /// </summary>
    public partial class AddEditTask : Page
    {
        private Tasks tempTask = new Tasks();
        public AddEditTask(Tasks selectedTask = null)
        {
            InitializeComponent();
            if (selectedTask != null)
            {
                tempTask = selectedTask;
            }
            else
            {
                tempTask.CreatedByUserID = User.Id; 
                tempTask.CreatedAt = DateTime.Now;
                tempTask.DueDate = DateTime.Now.Date.AddDays(5);
            }

            DataContext = tempTask;
            CBPriority.ItemsSource = ConnectObject.GetConnect().TaskPriorities.ToList();
            CBEmployee.ItemsSource = ConnectObject.GetConnect().Users.Where(u => u.RoleID == 3).ToList();
            CBManager.ItemsSource = ConnectObject.GetConnect().Users.Where(u => u.RoleID == 2).ToList();
            CBTaskStatus.ItemsSource = ConnectObject.GetConnect().TaskStatuses.ToList();
            if (User.RoleID == 3)
            {
                CBTaskStatus.ItemsSource = ((IEnumerable<TaskStatuses>)CBTaskStatus.ItemsSource).Where(s => s.StatusID == 1 || s.StatusID == 2).ToList();
            }
        }

        private void ButtonOKAddEdit_Click(object sender, RoutedEventArgs e) 
        {
            if (string.IsNullOrWhiteSpace(tempTask.Title) ||
                CBEmployee.SelectedItem == null ||
                CBTaskStatus.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
                var context = ConnectObject.GetConnect();

                if (tempTask.TaskID == 0) 
                {
                    context.Tasks.Add(tempTask);
                }
                else
                {
                    tempTask.UpdatedAt = DateTime.Now;
                }
            if (tempTask.PriorityID == 0 && CBPriority.Items.Count > 0)
            {
                tempTask.PriorityID = ((TaskPriorities)CBPriority.Items[0]).PriorityID;
            }
            context.SaveChanges();
                MessageBox.Show("Задача сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                FrameObject.frameMain.GoBack();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.GoBack();
        }
    }
}
