using System;

namespace MiniUnity
{
    /// <summary>
    /// Часы, отслеживающие игровое время.
    /// Т.е. игра может быть остановлена, и за это время в ней ничего не произойдет.
    /// Кроме того, тут можно задать масштаб времени.
    /// </summary>
    public class Clock : IClock
    {
        /// <summary>
        /// Текущее игровое время в формате DateTime
        /// </summary>
        /// <returns></returns>
        public DateTime Now()
        {
            if (_stopped) return LastGameTime;

            CurrentRealTime = DateTime.Now;
            
            // учтем масштаб времени
            TimeSpan RealTimeDelta = CurrentRealTime - LastRealTime;
            var gameTimeAdditionMSec = RealTimeDelta.TotalMilliseconds * GameTimeScale;
            TimeSpan GameTimeDelta = TimeSpan.FromMilliseconds(gameTimeAdditionMSec);

            CurrentGameTime = LastGameTime + GameTimeDelta;
            // запомним значения времени для следующего использования
            LastGameTime = CurrentGameTime;
            LastRealTime = CurrentRealTime;
            return CurrentGameTime;
        }

        public void SetDateTime(DateTime dateTime)
        {
            if (!Stopped) throw new NotSupportedException("Установка времени возможна только при остановленных часах");
            LastGameTime = dateTime;
        }

        #region Масштаб времени
        /// <summary>
        /// Масштаб между игровым временем и реальным - сколько игровых секунд проходит за одну реальную секунду
        /// </summary>
        public float GameTimeScale { get; set; } = 1;
        #endregion

        #region Пуск/останов часов
        /// <summary>
        /// Остановить игровое время
        /// </summary>
        public void Stop()
        {
            _stopped = true;
            // Определим и запомним текущее игровое время
            LastRealTime = DateTime.Now;
            LastGameTime = Now();
        }

        /// <summary>
        /// Запустить отсчет времени дальше
        /// </summary>
        public void Resume()
        {
            _stopped = false;
            // С этого момента пойдет счет реального времени 
            // Поэтому игровое время остается неизменным, 
            // а последнее реальное время мы запишем, чтоб считать от него:
            LastRealTime = DateTime.Now;
        }

        private DateTime CurrentRealTime;
        private DateTime CurrentGameTime;
        private DateTime LastRealTime;
        private DateTime LastGameTime;

        private bool _stopped = false;

        /// <summary>
        /// Часы остановлены?
        /// </summary>
        public bool Stopped
        {
            get { return _stopped; }
        }

        #endregion    
    }
}