using Project1st.Game.Algorithm;
using Project1st.Game.Core;
using Project1st.Game.Map;
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
        static public int EnemyHitPointMAX = 100;

        public Timer moveTimer;

        List<Location> path;
        public bool isMove;

        public Enemy()
        {
            base.Init();
            hitPoint = EnemyHitPointMAX;
            ID = 0;
            isMove = true;
            path = new List<Location>();

            StartTimer();
        }
        public Enemy(int x, int y)
        {
            base.Init();
            hitPoint = EnemyHitPointMAX;
            ID = 0;
            isMove = true;
            path = new List<Location>();
            Axis2D.x = x;
            Axis2D.y = y;

            StartTimer();
        }


        public void StartTimer()
        {
            moveTimer = new Timer(MoveTimer, null, 3000, 600 - (10 * (EnemyHitPointMAX - hitPoint)));

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

            if (GameManger.currField.GetElementAt(next.X, next.Y) != Field.field_info.wall &&
                GameManger.currField.FindEnemiesAt(next.X, next.Y) == null)
            {
                this.Axis2D.x = next.X;
                this.Axis2D.y = next.Y;
                path.RemoveAt(0);
            }
            else
            {
            }

            if (GameManger.player.Axis2D.x == this.Axis2D.x && GameManger.player.Axis2D.y == this.Axis2D.y)
            {
                GameManger.player.hitPoint -= 1;
                moveTimer.Dispose();
                if (GameManger.player.hitPoint == 0)
                {
                    GameManger.player.isLive = false;
                    GameManger.currField.GetEnemies().RemoveAll(x => x.Axis2D.x == this.Axis2D.x && x.Axis2D.y == this.Axis2D.y);
                    return;
                }
                GameManger.currField.GetEnemies().RemoveAll(x => x.Axis2D.x == this.Axis2D.x && x.Axis2D.y == this.Axis2D.y);
            }
        }
    }
}
