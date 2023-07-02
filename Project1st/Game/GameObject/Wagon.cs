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
        public static readonly int _WAGON_COUNT_MAX = 10;
        public static readonly int _WAGON_HITPOINT_MAX = 99;
        public static readonly float _WAGON_WEIGHT_MAX = 100;

        public float weight;
        public List<Items> inventory;
        public int startInvenIndex;


        public Wagon()
        {
            hitPoint = _WAGON_HITPOINT_MAX;
            weight = 0;
            startInvenIndex = 0;

            inventory = new List<Items>();
        }

        public void Follow(int x, int y)
        {
            this.Axis2D.x = x;
            this.Axis2D.y = y;
        }

    }
}
