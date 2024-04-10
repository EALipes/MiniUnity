using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MiniUnity
{
    /// <summary> Основной класс для всех объектов, из которых строится игра
    /// </summary>
    public class GameObject
    {

        /// <summary> Родительский объект
        /// </summary>
        public GameObject Parent { get; set; }

        /// <summary> Вложенные объекты, подчиняющиеся текущему
        /// </summary>
        protected List<GameObject> Children = new List<GameObject>();

        #region // Функции добавления и поиска объектов

        /// <summary> Найти в родителях
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

        /// <summary> Найти в подчиненных объектах объект указанного типа (или его потомка)
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

        /// <summary> Найти в подчиненных объектах все объекты указанного типа (или его потомка)
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

        /// <summary> Найти в подчиненных объектах поведение указанного типа (или его потомка)
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

        /// <summary> Найти в подчиненных объектах все объекты-поведения указанного типа (или его потомка)
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


        /// <summary> Вызывается при старте программы или сцены, после того, как все элементы уже созданы
        /// </summary>
        public virtual void Start()
        {
            foreach (GameObject c in Children)
            {
                c.Start();
                if (OnStart != null) OnStart(this);
            }
        }


        /// <summary> Обновить объект. 
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

        /// <summary> Событие вызывается при инициализации объекта игры
        /// </summary>
        public event OnStartHandler  OnStart;

        /// <summary> Событие вызывается при обновлении состояния объекта игры
        /// </summary>
        public event OnUpdateHandler  OnUpdate;

        public delegate void OnStartHandler(GameObject gameObject);
        public delegate void OnUpdateHandler(GameObject gameObject);


        #region Обработка событий клавиатуры и отрисовка

        #region Обработка событий клавиатуры

        

        #endregion


        #region Отрисовка

        /// <summary> Временное решение - тип приложения в виде перечисления
        /// </summary>
        public enum ApplicationType
        {
            ConsoleApp,
            WinFormsApp,
            WpfApp
        }

        /// <summary> Тип приложения (временное решение для указания, какие методы использовать при отрисовке и обработке клавиатуры)
        /// </summary>
        public static ApplicationType AppType { get; set; }

        /// <summary> Объект отрисовывает себя и пинает перерисоваться дочерние объекты
        /// Не рекомендуется вызывать  этот метод напрямую. Используйте RefreshDraw()
        /// </summary>
        protected virtual void Draw()
        {
            // Отрисовка себя 
            // в зависимости от типа приложения может быть разной
            // (пока тут пусто)
            if (AppType == ApplicationType.ConsoleApp)
            {
            }
            else if (AppType == ApplicationType.WinFormsApp)
            {
            }
            else if (AppType == ApplicationType.WpfApp)
            {
            }


            // Отрисовка дочерних объектов
            foreach (var child in Children)
            {
                child.Draw();
            }
        }



        /// <summary> Обновить (перерисовать) картинку.
        /// Именно этот метод должен вызываться, когда необходимо перерисовать объект.
        /// </summary>
        /// <remarks>
        /// Именно этот метод должен вызываться, когда необходимо перерисовать объект.
        /// Он может вызывать либо Draw, либо другой метод - 
        /// например, опосредованно вызывать Draw_OnWinFormsPaintEvent.
        /// Это уже сам объект лучше знает, как правильно сделать перерисовку. 
        /// По факту, если один объект требует обновить картинку, перерисовывается вся сцена.
        /// </remarks>
        public virtual void RefreshDraw()
        {
            if (Parent!=null) 
                Parent.RefreshDraw();
        }


        #region WinForms

        /// <summary> Отрисовка объектом себя в WinForms.
        /// Не рекомендуется вызывать  этот метод напрямую. Используйте RefreshDraw()
        /// </summary>
        /// <remarks>
        /// Тут нельзя прямо взять и нарисовать что-то. 
        /// Отрисовка делается при обработке окном события Paint,
        /// и может вызывать обработчики для этого события.
        /// Вот такой обработчик мы и должны определить.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Draw_OnWinFormsPaintEvent(object sender, PaintEventArgs e)
        {
            // Отрисовка себя 
            //(пока отрисовывать нечего)

            // Отрисовка дочерних объектов
            foreach (var child in Children)
            {
                child.Draw_OnWinFormsPaintEvent(sender, e);
            }
        }
        #endregion
        #endregion

        #endregion
    }
}