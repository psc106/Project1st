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

        public static readonly float _PLAYER_WEIGHT_MAX = 50;
        public static readonly int _PLAYER_HITPOINT_MAX = 99900;

        public float maxWeightSum;
        public float weight;

        public List<Items> inventory;
        public int startInvenIndex;

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

        //전투하는 플레이어 생성
        public Player(Player player)
        {
            this.equip = player.equip;
            this.weapon = player.weapon;
            this.attckPoint = player.attckPoint;
            this.hitPoint = player.hitPoint;
            this.isMeleeDelay = false;
            this.isRangeDelay = false;
            this.bulletCountMax = 3;
            this.light = player.light;
            this.Effects = new List<Effect>();
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
            hitPoint = _PLAYER_HITPOINT_MAX;
            maxWeightSum = _PLAYER_WEIGHT_MAX;
            ID = 0;
            isMeleeDelay = false;
            isRangeDelay = false;
            weight = 0;
            gold = 10000;
            bulletCountMax = 3;
            weapon = new Items();
            //원거리 공격력
            attckPoint = 25;
            light = 5;
            walk = 0;
            inventory = new List<Items>();
            Effects = new List<Effect>();
            wagonList = new List<Wagon>();

            startInvenIndex = 0;
        }
        public void RemoveFog()
        {
            int tmpLight = FogView.GetCurrTimeLight(this.light);

            Queue<FogView> q = new Queue<FogView>();

            q.Enqueue(new FogView(GameManger.player.Axis2D, 0));
            GameManger.currField.SetFogInfo(GameManger.player.Axis2D.x, GameManger.player.Axis2D.y, 1);

            for (int i = 0; i < FogView.GetRealView(tmpLight); i++)
            {
                if (q == null || q.Count == 0) return;
                FogView tmpFog = q.Dequeue();

                if (tmpFog.depth == tmpLight)
                {
                    return;
                }

                for (int j = 0; j < 4; j++)
                {
                    int tmpX = (tmpFog.x + _OBJECT_AXIS_MATRIX_X[j]);
                    int tmpY = (tmpFog.y + _OBJECT_AXIS_MATRIX_Y[j]);
                    

                    if ((tmpX<= -1 || tmpY <= -1 || tmpX >= FieldBase._FIELD_SIZE || tmpY >= FieldBase._FIELD_SIZE))
                    {
                        continue;
                    }
                    if (GameManger.currField.GetFogInfo(tmpX, tmpY) == 1)
                    {
                        continue;
                    }
                    GameManger.currField.SetFogInfo(tmpX, tmpY, 1);
                    q.Enqueue(new FogView(tmpX, tmpY, tmpFog.depth+1));
                }
            }
        }

        public void RemoveFog(int light)
        {
            Queue<FogView> q = new Queue<FogView>();

            q.Enqueue(new FogView(GameManger.player.Axis2D, 0));
            GameManger.currField.SetFogInfo(GameManger.player.Axis2D.x, GameManger.player.Axis2D.y, 1);

            for (int i = 0; i < FogView.GetRealView(light); i++)
            {
                if (q == null || q.Count == 0) return;
                FogView tmpFog = q.Dequeue();

                if (tmpFog.depth == light)
                {
                    return;
                }

                for (int j = 0; j < 4; j++)
                {
                    int tmpX = (tmpFog.x + _OBJECT_AXIS_MATRIX_X[j]);
                    int tmpY = (tmpFog.y + _OBJECT_AXIS_MATRIX_Y[j]);


                    if ((tmpX <= -1 || tmpY <= -1 || tmpX >= FieldBase._FIELD_SIZE || tmpY >= FieldBase._FIELD_SIZE))
                    {
                        continue;
                    }
                    if (GameManger.currField.GetFogInfo(tmpX, tmpY) == 1)
                    {
                        continue;
                    }
                    GameManger.currField.SetFogInfo(tmpX, tmpY, 1);
                    q.Enqueue(new FogView(tmpX, tmpY, tmpFog.depth + 1));
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
