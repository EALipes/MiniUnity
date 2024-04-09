//using System.Windows.Forms;

namespace MiniUnity
{
    public class Scene: GameObject
    {
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
            //foreach (var child in Children)
            //{
            //    child.Update();
            //}
            
            base.Update();
        }

        public bool IsOver { get; set; }


        #region Отрисовка

        #region Winforms


        #endregion


        //public override void RefreshDraw()
        //{
        //    if (AppType == ApplicationType.ConsoleApp)
        //    {
        //    }
        //    else if (AppType == ApplicationType.WinFormsApp)
        //    {
        //        if (GamePanel != null)
        //            GamePanel.Refresh();
        //    }
        //}

        #endregion
    }
}