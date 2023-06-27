using Project1st.Game.Core;
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
        public static readonly int _MAP_SIZE = 10;

        int[] axisX = { 1, -1, 0, 0 };
        int[] axisY = { 0, 0, -1, 1 };
        public Field[,] worldMap;
        public bool isDay;
        public Timer dayTimer;

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
                        worldMap[y, x] = new Forest(GameManger.random.Next(0, 16));

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

                        worldMap[y, x] = new Forest(GameManger.random.Next(0, 16));
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
                        worldMap[y, x] = new Forest();
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
                    else if (connect.x != 0 && worldMap[connect.y, connect.x].portals[1] == null)
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
            dayTimer = new Timer(SetDayTimer, null, 100, 120000);
        }

        public void SetDayTimer(object obj)
        {
            isDay = !isDay;
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

    public class Forest : Field
    {
        public List<Enemy> enemies;
        public Timer createTimer;

        public float[,] fogInfo;

        public Forest()
        {
            type = 0;
            int wallCount = 60;
            int wallSleep = 2;
            fogInfo = new float[_FIELD_SIZE, _FIELD_SIZE];

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
        }


        public Forest(int flag)
        {
            type = 0;
            int wallCount = 60;
            int wallSleep = 2;
            fogInfo = new float[_FIELD_SIZE, _FIELD_SIZE];

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


        public override void CreateDoor(int index)
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

        public override void CreateEnemy(object obj)
        {
            if (enemies.Count > 6) return;
            while (true)
            {
                int enemyX = GameManger.random.Next(_FIELD_SIZE);
                int enemyY = GameManger.random.Next(_FIELD_SIZE);
                if (fieldInfo[enemyY, enemyX] != field_info.tree)
                {
                    Enemy enemy = new Enemy(enemyX, enemyY);
                    enemies.Add(enemy);
                    break;
                }
            }
        }
        public override void RemoveEnemy(int index)
        {
            if (enemies.Count <= 0) return;
            if (enemies[index].moveTimer != null)
            {
                enemies[index].moveTimer.Dispose();
            }
            enemies.RemoveAt(index);
        }
        public override void RemoveEnemy(int x, int y)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                if (enemies[i].Axis2D.x == x && enemies[i].Axis2D.y == y)
                {
                    if (enemies[i].moveTimer != null)
                    {
                        enemies[i].moveTimer.Dispose();
                    }
                    enemies.RemoveAt(i);
                }
            }
        }

        public override Enemy FindEnemiesAt(int x, int y)
        {
            if (enemies == null) return null;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                if (enemies[i].Axis2D.x == x && enemies[i].Axis2D.y == y) return enemies[i];
            }
            return null;
        }
        public override void StopEnemies()
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
        public override void PlayEnemies()
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


        public override string[] ConvertMapToString(ref string[] line)
        {

            for (int y = 0; y < Field._FIELD_SIZE; y++)
            {
                line[y] = "";
                for (int x = 0; x < Field._FIELD_SIZE; x++)
                {
                    if (fogInfo[y, x] == 1)
                    {
                        //적 2순위
                        Enemy tmp = FindEnemiesAt(x, y);
                        if (tmp != null)
                        {
                            if (tmp.isLive)
                            {
                                if (tmp.hitPoint < Enemy.EnemyHitPointMAX / 20)
                                {
                                    line[y] += ".7.";
                                }
                                else if (tmp.hitPoint <= Enemy.EnemyHitPointMAX)
                                {
                                    line[y] += ".1.";
                                }
                                else
                                {
                                    line[y] += ".0.";
                                }
                                line[y] += "적.";
                            }
                            else if (!tmp.isLive)
                            {
                                line[y] += ".1.？.";
                            }
                            continue;
                        }
                    }

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


                    if (fogInfo[y, x] == 0)
                    {
                        line[y] += ".8.　.";
                    }
                    else
                    {
                        if (fogInfo[y, x] == 1)
                        {
                            if (fieldInfo[y, x] == Field.field_info.portal)
                            {
                                line[y] += ".2.";
                            }
                            else
                            {
                                line[y] += ".0.";

                            }
                        }
                        else if (fogInfo[y, x] > 0 && fogInfo[y, x] < 1)
                        {
                            if (fieldInfo[y, x] == Field.field_info.portal)
                            {
                                line[y] += ".12.";
                            }
                            else
                            {
                                line[y] += ".6.";
                            }

                        }

                        //맵정보 마지막
                        switch (fieldInfo[y, x])
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

            return line;
        }

        public override float[,] GetFogInfo()
        {
            return fogInfo;
        }
        public override float GetFogInfo(int x, int y)
        {
            return fogInfo[y,x];
        }
        public override void SetFogInfo(int x, int y, float info)
        {
            fogInfo[y, x] = info;
        }

        public override Timer GetCreateTimer()
        {
            return createTimer;
        }
        public override void SetCreateTimer(Timer timer)
        {
            createTimer = timer;
        }
        public override List<Enemy> GetEnemies()
        {
            return enemies;
        }

    }

    public class Town : Field
    {
    }

    public class City : Field
    {
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
