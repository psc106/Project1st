﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.GameObject
{
    public class Wagon : MoveObject
    {

        public Wagon() { }

        public void Follow(int x, int y)
        {
            this.Axis2D.x = x;
            this.Axis2D.y = y;
        }

    }
}
