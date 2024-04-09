using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Threading;
using System.Windows.Forms;

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

            //// Звуки
            //SoundPlayerGunFired = new SoundPlayer(Properties.Resources.CannonFiredAndProjectileFlies);
            ////SoundPlayerGunFired = new SoundPlayer(Properties.Resources.CannonFired);
            //SoundPlayerGunFired.Load();
            //SoundPlayerFlight = new SoundPlayer(Properties.Resources.ProjectileFlight1);
            //SoundPlayerFlight.Load();
            //SoundPlayerFall = new SoundPlayer(Properties.Resources.ProjectileFall3);
            //SoundPlayerFall.Load();


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
            Binding b = new Binding("Value", gameParams, "Speed", 
                formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            VelocityEdit.DataBindings.Add(b);

            Binding b2 = new Binding("Value", gameParams, "Speed");
            VelocityProgressBar1.DataBindings.Add(b2);

            Binding b3 = new Binding("Value", gameParams, "Speed",
            formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            VelocityTrackBar1.DataBindings.Add(b3);

            Binding bA = new Binding("Value", gameParams, "Angle",
            formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            AngleEdit.DataBindings.Add(bA);

            Binding bTS = new Binding("Value", gameParams, "TimeScale",
                formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            this.TimeScaleEdit.DataBindings.Add(bTS); //ValueChanged += new System.EventHandler(this.TimeScaleEdit_ValueChanged);

            Binding bS = new Binding("Value", gameParams, "GameScreenScale",
                formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            this.ScaleEdit.DataBindings.Add(bS); //ValueChanged += new System.EventHandler(this.ScaleEdit_ValueChanged);
            
            Binding bFPS = new Binding("Value", gameParams, "FramesPerSec",
                formattingEnabled: true, dataSourceUpdateMode: DataSourceUpdateMode.OnPropertyChanged);
            this.FramePerSecEdit.DataBindings.Add(bFPS); //ValueChanged += new System.EventHandler(this.FramePerSecEdit_ValueChanged);


            // Инициализация игры
            // (В консольном приложении это сделано в Main(), тут удобнее это  сделать при инициализации главной формы.
            game = new CannonGame();
            game.projectile.OnCallScreenRefresh = CallScreenRefresh;
            game.projectile.Start(); //DEBUG //только для инициализации, пока не налажен механизм вызова Paint через объект Game и Scene
            GameCanvasPanel.Paint += game.projectile.Draw_OnPaintOnWinFormsEvent;
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
            }
            finally
            {
                ControlPanel.Enabled = true;
            }

        }


        #region Отрисовка
        
        /// <summary> При вызове обновления экрана из игры должно вызываться вот это.
        /// </summary>
        //TODO! По идее, это надо делать на уровне настроек игры на платформу.
        private void CallScreenRefresh()
        {
            GameCanvasPanel.Refresh();
        }



        ///// <summary>
        ///// Событие отрисовки, привязанное к панели игры
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void GameCanvasPanel_Paint(object sender, PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //    PaintGameScreen(e.Graphics);
        //    var projectile = game?.projectile;
        //    if (projectile == null) return;
        //    //projectile.Draw_OnPaintOnWinFormsEvent(sender, e);
        //}

        ///// <summary>
        ///// Наша функция для отрисовки игры
        ///// </summary>
        ///// <param name="graphics">Графический контекст, получаемый из события Paint</param>
        //protected void PaintGameScreen(Graphics graphics)
        //{
        //    try
        //    {
        //        Pen bluePen = new Pen(Color.Blue, 3);
        //        Brush blueBrush = new SolidBrush(Color.Blue);
        //        Pen redPen = new Pen(Color.Red, 2);
        //        Pen blackPen = new Pen(Color.Black);

        //        // Масштаб экрана - в мм
        //        graphics.PageUnit = GraphicsUnit.Millimeter;

        //        var projectileRectSize = 5;
        //        // координаты ядра (в метрах)
        //        var prX = (float) game.projectile.Position.X;
        //        var prY = (float) game.projectile.Position.Y;
        //        // отмасштабируем эти координаты, чтоб все вместилось в экран
        //        // масштаб мы задаем в метрах на сантиметр, а экран у нас меряется в миллиметрах (GraphicsUnit.Millimeter)
        //        prX = prX * 10 / GameParams.GameScreenScale;
        //        prY = prY * 10 / GameParams.GameScreenScale;
        //        // учтем размер рисуемого прямоугольника, и соответственно сместим его начало
        //        // учтем, что началом координаты Y у нас должен быть конец (нижний) экрана
        //        // и что координата Y в игре направлена вверх, а у нас на экране - вниз
        //        var screenHeight = graphics.VisibleClipBounds.Height;
        //        var screenX = prX;
        //        var screenY = screenHeight - projectileRectSize - prY;
        //        if ((screenX < 0) || (screenX > graphics.VisibleClipBounds.Width) || (screenY < 0) || (screenY > graphics.VisibleClipBounds.Height))
        //        {
        //            return;
        //        }
        //        RectangleF r = new RectangleF(screenX, screenY, projectileRectSize, projectileRectSize);
        //        graphics.DrawEllipse(bluePen, r);
        //        graphics.FillEllipse(blueBrush, r);

        //        //Debug.WriteLine(r);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine("Ошибка отрисовки");
        //    }
        //}

        #endregion

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
            if (e.PropertyName == "Velocity")
                VelocityEdit.Value = (decimal)(sender as GameParameters)?.Speed;
        }

        private void AngleChanged_to_AngleEdit(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Angle")
                AngleEdit.Value = (decimal)(sender as GameParameters)?.Angle;
        }


        #endregion
    }



}
