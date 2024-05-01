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

            // Простой тест создания простого объекта
            var obj = container.Resolve(abstractionType);
            Assert.IsNotNull(obj, "Не удалось восстановить объект из контейнера");

            // Простой тест создания типизованного простого объекта
            var drawer = container.Resolve<IDraw>();
            Assert.IsNotNull(drawer, "Не удалось восстановить объект из контейнера");

            // TODO: А как насчет типизованного метода (вероятно, параметризованного)?
            // TODO: А нужен ли нам вообще этот слабо типизованный метод?
            // TODO: Наверное, нужна проверка, что реализация соответствует абстрактному интерфейсу?

            // TODO: А как насчет создания незаригистрированного неабстрактного типа?

            // TODO: А как насчет создания неабстрактного типа, требующего в конструкторе параметр абстрактного типа ?

            // TODO: Проверить работу с Singleton


        }


        [TestMethod]
        public void TestResolveSpecialized()
        {
            // Тестируем регистрацию и создание объекта с несколькими реализациями одного интерфейса 
            // и указанием предназначения, для какого класса каждая реализация предназначена
            var container = new Container();
            var abstractionType = typeof(IDraw);
            container.RegisterType(abstractionType, typeof(ProjectileDrawer), typeof(Projectile));
            container.RegisterType(abstractionType, typeof(CannonDrawerColored), typeof(Cannon));

            // Простой тест создания простого объекта
            //var drawer = container.Resolve<IDraw>();
            //Assert.IsNotNull(drawer, "Не удалось восстановить объект из контейнера");
            // TODO: Так, по идее, не должно сработать. Но проверим, попробуем
            /// Действительно, возникает исключение. 
            /// Resolve доработан, теперь он просто выдает null, но может и выдать исключение.
            /// TODO: Подумать, что делать в этом случае.
            /// TODO: Может быть, сделать так, чтоб контейнер находил одну из имеющихся специализированных ассоциаций? //Хотя, вряд ли это хорошая идея... Сейчас не соображу...

            // Достаем Drawer с указанием предназначения
            var drawer1 = container.Resolve<IDraw>(typeof(Projectile));
            Assert.IsNotNull(drawer1, "Не удалось восстановить объект из контейнера");
            var resolvedType = drawer1.GetType();
            var rightType = typeof(ProjectileDrawer);
            Assert.AreEqual(rightType, resolvedType, "Должен был создаться "+rightType.Name);

            // Попробуем создать игровой объект с полученным отрисовщиком
            var drawerNeeded = drawer1 as ProjectileDrawer;
            var projectile = new Projectile(drawerNeeded);

            // Тестируем работу объекта с отрисовщиком
            projectile.drawer.Draw(projectile);
            // TODO: Тут возникает зацикливание при вызове GameObjectDrawer<T>.Draw(object obj)
            // Тут, теоретически, можно было бы сделать метод без параметра, но лучше указать отрисовываемый объект явно - см. описание метода

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
