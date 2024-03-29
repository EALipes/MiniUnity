using System;

namespace MiniUnity.CannonGame
{
    public class CannonScene : Scene
    {
        protected Cannon Cannon;
        protected Projectile Projectile;
        protected CannonGame Game
        {
            get { return (this.Parent as CannonGame); }
        }

        public CannonScene()
        {
        }


        public override void Start()
        {
            base.Start();

            Cannon = GetComponent<Cannon>() as Cannon;
            if (Cannon==null) throw new NullReferenceException("Не найден объект Cannon");
            Projectile = GetComponent<Projectile>() as Projectile; 

            Cannon.Load(Projectile, Game.Angle, Game.Velocity);
            Cannon.Fire();
        }

        public override void Update()
        {
            // Сделаем обработку ввода с клавиатуры
            // По идее, это неправильно. Для этого должен бы быть какой-то специальный метод, в специальном месте, 
            // но я еще не придумал куда вставить управление игрой...
            CheckKeyboardCommands();

            // Если время остановлено - нечего тут обновлять, выходим
            if (Game.Orchestrator.Stopped) 
                return;

            //Console.WriteLine(DateTime.Now.Minute+":"+DateTime.Now.Second+"."+DateTime.Now.Millisecond);
            base.Update();

            // Если ядро упало - игра окончена.
            if (Projectile.Fallen)
                IsOver = true;
        }

        private void CheckKeyboardCommands()
        {
            // TODO: Это сделать изменяемым в зависимости от типа приложения!
            /*
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                if (key.KeyChar == ' ')
                    // пуск-стоп
                {
                    //IsStopped = !IsStopped;
                    if (!Game.Orchestrator.Stopped) 
                        Game.Orchestrator.Stop();
                    else
                        Game.Orchestrator.Resume();
                }

                if (
                    key.Key == ConsoleKey.Escape
                    ||
                    (key.KeyChar == 'x')||(key.KeyChar == 'X')||(key.KeyChar == 'ч')||(key.KeyChar == 'Ч')
                    )

                {
                    // Esc - выход из игры
                    // Пока просто указываем флаг завершения сцены
                    IsOver = true;
                }
            }
            */
        }
    }
}