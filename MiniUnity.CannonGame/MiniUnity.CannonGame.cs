using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using MiniUnity;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Threading;
using System.Windows.Forms;


namespace MiniUnity.CannonGame
{
    public class CannonGame : Game
    {
        public CannonGame()
        {
            //FramesPerSecond = CannonGameFramesPerSecond;
            Scene=new CannonScene();
            cannon = new Cannon();
            projectile = new Projectile();
            Scene.AddComponent(cannon);
            Scene.AddComponent(projectile);
        }

        //// TODO: Как задать начальный выстрел из пушки без перекрытия метода Play?
        //public override void Play()
        //{
        //    // TODO: Как задать начальный выстрел из пушки без перекрытия метода Play?
        //    cannon.Fire(projectile, Angle, Velocity);
        //    base.Play();
        //}

        public Cannon cannon;
        public Projectile projectile;

        public float Velocity;
        public float Angle;

        /// <summary>
        /// Масштаб изображения - метров в сантиметре экрана
        /// </summary>
        public float ScreenScale { get; set; }

        // Проигрывать звуки?
        public bool PlaySound { get; set; }
    }

    public class CannonScene : Scene
    {
        protected Cannon Cannon;
        protected Projectile Projectile;
        protected CannonGame Game
        {
            get { return (this.Parent as CannonGame); }
        }

        public CannonScene()
        {
        }


        public override void Start()
        {
            base.Start();

            Cannon = GetComponent<Cannon>() as Cannon;
            Projectile = GetComponent<Projectile>() as Projectile;

            Cannon.Load(Projectile, Game.Angle, Game.Velocity);
            Cannon.Fire();
        }

        public override void Update()
        {
            // Сделаем обработку ввода с клавиатуры
            // По идее, это неправильно. Для этого должен бы быть какой-то специальный метод, в специальном месте, 
            // но я еще не придумал куда вставить управление игрой...
            CheckKeyboardCommands();

            if (Game.Orchestrator.Stopped) return;

            //Console.WriteLine(DateTime.Now.Minute+":"+DateTime.Now.Second+"."+DateTime.Now.Millisecond);
            base.Update();

            if (Projectile.Fallen)
                IsOver = true;
        }

        private void CheckKeyboardCommands()
        {
            // TODO: Это сделать изменяемым в зависимости от типа приложения!
            /*
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                if (key.KeyChar == ' ')
                    // пуск-стоп
                {
                    //IsStopped = !IsStopped;
                    if (!Game.Orchestrator.Stopped) 
                        Game.Orchestrator.Stop();
                    else
                        Game.Orchestrator.Resume();
                }

                if (
                    key.Key == ConsoleKey.Escape
                    ||
                    (key.KeyChar == 'x')||(key.KeyChar == 'X')||(key.KeyChar == 'ч')||(key.KeyChar == 'Ч')
                    )

                {
                    // Esc - выход из игры
                    // Пока просто указываем флаг завершения сцены
                    IsOver = true;
                }
            }
            */
        }
    }

    public class Cannon: GameObject
    {
        public Vector3 Position = new Vector3();
        //public Point Position = new Point();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectile">Снаряд, которым будем стрелять</param>
        /// <param name="elevationAngle">Угол возвышения в градусах</param>
        /// <param name="velocity">Скорость снаряда</param>
        public void Fire(Projectile projectile, float elevationAngle, float velocity)
        {
            // Отрисовываем снаряд на месте пуска
            projectile.Update();

            projectile.Fallen = false;
            //projectile
            Console.WriteLine("Бабах!");

            projectile.Position.X = Position.Y;
            projectile.Position.Y = Position.Y;

            var elevationAngleInRadians = elevationAngle * Math.PI / 180;

            projectile.Velocity.X = (float) (velocity * Math.Cos(elevationAngleInRadians));
            projectile.Velocity.Y = (float) (velocity * Math.Sin(elevationAngleInRadians));
        }

