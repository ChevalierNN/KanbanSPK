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
using System.Data.Entity;

namespace FTSControl.Pages
{
    /// <summary>
    /// Логика взаимодействия для Worker.xaml
    /// </summary>
    public partial class Worker : Page
    {
        public Worker()
        {
            InitializeComponent();
            TBHello.Text = $"Здравствуйте,\n{User.FirstName} {User.Patronymic}!";
            LoadTasks();
        }

        private void ButtonEditTask_Click(object sender, RoutedEventArgs e)
        {
            var task = (sender as Button)?.DataContext as Tasks;
            if (task != null)
            {
                if (task.AssignedToUserID != User.Id)
                {
                    MessageBox.Show("Вы не можете редактировать чужую задачу!", "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (task.CurrentStatusID != 1)
                {
                    MessageBox.Show("Вы можете редактировать только задачи со статусом «В работе».", "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                FrameObject.frameMain.Navigate(new AddEditTask(task));
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            User.Clear(); 
            FrameObject.frameMain.Navigate(new Autorization());
        }

        private void LoadTasks()
        {
            var context = ConnectObject.GetConnect();
            var userId = User.Id;

            DGridInProcess.ItemsSource = context.Tasks.Where(t => t.AssignedToUserID == userId && t.CurrentStatusID == 1).ToList();
            DGridOnReview.ItemsSource = context.Tasks.Where(t => t.AssignedToUserID == userId && t.CurrentStatusID == 2).ToList();
            DGridDone.ItemsSource = context.Tasks.Where(t => t.AssignedToUserID == userId && t.CurrentStatusID == 3).ToList();

        }

        private void ButtonComments_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is Tasks task)
            {
                FrameObject.frameMain.Navigate(new Comments(task.TaskID));
            }
        }
    }
}
