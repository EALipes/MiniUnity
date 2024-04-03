using System;
using System.ComponentModel;
using System.Data;

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

        protected Scene scene;



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
}
