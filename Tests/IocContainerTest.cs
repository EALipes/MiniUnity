using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniUnity.IOC;

namespace Tests
{
    [TestClass]
    public class IocContainerTest
    {


        [TestMethod]
        public void TestProjectileAndDrawerClasses()
        {
            // Просто проверяем, что наши тестируемые классы работают и могут отработать вызываемые методы.

            var drawer = new ProjectileDrawer();
            var drawerColored = new ProjectileDrawerColored();

            {
                var projectile = new Projectile(drawer);
                projectile.Position = new Vector3(10, 10, 0);
                projectile.Velocity = new Vector3(10, 10, 0);
                // вывод 
                //Console.WriteLine();
                //Console.WriteLine(nameof(projectile) +": " + projectile.GetType().Name);
                //Console.WriteLine("вывод силами специального объекта");
                projectile.Draw();
            }

            {
                var projectile = new Projectile(drawerColored);
                projectile.Position = new Vector3(40, 40, 0);
                projectile.Velocity = new Vector3(20, 20, 0);
                // вывод 
                //Console.WriteLine();
                //Console.WriteLine(nameof(projectile) +": " + projectile.GetType().Name);
                //Console.WriteLine("вывод силами специального объекта");
                projectile.Draw();
            }
        }

        [TestMethod]
        public void TestRegisterType()
        {
            var container = new Container();
            container.RegisterType(typeof(IDrawProjectiles), typeof(ProjectileDrawer));
            var registered = container.IsRegistered(typeof(IDrawProjectiles), null);
            // TODO: А где нам узнать, есть ли какие-то реализации для вот такого интерфейса?
            // TODO: А если один класс реализует два интерфейса? И зарегистрирован как реализатор первого, а нам он нужен для второго?
            Assert.IsTrue(registered, "Должен быть зарегистрирован IDrawProjectiles");
            //Assert.IsFalse(registered, "Не должен быть зарегистрирован IDrawProjectiles");
            
            //var registered2 = container.IsRegistered(typeof(ProjectileDrawer), null);
            //Assert.IsTrue(registered2, "Не зарегистрирован ProjectileDrawer");
            //var registered3 = container.IsRegistered(typeof(ProjectileDrawerColored), null);
            //Assert.IsFalse(registered3, "Не должен быть зарегистрирован ProjectileDrawerColored");
        }

        [TestMethod]
        public void TestResolve()
        {
            var container = new Container();
            var abstractionType = typeof(IDrawProjectiles);
            container.RegisterType(abstractionType, typeof(ProjectileDrawer));
            var obj = container.Resolve(abstractionType);
            Assert.IsNotNull(obj, "Не удалось восстановить объект из контейнера");

            // TODO: А как насчет типизованного метода (вероятно, параметризованного)?
            // TODO: А нужен ли нам вообще этот слабо типизованный метод?
            // TODO: Наверное, нужна проверка, что реализация соответствует абстрактному интерфейсу?
            
            // TODO: А как насчет создания незаригистрированного неабстрактного типа?
            
            // TODO: А как насчет создания неабстрактного типа, требующего в конструкторе параметр абстрактного типа ?

            // TODO: Проверить работу с Singleton


        }


        [TestMethod]
        public void TestResolveAndUse()
        {
            var container = new Container();

            container.RegisterType(typeof(IDrawProjectiles), typeof(ProjectileDrawer));

            var obj = container.Resolve(typeof(IDrawProjectiles));
            Assert.IsNotNull(obj, "obj == null");
            var drawer = obj as IDrawProjectiles;
            Assert.IsNotNull(drawer, "drawer == null");

            var projectile = new Projectile(drawer);
            projectile.Draw(); 
        }


    }

    #region Classes for testing

    public interface IProjectile
    {
        Vector3 Position { get; set; }
        Vector3 Velocity { get; set; }
    }


    public class Projectile : IProjectile
    {
        public Projectile(IDrawProjectiles drawer)
        {
            this.drawer = drawer;
        }

        public void Draw()
        {
            drawer.Draw(this);
        }

        private IDrawProjectiles drawer;

        // Положение снаряда
        public Vector3 Position { get; set; } = new Vector3();

        // Скорость снаряда
        public Vector3 Velocity { get; set; } = new Vector3();

    }

    public interface IDrawProjectiles
    {
        void Draw(IProjectile p);
    }

    /// <summary> Отрисовщик ядра
    /// 
    /// </summary>
    public class ProjectileDrawer : IDrawProjectiles
    {
        public virtual void Draw(IProjectile p)
        {
            Console.WriteLine("вывод силами "+ GetType().Name +"." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            Console.WriteLine(p.GetType().Name + ":  " + nameof(p.Position) + "=" + p.Position + "; " + nameof(p.Velocity) + "=" + p.Velocity);
        }
    }


    /// <summary> Отрисовщик ядра, рисующий другим цветом
    /// 
    /// </summary>
    public class ProjectileDrawerColored : ProjectileDrawer, IDrawProjectiles
    {
        public override void Draw(IProjectile p)
        {
            Console.WriteLine("вывод силами "+ GetType().Name +"." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);

            var saveCol = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(p.GetType().Name + ":  " + nameof(p.Position) + "=" + p.Position + "; " + nameof(p.Velocity) + "=" + p.Velocity);
            Console.ForegroundColor = saveCol;
        }
    }


    #endregion
}
