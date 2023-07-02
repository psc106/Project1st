using Project1st.Game.Core;
using Project1st.Game.GameObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Map.Fields
{

    //전투 씬, 배틀 씬, 전투 필드
    public class Battle : FieldBase
    {
        //이전 필드와 플레이어의 값을 저장한다.
        public Player beforePlayerInfo;
        public FieldBase beforeFieldInfo;

        //적의 숫자
        public int enemyCount;
        //적의 정보
        public List<Enemy> enemies;
        //안개 정보
        public float[,] fogInfo;

        //생성자
        public Battle()
        {
            //필드 타입 지정
            type = FieldBase.field_type.battle;

            //이전 필드, 플레이어 정보 저장
            beforePlayerInfo = GameManger.player;
            beforeFieldInfo = GameManger.currField;

            //필드, 플레이어 정보 새로 작성
            GameManger.player = new Player(beforePlayerInfo);
            GameManger.currField = this;

            //적 설정
            enemies = new List<Enemy>();
            enemyCount = GameManger.random.Next(7, 13);

            //맵 정보
            fieldInfo = new field_info[_FIELD_SIZE, _FIELD_SIZE];
            fogInfo = new float[_FIELD_SIZE, _FIELD_SIZE];
            isWin = false;
            
            //맵 정보 초기화
            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fogInfo[y, x] = 0;
                    fieldInfo[y, x] = field_info.empty;
                }
            }

            //맵 정보 랜덤 설정
            for (int i = 0; i < 60; i++)
            {
                int x = GameManger.random.Next(_FIELD_SIZE);
                int y = GameManger.random.Next(_FIELD_SIZE);
                if (GameManger.random.Next(100) > 60)
                {
                    fieldInfo[y, x] = field_info.tree;
                }
                else
                {
                    fieldInfo[y, x] = field_info.mud;
                }
            }

            //플레이어 위치 설정
            while (true)
            {
                GameManger.player.axis.x = GameManger.random.Next(_FIELD_SIZE / 3);
                GameManger.player.axis.y = GameManger.random.Next(2, _FIELD_SIZE - 2);
                if (fieldInfo[GameManger.player.axis.y, GameManger.player.axis.x] != field_info.tree)
                {
                    break;
                }
            }


            //적 위치 설정
            for (int i = 0; i < enemyCount; i++)
            {
                while (true)
                {
                    int enemyX = GameManger.random.Next(_FIELD_SIZE / 2, _FIELD_SIZE);
                    int enemyY = GameManger.random.Next(2, _FIELD_SIZE - 2);
                    if (fieldInfo[enemyY, enemyX] != field_info.tree && enemies.Find(x => x.axis.x == enemyX && x.axis.y == enemyY) == null)
                    {
                        Enemy enemy = new Enemy();
                        enemy.axis.x = enemyX;
                        enemy.axis.y = enemyY;
                        enemies.Add(enemy);
                        break;
                    }
                }
            }
            GameManger.player.RemoveFog();
        }//[Battle()] 생성자 종료


        //적 정지
        public override void StopEnemies()
        {
            if (enemies == null) return;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].moveTimer != null)
                {
                    enemies[i].moveTimer.Dispose();
                }
                //enemies[i].enemyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
            return;
        }

        //적 이동 재시작
        public override void PlayEnemies()
        {
            if (enemies == null) return;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].moveTimer != null)
                {
                    enemies[i].StartBattleTimer();
                }
                //enemies[i].enemyTimer.Change(500, 500);
            }
        }

        //적 탐색
        public override Enemy FindEnemiesAt(int x, int y)
        {
            if (enemies == null) return null;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                if (enemies[i].axis.x == x && enemies[i].axis.y == y) return enemies[i];
            }
            return null;
        }

        //적 제거
        public override void RemoveEnemy(int x, int y)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                if (enemies[i].axis.x == x && enemies[i].axis.y == y)
                {
                    if (enemies[i].moveTimer != null)
                    {
                        enemies[i].moveTimer.Dispose();
                    }
                    enemies.RemoveAt(i);
                }
            }
        }

        //안개 정보 get
        public override float GetFogInfo(int x, int y)
        {
            return fogInfo[y, x];
        }
        //안개 정보 set
        public override void SetFogInfo(int x, int y, float info)
        {
            fogInfo[y, x] = info;
        }


        public override List<Enemy> GetEnemies()
        {
            return enemies;
        }

        //이동키 입력 처리
        public override bool PressMoveEvent()
        {
            bool isStun = false;

            //이전값 = 이동하기 전 좌표
            int beforeX = GameManger.player.axis.x;
            int beforeY = GameManger.player.axis.y;

            //현재값 = 이동한 후 좌표(외곽에 닿을시 이를 처리한 값)
            int currX = GameManger.player.Hold(GameManger.player.GetNextX(GameManger.player.direction), FieldBase._FIELD_SIZE);
            int currY = GameManger.player.Hold(GameManger.player.GetNextY(GameManger.player.direction), FieldBase._FIELD_SIZE);

            //빈칸으로 이동
            if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.empty ||
                GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.road)
            {
                //플레이어의 좌표를 재설정
                GameManger.player.MoveAndHold(GameManger.player.direction, FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);
            }

            //진흙으로 이동
            if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.mud)
            {
                //플레이어의 좌표를 재설정
                //만약 외곽에 걸릴 경우 처리하지않는다.
                if (GameManger.player.MoveAndHold(GameManger.player.direction, FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE))
                {
                    //함정 발동
                    if (GameManger.random.Next(1, 101) <= 10)
                    {
                        //입력할수없는 상태로 만든다.
                        GameManger.player.direction = 4;
                        isStun = true;
                    }
                }
            }

            //벽으로 이동 -> 아무런 효과도 없다.
            else if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.tree ||
                     GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.wall)
            {
            }

            //이전좌표와 현재 좌표가 다를 경우
            if (currX != beforeX || currY != beforeY)
            {
                //플레이어가 이동한걸로 판단하고 모든 적들의 경로를 재설정한다. 
                for (int i = 0; i < GameManger.currField.GetEnemies().Count; i++)
                {
                    GameManger.currField.GetEnemies()[i].isPlayerMove = true;
                }
            }

            //적에게 이동
            Enemy enemy = GameManger.currField.FindEnemiesAt(currX, currY);
            if (enemy != null && enemy.isLive)
            {
                //적을 넘어가지못한다.
                GameManger.player.axis.x = beforeX;
                GameManger.player.axis.y = beforeY;
            }

            return isStun;
        }//[PressMoveEvent] end


        //취소키 입력 처리
        //배틀 씬에서는 원거리 공격키와 같다.
        public override void PressNoEvent()
        {
            //방향을 지정한 경우만 발사된다.
            if (GameManger.player.direction >= 0 && GameManger.player.direction <= 3)
            {
                //총알을 필드상에 일정수 만큼만 존재할수있다.
                if (GameManger.player.bulletCount < Player._PLAYER_BULLET_COUNT_MAX)
                {
                    //딜레이가 없을시
                    if (!GameManger.player.isRangeDelay)
                    {
                        //딜레이 처리
                        GameManger.player.isRangeDelay = true;
                        GameManger.player.rangeDelay = new Timer(GameManger.player.DelayRangeTimer, null, 300, 0);

                        //발사
                        GameManger.player.Effects.Add(new Effect(GameManger.player.axis.x, GameManger.player.axis.y, 1, GameManger.player.direction));
                        GameManger.player.bulletCount += 1;
                    }
                }
            }
        }//[PressNoEvent] end

        //확인키 입력 처리
        //배틀씬에서는 근접 공격키와 같다.
        public override bool PressYesEvent()
        {
            //딜레이가 없을 경우
            if (!GameManger.player.isMeleeDelay)
            {
                //딜레이 처리
                GameManger.player.isMeleeDelay = true;
                GameManger.player.meleeDelay = new Timer(GameManger.player.DelayMeleeTimer, null, GameManger.player.weapon.weaponDelay, 0);

                //공격 방향 지정
                int nextX = GameManger.player.GetNextX(GameManger.player.direction);
                int nextY = GameManger.player.GetNextY(GameManger.player.direction);

                //지정한 좌표에 이펙트를 생성한다.
                //상하좌우 1곳만 공격
                if (GameManger.player.weapon.weaponType == 0)
                {
                    GameManger.player.Effects.Add(new Effect(nextX, nextY, GameManger.player.weapon.weaponType));
                }

                //상하좌우+해당 방향의 양옆
                if (GameManger.player.weapon.weaponType == 1)
                {
                    GameManger.player.Effects.Add(new Effect(nextX, nextY, GameManger.player.weapon.weaponType));
                    if (GameManger.player.direction / 2 == 0)//양쪽 보는중
                    {
                        GameManger.player.Effects.Add(new Effect(nextX, nextY + 1, GameManger.player.weapon.weaponType));
                        GameManger.player.Effects.Add(new Effect(nextX, nextY - 1, GameManger.player.weapon.weaponType));
                    }
                    else if (GameManger.player.direction / 2 == 1)//위쪽 보는중
                    {
                        GameManger.player.Effects.Add(new Effect(nextX + 1, nextY, GameManger.player.weapon.weaponType));
                        GameManger.player.Effects.Add(new Effect(nextX - 1, nextY, GameManger.player.weapon.weaponType));
                    }
                }

                //상하좌우+해당 방향으로 2칸더
                else if (GameManger.player.weapon.weaponType == 2)
                {
                    GameManger.player.Effects.Add(new Effect(nextX, nextY, GameManger.player.weapon.weaponType));
                    if (GameManger.player.direction == 0)//오른쪽 보는중
                    {
                        GameManger.player.Effects.Add(new Effect(nextX + 1, nextY, GameManger.player.weapon.weaponType));
                        GameManger.player.Effects.Add(new Effect(nextX + 2, nextY, GameManger.player.weapon.weaponType));
                    }
                    else if (GameManger.player.direction == 1)//왼쪽 보는중
                    {
                        GameManger.player.Effects.Add(new Effect(nextX - 1, nextY, GameManger.player.weapon.weaponType));
                        GameManger.player.Effects.Add(new Effect(nextX - 2, nextY, GameManger.player.weapon.weaponType));
                    }
                    else if (GameManger.player.direction == 2)//위쪽 보는중
                    {
                        GameManger.player.Effects.Add(new Effect(nextX, nextY - 1, GameManger.player.weapon.weaponType));
                        GameManger.player.Effects.Add(new Effect(nextX, nextY - 2, GameManger.player.weapon.weaponType));
                    }
                    else if (GameManger.player.direction == 3)//아래쪽 보는중
                    {
                        GameManger.player.Effects.Add(new Effect(nextX, nextY + 1, GameManger.player.weapon.weaponType));
                        GameManger.player.Effects.Add(new Effect(nextX, nextY + 2, GameManger.player.weapon.weaponType));
                    }
                }

            }

            //각 이펙트의 위치와 적의 위치를 보고 겹칠경우 데미지 처리.
            for (int i = 0; i < GameManger.player.Effects.Count; i++)
            {
                if (GameManger.player.Effects[i].type == -1) continue;
                if (GameManger.player.Effects[i] == null) continue;

                int effectX = GameManger.player.Effects[i].axis.x;
                int effectY = GameManger.player.Effects[i].axis.y;

                //적 공격
                Enemy currEnemy = GameManger.currField.FindEnemiesAt(effectX, effectY);
                if (currEnemy != null)
                {
                    if (currEnemy.isLive)
                    {
                        currEnemy.hitPoint -= GameManger.player.weapon.weaponStr;
                        currEnemy.moveTimer.Change(300, 600 - (10 * ((Enemy._ENEMY_HITPOINT_MAX - currEnemy.hitPoint) / Enemy._ENEMY_HITPOINT_MAX)));
                        if (currEnemy.hitPoint <= 0)
                        {
                            GameManger.currField.RemoveEnemy(effectX, effectY);

                            if (GameManger.currField.GetEnemies().Count == 0)
                            {
                                return true;
                            }
                        }
                    }
                    continue;
                }

            }
            return false;
        }//[PressYesEvent] end


        //현재 씬에서 출력할 정보를 스트링으로 가공하는 메서드
        public override string[] ConvertMapToString(ref string[] line)
        {
            for (int y = 0; y < BufferPrinter._BUFFER_SIZE; y++)
            {
                line[y] = "";
            }

            for (int y = 0; y < FieldBase._FIELD_SIZE; y++)
            {
                line[y] = "";
                for (int x = 0; x < FieldBase._FIELD_SIZE; x++)
                {
                    //이펙트 1순위
                    if (GameManger.player.Effects != null && GameManger.player.Effects.Count > 0)
                    {
                        List<Effect> currEffect;

                        try
                        {
                            //총알 이펙트가 해당 총알의 이동 타이머에서 REMOVE 된 뒤에 NULL이 들어오는 경우가 있어서 이를 방지해주기위해 try catch를 사용했다.
                            //같은 위치에 여러개의 이펙트가 있을 경우 모두 불러와서 저장한다.
                            currEffect = GameManger.player.Effects.FindAll(effect => (effect.axis.x == x && effect.axis.y == y));
                        }
                        catch
                        {
                             currEffect = new List<Effect>();
                        }

                        //만약에 isRemove가 true인 이펙트일 경우 이를 삭제한다.(원거리 공격)
                        foreach (Effect tmpEffect in currEffect)
                        {
                            if (tmpEffect != null)
                            {
                                if (tmpEffect.isRemove)
                                {
                                    GameManger.player.Effects.Remove(tmpEffect);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        //여러개중에 제일 앞에 있는 이펙트 하나만 사용한다.
                        Effect firstEffect = currEffect.FirstOrDefault();

                        //try-catch에 걸려서 아무것도 없는 경우가 아니라면 처리한다.
                        if (firstEffect != null)
                        {
                            //만약에 플레이어 위치와 같을 경우 플레이어를 우선 그린다.
                            if (GameManger.player.axis.x == x && GameManger.player.axis.y == y)
                            {
                                if (GameManger.player.hitPoint < 20)
                                {
                                    line[y] += ".6.";

                                }
                                else if (GameManger.player.hitPoint < 50)
                                {
                                    line[y] += ".5.";

                                }
                                else if (GameManger.player.hitPoint < 200)
                                {
                                    line[y] += ".4.";

                                }
                                else if (GameManger.player.hitPoint < int.MaxValue)
                                {
                                    line[y] += ".0.";

                                }

                                switch (GameManger.player.direction)
                                {
                                    case 0:
                                        line[y] += "▶.";
                                        break;
                                    case 1:
                                        line[y] += "◀.";
                                        break;
                                    case 2:
                                        line[y] += "▲.";
                                        break;
                                    case 3:
                                        line[y] += "▼.";
                                        break;
                                    case 4:
                                        line[y] += "！.";
                                        break;
                                    default:
                                        line[y] += "나.";
                                        break;
                                }
                            }

                            //플레이어가 아닐경우 이펙트부터 그린다.
                            else
                            {
                                line[y] += ".11." + Effect._EFFECT_STRING[firstEffect.type] + ".";                             
                            }

                            //원거리 공격이 아니면 무조건 삭제한다.
                            if (firstEffect.type != 4)
                            {
                                GameManger.player.Effects.Remove(firstEffect);
                            }
                            continue;
                        }
                        else
                        {
                        }
                    }

                    //안개 정보에 영향을 받는다. 
                    if (fogInfo[y, x] == 1)
                    {
                        //적 2순위
                        Enemy tmp = FindEnemiesAt(x, y);
                        if (tmp != null)
                        {
                            if (tmp.isLive)
                            {
                                if (tmp.hitPoint < Enemy._ENEMY_HITPOINT_MAX / 3)
                                {
                                    line[y] += ".8.";
                                }
                                else if (tmp.hitPoint <= Enemy._ENEMY_HITPOINT_MAX *2 / 3)
                                {
                                    line[y] += ".7.";
                                }
                                else
                                {
                                    line[y] += ".1.";
                                }

                                if (!tmp.isAttack)
                                {
                                    line[y] += "적.";
                                }
                                else
                                {
                                    line[y] += "喝.";
                                    tmp.isAttack = false;
                                }
                            }
                            else if (!tmp.isLive)
                            {
                                line[y] += ".1.？.";
                            }
                            continue;
                        }
                    }

                    //플레이어 4순위
                    if (GameManger.player.axis.x == x && GameManger.player.axis.y == y)
                    {
                        if (GameManger.player.isLive)
                        {
                            if (GameManger.player.hitPoint < 20)
                            {
                                line[y] += ".6.";

                            }
                            else if (GameManger.player.hitPoint < 50)
                            {
                                line[y] += ".5.";

                            }
                            else if (GameManger.player.hitPoint < 200)
                            {
                                line[y] += ".4.";

                            }
                            else if (GameManger.player.hitPoint < int.MaxValue)
                            {
                                line[y] += ".0.";

                            }

                            switch (GameManger.player.direction)
                            {
                                case 0:
                                    line[y] += "▶.";
                                    break;
                                case 1:
                                    line[y] += "◀.";
                                    break;
                                case 2:
                                    line[y] += "▲.";
                                    break;
                                case 3:
                                    line[y] += "▼.";
                                    break;
                                case 4:
                                    line[y] += "！.";
                                    break;
                                default:
                                    line[y] += "나.";
                                    break;
                            }

                        }
                        else
                        {
                            line[y] += ".3.Π.";
                        }
                        continue;
                    }


                    if (fogInfo[y, x] == 0)
                    {
                        //안개일 경우 공백
                        line[y] += ".8.　.";
                    }
                    else
                    {
                        if (fogInfo[y, x] == 1)
                        {
                            if (fieldInfo[y, x] == FieldBase.field_info.portal)
                            {
                                line[y] += ".2.";
                            }
                            else
                            {
                                line[y] += ".0.";

                            }
                        }
                        else if (fogInfo[y, x] > 0 && fogInfo[y, x] < 1)
                        {
                            if (fieldInfo[y, x] == FieldBase.field_info.portal)
                            {
                                line[y] += ".12.";
                            }
                            else
                            {
                                line[y] += ".6.";
                            }

                        }

                        //맵정보
                        switch (fieldInfo[y, x])
                        {
                            case FieldBase.field_info.empty:
                                line[y] += "　.";
                                break;
                            case FieldBase.field_info.mud:
                                line[y] += "＊.";
                                break;
                            case FieldBase.field_info.tree:
                                line[y] += "〓.";
                                break;
                            case FieldBase.field_info.road:
                                line[y] += "□.";
                                break;
                            case FieldBase.field_info.wall:
                                line[y] += "■.";
                                break;
                        }

                    }
                }

                line[y] += "  ";
            }

            try
            {
                //적이 총알에 죽어서 끝날 경우 currfield의 정보가 바껴서 이상한 값이 들어올때가 있어서 try-catch를 썼음
                line[FieldBase._FIELD_SIZE + 1] += $"체력{GameManger.player.hitPoint,3}/{Player._PLAYER_HITPOINT_MAX,3}";
                line[FieldBase._FIELD_SIZE + 1] += $"적 숫자{GameManger.currField.ReturnSelfToBattle().enemies.Count,2}/{GameManger.currField.ReturnSelfToBattle().enemyCount,2}";
                line[FieldBase._FIELD_SIZE + 1] += "\t\t\t\t\t\t\t\t\t";
            }
            catch
            {
                line[FieldBase._FIELD_SIZE + 1] += $"체력{GameManger.player.hitPoint,3}/{Player._PLAYER_HITPOINT_MAX,3}";
                line[FieldBase._FIELD_SIZE + 1] += "                       ";
                line[FieldBase._FIELD_SIZE + 1] += "\t\t\t\t\t\t\t\t\t";
            }



            for (int y = 0; y < BufferPrinter._BUFFER_SIZE; y++)
            {
                if (line[y].Length < FieldBase._FIELD_SIZE)
                {
                    for (int i = line[y].Length; i < FieldBase._FIELD_SIZE; i++)
                    {
                        line[y] += "　";
                    }
                }
            }

            return line;
        }
    }
}
