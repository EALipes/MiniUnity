using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;

namespace MiniUnity
{

    public class Game: GameObject
    {
        public Game()
        {
            Orchestrator = new UpdateOrchestrator();

            FramesPerSec = 25;
            IsOver = false; // 
        }

        public bool IsOver { get; set; } //= false;


        /// <summary>
        /// В игре, наверное, должна быть хотя бы одна сцена.
        /// Вероятно, дальше сделаем список сцен - но пока мне не очевидно, как с ними работать. Оставим на потом.
        /// </summary>
        protected Scene Scene
        {
            get => scene;
            set
            {
                if (value != null)
                {
                    scene = value;
                    // При вставке сцены в игру будем указывать саму игру как родителя.
                    scene.Parent = this;
                }
                else // value=null
                {
                    // При удалении сцены очистим родителя
                    if (scene != null) scene.Parent = null;
                }
            }
        }

        protected Scene scene { get; set; }



        //public void Finish()
        //{
        //    IsOver = true;
        //}

        #region Параметры игры

        /// <summary>  Количество кадров в секунду
        /// </summary>
        public int FramesPerSec
        {
            get { return Orchestrator.FramesPerSec;}
            set { Orchestrator.FramesPerSec = value; }
        }


        #endregion


        #region Таймер и ход времени

        // Тут прямое присваивание в будущем лучше бы заменить на более изощренный метод получения оркестратора - 
        // скажем, через IOContainer, или через конструктор с явным указанием компонентов игры.
        public UpdateOrchestrator Orchestrator; // = new UpdateOrchestrator();

        #endregion

        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Собственно, выполнение игры.
        /// </summary>
        /// <remarks>
        /// Пока непонятно, как это надо будет делать.
        /// Поэтому пока я сделал по-простому, отыгрываем только одну сцену, игру и сцену указываем явным образом.
        /// </remarks>
        public void Play(Game game, Scene scene)
        {
            // По идее, это не обязательно даже тут проверять, коли сцена задана явно в параметре. Убрать???
            if (Scene == null) throw new NullReferenceException("Не определено отыгрываемых сцен");

            game.IsOver = false;
            scene.IsOver = false;

            Orchestrator.Start();
            Scene.Start();
            
            // Вызываем цикл обновлений под управлением Orchestrator;
            // В цикле вызываем scene.Update()
            // Циклим пока не будет установлен флаг окончания игры или завершения сцены
            // можно было бы вызвать вот так:
            // Orchestrator.DoUpdates(scene.Update, (()=> game.IsOver || Scene.IsOver));
            // но, чтоб не вдаваться в разбор записи такого вида, сделаем функцию и вызовем ее явно:
            Orchestrator.DoUpdates(scene.Update, CycleIsOver);

            // вложенная функция - в C# такая запись возможна, но редко используется
            bool CycleIsOver()
            {
                var result = game.IsOver || Scene.IsOver;
                return result;
            }

        }

        public void Play()
        {
            Play(this, Scene);
        }

    }


    public class Scene: GameObject
    {
        public override void Start()
        {
            IsOver = false;
            // Установить начальные параметры всех объектов сцены
            base.Start();
        }

        /// <summary>
        /// Обновить объект. 
        /// Тут обновлятеся положение, или производится отрисовка, или т.п.
        /// </summary>
        public override void Update()
        {
            //foreach (var child in Children)
            //{
            //    child.Update();
            //}
            
            base.Update();
        }

        public bool IsOver { get; set; }
    }


    /// <summary>
    /// Основной класс для всех объектов, из которых строится игра
    /// </summary>
    public class GameObject
    {

        /// <summary>
        /// Родительский объект
        /// </summary>
        public GameObject Parent { get; set; }

        /// <summary>
        /// Вложенные объекты, подчиняющиеся текущему
        /// </summary>
        protected List<GameObject> Children = new List<GameObject>();

        #region // Функции добавления и поиска объектов

        /// <summary>
        /// Найти в родителях
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T  GetParentComponent<T>()
            where T:GameObject
        {
            GameObject result = null;
            if (Parent == null) return null;
            var parentType = Parent.GetType();
            if ((parentType == typeof(T))
                ||
                (parentType.IsSubclassOf(typeof(T))))
                return Parent as T;

            return Parent.GetParentComponent<T>();
        }

