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
        public static readonly float _PLAYER_WEIGHT_MAX = 50;   //플레이어 최대 무게
        public static readonly int _PLAYER_HITPOINT_MAX = 200;  //플레이어 최대 hp
        public static readonly int _PLAYER_BULLET_COUNT_MAX = 3;

        public IUseItem use;        //use 인터페이스
        public IEquipItem equip;    //equip 인터페이스

        public int gold;            //골드
        public int light;           //시야
        public int walk;            //아이템 등불 사용 시간

        public Items weapon;        //현재 무기
        public Timer meleeDelay;    //근접공격 딜레이 타이머
        public Timer rangeDelay;    //원거리공격 딜레이 타이머

        public List<Effect> Effects;    //각종 이펙트 저장
        
        public bool isMeleeDelay;   //근접공격 딜레이 유무
        public bool isRangeDelay;   //원거리공격 딜레이 유무
        
        public int bulletCount;     //현재 사용한 총알 갯수

        public List<Items> inventory;   //인벤토리
        public List<Wagon> wagonList;   //마차 리스트
        public float weight;            //플레이어 현재 무게

        public float maxWeightSum;      //플레이어 + 마차 무게 MAX 합

        public int startInvenIndex;     //플레이어 현재 인벤토리 커서 위치

        public Player()
        {
            Init();
        }
        public Player(int x, int y)
        {
            Init();
            axis.x = x;
            axis.y = y;

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
            this.light = player.light;
            this.Effects = new List<Effect>();
        }

        //현재 플레이어 + 각 마차 인벤토리 무게 합 
        public float SumWeight()
        {
            float sum = weight;
            foreach (var tmp in wagonList)
            {
                sum += tmp.weight;
            }

            return sum;
        }

        //초기화
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
            weapon = new Items();
            //원거리 공격력
            attckPoint = 25;
            light = 4;
            walk = 0;
            inventory = new List<Items>();
            Effects = new List<Effect>();
            wagonList = new List<Wagon>();

            startInvenIndex = 0;
        }

        /// <summary>
        /// 시야 값에 맞춰서 마름모꼴로 안개가 걷히는 메소드
        /// </summary>
        public void RemoveFog()
        {
            //계산된 시야 저장(밤/낮, 등불 아이템에 따라 달라짐)
            int tmpLight = FogView.GetCurrTimeLight(this.light);

            //큐를 사용하여 각 DEPTH를 차례로 저장하여 순서대로 POP하면 depth가 자동으로 정렬되서 나온다.
            Queue<FogView> q = new Queue<FogView>();

            //큐의 초기값 = 플레이어의 현재 위치
            q.Enqueue(new FogView(GameManger.player.axis, 0));
            GameManger.currField.SetFogInfo(GameManger.player.axis.x, GameManger.player.axis.y, 1);

            //특수한 계산식으로 구해낸 수 만큼 반복한다.
            for (int i = 0; i < FogView.GetRealView(tmpLight); i++)
            {
                //만약 fog처리가 겹쳐서 1이된 상태에서 또 포그 처리될경우 제대로 값이 저장되지않기 때문에 이를 처리해준다.
                if (q == null || q.Count == 0) return;

                //pop
                FogView tmpFog = q.Dequeue();

                //시야가 외곽에 겹쳐서 반복할 횟수가 남을 경우, depth가 특정한 값에 도달하면 바로 종료한다. 
                if (tmpFog.depth == tmpLight)
                {
                    return;
                }

                //현재 위치를 기준으로 동,서,남,북 처리한다.
                for (int j = 0; j < 4; j++)
                {
                    int tmpX = (tmpFog.x + _OBJECT_AXIS_MATRIX_X[j]);
                    int tmpY = (tmpFog.y + _OBJECT_AXIS_MATRIX_Y[j]);
                    
                    //외곽을 넘으면 저장하지않는다.
                    if ((tmpX<= -1 || tmpY <= -1 || tmpX >= FieldBase._FIELD_SIZE || tmpY >= FieldBase._FIELD_SIZE))
                    {
                        continue;
                    }

                    //이미 처리한 장소면 저장하지않는다.
                    if (GameManger.currField.GetFogInfo(tmpX, tmpY) == 1)
                    {
                        continue;
                    }

                    //depth를 1 올려서 push
                    GameManger.currField.SetFogInfo(tmpX, tmpY, 1);
                    q.Enqueue(new FogView(tmpX, tmpY, tmpFog.depth+1));
                }
            }
        }//[RemoveFog()] end


        //위의 RemoveFog()와 모든 부분이 동일하지만 플레이어의 시야와 상관없이 고정된 시야를 밝혀준다.
        public void RemoveFog(int light)
        {
            if (light == 0)
            {
                for (int y = 0; y < FieldBase._FIELD_SIZE; y++)
                {
                    for (int x = 0; x < FieldBase._FIELD_SIZE; x++)
                    {
                        //이전에 밝혔던 값들을 전부 방문했지만 현재는 밝히지 않았던 걸로 처리
                        //0 방문x / 0.5 방문o현재x / 1방문o현재o
                        if (GameManger.currField.GetFogInfo(x, y) == 1)
                        {
                            GameManger.currField.SetFogInfo(x, y, 0.5f);
                        }

                    }
                }
            }
            Queue<FogView> q = new Queue<FogView>();

            q.Enqueue(new FogView(GameManger.player.axis, 0));
            GameManger.currField.SetFogInfo(GameManger.player.axis.x, GameManger.player.axis.y, 1);

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
        }//[RemoveFog(int light)] end


        //딜레이 처리 타이머 메서드
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
