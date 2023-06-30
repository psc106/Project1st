using Project1st.Game.Core;
using Project1st.Game.Interface;
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
        public IUseItem use;
        public IEquipItem equip;

        public Timer meleeDelay;
        public Timer rangeDelay;

        public int gold;
        public int light;
        public int walk;

        public Items weapon;
        public int bulletCount;
        public int bulletCountMax;

        public bool isMeleeDelay;
        public bool isRangeDelay;

        public List<Effect> Effects;

        public List<Wagon> wagonList;

        public static readonly float playerWeightMax = 50;
        public static readonly int hitPointMax = 100;

        public float maxWeightSum;
        public float weight;

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

        public float SumWeight()
        {
            float sum = weight;
            foreach (var tmp in wagonList)
            {
                sum += tmp.weight;
            }

            return sum;
        }

        public override void Init()
        {
            base.Init();
            hitPoint = hitPointMax;
            maxWeightSum = playerWeightMax;
            ID = 0;
            isMeleeDelay = false;
            isRangeDelay = false;
            weight = 0;
            gold = 10000;
            bulletCountMax = 3;
            weapon = new Items();
            attckPoint = weapon.weaponStr;
            light = 5;
            walk = 0;
            inventory = new List<Items>();
            Effects = new List<Effect>();
            wagonList = new List<Wagon>();

            startInventoryIndex = 0;
        }

        public int GetRealView()
        {
            int light = this.light;

            if (GameManger.map.day == 0)
            {
                light += 3;
            }
            else if (GameManger.map.day == 1)
            {
                light -= 2;
            }
            return (((light - 1) * 2) - 2) * (light - 1) + 1;
        }

        public void RemoveFog()
        {
            Queue<Coordinate> q = new Queue<Coordinate>();

            q.Enqueue(GameManger.player.Axis2D);
            GameManger.currField.SetFogInfo(GameManger.player.Axis2D.x, GameManger.player.Axis2D.y, 1);

            int light = GetRealView();

            
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