        public void Fire()
        {
            Fire(Projectile, ElevationAngle, Velocity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectile">Снаряд, которым будем стрелять</param>
        /// <param name="elevationAngle">Угол возвышения в градусах</param>
        /// <param name="velocity">Скорость снаряда</param>
        public void Load(Projectile projectile, float elevationAngle, float velocity)
        {
            // Надо???
            Projectile = projectile;
            ElevationAngle = elevationAngle;
            Velocity = velocity;
        }
        public float Velocity { get; set; }

        public float ElevationAngle { get; set; }

        public Projectile Projectile { get; set; }
    }

    public class Projectile: GameObject
    {
        protected CannonGame Game;
        protected CannonScene Scene;

        // Может быть, перенести в GameObject?
        // Или в GameObject.Render?
        // Положение снаряда
        public Vector3 Position = new Vector3();
        //public Point Position = new Point();

        // Скорость снаряда
        public Vector3 Velocity = new Vector3();
        //public Point Velocity = new Point();

        // Отметим момент падения и перестанем сообщать о ранее упавшем снаряде
        public bool Fallen = false;

        private float time = 0;

        public override void Start()
        {
            //Game = GetParentComponent<Game>() as Game;
            Game = GetParentComponent<CannonGame>() as CannonGame;
            if (Game==null) throw new NullReferenceException("Не найден объект игры");
            Scene=GetParentComponent<CannonScene>() as CannonScene;
            if (Scene==null) throw new NullReferenceException("Не найден объект сцены");
            time = 0;
            base.Start();
        }

        public override void Update()
        {
            if (Fallen) return;

            // Прошло времени с прошлого обновления
            float dT = Game.Orchestrator.TimeDeltaFromLastUpdateInSeconds;
            time = time + dT;

            // Ускорение свободного падения - 9.81 м/с^2
            float G = -9.81f; //Направлено вниз, поэтому с минусом
            float dVY = G * dT;
            //float dVY2 = G * 1 / CannonGame.CannonGameFramesPerSecond;

            // Отрабатываем изменение положения; положение по Y меняется ускоренно.
            Position.X = Position.X + Velocity.X * dT;
            Position.Y = Position.Y + (Velocity.Y + dVY/2) * dT ;

            // Отрабатываем изменение скорости
            Velocity.Y = Velocity.Y + dVY;

            // Выводим данные о положении снаряда
            //Console.WriteLine(DateTime.Now.Minute+":"+DateTime.Now.Second+"."+DateTime.Now.Millisecond);
            Console.WriteLine("t="+time + "   X="+Position.X.ToString("F2") + "; Y="+Position.Y.ToString("F2") + "  V.Y = "+ Velocity.Y.ToString("F2"));
            //TODO! Убрать использование Console и сделать обобщенно

            // Если снаряд упал на землю - он останавливается, дальше не летит.
            if ((Position.Y < 0) & (Velocity.Y<=0))
            {
                if (!Fallen)
                    Fall();
            }

            base.Update();
            // Это надо будет перенести в Scene.Update, чтоб вызывалось один раз
            RefreshScreen();
        }

        private void Fall()
        {
            Position.Y = 0;
            Velocity.X = 0;
            Velocity.Y = 0;
            //TODO! Убрать использование Console и сделать обобщенно
            Console.WriteLine("Шлёп!");
            Fallen = true;
            Scene.IsOver = true;
        }


        #region Отрисовка ядра

        // * 1. Вызов из игры обновления экрана
        
        // Вероятно, это надо будет просто поставить в конце Scene.Update
        public void RefreshScreen()
        {
            if (OnCallScreenRefresh != null) OnCallScreenRefresh();
        }

        // Вызовы обновления на разных платформах

        public Action OnCallScreenRefresh { get; set; }

        // Вызов обновления в консольном приложении
        public void CallConsoleRedraw()
        {
            // Никаких специальных действий не требуется.
            // Просто вызываем консольный Redraw
            // Тут надо будет вызывать метод Сцены, а он уже должен вызывать методы вложенных в Сцену объектов
            Draw_WriteToConsole();
        }

        // Вызов обновления в приложении WinForms
        public void CallWinformsRedraw()
        {
            // Похоже, вообще не требуется.
            // Вместо этого форма установит в OnCallScreenRefresh свой this.Refresh()
        }


        // * 2. Отрисовка - вызывается из приложения, в котором работает игра


        /// <summary>
        /// Отрисовка ядра средствами консоли
        /// </summary>
        public void Draw_WriteToConsole()
        {
            Console.WriteLine(Position + ";  V = (" + Velocity +")");
            //Console.WriteLine("t="+time + "   X="+Position.X.ToString("F2") + "; Y="+Position.Y.ToString("F2") + "  V.Y = "+ Velocity.Y.ToString("F2"));
        }

        /// <summary>
        /// Функция отрисовки ядра средствами WinForms - в формате события, вызываемого компонентом формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Draw_OnPaintOnWinFormsEvent(object sender, PaintEventArgs e)
        {
            Draw_PaintOnWinForms(e.Graphics);
        }

        /// <summary>
        /// Функция отрисовки ядра средствами WinForms
        /// </summary>
        /// <param name="graphics"></param>
        public void Draw_PaintOnWinForms(Graphics graphics)
        {
            try
            {
                Pen bluePen = new Pen(Color.Blue, 3);
                Brush blueBrush = new SolidBrush(Color.Blue);
                Pen redPen = new Pen(Color.Red, 2);
                Pen blackPen = new Pen(Color.Black);

                // Масштаб экрана - в мм
                graphics.PageUnit = GraphicsUnit.Millimeter;

                var projectileRectSize = 5;
                // координаты ядра (в метрах)
                var prX = (float) Position.X;
                var prY = (float) Position.Y;
                // отмасштабируем эти координаты, чтоб все вместилось в экран
                // масштаб мы задаем в метрах на сантиметр, а экран у нас меряется в миллиметрах (GraphicsUnit.Millimeter)
                prX = prX * 10 / Game.ScreenScale;
                prY = prY * 10 / Game.ScreenScale;
                // учтем размер рисуемого прямоугольника, и соответственно сместим его начало
                // учтем, что началом координаты Y у нас должен быть конец (нижний) экрана
                // и что координата Y в игре направлена вверх, а у нас на экране - вниз
                var screenHeight = graphics.VisibleClipBounds.Height;
                var screenX = prX;
                var screenY = screenHeight - projectileRectSize - prY;
                // если вышли за пределы экрана - не рисуем
                if ((screenX < 0) || (screenX > graphics.VisibleClipBounds.Width) || (screenY < 0) ||
                    (screenY > graphics.VisibleClipBounds.Height))
                {
                    return;
                }

                RectangleF r = new RectangleF(screenX, screenY, projectileRectSize, projectileRectSize);
                graphics.DrawEllipse(bluePen, r);
                graphics.FillEllipse(blueBrush, r);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Ошибка отрисовки");
                Debug.WriteLine(e.GetType().Name);
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.Source);
                Debug.WriteLine(e.StackTrace);
            }

        }

        #endregion

    }


    

}
