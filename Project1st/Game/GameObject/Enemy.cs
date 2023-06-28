using Project1st.Game.Algorithm;
using Project1st.Game.Core;
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
        static public int EnemyHitPointMAX = 30;

        public Timer moveTimer;
        public Timer delayTimer;

        List<Location> path;
        public bool isMove;
        public bool isDelay;
        public bool isAttack;


        public Enemy()
        {
            base.Init();
            isDelay = false;
            isAttack = false;
            isMove = true;
            isLive = true;
            hitPoint = EnemyHitPointMAX;
            attckPoint = 1;
            ID = 0;
            path = new List<Location>();

            StartBattleTimer();
        }
        public Enemy(int x, int y)
        {
            base.Init();
            isDelay = false;
            isAttack = false;
            isMove = true;
            isLive = true;
            hitPoint = EnemyHitPointMAX;
            attckPoint = 0;
            ID = 0;
            path = new List<Location>();
            Axis2D.x = x;
            Axis2D.y = y;

            StartForestTimer();
        }


        public void StartForestTimer()
        {
            moveTimer = new Timer(MoveTimer, null, 2000, 400);

        }
        public void StartBattleTimer()
        {
            moveTimer = new Timer(FightTimer, null, 3000, 600 - (400 * ((EnemyHitPointMAX - hitPoint)/EnemyHitPointMAX)));

        }

        public void MoveTimer(object obj)
        {
            if (!isLive)
            {
                isLive = true;
            }

            if (isMove)
            {
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
            else
            {
            }

            //플레이어와 붙을 경우
            if (GameManger.player.Axis2D.x == this.Axis2D.x && GameManger.player.Axis2D.y == this.Axis2D.y)
            {
                GameManger.currField.StopEnemies();
                if (GameManger.currField.GetCreateTimer() != null)
                {
                    GameManger.currField.GetCreateTimer().Dispose();
                }

                moveTimer.Dispose();
                GameManger.currField.GetEnemies().RemoveAll(x => x.Axis2D.x == this.Axis2D.x && x.Axis2D.y == this.Axis2D.y);

                //Utility.currRoom.enemyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                new Battle(GameManger.currField.ReturnSelfToForest());
            }
        }

        public void FightTimer(object obj)
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
                    delayTimer = new Timer(DelayTimer, null, 600 - (300 * ((EnemyHitPointMAX - hitPoint) / EnemyHitPointMAX)),100);

                    GameManger.player.hitPoint -= 1;
                    if (GameManger.player.hitPoint == 0)
                    {
                        GameManger.player.isLive = false;
                        //게임오버 엔딩
                        //new Ending(GameManger.currField);
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
