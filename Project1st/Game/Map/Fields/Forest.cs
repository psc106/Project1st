using Project1st.Game.Core;
using Project1st.Game.GameObject;
using Project1st.Game.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Map.Fields
{

    public class Forest : FieldBase
    {
        public List<Enemy> enemies;
        public Timer createTimer;

        public float[,] fogInfo;

        public Forest()
        {
            type = 1;
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
            type = 1;
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

            for (int y = 0; y < FieldBase._FIELD_SIZE; y++)
            {
                line[y] = "";
                for (int x = 0; x < FieldBase._FIELD_SIZE; x++)
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
                            if (fieldInfo[y, x] == FieldBase.field_info.portal)
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
                            if (fieldInfo[y, x] == FieldBase.field_info.portal)
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
                            case FieldBase.field_info.empty:
                                line[y] += "ㄹ.";
                                break;
                            case FieldBase.field_info.mud:
                                line[y] += "＊.";
                                break;
                            case FieldBase.field_info.tree:
                                line[y] += "〓.";
                                break;
                            case FieldBase.field_info.portal:
                                bool isTownConnect = false;
                                if (x == _FIELD_SIZE - 1) isTownConnect = portals[0].isTown;
                                else if (x == 0) isTownConnect = portals[1].isTown;
                                else if (y == 0) isTownConnect = portals[2].isTown;
                                else if (y == _FIELD_SIZE - 1) isTownConnect = portals[3].isTown;

                                if (isTownConnect)
                                {
                                    line[y] += "문.";
                                }
                                else
                                {
                                    line[y] += "길.";
                                }
                                break;
                            case FieldBase.field_info.road:
                                line[y] += "□.";
                                break;
                            case FieldBase.field_info.wall:
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
            return fogInfo[y, x];
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

        public override bool Move(Coordinate axis)
        {
            return GameManger.player.Move(axis);
        }
    }
}
