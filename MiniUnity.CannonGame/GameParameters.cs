using System.ComponentModel;
using MiniUnity.CannonGame;

//using MiniUnity_Cannon_DesktopApp;

namespace MiniUnity.CannonGame
{
    /// <summary> Структура для передачи параметров игры
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

        /// <summary> Масштаб изображения - метров в сантиметре экрана
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

        /// <summary> Передача параметров в игру
        /// </summary>
        /// <param name="mainForm"></param>
        public void SetGameSettings(CannonGame game)
        {
            game.Angle = Angle;
            game.Velocity = Speed;
            game.FramesPerSec = FramesPerSec;
            game.Orchestrator.Clock.GameTimeScale = TimeScale;
            game.PlaySound = PlaySound;
            game.ScreenScale = GameScreenScale;
        }

        /// <summary> Получение параметров из игры
        /// </summary>
        public void GetGameSettings(CannonGame game)
        {
            Angle = game.Angle;
            Speed = game.Velocity;
            FramesPerSec = game.FramesPerSec;
            TimeScale = game.Orchestrator.Clock.GameTimeScale;
            PlaySound = game.PlaySound;
            GameScreenScale = game.ScreenScale;
        }
    }
}