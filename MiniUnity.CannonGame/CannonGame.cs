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

        protected Cannon cannon { get; set; }
        protected Projectile projectile { get; set; } //TODO: Может, убрать это? Пусть при каждом выстреле создается новое ядро?

        public float Velocity { get; set; }
        public float Angle { get; set; }

        /// <summary> Масштаб изображения - метров в сантиметре экрана
        /// </summary>
        public float ScreenScale { get; set; }

        // Проигрывать звуки?
        public bool PlaySound { get; set; }
    }
}