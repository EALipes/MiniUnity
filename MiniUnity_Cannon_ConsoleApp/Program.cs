using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MiniUnity;
using MiniUnity.CannonGame;
//using MiniUnity_CannonGame;

namespace MiniUnity_Cannon_ConsoleApp
{
    /// <summary>
    /// Программа игры "Пушка".
    /// Моделирует стрельбу из пушки и полет выпущенного снаряда.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var game = new CannonGame();
            var gameParams = new GameParameters();

            // Задаем параметры игры через GameParameters
            gameParams.Speed = 100;
            gameParams.Angle = 45;

            while (true)
            {
                ShowGameSettings(gameParams);
                gameParams.SetGameSettings(game);
                game.Play();
                Console.WriteLine();

                var repeat = PromptToRepeat("Сыграем еще? Y/N");
                if (!repeat)
                    return;
            }
        }

        private static bool PromptToRepeat(string prompt)
        {
            Console.WriteLine(prompt);
            //if (Console.KeyAvailable)
            var s = Console.ReadKey();
            var repeat = (s.KeyChar == 'y') | (s.KeyChar == 'Y') | (s.KeyChar == 'Д') | (s.KeyChar == 'д') |
                         (s.Key == ConsoleKey.Enter);
            if (repeat) return true;
            return false;
        }

        /// <summary>
        /// Служебная функция - читаем число с экрана.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public static float ReadValue(string prompt, float? defaultValue = null, string format="F2")
        {
            Console.Write(prompt);
            if (defaultValue.HasValue)
                Console.Write(" ["+defaultValue.Value.ToString(format)+"]");
            Console.WriteLine();
            float result;

            while (true)
            {
                var s = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(s) && defaultValue.HasValue)
                    return defaultValue.Value;
                if (float.TryParse(s, out result)) return result;
                Console.Beep();
            }
        }

        /// <summary>
        /// Вывод параметров игры
        /// </summary>
        /// <param name="gameParams"> </param>
        public static void ShowGameSettings(GameParameters gameParams)
        {

            Console.WriteLine("Параметры игры:");
            Console.WriteLine("---------------");

            Console.WriteLine("Скорость снаряда, метров в секунду: "+gameParams.Speed);
            Console.WriteLine("Угол возвышения, градусов: "+gameParams.Angle);
            Console.WriteLine("Частота обновления, кадров в секунду: "+gameParams.FramesPerSec);
            Console.WriteLine();
            
            Console.WriteLine("Нажмите S, если хотите изменить их.");
            var s = Console.ReadKey(true);
            var changeSettings = (s.KeyChar == 's') | (s.KeyChar == 'S');
            if (changeSettings)
            {
                EditGameSettings(gameParams);
                ShowGameSettings(gameParams);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private static void EditGameSettings(GameParameters gameParams)
        {
            var velocity = ReadValue("Скорость снаряда, метров в секунду:", gameParams.Speed);
            gameParams.Speed = velocity;
            var angle = ReadValue("Угол возвышения, градусов:", gameParams.Angle);
            gameParams.Angle = angle;
            var fps = ReadValue("Кадров в секунду:", gameParams.FramesPerSec);
            gameParams.FramesPerSec = (int)fps;
        }


        //private static void CheckKeyboardCommands()
/*
        private static void CheckKeyboardCommands()
        {
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
        }
    }
*/
    }


}




