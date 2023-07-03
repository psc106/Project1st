using Project1st.Game.GameObject;
using Project1st.Game.Item;
using Project1st.Game.Map;
using Project1st.Game.Map.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Core
{
    public class Core
    {
        //각 필드에서 보내온 출력 string을 저장하는 변수
        string[] line;


        public Core()
        {
            //게임 매니저 인스턴스.
            GameManger.Instance();

            //콘솔 초기화
            Console.WindowWidth = 200;
            Console.WindowHeight = BufferPrinter._BUFFER_SIZE;
            Console.CursorVisible = false;

            //출력할 높이(height, y축) 만큼 string 배열 설정
            line = new string[BufferPrinter._BUFFER_SIZE];

            //플레이어의 초기 위치(나중에 필드에서 나올때를 대비. 현재는 마을에서 시작하기 떄문에 필요없음)
            GameManger.player.axis.x = 0;
            GameManger.player.axis.y = 0;

            //테스트 코드
            //WorldMap.testPos = GameManger.currFieldPos;

            //플레이어가 시작하는 맵 위치 설정
            GameManger.currField = GameManger.worldMap.map[GameManger.currFieldPos.y, GameManger.currFieldPos.x];
            GameManger.currField.isFog = false;
            GameManger.currField.isCurrField = true;
        }

        public void Start()
        {

            //그리기 타이머
            GameManger.buffer.printTimer = new Timer(SetPrintMap, line, 100, 100);

            //이벤트 처리 용 bool 변수
            bool isBattleWin = false;
            bool isQuit = false;
            bool isStun = false;

            //입력 처리용 bool 변수
            bool isMove = false;
            bool isYes = false;
            bool isNo = false;

            //시작 필드 초기화
            GameManger.currField.InitEnter();
            
            //게임 시작
            while (true)
            {
                //입력 처리 변수 초기화
                isMove = false;                
                isYes = false;
                isNo = false;

                //stun 이벤트는 입력을 못받게 한다.
                //stun이 아닐때
                if (!isStun)
                {
                    //키(방향키, z,x,q 만 사용)
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.RightArrow:
                            isMove = true;
                            GameManger.player.direction = 0;
                            break;
                        case ConsoleKey.LeftArrow:
                            isMove = true;
                            GameManger.player.direction = 1;
                            break;
                        case ConsoleKey.UpArrow:
                            isMove = true;
                            GameManger.player.direction = 2;
                            break;
                        case ConsoleKey.DownArrow:
                            isMove = true;
                            GameManger.player.direction = 3;
                            break;
                        case ConsoleKey.Z:
                            isYes = true;
                            break;
                        case ConsoleKey.X:
                            isNo = true;
                            break;
                        case ConsoleKey.Q:
                            isQuit = true;
                            break;
                        default:
                            GameManger.player.direction = 5;
                            break;
                    }

                    //1ms동안 선 입력 제거
                    Thread.Sleep(1);
                    while (Console.KeyAvailable) Console.ReadKey(true);
                }
                else
                {
                    //1초 동안 선입력 제거
                    Thread.Sleep(1000);
                    while (Console.KeyAvailable) Console.ReadKey(true);

                    //스턴 해제
                    isStun = false;
                    GameManger.player.direction = 5;
                }



                #region 이벤트 처리

                //종료
                if (isQuit)
                {
                    GameManger.buffer.printTimer.Dispose();
                    return;
                }

                //패배
                if (!GameManger.player.isLive)
                {
                    GameManger.buffer.printTimer.Dispose();
                    return;
                }

                //적 죽일시 and 전투 승리시
                if (GameManger.currField.isWin || isBattleWin)
                {
                    //현재 씬이 전투씬일경우 -> 전투 승리
                    if (GameManger.currField.type == FieldBase.field_type.battle)
                    {
                        //전투후 player 체력저장
                        int afterHitPoint = GameManger.player.hitPoint;

                        //플레이어와 필드를 이전값으로 복구
                        GameManger.player = GameManger.currField.ReturnSelfToBattle().beforePlayerInfo;
                        GameManger.currField = GameManger.currField.ReturnSelfToBattle().beforeFieldInfo;
                        //전투후 player 체력 동기화
                        GameManger.player.hitPoint = afterHitPoint;

                        //필드에서 timer로 돌아가는 것들 재시작 시킴
                        GameManger.currField.InitEnter();

                        //승리 보상
                        GameManger.player.gold += 700;
                    }

                    //이벤트 플래그 초기화
                    isBattleWin = false;

                    //필드에서 적 죽일 경우 보상
                    //or 전투승리시 추가 보상
                    GameManger.player.gold += 300;
                    continue;
                }

                //엔딩 씬
                else if (GameManger.currField.type == FieldBase.field_type.ending)
                {
                    continue;
                }

                #endregion


                #region 입력 처리

                //이동 입력(방향키)
                if (isMove)
                {
                    //안개 처리
                    //숲, 전투 씬에는 안개가 있기 떄문에 이를 전처리한다.
                    if ((GameManger.currField.type == FieldBase.field_type.forest || GameManger.currField.type == FieldBase.field_type.battle))
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

                    //숲
                    if (GameManger.currField.type == FieldBase.field_type.forest)
                    {
                        //메뉴가 아닐경우(일반적으로 필드를 돌아다닐 경우)
                        if (!GameManger.currField.isMenu)
                        {
                            //이동
                            isStun = GameManger.currField.PressMoveEvent();
                            
                            //등불 사용시 쿨타임(걸음수로 따짐)
                            if (GameManger.player.walk > 0)
                            {
                                GameManger.player.walk -= 1;
                                if (GameManger.player.walk < 0)
                                {
                                    GameManger.player.light -= 2;
                                }
                            }
                        }

                        //메뉴일 경우
                        else
                        {
                            //메뉴 이동
                            GameManger.currField.PressMenuEvent();
                        }
                    }

                    //마을일 경우
                    else if (GameManger.currField.type == FieldBase.field_type.town)
                    {
                        //메뉴 이동
                        GameManger.currField.PressMenuEvent();
                        //커서 이동 방향 초기화
                        GameManger.player.direction = 5;
                    }

                    //전투일 경우
                    else if (GameManger.currField.type == FieldBase.field_type.battle)
                    {
                        //이동
                        //전투중에는 메뉴를 열수없다.
                        isStun = GameManger.currField.PressMoveEvent();
                    }

                    //안개 처리
                    if (!GameManger.currField.isMenu && (GameManger.currField.type == FieldBase.field_type.forest || GameManger.currField.type == FieldBase.field_type.battle))
                    {
                        GameManger.player.RemoveFog();
                    }

                }

                //확인(z) 누를 시
                if (isYes)
                {         
                    //확인 입력 처리
                    isBattleWin = GameManger.currField.PressYesEvent();                    

                    //마을일 경우만 커서 방향을 초기화한다.
                    if (GameManger.currField.type == FieldBase.field_type.town)
                    {
                        GameManger.player.direction = 5;
                    }
                }

                //취소(x) 누를시
                if (isNo)
                {
                    //취소 입력 처리
                    GameManger.currField.PressNoEvent();

                    //마을일 경우만 커서 방향을 초기화한다.
                    if (GameManger.currField.type == FieldBase.field_type.town)
                    {
                        GameManger.player.direction = 5;
                    }
                }
                #endregion
            }//[while] end
        }//[start] end


        //매 초마다 실행되는 출력 처리 메서드
        public void SetPrintMap(Object obj)
        {
            //각 필드에서 string을 받는다.
            GameManger.currField.ConvertMapToString(ref line);

            //만약 buffer가 작동중이 아니라면 실행한다.
            if (!GameManger.buffer.isWork)
            {
                GameManger.buffer.SetBuffer((string[])obj);
                GameManger.buffer.PrintBuffer();
            }

            //만약 buffer가 작동중이라면 실행하지 않는다.
            else
            {
            }
        }
    }
}
