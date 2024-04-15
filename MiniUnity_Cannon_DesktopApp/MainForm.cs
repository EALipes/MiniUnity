using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using MiniUnity;

// Используются для реализации INotifyPropertyChanged
//using System.Runtime.CompilerServices;
//using MiniUnity_Cannon_DesktopApp.Annotations;

using MiniUnity.CannonGame;
//using MiniUnity_CannonGame;
using MiniUnity_Cannon_DesktopApp.Properties;


namespace MiniUnity_Cannon_DesktopApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            gameParams = new GameParameters();

            InitializeComponent();


            //// События редактирования настроек игры
            //// Изменение параметров при ручном редактировании элементов управления
            //// Перенесено из InitializeComponent();
            //this.VelocityEdit.ValueChanged += new System.EventHandler(this.VelocityEdit_ValueChanged);
            //this.AngleEdit.ValueChanged += new System.EventHandler(this.AngleEdit_ValueChanged);

            //this.TimeScaleEdit.ValueChanged += new System.EventHandler(this.TimeScaleEdit_ValueChanged);
            //this.ScaleEdit.ValueChanged += new System.EventHandler(this.ScaleEdit_ValueChanged);
            //this.FramePerSecEdit.ValueChanged += new System.EventHandler(this.FramePerSecEdit_ValueChanged);

            //// Изменение элементов управления при изменении свойств gameParams
            //// Подключение реакции на изменение Velocity
            //gameParams.PropertyChanged += VelocityChanged_to_VelocityEdit;
            //// Подключение реакции на изменение Angle
            //gameParams.PropertyChanged += AngleChanged_to_AngleEdit;



            // Подключение к событиям через привязку
            Binding b = new Binding(nameof(VelocityEdit.Value), gameParams, nameof(GameParams.Speed), 
                formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            VelocityEdit.DataBindings.Add(b);

            Binding b2 = new Binding("Value", gameParams, nameof(GameParams.Speed));
            VelocityProgressBar1.DataBindings.Add(b2);

            Binding b3 = new Binding("Value", gameParams, nameof(GameParams.Speed),
            formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            VelocityTrackBar1.DataBindings.Add(b3);

            Binding bA = new Binding("Value", gameParams, nameof(GameParams.Angle),
            formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            AngleEdit.DataBindings.Add(bA);

            Binding bTS = new Binding("Value", gameParams, nameof(GameParams.TimeScale),
                formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            this.TimeScaleEdit.DataBindings.Add(bTS); //ValueChanged += new System.EventHandler(this.TimeScaleEdit_ValueChanged);

            Binding bS = new Binding("Value", gameParams, nameof(GameParams.GameScreenScale),
                formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            this.ScaleEdit.DataBindings.Add(bS); //ValueChanged += new System.EventHandler(this.ScaleEdit_ValueChanged);
            
            Binding bFPS = new Binding("Value", gameParams, nameof(GameParams.FramesPerSec),
                formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            this.FramePerSecEdit.DataBindings.Add(bFPS); //ValueChanged += new System.EventHandler(this.FramePerSecEdit_ValueChanged);


            // Обработка клавиатуры
            //KeyPress -= MainForm_KeyPress;
            //GameCanvasPanel.KeyPress += MainForm_KeyPress;
            //KeyDown -= MainForm_KeyDown;
            //GameCanvasPanel.KeyDown += MainForm_KeyDown;


            // Инициализация игры
            // (В консольном приложении это сделано в Main(), тут удобнее это  сделать при инициализации главной формы.
            game = new CannonGame();
            GameObject.AppType = GameObject.ApplicationType.WinFormsApp;
            // настройка отображения игры в панели
            game.GamePanel = GameCanvasPanel;
            // настройка получения клавиатурных нажатий
            game.KeyboardEventsControl = this;
        }

        #region Параметры игры
        private readonly GameParameters gameParams;

        

        #endregion

        #region Объекты игры
        protected CannonGame game;

        #endregion

        #region Вспомогательные переменные
        private SoundPlayer SoundPlayerGunFired;
        private SoundPlayer SoundPlayerFlight;
        private SoundPlayer SoundPlayerFall;

        //public bool ProjectileIsFlying { get; set; } = false;

        public GameParameters GameParams
        {
            get { return gameParams; }
        }

        public CannonGame Game
        {
            set { game = value; }
            get { return game; }
        }

        public GameParameters GameParams1
        {
            get { return gameParams; }
        }

        private DateTime ProjectileFlightStartTime;

        #endregion


        private void RunButton_Click(object sender, EventArgs e)
        {
            ControlPanel.Enabled = false;
            GameParams.SetGameSettings(game);
            try
            {
                game.Play();
                GameParams.GetGameSettings(game);
            }
            finally
            {
                ControlPanel.Enabled = true;
            }

        }


        #region События редактирования данных

        // События при редактировании данных руками

        private void TimeScaleEdit_ValueChanged(object sender, EventArgs e)
        {
            GameParams.TimeScale = (float)TimeScaleEdit.Value;
        }

        private void FramePerSecEdit_ValueChanged(object sender, EventArgs e)
        {
            GameParams.FramesPerSec = (int)FramePerSecEdit.Value;
        }

        private void ScaleEdit_ValueChanged(object sender, EventArgs e)
        {
            GameParams.GameScreenScale = (float)ScaleEdit.Value;
        }

        private void AngleEdit_ValueChanged(object sender, EventArgs e)
        {
            GameParams.Angle = (float)AngleEdit.Value;
        }

        private void VelocityEdit_ValueChanged(object sender, EventArgs e)
        {
            GameParams.Speed = (float) VelocityEdit.Value;
        }

        
        // Отработка событий изменения данных "самой программой" - чтоб изменения отразились в элементах управления

        private void VelocityChanged_to_VelocityEdit(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GameParams.Speed) )
            //if (e.PropertyName == "Velocity")
                VelocityEdit.Value = (decimal)(sender as GameParameters)?.Speed;
        }

        private void AngleChanged_to_AngleEdit(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GameParams.Angle))
            //if (e.PropertyName == "Angle")
                AngleEdit.Value = (decimal)(sender as GameParameters)?.Angle;
        }


        #endregion

        private void GameCanvasPanel_Paint(object sender, PaintEventArgs e)
        {
            //var paintcalled = true;
        }

        private void GameCanvasPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //e.
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show("Нажата клавиша " + e.KeyCode +"   "+ e.KeyValue + " " + e.Modifiers);
            //e.Handled = true;
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            //MessageBox.Show("Нажата клавиша " + e.KeyChar);

            //e.Handled = true;
        }
    }



}
