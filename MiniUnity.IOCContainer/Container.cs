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
            string name = null, LifeTimePolicy policy = LifeTimePolicy.CreateNew)
        {
            var e = FindRegisteredType(abstractionType, name);
            if (e == null)
            {
                e = new ContainerRegistrationElement(abstractionType, implementationType, name, policy);
                Registrations.Add(e);
            }
            else
            {
                // Вот тут надо подумать - то ли замещать найденный элемент, то ли оставлять, то ли выдавать ошибку
                // Пока сделаем замещение
                Registrations.Remove(e);
                e = new ContainerRegistrationElement(abstractionType, implementationType, name, policy);
                Registrations.Add(e);
            }
        }

        public void UnRegisterType(Type abstractionType, Type implementationType, string name = null)
        {
            var e = FindRegisteredType(abstractionType, name);
            if (e != null)
            {
                Registrations.Remove(e);
            }
        }

        //IContainer RegisterInstance(Type abstractionType, string name, object implementationInstance);

        //IContainer RegisterFactory(Type abstractionType, string name, Func<IContainer, Type, string, object> factory);

        public bool IsRegistered(Type type, string name)
        {
            var e = FindRegisteredType(type, name);
            if (e == null) 
                return false;
            else 
                return true;
        }

        protected ContainerRegistrationElement FindRegisteredType(Type type, string name)
        {
            foreach (var element in Registrations)
            {
                if (element.ImplementationType == type)
                {
                    if ( (String.IsNullOrWhiteSpace(name)) && (String.IsNullOrWhiteSpace(element.Name)) )
                        return element;
                    if ( String.Compare(name, element.Name, StringComparison.InvariantCultureIgnoreCase) == 0 )
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
        public object Resolve(Type type, string name=null)
        {
            var e = FindRegisteredType(type, name);
            if (e == null)
            {
                // TODO: Не знаю пока, что тут делать. Пока вернем Null
                return null;
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

        private object CreateNew(Type type)
        {
            var obj = Activator.CreateInstance(type);
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

        /// <summary> Необязательное имя для пары интерфейс-класс.
        /// Можно задать несколько реализаций одного интерфейса, тогда нужно будет дать им разные имена. 
        /// </summary>
        public string Name { get; private set; }

        
        /// <summary> Политика создания экземпляра - 
        /// будет ли при каждом вызове создаваться новый экземпляр указанного класса, 
        /// или же всегда будет возвращаться один экземпляр 
        /// </summary>
        public LifeTimePolicy Policy  { get; private set; }

        /// <summary> Экземпляр, обслуживающий вызовы в политике Singleton 
        /// </summary>
        public object Instance  { get; protected internal set; }


        public ContainerRegistrationElement(Type abstractionType, Type implementationType, string name=null, LifeTimePolicy policy=LifeTimePolicy.CreateNew)
        {
            AbstractionType = abstractionType;
            ImplementationType = implementationType;
            Name = name;
            Policy = policy;
        }
    }


}
