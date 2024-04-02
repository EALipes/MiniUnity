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

using MiniUnity.CannonGame;



namespace MiniUnity_Cannon_WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
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


            // Инициализация игры
            // (В консольном приложении это сделано в Main(), тут удобнее это  сделать при инициализации главной формы.
            game = new CannonGame();
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
