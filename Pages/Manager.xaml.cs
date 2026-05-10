using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
using Newtonsoft.Json;
using Microsoft.Win32;

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
        // Изменение задачи
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
        // Загрузка задач
        private void LoadTasks()
        {
            var context = ConnectObject.GetConnect();

            DGridInProcess.ItemsSource = context.Tasks.Where(t => t.CurrentStatusID == 1).ToList();
            DGridOnReview.ItemsSource = context.Tasks.Where(t => t.CurrentStatusID == 2).ToList();
            DGridDone.ItemsSource = context.Tasks.Where(t => t.CurrentStatusID == 3).ToList();
        }
        // Открытие диаграммы
        private void Dia_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.Navigate(new Dia());
        }
        // Комментарии
        private void ButtonComments_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is Tasks task)
            {
                FrameObject.frameMain.Navigate(new Comments(task.TaskID));
            }
        }
        // Выгрузка задач в Json 
        private void Export(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    FileName = $"SPKanban_Export_{DateTime.Now:yyyyMMdd_HHmmss}.json",
                    Title = "Экспорт данных задач"
                };

                if (saveFileDialog.ShowDialog() != true) return;

                var context = ConnectObject.GetConnect();

                var tasksInProcess = context.Tasks.Where(t => t.CurrentStatusID == 1).Select(t => new
                    {
                        t.TaskID,
                        t.Title,
                        t.Description,
                        t.DueDate,
                        t.CreatedAt,
                        t.UpdatedAt,
                        t.AssignedToUserID,
                        Priority = t.TaskPriorities != null ? t.TaskPriorities.PriorityName : null,
                        StatusID = t.CurrentStatusID
                    }).ToList();

                var tasksOnReview = context.Tasks.Where(t => t.CurrentStatusID == 2).Select(t => new
                    {
                        t.TaskID,
                        t.Title,
                        t.Description,
                        t.DueDate,
                        t.CreatedAt,
                        t.UpdatedAt,
                        t.AssignedToUserID,
                        Priority = t.TaskPriorities != null ? t.TaskPriorities.PriorityName : null,
                        StatusID = t.CurrentStatusID
                    }).ToList();

                var tasksDone = context.Tasks
                    .Where(t => t.CurrentStatusID == 3)
                    .Select(t => new
                    {
                        t.TaskID,
                        t.Title,
                        t.Description,
                        t.DueDate,
                        t.CreatedAt,
                        t.UpdatedAt,
                        t.AssignedToUserID,
                        Priority = t.TaskPriorities != null ? t.TaskPriorities.PriorityName : null,
                        StatusID = t.CurrentStatusID
                    }).ToList();

                var exportData = new
                {
                    ExportDate = DateTime.Now,
                    ExportedBy = $"{User.FirstName} {User.Patronymic}",
                    Summary = new
                    {
                        TotalTasks = tasksInProcess.Count + tasksOnReview.Count + tasksDone.Count,
                        InProcessCount = tasksInProcess.Count,
                        OnReviewCount = tasksOnReview.Count,
                        DoneCount = tasksDone.Count
                    },
                    Data = new
                    {
                        InProcess = tasksInProcess,
                        OnReview = tasksOnReview,
                        Done = tasksDone
                    }
                };

                string jsonString = JsonConvert.SerializeObject(exportData, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include
                    });

                File.WriteAllText(saveFileDialog.FileName, jsonString, new UTF8Encoding(false));

                MessageBox.Show($"Данные успешно экспортированы!\nВсего задач: {exportData.Summary.TotalTasks}","Экспорт завершен",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте данных:\n{ex.Message}\n\nДетали:\n{ex.InnerException?.Message}","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
    }
}
