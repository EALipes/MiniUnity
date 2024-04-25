using System;

namespace MiniUnity.CannonGame
{
    public class CannonScene : Scene
    {
        public Cannon Cannon { get; set; }

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
            var projectile = new Projectile();
            AddComponent(projectile);
            projectile.Start(); // Иначе он не находит параметров игры
            
            Cannon.Fire(projectile, Cannon.ElevationAngle, Cannon.Velocity);
            //Cannon.Fire(projectile, Game.Angle, Game.Velocity);
        }


        protected override void ProcessKey(GameKeyEventArgs e)
        {
            // Пробел - пуск/останов времени в игре
            if (e.KeyChar == ' ')
            {
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

        
    }
}