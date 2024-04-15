﻿namespace MiniUnity.CannonGame
{
    public class CannonGame : Game
    {
        public CannonGame()
        {
            //FramesPerSecond = CannonGameFramesPerSecond;
            Scene=new CannonScene();
            //cannon = new Cannon();
            ////projectile = new Projectile();
            //Scene.AddComponent(cannon);
            //Scene.AddComponent(projectile);
        }


        protected Cannon Cannon
        {
            get
            {
                return ((CannonScene) Scene).Cannon;
            }
        }
        //protected Projectile projectile { get; set; } //TODO: Может, убрать это? Пусть при каждом выстреле создается новое ядро?

        public float Velocity {
            get
            {
                return Cannon.Velocity;

            }
            set
            {
                Cannon.Velocity = value;
            }
        }

        public float Angle
        {
            get { return Cannon.ElevationAngle; }
            set { Cannon.ElevationAngle = value; }
        }

        /// <summary> Масштаб изображения - метров в сантиметре экрана
        /// </summary>
        public float ScreenScale { get; set; }

        /// <summary> Проигрывать звуки?
        /// 
        /// </summary>
        public bool PlaySound { get; set; }
    }
}