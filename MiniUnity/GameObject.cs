using System.Collections.Generic;
using System.Linq;

namespace MiniUnity
{
    /// <summary>
    /// Основной класс для всех объектов, из которых строится игра
    /// </summary>
    public class GameObject
    {

        /// <summary>
        /// Родительский объект
        /// </summary>
        public GameObject Parent { get; set; }

        /// <summary>
        /// Вложенные объекты, подчиняющиеся текущему
        /// </summary>
        protected List<GameObject> Children = new List<GameObject>();

        #region // Функции добавления и поиска объектов

        /// <summary>
        /// Найти в родителях
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T  GetParentComponent<T>()
            where T:GameObject
        {
            GameObject result = null;
            if (Parent == null) return null;
            var parentType = Parent.GetType();
            if ((parentType == typeof(T))
                ||
                (parentType.IsSubclassOf(typeof(T))))
                return Parent as T;

            return Parent.GetParentComponent<T>();
        }

        // При добавлении компонента у него устанавливается свойство Parent = this
        public void AddComponent(GameObject gameObject)
        {
            Children.Add(gameObject);
            gameObject.Parent = this;
        }
        public void RemoveComponent(GameObject gameObject)
        {
            Children.Remove(gameObject);
            gameObject.Parent = null;
        }

        /// <summary>
        /// Найти в подчиненных объектах объект указанного типа (или его потомка)
        /// Возвращается первый найденный подходящий объект
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>()
            where T:GameObject
        {
            // Мне лень честно писать поиск первого подходящего элемента такого типа в списке,
            // поэтому я использовал LINQ, хотя это заклинание проходят только на старших курсах Хогвартса )))
            var result = Children?.FirstOrDefault(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))) ) as T;
            return result;
        }

        /// <summary>
        /// Найти в подчиненных объектах все объекты указанного типа (или его потомка)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICollection<T> GetComponents<T>()
            where T:GameObject
        {
            // Тоже LINQ
            var result = Children?
                .Where(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))))
                .Select(b=> b as T)
                .ToList();
            return result;
        }

        /// <summary>
        /// Найти в подчиненных объектах поведение указанного типа (или его потомка)
        /// Возвращается первый найденный подходящий объект
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetBehavior<T>()
            where T:BehaviorComponent
        {
            // Мне лень честно писать поиск первого подходящего элемента такого типа в списке,
            // поэтому я использовал LINQ, хотя это заклинание проходят только на старших курсах Хогвартса )))
            var result = Children?.FirstOrDefault(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))) );
            return result as T;
        }

        /// <summary>
        /// Найти в подчиненных объектах все объекты-поведения указанного типа (или его потомка)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICollection<T> GetBehaviors<T>()
            where T:BehaviorComponent
        {
            // Тоже LINQ
            var result = Children?
                .Where(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))))
                .Select(b => b as T)
                .ToList();
            return result;
        }

        #endregion


        /// <summary>
        /// Вызывается при старте программы или сцены, после того, как все элементы уже созданы
        /// </summary>
        public virtual void Start()
        {
            foreach (GameObject c in Children)
            {
                c.Start();
                if (OnStart != null) OnStart(this);
            }
        }


        /// <summary>
        /// Обновить объект. 
        /// Тут обновляюеся положение, или производится отрисовка, или т.п.
        /// </summary>
        public virtual void Update()
        {
            foreach (GameObject c in Children)
            {
                c.Update();
                if (OnUpdate != null) OnUpdate(this);
            }
        }

        /// <summary>
        /// Событие вызывается при инициализации объекта игры
        /// </summary>
        public event OnStartHandler  OnStart;

        /// <summary>
        /// Событие вызывается при обновлении состояния объекта игры
        /// </summary>
        public event OnUpdateHandler  OnUpdate;

        public delegate void OnStartHandler(GameObject gameObject);
        public delegate void OnUpdateHandler(GameObject gameObject);

    }
}