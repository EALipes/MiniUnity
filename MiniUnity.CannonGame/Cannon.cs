using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Numerics;
using System.Windows.Forms;

namespace MiniUnity.CannonGame
{
    public class Cannon: GameObject
    {
        protected CannonGame Game { get; set; }
        protected CannonScene Scene { get; set; }

        public Vector3 Position { get; set; } = new Vector3();

        protected SoundPlayer cannonFiresSoundPlayer;

        protected bool JustFired = false;

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
        /// </summary>
        /// <param name="projectile">Снаряд, которым будем стрелять</param>
        /// <param name="elevationAngle">Угол возвышения в градусах</param>
        /// <param name="velocityScalar">Скорость снаряда</param>
        public void Fire(Projectile projectile, float elevationAngle, float velocityScalar)
        {
            Projectile = projectile; //TODO: нужно ли вообще это свойство?
            ElevationAngle = elevationAngle;
            Velocity = velocityScalar;

            projectile.Position = Position;
            RefreshDraw();
            if (Game.PlaySound)
                cannonFiresSoundPlayer.PlaySync();

            projectile.Fired(elevationAngle, velocityScalar);

            JustFired = true;
        }


        public void Fire()
        {
            Fire(Projectile, ElevationAngle, Velocity);
        }

        
        /// <summary> Скорость снаряда
        /// </summary>
        public float Velocity { get; set; }

        /// <summary> Угол возвышения (в градусах)
        /// </summary>
        public float ElevationAngle { get; set; }

        public Projectile Projectile { get; set; }

        public override void Update()
        {
            // Этот флаг работает только для одной отрисовки, и очищается на ближайшем Update()
            if (JustFired)
                JustFired = false;

            base.Update();
        }

        #region Отрисовка 

        // Console

        /// <summary> Отрисовка пушки в консольном приложении
        /// 
        /// </summary>
        protected override void Draw()
        {
            if (AppType == ApplicationType.ConsoleApp)
            {
                var s = this.GetType().Name + 
                        ":  X=" + Position.X.ToString("F1") + " Y=" + Position.Y.ToString("F1") + 
                        " " +nameof(ElevationAngle)+"=" + ElevationAngle + 
                        " "+ nameof(Velocity) + "=" + Velocity.ToString("F1");
                Console.WriteLine(s);
                if (JustFired)
                    Console.WriteLine("Бабах!");
            }

            base.Draw();
        }

        // Winforms

        /// <summary> Функция отрисовки пушки средствами WinForms
        /// </summary>
        /// <param name="graphics"></param>
        protected override void Draw_OnWinFormsPaintEvent(object sender, PaintEventArgs args)
        {
            if (Game == null)
            {
                Debug.Write("Game==null при отрисовке Cannon.Draw_OnWinFormsPaintEvent");
                return;
            }
            var graphics = args.Graphics;
            try
            {
                Pen bluePen = new Pen(Color.Blue, 3);
                Brush blueBrush = new SolidBrush(Color.Blue);
                Pen redPen = new Pen(Color.Red, 2);
                Pen blackPen = new Pen(Color.Black);
                Brush blackBrush = new SolidBrush(Color.Black);

                // Масштаб экрана - в мм
                graphics.PageUnit = GraphicsUnit.Millimeter;

                var cannonDiameter = 6;
                var cannonLength = 12;
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

                //// Пример рисования повернутого прямоугольника из сети
                //// https://learn.microsoft.com/en-us/dotnet/api/system.drawing.drawing2d.matrix.rotate?view=netframework-4.6#system-drawing-drawing2d-matrix-rotate(system-single-system-drawing-drawing2d-matrixorder)
                //{
                //    Pen myPen = new Pen(Color.Blue, 1);
                //    Pen myPen2 = new Pen(Color.Red, 1);

                //    // Draw the rectangle to the screen before applying the transform.
                //    graphics.DrawRectangle(myPen, 150, 50, 20, 10);

                //    // Create a matrix and rotate it 45 degrees.
                //    Matrix myMatrix = new Matrix();
                //    myMatrix.Rotate(45, MatrixOrder.Append);

                //    // Draw the rectangle to the screen again after applying the

                //    // transform.
                //    graphics.Transform = myMatrix;
                //    graphics.DrawRectangle(myPen2, 150, 50, 20, 10);
                //}

                //// Вот так получается прямоугольник повернутый, но не там
                //    {
                //        graphics.ResetTransform();
                //        GraphicsPath rectPath = new GraphicsPath();
                //        rectPath.AddRectangle(r);
                //        //rectPath.AddRectangle(new RectangleF(0, 0, cannonLength, cannonDiameter));
                //        var transformMatrix = new Matrix();
                //        //transformMatrix.Translate(0, screenHeight);
                //        transformMatrix.Rotate(-ElevationAngle);
                //        rectPath.Transform(transformMatrix);

                //        graphics.DrawPath(blackPen, rectPath);
                //        graphics.FillPath(blackBrush, rectPath);
                //    }

                // Вот так получается прямоугольник повернутый, и там где надо
                {
                    graphics.ResetTransform();
                    GraphicsPath rectPath = new GraphicsPath();
                    //rectPath.AddRectangle(r);
                    rectPath.AddRectangle(new RectangleF(0, 0, cannonLength, cannonDiameter));
                    var transformMatrix = new Matrix();
                    transformMatrix.Translate(0, screenHeight);
                    transformMatrix.Rotate(-ElevationAngle);
                    rectPath.Transform(transformMatrix);

                    graphics.DrawPath(blackPen, rectPath);
                    graphics.FillPath(blackBrush, rectPath);
                }

                //// Вот так получается прямуугольник на месте, но не повернутый.
                //// В графике положительный угол почему-то считается по часовой стрелке, а не влево, как в математике.
                //{ 
                //graphics.ResetTransform();
                //graphics.Transform.Rotate(-ElevationAngle);
                //graphics.DrawRectangle(blackPen, r);
                //graphics.FillRectangle(blackBrush, r);
                //}

                //// Вот так получается пушку вообще не видно
                //// В графике положительный угол почему-то считается по часовой стрелке, а не влево, как в математике.
                //{
                //    graphics.ResetTransform();


                //    //graphics.Transform.Rotate(-ElevationAngle);
                //    // вместо этого:
                //    Matrix myMatrix = new Matrix();
                //    myMatrix.Rotate(45, MatrixOrder.Append);
                //    graphics.Transform = myMatrix;


                //    graphics.DrawRectangle(blackPen, r);
                //    graphics.FillRectangle(blackBrush, r);
                //}


                if (JustFired)
                {
                    // Можно отрисовать огонь и дым, или что пушка откатилась
                }
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