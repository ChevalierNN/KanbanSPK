using FTSControl.Data;
using Microsoft.Win32;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
                    FileName = $"SPKanban_Tasks_{DateTime.Now:yyyyMMdd_HHmmss}.json",
                    Title = "Экспорт задач"
                };

                if (saveFileDialog.ShowDialog() != true) return;

                var context = ConnectObject.GetConnect();

                var allTasks = context.Tasks.Select(t => new
                    {t.TaskID, t.Title, t.Description, Priority = t.TaskPriorities != null ? t.TaskPriorities.PriorityName : null,
                    DueDate = t.DueDate, CreatedAt = t.CreatedAt, UpdatedAt = t.UpdatedAt, AssignedToUserID = t.AssignedToUserID,
                    StatusID = t.CurrentStatusID}).ToList();

                string jsonString = JsonConvert.SerializeObject(allTasks, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include
                    });

                File.WriteAllText(saveFileDialog.FileName, jsonString, new UTF8Encoding(false));

                MessageBox.Show($"Экспорт завершён!\nВсего задач: {allTasks.Count}","Успех",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта:\n{ex.Message}","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                    FileName = $"SPKanban_Tasks_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Title = "Экспорт задач в Excel"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                var context = ConnectObject.GetConnect();

                var inProcess = context.Tasks
                    .Where(t => t.CurrentStatusID == 1)
                    .Select(t => new
                    {
                        ID = t.TaskID,
                        Название = t.Title,
                        Описание = t.Description,
                        Срочность = t.TaskPriorities != null ? t.TaskPriorities.PriorityName : "",
                        Дедлайн = t.DueDate,
                        Создана = t.CreatedAt,
                        Исполнитель = t.Users != null ? t.Users.LastName : "",
                        Статус = "В работе"
                    }).ToList();

                var onReview = context.Tasks
                    .Where(t => t.CurrentStatusID == 2)
                    .Select(t => new
                    {
                        ID = t.TaskID,
                        Название = t.Title,
                        Описание = t.Description,
                        Срочность = t.TaskPriorities != null ? t.TaskPriorities.PriorityName : "",
                        Дедлайн = t.DueDate,
                        Создана = t.CreatedAt,
                        Исполнитель = t.Users != null ? t.Users.LastName : "",
                        Статус = "На проверке"
                    }).ToList();

                var done = context.Tasks
                    .Where(t => t.CurrentStatusID == 3)
                    .Select(t => new
                    {
                        ID = t.TaskID,
                        Название = t.Title,
                        Описание = t.Description,
                        Срочность = t.TaskPriorities != null ? t.TaskPriorities.PriorityName : "",
                        Дедлайн = t.DueDate,
                        Создана = t.CreatedAt,
                        Исполнитель = t.Users != null ? t.Users.LastName : "",
                        Статус = "Готово"
                    }).ToList();

                using (var package = new ExcelPackage())
                {
                    var ws1 = package.Workbook.Worksheets.Add("В работе");
                    LoadCollectionToWorksheet(ws1, inProcess);

                    var ws2 = package.Workbook.Worksheets.Add("На проверке");
                    LoadCollectionToWorksheet(ws2, onReview);

                    var ws3 = package.Workbook.Worksheets.Add("Готово");
                    LoadCollectionToWorksheet(ws3, done);

                    package.SaveAs(new FileInfo(saveFileDialog.FileName));
                }

                MessageBox.Show(
                    $"Экспорт завершён!\nВсего задач: {inProcess.Count + onReview.Count + done.Count}",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка экспорта:\n{ex.Message}\n\n{ex.InnerException?.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadCollectionToWorksheet(ExcelWorksheet ws, object data)
        {
            var dataTable = ToDataTable(data);
            var range = ws.Cells.LoadFromDataTable(dataTable, true);

            foreach (var col in dataTable.Columns)
            {
                var colIndex = ((System.Data.DataColumn)col).Ordinal + 1;
                ws.Column(colIndex).AutoFit();
            }

            using (var headerRange = ws.Cells[1, 1, 1, dataTable.Columns.Count])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 235, 245, 255));
                headerRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            for (int i = 1; i <= dataTable.Columns.Count; i++)
            {
                if (dataTable.Columns[i - 1].ColumnName == "Дедлайн" ||
                    dataTable.Columns[i - 1].ColumnName == "Создана")
                {
                    ws.Column(i).Style.Numberformat.Format = "dd.mm.yyyy hh:mm";
                }
            }
        }
        private DataTable ToDataTable(object collection)
        {
            var dataTable = new DataTable();

            if (collection == null)
                return dataTable;

            var enumerable = collection as System.Collections.IEnumerable;
            if (enumerable == null)
                return dataTable;

            var items = enumerable.Cast<object>().ToList();

            if (!items.Any())
                return dataTable;

            var properties = items[0].GetType().GetProperties();

            foreach (var prop in properties)
            {
                var columnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                if (columnType == typeof(DateTime))
                {
                    dataTable.Columns.Add(prop.Name, typeof(string));
                }
                else
                {
                    dataTable.Columns.Add(prop.Name, columnType);
                }
            }

            foreach (var item in items)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item, null);

                    if (value == null)
                        return DBNull.Value;

                    if (value is DateTime dt)
                        return dt.ToString("dd.MM.yyyy HH:mm");

                    return value;
                }).ToArray();

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
