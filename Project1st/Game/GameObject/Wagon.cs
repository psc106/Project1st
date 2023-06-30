using Project1st.Game.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.GameObject
{
    public class Wagon : MoveObject
    {
        public static readonly int wagonCountMax = 10;
        public static readonly int wagonHitPointMax = 99;
        public static readonly float wagonWeightMax = 100;

        public float weight;
        public List<Items> inventory;
        public int startWagonInvenIndex;


        public Wagon()
        {
            hitPoint = wagonHitPointMax;
            weight = 0;
            startWagonInvenIndex = 0;

            inventory = new List<Items>();
        }

        public void Follow(int x, int y)
        {
            this.Axis2D.x = x;
            this.Axis2D.y = y;
        }

    }
}
