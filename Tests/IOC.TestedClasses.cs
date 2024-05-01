using System;
using System.Numerics;

/// <summary> Набор простых классов для тестирования IOC-контейнера
/// 
/// </summary>
namespace IOC.TestedClasses
{

    #region Interfaces     
    /// <summary> Интерфейс для рисования игровых объектов.
    /// Он может вообще не определять никаких методов,
    /// играя роль просто метки объекта, используемого для отрисовки.
    /// А для каждой платформы будут уже свои потомки этого интерфейса, определяющие методы и параметры,
    /// используемые в этой платформе.
    /// </summary>
    public interface IDraw
    {
        //void Draw();
        void Draw(object obj);
        //void Draw<T>(T p);
        //void Draw(IProjectile p);
    }

    /// <summary> Интерфейс для рисования игровых объектов.
    /// </summary>
    /// <typeparam name="T">Какого типа объект будет отрисовываться</typeparam>
    public interface IDraw<T> : IDraw
    where T : GameObject
    {
        //void Draw<T>(T gameObject);
    }
    #endregion Interfaces

    #region GameObjects
    public class GameObject
    {
        public GameObject(IDraw drawer)
            //where T: this
        {
            this.drawer = drawer;
        }

        // TODO: Подозреваю, что придется сделать его public
        protected internal IDraw drawer;
        //protected internal IDraw drawer;

        public void Draw()
        {
            // Если мы хотим вызвать метод отрисовщика из игрового объекта - придется использовать непараметризованный интерфейс,
            // что не очень удобно и не очень красиво.
            // TODO: Похоже, нежелательно вызывать методы Drawer из GameObject - это создает лишние зависимости
            drawer.Draw(this);
        }
    }


    public class Projectile : GameObject
    {
        public Projectile(ProjectileDrawer drawer) : base(drawer)
        {
        }

        // Положение снаряда
        public Vector3 Position { get; set; } = new Vector3();
        // Скорость снаряда
        public Vector3 Velocity { get; set; } = new Vector3();
    }

    public class Cannon : GameObject
    {
        public Cannon(IDraw drawer) : base(drawer)
        {
        }

        public Vector3 Position { get; set; }
        public float Velocity { get; set; }
        public float Angle { get; set; }
    }

    public class Target : GameObject
    {
        public Target(IDraw drawer) : base(drawer)
        {
        }

        public Vector3 Center { get; set; } = new Vector3();
        public float Radius { get; set; }

    }
    #endregion GameObjects

    #region Drawers
    public class GameObjectDrawer<T> : IDraw<T>
    where T : GameObject
    {
        /// <summary> Отрисовка с указанием отрисовываемого объекта. 
        /// Причем с типизованным указанием - чтоб видеть его свойства, без приведений типа и т.п. трюков.
        /// </summary>
        /// <param name="p">Отрисовываемый объект</param>
        /// <remarks>
        /// ! Можно было бы задать отрисовываемый объект в свойстве отрисовщика.
        /// ! Но лучше сделать явный параметр.
        /// ! Тогда мы можем решать - будет ли создаваться свой отрисовщик для каждого объекта, 
        /// ! или же можно создать один отрисовщик и отрисовать им все объекты соотв. типа.
        /// Поэтому вызывать придется как projectile.drawer.Draw(projectile),
        /// что на первый взгляд кажется несколько излишним.
        /// </remarks>
        public virtual void Draw(T p)
        {
            // TODO: Надо ли тут возбуждать исключение???
            //throw new NotImplementedException("Не перекрыт метод Draw");
        }


        // TODO: И вот, похоже, такую фигню придется творить с каждым методом, который мы хотим вызывать через GameObject или непараметризованный IDrawer
        void IDraw.Draw(object obj)
        {
            T p = (T)obj;
            Draw(p);
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
            Console.WriteLine("вывод силами " + GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            Console.WriteLine(p.GetType().Name + ":  " + nameof(p.Position) + "=" + p.Position + "; " + nameof(p.Velocity) + "=" + p.Velocity);
        }
    }


    /// <summary> Отрисовщик ядра, рисующий другим цветом
    /// 
    /// </summary>
    public class ProjectileDrawerColored : ProjectileDrawer, IDraw
    {
        public override void Draw(Projectile p)
        {
            Console.WriteLine("вывод силами " + GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            var saveCol = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(p.GetType().Name + ":  " + nameof(p.Position) + "=" + p.Position + "; " + nameof(p.Velocity) + "=" + p.Velocity);
            Console.ForegroundColor = saveCol;
        }
    }

    public class CannonDrawerColored : GameObjectDrawer<Cannon>
    {
        public override void Draw(Cannon cannon)
        {
            Console.WriteLine("вывод силами " + GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            //var p = cannon;

            var saveCol = Console.ForegroundColor;
            var saveBkCol = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(cannon.GetType().Name + ":  " + nameof(cannon.Position) + "=" + cannon.Position + "; " + nameof(cannon.Velocity) + "=" + cannon.Velocity + "; " + nameof(cannon.Angle) + "=" + cannon.Angle);
            Console.ForegroundColor = saveCol;
            Console.BackgroundColor = saveBkCol;
        }
    }


    public class TargetDrawer : GameObjectDrawer<Target>
    {
        public override void Draw(Target target)
        {
            Console.WriteLine("вывод силами " + GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            var saveCol = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(target.GetType().Name + ":  " + nameof(target.Center) + "=" + target.Center + "; " + nameof(target.Radius) + "=" + target.Radius);
            Console.ForegroundColor = saveCol;
        }
    }
    #endregion Drawers
}