using Project1st.Game.Core;
using Project1st.Game.Item;
using Project1st.Game.Map.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game
{
 
    //좌표 저장
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

    //안개 정보 저장
    public class FogView
    {
        public int x;
        public int y;
        public int depth;

        public FogView() { }
        public FogView(int x, int y, int depth)
        {
            this.x = x;
            this.y = y;
            this.depth = depth;
        }
        public FogView(Coordinate axis, int depth)
        {
            this.x = axis.x;
            this.y = axis.y;
            this.depth = depth;
        }


        public static int GetCurrTimeLight(int tmpLight)
        {
            //int tmpLight = this.light;

            if (GameManger.worldMap.day == 0)
            {
                tmpLight += 2;
            }
            else if (GameManger.worldMap.day == 1)
            {
                tmpLight -= 2;
            }

            return tmpLight;
        }

        public static int GetRealView(int tmpLight)
        {
            //int tmpLight = GetCurrTimeLight();

            //1x4 (1) (4(1-1)+1)4
            //5x4 (2) (4(2-1)+4(1-1)+1)4
            //13x4(3) ((4(3-1))+(4(2-1))+4(1-1)+1)4
            int sum = 0;
            for (int i = 0; i < tmpLight; i++)
            {
                sum += i * 4;
            }
            return (sum + 1);
        }

    }

    //주점 정보 저장
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
                if (townX != -1 || townY != -1)
                {
                    str += $"[?, ?]에서 ";
                }
                else
                {
                    str += $"[{townX}, {townY}]에서 ";
                }
                str += $"{eventPrice.keepTurn}일 뒤에 {GameManger.db.database[itemId].name,4}이 가격을 {eventPrice.nextState}합니다";
            }
            else if (type == 1)
            {
                if (GameManger.worldMap.map[townY, townX].type == FieldBase.field_type.forest)
                {
                    str += $"[{townX}, {townY}] ";
                    str += "엔 숲길뿐이 없다";
                }
                else if (GameManger.worldMap.map[townY, townX].type == FieldBase.field_type.town)
                {
                    str += $"[{townX}, {townY}] ";
                    str += "에 마을이 있다";
                }
            }

            return str;
        }
    }

}
