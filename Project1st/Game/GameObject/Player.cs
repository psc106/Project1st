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
            hitPoint = 100;
            ID = 0;
            isCoolTime = false;
            gold = 10000;
            weapon = 1;
            viewCount = 5;
            hitPointMax = 100;
            inventory = new List<Items>();
            startInventoryIndex = 0;
        }

        public bool Move(Coordinate currAxis)
        {
            bool isStun = false;

            int beforeX = Axis2D.x;
            int beforeY = Axis2D.y;
            bool isWall = MoveAndHold(direction, FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);

            int currX = Axis2D.x;
            int currY = Axis2D.y;


            for (int y = 0; y < FieldBase._FIELD_SIZE; y++)
            {
                for (int x = 0; x < FieldBase._FIELD_SIZE; x++)
                {
                    if (GameManger.currField.GetFogInfo(x,y) == 1)
                    {
                        GameManger.currField.SetFogInfo(x, y, 0.5f);
                    }

                }
            }

            //빈칸으로 이동
            if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.empty ||
                GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.road)
            {
                if (!isWall)
                {
                }
            }

            //수풀로 이동
            if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.mud)
            {
                if (!isWall)
                {
                    if (GameManger.random.Next(1, 101) <= 10)
                    {
                        direction = 4;
                        isStun = true;
                    }
                }
            }
            //벽으로 이동
            else if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.tree ||
                     GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.wall)
            {
                Axis2D.x = beforeX;
                Axis2D.y = beforeY;
            }

            //텔레포트로 이동
            else if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.portal)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (GameManger.currField.portals[i] == null) { continue; }


                    if (currX == GameManger.currField.portals[i].axis.x &&
                        currY == GameManger.currField.portals[i].axis.y)
                    {
                        currAxis.x += AXIS_X[i];
                        currAxis.y += AXIS_Y[i];

                        Teleport(FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);

                        GameManger.currField.isCurrField = false;
                        GameManger.currField.StopEnemies();
                        if (GameManger.currField.GetCreateTimer() != null)
                        {
                            GameManger.currField.GetCreateTimer().Dispose();
                        }

                        //Utility.currRoom.enemyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                        GameManger.currField = GameManger.map.worldMap[currAxis.y, currAxis.x];
                        if (GameManger.currField.type == 1)
                        {

                            GameManger.currField.ReturnSelfToTown().Exit();
                            GameManger.currField.PlayEnemies();
                            GameManger.currField.SetCreateTimer(new Timer(GameManger.currField.CreateEnemy, null, 100, 10000));
                        }
                        GameManger.currField.isFog = false;
                        GameManger.currField.isCurrField = true;

                        if (GameManger.currField.type == 2)
                        {
                            GameManger.currField.ReturnSelfToTown().Enter();
                        }

                        break;
                    }
                }

            }

            if (GameManger.currField.type != 1)
            {
                return false;
            }

            for (int i = 0; i < GameManger.currField.GetEnemies().Count; i++)
            {
                GameManger.currField.GetEnemies()[i].isMove = true;
            }

            //적에게 이동
            Enemy enemy = GameManger.currField.FindEnemiesAt(currX, currY);
            if (enemy != null && enemy.isLive)
            {
                hitPoint -= 1;
                enemy.moveTimer.Dispose();
                if (hitPoint == 0)
                {
                    isLive = false;
                }
                GameManger.currField.GetEnemies().RemoveAll(x => x.Axis2D.x == enemy.Axis2D.x && x.Axis2D.y == enemy.Axis2D.y);
            }

            RemoveFog();

            return isStun;
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
