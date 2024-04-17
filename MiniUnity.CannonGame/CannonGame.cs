namespace MiniUnity.CannonGame
{
    public class CannonGame : Game
    {
        public CannonGame()
        {
            Scene=new CannonScene();
        }


        protected Cannon Cannon
        {
            get
            {
                return ((CannonScene) Scene).Cannon;
            }
        }

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