using Project1st.Game.GameObject;
using Project1st.Game.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Core
{
    public class Core
    {
        string[] line;
        Coordinate currFieldPos;

        public Core()
        {

            GameManger.Instance();

            line = new string[GameManger.buffer._BUFFER_SIZE];

            GameManger.player.Axis2D.x = 1;
            GameManger.player.Axis2D.y = 1;
            currFieldPos = new Coordinate(1, 1);

            GameManger.currField = GameManger.map.worldMap[currFieldPos.y, currFieldPos.x];
        }

        public void Start()
        {

            //그리기 타이머
            Timer timerField = new Timer(PrintMap, line, 100, 100);


            GameManger.currField.isFog = false;
            GameManger.currField.isCurrField = true;

            bool isMove = false;
            bool isAttack = false;
            bool isStun = false;


            Console.CursorVisible = false;
            while (true)
            {
                Console.CursorVisible = false;
                isMove = false;
                isAttack = false;

                if (!isStun)
                {

                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.RightArrow:
                            isMove = true;
                            GameManger.player.direction = 0;
                            break;
                        case ConsoleKey.LeftArrow:
                            isMove = true;
                            GameManger.player.direction = 1;
                            break;
                        case ConsoleKey.UpArrow:
                            isMove = true;
                            GameManger.player.direction = 2;
                            break;
                        case ConsoleKey.DownArrow:
                            isMove = true;
                            GameManger.player.direction = 3;
                            break;
                        case ConsoleKey.Z:
                            isAttack = true;
                            break;
                        case ConsoleKey.X:
                        default:
                            GameManger.player.direction = 5;
                            break;
                    }

                    Thread.Sleep(1);
                    while (Console.KeyAvailable) Console.ReadKey(true);

                }
                else
                {
                    Thread.Sleep(1000);
                    while (Console.KeyAvailable) Console.ReadKey(true);

                    isStun = false;
                    GameManger.player.direction = 5;
                }

                //패배
                if (!GameManger.player.isLive)
                {
                    timerField.Dispose();
                    return;
                }

                //이동
                if (isMove)
                {
                    GameManger.player.Move(currFieldPos, ref isStun);
                }
            }
        }        

        public void PrintMap(Object obj)
        {
            GameManger.currField.ConvertMapToString(ref line);
            if (!GameManger.buffer.isWork)
            {
                GameManger.buffer.SetBuffer((string[])obj);
                GameManger.buffer.PrintBuffer();
            }
            else
            {
                Console.Clear();
            }
        }
    }
}
