using System;
using System.ComponentModel;
using System.Data;


// для отрисовки в Windows.Forms
using System.Windows.Forms;

// для отрисовки в WPF
using System.Windows.Controls;

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


        /// <summary> В игре, наверное, должна быть хотя бы одна сцена.
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
                    //scene.Parent = this;
                    AddComponent(scene);
                }
                else // value=null
                {
                    // При удалении сцены очистим родителя
                    //if (scene != null) scene.Parent = null;
                    RemoveComponent(scene);
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
            IsOver = false;
            Orchestrator.Start();
            base.Start();
        }

        /// <summary> Собственно, выполнение игры.
        /// </summary>
        /// <remarks>
        /// Пока непонятно, как это надо будет делать.
        /// Поэтому пока я сделал по-простому, отыгрываем только одну сцену, игру и сцену указываем явным образом.
        /// </remarks>
        public void Play(Game game, Scene scene)
        {
            // По идее, это не обязательно даже тут проверять, коли сцена задана явно в параметре. Убрать???
            if (Scene == null) throw new NullReferenceException("Не определено отыгрываемых сцен");

            Start();

            RefreshDraw();
            
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


        #region Отрисовка

        /// <summary> Элемент управления, принимающий клавиатурные события, и передающий их игре.
        /// (Сейчас устанавливаем его в окно. Панель почему-то не имеет соотв. событий.)
        /// События клавиатуры передаются игре обработчиками DoOnKeyPress и DoOnKeyDown, унаследованными от GameObject.
        /// </summary>
        public System.Windows.Forms.Control KeyboardEventsControl
        {
            get
            {
                return _keyboardEventsControl;

            }
            set
            {
                if (_keyboardEventsControl != null)
                {
                    _keyboardEventsControl.KeyPress -= DoOnKeyPress;
                    _keyboardEventsControl.KeyDown -= DoOnKeyDown;
                }

                _keyboardEventsControl = value;
                if (_keyboardEventsControl != null)
                {
                    _keyboardEventsControl.KeyPress += DoOnKeyPress;
                    _keyboardEventsControl.KeyDown += DoOnKeyDown;
                }
            }
        }
        private System.Windows.Forms.Control _keyboardEventsControl;

        #region Winforms

        /// <summary> Панель или др. видимый элемент, на котором будет отображаться графика игры
        /// </summary>
        public System.Windows.Forms.Control GamePanel 
        {
            get
            {
                return _gamePanel;

            }
            set
            {
                if (_gamePanel != null)
                {
                    //_gamePanel.Paint -= Scene.Draw_OnWinFormsPaintEvent;
                    _gamePanel.Paint -= Draw_OnWinFormsPaintEvent;
                }

                _gamePanel = value;
                if (_gamePanel != null)
                {
                    //_gamePanel.Paint += Scene.Draw_OnWinFormsPaintEvent;
                    _gamePanel.Paint += Draw_OnWinFormsPaintEvent;
                }
            }
        }
        private System.Windows.Forms.Control _gamePanel;


        #endregion

        #region WPF
        protected System.Windows.Controls.Panel GameGrid
        //protected System.Windows.Controls.Grid GameGrid { get; set; }
        {
            get
            {
                return _gameGrid;

            }
            set
            {
                //if (_gameGrid != null)
                //    _gameGrid.Paint -= Draw_OnWPFPaintEvent;
                _gameGrid = value;
                //if (_gameGrid != null)
                //    _gameGrid.Paint += Draw_OnWPFPaintEvent;
            }
        }
        private System.Windows.Controls.Panel _gameGrid;

        #endregion

        public override void RefreshDraw()
        {
            if (AppType == ApplicationType.ConsoleApp)
            {
                // Просто отрисовываем объекты
                Draw();
            }
            else if (AppType == ApplicationType.WinFormsApp)
            {
                // Даем команду перерисовать панель игры
                if (GamePanel != null)
                    GamePanel.Refresh();
                // Даем приложению обработать это и другие события Windows
                if (Application.MessageLoop)
                {
                    Application.DoEvents();
                }
            }
            else if (AppType == ApplicationType.WpfApp)
            {
                if (GameGrid != null)
                {
                    //GameGrid.Refresh();
                }
            }
        }

        #endregion

    }
}