        // При добавлении компонента у него устанавливается свойство Parent = this
        public void AddComponent(GameObject gameObject)
        {
            Children.Add(gameObject);
            gameObject.Parent = this;
        }
        public void RemoveComponent(GameObject gameObject)
        {
            Children.Remove(gameObject);
            gameObject.Parent = null;
        }

        /// <summary>
        /// Найти в подчиненных объектах объект указанного типа (или его потомка)
        /// Возвращается первый найденный подходящий объект
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>()
            where T:GameObject
        {
            // Мне лень честно писать поиск первого подходящего элемента такого типа в списке,
            // поэтому я использовал LINQ, хотя это заклинание проходят только на старших курсах Хогвартса )))
            var result = Children?.FirstOrDefault(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))) ) as T;
            return result;
        }

        /// <summary>
        /// Найти в подчиненных объектах все объекты указанного типа (или его потомка)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICollection<T> GetComponents<T>()
            where T:GameObject
        {
            // Тоже LINQ
            var result = Children?
                .Where(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))))
                .Select(b=> b as T)
                .ToList();
            return result;
        }

        /// <summary>
        /// Найти в подчиненных объектах поведение указанного типа (или его потомка)
        /// Возвращается первый найденный подходящий объект
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetBehavior<T>()
            where T:BehaviorComponent
        {
            // Мне лень честно писать поиск первого подходящего элемента такого типа в списке,
            // поэтому я использовал LINQ, хотя это заклинание проходят только на старших курсах Хогвартса )))
            var result = Children?.FirstOrDefault(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))) );
            return result as T;
        }

        /// <summary>
        /// Найти в подчиненных объектах все объекты-поведения указанного типа (или его потомка)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICollection<T> GetBehaviors<T>()
            where T:BehaviorComponent
        {
            // Тоже LINQ
            var result = Children?
                .Where(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))))
                .Select(b => b as T)
                .ToList();
            return result;
        }

        #endregion


        /// <summary>
        /// Вызывается при старте программы или сцены, после того, как все элементы уже созданы
        /// </summary>
        public virtual void Start()
        {
            foreach (GameObject c in Children)
            {
                c.Start();
                if (OnStart != null) OnStart(this);
            }
        }


        /// <summary>
        /// Обновить объект. 
        /// Тут обновляюеся положение, или производится отрисовка, или т.п.
        /// </summary>
        public virtual void Update()
        {
            foreach (GameObject c in Children)
            {
                c.Update();
                if (OnUpdate != null) OnUpdate(this);
            }
        }

        /// <summary>
        /// Событие вызывается при инициализации объекта игры
        /// </summary>
        public event OnStartHandler  OnStart;

        /// <summary>
        /// Событие вызывается при обновлении состояния объекта игры
        /// </summary>
        public event OnUpdateHandler  OnUpdate;

        public delegate void OnStartHandler(GameObject gameObject);
        public delegate void OnUpdateHandler(GameObject gameObject);

    }

    /// <summary>
    /// Компонент поведения.
    /// Для каждой "стороны" поведения объекта можно задавать свой компонент.
    /// Шаблон проектирования «Компонент» (Component) был формализован в 1994 году, 
    /// в книге «Design Patterns: Elements of Reusable Object-Oriented Software», 
    /// написанной «бандой четырех». 
    /// Основная идея шаблона «Компонент» заключается в объединении связанных функций и данных в общий класс 
    /// и в то же время в сохранении каждого класса как можно более маленьким и узкоспе­циализированным.
    /// Кстати, активно применялся в Delphi и VisualBasic, вышедшими ок.1995
    /// </summary>
    public class BehaviorComponent : GameObject{}



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

        public interface IClock
    {
        /// <summary>
        /// Текущее игровое время в формате DateTime
        /// </summary>
        /// <returns></returns>
        DateTime Now();

        void SetDateTime(DateTime dateTime);

        /// <summary>
        /// Масштаб времени - игровых секунд за реальную секунду
        /// </summary>
        float GameTimeScale { get; set; }
        

        /// <summary>
        /// Остановить игровое время
        /// </summary>
        void Stop();

        /// <summary>
        /// Запустить отсчет времени дальше
        /// </summary>
        void Resume();
        
        /// <summary>
        /// Часы остановлены?
        /// </summary>
        bool Stopped { get; }

    }
}
