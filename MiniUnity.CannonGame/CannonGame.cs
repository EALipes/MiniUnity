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


        protected Cannon cannon { get; set; }
        protected Projectile projectile { get; set; } //TODO: Может, убрать это? Пусть при каждом выстреле создается новое ядро?

        public float Velocity { get; set; }
        public float Angle { get; set; }

        /// <summary> Масштаб изображения - метров в сантиметре экрана
        /// </summary>
        public float ScreenScale { get; set; }

        /// <summary> Проигрывать звуки?
        /// 
        /// </summary>
        public bool PlaySound { get; set; }
    }
}