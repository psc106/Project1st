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
        //아이템type
        public int type { private set; get; }
        //공격형태
        public int weaponType { private set; get; }
        //공격데미지
        public int weaponStr { private set; get; }
        //공격속도
        public int weaponDelay { private set; get; }
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
            this.qualityMax = 10;
            this.itemId = 9;
            this.type = 1;
            this.name = "검　　　　";
            this.price = 0;
            this.weight = 0;
            this.isOwn = true;

            if (type == 1)
            {
                switch (itemId)
                {
                    case 4:
                        weaponType = 1;
                        weaponDelay = 1300;
                        weaponStr = 80;
                        break;
                    case 5:
                        weaponType = 2;
                        weaponDelay = 300;
                        weaponStr = 30;
                        break;
                    case 6:
                        weaponType = 1;
                        weaponDelay = 800;
                        weaponStr = 55;
                        break;
                    case 7:
                        weaponType = 0;
                        weaponDelay = 150;
                        weaponStr = 15;
                        break;
                    case 8:
                        weaponType = 1;
                        weaponDelay = 3000;
                        weaponStr = 200;
                        break;
                    case 9:
                        weaponType = 2;
                        weaponDelay = 450;
                        weaponStr = 45;
                        break;
                    default:
                        weaponType = 0;
                        weaponDelay = 0;
                        weaponStr = 0;
                        break;
                }
            }
        }

        public Items(int itemId, int type, string name, int price, float qualityMax, float weight)
        {
            this.qualityMax = qualityMax;
            this.itemId = itemId;
            this.type = type;
            this.name = name;
            this.price = price;
            this.weight = weight;
            this.isOwn = true;

            this.count = GameManger.random.Next(1, 10);
            this.quality = qualityMax;

            if (type == 1)
            {
                switch (itemId)
                {
                    case 4:
                        weaponType = 1;
                        weaponDelay = 1300;
                        weaponStr = 60;
                        break;
                    case 5:
                        weaponType = 2;
                        weaponDelay = 300;
                        weaponStr = 20;
                        break;
                    case 6:
                        weaponType = 1;
                        weaponDelay = 800;
                        weaponStr = 45;
                        break;
                    case 7:
                        weaponType = 0;
                        weaponDelay = 150;
                        weaponStr = 10;
                        break;
                    case 8:
                        weaponType = 1;
                        weaponDelay = 2500;
                        weaponStr = 140;
                        break;
                    case 9:
                        weaponType = 2;
                        weaponDelay = 450;
                        weaponStr = 35;
                        break;
                    default:
                        weaponType = 0;
                        weaponDelay = 0;
                        weaponStr = 0;
                        break;
                }
            }
        }

        public Items(Items other)
        {
            this.qualityMax = other.qualityMax;
            this.itemId = other.itemId;
            this.type = other.type;
            this.name = other.name;
            this.price = other.price;
            this.weight = other.weight;
            this.isOwn = false;

            this.count = 1;
            this.quality = other.qualityMax;


            if (type == 1)
            {
                switch (itemId)
                {
                    case 4:
                        weaponType = 1;
                        weaponDelay = 1300;
                        weaponStr = 60;
                        break;
                    case 5:
                        weaponType = 2;
                        weaponDelay = 300;
                        weaponStr = 20;
                        break;
                    case 6:
                        weaponType = 1;
                        weaponDelay = 800;
                        weaponStr = 45;
                        break;
                    case 7:
                        weaponType = 0;
                        weaponDelay = 150;
                        weaponStr = 10;
                        break;
                    case 8:
                        weaponType = 1;
                        weaponDelay = 2500;
                        weaponStr = 140;
                        break;
                    case 9:
                        weaponType = 2;
                        weaponDelay = 450;
                        weaponStr = 35;
                        break;
                    default:
                        weaponType = 0;
                        weaponDelay = 0;
                        weaponStr = 0;
                        break;
                }
            }
        }

    }
}
