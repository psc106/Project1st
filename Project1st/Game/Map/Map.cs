using Project1st.Game.Core;
using Project1st.Game.GameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.Map
{
    public class WorldMap
    {
        public static readonly int _MAP_SIZE = 8;

        Field[,] map;
        int[] axisX = { 1, -1, 0, 0 };
        int[] axisY = { 0, 0, -1, 1 };
        public Field[,] field;
        public int size;


        public WorldMap()
        {
            field = new Field[_MAP_SIZE, _MAP_SIZE];
            for (int y = 0; y < _MAP_SIZE; y++)
            {
                for (int x = 0; x < _MAP_SIZE; x++)
                {

                    //홀수 행, 짝수 열
                    if (y % 2 == 0 && x % 2 == 1)
                    {
                        field[y, x] = new Field(GameManger.random.Next(0, 16));

                        if (x == 0 && field[y, x].portals[1] != null)
                        {
                            Portal currPortal = field[y, x].portals[1];
                            field[y, x].fieldInfomation[currPortal.axis.y, currPortal.axis.x] = 0;
                            field[y, x].portals[1] = null;
                        }
                        if (x == size - 1 && field[y, x].portals[0] != null)
                        {
                            Portal currPortal = field[y, x].portals[0];
                            field[y, x].fieldInfomation[currPortal.axis.y, currPortal.axis.x] = 0;
                            field[y, x].portals[0] = null;
                        }
                        if (y == size - 1 && field[y, x].portals[3] != null)
                        {
                            Portal currPortal = field[y, x].portals[3];
                            field[y, x].fieldInfomation[currPortal.axis.y, currPortal.axis.x] = 0;
                            field[y, x].portals[3] = null;
                        }
                        if (y == 0 && field[y, x].portals[2] != null)
                        {
                            Portal currPortal = field[y, x].portals[2];
                            field[y, x].fieldInfomation[currPortal.axis.y, currPortal.axis.x] = 0;
                            field[y, x].portals[2] = null;
                        }
                    }
                    else if (y % 2 == 1 && x % 2 == 0)
                    {

                        field[y, x] = new Field(GameManger.random.Next(0, 16));
                        if (x == 0 && field[y, x].portals[1] != null)
                        {
                            Portal currPortal = field[y, x].portals[1];
                            field[y, x].fieldInfomation[currPortal.axis.y, currPortal.axis.x] = 0;
                            field[y, x].portals[1] = null;
                        }
                        if (x == size - 1 && field[y, x].portals[0] != null)
                        {
                            Portal currPortal = field[y, x].portals[0];
                            field[y, x].fieldInfomation[currPortal.axis.y, currPortal.axis.x] = 0;
                            field[y, x].portals[0] = null;
                        }
                        if (y == size - 1 && field[y, x].portals[3] != null)
                        {
                            Portal currPortal = field[y, x].portals[3];
                            field[y, x].fieldInfomation[currPortal.axis.y, currPortal.axis.x] = 0;
                            field[y, x].portals[3] = null;
                        }
                        if (y == 0 && field[y, x].portals[2] != null)
                        {
                            Portal currPortal = field[y, x].portals[2];
                            field[y, x].fieldInfomation[currPortal.axis.y, currPortal.axis.x] = 0;
                            field[y, x].portals[2] = null;
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
                        if (x == size - 1 && cnt == 0) continue;
                        if (x == 0 && cnt == 1) continue;
                        if (y == size - 1 && cnt == 3) continue;
                        if (y == 0 && cnt == 2) continue;

                        if (field[y, x].portals[cnt] == null)
                        {
                            field[y, x].CreateDoor(cnt);
                            cnt += 1;
                            if (cnt == 4)
                            {
                                cnt = 0;
                            }
                        }
                    }
                    else if (y % 2 == 1 && x % 2 == 0)
                    {
                        if (x == size - 1 && cnt == 0) continue;
                        if (x == 0 && cnt == 1) continue;
                        if (y == size - 1 && cnt == 3) continue;
                        if (y == 0 && cnt == 2) continue;

                        if (field[y, x].portals[cnt] == null)
                        {
                            field[y, x].CreateDoor(cnt);
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
                        field[y, x] = new Field();
                        if (x != 0 && field[y, x - 1].portals[0] != null)
                        {
                            Portal nextDoor = field[y, x - 1].portals[0];

                            field[y, x].portals[1] = new Portal(0, nextDoor.axis.y);
                            Portal currDoor = field[y, x].portals[1];


                            field[y, x].fieldInfomation[currDoor.axis.y, currDoor.axis.x] = (int)Field._FIELD_INFORMATION.portal;

                        }

                        if (x != size - 1 && field[y, x + 1].portals[1] != null)
                        {
                            Portal nextDoor = field[y, x + 1].portals[1];

                            field[y, x].portals[0] = new Portal(Field._FIELD_SIZE - 1, nextDoor.axis.y);
                            Portal currDoor = field[y, x].portals[0];

                            field[y, x].fieldInfomation[currDoor.axis.y, currDoor.axis.x] = 3;


                        }

                        if (y != size - 1 && field[y + 1, x].portals[2] != null)
                        {
                            Portal nextDoor = field[y + 1, x].portals[2];

                            field[y, x].portals[3] = new Portal(nextDoor.axis.x, Field._FIELD_SIZE - 1);
                            Portal currDoor = field[y, x].portals[3];

                            field[y, x].fieldInfomation[currDoor.axis.y, currDoor.axis.x] = 3;

                        }

                        if (y != 0 && field[y - 1, x].portals[3] != null)
                        {
                            Portal nextDoor = field[y - 1, x].portals[3];

                            field[y, x].portals[2] = new Portal(nextDoor.axis.x, 0);
                            Portal currDoor = field[y, x].portals[2];

                            field[y, x].fieldInfomation[currDoor.axis.y, currDoor.axis.x] = 3;

                        }
                    }

                }

            }
        }
    }

    public class Field
    {
        public static readonly int _FIELD_SIZE = 15;

        public enum _FIELD_INFORMATION
        {
            road = 0, empty, mud, tree, wall, portal            
        }

        //0-길 1-빈땅 2-진창 3-나무 4-포탈
        public int[,] fieldInfomation;
        public Portal[] portals;

        public List<Enemy> enemies;

        public int type;
        public bool isFog;
        public Field()
        {
            type = 0;
            isFog = true;
            int wallCount = 60;
            int wallSleep = 2;
            fieldInfomation = new int[_FIELD_SIZE, _FIELD_SIZE];
            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    if (wallSleep == 0 && wallCount > 0)
                    {
                        fieldInfomation[y, x] = GameManger.random.Next(1,4);
                        if (fieldInfomation[y, x] == (int)_FIELD_INFORMATION.tree)
                        {
                            wallCount -= 1;
                            wallSleep = 3;
                        }
                    }
                    else if (wallSleep > 0 || wallCount == 0)
                    {
                        fieldInfomation[y, x] = GameManger.random.Next(1,3);
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
            isFog = true;
            int wallCount = 60;
            int wallSleep = 2;
            fieldInfomation = new int[_FIELD_SIZE, _FIELD_SIZE];
            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    if (wallSleep == 0 && wallCount > 0)
                    {
                        fieldInfomation[y, x] = GameManger.random.Next(1, 4);
                        if (fieldInfomation[y, x] == (int)_FIELD_INFORMATION.tree)
                        {
                            wallCount -= 1;
                            wallSleep = 3;
                        }
                    }
                    else if (wallSleep > 0 || wallCount == 0)
                    {
                        fieldInfomation[y, x] = GameManger.random.Next(1, 3);
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
            fieldInfomation[portals[index].axis.y, portals[index].axis.x] = (int)_FIELD_INFORMATION.portal;
        }

    }

    public class Portal
    {
        public Coordinate axis;

        public Portal() { }
        public Portal(int x, int y) 
        {
            this.axis.x = x;
            this.axis.y = y;
        }
    }

}
