using Project1st.Game.Core;
using Project1st.Game.GameObject;
using Project1st.Game.Interface;
using Project1st.Game.Item;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Project1st.Game.Map.Fields
{

    public class Forest : FieldBase
    {
        readonly int _ENEMY_COUNT_MAX = 6;

        public List<Enemy> enemies;
        public Timer createTimer;

        public float[,] fogInfo;

        public Forest()
        {
            //변수 초기화
            isMenu = false;
            type = FieldBase.field_type.forest;
            int wallCount = 60;
            int wallSleep = 2;
            fogInfo = new float[_FIELD_SIZE, _FIELD_SIZE];

            //맵,안개 정보 설정
            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fogInfo[y, x] = 0;
                    if (wallSleep == 0 && wallCount > 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1, 4);
                        if (fieldInfo[y, x] == field_info.tree)
                        {
                            wallCount -= 1;
                            wallSleep = 3;
                        }
                    }
                    else if (wallSleep > 0 || wallCount == 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1, 3);
                        wallSleep -= 1;
                    }
                }
            }
            enemies = new List<Enemy>();
            portals = new Portal[4];
        }


        public Forest(int flag)
        {
            //변수 초기화
            isMenu = false;
            type = FieldBase.field_type.forest;
            int wallCount = 60;
            int wallSleep = 2;
            fogInfo = new float[_FIELD_SIZE, _FIELD_SIZE];

            //맵,안개 정보 설정
            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fogInfo[y, x] = 0;
                    if (wallSleep == 0 && wallCount > 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1, 4);
                        if (fieldInfo[y, x] == field_info.tree)
                        {
                            wallCount -= 1;
                            wallSleep = 3;
                        }
                    }
                    else if (wallSleep > 0 || wallCount == 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1, 3);
                        wallSleep -= 1;
                    }
                }
            }

            enemies = new List<Enemy>();
            portals = new Portal[4];

            //포탈 생성 bit 플래그
            int bit = 1;

            for (int i = 0; i < portals.Length; i++)
            {
                if ((flag & bit) == bit)
                {
                    CreateDoor(i);
                }
                else
                {
                    portals[i] = null;
                }
                bit = bit << 1;
            }

            //portal[0].asd();
        }


        public override void CreateDoor(int index)
        {
            //동,서,남,북 순서.
            //가장자리를 제외한 랜덤한 자리에 생성
            //무조건 해당 방위의 벽에 붙어서 생성된다.
            switch (index)
            {
                case 0:
                    portals[index] = new Portal(_FIELD_SIZE - 1, GameManger.random.Next(1, _FIELD_SIZE - 1));
                    break;
                case 1:
                    portals[index] = new Portal(0, GameManger.random.Next(1, _FIELD_SIZE - 1));
                    break;
                case 2:
                    portals[index] = new Portal(GameManger.random.Next(1, _FIELD_SIZE - 1), 0);
                    break;
                case 3:
                    portals[index] = new Portal(GameManger.random.Next(1, _FIELD_SIZE - 1), _FIELD_SIZE - 1);
                    break;
            }

            //맵 정보에 반영
            fieldInfo[portals[index].axis.y, portals[index].axis.x] = field_info.portal;
        }

        //필드 입장시
        //적 이동, 적 생성 타이머 재 시작
        //최초 1회 시야 4칸 고정
        public override void InitEnter()
        {
            GameManger.currField.PlayEnemies();
            GameManger.currField.SetCreateTimer(new Timer(GameManger.currField.CreateEnemy, null, 100, 10000));

            GameManger.currField.isFog = false;
            GameManger.currField.isCurrField = true;

            GameManger.player.RemoveFog(4);
        }


        //필드 퇴장시
        //적 이동, 적 생성 타이머 종료
        public override void Exit()
        {
            GameManger.currField.isCurrField = false;

            GameManger.currField.StopEnemies();
            if (GameManger.currField.GetCreateTimer() != null)
            {
                GameManger.currField.GetCreateTimer().Dispose();
            }
        }


        //적 생성 메서드
        public override void CreateEnemy(object obj)
        {
            //최대수 제한 있음
            if (enemies.Count > _ENEMY_COUNT_MAX) return;

            //벽과 안 겹치는 부분에 생성
            while (true)
            {
                int enemyX = GameManger.random.Next(_FIELD_SIZE);
                int enemyY = GameManger.random.Next(_FIELD_SIZE);
                if (fieldInfo[enemyY, enemyX] != field_info.tree && (enemyX==GameManger.player.axis.x || enemyY == GameManger.player.axis.y))
                {
                    Enemy enemy = new Enemy(enemyX, enemyY);
                    enemies.Add(enemy);
                    break;
                }
            }
        }

        //해당 좌표의 적 제거
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

        //해당 좌표의 적 반환
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

        //모든 적 정지
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

        //모든 적 이동 재시작
        public override void PlayEnemies()
        {
            if (enemies == null) return;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].moveTimer != null)
                {
                    enemies[i].StartForestTimer();
                }
                //enemies[i].enemyTimer.Change(500, 500);
            }
        }
        public override void PlayEnemies(int time)
        {
            if (enemies == null) return;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].moveTimer != null)
                {
                    enemies[i].StartForestTimer(time);
                }
                //enemies[i].enemyTimer.Change(500, 500);
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

        public override Timer GetCreateTimer()
        {
            return createTimer;
        }
        public override void SetCreateTimer(Timer timer)
        {
            createTimer = timer;
        }
        public override List<Enemy> GetEnemies()
        {
            return enemies;
        }


        //이동키 입력 처리
        public override bool PressMoveEvent()
        {
            bool isHold = false;    //해당 변수가 false가 되면 마차도 같이 이동한다.
            bool isStun = false;    //스턴 이벤트

            //이전 좌표 = 이동하기 전 좌표
            int beforeX = GameManger.player.axis.x; 
            int beforeY = GameManger.player.axis.y;

            //현재 좌표 = 이동한 후 좌표(외곽이동시 외곽벽에 붙는 처리)
            int currX = GameManger.player.Hold(GameManger.player.GetNextX(GameManger.player.direction), FieldBase._FIELD_SIZE);
            int currY = GameManger.player.Hold(GameManger.player.GetNextY(GameManger.player.direction), FieldBase._FIELD_SIZE);

            //빈칸으로 이동
            if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.empty ||
                GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.road)
            {
                isHold = GameManger.player.MoveAndHold(GameManger.player.direction, FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);
            }

            //진흙으로 이동
            if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.mud)
            {
                isHold = GameManger.player.MoveAndHold(GameManger.player.direction, FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);
                if (GameManger.random.Next(1, 101) <= 5)
                {
                    GameManger.player.direction = 4;
                    if (GameManger.player.inventory.Count > 0)
                    {

                        Items item = GameManger.player.inventory[GameManger.random.Next(GameManger.player.inventory.Count)];
                        if (item.itemId >= 10)
                        {
                            //스턴 걸릴시 인벤토리에 있는 아이템 내구도 감소
                            item.quality -= 0.01f * GameManger.random.Next(5);
                            if (item.quality < 0)
                            {
                                item.quality = 0;
                            }
                        }
                    }
                    isStun = true;
                }

            }

            //벽으로 이동
            else if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.tree ||
                     GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.wall)
            {
                isHold = true;
            }

            //텔레포트로 이동
            else if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.portal)
            {
                isHold = true;
                GameManger.player.MoveAndHold(GameManger.player.direction, FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);

                for (int i = 0; i < 4; i++)
                {
                    if (GameManger.currField.portals[i] == null) { continue; }


                    if (currX == GameManger.currField.portals[i].axis.x &&
                        currY == GameManger.currField.portals[i].axis.y)
                    {
                        GameManger.currFieldPos.x += WorldMap._AXIS_MATRIX_X[i];
                        GameManger.currFieldPos.y += WorldMap._AXIS_MATRIX_Y[i];

                        //마차 위치 재설정
                        GameManger.player.Teleport(FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);
                        for (int j = 0; j < GameManger.player.wagonList.Count; j++)
                        {
                            GameManger.player.wagonList[j].axis.x = GameManger.player.axis.x;
                            GameManger.player.wagonList[j].axis.y = GameManger.player.axis.y;
                        }

                        Exit();

                        //Utility.currRoom.enemyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                        GameManger.currField = GameManger.worldMap.map[GameManger.currFieldPos.y, GameManger.currFieldPos.x];
                        GameManger.currField.InitEnter();

                    }
                }

            }

            //숲->숲 일 경우만
            if (GameManger.currField.type != FieldBase.field_type.forest)
            {
                return false;
            }

            //적 경로 재설정
            for (int j = 0; j < GameManger.currField.GetEnemies().Count; j++)
            {
                GameManger.currField.GetEnemies()[j].isPlayerMove = true;
            }

            //적에게 이동
            Enemy enemy = GameManger.currField.FindEnemiesAt(currX, currY);
            if (enemy != null && enemy.isLive)
            {
                //전투 인카운터
                GameManger.currField.Exit();
                GameManger.currField.GetEnemies().RemoveAll(x => x.axis.x == enemy.axis.x && x.axis.y == enemy.axis.y);

                new Battle();
            }

            //마차로 이동
            Wagon wagon = GameManger.player.wagonList.Find(x=>x.axis.x==currX && x.axis.y == currY);
            if (wagon != null)
            {
                //마차의 위치가 포탈이 아닐경우
                if (GameManger.currField.fieldInfo[GameManger.player.axis.y, GameManger.player.axis.x] != FieldBase.field_info.portal)
                {
                    //마차 파괴.
                    //마차안의 아이템 파괴
                    GameManger.player.wagonList.Remove(wagon);
                    GameManger.player.maxWeightSum -= 100;
                }
            }

            //만약 플레이어가 1칸이라도 이동하는데 성공한다면
            if (!isHold)
            {
                //모든 마차 1칸씩 이동
                for (int i = 0; i < GameManger.player.wagonList.Count; i++)
                {
                    currY = GameManger.player.wagonList[i].axis.y;
                    currX = GameManger.player.wagonList[i].axis.x;

                    GameManger.player.wagonList[i].Follow(beforeX, beforeY);

                    beforeY = currY;
                    beforeX = currX;
                }
            }

            return isStun;
        }


        //이동키 처리
        //메뉴 이벤트 처리
        public override void PressMenuEvent()
        {
            //인벤토리 상태일 경우
            if (GameManger.worldMap.isInventory)
            {
                //y축, x축 이동
                GameManger.worldMap.cursor.y += WorldMap._AXIS_MATRIX_Y[GameManger.player.direction];
                int nextX = GameManger.worldMap.cursor.x + WorldMap._AXIS_MATRIX_X[GameManger.player.direction];

                //x축이 -값이 된다면
                if (nextX < 0)
                {
                    //0으로 강제 대입
                    GameManger.worldMap.cursor.x = 0;
                }
                //x축이 wagonlist의 크기보다 커질경우
                else if (nextX >= GameManger.player.wagonList.Count)
                {
                    //wagonlist의 크기 강제 대입(0번칸은 플레이어의 인벤토리 이기 때문에 -1할 필요없다)
                    GameManger.worldMap.cursor.x = GameManger.player.wagonList.Count;
                }
                //그외 바로 대입
                else
                {
                    GameManger.worldMap.cursor.x = nextX;
                }

                //x축 이동이 성공할경우 
                if (GameManger.worldMap.cursor.x != nextX)
                {
                    //y축은 제일 위쪽을 가리킨다.
                    GameManger.worldMap.cursor.y = 0;
                }

                //플레이어 인벤토리
                if (GameManger.worldMap.cursor.x == 0)
                {
                    //인벤토리의 아이템이 10개 초과 들어있을 경우
                    if (GameManger.player.inventory.Count > 10)
                    {
                        //맨위는 위로 드래그
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                            GameManger.player.startInvenIndex -= 1;
                            //맨 위의 아이템일경우
                            if (GameManger.player.startInvenIndex < 0)
                            {
                                GameManger.worldMap.cursor.y = 9;
                                GameManger.player.startInvenIndex = GameManger.player.inventory.Count - 10;
                            }
                        }
                        //맨 아래는 아래로 드래그
                        else if (GameManger.worldMap.cursor.y >= 10)
                        {
                            GameManger.worldMap.cursor.y = 9;
                            GameManger.player.startInvenIndex += 1;
                            //맨 아래의 아이템일경우
                            if (GameManger.player.inventory.Count - GameManger.player.startInvenIndex < 10)
                            {
                                GameManger.worldMap.cursor.y = 0;
                                GameManger.player.startInvenIndex = 0;
                            }
                        }

                    }

                    //인벤토리 아이템이 10개이하일경우
                    else
                    {
                        //반대편 외곽으로 이동한다.
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = GameManger.player.inventory.Count - 1;
                        }

                        else if (GameManger.worldMap.cursor.y >= GameManger.player.inventory.Count)
                        {
                            GameManger.worldMap.cursor.y = 0;
                        }
                    }

                }
                //마차의 인벤토리
                else if (GameManger.worldMap.cursor.x >= 1)
                {
                    //마차 리스트가 제대로 못 불러올경우 return
                    if (GameManger.player.wagonList == null || GameManger.player.wagonList.Count == 0) return;
                    Wagon currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];


                    List<Items> currInventory = currWagon.inventory;

                    //인벤토리의 아이템이 10개 초과 들어있을 경우
                    if (currInventory.Count > 10)
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                            currWagon.startInvenIndex -= 1;
                            if (currWagon.startInvenIndex < 0)
                            {
                                GameManger.worldMap.cursor.y = 9;
                                currWagon.startInvenIndex = currInventory.Count - 10;
                            }
                        }
                        else if (GameManger.worldMap.cursor.y >= 10)
                        {
                            GameManger.worldMap.cursor.y = 9;
                            currWagon.startInvenIndex += 1;
                            if (currInventory.Count - currWagon.startInvenIndex < 10)
                            {
                                GameManger.worldMap.cursor.y = 0;
                                currWagon.startInvenIndex = 0;
                            }
                        }

                    }
                    //인벤토리 아이템이 10개이하일경우
                    else
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = currInventory.Count - 1;
                        }
                        else if (GameManger.worldMap.cursor.y >= currInventory.Count)
                        {
                            GameManger.worldMap.cursor.y = 0;
                        }
                    }
                }

            }

            //장비창일 경우
            else if (GameManger.worldMap.isEquip)
            {
                //인벤토리창일 경우와 같다
                GameManger.worldMap.cursor.y += WorldMap._AXIS_MATRIX_Y[GameManger.player.direction];
                int nextX =
                    GameManger.worldMap.cursor.x + WorldMap._AXIS_MATRIX_X[GameManger.player.direction];

                if (nextX < 0)
                {
                    GameManger.worldMap.cursor.x = 0;
                }
                else if (nextX >= GameManger.player.wagonList.Count)
                {
                    GameManger.worldMap.cursor.x = GameManger.player.wagonList.Count;
                }
                else
                {
                    GameManger.worldMap.cursor.x = nextX;
                }

                if (GameManger.worldMap.cursor.x != nextX)
                {
                    GameManger.worldMap.cursor.y = 0;
                }

                //각 인벤토리에서 장비인 아이템만 불러온다.

                List<Items> currInventory;
                List<Items> equipList;
                //플레이어 인벤토리
                if (GameManger.worldMap.cursor.x == 0)
                {
                    currInventory = GameManger.player.inventory;
                    equipList = currInventory.FindAll(tmp => tmp.type == 1);

                    if (equipList.Count > 10)
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                            GameManger.player.startInvenIndex -= 1;
                            if (GameManger.player.startInvenIndex < 0)
                            {
                                GameManger.player.startInvenIndex = 0;
                            }
                        }
                        else if (GameManger.worldMap.cursor.y >= 10)
                        {
                            GameManger.worldMap.cursor.y = 9;
                            GameManger.player.startInvenIndex += 1;
                            if (equipList.Count - GameManger.player.startInvenIndex < 10)
                            {
                                GameManger.player.startInvenIndex = equipList.Count - 10;
                            }
                        }

                    }
                    else
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                        }

                        else if (GameManger.worldMap.cursor.y >= GameManger.player.inventory.Count)
                        {
                            GameManger.worldMap.cursor.y = GameManger.player.inventory.Count - 1;
                        }
                    }

                }
                //마차 인벤토리
                else if (GameManger.worldMap.cursor.x >= 1)
                {

                    Wagon currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];
                    currInventory = currWagon.inventory;
                    equipList = currInventory.FindAll(tmp => tmp.type == 1);

                    if (GameManger.player.wagonList == null || GameManger.player.wagonList.Count == 0) return;

                    if (equipList.Count > 10)
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                            currWagon.startInvenIndex -= 1;
                            if (currWagon.startInvenIndex < 0)
                            {
                                currWagon.startInvenIndex = 0;
                            }
                        }
                        else if (GameManger.worldMap.cursor.y >= 10)
                        {
                            GameManger.worldMap.cursor.y = 9;
                            currWagon.startInvenIndex += 1;
                            if (equipList.Count - currWagon.startInvenIndex < 10)
                            {
                                currWagon.startInvenIndex = equipList.Count - 10;
                            }
                        }

                    }
                    else
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                        }
                        else if (GameManger.worldMap.cursor.y >= equipList.Count)
                        {
                            GameManger.worldMap.cursor.y = equipList.Count - 1;
                        }
                    }
                }
            }

            //제일 외곽 -> 메뉴창

            //메뉴창은
            //인벤토리
            //minimap on/off
            //장비
            //이렇게 이뤄져있다.
            else
            {
                GameManger.worldMap.cursor.y += WorldMap._AXIS_MATRIX_Y[GameManger.player.direction];
                if (GameManger.worldMap.cursor.y < 0)
                {
                    GameManger.worldMap.cursor.y = 2;
                }
                else if (GameManger.worldMap.cursor.y > 2)
                {
                    GameManger.worldMap.cursor.y = 0;
                }
            }
        }

        //취소키 입력 처리
        public override void PressNoEvent()
        {
            //메뉴가 아닐경우 -> 메뉴창 on
            if (!GameManger.currField.isMenu)
            {
                GameManger.worldMap.isInventory = false;
                GameManger.worldMap.isEquip = false;

                GameManger.currField.isMenu = true;

                //모든 적 정지
                GameManger.currField.StopEnemies();
                GameManger.currField.ReturnSelfToForest().createTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                //커서 위치 초기화
                GameManger.player.startInvenIndex = 0;
                GameManger.worldMap.cursor = new Coordinate(0, 0);

                GameManger.player.RemoveFog(0);
            }

            //메뉴창일경우
            else
            {
                //인벤토리창->메뉴창
                if (GameManger.worldMap.isInventory)
                {
                    GameManger.worldMap.isEquip = false;
                    GameManger.worldMap.isInventory = false;
                    GameManger.player.startInvenIndex = 0;
                    GameManger.worldMap.cursor = new Coordinate(0, 0);
                    return;
                }
                //장비창->메뉴창
                if (GameManger.worldMap.isEquip)
                {
                    GameManger.worldMap.isEquip = false;
                    GameManger.worldMap.isInventory = false;
                    GameManger.player.startInvenIndex = 0;
                    GameManger.worldMap.cursor = new Coordinate(0, 0);
                    return;
                }

                //메뉴창->필드 재시작
                GameManger.currField.isMenu = false;
                GameManger.currField.PlayEnemies(100);
                GameManger.currField.ReturnSelfToForest().createTimer.Change(100, 10000);
                GameManger.player.RemoveFog();
            }

        }

        //확인키 입력 처리
        public override bool PressYesEvent()
        {
            //메뉴창이 아닐경우
            if (!GameManger.currField.isMenu)
            {
                //방해물 제거용 공격
                if (!GameManger.player.isMeleeDelay)
                {
                    //3초 딜레이
                    GameManger.player.isMeleeDelay = true;
                    GameManger.player.meleeDelay = new Timer(GameManger.player.DelayMeleeTimer, null, 3000, 0);

                    int nextX = GameManger.player.Hold(GameManger.player.GetNextX(GameManger.player.direction), FieldBase._FIELD_SIZE);
                    int nextY = GameManger.player.Hold(GameManger.player.GetNextY(GameManger.player.direction), FieldBase._FIELD_SIZE);

                    GameManger.player.Effects.Add(new Effect(nextX, nextY, 0));

                    //적 공격
                    //공격이 성공할경우 한방에 죽인다.
                    Enemy currEnemy = GameManger.currField.FindEnemiesAt(nextX, nextY);
                    if (currEnemy != null)
                    {
                        if (currEnemy.isLive)
                        {
                            currEnemy.hitPoint -= currEnemy.hitPoint;
                            if (currEnemy.hitPoint <= 0)
                            {
                                GameManger.currField.RemoveEnemy(nextX, nextY);
                                return true;
                            }
                        }
                        return false;
                    }


                    //벽 공격
                    if (GameManger.currField.fieldInfo[nextY, nextX] == FieldBase.field_info.tree)
                    {
                        //공구 있을때만 파괴 가능
                        Items breakTool = GameManger.player.inventory.Find(x => x.itemId == 1);

                        if (GameManger.player.inventory.Find(x => x.itemId == 1) != null)
                        {
                            breakTool.count -= 1;
                            if (breakTool.count <= 0)
                            {
                                GameManger.player.inventory.Remove(breakTool);
                            }
                            GameManger.currField.fieldInfo[nextY, nextX] = 0;
                            return false;
                        }
                    }

                    //함정 공격
                    if (GameManger.currField.fieldInfo[nextY, nextX] == FieldBase.field_info.mud)
                    {
                        //공구 있을때만 파괴 가능
                        Items breakTool = GameManger.player.inventory.Find(x => x.itemId == 1);

                        if (GameManger.player.inventory.Find(x => x.itemId == 1) != null)
                        {
                            breakTool.count -= 1;
                            if (breakTool.count <= 0)
                            {
                                GameManger.player.inventory.Remove(breakTool);
                            }
                            GameManger.currField.fieldInfo[nextY, nextX] = 0;
                            return false;
                        }
                    }

                }
            }

            //메뉴창일 경우
            if (GameManger.currField.isMenu)
            {
                //메뉴창의 0번 커서 -> 인벤토리
                if (!GameManger.worldMap.isInventory && !GameManger.worldMap.isEquip && GameManger.worldMap.cursor.y == 0)
                {
                    //모든 커서 위치 재설정
                    GameManger.worldMap.isInventory = true;
                    GameManger.player.startInvenIndex = 0;
                    for (int i = 0; i < GameManger.player.wagonList.Count; i++)
                    {
                        GameManger.player.wagonList[i].startInvenIndex = 0;
                    }
                    GameManger.worldMap.cursor = new Coordinate(0, 0);
                    return false;
                }

                //메뉴창의 1번 커서 -> 미니맵
                if (!GameManger.worldMap.isInventory && !GameManger.worldMap.isEquip && GameManger.worldMap.cursor.y == 1)
                {
                    GameManger.worldMap.isMinimap = !GameManger.worldMap.isMinimap;

                    GameManger.currField.isMenu = false;
                    GameManger.player.RemoveFog();
                    return false;
                }

                //메뉴창의 1번 커서 -> 장비창
                if (!GameManger.worldMap.isInventory && !GameManger.worldMap.isEquip && GameManger.worldMap.cursor.y == 2)
                {
                    //모든 커서 위치 재설정
                    GameManger.worldMap.isEquip = true;
                    GameManger.player.startInvenIndex = 0;
                    for (int i = 0; i < GameManger.player.wagonList.Count; i++)
                    {
                        GameManger.player.wagonList[i].startInvenIndex = 0;
                    }
                    GameManger.worldMap.cursor = new Coordinate(0, 0);
                    return false;
                }

                //인벤토리 창 일 경우
                if (GameManger.worldMap.isInventory)
                {
                    //해당 아이템이 사용가능할 경우 사용한다.
                    List<Items> currInventory;
                    Items item;
                    Wagon currWagon = null;

                    //플레이어 인벤토리
                    if (GameManger.worldMap.cursor.x == 0)
                    {
                        currInventory = GameManger.player.inventory;
                        item = currInventory[GameManger.worldMap.cursor.y + GameManger.player.startInvenIndex];
                    }
                    //마차 인벤토리
                    else
                    {
                        currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];
                        currInventory = currWagon.inventory;
                        item = currInventory[GameManger.worldMap.cursor.y + currWagon.startInvenIndex];
                    }

                    //인터페이스 변수에 해당 필드의 기능을 불러와서 사용
                    GameManger.player.use = GameManger.currField;

                    if (GameManger.player.use.UseItem(item, currWagon))
                    {
                    }
                    return false;
                }

                //장비창일 경우
                if (GameManger.worldMap.isEquip)
                {
                    List<Items> equipList;
                    List<Items> currInventory;
                    Items item;
                    Wagon currWagon = null;

                    //플레이어 인벤토리
                    if (GameManger.worldMap.cursor.x == 0)
                    {
                        currInventory = GameManger.player.inventory;
                        equipList = currInventory.FindAll(tmp => tmp.type == 1);
                        if (equipList == null || equipList.Count == 0)
                        {
                            return false;
                        }
                        item = equipList[GameManger.worldMap.cursor.y + GameManger.player.startInvenIndex];
                    }
                    //마차 인벤토리
                    else
                    {
                        currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];
                        currInventory = currWagon.inventory;
                        equipList = currInventory.FindAll(tmp => tmp.type == 1);
                        if (equipList == null || equipList.Count == 0)
                        {
                            return false;
                        }
                        item = equipList[GameManger.worldMap.cursor.y + currWagon.startInvenIndex];
                    }

                    //인터페이스 변수에 해당 필드의 기능을 불러와서 사용
                    GameManger.player.equip = GameManger.currField;

                    if (GameManger.player.equip.EquipItem(item, currWagon))
                    {

                    }
                    return false;
                }
            }
            return false;
        }


        //필드 정보 -> 출력할 스트링으로 변환
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

                        currEffect = GameManger.player.Effects.FindAll(effect => (effect.axis.x == x && effect.axis.y == y));
                        Effect firstEffect = currEffect.FirstOrDefault();
                        if (firstEffect != null)
                        {
                            line[y] += ".11." + Effect._EFFECT_STRING[firstEffect.type] + ".";
                            if (firstEffect.type != 4)
                            {
                                GameManger.player.Effects.Remove(firstEffect);
                            }
                            continue;
                        }
                    }

                    if (fogInfo[y, x] == 1)
                    {
                        //적 2순위
                        Enemy enemyTmp = FindEnemiesAt(x, y);
                        if (enemyTmp != null)
                        {
                            if (enemyTmp.isLive)
                            {
                                if (enemyTmp.hitPoint < Enemy._ENEMY_HITPOINT_MAX / 20)
                                {
                                    line[y] += ".7.";
                                }
                                else if (enemyTmp.hitPoint <= Enemy._ENEMY_HITPOINT_MAX)
                                {
                                    line[y] += ".1.";
                                }
                                else
                                {
                                    line[y] += ".0.";
                                }
                                line[y] += "적.";
                            }
                            else if (!enemyTmp.isLive)
                            {
                                line[y] += ".3.？.";
                            }
                            continue;
                        }

                        //플레이어 3순위
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

                        //마차 4순위
                        Wagon wagon = GameManger.player.wagonList.Find(tmp => tmp.axis.x == x && tmp.axis.y == y);
                        if (wagon != null)
                        {
                            if (wagon.inventory.Count > 0)
                            {
                                if (wagon.hitPoint <= Wagon._WAGON_HITPOINT_MAX / 2)
                                {
                                    line[y] += ".11.ㅇ.";
                                }
                                else
                                {
                                    line[y] += ".2.ㅇ.";
                                }
                            }
                            else
                            {
                                if (wagon.hitPoint <= Wagon._WAGON_HITPOINT_MAX / 2)
                                {
                                    line[y] += ".9.ㅇ.";
                                }
                                else
                                {
                                    line[y] += ".3.ㅇ.";
                                }
                            }
                            continue;
                        }
                    }


                    //맵 정보
                    if (fogInfo[y, x] == 0)
                    {
                        if (GameManger.player.axis.x == x && GameManger.player.axis.y == y)
                        {
                            line[y] += ".4.나.";
                        }
                        else
                        {
                            line[y] += ".8.　.";
                        }
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

                        switch (fieldInfo[y, x])
                        {
                            case FieldBase.field_info.empty:
                                line[y] += "ㅁ.";
                                break;
                            case FieldBase.field_info.mud:
                                line[y] += "＊.";
                                break;
                            case FieldBase.field_info.tree:
                                line[y] += "〓.";
                                break;
                            case FieldBase.field_info.portal:
                                bool isTownConnect = false;
                                if (x == _FIELD_SIZE - 1) isTownConnect = portals[0].isTown;
                                else if (x == 0) isTownConnect = portals[1].isTown;
                                else if (y == 0) isTownConnect = portals[2].isTown;
                                else if (y == _FIELD_SIZE - 1) isTownConnect = portals[3].isTown;

                                if (isTownConnect)
                                {
                                    line[y] += "문.";
                                }
                                else
                                {
                                    line[y] += "길.";
                                }
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
            }

            //플레이어 정보 출력

            line[FieldBase._FIELD_SIZE + 1] += $"체력{GameManger.player.hitPoint,3}/{Player._PLAYER_HITPOINT_MAX,3}";
            line[FieldBase._FIELD_SIZE + 1] += $"\t시야{FogView.GetCurrTimeLight(GameManger.player.light),2}";
            line[FieldBase._FIELD_SIZE + 1] += $"\tDay {GameManger.worldMap.daySum,-4}";
            if (GameManger.worldMap.day == 0)
            {
                line[FieldBase._FIELD_SIZE + 1] += "(낮)";
            }
            else
            {
                line[FieldBase._FIELD_SIZE + 1] += "(밤)";
            }
            line[FieldBase._FIELD_SIZE + 2] += $"장비) {GameManger.player.weapon.name,-4}";
            line[FieldBase._FIELD_SIZE + 3] += $"골드) {GameManger.player.gold,10} gold";
            line[FieldBase._FIELD_SIZE + 4] += $"현재 좌표 ({GameManger.currFieldPos.x,2}, {GameManger.currFieldPos.y,2})";

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
            
            //메뉴창 아닐 경우
            if (!GameManger.currField.isMenu)
            {
                //미니맵 출력
                if (GameManger.worldMap.isMinimap)
                {
                    for (int y = 0; y < WorldMap._MAP_SIZE; y++)
                    {
                        for (int x = 0; x < WorldMap._MAP_SIZE; x++)
                        {
                            string[] tmp = GameManger.worldMap.map[y, x].MakeRoomToString();
                            line[y * 3] += tmp[0];
                            line[y * 3 + 1] += tmp[1];
                            line[y * 3 + 2] += tmp[2];
                        }
                    }
                }
            }
            //메뉴창 일 경우
            else
            {
                //인벤토리창 출력
                if (GameManger.worldMap.isInventory)
                {
                    for (int y = 0; y < 15; y++)
                    {
                        if (y == GameManger.worldMap.cursor.y)
                        {
                            line[y] += "▶";
                        }
                        else
                        {
                            line[y] += "　";
                        }

                        if (GameManger.worldMap.cursor.x == 0)
                        {
                            if (y + GameManger.player.startInvenIndex < GameManger.player.inventory.Count && y < 10)
                            {
                                Items item = GameManger.player.inventory[y + GameManger.player.startInvenIndex];

                                if (item.type == 0)
                                {
                                    line[y] += ".4.";
                                }
                                else
                                {
                                    line[y] += ".6.";
                                }

                                line[y] += $"{y + GameManger.player.startInvenIndex + 1,2}" + ")";
                                line[y] += $"{item.name,-6}";
                                line[y] += $"{item.price,-6:N0} ";
                                line[y] += $"{item.count,-3}";
                                if (item.type == 2)
                                {
                                    line[y] += $"{(int)(item.quality * 100),3}" + "%";
                                }
                                else
                                {
                                    line[y] += "∞";
                                }
                                line[y] += ".\t\t\t\t\t";
                            }
                        }
                        else if (GameManger.worldMap.cursor.x >= 1)
                        {
                            if (GameManger.player.wagonList == null || GameManger.player.wagonList.Count == 0) continue;
                            Wagon currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];

                            List<Items> currInventory = currWagon.inventory;

                            if (y + currWagon.startInvenIndex < currInventory.Count && y < 10)
                            {
                                Items item = currInventory[y + currWagon.startInvenIndex];
                                if (item.type == 0)
                                {
                                    line[y] += ".4.";
                                }
                                else if (item.type == 1)
                                {
                                    line[y] += ".6.";
                                }
                                else
                                {
                                    line[y] += ".8.";
                                }

                                line[y] += $"{y + currWagon.startInvenIndex + 1,2}" + ")";
                                line[y] += $"{item.name,-6}";
                                line[y] += $"{item.price,-6:N0} ";
                                line[y] += $"{item.count,-3}";
                                if (item.type == 2)
                                {
                                    line[y] += $"{(int)(item.quality * 100),3}" + "%";
                                }
                                else
                                {
                                    line[y] += "∞";
                                }
                                line[y] += ".\t\t\t\t\t";
                            }
                        }
                    }

                }

                //장비창 출력
                else if (GameManger.worldMap.isEquip)
                {
                    for (int y = 0; y < 15; y++)
                    {
                        if (y == GameManger.worldMap.cursor.y)
                        {
                            line[y] += "▶";
                        }
                        else
                        {
                            line[y] += "　";
                        }

                        if (GameManger.worldMap.cursor.x == 0)
                        {
                            List<Items> equipList = GameManger.player.inventory.FindAll(tmp => tmp.type == 1);

                            if (y + GameManger.player.startInvenIndex < equipList.Count && y < 10)
                            {
                                Items item = equipList[y + GameManger.player.startInvenIndex];
                                if (item.type == 0)
                                {
                                    line[y] += ".4.";
                                }
                                else if (item.type == 1)
                                {
                                    line[y] += ".8.";
                                }
                                else
                                {
                                    line[y] += ".10.";
                                }
                                line[y] += $"{y + GameManger.player.startInvenIndex + 1,2}" + ")";
                                line[y] += $"{item.name,-6} ";
                                line[y] += $"{item.weaponStr,4} ";
                                line[y] += $"{item.weaponDelay,5} ms";
                                line[y] += ".\t\t\t\t\t";
                            }
                        }
                        else if (GameManger.worldMap.cursor.x >= 1)
                        {
                            if (GameManger.player.wagonList == null || GameManger.player.wagonList.Count == 0) continue;
                            Wagon currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];

                            List<Items> equipList = currWagon.inventory.FindAll(tmp => tmp.type == 1);

                            if (y + currWagon.startInvenIndex < equipList.Count && y < 10)
                            {
                                Items item = equipList[y + currWagon.startInvenIndex];

                                line[y] += $"{y + currWagon.startInvenIndex + 1,2}" + ")";
                                line[y] += $"{item.name,-6}";
                                line[y] += "\t\t\t\t\t";
                            }
                        }
                    }

                }
                //메뉴창 출력
                else
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (y == GameManger.worldMap.cursor.y)
                        {
                            line[y] += "▶";
                        }
                        else
                        {
                            line[y] += "　";
                        }
                    }

                    line[0] += "인벤토리\t\t\t\t";
                    if (GameManger.worldMap.isMinimap)
                    {
                        line[1] += "미니맵 off\t\t\t";
                    }
                    else
                    {
                        line[1] += "미니맵 on\t\t\t";
                    }
                    line[2] += "장비\t\t\t\t\t\t";
                }

            }

            for (int y = 0; y < BufferPrinter._BUFFER_SIZE; y++)
            {
                line[y] += "\t\t\t\t\t\t\t\t\t";
            }
            return line;
        }

    }
}
