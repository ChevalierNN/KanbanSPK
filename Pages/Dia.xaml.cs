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

        private void DrawPieChart()
        {
            PieChartCanvas.Children.Clear();

            var context = ConnectObject.GetConnect();

            AllQuests.Text = $"Всего задач: {context.Tasks.Count()}";

            int inProcess = context.Tasks.Count(t => t.CurrentStatusID == 1);
            int onReview = context.Tasks.Count(t => t.CurrentStatusID == 2);
            int done = context.Tasks.Count(t => t.CurrentStatusID == 3);
            if (inProcess == 0 && onReview == 0 && done == 0)
            {
                var text = new TextBlock
                {
                    Text = "Нет задач",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 16
                };
                PieChartCanvas.Children.Add(text);
                return;
            }

            double centerX = PieChartCanvas.Width / 2;
            double centerY = PieChartCanvas.Height / 2;
            double radius = Math.Min(centerX, centerY) - 10;

            var colors = new[] { Brushes.LightBlue, Brushes.LightGoldenrodYellow, Brushes.LightGreen };
            var values = new[] { inProcess, onReview, done };
            var labels = new[] { $"В работе ({inProcess})", $"На проверке ({onReview})", $"Готово ({done})" };

            double startAngle = -90; 

            for (int i = 0; i < 3; i++)
            {
                if (values[i] == 0) continue; 

                double sweepAngle = 360.0 * values[i] / (inProcess + onReview + done); 

                var geometry = new PathGeometry();
                var figure = new PathFigure();
                figure.StartPoint = new Point(centerX, centerY);

                double startX = centerX + radius * Math.Cos(startAngle * Math.PI / 180);
                double startY = centerY + radius * Math.Sin(startAngle * Math.PI / 180);
                figure.Segments.Add(new LineSegment(new Point(startX, startY), true));

                double endX = centerX + radius * Math.Cos((startAngle + sweepAngle) * Math.PI / 180);
                double endY = centerY + radius * Math.Sin((startAngle + sweepAngle) * Math.PI / 180);
                figure.Segments.Add(new ArcSegment(
                    new Point(endX, endY),
                    new Size(radius, radius),
                    0,
                    sweepAngle > 180,
                    SweepDirection.Clockwise,
                    true
                ));

                figure.Segments.Add(new LineSegment(new Point(centerX, centerY), true));
                figure.IsClosed = true;

                geometry.Figures.Add(figure);

                var path = new Path
                {
                    Data = geometry,
                    Fill = colors[i],
                    Stroke = Brushes.White,
                    StrokeThickness = 1
                };

                PieChartCanvas.Children.Add(path);

                double midAngle = startAngle + sweepAngle / 2;
                double textRadius = radius * 0.6;
                double textX = centerX + textRadius * Math.Cos(midAngle * Math.PI / 180);
                double textY = centerY + textRadius * Math.Sin(midAngle * Math.PI / 180);

                var textBlock = new TextBlock
                {
                    Text = labels[i],
                    FontSize = 10,
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Canvas.SetLeft(textBlock, textX - 30);
                Canvas.SetTop(textBlock, textY - 10);
                PieChartCanvas.Children.Add(textBlock);

                startAngle += sweepAngle;
            }
        }

        private void LoadTasks()
        {
           var context = ConnectObject.GetConnect();
           DrawPieChart(); 

        }
    }
}
