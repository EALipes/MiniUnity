using System;
using System.Threading;

namespace MiniUnity
{
    /// <summary>
    /// Дирижер обновлений. 
    /// Отвечает за ход времени в игре, вызов обновлений и т.п.
    /// </summary>
    /// <remarks>
    /// (1)
    /// Он предоставляет объектам игры функционал часов (IClock),
    /// но при этом позволяет заменить реализацию часов - например, на более точные часы, или более быстрые, или т.п.
    /// (2)
    /// Позволяет определить еще и время, прошедшее с момента последнего обновления.
    /// Некоторые изменения требуют именно промежутка времени.
    /// (3)
    /// Именно этот объект отвечает за вызов обновлений.
    /// В соответствии с архитектурным принципом "информационного эксперта":
    /// Функционал должен быть возложен на тот объект, который имеет всю необходимую информацию для его выполнения.
    /// Поэтому он точно знает, когда (по игровому времени) было выполнено последнее обновление, 
    /// и, соответственно, сколько прошло игрового времени с этого момента. 
    /// </remarks>
    public class UpdateOrchestrator: IClock
    {
        public UpdateOrchestrator()
        {
            Clock = new Clock();
        }

        /// <summary>
        /// Часы - отражают игровое время
        /// </summary>
        public IClock Clock { get; set; }

        /// <summary>
        /// Текущее игровое время
        /// </summary>
        /// <returns></returns>
        public DateTime Now()
        {
            return Clock.Now();
        }

        public void SetDateTime(DateTime dateTime)
        {
            if (!Stopped) throw new NotSupportedException("Установка времени возможна только при остановленных часах");
            Clock.SetDateTime(dateTime);
            // обнуляем момент последнего апдейта
            LastUpdateTime = dateTime;
            TimeDeltaFromLastUpdate = TimeSpan.Zero;
            TimeDeltaFromLastUpdateInSeconds = 0;
        }

        /// <summary>
        /// Момент предыдущего обновления (в игровом времени)
        /// </summary>
        public DateTime LastUpdateTime { get; protected set; }
        
        /// <summary>
        /// Момент вызова текущего обновления (в игровом времени)
        /// </summary>
        public DateTime CurrentUpdateTime { get; protected set; }

        /// <summary>
        /// Промежуток времени, прошедший с предыдущего обновления
        /// </summary>
        public TimeSpan TimeDeltaFromLastUpdate { get; protected set; }
        /// <summary>
        /// Промежуток времени, прошедший с предыдущего обновления - в секундах
        /// </summary>
        public float TimeDeltaFromLastUpdateInSeconds { get; protected set; }

        /// <summary>
        /// Масштаб времени - игровых секунд за реальную секунду
        /// </summary>
        public float GameTimeScale
        {
            get { return Clock.GameTimeScale;}
            set { Clock.GameTimeScale = value; }
        }





        #region Пуск/останов часов
        /// <summary>
        /// Остановить игровое время
        /// </summary>
        public void Stop()
        {
            Clock.Stop();
        }

        /// <summary>
        /// Запустить отсчет времени дальше
        /// </summary>
        public void Resume()
        {
            Clock.Resume();
        }


        public bool Stopped
        {
            get { return Clock.Stopped; }
        }
        #endregion

        public void Start()
        {
            CurrentUpdateTime = Now();
            LastUpdateTime = CurrentUpdateTime;
            TimeDeltaFromLastUpdate = TimeSpan.Zero;
            TimeDeltaFromLastUpdateInSeconds = 0;
        }

        /// <summary>
        /// Вызвать обновления всех объектов игры
        /// </summary>
        /// <remarks>
        /// Поскольку объект существует в игре в готовом виде, то вызов обновлений других объектов привязывается к нему через событие OnUpdate().
        /// Это событие указывает на Action - т.е. процедуру без параметров.
        /// Сюда можно прицепить сколько угодно процедур обновления.
        /// Можно даже прицепить несколько процедур от одного объекта.
        /// </remarks>
        public void Update(UpdateFunc updateFunctionCall)
        {
            // Вычисляем время с последнего обновления
            CurrentUpdateTime = Now();
            TimeDeltaFromLastUpdate = CurrentUpdateTime - LastUpdateTime;
            TimeDeltaFromLastUpdateInSeconds = (float)TimeDeltaFromLastUpdate.TotalSeconds;
            
            // Вызываем обновления
            updateFunctionCall();
            //if (OnUpdate != null)
            //    OnUpdate();
            
            // Запоминаем время последнего обновления
            LastUpdateTime = CurrentUpdateTime;
            TimeDeltaFromLastUpdate = TimeSpan.Zero;
            TimeDeltaFromLastUpdateInSeconds = 0;
        }

        
        /// <summary>
        /// Событие обновления.
        /// Для вызова обновлений игровые объекты подписываются на это событие, прицепляя к нему свои процедуры обновления.
        /// </summary>
        public event Action OnUpdate;


        /// <summary>
        /// Частота вызова обновлений - кадров в секунду
        /// </summary>
        public int FramesPerSec { get; set; }


        /// <summary>
        /// Цикл обновления объектов игры.
        /// Вызывается из игры.
        /// </summary>
        /// <remarks>
        /// Вызывать обновления должен именно Orchestrator, 
        /// потому что он знает, когда вызвать следующее обновление, 
        /// и при этом может отслеживать, когда было предыдущее, сколько времени прошло между ними - 
        /// и может передать эту информацию обновляемым объектам.
        /// Но при этом он не знает, что именно нужно делать. Поэтому "что делать" - задается ему параметром.
        /// Он также должен знать, когда закончить обновления.
        /// Поэтому передадим ему параметр, определяющий это.
        /// </remarks>
        /// <remarks>
        /// можно и так:
        /// public void DoUpdates(Action updateFunctionCall, Func<bool> hasFinished) 
        /// но мы сделаем традиционно для понятности
        /// </remarks>
        public void DoUpdates(UpdateFunc updateFunctionCall, HasFinishedFunc hasFinished) 
        {
            while (!hasFinished())
            {
                // Вызываем обновление объектов
                //updateFunctionCall();
                Update(updateFunctionCall);
                // Ожидаем момента следующего обновления, на это время отдаем процессор операционной системе
                Thread.Sleep(1000/FramesPerSec);
            }
        }

        public delegate void UpdateFunc();

        public delegate bool HasFinishedFunc();
    }
}