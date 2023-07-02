using Project1st.Game.Core;
using Project1st.Game.GameObject;
using Project1st.Game.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Project1st.Game.Map.Fields
{


    public class Town : FieldBase
    {
        public List<Items> shop;
        public List<PubEvent> pubEvents;

        //2.00 ~ 0.00
        public Dictionary<int, PriceRate> priceRate;

        public int startShopIndex;

        public int gold;
        public int mainPosition;
        public Coordinate cursorPosition;

        public Town()
        {
            gold = 2000;
            pubEvents = new List<PubEvent>();
            shop = new List<Items>();
            priceRate = new Dictionary<int, PriceRate>();
            type = 2;
            mainPosition = 0;
            cursorPosition = new Coordinate(0, 0);
            startShopIndex = 0;
            for (int i = 0; i < 4; i++)
            {
                if (portals[i] != null)
                {
                    fieldInfo[portals[i].axis.y, portals[i].axis.x] = field_info.portal;

                }
            }
            for (int i = 0; i < 60; i++)
            {
                priceRate.Add(i, new PriceRate());
            }

            priceRate.Add(100, new PriceRate(1));
        }

        public Town(FieldBase field)
        {
            isFog = field.isFog;
            gold = 2000;
            pubEvents = new List<PubEvent>();
            shop = new List<Items>();
            priceRate = new Dictionary<int, PriceRate>();
            type = 2;
            mainPosition = 0;
            cursorPosition = new Coordinate(0, 0);
            startShopIndex = 0;

            portals = field.portals;
            for (int i = 0; i < 4; i++)
            {
                if (portals[i] != null)
                {
                    fieldInfo[portals[i].axis.y, portals[i].axis.x] = field_info.portal;

                }
            }


            //기본 판매 아이템
            shop.Add(GameManger.db.database[0]);
            shop.Add(GameManger.db.database[1]);
            shop.Add(GameManger.db.database[2]);
            shop.Add(GameManger.db.database[3]);

            //무기
            int count = GameManger.random.Next(3);
            for (int i = 0; i < count; i++)
            {
                int id = GameManger.random.Next(4, 9);
                if (shop.Find(x => x.itemId == id)==null)
                {
                    shop.Add(GameManger.db.database[id]);
                }
            }

            //판매물품
            count = GameManger.random.Next(2, 7);
            for (int i = 0; i < count; i++)
            {
                int id = GameManger.random.Next(10, 51);
                if (shop.Find(x => x.itemId == id) == null)
                {
                    shop.Add(GameManger.db.database[id]);
                }
            }

            for (int i = 0; i < 60; i++)
            {
                if (shop.Find(x => x.itemId == i) == null)
                {
                    priceRate.Add(i, new PriceRate(.5f));
                }
                else
                {
                    priceRate.Add(i, new PriceRate(1.1f));
                }
            }
            priceRate.Add(100, new PriceRate(1));
        }

        public override void InitEnter()
        {
            mainPosition = 0;
            cursorPosition = new Coordinate(0, 0);
            GameManger.worldMap.dayTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        public override void Exit()
        {
            GameManger.worldMap.dayTimer.Change(100, 60000);
        }

        public override void PressMenuEvent()
        {

            if (mainPosition == 0)
            {
                cursorPosition.y = cursorPosition.y + WorldMap._AXIS_MATRIX_Y[GameManger.player.direction];

                if (cursorPosition.y < 0)
                {
                    cursorPosition.y = 3;
                }
                else if (cursorPosition.y > 3)
                {
                    cursorPosition.y = 0;
                }
            }

            else if (mainPosition == 1)
            {
                cursorPosition.x = cursorPosition.x + WorldMap._AXIS_MATRIX_X[GameManger.player.direction];
                cursorPosition.y = cursorPosition.y + WorldMap._AXIS_MATRIX_Y[GameManger.player.direction];

                if (cursorPosition.x < 0)
                {
                    cursorPosition.x = 0;
                }
                else if (cursorPosition.x > GameManger.player.wagonList.Count + 1)
                {
                    cursorPosition.x = GameManger.player.wagonList.Count + 1;
                }

                //구매창
                if (cursorPosition.x == 0)
                {
                    if (shop.Count > 30)
                    {
                        if (cursorPosition.y < 0)
                        {
                            cursorPosition.y = 0;
                            startShopIndex -= 1;
                            if (startShopIndex < 0)
                            {
                                startShopIndex = 0;
                            }
                        }
                        else if (cursorPosition.y >= 30)
                        {
                            cursorPosition.y = 29;
                            startShopIndex += 1;
                            if (shop.Count - startShopIndex < 30)
                            {
                                startShopIndex = shop.Count - 30;
                            }
                        }

                    }
                    else
                    {
                        if (cursorPosition.y < 0)
                        {
                            cursorPosition.y = 0;
                        }
                        else if (cursorPosition.y >= shop.Count)
                        {
                            cursorPosition.y = shop.Count - 1;
                        }
                    }
                }
                //판매창
                else if (cursorPosition.x >= 1)
                {
                    List<Items> currInventory = null;
                    Wagon currWagon = null;

                    if (cursorPosition.x == 1)
                    {
                        currInventory = GameManger.player.inventory;
                        if (currInventory == null || currInventory.Count == 0) { }
                    }
                    else
                    {
                        currWagon = GameManger.player.wagonList[cursorPosition.x - 2];
                        currInventory = currWagon.inventory;
                        if (currInventory == null || currInventory.Count == 0) { }
                    }

                    if (currInventory.Count > 30)
                    {
                        if (cursorPosition.y < 0)
                        {
                            cursorPosition.y = 0;

                            if (cursorPosition.x == 1)
                            {
                                GameManger.player.startInvenIndex -= 1;
                                if (GameManger.player.startInvenIndex < 0)
                                {
                                    GameManger.player.startInvenIndex = 0;
                                }
                            }
                            else
                            {
                                currWagon.startInvenIndex -= 1;
                                if (currWagon.startInvenIndex < 0)
                                {
                                    currWagon.startInvenIndex = 0;
                                }
                            }
                        }
                        else if (cursorPosition.y >= 30)
                        {
                            cursorPosition.y = 29;
                            if (cursorPosition.x == 1)
                            {
                                GameManger.player.startInvenIndex -= 1;
                                if (GameManger.player.startInvenIndex < 0)
                                {
                                    GameManger.player.startInvenIndex = 0;
                                }
                                if (currInventory.Count - GameManger.player.startInvenIndex < 30)
                                {
                                    GameManger.player.startInvenIndex = currInventory.Count - 30;
                                }
                            }
                            else
                            {
                                currWagon.startInvenIndex -= 1;
                                if (currWagon.startInvenIndex < 0)
                                {
                                    currWagon.startInvenIndex = 0;
                                }
                                if (currInventory.Count - currWagon.startInvenIndex < 30)
                                {
                                    currWagon.startInvenIndex = currInventory.Count - 30;
                                }
                            }
                        }

                    }
                    else
                    {
                        if (cursorPosition.y < 0)
                        {
                            cursorPosition.y = 0;
                        }
                        else if (cursorPosition.y > currInventory.Count - 1 && currInventory.Count == 0)
                        {
                            cursorPosition.y = 0;
                        }
                        else if (cursorPosition.y > currInventory.Count - 1 && currInventory.Count > 0)
                        {
                            cursorPosition.y = currInventory.Count - 1;
                        }
                    }
                }
            }
            else if (mainPosition == 2)
            {
                cursorPosition.y = cursorPosition.y + WorldMap._AXIS_MATRIX_Y[GameManger.player.direction];
                if (cursorPosition.y < 0)
                {
                    cursorPosition.y = 1;
                }
                else if (cursorPosition.y > 1)
                {
                    cursorPosition.y = 0;
                }
            }
            else if (mainPosition == 3)
            {
                cursorPosition.y = cursorPosition.y + WorldMap._AXIS_MATRIX_Y[GameManger.player.direction];
                if (cursorPosition.y < 0)
                {
                    cursorPosition.y = 0;
                }
                else if (cursorPosition.y > 0)
                {
                    cursorPosition.y = 0;
                }
            }
            else if (mainPosition == 4)
            {
                cursorPosition.y = cursorPosition.y + WorldMap._AXIS_MATRIX_Y[GameManger.player.direction];

                if (cursorPosition.y < 0)
                {
                    cursorPosition.y = 4 - 1;
                }
                else if (cursorPosition.y > 4 - 1)
                {
                    cursorPosition.y = 0;
                }

            }
        }

        public override bool PressYesEvent()
        {

            if (mainPosition == 0)
            {
                mainPosition = (cursorPosition.y + 1);
                cursorPosition.x = 0;
                cursorPosition.y = 0;

                startShopIndex = 0;
                GameManger.player.startInvenIndex = 0;
            }
            else if (mainPosition == 1)
            {

                //구매
                if (cursorPosition.x == 0)
                {
                    Items item = shop[(cursorPosition.y + startShopIndex)];

                    if (item.count == 0)
                    {
                    }
                    else if (GameManger.player.gold < (int)(item.price * priceRate[item.itemId].currRate))
                    {
                    }
                    else
                    {
                        //마차구매시
                        if (item.itemId == 3)
                        {
                            if (GameManger.player.wagonList.Count < Wagon._WAGON_COUNT_MAX)
                            {
                                GameManger.player.wagonList.Add(new Wagon());
                                GameManger.player.maxWeightSum += Wagon._WAGON_WEIGHT_MAX;
                                gold += (int)(item.price * priceRate[item.itemId].currRate);
                                GameManger.player.gold -= (int)(item.price * priceRate[item.itemId].currRate);
                                return false;
                            }
                            else
                            {
                                return false;
                            }
                        }

                        List<Items> currInventory = null;
                        Wagon currWagon = null;

                        //현재 여유분이 존재할거 같다.
                        if (GameManger.player.maxWeightSum >= item.weight + GameManger.player.SumWeight())
                        {
                            //1. 현재 플레이어의 인벤토리와 비교
                            if (Player._PLAYER_WEIGHT_MAX >= item.weight + GameManger.player.weight)
                            {
                                currInventory = GameManger.player.inventory;
                            }
                            //2. 웨건 리스트와 비교
                            else if (GameManger.player.wagonList.Count > 0)
                            {
                                foreach (var tmp in GameManger.player.wagonList)
                                {
                                    //발견시 바로 그 웨건 인벤토리 사용
                                    if (Wagon._WAGON_WEIGHT_MAX >= item.weight + tmp.weight)
                                    {
                                        currWagon = tmp;
                                        currInventory = tmp.inventory;
                                        break;
                                    }
                                }

                                //웨건 인벤토리에 여유분이 있지만 나눠져서 있을경우 null이 되어서 구매 실패
                                if (currInventory == null)
                                {
                                    return false;
                                }
                            }
                            //혹시모를 예외사항
                            else
                            {
                                return false;
                            }
                        }
                        //여유분이 안될경우
                        else
                        {
                            return false;
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
                        gold += (int)(item.price * priceRate[item.itemId].currRate);
                        GameManger.player.gold -= (int)(item.price * priceRate[item.itemId].currRate);

                        //수량 감소
                        item.count -= 1;

                        if (item.itemId == 100)
                        {

                            Exit();

                            GameManger.currField = new Ending(2);
                        }
                        else
                        {
                            if (!item.isOwn && item.count == 0)
                            {
                                shop.RemoveAt(cursorPosition.y + startShopIndex);
                                if (cursorPosition.y + startShopIndex > shop.Count)
                                {
                                    startShopIndex -= 1;
                                    if (startShopIndex < 0)
                                    {
                                        startShopIndex = 0;
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
                else if (cursorPosition.x >= 1)
                {
                    Items item = null;
                    List<Items> currInventory = null;
                    Wagon currWagon = null;

                    if (cursorPosition.x == 1)
                    {
                        currInventory = GameManger.player.inventory;
                        if (currInventory == null || currInventory.Count == 0)
                        {
                            return false;
                        }
                        item = currInventory[(cursorPosition.y + GameManger.player.startInvenIndex)];
                    }
                    else
                    {
                        currWagon = GameManger.player.wagonList[cursorPosition.x - 2];
                        currInventory = currWagon.inventory;
                        if (currInventory == null || currInventory.Count == 0)
                        {
                            return false;
                        }
                        item = currInventory[(cursorPosition.y + currWagon.startInvenIndex)];
                    }

                    if (item.count == 0)
                    {
                    }
                    else if (gold < (int)(item.price * priceRate[item.itemId].currRate * 0.7))
                    {
                    }
                    else
                    {
                        if (cursorPosition.x == 1)
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
                            gold -= (int)(item.price * priceRate[item.itemId].currRate * 0.7 * item.quality);
                            GameManger.player.gold += (int)(item.price * priceRate[item.itemId].currRate * 0.7 * item.quality);
                        }
                        else
                        {
                            gold -= (int)(item.price * priceRate[item.itemId].currRate * 0.7);
                            GameManger.player.gold += (int)(item.price * priceRate[item.itemId].currRate * 0.7);
                        }

                        //아이템 감소
                        item.count -= 1;
                        if (item.count == 0)
                        {
                            currInventory.Remove(item);
                            if (cursorPosition.x == 1)
                            {
                                if (cursorPosition.y + GameManger.player.startInvenIndex > currInventory.Count)
                                {
                                    GameManger.player.startInvenIndex -= 1;
                                    if (GameManger.player.startInvenIndex < 0)
                                    {
                                        GameManger.player.startInvenIndex = 0;
                                    }
                                }
                            }
                            else
                            {
                                if (cursorPosition.y + currWagon.startInvenIndex > currInventory.Count)
                                {
                                    currWagon.startInvenIndex -= 1;
                                    if (currWagon.startInvenIndex < 0)
                                    {
                                        currWagon.startInvenIndex = 0;
                                    }
                                }
                            }

                            if (cursorPosition.y >= currInventory.Count)
                            {
                                cursorPosition.y = currInventory.Count - 1;
                            }
                        }

                        //퀄리티가 같은 아이템 모아서 저장
                        int itemIndex = shop.FindIndex(x => x.itemId == item.itemId && x.quality == item.quality);
                        if (itemIndex == -1)
                        {
                            shop.Add(new Items(item));
                        }
                        else
                        {
                            shop[itemIndex].count += 1;
                        }
                        shop.OrderBy(x => x.isOwn == true);
                    }
                }
            }
            else if (mainPosition == 2)
            {
                if (cursorPosition.y == 1)
                {
                    if (pubEvents.Count > 10)
                    {
                        return false;
                    }
                    if (GameManger.player.gold >= 500)
                    {
                        GameManger.player.gold -= 500;

                        PubEvent events = new PubEvent();

                        events.townX = GameManger.random.Next(WorldMap._MAP_SIZE);
                        events.townY = GameManger.random.Next(WorldMap._MAP_SIZE);
                        events.type = 1;

                        if (GameManger.worldMap.map[events.townY, events.townX].type == 1)
                        {
                            GameManger.worldMap.map[events.townY, events.townX].isFog = false;
                        }
                        else if (GameManger.worldMap.map[events.townY, events.townX].type == 2)
                        {
                            GameManger.worldMap.map[events.townY, events.townX].isFog = false;
                        }
                        pubEvents.Add(events);
                    }
                }
                else if (cursorPosition.y == 0)
                {
                    if (pubEvents.Count > 10)
                    {
                        return false;
                    }
                    if (GameManger.player.gold >= 500)
                    {
                        GameManger.player.gold -= 500;

                        PubEvent events = new PubEvent();
                        events.type = 0;

                        Town townTmp = GameManger.worldMap.townList[GameManger.random.Next(GameManger.worldMap.townList.Count)];
                        int randomItemID = GameManger.random.Next(10, 51);
                        PriceRate priceTmp = townTmp.priceRate[randomItemID];


                        for (int i = 0; i < WorldMap._MAP_SIZE; i++)
                        {
                            for (int j = 0; j < WorldMap._MAP_SIZE; j++)
                            {
                                if (GameManger.worldMap.map[i, j].Equals(townTmp))
                                {
                                    events.townX = j;
                                    events.townY = i;
                                    break;
                                }
                            }
                        }

                        events.itemId = randomItemID;
                        events.eventPrice = priceTmp;

                        pubEvents.Add(events);
                    }
                }
            }
            else if (mainPosition == 3)
            {
                if (cursorPosition.y == 0)
                {
                    if (GameManger.player.gold >= 100)
                    {
                        GameManger.worldMap.SetDayTimer(null);
                        GameManger.player.hitPoint = Player._PLAYER_HITPOINT_MAX;
                        GameManger.player.gold -= 100;

                        mainPosition = 0;
                        cursorPosition.x = 0;
                        cursorPosition.y = 0;
                    }
                }
            }
            else if (mainPosition == 4)
            {
                if (GameManger.currField.portals[cursorPosition.y] != null)
                {

                    GameManger.currFieldPos.y += WorldMap._AXIS_MATRIX_Y[cursorPosition.y];
                    GameManger.currFieldPos.x += WorldMap._AXIS_MATRIX_X[cursorPosition.y];
                    GameManger.currField.isFog = false;
                    GameManger.currField.isCurrField = false;

                    Exit();

                    GameManger.currField = GameManger.worldMap.map[GameManger.currFieldPos.y, GameManger.currFieldPos.x];

                    if (cursorPosition.y == 0)
                    {
                        GameManger.player.Axis2D.x = GameManger.currField.portals[1].axis.x;
                        GameManger.player.Axis2D.y = GameManger.currField.portals[1].axis.y;
                    }
                    else if (cursorPosition.y == 1)
                    {
                        GameManger.player.Axis2D.x = GameManger.currField.portals[0].axis.x;
                        GameManger.player.Axis2D.y = GameManger.currField.portals[0].axis.y;
                    }
                    else if (cursorPosition.y == 2)
                    {
                        GameManger.player.Axis2D.x = GameManger.currField.portals[3].axis.x;
                        GameManger.player.Axis2D.y = GameManger.currField.portals[3].axis.y;
                    }
                    else if (cursorPosition.y == 3)
                    {
                        GameManger.player.Axis2D.x = GameManger.currField.portals[2].axis.x;
                        GameManger.player.Axis2D.y = GameManger.currField.portals[2].axis.y;
                    }

                    if (GameManger.currField.type == 1)
                    {
                        GameManger.currField.InitEnter();
                    }

                    for (int i = 0; i < GameManger.player.wagonList.Count; i++)
                    {
                        GameManger.player.wagonList[i].Axis2D.x = GameManger.player.Axis2D.x;
                        GameManger.player.wagonList[i].Axis2D.y = GameManger.player.Axis2D.y;
                    }
                }
            }
            return false;
        }
        public override void PressNoEvent()
        {
            if (mainPosition != 0)
            {
                mainPosition = 0;
                cursorPosition.x = 0;
                cursorPosition.y = 0;
            }

        }


        public override string[] ConvertMapToString(ref string[] line)
        {

            for (int y = 0; y < line.Length; y++)
            {
                line[y] = "";
                if (mainPosition == 0)
                {
                    if (y == cursorPosition.y)
                    {
                        line[y] += "▶";
                    }
                    else
                    {
                        line[y] += "　";
                    }
                    switch (y)
                    {
                        case 0:
                            line[y] += " 상점\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";
                            break;
                        case 1:
                            line[y] += " 주점\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";
                            break;
                        case 2:
                            line[y] += " 여관\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";
                            break;
                        case 3:
                            line[y] += " 나가기\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";
                            break;
                        default:
                            line[y] += "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";
                            break;
                    }
                }
                else if (mainPosition == 1)
                {
                    //구매창
                    if (cursorPosition.x == 0)
                    {
                        if (y == cursorPosition.y)
                        {
                            line[y] += "▶";
                        }
                        else
                        {
                            line[y] += "　";
                        }
                    }
                    else
                    {
                        line[y] += "　";
                    }

                    if (y + startShopIndex < shop.Count && y < 30)
                    {
                        Items item = shop[y + startShopIndex];
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

                        line[y] += $"{y + startShopIndex + 1,2}" + ")";
                        line[y] += $"{item.name,-6}";
                        if (item.itemId != 100)
                        {
                            line[y] += $"{(int)(item.price * priceRate[item.itemId].currRate),-6:N0} ";
                        }
                        else
                        {
                            line[y] += $"{(int)(item.price),-6:N0} ";
                        }

                        if (item.itemId == 3)
                        {
                            line[y] += $"( - ) ";
                            line[y] += $"{Wagon._WAGON_COUNT_MAX - GameManger.player.wagonList.Count,-3}";
                        }
                        else if (item.itemId < 3)
                        {
                            line[y] += $"({item.weight,3}) ";
                            line[y] += $"{"∞",-3}";
                        }
                        else
                        {
                            line[y] += $"({item.weight,3}) ";
                            line[y] += $"{item.count,-3}";
                        }
                        line[y] += ".\t\t\t";
                    }
                    else
                    {
                        line[y] += "\t\t\t\t\t";


                    }

                    //판매창
                    if (cursorPosition.x >= 1)
                    {
                        if (y == cursorPosition.y)
                        {
                            line[y] += "▶";
                        }
                        else
                        {
                            line[y] += "　";
                        }
                    }
                    else
                    {
                        line[y] += "　";
                    }


                    Items myItem = null;
                    List<Items> currInventory = null;
                    Wagon currWagon = null;

                    if (cursorPosition.x <= 1)
                    {
                        currInventory = GameManger.player.inventory;

                        if (y + GameManger.player.startInvenIndex < currInventory.Count && y < 30)
                        {

                            if (currInventory == null || currInventory.Count == 0) continue;
                            myItem = currInventory[(y + GameManger.player.startInvenIndex)];
                            if (myItem.type == 0)
                            {
                                line[y] += ".4.";
                            }
                            else if (myItem.type == 1)
                            {
                                line[y] += ".8.";
                            }
                            else
                            {
                                line[y] += ".10.";
                            }
                            line[y] += $"{y + GameManger.player.startInvenIndex + 1,2}" + ")";
                            line[y] += $"{myItem.name,-6}";
                            if (myItem.type == 2)
                            {
                                line[y] += $"{(int)(myItem.price * priceRate[myItem.itemId].currRate * 0.7f * myItem.quality),-6:N0} ";
                            }
                            else
                            {
                                line[y] += $"{(int)(myItem.price * priceRate[myItem.itemId].currRate * 0.7f),-6:N0} ";
                            }
                            line[y] += $"({myItem.weight,3}) ";
                            line[y] += $"{myItem.count,-3}";
                            line[y] += ".\t\t\t";
                        }
                        else
                        {
                            line[y] += "\t\t\t\t\t";

                        }
                    }
                    else
                    {
                        if (GameManger.player.wagonList.Count == 0) continue;
                        currWagon = GameManger.player.wagonList[cursorPosition.x - 2];
                        currInventory = currWagon.inventory;

                        if (y + currWagon.startInvenIndex < currInventory.Count && y < 30)
                        {
                            if (currInventory == null || currInventory.Count == 0) continue;
                            myItem = currInventory[(y + currWagon.startInvenIndex)];

                            if (myItem.type == 0)
                            {
                                line[y] += ".4.";
                            }
                            else if (myItem.type == 1)
                            {
                                line[y] += ".8.";
                            }
                            else
                            {
                                line[y] += ".10.";
                            }


                            line[y] += $"{y + currWagon.startInvenIndex + 1,2}" + ")";
                            line[y] += $"{myItem.name,-6}";
                            if (myItem.type == 2)
                            {
                                line[y] += $"{(int)(myItem.price * priceRate[myItem.itemId].currRate * 0.7f * myItem.quality),-6:N0} ";
                            }
                            else
                            {
                                line[y] += $"{(int)(myItem.price * priceRate[myItem.itemId].currRate * 0.7f),-6:N0} ";
                            }
                            line[y] += $"({myItem.weight,-3})";
                            line[y] += $"{myItem.count,-3}";
                            line[y] += ".\t\t\t";
                        }
                        else
                        {
                            line[y] += "\t\t\t\t\t";

                        }
                    }

                    /*if (y + GameManger.player.startInventoryIndex < GameManger.player.inventory.Count && y < 30)
                    {
                        Items item = GameManger.player.inventory[y + GameManger.player.startInventoryIndex];
                        line[y] += $"{y + GameManger.player.startInventoryIndex + 1,2}" + ")";
                        line[y] += $"{item.name,-6}";
                        if (item.type == 2)
                        {
                            line[y] += $"{(int)(item.price * priceRate[item.itemId].currRate * 0.7f * item.quality),-6:N0} ";
                        }
                        else
                        {
                            line[y] += $"{(int)(item.price * priceRate[item.itemId].currRate * 0.7f),-6:N0} ";
                        }
                        line[y] += $"{item.count,-3}";
                        line[y] += "\t\t\t";

                    }
                    else
                    {
                        line[y] += "\t\t\t\t\t";

                    }*/
                    if (y == 30)
                    {
                        line[30] = "\t";
                        line[30] += $"{gold,8}";
                        line[30] += "\t\t\t\t\t\t";
                        line[30] += $"{GameManger.player.gold,8}";
                        line[y] += "\t\t";
                    }
                    if (y == 31)
                    {
                        line[31] = "\t";
                        line[31] += "\t\t\t\t\t\t\t ";
                        if (cursorPosition.x <= 1)
                        {
                            line[31] += $"{GameManger.player.weight,4}" + "/";
                            line[31] += $"{Player._PLAYER_WEIGHT_MAX,-4}";
                            line[31] += $"{GameManger.player.SumWeight(),4}" + "/";
                            line[31] += $"{GameManger.player.maxWeightSum,-4}";
                        }
                        else
                        {
                            line[31] += $"{GameManger.player.wagonList[cursorPosition.x - 2].weight,4}" + "/";
                            line[31] += $"{Wagon._WAGON_WEIGHT_MAX,-4}";
                            line[31] += $"{GameManger.player.SumWeight(),4}" + "/";
                            line[31] += $"{GameManger.player.maxWeightSum,-4}";
                        }
                        line[y] += "\t\t";
                    }

                }
                else if (mainPosition == 2)
                {
                    if (y == cursorPosition.y)
                    {
                        line[y] += "▶";
                    }
                    else
                    {
                        line[y] += "　";
                    }
                    switch (y)
                    {
                        case 0:
                            line[y] += " 주변 소문\t\t\t\t\t\t\t\t\t\t\t\t";
                            break;
                        case 1:
                            line[y] += " 위치 확인\t\t\t\t\t\t\t\t\t\t\t\t";
                            break;
                        default:
                            line[y] += "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";
                            break;
                    }
                }
                else if (mainPosition == 3)
                {
                    if (y == cursorPosition.y)
                    {
                        line[y] += "▶";
                    }
                    else
                    {
                        line[y] += "　";
                    }
                    switch (y)
                    {
                        case 0:
                            line[y] += " 잠자기 .4.(100 Gold).\t\t\t\t\t\t\t\t\t\t\t\t";
                            break;
                        default:
                            line[y] += "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";
                            break;
                    }
                }
                if (mainPosition == 4)
                {
                    if (y == cursorPosition.y)
                    {
                        line[y] += "▶";
                    }
                    else
                    {
                        line[y] += "　";
                    }
                }
            }

            if (mainPosition == 4)
            {
                if (portals[0] != null)
                {
                    line[0] += " .2.동문.                                                                 ";
                }
                else
                {
                    line[0] += " .6.동문.                                                                 ";
                }
                if (portals[1] != null)
                {
                    line[1] += " .2.서문.                                                                   ";
                }
                else
                {
                    line[1] += " .6.서문.                                                                  ";
                }
                if (portals[2] != null)
                {
                    line[2] += " .2.북문.                                                                  ";
                }
                else
                {
                    line[2] += " .6.북문.                                                                  ";
                }
                if (portals[3] != null)
                {
                    line[3] += " .2.남문.                                                                  ";
                }
                else
                {
                    line[3] += " .6.남문.                                                                  ";
                }
            }

            if (mainPosition == 2)
            {
                for (int i = 0; i < pubEvents.Count; i++)
                {
                    line[i + 5] = "";
                    line[i + 5] += pubEvents[i].ShowEvent();
                }
            }

            return line;
        }

    }
}

