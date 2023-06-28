﻿using System;
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
        
        //개수
        public int count;
        //시세
        public float priceRate;
        //특산품
        public bool isOwn;

        public Items()
        {
        }

        public Items(int itemId, string name, int price)
        {
            this.itemId = itemId;
            this.name = name;
            this.price = price;
            this.isOwn = true;

            this.count = 2;
        }

        public Items(Items other)
        {
            this.itemId = other.itemId;
            this.name = other.name;
            this.price = other.price;
            this.isOwn = false;

            this.count = 1;
            qualityMax = 1;
        }

        public static List<Items> CreateStandard()
        {
            List<Items> return_ = new List<Items>();

            Items standard1 = new Items(1, "포션", 100);
            Items standard2 = new Items(2, "장작", 30);
            Items standard3 = new Items(3, "수리도구", 70);

            return_.Add(standard1);
            return_.Add(standard2);
            return_.Add(standard3);

            return return_;
        }
    }
}