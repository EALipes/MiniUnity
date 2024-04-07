using System;
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
        
        public float Velocity { get; set; }

        public float ElevationAngle { get; set; }

        public Projectile Projectile { get; set; }
    }
}