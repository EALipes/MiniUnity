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
        public GameObject Parent { get; private set; }

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
            if (gameObject.Parent!=null)
                throw new InvalidOperationException("Нельзя вставить объект, уже принадлежащий другому родителю");

            Children.Add(gameObject);
            gameObject.Parent = this;
        }
        public void RemoveComponent(GameObject gameObject)
        {
            if (Children.Remove(gameObject))
            {
                // будет установлено только если этот объект действительно был в списке
                gameObject.Parent = null;
            }
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
            var result = Children?.FirstOrDefault(b => (b is T )) as T;
            //var result = Children?.FirstOrDefault(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))) ) as T;
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
                .Where(b => b is T)
                //.Where(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))))
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
            var result = Children?.FirstOrDefault(b => b is T );
            //var result = Children?.FirstOrDefault(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))) );
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
                .Where(b => b is T)
                //.Where(b => (b.GetType() == typeof(T)) || (b.GetType().IsSubclassOf(typeof(T))))
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
            }
            if (OnStart != null) OnStart(this);
        }


        /// <summary> Обновить объект. 
        /// Тут обновляюеся положение, или производится отрисовка, или т.п.
        /// </summary>
        public virtual void Update()
        {
            // Теоретически, обработку клавиатурных событий мы могли бы разместить и здесь.
            // Но лучше перехватим это одним объектом на самом верхнем уровне иерархии,
            // чтоб не тормозить работу лишними вызовами.
            //GetAndProcessKeyboardEvents();

            // Обновляем дочерние объекты
            foreach (GameObject c in Children.ToArray())
            {
                c.Update();
            }
            if (OnUpdate != null) OnUpdate(this);
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


        /// <summary> Основная используемая функция - получение и обработка клавиатурных событий
        ///  </summary>
        public virtual void GetAndProcessKeyboardEvents()
        {
            if (AppType == ApplicationType.ConsoleApp)
            {
                var gameKeyEventArgs = GetKeyAvailable();
                if (gameKeyEventArgs!=null)
                    ProcessKey(gameKeyEventArgs);
            }
            else if (AppType == ApplicationType.WinFormsApp)
            {
                // Для получения и обработки клавиатурных событий просто вызываем обработку событий окна.
                // При этом для полученных клавиатурных событий будут вызваны наши обработчики - DoOnKeyPress и DoOnKeyDown
                // А уже в этих обработчиках будет вызвана функция ProcessKey
                Application.DoEvents();
            }
            else if (AppType == ApplicationType.WpfApp)
            {
                // тут пока не знаю как 
            }
        }

        /// <summary> Основная перекрываемая функция обработки клавиатуры объектами
        ///  </summary>
        /// <param name="e"></param>
        protected virtual void ProcessKey(GameKeyEventArgs e)
        {
            // тут может быть обработка события самим объектом

            // передача события дочерним объектам, если сам объект его не обработал
            // ! Чтоб этот механизм отработал, не забывает при перекрытии метода
            // ! после обработки нажатия вызывать base.ProcessKey(e)
            if (e.Handled) return;
            foreach (var child in Children.ToArray())
            {
                child.ProcessKey(e);
                if (e.Handled) return;
            }
        }

        /// <summary> Сделаем свой формат для описания клавиатурных событий,
        /// поскольку в разных платформах для этого используются разные наборы типов.
        /// Мы сделаем такую структуру, которую можно будет преобразовать в то, что потребуется.
        /// </summary>
        public class GameKeyEventArgs : EventArgs
        {
            /// <summary> Если в обработчике устанавливается  Handled - то дальнейшая обработка этого нажатия уже не производится.
            /// </summary>
            public bool Handled { get; set; } = false;

            // Остальное можно и не делать - сделать потом, у производных классов.

            //public System.Windows.Forms.Keys KeyCode { get; set; }
            public int KeyCode { get; set; }
            public Char KeyChar { get; set; }
            public int Modifiers { get; set; }
        }

        #region Console

        /// <summary> Опрос консоли и получение нажатия клавиши (если есть) в формате GameKeyEventArgs 
        /// </summary>
        /// <returns></returns>
        protected virtual GameKeyEventArgs GetKeyAvailable()
        {
            if (AppType == ApplicationType.ConsoleApp)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    var eventArgs = new GameKeyEventArgs();
                    eventArgs.KeyChar = key.KeyChar;
                    eventArgs.KeyCode = (int) key.Key;
                    eventArgs.Modifiers = (int) key.Modifiers;

                    // Тут можно было бы сделать обработку:
                    //ProcessKey(eventArgs);
                    // Но мы сделаем это явным образом
                    return eventArgs;
                }
                else
                    // если не было нажатия клавиш
                    return null;
            }
            else
                // для всех остальных типов приложений
                return null;
        }

        #endregion

        #region WinForms

        protected virtual void DoOnKeyPress(object sender, KeyPressEventArgs e)
        {
            var eventArgs = new GameKeyEventArgs();
            eventArgs.KeyChar = e.KeyChar;
            //eventArgs.KeyCode = (int) e.Key;
            //eventArgs.Modifiers = (int) e.Modifiers;

            // Тут мы вызовем обработку сразу - т.к. мы не можем явно потребовать у окна клавиатурного ввода,
            // событие ввода инициируется окном.
            // Т.е. мы не знаем, когда именно оно случится.
            // Поэтому просто сразу прикручиваем к нему наш обработчик.
            ProcessKey(eventArgs);
            // Если событие было обработано у нас - укажем это, чтоб ононный обработчик это знал,
            // и больше его не передавал куда-то еще
            e.Handled = eventArgs.Handled;
        }

        protected virtual void DoOnKeyDown(object sender, KeyEventArgs e)
        {
            var eventArgs = new GameKeyEventArgs();
            //eventArgs.KeyChar = e. KeyChar;
            eventArgs.KeyCode = (int) e.KeyCode;
            eventArgs.Modifiers = (int) e.Modifiers;

            ProcessKey(eventArgs);
            e.Handled = eventArgs.Handled;

            // Тут мы вызовем обработку сразу - т.к. мы не можем явно потребовать у окна клавиатурного ввода,
            // событие ввода инициируется окном.
            // Т.е. мы не знаем, когда именно оно случится.
            // Поэтому просто сразу прикручиваем к нему наш обработчик.
            ProcessKey(eventArgs);
            // Если событие было обработано у нас - укажем это, чтоб ононный обработчик это знал,
            // и больше его не передавал куда-то еще
            e.Handled = eventArgs.Handled;
        }

        #endregion
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

