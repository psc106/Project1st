using Project1st.Game.Algorithm;
using Project1st.Game.Core;
using Project1st.Game.Item;
using Project1st.Game.Map;
using Project1st.Game.Map.Fields;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.GameObject
{
    public class Enemy : MoveObject
    {
        public static readonly int _ENEMY_HITPOINT_MAX = 200;

        public Timer moveTimer;
        public Timer delayTimer;

        List<Location> path;
        public bool isMove;
        public bool isDelay;
        public bool isAttack;

        public ObjectBase target;

        public Enemy()
        {
            base.Init();
            isDelay = false;
            isAttack = false;
            isMove = true;
            isLive = true;
            hitPoint = _ENEMY_HITPOINT_MAX - GameManger.random.Next(100);
            attckPoint = GameManger.random.Next(2,11);
            ID = 0;
            path = new List<Location>();

            if (GameManger.random.Next(100) >= 70)
            {
                target = GameManger.player;
            }
            else
            {
                /*if (GameManger.player.wagonList!=null && GameManger.player.wagonList.Count > 0)
                {
                    target = GameManger.player.wagonList[GameManger.random.Next(GameManger.player.wagonList.Count)];
                }
                else*/
                {
                    target = GameManger.player;
                }
            }
            StartBattleTimer();
        }
        public Enemy(int x, int y)
        {
            base.Init();
            isDelay = false;
            isAttack = false;
            isMove = true;
            isLive = true;
            hitPoint = _ENEMY_HITPOINT_MAX - GameManger.random.Next(100);
            attckPoint = 35;
            ID = 0;
            path = new List<Location>();
            Axis2D.x = x;
            Axis2D.y = y;

            if (GameManger.random.Next(100) >= 70)
            {
                target = GameManger.player;
            }
            else
            {
                if (GameManger.player.wagonList != null && GameManger.player.wagonList.Count > 0)
                {
                    target = GameManger.player.wagonList[GameManger.random.Next(GameManger.player.wagonList.Count)];
                }
                else
                {
                    target = GameManger.player;
                }
            }

            StartForestTimer();
        }

        void SetTarget()
        {
            if (GameManger.random.Next(100) >= 70)
            {
                target = GameManger.player;
            }
            else
            {
                if (GameManger.player.wagonList != null && GameManger.player.wagonList.Count > 0)
                {
                    target = GameManger.player.wagonList[GameManger.random.Next(GameManger.player.wagonList.Count)];
                }
                else
                {
                    target = GameManger.player;
                }
            }
        }


        public void StartForestTimer()
        {
            moveTimer = new Timer(SetMoveTimer, null, 2000, 400);

        }
        public void StartBattleTimer()
        {
            moveTimer = new Timer(SetFightTimer, null, 0, 600 - (400 * ((_ENEMY_HITPOINT_MAX - hitPoint)/_ENEMY_HITPOINT_MAX)));

        }

        public void SetMoveTimer(object obj)
        {
            if (!isLive)
            {
                isLive = true;
            }

            if (isMove)
            {
                if (target == null)
                {
                    SetTarget();
                }
                path = EAlgorithm.Go(this);
                isMove = false;
            }
            if (path == null || path.Count == 0) return;
            Location next = path[0];

            if (GameManger.currField.GetElementAt(next.X, next.Y) != FieldBase.field_info.wall &&
                GameManger.currField.FindEnemiesAt(next.X, next.Y) == null)
            {
                this.Axis2D.x = next.X;
                this.Axis2D.y = next.Y;
                path.RemoveAt(0);
            }

            if (isLive)
            {
                List<Wagon> wagon = GameManger.player.wagonList.FindAll(tmp => tmp.Axis2D.x == this.Axis2D.x && tmp.Axis2D.y == this.Axis2D.y);
                //마차와 붙을 경우
                if (wagon != null && wagon.Count > 0)
                {
                    for (int i = 0; i < wagon.Count; i++)
                    {
                        wagon[i].hitPoint -= attckPoint;
                        if (wagon[i].hitPoint <= 0)
                        {
                            GameManger.player.wagonList.Remove(wagon[i]);
                            continue;
                        }

                        if (GameManger.player.inventory.Count > 0)
                        {
                            Items item = GameManger.player.inventory[GameManger.random.Next(GameManger.player.inventory.Count)];
                            if (item.itemId >= 10)
                            {
                                item.quality -= 0.01f * GameManger.random.Next(5);
                                if (item.quality < 0)
                                {
                                    item.quality = 0;
                                }
                            }
                        }
                    }

                    moveTimer.Dispose();
                    GameManger.currField.GetEnemies().RemoveAll(x => x.Axis2D.x == this.Axis2D.x && x.Axis2D.y == this.Axis2D.y);
                    return;
                }


                //플레이어와 붙을 경우
                if (GameManger.player.Axis2D.x == this.Axis2D.x && GameManger.player.Axis2D.y == this.Axis2D.y)
                {

                    GameManger.currField.Exit();
                    GameManger.currField.GetEnemies().RemoveAll(x => x.Axis2D.x == this.Axis2D.x && x.Axis2D.y == this.Axis2D.y);

                    new Battle();
                    return;
                }
            }
        }

        public void SetFightTimer(object obj)
        {
            if (!isLive)
            {
                return;
            }

            if (isMove)
            {
                path = EAlgorithm.Go(this);
                isMove = false;
            }
            if (path == null || path.Count == 0) return;
            Location next = path[0];

            if (GameManger.currField.GetElementAt(next.X, next.Y) != FieldBase.field_info.wall &&
                GameManger.currField.FindEnemiesAt(next.X, next.Y) == null &&
                (GameManger.player.Axis2D.x != next.X || GameManger.player.Axis2D.y != next.Y))
            {
                this.Axis2D.x = next.X;
                this.Axis2D.y = next.Y;
                path.RemoveAt(0);
            }

            else if(GameManger.player.Axis2D.x==next.X && GameManger.player.Axis2D.y == next.Y)
            {
                if (!isDelay)
                {
                    isAttack = true;
                    isDelay = true;
                    delayTimer = new Timer(DelayTimer, null, 600 - (300 * ((_ENEMY_HITPOINT_MAX - hitPoint) / _ENEMY_HITPOINT_MAX)),100);

                    GameManger.player.hitPoint -= attckPoint;
                    if (GameManger.player.hitPoint<=0)
                    {
                        GameManger.player.isLive = false;
                        //게임오버 엔딩
                        new Ending(1);
                    }
                }
            }
        }


        public void DelayTimer(object obj)
        {
            isDelay = false;
            delayTimer.Dispose();
        }
    }
}
