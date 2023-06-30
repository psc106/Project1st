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
        int[] axisX = { 1, -1, 0, 0 };
        int[] axisY = { 0, 0, -1, 1 };

        string[] line;

        public Core()
        {
            GameManger.Instance();

            Console.WindowWidth = 200;
            Console.WindowHeight = GameManger.buffer._BUFFER_SIZE;

            line = new string[GameManger.buffer._BUFFER_SIZE];

            GameManger.player.Axis2D.x = 1;
            GameManger.player.Axis2D.y = 1;

            //테스트 코드
            WorldMap.testPos = GameManger.currFieldPos;

            GameManger.currField = GameManger.map.worldMap[GameManger.currFieldPos.y, GameManger.currFieldPos.x];
        }

        public void Start()
        {

            //그리기 타이머
            GameManger.buffer.printTimer = new Timer(SetPrintMap, line, 100, 100);


            GameManger.currField.isFog = false;
            GameManger.currField.isCurrField = true;

            bool isWin = false;
            bool isQuit = false;

            bool isMove = false;
            bool isStun = false;


            bool isYes = false;
            bool isNo = false;
            bool isFirst = false;
            
            GameManger.player.RemoveFog();
            if (GameManger.currField.type == 2)
            {
                GameManger.currField.ReturnSelfToTown().Enter();
            }


            Console.CursorVisible = false;
            while (true)
            {
                isMove = false;
                
                isYes = false;
                isNo = false;

                if (!isStun)
                {
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
                    Thread.Sleep(1);
                    while (Console.KeyAvailable) Console.ReadKey(true);
                }
                else
                {
                    Thread.Sleep(1000);
                    while (Console.KeyAvailable) Console.ReadKey(true);

                    isStun = false;
                    GameManger.player.direction = 5;
                }

                //종료
                if (isQuit)
                {
                    GameManger.buffer.printTimer.Dispose();
                    return;
                }

                if (GameManger.currField.type == 1)
                {
                    //이동
                    if (isMove)
                    {
                        if (!GameManger.currField.isMenu)
                        {
                            isStun = GameManger.currField.Move(GameManger.currFieldPos);
                        }
                        else
                        {
                            if (GameManger.map.isInventory)
                            {
                                //GameManger.map.cursor.x = GameManger.map.cursor.x + axisX[GameManger.player.direction];
                                GameManger.map.cursor.y = GameManger.map.cursor.y + axisY[GameManger.player.direction];

                                //구매창
                                if (GameManger.player.inventory.Count > 10)
                                {
                                    if (GameManger.map.cursor.y < 0)
                                    {
                                        GameManger.map.cursor.y = 0;
                                        GameManger.player.startInventoryIndex -= 1;
                                        if (GameManger.player.startInventoryIndex < 0)
                                        {
                                            GameManger.player.startInventoryIndex = 0;
                                        }
                                    }
                                    else if (GameManger.map.cursor.y >= 10)
                                    {
                                        GameManger.map.cursor.y = 9;
                                        GameManger.player.startInventoryIndex += 1;
                                        if (GameManger.player.inventory.Count - GameManger.player.startInventoryIndex < 10)
                                        {
                                            GameManger.player.startInventoryIndex = GameManger.player.inventory.Count - 10;
                                        }
                                    }

                                }
                                else
                                {
                                    if (GameManger.map.cursor.y < 0)
                                    {
                                        GameManger.map.cursor.y = 0;
                                    }
                                    else if (GameManger.map.cursor.y >= GameManger.player.inventory.Count)
                                    {
                                        GameManger.map.cursor.y = GameManger.player.inventory.Count - 1;
                                    }
                                }

                            }
                            else if (GameManger.map.isEquip)
                            {
                                GameManger.map.cursor.y = GameManger.map.cursor.y + axisY[GameManger.player.direction];
                                if (GameManger.map.cursor.y < 0)
                                {
                                    GameManger.map.cursor.y = 2;
                                }
                                else if (GameManger.map.cursor.y > 2)
                                {
                                    GameManger.map.cursor.y = 0;
                                }
                            }
                            else
                            {
                                GameManger.map.cursor.y = GameManger.map.cursor.y + axisY[GameManger.player.direction];
                                if (GameManger.map.cursor.y < 0)
                                {
                                    GameManger.map.cursor.y = 2;
                                }
                                else if (GameManger.map.cursor.y > 2)
                                {
                                    GameManger.map.cursor.y = 0;
                                }
                            }
                        }
                    }

                    //메뉴창
                    if (isNo)
                    {
                        if (!GameManger.currField.isMenu)
                        {
                            GameManger.map.isInventory = false;
                            GameManger.map.isEquip = false;

                            GameManger.currField.isMenu = true;
                            GameManger.currField.StopEnemies();
                            GameManger.currField.ReturnSelfToForest().createTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                            GameManger.player.startInventoryIndex = 0;
                            GameManger.map.cursor = new Coordinate(0, 0);
                        }

                        else
                        {
                            if (GameManger.map.isInventory)
                            {
                                GameManger.map.isInventory = false;
                                GameManger.player.startInventoryIndex = 0;
                                GameManger.map.cursor = new Coordinate(0, 0);
                                continue;
                            }
                            if (GameManger.map.isEquip)
                            {
                                GameManger.map.isEquip = false;
                                GameManger.player.startInventoryIndex = 0;
                                GameManger.map.cursor = new Coordinate(0, 0);
                                continue;
                            }

                            GameManger.currField.isMenu = false;
                            GameManger.currField.PlayEnemies();
                            GameManger.currField.ReturnSelfToForest().createTimer.Change(100, 10000);
                        }
                    }

                    //공격
                    if (isYes)
                    {
                        if (!GameManger.currField.isMenu) 
                        {
                            if (!GameManger.player.isMeleeDelay)
                            {
                                GameManger.player.isMeleeDelay = true;
                                GameManger.player.meleeDelay = new Timer(GameManger.player.DelayMeleeTimer, null, 1000, 0);

                                int nextX = GameManger.player.Hold(GameManger.player.GetNextX(GameManger.player.direction), FieldBase._FIELD_SIZE);
                                int nextY = GameManger.player.Hold(GameManger.player.GetNextY(GameManger.player.direction), FieldBase._FIELD_SIZE);

                                GameManger.player.Effects.Add(new Effect(nextX, nextY, 0));
                            

                                //적 공격
                                Enemy currEnemy = GameManger.currField.FindEnemiesAt(nextX, nextY);
                                if (currEnemy != null)
                                {
                                    if (currEnemy.isLive)
                                    {
                                        currEnemy.hitPoint -= currEnemy.hitPoint;
                                        if (currEnemy.hitPoint <= 0)
                                        {
                                            GameManger.currField.RemoveEnemy(nextX, nextY);

                                            if (GameManger.currField.GetEnemies().Count == 0)
                                            {
                                                isWin = true;
                                            }
                                        }
                                    }
                                    continue;
                                }


                                //벽 공격
                                if (GameManger.currField.fieldInfo[nextY, nextX] == FieldBase.field_info.tree)
                                {
                                    Items tool = GameManger.player.inventory.Find(x => x.itemId == 1);

                                    if (GameManger.player.inventory.Find(x => x.itemId == 1) != null)
                                    {
                                        tool.count -= 1;
                                        if (tool.count <= 0)
                                        {
                                            GameManger.player.inventory.Remove(tool);
                                        }
                                        GameManger.currField.fieldInfo[nextY, nextX] = 0;
                                        continue;
                                    }
                                }

                                //함정 공격
                                if (GameManger.currField.fieldInfo[nextY, nextX] == FieldBase.field_info.mud)
                                {
                                    GameManger.currField.fieldInfo[nextY, nextX] = 0;

                                    continue;
                                }

                            }
                        }
                    
                        if (GameManger.currField.isMenu)
                        {
                            if (!GameManger.map.isInventory && !GameManger.map.isEquip && GameManger.map.cursor.y == 1)
                            {
                                GameManger.map.isMinimap = !GameManger.map.isMinimap;

                                GameManger.currField.isMenu = false;
                            }

                            if (!GameManger.map.isInventory && GameManger.map.cursor.y == 0)
                            {
                                GameManger.map.isInventory = true;
                                GameManger.player.startInventoryIndex = 0;
                                GameManger.map.cursor = new Coordinate(0, 0);
                            }

                            if (!GameManger.map.isEquip && GameManger.map.cursor.y == 2)
                            {
                                GameManger.map.isEquip = true;
                                GameManger.player.startInventoryIndex = 0;
                                GameManger.map.cursor = new Coordinate(0, 0);
                            }
                        }
                    }
                }

                else if (GameManger.currField.type == 2)
                {
                    Town currTown = GameManger.currField.ReturnSelfToTown();
                    //커서 이동
                    if (isMove)
                    {
                        currTown.Move();

                    }

                    //확인
                    if (isYes)
                    {
                        if (currTown.mainPosition == 0)
                        {
                            currTown.mainPosition = (currTown.cursorPosition.y + 1);
                            currTown.cursorPosition.x = 0;
                            currTown.cursorPosition.y = 0;

                            currTown.startShopIndex = 0;
                            GameManger.player.startInventoryIndex = 0;

                            if (!isFirst)
                            {
                                isFirst = true;
                            }
                        }
                        else if (currTown.mainPosition == 1)
                        {
                            if (isFirst)
                            {
                            }

                            //구매
                            if (currTown.cursorPosition.x == 0)
                            {
                                Items item = currTown.shop[(currTown.cursorPosition.y + currTown.startShopIndex)];

                                if (item.count == 0)
                                {
                                }
                                else if (GameManger.player.gold < (int)(item.price * currTown.priceRate[item.itemId].currRate))
                                {
                                }
                                else
                                {
                                    //마차구매시
                                    if (item.itemId == 3)
                                    {
                                        if (GameManger.player.wagonList.Count < Wagon.wagonCountMax)
                                        {
                                            GameManger.player.wagonList.Add(new Wagon());
                                            GameManger.player.maxWeight += Wagon.wagonWeightMax;
                                            currTown.gold += (int)(item.price * currTown.priceRate[item.itemId].currRate);
                                            GameManger.player.gold -= (int)(item.price * currTown.priceRate[item.itemId].currRate);
                                            continue;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                    List<Items> currInventory = null;
                                    Wagon currWagon = null;

                                    //현재 여유분이 존재할거 같다.
                                    if (GameManger.player.maxWeight >= item.weight + GameManger.player.SumWeight())
                                    {
                                        //1. 현재 플레이어의 인벤토리와 비교
                                        if (Player.playerWeightMax >= item.weight + GameManger.player.weight)
                                        {
                                            currInventory = GameManger.player.inventory;
                                        }
                                        //2. 웨건 리스트와 비교
                                        else if (GameManger.player.wagonList.Count > 0)
                                        {
                                            foreach (var tmp in GameManger.player.wagonList)
                                            {
                                                //발견시 바로 그 웨건 인벤토리 사용
                                                if (Wagon.wagonWeightMax >= item.weight + tmp.weight)
                                                {
                                                    currWagon = tmp;
                                                    currInventory = tmp.inventory;
                                                    break;
                                                }
                                            }

                                            //웨건 인벤토리에 여유분이 있지만 나눠져서 있을경우 null이 되어서 구매 실패
                                            if (currInventory == null) continue;
                                        }
                                        //혹시모를 예외사항
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    //여유분이 안될경우
                                    else
                                    {
                                        continue;
                                    }

                                    //무게 증가
                                    if (currWagon == null)
                                    {
                                        GameManger.player.weight += item.weight;
                                    }
                                    else
                                    {
                                        currWagon.weight += item.weight;
                                    }

                                    //골드 교환
                                    currTown.gold += (int)(item.price * currTown.priceRate[item.itemId].currRate);
                                    GameManger.player.gold -= (int)(item.price * currTown.priceRate[item.itemId].currRate);

                                    //수량 감소
                                    item.count -= 1;

                                    if (item.itemId == 100)
                                    {

                                        currTown.Exit();

                                        GameManger.currField = new Ending(2);
                                    }
                                    else
                                    {
                                        if (!item.isOwn && item.count == 0)
                                        {
                                            currTown.shop.RemoveAt(currTown.cursorPosition.y + currTown.startShopIndex);
                                            if (currTown.cursorPosition.y + currTown.startShopIndex > currTown.shop.Count)
                                            {
                                                currTown.startShopIndex -= 1;
                                                if (currTown.startShopIndex < 0)
                                                {
                                                    currTown.startShopIndex = 0;
                                                }
                                            }
                                        }
                                    }

                                    //퀄리티 max인 같은 아이템에 저장
                                    int itemIndex;
                                    //사용아이템 무조건 한곳에 모음
                                    if (item.itemId < 10)
                                    {
                                        itemIndex = currInventory.FindIndex(x => x.itemId == item.itemId && x.quality == int.MaxValue);
                                    }
                                    else
                                    {
                                        itemIndex = currInventory.FindIndex(x => x.itemId == item.itemId && x.quality == 1);
                                    }

                                    if (itemIndex == -1)
                                    {
                                        currInventory.Add(new Items(item));
                                    }
                                    else
                                    {
                                        currInventory[itemIndex].count += 1;
                                    }

                                    currInventory.OrderBy(x => x.itemId);
                                }
                            }
                            //판매
                            else if (currTown.cursorPosition.x >= 1)
                            {
                                Items item = null;
                                List<Items> currInventory = null;
                                Wagon currWagon = null;

                                if (currTown.cursorPosition.x == 1)
                                {
                                    currInventory = GameManger.player.inventory;
                                    if (currInventory == null || currInventory.Count == 0) continue;
                                    item = currInventory[(currTown.cursorPosition.y + GameManger.player.startInventoryIndex)];
                                }
                                else
                                {
                                    currWagon = GameManger.player.wagonList[currTown.cursorPosition.x - 2];
                                    currInventory = currWagon.inventory; 
                                    if (currInventory == null || currInventory.Count == 0) continue;
                                    item = currInventory[(currTown.cursorPosition.y + currWagon.startWagonInvenIndex)];
                                }

                                if (item.count == 0)
                                {
                                }
                                else if (currTown.gold < (int)(item.price * currTown.priceRate[item.itemId].currRate * 0.7))
                                {
                                }
                                else
                                {
                                    if (currTown.cursorPosition.x == 1)
                                    {
                                        //무게 감소
                                        GameManger.player.weight -= item.weight;
                                    }
                                    else
                                    {
                                        currWagon.weight -= item.weight;
                                    }

                                    //골드 교환
                                    if (item.type == 2)
                                    {
                                        currTown.gold -= (int)(item.price * currTown.priceRate[item.itemId].currRate * 0.7 * item.quality);
                                        GameManger.player.gold += (int)(item.price * currTown.priceRate[item.itemId].currRate * 0.7 * item.quality);
                                    }
                                    else
                                    {
                                        currTown.gold -= (int)(item.price * currTown.priceRate[item.itemId].currRate * 0.7);
                                        GameManger.player.gold += (int)(item.price * currTown.priceRate[item.itemId].currRate * 0.7);
                                    }

                                    //아이템 감소
                                    item.count -= 1;
                                    if (item.count == 0)
                                    {
                                        currInventory.Remove(item);
                                        if (currTown.cursorPosition.x == 1)
                                        {
                                            if (currTown.cursorPosition.y + GameManger.player.startInventoryIndex > currInventory.Count)
                                            {
                                                GameManger.player.startInventoryIndex -= 1;
                                                if (GameManger.player.startInventoryIndex < 0)
                                                {
                                                    GameManger.player.startInventoryIndex = 0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (currTown.cursorPosition.y + currWagon.startWagonInvenIndex > currInventory.Count)
                                            {
                                                currWagon.startWagonInvenIndex -= 1;
                                                if (currWagon.startWagonInvenIndex < 0)
                                                {
                                                    currWagon.startWagonInvenIndex = 0;
                                                }
                                            }
                                        }

                                        if (currTown.cursorPosition.y >= currInventory.Count)
                                        {
                                            currTown.cursorPosition.y = currInventory.Count - 1;
                                        }
                                    }

                                    //퀄리티가 같은 아이템 모아서 저장
                                    int itemIndex = currTown.shop.FindIndex(x => x.itemId == item.itemId && x.quality == item.quality);
                                    if (itemIndex == -1)
                                    {
                                        currTown.shop.Add(new Items(item));
                                    }
                                    else
                                    {
                                        currTown.shop[itemIndex].count += 1;
                                    }
                                    currTown.shop.OrderBy(x => x.isOwn==true);
                                }
                            }
                        }
                        else if (currTown.mainPosition == 2)
                        {
                        }
                        else if (currTown.mainPosition == 3)
                        {                            
                            if (currTown.cursorPosition.y == 0){
                                if (GameManger.player.gold >= 5)
                                {
                                    GameManger.map.SetDayTimer(null);
                                    GameManger.player.hitPoint = GameManger.player.hitPointMax;
                                    GameManger.player.gold -= 5;

                                    currTown.mainPosition = 0;
                                    currTown.cursorPosition.x = 0;
                                    currTown.cursorPosition.y = 0;
                                }
                            }
                        }
                        else if (currTown.mainPosition == 4)
                        {
                            if (GameManger.currField.portals[currTown.cursorPosition.y] != null)
                            {

                                GameManger.currFieldPos.y += axisY[currTown.cursorPosition.y];
                                GameManger.currFieldPos.x += axisX[currTown.cursorPosition.y];
                                GameManger.currField.isFog = false;
                                GameManger.currField.isCurrField = false;

                                currTown.Exit();

                                GameManger.currField = GameManger.map.worldMap[GameManger.currFieldPos.y, GameManger.currFieldPos.x];

                                if (currTown.cursorPosition.y == 0)
                                {
                                    GameManger.player.Axis2D.x = GameManger.currField.portals[1].axis.x;
                                    GameManger.player.Axis2D.y = GameManger.currField.portals[1].axis.y;
                                }
                                else if (currTown.cursorPosition.y == 1)
                                {
                                    GameManger.player.Axis2D.x = GameManger.currField.portals[0].axis.x;
                                    GameManger.player.Axis2D.y = GameManger.currField.portals[0].axis.y;
                                }
                                else if (currTown.cursorPosition.y == 2)
                                {
                                    GameManger.player.Axis2D.x = GameManger.currField.portals[3].axis.x;
                                    GameManger.player.Axis2D.y = GameManger.currField.portals[3].axis.y;
                                }
                                else if (currTown.cursorPosition.y == 3)
                                {
                                    GameManger.player.Axis2D.x = GameManger.currField.portals[2].axis.x;
                                    GameManger.player.Axis2D.y = GameManger.currField.portals[2].axis.y;
                                }

                                if (GameManger.currField.type == 1)
                                {
                                    GameManger.currField.PlayEnemies();
                                    GameManger.currField.SetCreateTimer(new Timer(GameManger.currField.CreateEnemy, null, 100, 10000));
                                }
                                GameManger.currField.isFog = false;
                                GameManger.currField.isCurrField = true;


                                GameManger.player.RemoveFog();

                                for (int i = 0; i < GameManger.player.wagonList.Count; i++)
                                {
                                    GameManger.player.wagonList[i].Axis2D.x = GameManger.player.Axis2D.x;
                                    GameManger.player.wagonList[i].Axis2D.y = GameManger.player.Axis2D.y;
                                }
                            }
                        }
                    }

                    //취소
                    if (isNo)
                    {
                        if (currTown.mainPosition != 0)
                        {
                            currTown.mainPosition = 0;
                            currTown.cursorPosition.x = 0;
                            currTown.cursorPosition.y = 0;
                        }
                    }

                    //커서 방향 초기화
                    GameManger.player.direction = 5;
                }


                else if (GameManger.currField.type == 3)
                {

                    //패배
                    if (!GameManger.player.isLive)
                    {
                        GameManger.buffer.printTimer.Dispose();
                        return;
                    }

                    //이동
                    if (isMove)
                    {
                        isStun = GameManger.currField.Move();
                    }

                    //원거리 공격
                    if (isNo)
                    {
                        if (GameManger.player.direction >= 0 && GameManger.player.direction <= 3)
                        {
                            if (GameManger.player.bulletCount < GameManger.player.bulletCountMax)
                            {
                                if (!GameManger.player.isRangeDelay)
                                {
                                    GameManger.player.isRangeDelay = true;
                                    GameManger.player.rangeDelay = new Timer(GameManger.player.DelayRangeTimer, null, 300, 0);

                                    GameManger.player.Effects.Add(new Effect(GameManger.player.Axis2D.x, GameManger.player.Axis2D.y, 1, GameManger.player.direction));
                                    GameManger.player.bulletCount += 1;
                                }
                            }
                        }
                    }

                    //근접 공격
                    if (isYes)
                    {
                        if (!GameManger.player.isMeleeDelay)
                        {
                            GameManger.player.isMeleeDelay = true;
                            GameManger.player.meleeDelay = new Timer(GameManger.player.DelayMeleeTimer, null, 450, 0);

                            int nextX = GameManger.player.GetNextX(GameManger.player.direction);
                            int nextY = GameManger.player.GetNextY(GameManger.player.direction);

                            GameManger.player.Effects.Add(new Effect(nextX, nextY, 1));

                            if (GameManger.player.weapon == 1)
                            {
                                if (GameManger.player.direction / 2 == 0)//양쪽 보는중
                                {
                                    GameManger.player.Effects.Add(new Effect(nextX, nextY + 1, 1));
                                    GameManger.player.Effects.Add(new Effect(nextX, nextY - 1, 1));
                                }
                                else if (GameManger.player.direction / 2 == 1)//위쪽 보는중
                                {
                                    GameManger.player.Effects.Add(new Effect(nextX + 1, nextY, 1));
                                    GameManger.player.Effects.Add(new Effect(nextX - 1, nextY, 1));
                                }
                            }
                            else if (GameManger.player.weapon == 2)
                            {
                                if (GameManger.player.direction == 0)//오른쪽 보는중
                                {
                                    //AttackEffect.Add(new MyEffect(nextX + 1, nextY, 0));
                                    //AttackEffect.Add(new MyEffect(nextX + 2, nextY, 0));
                                }
                                else if (GameManger.player.direction == 1)//왼쪽 보는중
                                {
                                    //AttackEffect.Add(new MyEffect(nextX - 1, nextY, 0));
                                    //AttackEffect.Add(new MyEffect(nextX - 2, nextY, 0));
                                }
                                else if (GameManger.player.direction == 2)//위쪽 보는중
                                {
                                    //AttackEffect.Add(new MyEffect(nextX, nextY - 1, 0));
                                    //AttackEffect.Add(new MyEffect(nextX, nextY - 2, 0));
                                }
                                else if (GameManger.player.direction == 3)//아래쪽 보는중
                                {
                                    //AttackEffect.Add(new MyEffect(nextX, nextY + 1, 0));
                                    //AttackEffect.Add(new MyEffect(nextX, nextY + 2, 0));
                                }
                            }

                        }

                        for (int i = 0; i < GameManger.player.Effects.Count; i++)
                        {
                            if (GameManger.player.Effects[i].type == -1) continue;
                            if (GameManger.player.Effects[i] == null) continue;

                            int nextX = GameManger.player.Effects[i].Axis2D.x;
                            int nextY = GameManger.player.Effects[i].Axis2D.y;

                            //적 공격
                            Enemy currEnemy = GameManger.currField.FindEnemiesAt(nextX, nextY);
                            if (currEnemy != null)
                            {
                                if (currEnemy.isLive)
                                {
                                    currEnemy.hitPoint -= GameManger.player.attckPoint;
                                    currEnemy.moveTimer.Change(300, 600 - (400 * ((Enemy.EnemyHitPointMAX - currEnemy.hitPoint) / Enemy.EnemyHitPointMAX)));
                                    if (currEnemy.hitPoint <= 0)
                                    {
                                        GameManger.currField.RemoveEnemy(nextX, nextY);

                                        if (GameManger.currField.GetEnemies().Count == 0)
                                        {
                                            isWin = true;
                                        }
                                    }
                                }
                                continue;
                            }

                        }
                    }

                    if (!GameManger.currField.ReturnSelfToBattle().isWin && isWin)
                    {
                        GameManger.currField.ReturnSelfToBattle().beforePlayerInfo.hitPoint = GameManger.player.hitPoint;
                        GameManger.currField.StopEnemies();

                        //Utility.currRoom.enemyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                        GameManger.player = GameManger.currField.ReturnSelfToBattle().beforePlayerInfo;
                        GameManger.currField = GameManger.currField.ReturnSelfToBattle().beforeFieldInfo;
                        if (GameManger.currField.type == 1)
                        {
                            GameManger.currField.PlayEnemies();
                            GameManger.currField.SetCreateTimer(new Timer(GameManger.currField.CreateEnemy, null, 100, 10000));
                        }
                        GameManger.currField.isFog = false;
                        GameManger.currField.isCurrField = true;
                        isWin = false;

                        GameManger.player.RemoveFog();
                    }
                }
                else if (GameManger.currField.type == 4)
                {
                }

            }
        }

        public void SetPrintMap(Object obj)
        {
            GameManger.currField.ConvertMapToString(ref line);
            if (!GameManger.buffer.isWork)
            {
                GameManger.buffer.SetBuffer((string[])obj);
                GameManger.buffer.PrintBuffer();
            }
            else
            {
                Console.Clear();
            }
        }
    }
}