/*
        /// <summary> Коды клавиш - из System
        /// 
        /// </summary>
        [Serializable]
        public enum ConsoleKey
        {
            Backspace = 8,
            Tab = 9,
            Clear = 12, // 0x0000000C
            Enter = 13, // 0x0000000D
            Pause = 19, // 0x00000013
            Escape = 27, // 0x0000001B
            Spacebar = 32, // 0x00000020
            PageUp = 33, // 0x00000021
            PageDown = 34, // 0x00000022
            End = 35, // 0x00000023
            Home = 36, // 0x00000024
            LeftArrow = 37, // 0x00000025
            UpArrow = 38, // 0x00000026
            RightArrow = 39, // 0x00000027
            DownArrow = 40, // 0x00000028
            Select = 41, // 0x00000029
            Print = 42, // 0x0000002A
            Execute = 43, // 0x0000002B
            PrintScreen = 44, // 0x0000002C
            Insert = 45, // 0x0000002D
            Delete = 46, // 0x0000002E
            Help = 47, // 0x0000002F
            D0 = 48, // 0x00000030
            D1 = 49, // 0x00000031
            D2 = 50, // 0x00000032
            D3 = 51, // 0x00000033
            D4 = 52, // 0x00000034
            D5 = 53, // 0x00000035
            D6 = 54, // 0x00000036
            D7 = 55, // 0x00000037
            D8 = 56, // 0x00000038
            D9 = 57, // 0x00000039
            A = 65, // 0x00000041
            B = 66, // 0x00000042
            C = 67, // 0x00000043
            D = 68, // 0x00000044
            E = 69, // 0x00000045
            F = 70, // 0x00000046
            G = 71, // 0x00000047
            H = 72, // 0x00000048
            I = 73, // 0x00000049
            J = 74, // 0x0000004A
            K = 75, // 0x0000004B
            L = 76, // 0x0000004C
            M = 77, // 0x0000004D
            N = 78, // 0x0000004E
            O = 79, // 0x0000004F
            P = 80, // 0x00000050
            Q = 81, // 0x00000051
            R = 82, // 0x00000052
            S = 83, // 0x00000053
            T = 84, // 0x00000054
            U = 85, // 0x00000055
            V = 86, // 0x00000056
            W = 87, // 0x00000057
            X = 88, // 0x00000058
            Y = 89, // 0x00000059
            Z = 90, // 0x0000005A
            LeftWindows = 91, // 0x0000005B
            RightWindows = 92, // 0x0000005C
            Applications = 93, // 0x0000005D
            Sleep = 95, // 0x0000005F
            NumPad0 = 96, // 0x00000060
            NumPad1 = 97, // 0x00000061
            NumPad2 = 98, // 0x00000062
            NumPad3 = 99, // 0x00000063
            NumPad4 = 100, // 0x00000064
            NumPad5 = 101, // 0x00000065
            NumPad6 = 102, // 0x00000066
            NumPad7 = 103, // 0x00000067
            NumPad8 = 104, // 0x00000068
            NumPad9 = 105, // 0x00000069
            Multiply = 106, // 0x0000006A
            Add = 107, // 0x0000006B
            Separator = 108, // 0x0000006C
            Subtract = 109, // 0x0000006D
            Decimal = 110, // 0x0000006E
            Divide = 111, // 0x0000006F
            F1 = 112, // 0x00000070
            F2 = 113, // 0x00000071
            F3 = 114, // 0x00000072
            F4 = 115, // 0x00000073
            F5 = 116, // 0x00000074
            F6 = 117, // 0x00000075
            F7 = 118, // 0x00000076
            F8 = 119, // 0x00000077
            F9 = 120, // 0x00000078
            F10 = 121, // 0x00000079
            F11 = 122, // 0x0000007A
            F12 = 123, // 0x0000007B
            F13 = 124, // 0x0000007C
            F14 = 125, // 0x0000007D
            F15 = 126, // 0x0000007E
            F16 = 127, // 0x0000007F
            F17 = 128, // 0x00000080
            F18 = 129, // 0x00000081
            F19 = 130, // 0x00000082
            F20 = 131, // 0x00000083
            F21 = 132, // 0x00000084
            F22 = 133, // 0x00000085
            F23 = 134, // 0x00000086
            F24 = 135, // 0x00000087
            BrowserBack = 166, // 0x000000A6
            BrowserForward = 167, // 0x000000A7
            BrowserRefresh = 168, // 0x000000A8
            BrowserStop = 169, // 0x000000A9
            BrowserSearch = 170, // 0x000000AA
            BrowserFavorites = 171, // 0x000000AB
            BrowserHome = 172, // 0x000000AC
            VolumeMute = 173, // 0x000000AD
            VolumeDown = 174, // 0x000000AE
            VolumeUp = 175, // 0x000000AF
            MediaNext = 176, // 0x000000B0
            MediaPrevious = 177, // 0x000000B1
            MediaStop = 178, // 0x000000B2
            MediaPlay = 179, // 0x000000B3
            LaunchMail = 180, // 0x000000B4
            LaunchMediaSelect = 181, // 0x000000B5
            LaunchApp1 = 182, // 0x000000B6
            LaunchApp2 = 183, // 0x000000B7
            Oem1 = 186, // 0x000000BA
            OemPlus = 187, // 0x000000BB
            OemComma = 188, // 0x000000BC
            OemMinus = 189, // 0x000000BD
            OemPeriod = 190, // 0x000000BE
            Oem2 = 191, // 0x000000BF
            Oem3 = 192, // 0x000000C0
            Oem4 = 219, // 0x000000DB
            Oem5 = 220, // 0x000000DC
            Oem6 = 221, // 0x000000DD
            Oem7 = 222, // 0x000000DE
            Oem8 = 223, // 0x000000DF
            Oem102 = 226, // 0x000000E2
            Process = 229, // 0x000000E5
            Packet = 231, // 0x000000E7
            Attention = 246, // 0x000000F6
            CrSel = 247, // 0x000000F7
            ExSel = 248, // 0x000000F8
            EraseEndOfFile = 249, // 0x000000F9
            Play = 250, // 0x000000FA
            Zoom = 251, // 0x000000FB
            NoName = 252, // 0x000000FC
            Pa1 = 253, // 0x000000FD
            OemClear = 254, // 0x000000FE
        }
*/

    }
}