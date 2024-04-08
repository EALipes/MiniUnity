using System;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Numerics;

namespace MiniUnity.CannonGame
{
    public class Cannon: GameObject
    {
        protected CannonGame Game { get; set; }
        protected CannonScene Scene { get; set; }

        public Vector3 Position { get; set; } = new Vector3();

        protected SoundPlayer cannonFiresSoundPlayer;

        public Cannon()
        {
            cannonFiresSoundPlayer = new SoundPlayer();
            cannonFiresSoundPlayer.Stream = Resources.CannonFired;
            cannonFiresSoundPlayer.Load();
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
            base.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectile">Снаряд, которым будем стрелять</param>
        /// <param name="elevationAngle">Угол возвышения в градусах</param>
        /// <param name="velocityScalar">Скорость снаряда</param>
        public void Fire(Projectile projectile, float elevationAngle, float velocityScalar)
        {
            if (Game.PlaySound)
                //SoundPlayerGunFired.Play();
                //new SoundPlayer( Resources.CannonFiredAndProjectileFlies).Play();
                cannonFiresSoundPlayer.PlaySync();

            projectile.Fired(elevationAngle, velocityScalar);

            //// Отрисовываем снаряд на месте пуска
            //projectile.Update();

            //projectile.Fallen = false;
            ////projectile
            //// TODO! Вывод поправить, сделать в зависимости от типа приложения.
            Console.WriteLine("Бабах!");

            projectile.Position = Position;

            //var elevationAngleInRadians = elevationAngle * Math.PI / 180;

            //var velocity = projectile.Velocity;
            //velocity.X = (float) (velocityScalar * Math.Cos(elevationAngleInRadians));
            //velocity.Y = (float) (velocityScalar * Math.Sin(elevationAngleInRadians));
            //projectile.Velocity = velocity;
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
        
        /// <summary>
        /// Скорость снаряда
        /// </summary>
        public float Velocity { get; set; }

        /// <summary>
        /// Угол возвышения (в градусах)
        /// </summary>
        public float ElevationAngle { get; set; }

        public Projectile Projectile { get; set; }


        #region Отрисовка пушки

        /// <summary>
        /// Функция отрисовки пушки средствами WinForms
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
                Brush blackBrush = new SolidBrush(Color.Black);

                // Масштаб экрана - в мм
                graphics.PageUnit = GraphicsUnit.Millimeter;

                var cannonDiameter = 5;
                var cannonLength = 20;
                // координаты ядра (в метрах)
                var prX = (float) Position.X;
                var prY = (float) Position.Y;
                // отмасштабируем эти координаты, чтоб все вместилось в экран
                // масштаб мы задаем в метрах на сантиметр, а экран у нас меряется в миллиметрах (GraphicsUnit.Millimeter)
                prX = prX * 10 / Game.ScreenScale;
                prY = prY * 10 / Game.ScreenScale;
                // учтем, что началом координаты Y у нас должен быть конец (нижний) экрана
                // и что координата Y в игре направлена вверх, а у нас на экране - вниз
                var screenHeight = graphics.VisibleClipBounds.Height;
                var screenX = prX;
                var screenY = screenHeight - cannonDiameter - prY;
                // если вышли за пределы экрана - не рисуем
                if ((screenX < 0) || (screenX > graphics.VisibleClipBounds.Width) || (screenY < 0) ||
                    (screenY > graphics.VisibleClipBounds.Height))
                {
                    return;
                }

                Rectangle r = new Rectangle((int) screenX, (int) screenY, cannonLength, cannonDiameter);
                // В графике положительный угол почему-то считается по часовой стрелке, а не влево, как в математике.
                graphics.Transform.Rotate(-ElevationAngle);
                graphics.DrawRectangle(blackPen, r);
                graphics.FillRectangle(blueBrush, r);
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