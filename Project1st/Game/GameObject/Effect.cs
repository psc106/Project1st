using Project1st.Game.Core;
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
    //MoveObject 상속.
    public class Effect : MoveObject
    {
        //이펙트 표현시 출력할 문자 - 해당 클래스의 type변수와 연관
        public static readonly string[] _EFFECT_STRING = { "／", "⇔", "Δ", "€", "˚" };

        //이펙트 타입
        public int type;
        
        //텍스트 출력 판별
        public bool isRemove;

        //원거리 공격일 경우 움직이기 때문에 timer를 지정해줘야한다.
        public Timer bulletTimer;

        public Effect()
        {
            type = 0;
        }

        public Effect(int type)
        {
            this.type = type;
        }

        //근접 공격
        public Effect(int x, int y, int type)
        {
            axis.x = x;
            axis.y = y;
            this.type = type;
        }

        //원거리 공격
        public Effect(int x, int y, int type, int direction)
        {
            axis.x = x;
            axis.y = y;
            this.type = 4;
            this.direction = direction;
            bulletTimer = new Timer(SetBulletTimer, null, 1, 150);
            isRemove = false;
        }


        //150ms 마다 출력되는 총알 이동 메서드
        public void SetBulletTimer(object obj)
        {
            int nextX = this.GetNextX(direction);
            int nextY = this.GetNextY(direction);

            //벽, 외곽에 부딪칠 경우
            if (nextX == -1 || nextY == -1 || nextX == FieldBase._FIELD_SIZE || nextY == FieldBase._FIELD_SIZE||
                GameManger.currField.GetElementAt(nextX, nextY) == FieldBase.field_info.tree ||
                GameManger.currField.GetElementAt(nextX, nextY) == FieldBase.field_info.wall)
            {
                //총알 삭제
                bulletTimer.Dispose();
                GameManger.player.bulletCount -= 1;
                isRemove = true;

                return;
            }

            //빈 칸일 경우 이동
            if ((GameManger.currField.GetElementAt(nextX, nextY) != FieldBase.field_info.road ||
                 GameManger.currField.GetElementAt(nextX, nextY) == FieldBase.field_info.empty) &&
                 GameManger.currField.FindEnemiesAt(nextX, nextY) == null)
            {
                this.axis.x = nextX;
                this.axis.y = nextY;
                return;
            }

            //적일 경우
            Enemy currEnemy = GameManger.currField.FindEnemiesAt(nextX, nextY);
            if (currEnemy != null)
            {
                //적이 활성화 되있는 상태일 경우
                if (currEnemy.isLive)
                {
                    //적 체력 감소
                    currEnemy.hitPoint -= GameManger.player.attckPoint;
                    currEnemy.moveTimer.Change(300, 600 - (10 * ((Enemy._ENEMY_HITPOINT_MAX - currEnemy.hitPoint) / Enemy._ENEMY_HITPOINT_MAX)));
                    
                    //총알 삭제
                    bulletTimer.Dispose();
                    GameManger.player.bulletCount -= 1;
                    isRemove = true;

                    //적 체력이 다 닳을 경우
                    if (currEnemy.hitPoint <= 0)
                    {
                        //적 삭제
                        GameManger.currField.RemoveEnemy(nextX, nextY);

                        //적이 0명일 경우
                        if (GameManger.currField.GetEnemies().Count == 0)
                        {
                            //승리 이벤트
                            GameManger.currField.isWin = true;
                        }
                    }
                }
            }
        }//[SetBulletTimer] end

    }
}
