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

        int currFieldX;
        int currFieldY;

        int[] axisX = { 1, -1, 0, 0, 0 };
        int[] axisY = { 0, 0, -1, 1, 0 };
        public Core()
        {

            GameManger.Instance();

            line = new string[GameManger.buffer._BUFFER_SIZE];

            GameManger.player.Axis2D.x = 1;
            GameManger.player.Axis2D.y = 1;

            currFieldX = 1;
            currFieldY = 1;

            GameManger.currField = GameManger.map.worldMap[currFieldY, currFieldX];
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
                    int beforeX = GameManger.player.Axis2D.x;
                    int beforeY = GameManger.player.Axis2D.y;
                    bool isWall = GameManger.player.MoveAndHold(GameManger.player.direction, Field._FIELD_SIZE, Field._FIELD_SIZE);

                    int currX = GameManger.player.Axis2D.x;
                    int currY = GameManger.player.Axis2D.y;


                    for (int y = 0; y < Field._FIELD_SIZE; y++)
                    {
                        for (int x = 0; x < Field._FIELD_SIZE; x++)
                        {
                            if (GameManger.currField.fogInfo[y, x] == 1)
                            {
                                GameManger.currField.fogInfo[y, x] = 0.5f;
                            }

                        }
                    }

                    //빈칸으로 이동
                    if (GameManger.currField.fieldInfo[currY, currX] == Field.field_info.empty ||
                        GameManger.currField.fieldInfo[currY, currX] == Field.field_info.road)
                    {
                        if (!isWall)
                        {
                        }
                    }

                    //수풀로 이동
                    if (GameManger.currField.fieldInfo[currY, currX] == Field.field_info.mud)
                    {
                        if (!isWall)
                        {
                            if (GameManger.random.Next(1, 101) <= 10)
                            {
                                GameManger.player.direction = 4;
                                isStun = true;
                            }
                        }
                    }
                    //벽으로 이동
                    else if (GameManger.currField.fieldInfo[currY, currX] == Field.field_info.tree ||
                             GameManger.currField.fieldInfo[currY, currX] == Field.field_info.wall)
                    {
                        GameManger.player.Axis2D.x = beforeX;
                        GameManger.player.Axis2D.y = beforeY;
                    }

                    //텔레포트로 이동
                    else if (GameManger.currField.fieldInfo[currY, currX] == Field.field_info.portal)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (GameManger.currField.portals[i] == null) { continue; }


                            if (currX == GameManger.currField.portals[i].axis.x &&
                                currY == GameManger.currField.portals[i].axis.y)
                            {
                                switch (i)
                                {
                                    case 0:
                                        currFieldX += 1;
                                        break;
                                    case 1:
                                        currFieldX -= 1;

                                        break;
                                    case 2:
                                        currFieldY -= 1;

                                        break;
                                    case 3:
                                        currFieldY += 1;

                                        break;
                                }

                                GameManger.player.Teleport(Field._FIELD_SIZE, Field._FIELD_SIZE);
                                GameManger.player.score -= 10;

                                GameManger.currField.isCurrField = false;

                                //Utility.currRoom.enemyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                                GameManger.currField = GameManger.map.worldMap[currFieldY, currFieldX];
                                if (GameManger.currField.type == 0)
                                {
                                }
                                GameManger.currField.isFog = false;
                                GameManger.currField.isCurrField = true;

                                break;
                            }
                        }

                    }

                    //적에게 이동
                    Enemy enemy = GameManger.currField.FindEnemiesAt(currX, currY);
                    if (enemy != null && enemy.isLive)
                    {
                        GameManger.player.hitPoint -= 1;
                        enemy.moveTimer.Dispose();
                        if (GameManger.player.hitPoint == 0)
                        {
                            GameManger.player.isLive = false;
                        }
                        GameManger.currField.enemies.RemoveAll(x => x.Axis2D.x == enemy.Axis2D.x && x.Axis2D.y == enemy.Axis2D.y);
                    }

                    Queue<Coordinate> q = new Queue<Coordinate>();

                    q.Enqueue(GameManger.player.Axis2D);
                    GameManger.currField.fogInfo[GameManger.player.Axis2D.y, GameManger.player.Axis2D.x] = 1;

                    int viewSetting = GameManger.player.viewCount;
                    if (GameManger.map.isDay)
                    {
                        viewSetting += 3;
                    }
                    else
                    {
                        viewSetting -= 2;
                    }
                    viewSetting = ( ( (viewSetting - 1) *2) -2) * (viewSetting - 1) + 1;
                    for (int i = 0; i < viewSetting; i++)
                    {
                        Coordinate tmp = q.Dequeue();

                        if ((tmp.x <= -1 || tmp.y <= -1 || tmp.x >= Field._FIELD_SIZE || tmp.y >= Field._FIELD_SIZE))
                        {}
                        else
                        {
                            GameManger.currField.fogInfo[tmp.y, tmp.x] = 1;
                        }
                        for (int j = 0; j < 4; j++)
                        {
                            int tmpX = (tmp.x + axisX[j]);
                            int tmpY = (tmp.y + axisY[j]);
                            q.Enqueue(new Coordinate(tmpX, tmpY));                            
                        }
                    }

                    foreach (var tmp in q)
                    {
                        if ((tmp.x <= -1 || tmp.y <= -1 || tmp.x >= Field._FIELD_SIZE || tmp.y >= Field._FIELD_SIZE))
                        {
                            continue;
                        }

                        if (GameManger.currField.fogInfo[tmp.y, tmp.x] != 1)
                        {
                            GameManger.currField.fogInfo[tmp.y, tmp.x] = 1;
                        }
                    }
                        
                    
                }
            }
        }

        public void MapToStringArray(ref string[] line)
        {

            for (int y = 0; y < Field._FIELD_SIZE; y++)
            {
                line[y] = "";
                for (int x = 0; x < Field._FIELD_SIZE; x++)
                {


                    //플레이어 4순위
                    if (GameManger.player.Axis2D.x == x && GameManger.player.Axis2D.y == y)
                    {
                        if (GameManger.player.isLive)
                        {
                            if (GameManger.player.hitPoint < 20)
                            {
                                line[y] += ".6.";

                            }
                            else if (GameManger.player.hitPoint < 50)
                            {
                                line[y] += ".5.";

                            }
                            else if (GameManger.player.hitPoint < 200)
                            {
                                line[y] += ".4.";

                            }
                            else if (GameManger.player.hitPoint < 9999)
                            {
                                line[y] += ".0.";

                            }

                            switch (GameManger.player.direction)
                            {
                                case 0:
                                    line[y] += "▶.";
                                    break;
                                case 1:
                                    line[y] += "◀.";
                                    break;
                                case 2:
                                    line[y] += "▲.";
                                    break;
                                case 3:
                                    line[y] += "▼.";
                                    break;
                                case 4:
                                    line[y] += "！.";
                                    break;
                                default:
                                    line[y] += "나.";
                                    break;
                            }

                        }
                        else
                        {
                            line[y] += ".3.Π.";
                        }
                        continue;
                    }


                    if (GameManger.currField.fogInfo[y, x] == 0)
                    {
                        line[y] += ".8.　.";
                    }
                    else
                    {
                        if (GameManger.currField.fogInfo[y, x] == 1)
                        {
                            if (GameManger.currField.fieldInfo[y, x] == Field.field_info.portal)
                            {
                                line[y] += ".2.";
                            }
                            else
                            {
                                line[y] += ".0.";

                            }
                        }
                        else if (GameManger.currField.fogInfo[y, x] > 0 && GameManger.currField.fogInfo[y, x] < 1)
                        {
                            if (GameManger.currField.fieldInfo[y, x] == Field.field_info.portal)
                            {
                                line[y] += ".12.";
                            }
                            else
                            {
                                line[y] += ".6.";
                            }

                        }

                        //맵정보 마지막
                        switch (GameManger.currField.fieldInfo[y, x])
                        {
                            case Field.field_info.empty:
                                line[y] += "ㄹ.";
                                break;
                            case Field.field_info.mud:
                                line[y] += "＊.";
                                break;
                            case Field.field_info.tree:
                                line[y] += "〓.";
                                break;
                            case Field.field_info.portal:
                                line[y] += "문.";
                                break;
                            case Field.field_info.road:
                                line[y] += "□.";
                                break;
                            case Field.field_info.wall:
                                line[y] += "■.";
                                break;
                        }

                    }



                }


                line[y] += "  ";
            }

            //미니맵
            /*for (int y = 0; y < WorldMap._MAP_SIZE; y++)
            {
                for (int x = 0; x < WorldMap._MAP_SIZE; x++)
                {
                    string[] tmp = GameManger.map.worldMap[y, x].MakeRoomToString();

                    line[y * 3] += tmp[0];
                    line[y * 3 + 1] += tmp[1];
                    line[y * 3 + 2] += tmp[2];
                }
            }*/
        }

        public void PrintMap(Object obj)
        {
            MapToStringArray(ref line);
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
