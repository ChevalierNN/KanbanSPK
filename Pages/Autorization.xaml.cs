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
        public Autorization()
        {
            InitializeComponent();
        }
        // Метод для аунтификации пользователя
        private void Enter(object sender, RoutedEventArgs e)
        {
            var context = ConnectObject.GetConnect();
            Error.Text = string.Empty;
            if (string.IsNullOrEmpty(Login.Text) && string.IsNullOrEmpty(Password.Password))
            {
                Error.Text = "Введите логин и пароль!";
                return;
            }
            if (string.IsNullOrEmpty(Login.Text))
            {
                Error.Text = "Введите логин!";
                Login.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Password.Password))
            {
                Error.Text = "Введите пароль!";
                Password.Focus();
                return;
            }

            var user = ConnectObject.GetConnect().Users.FirstOrDefault(u => u.Login == Login.Text); // подключение базы данных

            if (user == null)
            {
                Error.Text = "Пользователь не найден!";
                return;
            }
            if (user.StatusID == 2)
            {
                MessageBox.Show("Ваш аккаунт был заблокирован! Обратитесь к администратору.", "Вы заблокированы", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (user.Password == Password.Password)
            {
                var captchaWindow = new Captcha();
                bool? captchaResult = captchaWindow.ShowDialog();

                if (captchaResult == true)
                {
                    user.MistakeCount = 0;
                    context.SaveChanges();
                    switch (user.RoleID)
                    {
                        case 1:
                            FrameObject.frameMain.Navigate(new Admin());
                            break;
                        case 2:
                            FrameObject.frameMain.Navigate(new Manager());
                            break;
                        case 3:
                            FrameObject.frameMain.Navigate(new Worker());
                            break;
                    }
                }
                else
                {
                    user.MistakeCount++;
                    if (user.MistakeCount >= 3)
                    {
                        user.StatusID = 2;
                        context.SaveChanges();
                        MessageBox.Show("Капча решена неверно. Аккаунт заблокирован.", "Блокировка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        context.SaveChanges();
                        MessageBox.Show($"Капча решена неверно! Осталось {3 - user.MistakeCount} попыток.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                user.MistakeCount++;

                if (user.MistakeCount >= 3)
                {
                    user.StatusID = 2;
                    context.SaveChanges();
                    MessageBox.Show("Пароль неверный. Аккаунт заблокирован.", "Блокировка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    context.SaveChanges();
                    MessageBox.Show($"Пароль неверный! Осталось {3 - user.MistakeCount} попыток.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}