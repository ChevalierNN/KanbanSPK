using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace FTSControl.Pages
{
    /// <summary>
    /// Логика взаимодействия для CaptchaWindow.xaml
    /// </summary>
    public partial class CaptchaWindow : Window
    {
        private List<Image> _puzzlePieces = new List<Image>();
        private bool _isDragging = false;
        private Image _draggedImage;
        private Point _offset;
        private bool _isCaptchaPassed = false;
        public bool IsCaptchaPassed
        {
            get { return _isCaptchaPassed; }
        }
        public CaptchaWindow()
        {
            InitializeComponent();
            InitializePuzzle();
        }
        private void InitializePuzzle()
        {
            PuzzleCanvas.Children.Clear();
            _puzzlePieces.Clear();

            string captchaPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"..\..\CaptchaResource");

            for (int i = 1; i <= 4; i++)
            {
                string imagePath = System.IO.Path.Combine(captchaPath, $"{i}.png");
                if (!File.Exists(imagePath))
                {
                    MessageBox.Show($"Не найден файл: {imagePath}");
                    continue;
                }
                Image image = new Image
                {
                    Width = 80,
                    Height = 80,
                    Tag = i 
                };

                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.EndInit();
                    image.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                    continue;
                }

                Random rand = new Random();
                Canvas.SetLeft(image, rand.Next(0, 120));
                Canvas.SetTop(image, rand.Next(0, 120));

                image.MouseDown += Image_MouseDown;
                image.MouseMove += Image_MouseMove;
                image.MouseUp += Image_MouseUp;

                PuzzleCanvas.Children.Add(image);
                _puzzlePieces.Add(image);
            }

            ResultText.Text = "";
        }


        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _draggedImage = sender as Image;

            if (_draggedImage != null)
            {
                _isDragging = true;
                _offset = e.GetPosition(_draggedImage);
                _draggedImage.CaptureMouse(); 
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _draggedImage != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(PuzzleCanvas);
                Canvas.SetLeft(_draggedImage, currentPosition.X - _offset.X);
                Canvas.SetTop(_draggedImage, currentPosition.Y - _offset.Y);
            }
        }


        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _draggedImage.ReleaseMouseCapture();
                _draggedImage = null;
            }
        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            Point[] targetPositions = new[] {
                new Point(0, 0),   
                new Point(80, 0),  
                new Point(0, 80),  
                new Point(80, 80)  
            };

            bool isCorrect = true;

            for (int i = 0; i < _puzzlePieces.Count; i++)
            {
                double left = Canvas.GetLeft(_puzzlePieces[i]);
                double top = Canvas.GetTop(_puzzlePieces[i]);
                Point target = targetPositions[i];
                byte tolerance = 7;

                if (Math.Abs(left - target.X) > tolerance || Math.Abs(top - target.Y) > tolerance)
                {
                    isCorrect = false;
                    break;
                }
            }

            if (isCorrect)
            {
                _isCaptchaPassed = true;
                ResultText.Text = "Проверка пройдена!";
                ResultText.Foreground = Brushes.Green;

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    this.DialogResult = true; 
                };
                timer.Start();
            }
            else
            {
                ResultText.Text = "Пазл собран неверно! Попробуйте еще раз";
                ResultText.Foreground = Brushes.Red;
            }
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {

            InitializePuzzle();
        }
    }
}
