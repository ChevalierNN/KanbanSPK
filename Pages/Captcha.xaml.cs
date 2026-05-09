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
using System.Windows.Shapes;

namespace FTSControl.Pages
{
    /// <summary>
    /// Логика взаимодействия для Captcha.xaml
    /// </summary>
    public partial class Captcha : Window
    {
        private readonly Image[] images;
        private readonly int[] rotat = new int[4];
        public Captcha()
        {
            InitializeComponent();
            images = new[] { ARB, ARC, ARD, ARF };
            Random rand = new Random();
            for (int i = 0; i < 4; i++)
            {
                rotat[i] = rand.Next(0, 4) * 90;
                ApplyRotat(images[i], rotat[i]);
            }
        }
        // Поворачивает изображение на 90 градусов
        private void Selected_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = (Image)sender;
            int index = Array.IndexOf(images, img);
            if (index == -1) return;
            rotat[index] = (rotat[index] + 90) % 360;
            ApplyRotat(img, rotat[index]);

        }
        // Проверка все ли изображения повёрнуты верно
        private void CheckBTN(object sender, RoutedEventArgs e)
        {
            bool isCorrect = Array.TrueForAll(rotat, a => a == 0);
            DialogResult = isCorrect;
            Close();
        }
        private void ApplyRotat(Image img, int rotat)
        {
            img.RenderTransformOrigin = new Point(0.5, 0.5);
            img.RenderTransform = new RotateTransform(rotat);
        }

    }
}
