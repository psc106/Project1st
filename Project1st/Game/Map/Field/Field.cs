using Project1st.Game.Core;
using Project1st.Game.GameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Map
{

    public class Field
    {
        public static readonly int _FIELD_SIZE = 15;

        public enum field_info : byte
        {
            road = 0, empty, mud, tree, wall, portal, error = 90
        }

        //0-길 1-빈땅 2-진창 3-나무 4-포탈
        public Portal[] portals;
        public field_info[,] fieldInfo;

        public int type;
        public bool isFog;
        public bool isCurrField;
        public Field()
        {
            type = 0;
            portals = new Portal[4];
            fieldInfo = new field_info[_FIELD_SIZE, _FIELD_SIZE];

            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fieldInfo[y, x] = field_info.empty;
                }
            }
        }


        public Field(int flag)
        {
            type = 0;

            portals = new Portal[4];
            fieldInfo = new field_info[_FIELD_SIZE, _FIELD_SIZE];
            int bit = 1;

            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fieldInfo[y, x] = field_info.empty;
                }
            }

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


        public virtual void CreateDoor(int index)
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
        }

        public virtual float[,] GetFogInfo()
        {
            return null;
        }
        public virtual float GetFogInfo(int x, int y)
        {
            return 0;
        }
        public virtual void SetFogInfo(int x, int y, float info)
        {
        }

        public virtual string[] ConvertMapToString(ref string[] strings)
        {
            strings = null;
            return null;
        }

        public virtual List<Enemy> GetEnemies()
        {
            return null;
        }
        public virtual Timer GetCreateTimer()
        {
            return null;
        }
        public virtual void SetCreateTimer(Timer timer)
        {
        }
        public virtual void CreateEnemy(object obj)
        {
        }

        public field_info GetElementAt(int x, int y)
        {
            if (fieldInfo == null) return field_info.error;
            return fieldInfo[y, x];
        }
        public virtual void RemoveEnemy(int index)
        {
        }
        public virtual void RemoveEnemy(int x, int y)
        {
        }
        public virtual Enemy FindEnemiesAt(int x, int y)
        {
            return null;
        }
        public virtual void StopEnemies()
        {
        }
        public virtual void PlayEnemies()
        {
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


    public class Forest : Field
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

    }

    public class Town : Field
    {
        public Town()
        {
            type = 2;
        }

        public Town(Field field)
        {
            type = 2;
            portals = field.portals;
            for (int i = 0; i < 4; i++)
            {
                if (portals[i] != null)
                {
                    fieldInfo[portals[i].axis.y, portals[i].axis.x] = field_info.portal;
                }
            }
        }


        public override string[] ConvertMapToString(ref string[] line)
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


                    line[y] += ".0.";
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
                line[y] += "  ";
            }

            return line;
        }

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
