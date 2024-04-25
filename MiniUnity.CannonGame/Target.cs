using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniUnity.CannonGame
{
    public class Target : GameObject
    {
        protected CannonGame Game { get; set; }
        protected CannonScene Scene { get; set; }

        public Vector3 Position { get; set; } = new Vector3();

        protected SoundPlayer targetHitSoundPlayer;

        public Target()
        {
            targetHitSoundPlayer = new SoundPlayer(Resources.TargetHit);
            SetNewPosition();
        }

        public override void Start()
        {
            Game = GetParentComponent<CannonGame>() as CannonGame;
            if (Game == null)
                throw new NullReferenceException("Не найден объект игры");

            Scene = GetParentComponent<CannonScene>() as CannonScene;
            if (Scene == null)
                throw new NullReferenceException("Не найден объект сцены");

            base.Start();
        }

        public override void Update()
        {
            //надо узнать растояние между снарядом и мишенью (может делать это не внутри, а где-то снаружи???)
            //берем из сцены положение снаряда
            //Projectile projectile = Scene.GetComponent<Projectile>;
            //Vector3 projectilePosition = projectile.Position;
            //или так
            /*
            Vector3 projectilePosition = Scene.GetComponent<Projectile>.Position;

            //определяем кординаты вектора
            Vector3 vectorCoordinates = projectilePosition - Position;

            //определяем расстояние между снарядом и мишенью
            double distanceToTarget = Math.Abs(Math.Sqrt(Math.Pow(vectorCoordinates.X, 2) + Math.Pow(vectorCoordinates.Y, 2)));

            //если расстояние между снарядом и мишенью меньше, чем сумма их размеров (т.е. объекты соприкаснулись), то попадание есть
            //пока что условно указываем размер
            int targetSize = 3;
            int projectileSize = 3;

            if (distanceToTarget < targetSize + projectileSize)
            {
                Hit();
            }
            */

            base.Update();
        }

        private void Hit()
        {
            if (Game.PlaySound)
                targetHitSoundPlayer.Play();

            //Меняем положение мишени на новое
            SetNewPosition();
        }

        private void SetNewPosition()
        {
            //установить мишень в случайном месте в границах игрового (но как узнать размеры игрового поля?)
        }

        protected override void Draw_OnWinFormsPaintEvent(object sender, PaintEventArgs args)
        {

            //мишень (и пушку) надо отрисовывать до начала игры, до выстрела
            if (Game == null)
            {
                Debug.Write("Game==null при отрисовке Projectile.Draw_OnWinFormsPaintEvent");
                return;
            }

            var graphics = args.Graphics;

            try
            {
                graphics.ResetTransform();

                Pen redPen = new Pen(Color.Red, 3);
                Brush redBrush = new SolidBrush(Color.Red);

                // Масштаб экрана - в мм
                graphics.PageUnit = GraphicsUnit.Millimeter;

                var targetRectSize = 5;

                // координаты ммишени (в метрах)
                var prX = (float)Position.X;
                var prY = (float)Position.Y;

                // отмасштабируем эти координаты, чтоб все вместилось в экран
                // масштаб мы задаем в метрах на сантиметр, а экран у нас меряется в миллиметрах (GraphicsUnit.Millimeter)
                prX = prX * 10 / Game.ScreenScale;
                prY = prY * 10 / Game.ScreenScale;

                // учтем размер рисуемого прямоугольника, и соответственно сместим его начало
                // учтем, что началом координаты Y у нас должен быть конец (нижний) экрана
                // и что координата Y в игре направлена вверх, а у нас на экране - вниз
                var screenHeight = graphics.VisibleClipBounds.Height;
                var screenX = prX;
                var screenY = screenHeight - targetRectSize - prY;

                // если вышли за пределы экрана - не рисуем
                if ((screenX < 0) || (screenX > graphics.VisibleClipBounds.Width) || (screenY < 0) ||
                    (screenY > graphics.VisibleClipBounds.Height))
                {
                    return;
                }

                RectangleF r = new RectangleF(screenX, screenY, targetRectSize, targetRectSize);
                graphics.DrawEllipse(redPen, r);
                graphics.FillEllipse(redBrush, r);

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
    }
}
