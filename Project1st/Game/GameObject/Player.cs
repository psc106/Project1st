using Project1st.Game.Core;
using Project1st.Game.Item;
using Project1st.Game.Map;
using Project1st.Game.Map.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.GameObject
{
    public class Player : MoveObject
    {
        public Timer attackTimer;

        public int gold;
        public bool isCoolTime;
        public int weapon;
        public int viewCount;

        public int hitPointMax = 100;

        public List<Items> inventory;
        public int startInventoryIndex;

        public Player()
        {
            Init();
        }
        public Player(int x, int y)
        {
            Init();
            Axis2D.x = x;
            Axis2D.y = y;

        }

        public override void Init()
        {
            base.Init();
            hitPointMax = 100;
            hitPoint = 100;
            attckPoint = 1;
            ID = 0;
            isCoolTime = false;
            gold = 10000;
            weapon = 1;
            viewCount = 5;
            inventory = new List<Items>();
            startInventoryIndex = 0;
        }

        public void RemoveFog()
        {
            Queue<Coordinate> q = new Queue<Coordinate>();

            q.Enqueue(GameManger.player.Axis2D);
            GameManger.currField.SetFogInfo(GameManger.player.Axis2D.x, GameManger.player.Axis2D.y, 1);

            int viewSetting = GameManger.player.viewCount;
            if (GameManger.map.isDay)
            {
                viewSetting += 3;
            }
            else
            {
                viewSetting -= 2;
            }
            viewSetting = (((viewSetting - 1) * 2) - 2) * (viewSetting - 1) + 1;
            for (int i = 0; i < viewSetting; i++)
            {
                Coordinate tmp = q.Dequeue();

                if ((tmp.x <= -1 || tmp.y <= -1 || tmp.x >= FieldBase._FIELD_SIZE || tmp.y >= FieldBase._FIELD_SIZE))
                { }
                else
                {
                    GameManger.currField.SetFogInfo(tmp.x, tmp.y, 1);
                }
                for (int j = 0; j < 4; j++)
                {
                    int tmpX = (tmp.x + AXIS_X[j]);
                    int tmpY = (tmp.y + AXIS_Y[j]);
                    q.Enqueue(new Coordinate(tmpX, tmpY));
                }
            }

            foreach (var tmp in q)
            {
                if ((tmp.x <= -1 || tmp.y <= -1 || tmp.x >= FieldBase._FIELD_SIZE || tmp.y >= FieldBase._FIELD_SIZE))
                {
                    continue;
                }

                if (GameManger.currField.GetFogInfo(tmp.x, tmp.y) != 1)
                {
                    GameManger.currField.SetFogInfo(tmp.x, tmp.y, 1);
                }
            }
        }
    }
}
