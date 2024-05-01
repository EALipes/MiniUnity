using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniUnity.IOC
{
    // Примерно такое было в Unity
    public interface IContainer : IDisposable
    {
        IContainer RegisterType(Type abstractionType, Type implementationType, string name=null, LifeTimePolicy policy = LifeTimePolicy.CreateNew );

        void UnRegisterType(Type abstractionType, Type implementationType, string name = null);

        //IContainer RegisterInstance(Type abstractionType, string name, object implementationInstance);

        //IContainer RegisterFactory(Type abstractionType, string name, Func<IContainer, Type, string, object> factory);

        bool IsRegistered(Type type, string name);

        IEnumerable<IContainerRegistration> Registrations { get; }

        object Resolve(Type type, string name);

        //object Build(Type type, object existing);

    }


    /// <summary> Используемые политики создания объектов 
    /// </summary>
    public enum LifeTimePolicy
    {
        /// <summary> Каждый раз создается новый объект 
        /// </summary>
        CreateNew = 1,
        /// <summary> Создается один объект, отрабатывающий все обращения к указанному интерфейсу
        /// </summary>
        Singletone = 2
    };


    // Примерно такое было в Unity
    public interface IContainerRegistration
    {
        /// <summary> Интерфейс (или класс),  используемый как абстракция
        /// </summary>
        Type AbstractionType { get; }

        /// <summary> Реальный класс, реализующий этот интерфейс 
        /// </summary>
        //Type MappedToType { get; }
        Type ImplementationType { get; }

        /// <summary> Необязательное имя для пары интерфейс-класс.
        /// Можно задать несколько реализаций одного интерфейса, тогда нужно будет дать им разные имена. 
        /// </summary>
        string Name { get; }

    }


    /// <summary> Контейнер, способный создавать объекты требуемого абстрактного типа
    /// Конкретный тип создаваемого объекта ищется в списке регистрированных типов.
    /// 
    /// </summary>
    public class Container
    {
        public void RegisterType(Type abstractionType, Type implementationType,
            Type usedForType = null, LifeTimePolicy policy = LifeTimePolicy.CreateNew)
        {
            var e = FindRegisteredType(abstractionType, usedForType);
            if (e == null)
            {
                e = new ContainerRegistrationElement(abstractionType, implementationType, usedForType, policy);
                Registrations.Add(e);
            }
            else
            {
                // TODO: Вот тут надо подумать - то ли замещать найденный элемент, то ли оставлять, то ли выдавать ошибку
                // Пока сделаем замещение
                Registrations.Remove(e);
                e = new ContainerRegistrationElement(abstractionType, implementationType, usedForType, policy);
                Registrations.Add(e);
            }
        }

        public void UnRegisterType(Type abstractionType, Type implementationType, Type usedForType = null)
        {
            var l = Registrations
                .Where(e => (
                             (e.AbstractionType == abstractionType) 
                             && (e.ImplementationType == implementationType) 
                             && (e.UsedForType == usedForType))
                            )
                .ToList();
            foreach (var element in l)
            {
                Registrations.Remove(element);
            }
            
        }


        public bool IsRegistered(Type abstractionType, Type usedForType=null)
        {
            var e = FindRegisteredType(abstractionType, usedForType);
            return (e != null); 
        }

        /// <summary> Поиск зарегистрированной реализации абстрактного типа (интерфейса)
        /// 
        /// </summary>
        /// <param name="type">абстрактный тип или интерфейс</param>
        /// <param name="name">необязательное имя для регистрации
        /// <remarks>
        /// Может быть зарегистрирован один элемент указанного типа без имени, и произвольное количество элементов того же типа с разными именами
        /// </remarks>
        /// </param>
        /// <returns></returns>
        /// 
        protected ContainerRegistrationElement FindRegisteredType(Type type, Type usedForType=null)
        {
            foreach (var element in Registrations)
            {
                if ((element.AbstractionType == type)
                    && (element.UsedForType==usedForType)) 
                {
                    return element;
                }
            }

            return null;
        }



        protected List<ContainerRegistrationElement> Registrations { get; } = new List<ContainerRegistrationElement>();

        /// <summary> Создание объекта требуемого типа
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object Resolve(Type abstractionType, Type usedForType = null)
        {
            var e = FindRegisteredType(abstractionType, usedForType);
            if (e == null)
            {
                // Если нет указаний, какой тип создавать вместо указанного - пробуем создать указанный
                var obj = CreateNew(abstractionType);
                return obj;
            }

            if (e.Policy == LifeTimePolicy.Singletone)
            {
                if (e.Instance != null)
                    return e.Instance;
                else
                {
                    var obj = CreateNew(e.ImplementationType);
                    e.Instance = obj;
                    return obj;
                }
            }

            // 
            {
                var obj = CreateNew(e.ImplementationType);
                return obj;
            }
        }

        // То же, но в типизованном варианте. Попробуем избавиться от создания нетипизованного объекта
        public T Resolve<T>(Type usedForType = null)
            where T:class
        {
            var abstractionType = typeof(T);
            var e = FindRegisteredType(abstractionType, usedForType);
            if (e == null)
            {
                // Если нет указаний, какой тип создавать вместо указанного - пробуем создать указанный
                if (abstractionType.IsAbstract)
                {
                    //throw new MissingMethodException("Не указан конкретный создаваемый класс, создать объект типа "+abstractionType.Name + " не удалось");
                    return null;
                }
                var obj = CreateNew<T>();
                //var obj = CreateNew(abstractionType);
                return obj;
            }

            // Если указано, что используется один экземпляр:
            if (e.Policy == LifeTimePolicy.Singletone)
            {
                if (e.Instance == null)
                {
                    var obj = CreateNew(e.ImplementationType);
                    //var obj = CreateNew(e.ImplementationType);
                    e.Instance = obj;
                }
                return e.Instance as T;
            }

            // Самый общий случай:
            {
                var obj = CreateNew(e.ImplementationType);
                var typed = obj as T;
                return typed;
            }
        }

        private object CreateNew(Type type)
        {
            var obj = Activator.CreateInstance(type);
            //var typed = obj as typeof(type);
            return obj;
        }

        private T CreateNew<T>()
            where T:class
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance<T>();
            return obj;
        }
    }


    public class ContainerRegistrationElement
    {
        /// <summary> Интерфейс (или класс),  используемый как абстракция
        /// </summary>
        public Type AbstractionType { get; private set; }

        /// <summary> Реальный класс, реализующий этот интерфейс 
        /// </summary>
        //Type MappedToType { get; }
        public Type ImplementationType { get; private set; }

        /// <summary> Тип объекта, для которого предназначен создаваемый объект
        /// (например, IDrawer для объектов типа Projectile - умеет отрисовывать ядро, поэтому тут будет стоять Projectile)
        /// Можно задать несколько реализаций одного интерфейса, для разных использующих классов. 
        /// </summary>
        public Type UsedForType { get; private set; }
        /// ClientType
        /// UserType
        /// UsedForType


        /// <summary> Политика создания экземпляра - 
        /// будет ли при каждом вызове создаваться новый экземпляр указанного класса, 
        /// или же всегда будет возвращаться один экземпляр 
        /// </summary>
        public LifeTimePolicy Policy  { get; private set; }

        /// <summary> Экземпляр, обслуживающий вызовы в политике Singleton 
        /// </summary>
        public object Instance  { get; protected internal set; }


        public ContainerRegistrationElement(Type abstractionType, Type implementationType, Type usedForType=null, LifeTimePolicy policy=LifeTimePolicy.CreateNew)
        {
            AbstractionType = abstractionType;
            ImplementationType = implementationType;
            UsedForType = usedForType;
            Policy = policy;
        }
    }


}
