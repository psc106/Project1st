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

        public Dictionary<int, Items> shop;

        public int mainPosition;
        public Coordinate cursorPosition;

        public Town()
        {
            shop = new Dictionary<int, Items>();
            type = 2;
            mainPosition = 0;
            cursorPosition = new Coordinate(0, 0);
        }

        public Town(FieldBase field)
        {
            shop = new Dictionary<int, Items>();
            type = 2;
            mainPosition = 0;
            cursorPosition = new Coordinate(0, 0);

            portals = field.portals;
            for (int i = 0; i < 4; i++)
            {
                if (portals[i] != null)
                {
                    fieldInfo[portals[i].axis.y, portals[i].axis.x] = field_info.portal;
                }
            }

            foreach (var tmp in Items.CreateStandard())
            {
                shop.Add(tmp.itemId, tmp);
            }
        }


        public void EnterTown()
        {
            mainPosition = 0;
            cursorPosition = new Coordinate(0, 0);
        }


        public void ExitTown()
        {

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
                            line[y] += " 상점";
                            break;
                        case 1:
                            line[y] += " 주점";
                            break;
                        case 2:
                            line[y] += " 여관";
                            break;
                        case 3:
                            line[y] += " 나가기";
                            break;
                        default:
                            line[y] += "    ";
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
                    //아이템창
                    else if (cursorPosition.x == 1)
                    {
                        if (y < 13)
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
                        else if (y > 13)
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
                    //판매창
                    else if (cursorPosition.x == 2)
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
                            line[y] += " 주변 소문";
                            break;
                        case 1:
                            line[y] += " 위치 확인";
                            break;
                        default:
                            line[y] += "    ";
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
                            line[y] += " 잠자기";
                            break;
                        default:
                            line[y] += "    ";
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
                int count = 0;

                if (portals[0] != null)
                {
                    line[count] += " 동문";
                    count += 1;
                }
                if (portals[1] != null)
                {
                    line[count] += " 서문";
                    count += 1;
                }
                if (portals[2] != null)
                {
                    line[count] += " 북문";
                    count += 1;
                }
                if (portals[3] != null)
                {
                    line[count] += " 남문";
                    count += 1;
                }
            }

            return line;
        }

    }
}
