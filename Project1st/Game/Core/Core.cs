using Project1st.Game.GameObject;
using Project1st.Game.Item;
using Project1st.Game.Map;
using Project1st.Game.Map.Fields;
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

        public Core()
        {
            GameManger.Instance();

            Console.WindowWidth = 200;
            Console.WindowHeight = GameManger.buffer._BUFFER_SIZE;

            line = new string[GameManger.buffer._BUFFER_SIZE];

            GameManger.player.Axis2D.x = 1;
            GameManger.player.Axis2D.y = 1;

            //테스트 코드
            WorldMap.testPos = GameManger.currFieldPos;

            GameManger.currField = GameManger.worldMap.map[GameManger.currFieldPos.y, GameManger.currFieldPos.x];
        }

        public void Start()
        {

            //그리기 타이머
            GameManger.buffer.printTimer = new Timer(SetPrintMap, line, 50, 200);


            GameManger.currField.isFog = false;
            GameManger.currField.isCurrField = true;

            bool isBattleWin = false;
            bool isQuit = false;

            bool isMove = false;
            bool isStun = false;


            bool isYes = false;
            bool isNo = false;

            if (GameManger.currField.type == 2)
            {
                GameManger.currField.InitEnter();
            }
            else if (GameManger.currField.type == 1) 
            {
                GameManger.player.RemoveFog();
            }


            Console.CursorVisible = false;
            while (true)
            {
                isMove = false;
                
                isYes = false;
                isNo = false;

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
                            isYes = true;
                            break;
                        case ConsoleKey.X:
                            isNo = true;
                            break;
                        case ConsoleKey.Q:
                            isQuit = true;
                            break;
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



                #region 이벤트 처리

                //종료
                if (isQuit)
                {
                    GameManger.buffer.printTimer.Dispose();
                    return;
                }

                //패배
                if (!GameManger.player.isLive)
                {
                    GameManger.buffer.printTimer.Dispose();
                    return;
                }

                //적 죽일시 and 승리시
                if (GameManger.currField.isWin || isBattleWin)
                {
                    if (GameManger.currField.type == 3)
                    {
                        GameManger.currField.ReturnSelfToBattle().beforePlayerInfo.hitPoint = GameManger.player.hitPoint;
                        GameManger.currField.StopEnemies();

                        //Utility.currRoom.enemyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                        GameManger.player = GameManger.currField.ReturnSelfToBattle().beforePlayerInfo;
                        GameManger.currField = GameManger.currField.ReturnSelfToBattle().beforeFieldInfo;

                        GameManger.currField.InitEnter();
                        GameManger.player.gold += 700;
                    }

                    isBattleWin = false;

                    GameManger.player.gold += 300;
                    continue;
                }

                //엔딩 씬
                else if (GameManger.currField.type == 4)
                {
                    continue;
                }

                #endregion


                #region 입력 처리

                if (isMove)
                {
                    if ((GameManger.currField.type == 1 || GameManger.currField.type == 3))
                    {
                        for (int y = 0; y < FieldBase._FIELD_SIZE; y++)
                        {
                            for (int x = 0; x < FieldBase._FIELD_SIZE; x++)
                            {
                                if (GameManger.currField.GetFogInfo(x, y) == 1)
                                {
                                    GameManger.currField.SetFogInfo(x, y, 0.5f);
                                }

                            }
                        }
                    }

                    if (GameManger.currField.type == 1)
                    {
                        if (!GameManger.currField.isMenu)
                        {
                            isStun = GameManger.currField.PressMoveEvent();
                            if (GameManger.player.walk > 0)
                            {
                                GameManger.player.walk -= 1;
                                if (GameManger.player.walk < 0)
                                {
                                    GameManger.player.light -= 3;
                                }
                            }
                        }
                        else
                        {
                            GameManger.currField.PressMenuEvent();
                        }
                    }

                    else if (GameManger.currField.type == 2)
                    {
                        GameManger.currField.PressMenuEvent();
                        GameManger.player.direction = 5;
                    }
                    else if (GameManger.currField.type == 3)
                    {
                        isStun = GameManger.currField.PressMoveEvent();
                    }

                    if ((GameManger.currField.type == 1 || GameManger.currField.type == 3))
                    {
                        GameManger.player.RemoveFog();
                    }

                }
                if (isYes)
                {                    
                    isBattleWin = GameManger.currField.PressYesEvent();
                    

                    if (GameManger.currField.type == 2)
                    {
                        GameManger.player.direction = 5;
                    }
                }
                if (isNo)
                {
                    GameManger.currField.PressNoEvent();

                    if (GameManger.currField.type == 2)
                    {
                        GameManger.player.direction = 5;
                    }
                }
                #endregion
            }
        }

        public void SetPrintMap(Object obj)
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
