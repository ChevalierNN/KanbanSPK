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
    /// Логика взаимодействия для AddComments.xaml
    /// </summary>
    public partial class AddComments : Page
    {
        private readonly int _taskId;
        public AddComments(int taskId, int commentId = 0)
        {
            InitializeComponent();
            _taskId = taskId;
        }

        private void AddComment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = InputComments.Text?.Trim();
                if (string.IsNullOrWhiteSpace(text))
                {
                    MessageBox.Show("Текст комментария не может быть пустым.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var context = ConnectObject.GetConnect();

                var newComment = new TaskComments
                {
                    TaskID = _taskId,
                    UserID = 1, 
                    CommentText = text,
                    CreatedAt = DateTime.Now
                };

                context.TaskComments.Add(newComment);
                context.SaveChanges();

                MessageBox.Show("Комментарий добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                FrameObject.frameMain.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.GoBack();
        }
    }
}