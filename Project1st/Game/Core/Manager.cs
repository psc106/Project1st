﻿using Project1st.Game.GameObject;
using Project1st.Game.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.Core
{
    public static class GameManger
    {
        public static BufferPrinter buffer;
        public static Player player;
        public static WorldMap map;
        public static Random random;

        public static void Instance()
        {
            buffer = new BufferPrinter();
            player = new Player();
            map = new WorldMap();

            random = new Random();
        }
    }
}
