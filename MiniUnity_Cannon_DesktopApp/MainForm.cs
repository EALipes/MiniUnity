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


            // Инициализация игры
            // (В консольном приложении это сделано в Main(), тут удобнее это  сделать при инициализации главной формы.
            game = new CannonGame();
            game.projectile.OnCallScreenRefresh = CallScreenRefresh;
            game.projectile.Start(); //DEBUG //только для инициализации, пока не налажен механизм вызова Paint через объект Game и Scene
            GameCanvasPanel.Paint += game.projectile.Draw_OnPaintOnWinFormsEvent;
        }

        #region Параметры игры
        private readonly GameParameters gameParams;

        // Возможно, это даже стоит отправить в сам GameSettings - потому что будет использоваться не только с этой формой
        private void SetGameSettings()
        {
            game.Angle = gameParams.Angle;
            game.Velocity = gameParams.Speed;
            game.FramesPerSec = gameParams.FramesPerSec;
            game.Orchestrator.Clock.GameTimeScale = gameParams.TimeScale;
            game.PlaySound = gameParams.PlaySound;
            game.ScreenScale = gameParams.GameScreenScale;
        }

        #endregion

        #region Объекты игры
        // Ядро
        //protected Projectile projectile;

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

        private DateTime ProjectileFlightStartTime;

        #endregion


        private void RunButton_Click(object sender, EventArgs e)
        {
            ControlPanel.Enabled = false;
            SetGameSettings();
            try
            {
                game.Play();
                new SoundPlayer(Resources.ProjectileFall3).Play();
            }
            finally
            {
                ControlPanel.Enabled = true;
            }

        }


        #region Отрисовка
        
        /// <summary>
        /// При вызове обновления экрана из игры должно вызываться вот это.
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
        #endregion
    }


    /// <summary>
    /// Структура для передачи параметров игры
    /// </summary>
    public class GameParameters : INotifyPropertyChanged
    {
        public GameParameters()
        {
        }

        public int FramesPerSec
        {
            get { return _framesPerSec; }

            set
            {
                _framesPerSec = value;
                OnPropertyChanged("FramesPerSec");
            }
        }
        private int _framesPerSec = 25;

        public float TimeScale
        {
            get
            {
                return _timeScale;
            }

            set
            {
                _timeScale = value;
                OnPropertyChanged("TimeScale");
            }
        }
        private float _timeScale = 1;

        public bool PlaySound
        {
            get
            {
                return _playSound;
            }

            set
            {
                _playSound = value;
                OnPropertyChanged("PlaySound");
            }
        }
        private bool _playSound = true;

        /// <summary>
        /// Масштаб изображения - метров в сантиметре экрана
        /// </summary>
        public float GameScreenScale
        {
            get
            {
                return _gameScreenScale;
            }

            set
            {
                _gameScreenScale = value;
                OnPropertyChanged("GameScreenScale");
            }
        }
        private float _gameScreenScale = 100.0f;

        public float Speed
        {
            get
            {
                return _speed;
            }

            set
            {
                _speed = value;
                OnPropertyChanged("Speed");
            }
        }
        private float _speed = 100;

        public float Angle
        {
            get
            {
                return _angle;
            }

            set
            {
                _angle = value;
                OnPropertyChanged("Angle");
            }
        }
        private float _angle = 45;


        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }



    //public interface INotifyPropertyChanged
    //{
    //    /// <summary>Occurs when a property value changes.</summary>
    //    event PropertyChangedEventHandler PropertyChanged;
    //}

    //public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    //public class PropertyChangedEventArgs : EventArgs
    //{
    //    public virtual string PropertyName
    //    {
    //       get 
    //       {
    //            return this.propertyName;
    //       }
    //    }
    //    private readonly string propertyName;

    //    public PropertyChangedEventArgs(string propertyName)
    //    {
    //        this.propertyName = propertyName;
    //    }
    //}


}
