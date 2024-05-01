using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniUnity.IOC;

using IOC.TestedClasses;

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
            container.RegisterType(typeof(IDraw<Projectile>), typeof(ProjectileDrawer));
            //container.RegisterType(typeof(IDraw), typeof(ProjectileDrawer));
            var registered = container.IsRegistered(typeof(IDraw), null);
            Assert.IsTrue(registered, "Должен быть зарегистрирован IDrawProjectiles");
            
            // TODO: Проверить регистрацию для случаев: - Просто тип 
            // TODO: Проверить регистрацию для случаев: - Тип с указанием предназначения 
            // TODO: Проверить регистрацию для случаев: - Повторная регистрация - должна выдаваться ошибка
            // TODO: Проверить регистрацию для случаев: - Регистрация без назначения + Регистрация с назначением - допустимо?? (Пока не решил)
            // TODO: Проверить регистрацию для случаев: - При проверке IsRegistered сравнение UsedForType c null должно обрабатываться корректно во всех случаях
            // (type==type, type!=null, null!=type, null==null)


            // TODO: Проверить - соответствует ли регистрируемый абстрактный тип указанному конкретному типу (приводим ли implementationType к abstractionType

        }

        [TestMethod]
        public void TestResolve()
        {
            var container = new Container();
            var abstractionType = typeof(IDraw);
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

            container.RegisterType(typeof(IDraw), typeof(ProjectileDrawer));

            var obj = container.Resolve(typeof(IDraw));
            Assert.IsNotNull(obj, "obj == null");
            var drawer = obj as IDraw;
            var projectileDrawer = obj as ProjectileDrawer;
            Assert.IsNotNull(drawer, "drawer == null");
            Assert.IsNotNull(projectileDrawer, "projectileDrawer == null");

            var projectile = new Projectile(projectileDrawer);
            projectile.Draw(); 
        }


    }

}
