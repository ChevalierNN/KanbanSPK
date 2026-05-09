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
    /// Логика взаимодействия для Dia.xaml
    /// </summary>
    public partial class Dia : Page
    {
        public Dia()
        {
            InitializeComponent();
            LoadTasks();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            FrameObject.frameMain.GoBack();
        }
        // Метод для отрисовки диаграммы
        private void DrawPieChart()
        {
            PieChartCanvas.Children.Clear();
            var context = ConnectObject.GetConnect();
            var tasks = context.Tasks.ToList();
            var counts = new[] { 1, 2, 3 }.Select(s => tasks.Count(t => t.CurrentStatusID == s)).ToArray();
            int total = counts.Sum();

            AllQuests.Text = $"Всего задач: {tasks.Count}";
            if (total == 0)
            {
                PieChartCanvas.Children.Add(new TextBlock { Text = "Нет задач", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontSize = 16 });
                return;
            }

            double cx = PieChartCanvas.Width / 2, cy = PieChartCanvas.Height / 2, r = Math.Min(cx, cy) - 10;
            var colors = new[] { Brushes.LightBlue, Brushes.LightGoldenrodYellow, Brushes.LightGreen };
            var labels = new[] { $"В работе ({counts[0]})", $"На проверке ({counts[1]})", $"Готово ({counts[2]})" };
            double startAngle = -90;

            for (int i = 0; i < 3; i++)
            {
                if (counts[i] == 0) continue;
                double sweep = 360.0 * counts[i] / total;
                double rad = Math.PI / 180;

                var fig = new PathFigure { StartPoint = new Point(cx, cy), IsClosed = true };
                fig.Segments.Add(new LineSegment(new Point(cx + r * Math.Cos(startAngle * rad), cy + r * Math.Sin(startAngle * rad)), true));
                fig.Segments.Add(new ArcSegment(new Point(cx + r * Math.Cos((startAngle + sweep) * rad), cy + r * Math.Sin((startAngle + sweep) * rad)), new Size(r, r), 0, sweep > 180, SweepDirection.Clockwise, true));
                fig.Segments.Add(new LineSegment(new Point(cx, cy), true));

                PieChartCanvas.Children.Add(new Path { Data = new PathGeometry { Figures = { fig } }, Fill = colors[i], Stroke = Brushes.White, StrokeThickness = 1 });

                double mid = startAngle + sweep / 2, tr = r * 0.6;
                var tb = new TextBlock { Text = labels[i], FontSize = 10, Foreground = Brushes.Black };
                Canvas.SetLeft(tb, cx + tr * Math.Cos(mid * rad) - 30);
                Canvas.SetTop(tb, cy + tr * Math.Sin(mid * rad) - 10);
                PieChartCanvas.Children.Add(tb);

                startAngle += sweep;
            }
        }

        private void LoadTasks()
        {
           var context = ConnectObject.GetConnect();
           DrawPieChart(); 
        }
    }
}
