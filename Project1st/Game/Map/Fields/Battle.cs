using Project1st.Game.Core;
using Project1st.Game.GameObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.Map.Fields
{
    public class Battle : FieldBase
    {
        public Player beforePlayerInfo;
        public FieldBase beforeFieldInfo;

        public int enemyCount;
        public List<Enemy> enemies;
        public float[,] fogInfo;

        public Battle(Forest currField)
        {
            type = 3;

            beforePlayerInfo = GameManger.player;
            beforeFieldInfo = GameManger.currField;

            GameManger.player = new Player();
            GameManger.currField = this;

            enemies = new List<Enemy>();
            enemyCount = GameManger.random.Next(7, 13);
            fieldInfo = new field_info[_FIELD_SIZE, _FIELD_SIZE];
            fogInfo = new float[_FIELD_SIZE, _FIELD_SIZE];

            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fogInfo[y, x] = 0;
                    fieldInfo[y, x] = field_info.empty;
                }
            }

            for (int i = 0; i < 60; i++)
            {
                int x = GameManger.random.Next(_FIELD_SIZE);
                int y = GameManger.random.Next(_FIELD_SIZE);
                if (GameManger.random.Next(100) > 60)
                {
                    fieldInfo[y, x] = field_info.tree;
                }
                else
                {
                    fieldInfo[y, x] = field_info.mud;
                }
            }

            while (true)
            {
                GameManger.player.Axis2D.x = GameManger.random.Next(_FIELD_SIZE / 3);
                GameManger.player.Axis2D.y = GameManger.random.Next(2, _FIELD_SIZE - 2);
                if (fieldInfo[GameManger.player.Axis2D.y, GameManger.player.Axis2D.x] != field_info.tree)
                {
                    break;
                }
            }

            for (int i = 0; i < enemyCount; i++)
            {
                while (true)
                {
                    int enemyX = GameManger.random.Next(_FIELD_SIZE / 2, _FIELD_SIZE);
                    int enemyY = GameManger.random.Next(2, _FIELD_SIZE - 2);
                    if (fieldInfo[enemyY, enemyX] != field_info.tree && enemies.Find(x => x.Axis2D.x == enemyX && x.Axis2D.y == enemyY) == null)
                    {
                        Enemy enemy = new Enemy();
                        enemy.Axis2D.x = enemyX;
                        enemy.Axis2D.y = enemyY;
                        enemies.Add(enemy);
                        break;
                    }
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
                    enemies[i].StartBattleTimer();
                }
                //enemies[i].enemyTimer.Change(500, 500);
            }
        }

        public override List<Enemy> GetEnemies()
        {
            return enemies;
        }
        public override bool Move()
        {
            bool isStun = false;

            int beforeX = GameManger.player.Axis2D.x;
            int beforeY = GameManger.player.Axis2D.y;

            int currX = GameManger.player.Hold(GameManger.player.GetNextX(GameManger.player.direction), FieldBase._FIELD_SIZE);
            int currY = GameManger.player.Hold(GameManger.player.GetNextY(GameManger.player.direction), FieldBase._FIELD_SIZE);


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

            //빈칸으로 이동
            if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.empty ||
                GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.road)
            {
                GameManger.player.MoveAndHold(GameManger.player.direction, FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);
            }

            //수풀로 이동
            if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.mud)
            {
                if (GameManger.player.MoveAndHold(GameManger.player.direction, FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE))
                {
                    if (GameManger.random.Next(1, 101) <= 10)
                    {
                        GameManger.player.direction = 4;
                        isStun = true;
                    }
                }
            }
            //벽으로 이동
            else if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.tree ||
                     GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.wall)
            {
            }
            if (currX != beforeX || currY != beforeY)
            {
                for (int i = 0; i < GameManger.currField.GetEnemies().Count; i++)
                {
                    GameManger.currField.GetEnemies()[i].isMove = true;
                }
            }

            //적에게 이동
            Enemy enemy = GameManger.currField.FindEnemiesAt(currX, currY);
            if (enemy != null && enemy.isLive)
            {
                GameManger.player.Axis2D.x = beforeX;
                GameManger.player.Axis2D.y = beforeY;
                //GameManger.currField.GetEnemies().RemoveAll(x => x.Axis2D.x == enemy.Axis2D.x && x.Axis2D.y == enemy.Axis2D.y);
            }
            GameManger.player.RemoveFog();

            return isStun;
        }

        public override string[] ConvertMapToString(ref string[] line)
        {

            for (int y = 0; y < FieldBase._FIELD_SIZE; y++)
            {
                line[y] = "";
                for (int x = 0; x < FieldBase._FIELD_SIZE; x++)
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

                            if (!tmp.isAttack)
                            {
                                line[y] += "적.";
                            }
                            else
                            {
                                line[y] += "喝.";
                                tmp.isAttack = false;
                            }
                        }
                        else if (!tmp.isLive)
                        {
                            line[y] += ".1.？.";
                        }
                        continue;
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


                    if (fogInfo[y, x] == 1)
                    {
                        line[y] += ".0.";
                    }
                    else if (fogInfo[y, x] >= 0 && fogInfo[y, x] < 1)
                    {
                        line[y] += ".6.";
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
                        case FieldBase.field_info.road:
                            line[y] += "□.";
                            break;
                        case FieldBase.field_info.wall:
                            line[y] += "■.";
                            break;
                    }

                }

                line[y] += "  ";
            }

            return line;
        }
    }
}
