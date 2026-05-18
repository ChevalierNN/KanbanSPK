using FTSControl.Data;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
using System.Security.Cryptography;

namespace FTSControl.Pages
{
    public partial class Admin : Page
    {
        private List<Users> _originalUsersList;

        public Admin()
        {
            InitializeComponent();

            _originalUsersList = ConnectObject.GetConnect().Users.ToList();

            var displayList = _originalUsersList.Select(u => new
            {
                u.UserID,
                u.LastName,
                u.FirstName,
                u.Patronymic,
                u.Login,
                Password = HashPassword(u.Password),
                Roles = u.Roles,
                Departments = u.Departments,
                UserStatuses = u.UserStatuses,
                u.Email,
                u.Phone,
                u.CreatedAt,
                u.UpdatedAt
            }).ToList();

            DGridEmployees.ItemsSource = displayList;
        }
        // Шифрование пароля
        private string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        private void ButtonAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.Navigate(new AddEdithEmployee());
        }

        private void BackAutorization_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.GoBack();
        }

        private void ButtonEditEmployee_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            dynamic item = button?.DataContext;

            if (item != null)
            {
                int userId = item.UserID;
                var userToEdit = _originalUsersList.FirstOrDefault(u => u.UserID == userId);

                if (userToEdit != null)
                {
                    FrameObject.frameMain.Navigate(new AddEdithEmployee(userToEdit));
                }
            }
        }
        // Экспорт в виде json
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

                var employees = _originalUsersList.Select(u => new
                {
                    u.UserID,
                    u.LastName,
                    u.FirstName,
                    u.Patronymic,
                    u.Login,
                    u.Password,
                    Role = u.Roles != null ? u.Roles.RoleName : "Не указана",
                    Department = u.Departments != null ? u.Departments.DepartmentName : "Не указан",
                    Email = u.Email,
                    Phone = u.Phone,
                    Status = u.UserStatuses != null ? u.UserStatuses.StatusName : "Неизвестен",
                    u.CreatedAt,
                    u.UpdatedAt
                }).ToList();

                var exportData = employees;

                string jsonString = JsonConvert.SerializeObject(exportData, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include
                });

                File.WriteAllText(saveFileDialog.FileName, jsonString, new UTF8Encoding(false));

                MessageBox.Show($"Экспорт завершён!\nСотрудников: {employees.Count}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта:\n{ex.Message}\n\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Экспорт Excel
        private void ExportExcel(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                    FileName = $"SPKanban_Users_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Title = "Экспорт списка сотрудников"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                var employees = _originalUsersList
                    .Select(u => new
                    {
                        ID = u.UserID,
                        Фамилия = u.LastName,
                        Имя = u.FirstName,
                        Отчество = u.Patronymic,
                        Логин = u.Login,
                        Пароль = u.Password,
                        Роль = u.Roles != null ? u.Roles.RoleName : "Не указана",
                        Департамент = u.Departments != null ? u.Departments.DepartmentName : "Не указан",
                        Почта = u.Email,
                        Телефон = u.Phone,
                        Статус = u.UserStatuses != null ? u.UserStatuses.StatusName : "Неизвестен",
                        Создан = u.CreatedAt,
                        Обновлён = u.UpdatedAt
                    }).ToList();

                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Сотрудники");
                    LoadCollectionToWorksheet(ws, employees);
                    package.SaveAs(new FileInfo(saveFileDialog.FileName));
                }

                MessageBox.Show($"Экспорт завершён!\nСотрудников: {employees.Count}","Успех",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта:\n{ex.Message}\n\n{ex.InnerException?.Message}","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void LoadCollectionToWorksheet(ExcelWorksheet ws, object data)
        {
            var dataTable = ToDataTable(data);
            var range = ws.Cells.LoadFromDataTable(dataTable, true);

            for (int i = 1; i <= dataTable.Columns.Count; i++)
            {
                ws.Column(i).AutoFit();
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
                if (dataTable.Columns[i - 1].ColumnName == "Создан" ||
                    dataTable.Columns[i - 1].ColumnName == "Обновлён")
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