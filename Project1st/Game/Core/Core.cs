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
        int[] axisX = { 1, -1, 0, 0 };
        int[] axisY = { 0, 0, -1, 1 };

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
            GameManger.buffer.printTimer = new Timer(PrintMap, line, 100, 100);


            GameManger.currField.isFog = false;
            GameManger.currField.isCurrField = true;

            bool isMove = false;
            bool isStun = false;

            bool isAttack = false;
            bool isYes = false;
            bool isNo = false;
            bool isFirst = false;
            
            GameManger.player.RemoveFog();
            if (GameManger.currField.type == 2)
            {
                GameManger.currField.ReturnSelfToTown().Enter();
            }

            while (true)
            {
                Console.CursorVisible = false;
                isMove = false;
                isAttack = false;
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


                if (GameManger.currField.type == 1)
                {                    

                    //패배
                    if (!GameManger.player.isLive)
                    {
                        GameManger.buffer.printTimer.Dispose();
                        return;
                    }

                    //이동
                    if (isMove)
                    {
                        isStun = GameManger.currField.Move(currFieldPos);
                    }

                    //메뉴창
                    if (isNo)
                    {
                    }

                    //공격
                    if (isYes)
                    {
                    }
                }

                else if (GameManger.currField.type == 2)
                {
                    Town currTown = GameManger.currField.ReturnSelfToTown();
                    //커서 이동
                    if (isMove)
                    {
                        currTown.Move();

                    }

                    //확인
                    if (isYes)
                    {
                        if (currTown.mainPosition == 0)
                        {
                            currTown.mainPosition = (currTown.cursorPosition.y + 1);
                            currTown.cursorPosition.x = 0;
                            currTown.cursorPosition.y = 0;

                            currTown.startShopIndex = 0;
                            GameManger.player.startInventoryIndex = 0;

                            if (!isFirst)
                            {
                                isFirst = true;
                            }
                        }
                        else if (currTown.mainPosition == 1)
                        {
                            if (isFirst)
                            {
                            }

                            //구매
                            if (currTown.cursorPosition.x == 0)
                            {
                                Items item = currTown.shop[(currTown.cursorPosition.y + currTown.startShopIndex)];

                                if (item.count == 0)
                                {
                                }
                                else if (GameManger.player.gold < (int)(item.price * currTown.priceRate[item.itemId].rate))
                                {
                                }
                                else
                                {
                                    currTown.shop[(currTown.cursorPosition.y + currTown.startShopIndex)].count -= 1;
                                    currTown.gold += (int)(item.price * currTown.priceRate[item.itemId].rate);
                                    GameManger.player.gold -= (int)(item.price * currTown.priceRate[item.itemId].rate);
                                    int itemIndex = GameManger.player.inventory.FindIndex(x => x.itemId == item.itemId);
                                    if (itemIndex == -1)
                                    {
                                        GameManger.player.inventory.Add(new Items(item));
                                    }
                                    else
                                    {
                                        GameManger.player.inventory[itemIndex].count += 1;
                                    }
                                }
                            }
                            //판매
                            else if (currTown.cursorPosition.x == 1)
                            {                             

                                Items item = GameManger.player.inventory[(currTown.cursorPosition.y + GameManger.player.startInventoryIndex)];
                               
                                if (item.count == 0)
                                {
                                }
                                else if (currTown.gold < (int)(item.price * currTown.priceRate[item.itemId].rate * 0.7))
                                {
                                }
                                else
                                {
                                    GameManger.player.inventory[(currTown.cursorPosition.y + GameManger.player.startInventoryIndex)].count -= 1;
                                    currTown.gold -= (int)(item.price * currTown.priceRate[item.itemId].rate * 0.7);
                                    GameManger.player.gold += (int)(item.price * currTown.priceRate[item.itemId].rate * 0.7);

                                    int itemIndex = currTown.shop.FindIndex(x => x.itemId == item.itemId);
                                    if (itemIndex == -1)
                                    {
                                        currTown.shop.Add(new Items(item));
                                    }
                                    else
                                    {
                                        currTown.shop[itemIndex].count += 1;
                                    }
                                }
                            }
                        }
                        else if (currTown.mainPosition == 4)
                        {
                            if (GameManger.currField.portals[currTown.cursorPosition.y] != null)
                            {

                                currFieldPos.y += axisY[currTown.cursorPosition.y];
                                currFieldPos.x += axisX[currTown.cursorPosition.y];

                                GameManger.currField = GameManger.map.worldMap[currFieldPos.y, currFieldPos.x];

                                if (currTown.cursorPosition.y == 0)
                                {
                                    GameManger.player.Axis2D.x = GameManger.currField.portals[1].axis.x;
                                    GameManger.player.Axis2D.y = GameManger.currField.portals[1].axis.y;
                                }
                                else if (currTown.cursorPosition.y == 1)
                                {
                                    GameManger.player.Axis2D.x = GameManger.currField.portals[0].axis.x;
                                    GameManger.player.Axis2D.y = GameManger.currField.portals[0].axis.y;
                                }
                                else if (currTown.cursorPosition.y == 2)
                                {
                                    GameManger.player.Axis2D.x = GameManger.currField.portals[3].axis.x;
                                    GameManger.player.Axis2D.y = GameManger.currField.portals[3].axis.y;
                                }
                                else if (currTown.cursorPosition.y == 3)
                                {
                                    GameManger.player.Axis2D.x = GameManger.currField.portals[2].axis.x;
                                    GameManger.player.Axis2D.y = GameManger.currField.portals[2].axis.y;
                                }

                                if (GameManger.currField.type == 1)
                                {
                                    GameManger.currField.PlayEnemies();
                                    GameManger.currField.SetCreateTimer(new Timer(GameManger.currField.CreateEnemy, null, 100, 10000));
                                }
                                GameManger.currField.isFog = false;
                                GameManger.currField.isCurrField = true;


                                GameManger.player.RemoveFog();

                            }
                        }
                    }

                    //취소
                    if (isNo)
                    {
                        if (currTown.mainPosition != 0)
                        {
                            currTown.mainPosition = 0;
                            currTown.cursorPosition.x = 0;
                            currTown.cursorPosition.y = 0;
                        }
                    }

                    //커서 방향 초기화
                    GameManger.player.direction = 5;
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
