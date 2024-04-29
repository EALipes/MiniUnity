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

        protected SoundPlayer targetHitSoundPlayer;
        protected bool targetOff;

        public Vector3 Position { get; set; } = new Vector3();
        public int targetSize { get; set; }

        public Target()
        {
            targetHitSoundPlayer = new SoundPlayer(Resources.TargetHit);
            targetOff = true;
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
            base.Update();
        }

        private void Hit()
        {
            if (Game.PlaySound)
                targetHitSoundPlayer.Play();
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
                //TODO: надо как-то связать размер мишени с масштабом экрана?
                graphics.ResetTransform();

                // Масштаб экрана - в мм
                graphics.PageUnit = GraphicsUnit.Millimeter;

                //Узнаем размер экрана
                var screenHeight = graphics.VisibleClipBounds.Height;
                var screenLenght = graphics.VisibleClipBounds.Right;

                //если мишень не активна, задать ей расположение и размер
                if(targetOff)
                {
                    //Задать случайное место расопложения
                    Random random = new Random();
                    int randomNumber = random.Next(60, (int)screenLenght);
                    Position = new Vector3(randomNumber, 0, 0);

                    //Задать случайный размер мишени
                    targetSize = random.Next(10, 40);

                    targetOff = !targetOff;
                }

                //Определяем начало и конец мишени
                PointF pointStart = new PointF(Position.X, screenHeight);
                PointF pointEnd = new PointF(Position.X - targetSize, screenHeight);

                Pen redPen = new Pen(Color.Red, 2f);
                graphics.DrawLine(redPen, pointStart, pointEnd);

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
