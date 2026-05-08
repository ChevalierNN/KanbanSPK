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
    /// Логика взаимодействия для Manager.xaml
    /// </summary>
    public partial class Manager : Page
    {
        public Manager()
        {
            InitializeComponent();
            TBHello.Text = $"Здравствуйте,\n{User.FirstName} {User.Patronymic}!";
            LoadTasks(); 
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.Navigate(new AddEditTask()); 
        }

        private void ButtonEditTask_Click(object sender, RoutedEventArgs e)
        {
            var task = (sender as Button)?.DataContext as Tasks;
            if (task != null)
            {
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

            DGridInProcess.ItemsSource = context.Tasks.Where(t => t.CurrentStatusID == 1).ToList();
            DGridOnReview.ItemsSource = context.Tasks.Where(t => t.CurrentStatusID == 2).ToList();
            DGridDone.ItemsSource = context.Tasks.Where(t => t.CurrentStatusID == 3).ToList();
        }

        private void Dia_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.Navigate(new Dia());
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
