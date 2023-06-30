using Project1st.Game.Core;
using Project1st.Game.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game
{
    public class Coordinate
    {
        public int x;
        public int y;

        public Coordinate() { }
        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class PubEvent
    {
        public int type;
        public int itemId;
        public PriceRate eventPrice;
        public int townX;
        public int townY;

        public string ShowEvent()
        {
            string str = "";
            if (type == 0)
            {
                str += $"[{townX}, {townY}]에서 ";
                str += $"{eventPrice.keepTurn}일 뒤에 {GameManger.db.database[itemId].name,4}이 가격을 {eventPrice.nextState}합니다";
            }
            else if (type == 1)
            {
                if (GameManger.map.worldMap[townY, townX].type == 1)
                {
                    str += $"[{townX}, {townY}] ";
                    str += "엔 숲길뿐이 없다";
                }
                else if (GameManger.map.worldMap[townY, townX].type == 2)
                {
                    str += $"[{townX}, {townY}] ";
                    str += "에 마을이 있다";
                }
            }

            return str;
        }
    }

}
