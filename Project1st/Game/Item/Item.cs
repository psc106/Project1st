using Project1st.Game.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.Item
{
    public class Items
    {
        //아이템id
        public int itemId { private set; get; }
        //이름
        public string name { private set; get; }
        //가격
        public int price { private set; get; }
        //최대 내구도
        public float qualityMax { private set; get; }
        //무게
        public float weight { private set; get; }

        //현재 내구도
        public float quality;
        //개수
        public int count;
        //특산품
        public bool isOwn;

        public Items()
        {
        }

        public Items(int itemId, string name, int price, float qualityMax, float weight)
        {
            this.qualityMax = qualityMax;
            this.itemId = itemId;
            this.name = name;
            this.price = price;
            this.weight = weight;
            this.isOwn = true;

            this.count = GameManger.random.Next(3);
        }

        public Items(Items other)
        {
            this.qualityMax = 1;
            this.itemId = other.itemId;
            this.name = other.name;
            this.price = other.price;
            this.weight = other.weight;
            this.isOwn = false;

            this.count = 1;
            this.quality = 1;
        }

        public static List<Items> CreateStandard()
        {
            List<Items> return_ = new List<Items>();

            Items standard1 = new Items(1, "포션", 100, int.MaxValue, 0);
            Items standard2 = new Items(2, "장작", 30, int.MaxValue, 10);
            Items standard3 = new Items(3, "수리도구", 70, int.MaxValue, 5);

            return_.Add(standard1);
            return_.Add(standard2);
            return_.Add(standard3);

            return return_;
        }
    }
}
