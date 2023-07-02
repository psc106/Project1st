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
    //적 클래스
    public class Enemy : MoveObject
    {
        public static readonly int _ENEMY_HITPOINT_MAX = 200;

        //이동 타이머
        public Timer moveTimer;
        //공격 딜레이 타이머
        public Timer delayTimer;

        //이동할 경로 - a*
        List<Location> path;


        public bool isPlayerMove;       //플레이어가 이동했는지 판별(알고리즘 재설정)
        public bool isDelay;            //현재 공격 딜레이 상태인지 판별(공격 처리)
        public bool isAttack;           //현재 공격상태인지 판별(이미지 출력)

        //마차나 플레이어를 타겟으로 한다.
        public ObjectBase target;   

        //전투씬일 경우 이 생성자를 부른다.
        public Enemy()
        {
            base.Init();

            isDelay = false;
            isAttack = false;
            isPlayerMove = true;
            isLive = true;
            
            hitPoint = _ENEMY_HITPOINT_MAX - GameManger.random.Next(100);
            attckPoint = GameManger.random.Next(2,11);
            
            ID = 0;
            
            path = new List<Location>();

            target = GameManger.player;

            //전투씬에서 마차를 구현하지못하여서 주석처리하였음
            //SetTarget();

            target = GameManger.player;

            StartBattleTimer();
        }//[Enemy()] end


        //숲 씬일 경우 이 생성자를 부른다.
        public Enemy(int x, int y)
        {
            base.Init();

            isDelay = false;
            isAttack = false;
            isPlayerMove = true;
            isLive = true;

            hitPoint = _ENEMY_HITPOINT_MAX - GameManger.random.Next(100);
            attckPoint = 35;

            ID = 0;

            path = new List<Location>();

            axis.x = x;
            axis.y = y;

            SetTarget();

            StartForestTimer();
        }// [Enemy(int x, int y)] end


        void SetTarget()
        {

            //70%확률로 플레이어
            //30%확률로 마차
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
        public void StartForestTimer(int time)
        {
            moveTimer = new Timer(SetMoveTimer, null, time, 400);
        }
        public void StartBattleTimer()
        {
            moveTimer = new Timer(SetFightTimer, null, 0, 600 - (10 * ((_ENEMY_HITPOINT_MAX - hitPoint)/_ENEMY_HITPOINT_MAX)));
        }

        //적 이동 메서드(숲씬)
        public void SetMoveTimer(object obj)
        {
            //적 이동 활성화
            if (!isLive)
            {
                isLive = true;
            }

            //플레이어가 이동할 경우
            if (isPlayerMove)
            {
                //타겟이 파괴된 상태면 타겟 재설정
                if (target == null)
                {
                    SetTarget();
                }
                //a*알고리즘으로 경로 설정
                path = EAlgorithm.Go(this);
                isPlayerMove = false;
            }

            //만약 경로가 제대로 지정이 안될경우 정지 상태
            if (path == null || path.Count == 0) return;

            //플레이어가 이동하기 전까지 경로를 차례로 이동한다.
            Location next = path[0];

            //다음 경로가 비어있을 경우 이동한다.
            if (GameManger.currField.GetElementAt(next.X, next.Y) != FieldBase.field_info.wall &&
                GameManger.currField.FindEnemiesAt(next.X, next.Y) == null)
            {
                this.axis.x = next.X;
                this.axis.y = next.Y;
                path.RemoveAt(0);
            }

            //활성화된 상태
            if (isLive)
            {
                //마차와 붙을 경우
                List<Wagon> wagon = GameManger.player.wagonList.FindAll(tmp => tmp.axis.x == this.axis.x && tmp.axis.y == this.axis.y);
                if (wagon != null && wagon.Count > 0)
                {
                    for (int i = 0; i < wagon.Count; i++)
                    {
                        //마차 체력 감소
                        wagon[i].hitPoint -= attckPoint;
                        //마차 체력 0일시 파괴
                        //파괴시 마차안에 있던 아이템도 모두 파괴
                        if (wagon[i].hitPoint <= 0)
                        {
                            GameManger.player.wagonList.Remove(wagon[i]);
                            continue;
                        }

                        //파괴 안될 시 마차안에 있던 랜덤 아이템 내구도 감소 
                        if (wagon[i].inventory.Count > 0)
                        {
                            Items item = wagon[i].inventory[GameManger.random.Next(wagon[i].inventory.Count)];
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

                    //해당 적은 부딫칠 경우 소멸.
                    moveTimer.Dispose();
                    GameManger.currField.GetEnemies().RemoveAll(x => x.axis.x == this.axis.x && x.axis.y == this.axis.y);
                    
                    return;
                }


                //플레이어와 붙을 경우
                if (GameManger.player.axis.x == this.axis.x && GameManger.player.axis.y == this.axis.y)
                {
                    //현재 씬 exit
                    GameManger.currField.Exit();

                    //해당 적 소멸.
                    GameManger.currField.GetEnemies().RemoveAll(x => x.axis.x == this.axis.x && x.axis.y == this.axis.y);

                    //배틀 씬으로 변환
                    new Battle();
                    return;
                }
            }
        }//[SetMoveTimer] end


        //적 전투 메서드(전투 씬)
        //위의 적 이동 메서드와 상당 부분 동일함.
        public void SetFightTimer(object obj)
        {
            if (!isLive)
            {
                return;
            }

            if (isPlayerMove)
            {
                path = EAlgorithm.Go(this);
                isPlayerMove = false;
            }
            if (path == null || path.Count == 0) return;
            Location next = path[0];

            if (GameManger.currField.GetElementAt(next.X, next.Y) != FieldBase.field_info.wall &&
                GameManger.currField.FindEnemiesAt(next.X, next.Y) == null &&
                (GameManger.player.axis.x != next.X || GameManger.player.axis.y != next.Y))
            {
                this.axis.x = next.X;
                this.axis.y = next.Y;
                path.RemoveAt(0);
            }

            //플레이어와 붙을 시
            else if(GameManger.player.axis.x==next.X && GameManger.player.axis.y == next.Y)
            {
                //현재 공격 딜레이가 없다면
                if (!isDelay)
                {
                    //공격상태 설정
                    isAttack = true;
                    isDelay = true;

                    //딜레이 설정
                    delayTimer = new Timer(DelayTimer, null, 600 - (300 * ((_ENEMY_HITPOINT_MAX - hitPoint) / _ENEMY_HITPOINT_MAX)),100);

                    //체력 감소
                    GameManger.player.hitPoint -= attckPoint;
                    //플레이어 체력이 0이 되면
                    if (GameManger.player.hitPoint<=0)
                    {
                        GameManger.player.isLive = false;
                        //게임오버 엔딩
                        new Ending(1);
                    }
                }
            }
        }//[SetFightTimer] end


        public void DelayTimer(object obj)
        {
            isDelay = false;
            delayTimer.Dispose();
        }
    }
}
