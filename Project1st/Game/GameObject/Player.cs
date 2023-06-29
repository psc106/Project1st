using Project1st.Game.Core;
using Project1st.Game.Item;
using Project1st.Game.Map;
using Project1st.Game.Map.Fields;
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
        public Timer meleeDelay;
        public Timer rangeDelay;

        public int gold;
        public int light;
        public float weight;
        public float maxWeight;

        public int weapon;
        public int bulletCount;
        public int bulletCountMax;

        public bool isMeleeDelay;
        public bool isRangeDelay;

        public int hitPointMax;

        public List<Effect> Effects;

        public List<Wagon> wagonList;
        public List<Items> inventory;
        public int startInventoryIndex;

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
            hitPointMax = 100;
            hitPoint = 100;
            attckPoint = 10;
            ID = 0;
            isMeleeDelay = false;
            isRangeDelay = false;
            maxWeight = 50;
            weight = 0;
            gold = 230;
            bulletCountMax = 3;
            weapon = 0;
            light = 5;
            inventory = new List<Items>();
            Effects = new List<Effect>();
            wagonList = new List<Wagon>();

            startInventoryIndex = 0;
        }

        public void RemoveFog()
        {
            Queue<Coordinate> q = new Queue<Coordinate>();

            q.Enqueue(GameManger.player.Axis2D);
            GameManger.currField.SetFogInfo(GameManger.player.Axis2D.x, GameManger.player.Axis2D.y, 1);

            int light = GameManger.player.light;
            if (GameManger.map.day ==0)
            {
                light += 3;
            }
            else if (GameManger.map.day == 1)
            {
                light -= 2;
            }
            light = (((light - 1) * 2) - 2) * (light - 1) + 1;
            for (int i = 0; i < light; i++)
            {
                Coordinate tmp = q.Dequeue();

                if ((tmp.x <= -1 || tmp.y <= -1 || tmp.x >= FieldBase._FIELD_SIZE || tmp.y >= FieldBase._FIELD_SIZE))
                { }
                else
                {
                    GameManger.currField.SetFogInfo(tmp.x, tmp.y, 1);
                }
                for (int j = 0; j < 4; j++)
                {
                    int tmpX = (tmp.x + AXIS_X[j]);
                    int tmpY = (tmp.y + AXIS_Y[j]);
                    q.Enqueue(new Coordinate(tmpX, tmpY));
                }
            }

            foreach (var tmp in q)
            {
                if ((tmp.x <= -1 || tmp.y <= -1 || tmp.x >= FieldBase._FIELD_SIZE || tmp.y >= FieldBase._FIELD_SIZE))
                {
                    continue;
                }

                if (GameManger.currField.GetFogInfo(tmp.x, tmp.y) != 1)
                {
                    GameManger.currField.SetFogInfo(tmp.x, tmp.y, 1);
                }
            }
        }


        public void DelayMeleeTimer(object obj)
        {
            isMeleeDelay = false;
            meleeDelay.Dispose();
        }
        public void DelayRangeTimer(object obj)
        {
            isRangeDelay = false;
            rangeDelay.Dispose();
        }
    }
}
