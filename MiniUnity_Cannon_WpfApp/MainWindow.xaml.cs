using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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

using MiniUnity.CannonGame;



namespace MiniUnity_Cannon_WpfApp
{
    /// <summary> Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            gameParams = new GameParameters();

            InitializeComponent();
            
            // Подключение (привязка) элементов управления 
            
            var b = new Binding("Speed");
            b.Source = gameParams;
            VelocityEdit.SetBinding(TextBox.TextProperty, b);

            var b2 = new Binding("Speed");
            b2.Source = gameParams;
            VelocitySlider.Minimum = 0;
            VelocitySlider.Maximum = 2000;
            //VelocitySlider.Ticks.
            VelocitySlider.SetBinding(Slider.ValueProperty, b2);

            var bA = new Binding("Angle");
            bA.Source = gameParams;
            AngleEdit.SetBinding(TextBox.TextProperty, bA);

            var bA2 = new Binding("Angle");
            bA2.Source = gameParams;
            AngleSlider.Minimum = 0;
            AngleSlider.Maximum = 90;
            AngleSlider.SetBinding(Slider.ValueProperty, bA2);


            //GameCanvas.
            gameParams.PropertyChanged += GameParamsPropertyChanged;

            // Инициализация игры
            // (В консольном приложении это сделано в Main(), тут удобнее это  сделать при инициализации главной формы.
            game = new CannonGame();

            // Отрисовка - отладочный говнокод
            //game.projectile.OnCallScreenRefresh = CallScreenRefresh;
            game.projectile.Start(); //DEBUG //только для инициализации, пока не налажен механизм вызова Paint через объект Game и Scene

        }

        private void GameParamsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Angle")
            {
                //CannonRect.GeometryTransform.
                //RotateTransform.Angle = -gameParams.Angle;
            }
        }

        /// <summary> Отрисовка пушки и ядра
        /// </summary>
        private void CallScreenRefresh()
        {
            Vector3 Position = game.projectile.Position;

            var prX = (double) Position.X;
            var prY = (double) Position.Y;
            //var prX = (float) Position.X;
            //var prY = (float) Position.Y;
            // отмасштабируем эти координаты, чтоб все вместилось в экран
            // на экране в WPF используются независимые от устройства единицы, равные 1/96 дюйма
            // т.е. 1 ед = 1/96 * 2.54 см
            // в 1 см - 96/2.54 единиц
            // 1 см =  37.79 единиц
            var ScaleCoeff = 37.79;
            //var ScaleCoeff = 37.79f;
            prX = prX * ScaleCoeff / game.ScreenScale;
            prY = prY * ScaleCoeff / game.ScreenScale;
            // учтем размер рисуемого прямоугольника, и соответственно сместим его начало
            // учтем, что началом координаты Y у нас должен быть конец (нижний) экрана
            // и что координата Y в игре направлена вверх, а у нас на экране - вниз
            var projectileRectSize = 10;
            var screenHeight = GameCanvasGrid.ActualHeight;
            var screenX = prX;
            var screenY = screenHeight - projectileRectSize - prY;


            ////var left = ProjectileEllipse.GetValue(Canvas.LeftProperty);
            ////var top = ProjectileEllipse.GetValue(Canvas.TopProperty);
            ////var left2 = Canvas.GetLeft(ProjectileEllipse);
            ////var top2 = Canvas.GetTop(ProjectileEllipse);

            ////Canvas.SetLeft(ProjectileEllipse, screenX);
            ////Canvas.SetTop(ProjectileEllipse, screenY);

            //ProjectileEllipse.SetValue(Canvas.LeftProperty, screenX);
            //ProjectileEllipse.SetValue(Canvas.TopProperty, screenY);

            Debug.WriteLine("screenX="+screenX.ToString("F1")+" screenY="+screenY.ToString("F1"));



            // *** Говнокод --------------------------------
            
            // Пытаемся что-то нарисовать на форме

            GameCanvasGrid.InvalidateVisual();

            var drawingGroup = new DrawingGroup();
            
            //var projectileEllipse = new Ellipse();
            //projectileEllipse.Width = projectileRectSize;
            //projectileEllipse.Height = projectileRectSize;
            //Canvas.SetLeft(projectileEllipse, screenX);
            //Canvas.SetTop(projectileEllipse, screenY);

            // строим кружок в рассчитанных координатах
            var projectileEllipse = new EllipseGeometry(new Point(screenX, screenY), 5, 5);
            
            GeometryGroup geometryGroup = new GeometryGroup();
            geometryGroup.Children.Add(projectileEllipse);

            // Сохраняем описание геометрии
            GeometryDrawing geometryDrawing = new GeometryDrawing();
            //geometryDrawing.Geometry = geometryGroup;
            geometryDrawing.Geometry = projectileEllipse;
    
            // Настраиваем перо
            geometryDrawing.Pen = new Pen(Brushes.Blue, 0.005);
    
            // Добавляем готовый слой в контейнер отображения
            drawingGroup.Children.Add(geometryDrawing);

            Image1.Source = new DrawingImage(drawingGroup);
            //Image1.Source = new DrawingImage(projectileEllipse);

        }

        #region Параметры игры
        private readonly GameParameters gameParams;
        #endregion

        #region Объекты игры
        protected CannonGame game;

        #endregion

        private void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            gameParams.SetGameSettings(game);
            game.Play();
        }
    }
}
