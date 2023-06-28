using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.GameObject
{
    public class Effect : ObjectBase
    {
        public static string[] effectString = { "／", "€", "Δ", "⇔" };
        public int type;

        public Effect()
        {
            type = 0;
        }

        public Effect(int type)
        {
            this.type = type;
        }

        public Effect(int x, int y, int type)
        {
            Axis2D.x = x;
            Axis2D.y = y;
            this.type = type;
        }
    }
}
