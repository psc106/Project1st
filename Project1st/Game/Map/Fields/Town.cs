using Project1st.Game.Core;
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
        //2.00 ~ 0.00
        public Dictionary<int, PriceRate> priceRate;

        public int startShopIndex;

        public int gold;
        public int mainPosition;
        public Coordinate cursorPosition;

        public Town()
        {
            gold = 2000;
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
        }

        public Town(FieldBase field)
        {
            isFog = field.isFog;
            gold = 2000;
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
           // shop.Add(GameManger.db.database[3]);

            //무기
            int count = GameManger.random.Next(3);
            for (int i = 0; i < count; i++)
            {
                int id = GameManger.random.Next(4, 10);
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
        }

        public void Init()
        {
            gold = 8000;
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
                        line[y] += $"{y + startShopIndex + 1,2}" + ")";
                        line[y] += $"{item.name,-6}";
                        line[y] += $"{(int)(item.price * priceRate[item.itemId].currRate),-6:N0} ";
                        line[y] += $"{item.count,-3}";
                        line[y] += "\t\t\t";
                    }
                    else
                    {
                        line[y] += "\t\t\t\t\t";


                    }

                    //판매창
                    if (cursorPosition.x == 1)
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

                    if (y + GameManger.player.startInventoryIndex < GameManger.player.inventory.Count && y < 30)
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

                    }
                    if (y == 30)
                    {
                        line[30] = "\t";
                        line[30] += GameManger.currField.ReturnSelfToTown().gold;
                        line[30] += "\t\t\t\t\t\t";
                        line[30] += GameManger.player.gold;
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
                            line[y] += " 잠자기\t\t\t\t\t\t\t\t\t\t\t\t";
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
                else if (cursorPosition.x > 1)
                {
                    cursorPosition.x = 1;
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
                else if (cursorPosition.x == 1)
                {
                    if (GameManger.player.inventory.Count > 30)
                    {
                        if (cursorPosition.y < 0)
                        {
                            cursorPosition.y = 0;
                            GameManger.player.startInventoryIndex -= 1;
                            if (GameManger.player.startInventoryIndex < 0)
                            {
                                GameManger.player.startInventoryIndex = 0;
                            }
                        }
                        else if (cursorPosition.y >= 30)
                        {
                            cursorPosition.y = 29;
                            GameManger.player.startInventoryIndex += 1;
                            if (GameManger.player.inventory.Count - GameManger.player.startInventoryIndex < 30)
                            {
                                GameManger.player.startInventoryIndex = GameManger.player.inventory.Count - 30;
                            }
                        }

                    }
                    else
                    {
                        if (cursorPosition.y < 0)
                        {
                            cursorPosition.y = 0;
                        }
                        else if (cursorPosition.y >= GameManger.player.inventory.Count)
                        {
                            cursorPosition.y = GameManger.player.inventory.Count - 1;
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

