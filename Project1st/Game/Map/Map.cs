﻿using Project1st.Game.Core;
using Project1st.Game.GameObject;
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
        public static readonly int _MAP_SIZE = 6;

        int[] axisX = { 1, -1, 0, 0 };
        int[] axisY = { 0, 0, -1, 1 };
        public Field[,] worldMap;
        public bool isDay;


        public WorldMap()
        {
            isDay = true;
            worldMap = new Field[_MAP_SIZE, _MAP_SIZE];
            for (int y = 0; y < _MAP_SIZE; y++)
            {
                for (int x = 0; x < _MAP_SIZE; x++)
                {

                    //홀수 행, 짝수 열
                    if (y % 2 == 0 && x % 2 == 1)
                    {
                        worldMap[y, x] = new Field(GameManger.random.Next(0, 16));

                        if (x == 0 && worldMap[y, x].portals[1] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[1];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = Field.field_info.empty;
                            worldMap[y, x].portals[1] = null;
                        }
                        if (x == _MAP_SIZE - 1 && worldMap[y, x].portals[0] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[0];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = Field.field_info.empty;
                            worldMap[y, x].portals[0] = null;
                        }
                        if (y == _MAP_SIZE - 1 && worldMap[y, x].portals[3] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[3];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = Field.field_info.empty;
                            worldMap[y, x].portals[3] = null;
                        }
                        if (y == 0 && worldMap[y, x].portals[2] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[2];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = Field.field_info.empty;
                            worldMap[y, x].portals[2] = null;
                        }
                    }
                    else if (y % 2 == 1 && x % 2 == 0)
                    {

                        worldMap[y, x] = new Field(GameManger.random.Next(0, 16));
                        if (x == 0 && worldMap[y, x].portals[1] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[1];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = Field.field_info.empty;
                            worldMap[y, x].portals[1] = null;
                        }
                        if (x == _MAP_SIZE - 1 && worldMap[y, x].portals[0] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[0];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = Field.field_info.empty;
                            worldMap[y, x].portals[0] = null;
                        }
                        if (y == _MAP_SIZE - 1 && worldMap[y, x].portals[3] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[3];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = Field.field_info.empty;
                            worldMap[y, x].portals[3] = null;
                        }
                        if (y == 0 && worldMap[y, x].portals[2] != null)
                        {
                            Portal currPortal = worldMap[y, x].portals[2];
                            worldMap[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = Field.field_info.empty;
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
                        worldMap[y, x] = new Field();
                        if (x != 0 && worldMap[y, x - 1].portals[0] != null)
                        {
                            Portal nextDoor = worldMap[y, x - 1].portals[0];

                            worldMap[y, x].portals[1] = new Portal(0, nextDoor.axis.y);
                            Portal currDoor = worldMap[y, x].portals[1];


                            worldMap[y, x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = Field.field_info.portal;

                        }

                        if (x != _MAP_SIZE - 1 && worldMap[y, x + 1].portals[1] != null)
                        {
                            Portal nextDoor = worldMap[y, x + 1].portals[1];

                            worldMap[y, x].portals[0] = new Portal(Field._FIELD_SIZE - 1, nextDoor.axis.y);
                            Portal currDoor = worldMap[y, x].portals[0];

                            worldMap[y, x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = Field.field_info.portal;


                        }

                        if (y != _MAP_SIZE - 1 && worldMap[y + 1, x].portals[2] != null)
                        {
                            Portal nextDoor = worldMap[y + 1, x].portals[2];

                            worldMap[y, x].portals[3] = new Portal(nextDoor.axis.x, Field._FIELD_SIZE - 1);
                            Portal currDoor = worldMap[y, x].portals[3];

                            worldMap[y, x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = Field.field_info.portal;

                        }

                        if (y != 0 && worldMap[y - 1, x].portals[3] != null)
                        {
                            Portal nextDoor = worldMap[y - 1, x].portals[3];

                            worldMap[y, x].portals[2] = new Portal(nextDoor.axis.x, 0);
                            Portal currDoor = worldMap[y, x].portals[2];

                            worldMap[y, x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = Field.field_info.portal;

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
                    else if (connect.x != 0 && worldMap[connect.y, connect.x].portals[1]==null)
                    {
                        worldMap[connect.y, connect.x].CreateDoor(1);
                        worldMap[connect.y, connect.x - 1].CreateDoor(0);
                    }

                    else if (connect.x != _MAP_SIZE - 1 && worldMap[connect.y, connect.x].portals[0] == null)
                    {
                        worldMap[connect.y, connect.x].CreateDoor(0);
                        worldMap[connect.y, connect.x + 1].CreateDoor(1);
                    }

                    else if (connect.y != _MAP_SIZE - 1 && worldMap[connect.y, connect.x].portals[3] == null)
                    {
                        worldMap[connect.y, connect.x].CreateDoor(3);
                        worldMap[connect.y + 1, connect.x].CreateDoor(2);
                    }

                    else if (connect.y != 0 && worldMap[connect.y, connect.x].portals[2] == null)
                    {
                        worldMap[connect.y, connect.x].CreateDoor(2);
                        worldMap[connect.y - 1, connect.x].CreateDoor(3);
                    }
                }
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

    public class Field
    {
        public static readonly int _FIELD_SIZE = 15;

        public enum field_info : byte
        {
            road = 0, empty, mud, tree, wall, portal, error=90       
        }

        //0-길 1-빈땅 2-진창 3-나무 4-포탈
        public field_info[,] fieldInfo;
        public float[,] fogInfo;
        public Portal[] portals;

        public List<Enemy> enemies;


        public int type;
        public bool isFog;
        public bool isCurrField;
        public Field()
        {
            type = 0;
            int wallCount = 60;
            int wallSleep = 2;
            fogInfo = new float[_FIELD_SIZE, _FIELD_SIZE];
            fieldInfo = new field_info[_FIELD_SIZE, _FIELD_SIZE];
            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fogInfo[y, x] = 0;
                    if (wallSleep == 0 && wallCount > 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1,4);
                        if (fieldInfo[y, x] == field_info.tree)
                        {
                            wallCount -= 1;
                            wallSleep = 3;
                        }
                    }
                    else if (wallSleep > 0 || wallCount == 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1,3);
                        wallSleep -= 1;
                    }
                }
            }
            enemies = new List<Enemy>();
            portals = new Portal[4];
        }


        public Field(int flag)
        {
            type = 0;
            int wallCount = 60;
            int wallSleep = 2;
            fogInfo = new float[_FIELD_SIZE, _FIELD_SIZE];
            fieldInfo = new field_info[_FIELD_SIZE, _FIELD_SIZE];
            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fogInfo[y, x] = 0;
                    if (wallSleep == 0 && wallCount > 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1, 4);
                        if (fieldInfo[y, x] == field_info.tree)
                        {
                            wallCount -= 1;
                            wallSleep = 3;
                        }
                    }
                    else if (wallSleep > 0 || wallCount == 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1, 3);
                        wallSleep -= 1;
                    }
                }
            }

            enemies = new List<Enemy>();
            portals = new Portal[4];
            int bit = 1;

            for (int i = 0; i < portals.Length; i++)
            {
                if ((flag & bit) == bit)
                {
                    CreateDoor(i);
                }
                else
                {
                    portals[i] = null;
                }
                bit = bit << 1;
            }

            //portal[0].asd();
        }


        public void CreateDoor(int index)
        {
            switch (index)
            {
                case 0:
                    portals[index] = new Portal(_FIELD_SIZE - 1, GameManger.random.Next(1, _FIELD_SIZE - 1));
                    break;
                case 1:
                    portals[index] = new Portal(0, GameManger.random.Next(1, _FIELD_SIZE - 1));
                    break;
                case 2:
                    portals[index] = new Portal(GameManger.random.Next(1, _FIELD_SIZE - 1), 0);
                    break;
                case 3:
                    portals[index] = new Portal(GameManger.random.Next(1, _FIELD_SIZE - 1), _FIELD_SIZE - 1);
                    break;
            }
            fieldInfo[portals[index].axis.y, portals[index].axis.x] = field_info.portal;
        }

        public field_info GetElementAt(int x, int y)
        {
            if (fieldInfo == null) return field_info.error;
            return fieldInfo[y, x];
        }
        public Enemy FindEnemiesAt(int x, int y)
        {
            if (enemies == null) return null;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                if (enemies[i].Axis2D.x == x && enemies[i].Axis2D.y == y) return enemies[i];
            }
            return null;
        }
        public void StopEnemies()
        {
            if (enemies == null) return;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].moveTimer != null)
                {
                    enemies[i].moveTimer.Dispose();
                }
                //enemies[i].enemyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
            return;
        }
        public void PlayEnemies()
        {
            if (enemies == null) return;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].moveTimer != null)
                {
                    enemies[i].StartTimer();
                }
                //enemies[i].enemyTimer.Change(500, 500);
            }
        }


        public string[] MakeRoomToString()
        {
            string[] line = new string[3];

            for (int y = 0; y < line.Length; y++)
            {
                line[y] = "";

            }

            if (!this.isFog && this.portals[2] != null)
            {
                line[0] += "　↓　";
            }
            else
            {
                line[0] += "　　　";
            }

            if (!this.isFog && this.portals[1] != null)
            {
                line[1] += "→";
            }
            else
            {
                line[1] += "　";
            }

            if (!this.isFog)
            {
                if (!this.isCurrField)
                {
                    line[1] += "□";
                }
                else
                {
                    line[1] += "■";
                }
            }
            else
            {
                line[1] += "　";
            }

            if (!this.isFog && this.portals[0] != null)
            {
                line[1] += "←";
            }
            else
            {
                line[1] += "　";
            }
            if (!this.isFog && this.portals[3] != null)
            {
                line[2] += "　↑　";
            }
            else
            {
                line[2] += "　　　";
            }

            return line;
        }


    }

    public class Portal
    {
        public Coordinate axis;

        public Portal() 
        {
            axis = new Coordinate();        
        }
        public Portal(int x, int y) 
        {
            axis = new Coordinate(x, y);
        }
    }

}
