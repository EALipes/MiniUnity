using System;
using System.Numerics;

namespace IOC.TestedClasses3
{
    /// 
    /// Изящный вариант вынесения зависимостей, предложенный Дмитрием Топорковым
    /// 


    #region Interfaces
    public interface IDraw
    {
        void Draw();
    }

    public interface IDraw<T>
    {
        void Draw(T gameObject);
    }

    #endregion


    #region GameObjects

    // Вообще ничего не знает о рисовании
    public class GameObject
    {
    }

    public class Projectile : GameObject
    {
        // Положение снаряда
        public Vector3 Position { get; set; } = new Vector3();

        // Скорость снаряда
        public Vector3 Velocity { get; set; } = new Vector3();
    }

    public class Cannon : GameObject
    {
        public Vector3 Position { get; set; }
        public float Velocity { get; set; }
        public float Angle { get; set; }
    }

    public class Target : GameObject
    {
        public Vector3 Center { get; set; } = new Vector3();
        public float Radius { get; set; }
    }


    #endregion


    #region Drawers

    public class GameObjectDrawer<T>: IDraw<T>
    where T:GameObject
    {
        public virtual void Draw(T p)
        {
            
        }

    }

    /// <summary> Отрисовщик ядра
    /// 
    /// </summary>
    public class ProjectileDrawer : GameObjectDrawer<Projectile>
    {
        //public void Draw(GameObject p)
        //{
        //    Draw(p as Projectile);
        //}

        public override void Draw(Projectile p)
        {
            Console.WriteLine("вывод силами "+ GetType().Name +"." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            Console.WriteLine(p.GetType().Name + ":  " + nameof(p.Position) + "=" + p.Position + "; " + nameof(p.Velocity) + "=" + p.Velocity);
        }
    }


    /// <summary> Отрисовщик ядра, рисующий другим цветом
    /// 
    /// </summary>
    public class ProjectileDrawerColored : ProjectileDrawer //, IDraw
    {
        public override void Draw(Projectile p)
        {
            Console.WriteLine("вывод силами "+ GetType().Name +"." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            var saveCol = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(p.GetType().Name + ":  " + nameof(p.Position) + "=" + p.Position + "; " + nameof(p.Velocity) + "=" + p.Velocity);
            Console.ForegroundColor = saveCol;
        }
    }

    public class CannonDrawer : GameObjectDrawer<Cannon> //, IDraw
    {
        public override void Draw(Cannon cannon)
        {
            Console.WriteLine("вывод силами "+ GetType().Name +"." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            var saveCol = Console.ForegroundColor;
            var saveBkCol = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(cannon.GetType().Name + ":  " + nameof(cannon.Position) + "=" + cannon.Position + "; " + nameof(cannon.Velocity) + "=" + cannon.Velocity+ "; " + nameof(cannon.Angle)+"="+cannon.Angle);
            Console.ForegroundColor = saveCol;
            Console.BackgroundColor = saveBkCol;
        }
    }
    

    public class TargetDrawer : GameObjectDrawer<Target> //, IDraw
    {
        public override void Draw(Target target)
        {
            Console.WriteLine("вывод силами "+ GetType().Name +"." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            var saveCol = Console.ForegroundColor;
            var saveBkCol = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(target.GetType().Name + ":  " + nameof(target.Center) + "=" + target.Center + "; " + nameof(target.Radius) + "=" + target.Radius);
            Console.ForegroundColor = saveCol;
            Console.BackgroundColor = saveBkCol;
        }
    }
    

    #endregion


    #region Wrappers

    // Объекты, хранящие пары соответствий - GameObject --- Drawer

    public class GameObjectWrapper<T> : IDraw 
        where T: GameObject
    {
        private T gameObject;
        private IDraw<T> drawer;
	
        public GameObjectWrapper(T gameObject, IDraw<T> drawer) {
            this.gameObject = gameObject;
            this.drawer = drawer;
        }
	
        public void Draw()
        {
            drawer.Draw(gameObject);
        }
    }

    public class CannonWrapper : GameObjectWrapper<Cannon> 
    {
        public CannonWrapper(Cannon gameObject, CannonDrawer drawer) : base(gameObject, drawer) 
        {
        }
    }
    

    #endregion


    //List<IDraw> gameObjects = new List<IDraw>();
    //gameObjects.add(new CannonWrapper(new Cannon(), new CannonWPFDrawer()));



}
