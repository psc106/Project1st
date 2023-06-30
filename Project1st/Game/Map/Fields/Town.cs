using Project1st.Game.Core;
using Project1st.Game.GameObject;
using Project1st.Game.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.Map.Fields
{


    public class Town : FieldBase
    {
        int[] axisX = { 1, -1, 0, 0 };
        int[] axisY = { 0, 0, -1, 1 };

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

        public void Enter()
        {
            mainPosition = 0;
            cursorPosition = new Coordinate(0, 0);
            GameManger.map.dayTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        public void Exit()
        {
            GameManger.map.dayTimer.Change(100, 60000);
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
                            line[y] += $"{Wagon.wagonCountMax - GameManger.player.wagonList.Count,-3}";
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

                        if (y + GameManger.player.startInventoryIndex < currInventory.Count && y < 30)
                        {

                            if (currInventory == null || currInventory.Count == 0) continue;
                            myItem = currInventory[(y + GameManger.player.startInventoryIndex)];
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
                            line[y] += $"{y + GameManger.player.startInventoryIndex + 1,2}" + ")";
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

                        if (y + currWagon.startWagonInvenIndex < currInventory.Count && y < 30)
                        {
                            if (currInventory == null || currInventory.Count == 0) continue;
                            myItem = currInventory[(y + currWagon.startWagonInvenIndex)];

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


                            line[y] += $"{y + currWagon.startWagonInvenIndex + 1,2}" + ")";
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
                        line[30] += $"{GameManger.currField.ReturnSelfToTown().gold,8}";
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
                            line[31] += $"{Player.playerWeightMax,-4}";
                            line[31] += $"{GameManger.player.SumWeight(),4}" + "/";
                            line[31] += $"{GameManger.player.maxWeightSum,-4}";
                        }
                        else
                        {
                            line[31] += $"{GameManger.player.wagonList[cursorPosition.x - 2].weight,4}" + "/";
                            line[31] += $"{Wagon.wagonWeightMax,-4}";
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
                    PubEvent pubEvent = GameManger.currField.ReturnSelfToTown().pubEvents[i];
                    line[i + 5] += pubEvent.ShowEvent();
                }
            }

            return line;
        }

        public override bool Move()
        {

            if (mainPosition == 0)
            {
                cursorPosition.y = cursorPosition.y + axisY[GameManger.player.direction];

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
                cursorPosition.x = cursorPosition.x + axisX[GameManger.player.direction];
                cursorPosition.y = cursorPosition.y + axisY[GameManger.player.direction];

                if (cursorPosition.x < 0)
                {
                    cursorPosition.x = 0;
                }
                else if (cursorPosition.x > GameManger.player.wagonList.Count+1)
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
                                GameManger.player.startInventoryIndex -= 1;
                                if (GameManger.player.startInventoryIndex < 0)
                                {
                                    GameManger.player.startInventoryIndex = 0;
                                }
                            }
                            else
                            {
                                currWagon.startWagonInvenIndex -= 1;
                                if (currWagon.startWagonInvenIndex < 0)
                                {
                                    currWagon.startWagonInvenIndex = 0;
                                }
                            }
                        }
                        else if (cursorPosition.y >= 30)
                        {
                            cursorPosition.y = 29;
                            if (cursorPosition.x == 1)
                            {
                                GameManger.player.startInventoryIndex -= 1;
                                if (GameManger.player.startInventoryIndex < 0)
                                {
                                    GameManger.player.startInventoryIndex = 0;
                                }
                                if (currInventory.Count - GameManger.player.startInventoryIndex < 30)
                                {
                                    GameManger.player.startInventoryIndex = currInventory.Count - 30;
                                }
                            }
                            else
                            {
                                currWagon.startWagonInvenIndex -= 1;
                                if (currWagon.startWagonInvenIndex < 0)
                                {
                                    currWagon.startWagonInvenIndex = 0;
                                }
                                if (currInventory.Count - currWagon.startWagonInvenIndex < 30)
                                {
                                    currWagon.startWagonInvenIndex = currInventory.Count - 30;
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
                        else if (cursorPosition.y > currInventory.Count-1 && currInventory.Count==0)
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
                cursorPosition.y = cursorPosition.y + axisY[GameManger.player.direction];
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
                cursorPosition.y = cursorPosition.y + axisY[GameManger.player.direction];
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
                cursorPosition.y = cursorPosition.y + axisY[GameManger.player.direction];

                if (cursorPosition.y < 0)
                {
                    cursorPosition.y = 4 - 1;
                }
                else if (cursorPosition.y > 4 - 1)
                {
                    cursorPosition.y = 0;
                }

            }
            return false;
            
        }
    }
}

