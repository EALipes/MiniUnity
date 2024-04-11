using System;

namespace MiniUnity
{
    public interface IClock
    {
        /// <summary> Текущее игровое время в формате DateTime
        /// </summary>
        /// <returns></returns>
        DateTime Now();

        void SetDateTime(DateTime dateTime);

        /// <summary> Масштаб времени - игровых секунд за реальную секунду
        /// </summary>
        float GameTimeScale { get; set; }
        

        /// <summary> Остановить игровое время
        /// </summary>
        void Stop();

        /// <summary> Запустить отсчет времени дальше
        /// </summary>
        void Resume();
        
        /// <summary> Часы остановлены?
        /// </summary>
        bool Stopped { get; }

    }
}