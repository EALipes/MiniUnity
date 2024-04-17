//using System.Windows.Forms;

using System.Windows.Forms;

namespace MiniUnity
{
    public class Scene: GameObject
    {

        protected Game Game
        {
            get { return (this.Parent as Game); }
        }


        public override void Start()
        {
            IsOver = false;
            // Установить начальные параметры всех объектов сцены
            base.Start();
        }



        /// <summary> Обновить объект. 
        /// Тут обновлятеся положение, или производится отрисовка, или т.п.
        /// </summary>
        public override void Update()
        {
            // Сделаем обработку ввода с клавиатуры
            GetAndProcessKeyboardEvents();

            // Если время остановлено - нечего тут обновлять, выходим
            if (Game.Orchestrator.Stopped) 
                return;

            //Console.WriteLine(DateTime.Now.Minute+":"+DateTime.Now.Second+"."+DateTime.Now.Millisecond);
            // Обновление входящих объектов
            base.Update();
        }

        public bool IsOver { get; set; }


        #region Отрисовка

        #region Winforms

        #endregion
        #endregion
    }
}