using Project1st.Game.DataBase;
using Project1st.Game.GameObject;
using Project1st.Game.Map;
using Project1st.Game.Map.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.Core
{
    //게임 매니저
    //static으로 이뤄진 값들이며 각 클래스들은 이를 참조하여 동작한다.
    public static class GameManger
    {
        public static BufferPrinter buffer;
        public static Player player;
        public static WorldMap worldMap;
        public static FieldBase currField;

        public static DB db;
        public static Random random;
        public static Coordinate currFieldPos;

        public static void Instance()
        {
            random = new Random();

            db = new DB();
            db.PutData();

            worldMap = new WorldMap();
            buffer = new BufferPrinter();
            player = new Player();
            currFieldPos = new Coordinate(0, 0);

        }
    }
}
