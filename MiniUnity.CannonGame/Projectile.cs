using System;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Numerics;
using System.Windows.Forms;

namespace MiniUnity.CannonGame
{
    public class Projectile: GameObject
    {
        protected CannonGame Game { get; set; }
        protected CannonScene Scene { get; set; }

        // Может быть, перенести в GameObject?
        // Или в GameObject.Render?
        // Положение снаряда
        public Vector3 Position { get; set; } = new Vector3();

        // Скорость снаряда
        public Vector3 Velocity { get; set; } = new Vector3();

        /// <summary> Снаряд летит - => меняет положение и скорость 
        /// </summary>
        private bool Flying { get; set; } = false;
       
        /// <summary> Только что упал, нужно издать Плюх, м/б отрисовать брызги, после чего этот флаг очистить 
        /// </summary>
        private bool JustFallen { get; set; } = false;

        // отслеживаем время полета
        private float flightTime = 0;

        protected SoundPlayer projectileFliesSoundPlayer;
        protected SoundPlayer projectileFallSoundPlayer;

        public Projectile()
        {
            projectileFliesSoundPlayer = new SoundPlayer(Resources.ProjectileFlight);
            projectileFallSoundPlayer = new SoundPlayer(Resources.ProjectileFall);
            
        }

        public override void Start()
        {
            //Game = GetParentComponent<Game>() as Game;
            Game = GetParentComponent<CannonGame>() as CannonGame;
            if (Game==null) 
                throw new NullReferenceException("Не найден объект игры");
            Scene=GetParentComponent<CannonScene>() as CannonScene;
            if (Scene==null) 
                throw new NullReferenceException("Не найден объект сцены");
            //time = 0;
            base.Start();
        }


        public override void Update()
        {
            // этот флаг очищается сразу при ближайшем Update
            if (JustFallen)
                JustFallen = false;

            // Если снаряд не летит - он лежит где был, нечего обновлять.
            if (!Flying) return;

            // Прошло времени с прошлого обновления
            float dT = Game.Orchestrator.TimeDeltaFromLastUpdateInSeconds;
            // время полета
            flightTime = flightTime + dT;

            // Ускорение свободного падения - 9.81 м/с^2
            // Учтем падение вертикальной скорости
            float dVY = Physics.G * dT;
            Vector3 acceleration = new Vector3(0, Physics.G, 0);

            // Отрабатываем изменение положения; положение по Y меняется ускоренно.
            //var X = Position.X + Velocity.X * dT;
            //var Y = Position.Y + (Velocity.Y + dVY/2) * dT ;
            //Position = new Vector3(X, Y, 0);
            // наверное, можно и так:
            Position = Position + (Velocity * dT) + (acceleration * dT / 2);


            // Отрабатываем изменение скорости
            //var velocity = Velocity;
            //velocity.Y = Velocity.Y + dVY;
            //Velocity = velocity;
            // попробуем так:
            Velocity = Velocity + acceleration * dT;


            // Если снаряд упал на землю - он останавливается, дальше не летит.
            if ((Position.Y <= 0) & (Velocity.Y<=0))
            {
                if (Flying)
                    Fall();
            }

            base.Update();
            // Это надо будет перенести в Scene.Update, чтоб вызывалось один раз
            RefreshDraw();
        }


        public void Fired(float elevationAngle, float velocityScalar)
        {
            // Включаем звук
            if (Game.PlaySound)
                projectileFliesSoundPlayer.PlayLooping();
            //projectileFliesSoundPlayer.Play();

            // Отрисовываем снаряд на месте пуска
            //Update();
            RefreshDraw();

            // Начинаем отсчет времени полета
            Flying = true;
            flightTime = 0;

            // Придаем снаряду начальную скорость
            var elevationAngleInRadians = elevationAngle * Math.PI / 180;
            var velocity = Velocity;
            velocity.X = (float) (velocityScalar * Math.Cos(elevationAngleInRadians));
            velocity.Y = (float) (velocityScalar * Math.Sin(elevationAngleInRadians));
            Velocity = velocity;
        }

        private void Fall()
        {
            // При столкновении с землёй скорость обнуляется, высота тоже.
            var position = Position;
            position.Y = 0;
            Position = position;
            Velocity=Vector3.Zero;
            
            // Больше ядро не летит, поэтому лежит себе на месте, и обновлять его и перерисовывать больше не нужно.
            Flying = false;


            
            //Console.WriteLine("Шлёп!"); // Перенесено в Draw()
            JustFallen = true; // Надо один раз отрисовать Шлеп! и брызги.
            // И издать звук - это мы можем пока сделать здесь.
            if (Game.PlaySound)
                projectileFallSoundPlayer.Play();


            // Если ядро упало - игра окончена.
            //Scene.IsOver = true;
        }


        #region Отрисовка

        // Console

        /// <summary> Отрисовка ядра средствами консоли
        /// </summary>
        protected override void Draw()
        {
            if (AppType == ApplicationType.ConsoleApp)
            {
                if (JustFallen)
                    Console.WriteLine("Шлёп!");

                Console.WriteLine(Position + ";  V = (" + Velocity + ")");
                //Console.WriteLine("t="+FlightTime + "   X="+Position.X.ToString("F2") + "; Y="+Position.Y.ToString("F2") + "  V.Y = "+ Velocity.Y.ToString("F2"));
            }

            base.Draw();
        }

        // WinForms

        /// <summary> Функция отрисовки ядра средствами WinForms - в формате события, вызываемого компонентом формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override  void Draw_OnWinFormsPaintEvent(object sender, PaintEventArgs args)
        {
            if (Game == null)
            {
                Debug.Write("Game==null при отрисовке Projectile.Draw_OnWinFormsPaintEvent");
                return;
            }

            var graphics = args.Graphics;
            try
            {
                graphics.ResetTransform();

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

                // Вызываем унаследованный метод
                base.Draw_OnWinFormsPaintEvent(sender, args);
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