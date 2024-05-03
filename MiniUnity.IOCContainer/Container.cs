using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        //public object Resolve(Type abstractionType, Type usedForType = null)
        //{
        //    var e = FindRegisteredType(abstractionType, usedForType);
        //    if (e == null)
        //    {
        //        // Если нет указаний, какой тип создавать вместо указанного - пробуем создать указанный
        //        var obj = CreateNew(abstractionType);
        //        return obj;
        //    }

        //    if (e.Policy == LifeTimePolicy.Singletone)
        //    {
        //        if (e.Instance != null)
        //            return e.Instance;
        //        else
        //        {
        //            var obj = CreateNew(e.ImplementationType);
        //            e.Instance = obj;
        //            return obj;
        //        }
        //    }

        //    // 
        //    {
        //        var obj = CreateNew(e.ImplementationType);
        //        return obj;
        //    }
        //}

        // То же, но в типизованном варианте. Попробуем избавиться от создания нетипизованного объекта
        public object Resolve(Type abstractionType, Type usedForType = null)
        //public T Resolve<T>(Type usedForType = null)
        //    where T:class
        {
            // Определяем, какой тип следует создавать
            Type typeToCreate = null;
            //var abstractionType = typeof(T);
            // Ищем указания по созданию объекта в контейнере
            var implementationInfo = FindRegisteredType(abstractionType, usedForType);
            if (implementationInfo == null)
            {
                // Если нет указаний, какой тип создавать вместо указанного - пробуем просто создать указанный
                typeToCreate = abstractionType;
            }
            else
            {
                typeToCreate = implementationInfo.ImplementationType;
            }

            
            // Создаем экземпляр
            // 
            if (typeToCreate==null)
                throw new MissingMethodException("Не удалось определить создаваемый класс, наследующий "+abstractionType.Name);
            if (typeToCreate.IsAbstract)
                throw new MissingMethodException("Не указан конкретный создаваемый класс, создать объект типа "+typeToCreate.Name + " не удалось");
            
            // Если не найдено специальных указаний для создания объекта - то просто пытаемся создаем объект
            // (т.е. как если б мы создавали его не используя контейнер, а вызывая конструктор)
            if (implementationInfo == null)
            {
                var obj = CreateNew(typeToCreate);
                return obj;
            }

            else
            {
                // Если указания все же есть - создаем объект в соответствии с ними
                // Если указано, что используется один экземпляр: если он уже есть - используем, если нет - создаем, потом используем
                if (implementationInfo.Policy == LifeTimePolicy.Singletone)
                {
                    if (implementationInfo.Instance == null)
                    {
                        var obj = CreateNew(implementationInfo.ImplementationType);
                        implementationInfo.Instance = obj;
                    }
                    // Возвращаем уже имеющийся экземпляр - один для всех вызовов
                    return implementationInfo.Instance;
                }

                // Самый общий случай: создаем экземпляр указанного класса
                {
                    var obj = CreateNew(implementationInfo.ImplementationType);
                    return obj;
                }
            }
        }

        public T Resolve<T>(Type usedForType = null)
            where T : class
        {
            var obj = Resolve(typeof(T), usedForType);
            var typed = (T) obj;
            return typed;
        }


        private object CreateNew(Type type)
        {
            if (type.IsAbstract)
                throw new MissingMethodException("Невозможно создать объект абстрактного типа "+type.Name);

            //// Самый простой способ - при наличии конструктора по умолчанию, если это подходит
            //var obj = Activator.CreateInstance(type);
            //return obj;

            // Более сложный способ - ищем и вызываем конструктор через Reflection
            
            // Находим конструкторы указанного класса
            var constructors = type.GetConstructors();

            // Если мы создаем экземпляр через контейнер, то, скорей всего, нам нужен не конструктор по умолчанию, а какой-нибудь конструктор с дополнительными параметрами.
            // Можно также указать атрибут - какой конструктор использовать для DI
            ConstructorInfo mostComplicatedConstructor = null;
            ConstructorInfo defaultConstructor = null;
            ConstructorInfo recommendedConstructor = null;
            ConstructorInfo constructorToUse = null;
            int maxParameterCount = 0;
            // изучаем имеющиеся конструкторы
            foreach (var constructor in constructors)
            {
                // Непубличные конструкторы пропускаем
                if (!(constructor.IsPublic)||(constructor.IsAssembly)) continue;

                // Проверяем количество параметров
                var paramCount = constructor.GetParameters().Length;
                if (paramCount == 0)
                    defaultConstructor = constructor;
                if (paramCount > maxParameterCount)
                {
                    mostComplicatedConstructor = constructor;
                    maxParameterCount = paramCount;
                }

                // Проверяем атрибут рекомендованного конструктора
                var recommendedAttr = constructor.GetCustomAttribute<ConstructorInjectionAttribute>();
                if (recommendedAttr != null)
                    recommendedConstructor = constructor;
            }

            // Выбираем, какой конструктор использовать - если что-то вообще нашлось
            if (recommendedConstructor != null)
                constructorToUse = recommendedConstructor;
            if (mostComplicatedConstructor != null)
                constructorToUse = mostComplicatedConstructor;
            if (defaultConstructor != null)
                constructorToUse = defaultConstructor;
            if (constructorToUse==null)
                throw new InvalidOperationException();

            // Создаем объект с помощью найденного конструктора
            var paramsToUse = new List<object>(maxParameterCount);
            //var paramsToUse = new object[maxParameterCount]();
            for (int i = 0; i < maxParameterCount; i++)
            {
                
            }

            var paramValuesList = new List<object>();
            foreach (var parameterInfo in constructorToUse.GetParameters())
            {
                var paramType = parameterInfo.ParameterType;
                // Объекты для параметров создаем через Resolve, им  тоже могут понадобиться объекты в параметрах
                // В качестве второго аргумента для Resolve указываем создаваемый тип - для одного интерфейса могут быть разные реализации для разных типов
                if (paramType.IsClass)
                {
                    var paramObj = Resolve(paramType, type);
                    paramValuesList.Add(paramObj);
                }
                else // не класс
                if (parameterInfo.HasDefaultValue)
                {
                    var paramObj = parameterInfo.DefaultValue;
                    // TODO: Тут можно было бы задать значение из набора параметров типа params у метода CreateNew(type, params paramValuesConfigured)
                }
            }

            var paramValues = paramValuesList.ToArray();

            //constructorToUse.Invoke()
            //var obj = Construct
            var obj = Activator.CreateInstance(type, paramValues);
            return obj;
        }

        private T CreateNew<T>()
            where T:class
        {
            var type = typeof(T);
            var obj = CreateNew(type);
            var typed = (T) obj;
            return typed;

            //var type = typeof(T);
            //var obj = Activator.CreateInstance<T>();
            //return obj;
        }
    }


    /// <summary> Атрибут, указывающий на конструктор, который следует использовать при создании объекта через DI-контейнер
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public class ConstructorInjectionAttribute : Attribute
    {

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
