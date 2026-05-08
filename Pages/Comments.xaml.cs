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
    /// Логика взаимодействия для Comments.xaml
    /// </summary>
    public partial class Comments : Page
    {
        private int _currentTaskId;
        public Comments(int taskId)
        {
            InitializeComponent();
            _currentTaskId = taskId;
            LoadComments();
        }

        private void LoadComments()
        {
            try
            {
                var context = ConnectObject.GetConnect();

                var comments = context.TaskComments.Include(c => c.Users).Where(c => c.TaskID == _currentTaskId) .OrderByDescending(c => c.CreatedAt).ToList();

                DGridComments.ItemsSource = comments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке комментариев: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonComments_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.Navigate(new AddComments(_currentTaskId));
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.GoBack();
        }
    }
}
