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
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Page
    {
        private int _failedAttempts = 0;
        private const int MaxAttempts = 3;
        public Autorization()
        {
            InitializeComponent();
        }
        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            TBLogin.BorderBrush = Brushes.Gray;
            TBPassword.BorderBrush = Brushes.Gray;

            if (string.IsNullOrWhiteSpace(TBLogin.Text))
            {
                MessageBox.Show("Введите логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                TBLogin.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(TBPassword.Password))
            {
                MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                TBPassword.Focus();
                return;
            }

            var context = ConnectObject.GetConnect();

            var user = context.Users
                .FirstOrDefault(u => u.Login == TBLogin.Text);

            if (user == null)
            {
                _failedAttempts++;
                if (_failedAttempts >= MaxAttempts)
                {
                    MessageBox.Show("Превышено количество попыток.\nОбратитесь к администратору.", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    _failedAttempts = 0; 
                }
                else
                {
                    MessageBox.Show($"Неверный логин или пароль. Осталось попыток: {MaxAttempts - _failedAttempts}", "Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return;
            }

            if (user.Password != TBPassword.Password)
            {
                _failedAttempts++;

                if (_failedAttempts >= MaxAttempts)
                {
                    user.StatusID = 2; 
                    try
                    {
                        context.SaveChanges();
                        MessageBox.Show($"{user.FirstName} {user.LastName}, ваш аккаунт заблокирован!\nОбратитесь к администратору.", "Доступ запрещён",MessageBoxButton.OK, MessageBoxImage.Warning);
                        _failedAttempts = 0; 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка блокировки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return;
                }
                else
                {
                    MessageBox.Show($"Неверный пароль. Осталось попыток: {MaxAttempts - _failedAttempts}", "Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return;
            }
            _failedAttempts = 0;
            if (user.StatusID == 2)
            {
                MessageBox.Show($"{user.FirstName} {user.LastName}, ваш аккаунт заблокирован!\nОбратитесь к администратору.", "Доступ запрещён",MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var captcha = new CaptchaWindow();
            if (captcha.ShowDialog() != true || !captcha.IsCaptchaPassed)
            {
                MessageBox.Show("Проверка безопасности не пройдена.", "Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            User.Id = user.UserID;
            User.Login = user.Login;
            User.FirstName = user.FirstName;
            User.LastName = user.LastName;
            User.Patronymic = user.Patronymic;
            User.RoleID = user.RoleID;
            User.StatusID = user.StatusID;

            MessageBox.Show($"Добро пожаловать, {User.FirstName}!", "Успех",MessageBoxButton.OK, MessageBoxImage.Information);

            switch (user.RoleID)
            {
                case 1: FrameObject.frameMain.Navigate(new Admin()); break;
                case 2: FrameObject.frameMain.Navigate(new Manager()); break;
                case 3: FrameObject.frameMain.Navigate(new Worker()); break;
                default: MessageBox.Show("Неизвестная роль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); break;
            }
        }

        private void TB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text)) tb.BorderBrush = Brushes.Red;
            else if (sender is PasswordBox pb && string.IsNullOrWhiteSpace(pb.Password)) pb.BorderBrush = Brushes.Red;
        }
    }
}