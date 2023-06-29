using Project1st.Game.Core;
using Project1st.Game.GameObject;
using Project1st.Game.Item;
using Project1st.Game.Map.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Map
{
    public class WorldMap
    {
        public static readonly int _MAP_SIZE = 10;

        int[] axisX = { 1, -1, 0, 0 };
        int[] axisY = { 0, 0, -1, 1 };
        public FieldBase[,] worldMap;
        public int day;
        public Timer dayTimer;
        public static Coordinate testPos;


        public WorldMap()
        {
            day = 0;
            worldMap = new FieldBase[_MAP_SIZE, _MAP_SIZE];

            for (int y = 0; y < _MAP_SIZE; y++)
            {
                for (int x = 0; x < _MAP_SIZE; x++)
                {

                    //홀수 행, 짝수 열
                    if (y % 2 == 0 && x % 2 == 1)
                    {
                        worldMap[y, x] = new Forest(GameManger.random.Next(0, 16));

                        if (x == 0 && worldMap[y, x].portals[1] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[1];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            worldMap[y, x].portals[1] = null;
                        }
                        if (x == _MAP_SIZE - 1 && worldMap[y, x].portals[0] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[0];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            worldMap[y, x].portals[0] = null;
                        }
                        if (y == _MAP_SIZE - 1 && worldMap[y, x].portals[3] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[3];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            worldMap[y, x].portals[3] = null;
                        }
                        if (y == 0 && worldMap[y, x].portals[2] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[2];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            worldMap[y, x].portals[2] = null;
                        }
                    }
                    else if (y % 2 == 1 && x % 2 == 0)
                    {

                        worldMap[y, x] = new Forest(GameManger.random.Next(0, 16));
                        if (x == 0 && worldMap[y, x].portals[1] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[1];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            worldMap[y, x].portals[1] = null;
                        }
                        if (x == _MAP_SIZE - 1 && worldMap[y, x].portals[0] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[0];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            worldMap[y, x].portals[0] = null;
                        }
                        if (y == _MAP_SIZE - 1 && worldMap[y, x].portals[3] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[3];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            worldMap[y, x].portals[3] = null;
                        }
                        if (y == 0 && worldMap[y, x].portals[2] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[2];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            worldMap[y, x].portals[2] = null;
                        }
                    }

                }

            }

            int cnt = 0;
            for (int y = 0; y < _MAP_SIZE; y++)
            {
                for (int x = 0; x < _MAP_SIZE; x++)
                {

                    //홀수 행, 짝수 열
                    if (y % 2 == 0 && x % 2 == 1)
                    {
                        if (x == _MAP_SIZE - 1 && cnt == 0) continue;
                        if (x == 0 && cnt == 1) continue;
                        if (y == _MAP_SIZE - 1 && cnt == 3) continue;
                        if (y == 0 && cnt == 2) continue;

                        if (worldMap[y, x].portals[cnt] == null)
                        {
                            worldMap[y, x].CreateDoor(cnt);
                            cnt += 1;
                            if (cnt == 4)
                            {
                                cnt = 0;
                            }
                        }
                    }
                    else if (y % 2 == 1 && x % 2 == 0)
                    {
                        if (x == _MAP_SIZE - 1 && cnt == 0) continue;
                        if (x == 0 && cnt == 1) continue;
                        if (y == _MAP_SIZE - 1 && cnt == 3) continue;
                        if (y == 0 && cnt == 2) continue;

                        if (worldMap[y, x].portals[cnt] == null)
                        {
                            worldMap[y, x].CreateDoor(cnt);
                            cnt += 1;
                            if (cnt == 4)
                            {
                                cnt = 0;
                            }
                        }
                    }

                }

            }

            for (int y = 0; y < _MAP_SIZE; y++)
            {
                for (int x = 0; x < _MAP_SIZE; x++)
                {
                    //홀수 행, 짝수 열
                    if ((y % 2 == 0 && x % 2 == 0) || (y % 2 == 1 && x % 2 == 1))
                    {
                        worldMap[y, x] = new Forest();
                        if (x != 0 && worldMap[y, x - 1].portals[0] != null)
                        {
                            Portal nextDoor = worldMap[y, x - 1].portals[0];

                            worldMap[y, x].portals[1] = new Portal(0, nextDoor.axis.y);
                            Portal currDoor = worldMap[y, x].portals[1];


                            worldMap[y, x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;

                        }

                        if (x != _MAP_SIZE - 1 && worldMap[y, x + 1].portals[1] != null)
                        {
                            Portal nextDoor = worldMap[y, x + 1].portals[1];

                            worldMap[y, x].portals[0] = new Portal(FieldBase._FIELD_SIZE - 1, nextDoor.axis.y);
                            Portal currDoor = worldMap[y, x].portals[0];

                            worldMap[y, x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;


                        }

                        if (y != _MAP_SIZE - 1 && worldMap[y + 1, x].portals[2] != null)
                        {
                            Portal nextDoor = worldMap[y + 1, x].portals[2];

                            worldMap[y, x].portals[3] = new Portal(nextDoor.axis.x, FieldBase._FIELD_SIZE - 1);
                            Portal currDoor = worldMap[y, x].portals[3];

                            worldMap[y, x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;

                        }

                        if (y != 0 && worldMap[y - 1, x].portals[3] != null)
                        {
                            Portal nextDoor = worldMap[y - 1, x].portals[3];

                            worldMap[y, x].portals[2] = new Portal(nextDoor.axis.x, 0);
                            Portal currDoor = worldMap[y, x].portals[2];

                            worldMap[y, x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;

                        }
                    }

                }

            }

            Coordinate connect;
            for (int y = 0; y < _MAP_SIZE; y++)
            {
                for (int x = 0; x < _MAP_SIZE; x++)
                {
                    connect = check(x, y);
                    if (connect == null)
                    {
                        continue;
                    }
                    else if (connect.x != 0 && worldMap[connect.y, connect.x].portals[1] == null)
                    {
                        worldMap[connect.y, connect.x - 1].CreateDoor(0);
                        Portal nextDoor = worldMap[connect.y, connect.x - 1].portals[0];

                        worldMap[connect.y, connect.x].portals[1] = new Portal(0, nextDoor.axis.y);
                        Portal currDoor = worldMap[connect.y, connect.x].portals[1];


                        worldMap[connect.y, connect.x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;
                        worldMap[connect.y, connect.x -1].fieldInfo[nextDoor.axis.y, nextDoor.axis.x] = FieldBase.field_info.portal;

                    }

                    else if (connect.x != _MAP_SIZE - 1 && worldMap[connect.y, connect.x].portals[0] == null)
                    {
                        worldMap[connect.y, connect.x + 1].CreateDoor(1);
                        Portal nextDoor = worldMap[connect.y, connect.x + 1].portals[1];

                        worldMap[connect.y, connect.x].portals[0] = new Portal(FieldBase._FIELD_SIZE - 1, nextDoor.axis.y);
                        Portal currDoor = worldMap[connect.y, connect.x].portals[0];

                        worldMap[connect.y, connect.x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;
                        worldMap[connect.y, connect.x + 1].fieldInfo[nextDoor.axis.y, nextDoor.axis.x] = FieldBase.field_info.portal;
                    }

                    else if (connect.y != _MAP_SIZE - 1 && worldMap[connect.y, connect.x].portals[3] == null)
                    {
                        worldMap[connect.y + 1, connect.x].CreateDoor(2);
                        Portal nextDoor = worldMap[connect.y + 1, connect.x].portals[2];

                        worldMap[connect.y, connect.x].portals[3] = new Portal(nextDoor.axis.x, FieldBase._FIELD_SIZE - 1);
                        Portal currDoor = worldMap[connect.y, connect.x].portals[3];

                        worldMap[connect.y, connect.x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;
                        worldMap[connect.y +1, connect.x].fieldInfo[nextDoor.axis.y, nextDoor.axis.x] = FieldBase.field_info.portal;
                    }

                    else if (connect.y != 0 && worldMap[connect.y, connect.x].portals[2] == null)
                    {
                        worldMap[connect.y - 1, connect.x].CreateDoor(3);
                        Portal nextDoor = worldMap[connect.y - 1, connect.x].portals[3];

                        worldMap[connect.y, connect.x].portals[2] = new Portal(nextDoor.axis.x, 0);
                        Portal currDoor = worldMap[connect.y, connect.x].portals[2];

                        worldMap[connect.y, connect.x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;
                        worldMap[connect.y -1, connect.x].fieldInfo[nextDoor.axis.y, nextDoor.axis.x] = FieldBase.field_info.portal;
                    }
                }
            }

            Town startTown = new Town(worldMap[1, 1]);
            Town finalTown = new Town(worldMap[_MAP_SIZE-1, _MAP_SIZE-1]);
            finalTown.shop.Add(new Items(GameManger.db.database[100]));

            int tmpx = 1;
            int tmpy = 1;
            worldMap[tmpy, tmpx] = startTown;

            for (int i = 0; i < 4; i++)
            {
                if (worldMap[tmpy, tmpx].portals[i] != null) {
                    switch (i) 
                    {
                        case 0:
                            worldMap[tmpy, tmpx + 1].portals[1].isTown = true;
                            break;
                        case 1:
                            worldMap[tmpy, tmpx - 1].portals[0].isTown = true;
                            break;

                        case 2:
                            worldMap[tmpy - 1, tmpx].portals[3].isTown = true;
                            break;

                        case 3:
                            worldMap[tmpy + 1, tmpx].portals[2].isTown = true;
                            break;

                    }
                }
            }
            tmpx = _MAP_SIZE - 1;
            tmpy = _MAP_SIZE - 1;
            worldMap[_MAP_SIZE - 1, _MAP_SIZE - 1] = finalTown;

            for (int i = 0; i < 4; i++)
            {
                if (worldMap[tmpy, tmpx].portals[i] != null)
                {
                    switch (i)
                    {
                        case 0:
                            worldMap[tmpy, tmpx + 1].portals[1].isTown = true;
                            break;
                        case 1:
                            worldMap[tmpy, tmpx - 1].portals[0].isTown = true;
                            break;

                        case 2:
                            worldMap[tmpy - 1, tmpx].portals[3].isTown = true;
                            break;

                        case 3:
                            worldMap[tmpy + 1, tmpx].portals[2].isTown = true;
                            break;

                    }
                }
            }
            //int count = GameManger.random.Next(3, 6);
            int count = 20;

            while (count > 0)
            {
                tmpx = GameManger.random.Next(0, _MAP_SIZE);
                tmpy = GameManger.random.Next(0, _MAP_SIZE);

                if (worldMap[tmpy, tmpx].type != 2)
                {
                    if (tmpx != 0 && worldMap[tmpy, tmpx - 1].type == 2)
                    {
                        continue;
                    }

                    else if (tmpx != _MAP_SIZE - 1 && worldMap[tmpy, tmpx + 1].type == 2)
                    {
                        continue;
                    }

                    else if (tmpy != _MAP_SIZE - 1 && worldMap[tmpy + 1, tmpx].type == 2)
                    {
                        continue;
                    }

                    else if (tmpy != 0 && worldMap[tmpy - 1, tmpx].type == 2)
                    {
                        continue;
                    }

                    else
                    {
                        worldMap[tmpy, tmpx] = new Town(worldMap[tmpy, tmpx]);
                        count -= 1;

                        for (int i = 0; i < 4; i++)
                        {
                            if (worldMap[tmpy, tmpx].portals[i] != null)
                            {
                                switch (i)
                                {
                                    case 0:
                                        worldMap[tmpy, tmpx + 1].portals[1].isTown = true;
                                        break;
                                    case 1:
                                        worldMap[tmpy, tmpx - 1].portals[0].isTown = true;
                                        break;

                                    case 2:
                                        worldMap[tmpy - 1, tmpx].portals[3].isTown = true;
                                        break;

                                    case 3:
                                        worldMap[tmpy + 1, tmpx].portals[2].isTown = true;
                                        break;

                                }
                            }
                        }
                    }
                }
            }

            dayTimer = new Timer(SetDayTimer, null, 100, 60000);
        }

        public void SetDayTimer(object obj)
        {
            day += 1;

            if(day == 2){

                for (int y = 0; y < WorldMap._MAP_SIZE; y++)
                {
                    for (int x = 0; x < WorldMap._MAP_SIZE; x++)
                    {
                        if (GameManger.map.worldMap[y, x].type == 2)
                        {
                            foreach (var a in GameManger.map.worldMap[y, x].ReturnSelfToTown().priceRate)
                            {
                                if (a.Value.keepTurn == 0)
                                {
                                    for (int i = 3; i < GameManger.map.worldMap[y, x].ReturnSelfToTown().shop.Count; i++)
                                    {
                                        if (GameManger.map.worldMap[y, x].ReturnSelfToTown().shop[i].count < 20)
                                        {
                                            GameManger.random.Next(3, 11);
                                        }
                                    }
                                }
                                a.Value.ChangePriceRate();
                            }
                        }
                    }
                }
                day = 0;
            }
        }

        Coordinate check(int x, int y)
        {
            if (x == -1 || x == _MAP_SIZE || y == -1 || y == _MAP_SIZE) return null;
            if (worldMap[y, x].isFog) return null;

            Coordinate last = null;

            for (int i = 0; i < 4; i++)
            {
                if (worldMap[y, x].portals[i] != null)
                {
                    worldMap[y, x].isFog = true;
                    last = check(x + axisX[i], y + axisY[i]);
                }
            }

            if (last == null)
            {
                // Console.WriteLine(x + ", " + y + " ");

                worldMap[y, x].isFog = true;
                return new Coordinate(x, y);
            }
            else
            {
                // Console.WriteLine("_"+x + ", " + y + " "+"("+last.x+"/"+last.y+")");
                return last;
            }
        }
    }

}
