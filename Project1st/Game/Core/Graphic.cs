using Project1st.Game.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Core
{
    public class BufferPrinter
    {
        private string[] buffer;
        public readonly int _BUFFER_SIZE;
        public Timer printTimer;

        public bool isWork { private set; get; }

        private enum Color
        {
            WHITE = 0, RED, GREEN, BLUE,
            YELLOW = 4, CYAN, DARK_GRAY, DARK_RED,
            DARK_MAGENTA = 8, DARK_BLUE, DARK_CYAN, DARK_GREEN, 
            DARK_YELLOW = 12, BLACK=99
        }

        public BufferPrinter()
        {
            isWork = false;
            _BUFFER_SIZE = 45;
            buffer = new string[_BUFFER_SIZE];
        }

        public void SetBuffer(string[] map)
        {
            buffer = map;
        }

        public void PrintBuffer()
        {
            if (isWork) { return; }
            if (buffer == null) { return; }

            isWork = true;

            for (int y = 0; y < buffer.Length; y++)
            {
                if (buffer[y] == null) { break; }

                Console.SetCursorPosition(0, y);
                string[] splitString = buffer[y].Split('.');

                for (int i = 0; i < splitString.Length; i++)
                {

                    if (i % 3 == 0)
                    {
                        Console.Write(splitString[i]);
                    }
                    else if (i % 3 == 1)
                    {
                        Color color = (Color)int.Parse(splitString[i]);
                        switch (color)
                        {
                            //0
                            case Color.WHITE:
                                Console.ForegroundColor = ConsoleColor.White;
                                break;
                            //1
                            case Color.RED:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            //2
                            case Color.GREEN:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            //3
                            case Color.BLUE:
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            //4
                            case Color.YELLOW:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            //5
                            case Color.CYAN:
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                            //6
                            case Color.DARK_GRAY:
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                break;
                            //7
                            case Color.DARK_RED:
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                break;
                            //8
                            case Color.DARK_MAGENTA:
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                break;
                            //9
                            case Color.DARK_BLUE:
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                break;
                            //10
                            case Color.DARK_CYAN:
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                break;
                            //11
                            case Color.DARK_GREEN:
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                break;
                            //12
                            case Color.DARK_YELLOW:
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                break;
                            //99?
                            case Color.BLACK:
                                Console.ForegroundColor = ConsoleColor.Black;
                                break;
                            default:
                                break;
                        }   //[switch] end 컬러체크
                    }
                    else if (i % 3 == 2)
                    {
                        Console.Write(splitString[i]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }


                }// 1행 출력 종료

            }//모든 행 출력 종료

            Console.SetCursorPosition(50, 40);
            Console.WriteLine(WorldMap.testPos.x+", "+ WorldMap.testPos.y);

            isWork = false;
        }

    }
}
