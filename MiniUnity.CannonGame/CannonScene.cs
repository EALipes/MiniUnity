using System;

namespace MiniUnity.CannonGame
{
    public class CannonScene : Scene
    {
        public Cannon Cannon { get; set; }
        protected Projectile Projectile;
        protected CannonGame Game
        {
            get { return (this.Parent as CannonGame); }
        }

        public CannonScene()
        {
            Cannon = new Cannon();
            AddComponent(Cannon);
        }


        public override void Start()
        {
            base.Start();

            Cannon = GetComponent<Cannon>() as Cannon;
            if (Cannon==null) throw new NullReferenceException("Не найден объект Cannon");
            
            CannonFire();
        }

        private void CannonFire()
        {
            //Projectile = GetComponent<Projectile>() as Projectile; 
            Projectile = new Projectile();
            AddComponent(Projectile);
            Projectile.Start(); // Иначе он не находит параметров игры
            // TODO: Надо бы как-то уменьшить зависимости между Projectile и Game

            Cannon.Fire(Projectile, Game.Angle, Game.Velocity);
        }

        public override void Update()
        {
            // Сделаем обработку ввода с клавиатуры
            // По идее, это неправильно. Для этого должен бы быть какой-то специальный метод, в специальном месте, 
            // но я еще не придумал куда вставить управление игрой...
            // TODO: Перенести обработку клавиатурных команд в GameObject.Update() или в Scene..Update(), а тут только перекрыть их обработку
            //
            //CheckKeyboardCommands();
            GetAndProcessKeyboardEvents();

            // Если время остановлено - нечего тут обновлять, выходим
            // TODO: Надо ли это тут? Может, перенести в Scene?
            // TODO: Может, это вообще не нужно???
            if (Game.Orchestrator.Stopped) 
                return;

            //Console.WriteLine(DateTime.Now.Minute+":"+DateTime.Now.Second+"."+DateTime.Now.Millisecond);
            base.Update();

            // ! Перенесено в Projectile
            // Если ядро упало - игра окончена.
            //if (Projectile.Fallen)
            //    IsOver = true;
        }

        protected override void ProcessKey(GameKeyEventArgs e)
        {
            // Пробел - пуск/останов времени в игре
            if (
                (e.KeyChar == ' ')
                ||
                (e.KeyChar == '\0')
                )
                // пуск-стоп
            {
                //IsStopped = !IsStopped;
                if (!Game.Orchestrator.Stopped) 
                    Game.Orchestrator.Stop();
                else
                    Game.Orchestrator.Resume();
            }

            // Escape или X(eXit) в любом регистре и языке - конец игры.
            else if (
                //e.KeyCode == 27
                e.KeyCode == (int) ConsoleKey.Escape
                ||
                (e.KeyChar == 'x')||(e.KeyChar == 'X')||(e.KeyChar == 'ч')||(e.KeyChar == 'Ч')
            )

            {
                // Esc - выход из игры
                // Пока просто указываем флаг завершения сцены
                IsOver = true;
            }

            // F - выстрел
            else if (
                (e.KeyChar == 'F')
                ||
                (e.KeyChar == 'f')
                ||
                (e.KeyChar == 'А')
                ||
                (e.KeyChar == 'а')
            )
            {
                CannonFire();
            }

            // Вверх - увеличить угол на 5 градусов
            //else if (e.KeyCode == (int)System.Windows.Forms.Keys.Up) //38
            else  if (e.KeyCode == (int)ConsoleKey.UpArrow) //38
            {
                Cannon.ElevationAngle+=5;
            }

            // Вниз - уменьшить угол на 5 градусов
            //else if (e.KeyCode == (int)System.Windows.Forms.Keys.Down)
            else if (e.KeyCode == (int)ConsoleKey.DownArrow) //40
            {
                Cannon.ElevationAngle-=5;
            }

            // Вправо - увеличить скорость на 1 м/с
            //else if (e.KeyCode == (int)System.Windows.Forms.Keys.Right)
            else if (e.KeyCode == (int)ConsoleKey.RightArrow) //39
            {
                Cannon.Velocity+=1;
            }

            // Влево - уменьшить скорость на 1м/с
            //else if (e.KeyCode == (int)System.Windows.Forms.Keys.Left)
            else if (e.KeyCode == (int)ConsoleKey.LeftArrow) //37
            {
                Cannon.Velocity-=1;
            }

            base.ProcessKey(e);
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