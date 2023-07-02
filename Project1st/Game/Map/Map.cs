using Project1st.Game.Core;
using Project1st.Game.GameObject;
using Project1st.Game.Item;
using Project1st.Game.Map.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Map
{
    public class WorldMap
    {
        public static readonly int _MAP_SIZE = 4;

        public static readonly int[] _AXIS_MATRIX_X = { 1, -1, 0, 0 ,0,0};
        public static readonly int[] _AXIS_MATRIX_Y = { 0, 0, -1, 1 ,0,0};

        //맵은 field로 구성되어있다.
        public FieldBase[,] map;
        //마을 리스트
        public List<Town> townList;

        //날짜 경과 시스템
        public int day;
        public int daySum;
        public Timer dayTimer;
        //public static Coordinate testPos;


        //메뉴창
        public Coordinate cursor;
        
        public bool isMinimap;
        public bool isInventory;
        public bool isEquip;


        public WorldMap()
        {
            //0일 밤부터 시작
            //나가면 1일 낮이 되기때문에
            day = 1;
            daySum = 0;

            //배열, 리스트 생성
            map = new FieldBase[_MAP_SIZE, _MAP_SIZE];
            townList = new List<Town>();

            //메뉴 초기화
            cursor = new Coordinate(0, 0);
            isMinimap = true;
            isInventory = false;
            isEquip = false;

            //map의 구성요소를 채워준다.
            //홀행/짝열, 짝행/홀열을 모두 숲으로 만든다.
            //이 때 포탈도 같이 생성시킨다.

            //? o ? o ?
            //o ? o ? o
            //? o ? o ?
            //o ? o ? o
            //? o ? o ?

            //?->아직 생성 안함  o->숲으로 생성함
            for (int y = 0; y < _MAP_SIZE; y++)
            {
                for (int x = 0; x < _MAP_SIZE; x++)
                {
                    //홀수 행, 짝수 열
                    if (y % 2 == 0 && x % 2 == 1)
                    {
                        //숲 필드 생성
                        map[y, x] = new Forest(GameManger.random.Next(0, 16));

                        //만약에 외곽으로 포탈이 생성된다면 이를 없앤다.
                        //<-
                        if (x == 0 && map[y, x].portals[1] != null)
                        {
                            Portal currPortal = map[y, x].portals[1];
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            map[y, x].portals[1] = null;
                        }
                        //->
                        if (x == _MAP_SIZE - 1 && map[y, x].portals[0] != null)
                        {
                            Portal currPortal = map[y, x].portals[0];
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            map[y, x].portals[0] = null;
                        }
                        //↓
                        if (y == _MAP_SIZE - 1 && map[y, x].portals[3] != null)
                        {
                            Portal currPortal = map[y, x].portals[3];
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            map[y, x].portals[3] = null;
                        }
                        //↑
                        if (y == 0 && map[y, x].portals[2] != null)
                        {
                            Portal currPortal = map[y, x].portals[2];
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            map[y, x].portals[2] = null;
                        }
                    }

                    //짝수 행, 홀수 열
                    //위와 동일하다.
                    else if (y % 2 == 1 && x % 2 == 0)
                    {

                        map[y, x] = new Forest(GameManger.random.Next(0, 16));
                        if (x == 0 && map[y, x].portals[1] != null)
                        {
                            Portal currPortal = map[y, x].portals[1];
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            map[y, x].portals[1] = null;
                        }
                        if (x == _MAP_SIZE - 1 && map[y, x].portals[0] != null)
                        {
                            Portal currPortal = map[y, x].portals[0];
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            map[y, x].portals[0] = null;
                        }
                        if (y == _MAP_SIZE - 1 && map[y, x].portals[3] != null)
                        {
                            Portal currPortal = map[y, x].portals[3];
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            map[y, x].portals[3] = null;
                        }
                        if (y == 0 && map[y, x].portals[2] != null)
                        {
                            Portal currPortal = map[y, x].portals[2];
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.empty;
                            map[y, x].portals[2] = null;
                        }
                    }

                }

            }//[for] 1차 숲 생성 end


            //생성한 숲에 포탈을 한번 더 추가한다.
            //포탈을 동서북남순으로 생성시켜서 최소한의 연결을 보장해준다.
            int cnt = 0;
            for (int y = 0; y < _MAP_SIZE; y++)
            {
                for (int x = 0; x < _MAP_SIZE; x++)
                {
                    //홀수 행, 짝수 열
                    if (y % 2 == 0 && x % 2 == 1)
                    {
                        //만약 이번 포탈이 외곽에 부딫치는 경우는 그냥 넘어간다.
                        if (x == _MAP_SIZE - 1 && cnt == 0) continue;
                        if (x == 0 && cnt == 1) continue;
                        if (y == _MAP_SIZE - 1 && cnt == 3) continue;
                        if (y == 0 && cnt == 2) continue;

                        //해당 방향에 포탈이 없으면 생성 시킨다.
                        if (map[y, x].portals[cnt] == null)
                        {
                            map[y, x].CreateDoor(cnt);
                            cnt += 1;
                            if (cnt == 4)
                            {
                                cnt = 0;
                            }
                        }
                    }
                    //짝수 행, 홀수 열
                    //위와 동일하다.
                    else if (y % 2 == 1 && x % 2 == 0)
                    {
                        if (x == _MAP_SIZE - 1 && cnt == 0) continue;
                        if (x == 0 && cnt == 1) continue;
                        if (y == _MAP_SIZE - 1 && cnt == 3) continue;
                        if (y == 0 && cnt == 2) continue;

                        if (map[y, x].portals[cnt] == null)
                        {
                            map[y, x].CreateDoor(cnt);
                            cnt += 1;
                            if (cnt == 4)
                            {
                                cnt = 0;
                            }
                        }
                    }

                }
            }//[for] 2차 포탈 생성 end


            //필드가 생성되지않은 부분에 숲을 생성해준다.
            //이때 포탈은 생성하지않고 이전에 생성되있던 숲의 포탈과 연결시킨다.
            for (int y = 0; y < _MAP_SIZE; y++)
            {
                for (int x = 0; x < _MAP_SIZE; x++)
                {
                    //홀수 행, 홀수 열
                    if ((y % 2 == 0 && x % 2 == 0) || (y % 2 == 1 && x % 2 == 1))
                    {
                        //포탈없이 숲만 생성
                        map[y, x] = new Forest();

                        //서쪽포탈 -> 서쪽숲의 동쪽 포탈과 연결
                        if (x != 0 && map[y, x - 1].portals[0] != null)
                        {
                            //서쪽(x-1)의 동쪽 포탈(portals[0])
                            Portal connectPortal = map[y, x - 1].portals[0];

                            //연결된포탈의 y축을 가져와서 저장
                            map[y, x].portals[1] = new Portal(0, connectPortal.axis.y);
                            Portal currPortal = map[y, x].portals[1];

                            //필드정보에 포탈 정보 기입
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.portal;

                        }

                        //동쪽 포탈 -> 동쪽숲의 서쪽 포탈과 연결
                        if (x != _MAP_SIZE - 1 && map[y, x + 1].portals[1] != null)
                        {
                            //동쪽(x+1)의 서쪽 포탈(portals[1])
                            Portal connectPortal = map[y, x + 1].portals[1];

                            //연결된포탈의 y축을 가져와서 저장
                            map[y, x].portals[0] = new Portal(FieldBase._FIELD_SIZE - 1, connectPortal.axis.y);
                            Portal currPortal = map[y, x].portals[0];

                            //필드정보에 포탈 정보 기입
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.portal;


                        }

                        //남쪽 포탈 -> 남쪽 숲의 북쪽 포탈과 연결
                        if (y != _MAP_SIZE - 1 && map[y + 1, x].portals[2] != null)
                        {
                            //남쪽(y+1)의 북쪽 포탈(portals[2])
                            Portal connectPortal = map[y + 1, x].portals[2];

                            //연결된포탈의 x축을 가져와서 저장
                            map[y, x].portals[3] = new Portal(connectPortal.axis.x, FieldBase._FIELD_SIZE - 1);
                            Portal currPortal = map[y, x].portals[3];

                            //필드정보에 포탈 정보 기입
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.portal;

                        }

                        //북쪽 포탈 -> 북쪽 숲의 남쪽 포탈과 연결
                        if (y != 0 && map[y - 1, x].portals[3] != null)
                        {
                            //북쪽(y-1)의 남쪽 포탈(portals[3])
                            Portal connectPortal = map[y - 1, x].portals[3];

                            //연결된포탈의 x축을 가져와서 저장
                            map[y, x].portals[2] = new Portal(connectPortal.axis.x, 0);
                            Portal currPortal = map[y, x].portals[2];

                            //필드정보에 포탈 정보 기입
                            map[y, x].fieldInfo[currPortal.axis.y, currPortal.axis.x] = FieldBase.field_info.portal;

                        }
                    }
                }
            }//[for] 3차 숲+포탈 생성 end


            //연결되지않은 숲들을 서로 연결시켜줌
            Coordinate connect;
            for (int y = 0; y < _MAP_SIZE; y++)
            {
                for (int x = 0; x < _MAP_SIZE; x++)
                {
                    //재귀 함수 check로 자기자신 또는 연결된 필드의 제일 바깥부분을 가져옴
                    connect = check(x, y);

                    //불러오는데 실패할경우 다음 수를 본다.
                    if (connect == null)
                    {
                        continue;
                    }

                    //불러온 필드의 모든 방향에 포탈을 생성.
                    //단 외곽을 넘을 경우 이는 생성하지않는다.

                    //동
                    if (connect.x != _MAP_SIZE - 1 && map[connect.y, connect.x].portals[0] == null)
                    {
                        map[connect.y, connect.x + 1].CreateDoor(1);
                        Portal nextDoor = map[connect.y, connect.x + 1].portals[1];

                        map[connect.y, connect.x].portals[0] = new Portal(FieldBase._FIELD_SIZE - 1, nextDoor.axis.y);
                        Portal currDoor = map[connect.y, connect.x].portals[0];

                        map[connect.y, connect.x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;
                        map[connect.y, connect.x + 1].fieldInfo[nextDoor.axis.y, nextDoor.axis.x] = FieldBase.field_info.portal;
                    }
                    //서
                    if (connect.x != 0 && map[connect.y, connect.x].portals[1] == null)
                    {
                        map[connect.y, connect.x - 1].CreateDoor(0);
                        Portal nextDoor = map[connect.y, connect.x - 1].portals[0];

                        map[connect.y, connect.x].portals[1] = new Portal(0, nextDoor.axis.y);
                        Portal currDoor = map[connect.y, connect.x].portals[1];


                        map[connect.y, connect.x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;
                        map[connect.y, connect.x - 1].fieldInfo[nextDoor.axis.y, nextDoor.axis.x] = FieldBase.field_info.portal;

                    }
                    //남
                    if (connect.y != _MAP_SIZE - 1 && map[connect.y, connect.x].portals[3] == null)
                    {
                        map[connect.y + 1, connect.x].CreateDoor(2);
                        Portal nextDoor = map[connect.y + 1, connect.x].portals[2];

                        map[connect.y, connect.x].portals[3] = new Portal(nextDoor.axis.x, FieldBase._FIELD_SIZE - 1);
                        Portal currDoor = map[connect.y, connect.x].portals[3];

                        map[connect.y, connect.x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;
                        map[connect.y + 1, connect.x].fieldInfo[nextDoor.axis.y, nextDoor.axis.x] = FieldBase.field_info.portal;
                    }
                    //북
                    if (connect.y != 0 && map[connect.y, connect.x].portals[2] == null)
                    {
                        map[connect.y - 1, connect.x].CreateDoor(3);
                        Portal nextDoor = map[connect.y - 1, connect.x].portals[3];

                        map[connect.y, connect.x].portals[2] = new Portal(nextDoor.axis.x, 0);
                        Portal currDoor = map[connect.y, connect.x].portals[2];

                        map[connect.y, connect.x].fieldInfo[currDoor.axis.y, currDoor.axis.x] = FieldBase.field_info.portal;
                        map[connect.y - 1, connect.x].fieldInfo[nextDoor.axis.y, nextDoor.axis.x] = FieldBase.field_info.portal;
                    }

                }
            }//[for] 4차 포탈 생성 end


            //시작 마을, 마지막 마을 생성(고정)
            //생성자를 통해서 포탈을 자동으로 생성한다.
            Town startTown = new Town(map[0, 0]);
            Town finalTown = new Town(map[_MAP_SIZE - 1, _MAP_SIZE - 1]);

            //마지막 마을에는 집문서 팔음(승리조건)
            finalTown.shop.Add(new Items(GameManger.db.database[100]));

            //타운리스트에 추가
            townList.Add(startTown);
            townList.Add(finalTown);

            //시작 마을
            int tmpx = 0;
            int tmpy = 0;
            map[tmpy, tmpx] = startTown;
            //포탈에 마을로 가는 포탈인지에 대한 정보를 넣는다.
            for (int i = 0; i < 4; i++)
            {
                if (map[tmpy, tmpx].portals[i] != null)
                {
                    switch (i)
                    {
                        case 0:
                            map[tmpy, tmpx + 1].portals[1].isTown = true;
                            break;
                        case 1:
                            map[tmpy, tmpx - 1].portals[0].isTown = true;
                            break;
                        case 2:
                            map[tmpy - 1, tmpx].portals[3].isTown = true;
                            break;
                        case 3:
                            map[tmpy + 1, tmpx].portals[2].isTown = true;
                            break;

                    }
                }
            }


            //마지막 마을
            tmpx = _MAP_SIZE - 1;
            tmpy = _MAP_SIZE - 1;
            map[_MAP_SIZE - 1, _MAP_SIZE - 1] = finalTown;
            //포탈에 마을로 가는 포탈인지에 대한 정보를 넣는다.
            for (int i = 0; i < 4; i++)
            {
                if (map[tmpy, tmpx].portals[i] != null)
                {
                    switch (i)
                    {
                        case 0:
                            map[tmpy, tmpx + 1].portals[1].isTown = true;
                            break;
                        case 1:
                            map[tmpy, tmpx - 1].portals[0].isTown = true;
                            break;

                        case 2:
                            map[tmpy - 1, tmpx].portals[3].isTown = true;
                            break;

                        case 3:
                            map[tmpy + 1, tmpx].portals[2].isTown = true;
                            break;

                    }
                }
            }

            //마을 갯수
            int count = _MAP_SIZE;

            //갯수만큼 마을을 생성한다.
            while (count > 0)
            {
                tmpx = GameManger.random.Next(0, _MAP_SIZE);
                tmpy = GameManger.random.Next(0, _MAP_SIZE);
                
                //같은 위치에 마을이 없을 경우
                if (map[tmpy, tmpx].type != FieldBase.field_type.town)
                {
                    //마을은 최소 1칸이상 떨어져 있어야한다.
                    if (tmpx != 0 && map[tmpy, tmpx - 1].type == FieldBase.field_type.town)
                    {
                        continue;
                    }

                    else if (tmpx != _MAP_SIZE - 1 && map[tmpy, tmpx + 1].type == FieldBase.field_type.town)
                    {
                        continue;
                    }

                    else if (tmpy != _MAP_SIZE - 1 && map[tmpy + 1, tmpx].type == FieldBase.field_type.town)
                    {
                        continue;
                    }

                    else if (tmpy != 0 && map[tmpy - 1, tmpx].type == FieldBase.field_type.town)
                    {
                        continue;
                    }

                    //모든 조건을 만족할 경우
                    else
                    {
                        //해당 위치에 마을 생성
                        Town tmp = new Town(map[tmpy, tmpx]);
                        map[tmpy, tmpx] = tmp;
                        townList.Add(tmp);
                        count -= 1;

                        //포탈에 마을로 가는 포탈인지에 대한 정보를 넣는다.
                        for (int i = 0; i < 4; i++)
                        {
                            if (map[tmpy, tmpx].portals[i] != null)
                            {
                                switch (i)
                                {
                                    case 0:
                                        map[tmpy, tmpx + 1].portals[1].isTown = true;
                                        break;
                                    case 1:
                                        map[tmpy, tmpx - 1].portals[0].isTown = true;
                                        break;
                                    case 2:
                                        map[tmpy - 1, tmpx].portals[3].isTown = true;
                                        break;
                                    case 3:
                                        map[tmpy + 1, tmpx].portals[2].isTown = true;
                                        break;

                                }
                            }
                        }
                    }
                }
            }//[while] 마을생성 end

            //날짜 경과 timer
            dayTimer = new Timer(SetDayTimer, null, 100, 60000);
        }

        //필드가 다른 필드와 연결되어있는지 확인하는 재귀 메서드.
        Coordinate check(int x, int y)
        {
            //외곽에 닿을 경우 null 반환
            if (x == -1 || x == _MAP_SIZE || y == -1 || y == _MAP_SIZE) return null;
            //이전에 한번 접근했을 경우 null 반환
            if (map[y, x].isFog) return null;

            //초기값 null
            Coordinate last = null;

            //상하좌우로 뻗어간다.
            for (int i = 0; i < 4; i++)
            {
                //연결된 포탈이 있다면
                if (map[y, x].portals[i] != null)
                {
                    //현재위치의 fog를 true로 바꾼다
                    map[y, x].isFog = true;
                    //다음 위치를 check한다.
                    last = check(x + _AXIS_MATRIX_X[i], y + _AXIS_MATRIX_Y[i]);
                }
            }

            //4방향 모든 위치에 포탈이 없거나, 외곽에 닿았거나, 이전에 한번 접근했을경우 null이 된다.
            if (last == null)
            {
                //현재 위치의 fog를 채우고
                map[y, x].isFog = true;
                //해당 좌표를 반환
                return new Coordinate(x, y);
            }
            //무언가 한번 값이 반환됬다면 그값을 계속 반환한다.
            else
            {
                return last;
            }
        }//[check] end


        //날짜 경과 메서드
        public void SetDayTimer(object obj)
        {
            //day
            //0 : 낮
            //1 : 밤
            day += 1;

            //만약에 아이템(등불)을 사용 했다면 이를 감소시킨다.
            if (GameManger.player.walk > 0)
            {
                GameManger.player.walk = -30;

                //이렇게 감소된 상태에서 사용 시간으 0이 된다면
                if (GameManger.player.walk < 0)
                {
                    //아이템 효과 없앤다.
                    GameManger.player.walk = 0;
                    GameManger.player.light -= 2;
                }
            }

            //day가 2이상 될경우 하루가 경과된걸로 처리한다.
            if (day >= 2)
            {
                //날짜
                daySum += 1;

                //모든 마을
                for (int y = 0; y < townList.Count; y++)
                {
                    //매일 마을의 아이템 시세를 관리한다
                    foreach (var townPriceTmp in townList[y].priceRate)
                    {
                        townPriceTmp.Value.ChangePriceRate();
                        //시세 유지가 0턴인 아이템은 수량증가
                        if (townPriceTmp.Value.keepTurn == 0)
                        {
                            int currItemIndex = townList[y].shop.FindIndex(x => x.itemId == townPriceTmp.Key && x.isOwn);
                            if (currItemIndex!=-1)
                            {
                                townList[y].shop[currItemIndex].count += GameManger.random.Next(3, 11);
                            }
                        }
                    }

                    //주점에서 얻은 정보 삭제
                    townList[y].pubEvents.Clear();

                    //해당 마을의 상점 재산 복구
                    if (townList[y].gold < 1000)
                    {
                        townList[y].gold = 2000;
                    }
                    else if (townList[y].gold < 20000)
                    {

                        townList[y].gold += 1000;
                    }
                }
                day = 0;
            }
        }//[SetDayTimer] end

    }

}
