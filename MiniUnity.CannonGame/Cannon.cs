using System;
using System.Numerics;

namespace MiniUnity.CannonGame
{
    public class Cannon: GameObject
    {
        public Vector3 Position = new Vector3();
        //public Point Position = new Point();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectile">Снаряд, которым будем стрелять</param>
        /// <param name="elevationAngle">Угол возвышения в градусах</param>
        /// <param name="velocity">Скорость снаряда</param>
        public void Fire(Projectile projectile, float elevationAngle, float velocity)
        {
            // Отрисовываем снаряд на месте пуска
            projectile.Update();

            projectile.Fallen = false;
            //projectile
            // TODO! Вывод поправить, сделать в зависимости от типа приложения.
            Console.WriteLine("Бабах!");

            projectile.Position.X = Position.X;
            projectile.Position.Y = Position.Y;

            var elevationAngleInRadians = elevationAngle * Math.PI / 180;

            projectile.Velocity.X = (float) (velocity * Math.Cos(elevationAngleInRadians));
            projectile.Velocity.Y = (float) (velocity * Math.Sin(elevationAngleInRadians));
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