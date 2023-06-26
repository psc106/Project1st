using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.GameObject
{
    public class MainObject
    {
        public Coordinate Axis2D;
    }

    public class MoveObject : MainObject
    {
        public int direction { get; set; }
    }
}
