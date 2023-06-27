using Project1st.Game.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.GameObject
{
    public class Player : MoveObject
    {
        public Timer attackTimer;

        public int score;
        public bool isCoolTime;
        public int weapon;
        public int viewCount;

        public Player()
        {
            Init();
        }
        public Player(int x, int y)
        {
            Init();
            Axis2D.x = x;
            Axis2D.y = y;
        }

        public override void Init()
        {
            base.Init();
            hitPoint = 100;
            ID = 0;
            isCoolTime = false;
            score = 0;
            weapon = 1;
            viewCount = 5;
        }
    }
}
