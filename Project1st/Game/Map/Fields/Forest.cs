using Project1st.Game.Core;
using Project1st.Game.GameObject;
using Project1st.Game.Interface;
using Project1st.Game.Item;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Project1st.Game.Map.Fields
{

    public class Forest : FieldBase
    {
        readonly int _ENEMY_COUNT_MAX = 6;

        public List<Enemy> enemies;
        public Timer createTimer;

        public float[,] fogInfo;

        public Forest()
        {
            isMenu = false;
            type = 1;
            int wallCount = 60;
            int wallSleep = 2;
            fogInfo = new float[_FIELD_SIZE, _FIELD_SIZE];

            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fogInfo[y, x] = 0;
                    if (wallSleep == 0 && wallCount > 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1, 4);
                        if (fieldInfo[y, x] == field_info.tree)
                        {
                            wallCount -= 1;
                            wallSleep = 3;
                        }
                    }
                    else if (wallSleep > 0 || wallCount == 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1, 3);
                        wallSleep -= 1;
                    }
                }
            }
            enemies = new List<Enemy>();
            portals = new Portal[4];
        }


        public Forest(int flag)
        {
            isMenu = false;
            type = 1;
            int wallCount = 60;
            int wallSleep = 2;
            fogInfo = new float[_FIELD_SIZE, _FIELD_SIZE];

            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fogInfo[y, x] = 0;
                    if (wallSleep == 0 && wallCount > 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1, 4);
                        if (fieldInfo[y, x] == field_info.tree)
                        {
                            wallCount -= 1;
                            wallSleep = 3;
                        }
                    }
                    else if (wallSleep > 0 || wallCount == 0)
                    {
                        fieldInfo[y, x] = (field_info)GameManger.random.Next(1, 3);
                        wallSleep -= 1;
                    }
                }
            }

            enemies = new List<Enemy>();
            portals = new Portal[4];
            int bit = 1;

            for (int i = 0; i < portals.Length; i++)
            {
                if ((flag & bit) == bit)
                {
                    CreateDoor(i);
                }
                else
                {
                    portals[i] = null;
                }
                bit = bit << 1;
            }

            //portal[0].asd();
        }


        public override void CreateDoor(int index)
        {
            switch (index)
            {
                case 0:
                    portals[index] = new Portal(_FIELD_SIZE - 1, GameManger.random.Next(1, _FIELD_SIZE - 1));
                    break;
                case 1:
                    portals[index] = new Portal(0, GameManger.random.Next(1, _FIELD_SIZE - 1));
                    break;
                case 2:
                    portals[index] = new Portal(GameManger.random.Next(1, _FIELD_SIZE - 1), 0);
                    break;
                case 3:
                    portals[index] = new Portal(GameManger.random.Next(1, _FIELD_SIZE - 1), _FIELD_SIZE - 1);
                    break;
            }
            fieldInfo[portals[index].axis.y, portals[index].axis.x] = field_info.portal;
        }


        public override void InitEnter()
        {
            GameManger.currField.PlayEnemies();
            GameManger.currField.SetCreateTimer(new Timer(GameManger.currField.CreateEnemy, null, 100, 10000));

            GameManger.currField.isFog = false;
            GameManger.currField.isCurrField = true;

            GameManger.player.RemoveFog(4);
        }


        public override void Exit()
        {
            GameManger.currField.isCurrField = false;
            GameManger.currField.StopEnemies();
            if (GameManger.currField.GetCreateTimer() != null)
            {
                GameManger.currField.GetCreateTimer().Dispose();
            }
        }





        public override void CreateEnemy(object obj)
        {
            if (enemies.Count > _ENEMY_COUNT_MAX) return;
            while (true)
            {
                int enemyX = GameManger.random.Next(_FIELD_SIZE);
                int enemyY = GameManger.random.Next(_FIELD_SIZE);
                if (fieldInfo[enemyY, enemyX] != field_info.tree)
                {
                    Enemy enemy = new Enemy(enemyX, enemyY);
                    enemies.Add(enemy);
                    break;
                }
            }
        }
        public override void RemoveEnemy(int index)
        {
            if (enemies.Count <= 0) return;
            if (enemies[index].moveTimer != null)
            {
                enemies[index].moveTimer.Dispose();
            }
            enemies.RemoveAt(index);
        }
        public override void RemoveEnemy(int x, int y)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                if (enemies[i].Axis2D.x == x && enemies[i].Axis2D.y == y)
                {
                    if (enemies[i].moveTimer != null)
                    {
                        enemies[i].moveTimer.Dispose();
                    }
                    enemies.RemoveAt(i);
                }
            }
        }

        public override Enemy FindEnemiesAt(int x, int y)
        {
            if (enemies == null) return null;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;
                if (enemies[i].Axis2D.x == x && enemies[i].Axis2D.y == y) return enemies[i];
            }
            return null;
        }
        public override void StopEnemies()
        {
            if (enemies == null) return;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].moveTimer != null)
                {
                    enemies[i].moveTimer.Dispose();
                }
                //enemies[i].enemyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
            return;
        }
        public override void PlayEnemies()
        {
            if (enemies == null) return;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].moveTimer != null)
                {
                    enemies[i].StartForestTimer();
                }
                //enemies[i].enemyTimer.Change(500, 500);
            }
        }

        public override float[,] GetFogInfo()
        {
            return fogInfo;
        }
        public override float GetFogInfo(int x, int y)
        {
            return fogInfo[y, x];
        }
        public override void SetFogInfo(int x, int y, float info)
        {
            fogInfo[y, x] = info;
        }

        public override Timer GetCreateTimer()
        {
            return createTimer;
        }
        public override void SetCreateTimer(Timer timer)
        {
            createTimer = timer;
        }
        public override List<Enemy> GetEnemies()
        {
            return enemies;
        }

        public override bool PressMoveEvent()
        {

            bool isHold = false;
            bool isStun = false;

            int beforeX = GameManger.player.Axis2D.x;
            int beforeY = GameManger.player.Axis2D.y;

            int currX = GameManger.player.Hold(GameManger.player.GetNextX(GameManger.player.direction), FieldBase._FIELD_SIZE);
            int currY = GameManger.player.Hold(GameManger.player.GetNextY(GameManger.player.direction), FieldBase._FIELD_SIZE);

            //빈칸으로 이동
            if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.empty ||
                GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.road)
            {
                isHold = GameManger.player.MoveAndHold(GameManger.player.direction, FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);

            }

            //수풀로 이동
            if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.mud)
            {
                isHold = GameManger.player.MoveAndHold(GameManger.player.direction, FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);
                if (GameManger.random.Next(1, 101) <= 5)
                {
                    GameManger.player.direction = 4;
                    if (GameManger.player.inventory.Count > 0)
                    {

                        Items item = GameManger.player.inventory[GameManger.random.Next(GameManger.player.inventory.Count)];
                        if (item.itemId >= 10)
                        {
                            item.quality -= 0.01f * GameManger.random.Next(5);
                            if (item.quality < 0)
                            {
                                item.quality = 0;
                            }
                        }
                    }
                    isStun = true;
                }

            }
            //벽으로 이동
            else if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.tree ||
                     GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.wall)
            {
                isHold = true;
            }

            //텔레포트로 이동
            else if (GameManger.currField.fieldInfo[currY, currX] == FieldBase.field_info.portal)
            {
                isHold = true;
                GameManger.player.MoveAndHold(GameManger.player.direction, FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);

                for (int i = 0; i < 4; i++)
                {
                    if (GameManger.currField.portals[i] == null) { continue; }


                    if (currX == GameManger.currField.portals[i].axis.x &&
                        currY == GameManger.currField.portals[i].axis.y)
                    {
                        GameManger.currFieldPos.x += WorldMap._AXIS_MATRIX_X[i];
                        GameManger.currFieldPos.y += WorldMap._AXIS_MATRIX_Y[i];

                        GameManger.player.Teleport(FieldBase._FIELD_SIZE, FieldBase._FIELD_SIZE);
                        for (int j = 0; j < GameManger.player.wagonList.Count; j++)
                        {
                            GameManger.player.wagonList[j].Axis2D.x = GameManger.player.Axis2D.x;
                            GameManger.player.wagonList[j].Axis2D.y = GameManger.player.Axis2D.y;
                        }

                        Exit();

                        //Utility.currRoom.enemyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                        GameManger.currField = GameManger.worldMap.map[GameManger.currFieldPos.y, GameManger.currFieldPos.x];
                        GameManger.currField.InitEnter();

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

                GameManger.currField.Exit();
                GameManger.currField.GetEnemies().RemoveAll(x => x.Axis2D.x == enemy.Axis2D.x && x.Axis2D.y == enemy.Axis2D.y);

                new Battle();
            }

            Wagon wagon = GameManger.player.wagonList.Find(x=>x.Axis2D.x==currX && x.Axis2D.y == currY);
            if (wagon != null)
            {
                GameManger.player.wagonList.Remove(wagon);
                GameManger.player.maxWeightSum -= 100;
            }


            if (!isHold)
            {
                for (int i = 0; i < GameManger.player.wagonList.Count; i++)
                {
                    currY = GameManger.player.wagonList[i].Axis2D.y;
                    currX = GameManger.player.wagonList[i].Axis2D.x;

                    GameManger.player.wagonList[i].Follow(beforeX, beforeY);

                    beforeY = currY;
                    beforeX = currX;
                }
            }

            return isStun;
        }

        public override void PressMenuEvent()
        {
            if (GameManger.worldMap.isInventory)
            {
                //GameManger.map.cursor.x = GameManger.map.cursor.x + axisX[GameManger.player.direction];
                GameManger.worldMap.cursor.y = GameManger.worldMap.cursor.y + WorldMap._AXIS_MATRIX_Y[GameManger.player.direction];
                int nextX = GameManger.worldMap.cursor.x + WorldMap._AXIS_MATRIX_X[GameManger.player.direction];

                if (nextX < 0)
                {
                    GameManger.worldMap.cursor.x = 0;
                }
                else if (nextX >= GameManger.player.wagonList.Count)
                {
                    GameManger.worldMap.cursor.x = GameManger.player.wagonList.Count;
                }
                else
                {
                    GameManger.worldMap.cursor.x = nextX;
                }

                if (GameManger.worldMap.cursor.x != nextX)
                {
                    GameManger.worldMap.cursor.y = 0;
                }

                if (GameManger.worldMap.cursor.x == 0)
                {
                    //구매창
                    if (GameManger.player.inventory.Count > 10)
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                            GameManger.player.startInvenIndex -= 1;
                            if (GameManger.player.startInvenIndex < 0)
                            {
                                GameManger.player.startInvenIndex = 0;
                            }
                        }
                        else if (GameManger.worldMap.cursor.y >= 10)
                        {
                            GameManger.worldMap.cursor.y = 9;
                            GameManger.player.startInvenIndex += 1;
                            if (GameManger.player.inventory.Count - GameManger.player.startInvenIndex < 10)
                            {
                                GameManger.player.startInvenIndex = GameManger.player.inventory.Count - 10;
                            }
                        }

                    }
                    else
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                        }

                        else if (GameManger.worldMap.cursor.y >= GameManger.player.inventory.Count)
                        {
                            GameManger.worldMap.cursor.y = GameManger.player.inventory.Count - 1;
                        }
                    }

                }
                else if (GameManger.worldMap.cursor.x >= 1)
                {
                    if (GameManger.player.wagonList == null || GameManger.player.wagonList.Count == 0) return;
                    Wagon currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];


                    List<Items> currInventory = currWagon.inventory;

                    //구매창
                    if (currInventory.Count > 10)
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                            currWagon.startInvenIndex -= 1;
                            if (currWagon.startInvenIndex < 0)
                            {
                                currWagon.startInvenIndex = 0;
                            }
                        }
                        else if (GameManger.worldMap.cursor.y >= 10)
                        {
                            GameManger.worldMap.cursor.y = 9;
                            currWagon.startInvenIndex += 1;
                            if (currInventory.Count - currWagon.startInvenIndex < 10)
                            {
                                currWagon.startInvenIndex = currInventory.Count - 10;
                            }
                        }

                    }
                    else
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                        }
                        else if (GameManger.worldMap.cursor.y >= currInventory.Count)
                        {
                            GameManger.worldMap.cursor.y = currInventory.Count - 1;
                        }
                    }
                }

            }
            else if (GameManger.worldMap.isEquip)
            {
                GameManger.worldMap.cursor.y = GameManger.worldMap.cursor.y + WorldMap._AXIS_MATRIX_Y[GameManger.player.direction];
                int nextX =
                    GameManger.worldMap.cursor.x + WorldMap._AXIS_MATRIX_X[GameManger.player.direction];

                if (nextX < 0)
                {
                    GameManger.worldMap.cursor.x = 0;
                }
                else if (nextX >= GameManger.player.wagonList.Count)
                {
                    GameManger.worldMap.cursor.x = GameManger.player.wagonList.Count;
                }
                else
                {
                    GameManger.worldMap.cursor.x = nextX;
                }

                if (GameManger.worldMap.cursor.x != nextX)
                {
                    GameManger.worldMap.cursor.y = 0;
                }

                List<Items> currInventory;
                List<Items> equipList;
                if (GameManger.worldMap.cursor.x == 0)
                {
                    currInventory = GameManger.player.inventory;
                    equipList = currInventory.FindAll(tmp => tmp.type == 1);

                    //구매창
                    if (equipList.Count > 10)
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                            GameManger.player.startInvenIndex -= 1;
                            if (GameManger.player.startInvenIndex < 0)
                            {
                                GameManger.player.startInvenIndex = 0;
                            }
                        }
                        else if (GameManger.worldMap.cursor.y >= 10)
                        {
                            GameManger.worldMap.cursor.y = 9;
                            GameManger.player.startInvenIndex += 1;
                            if (equipList.Count - GameManger.player.startInvenIndex < 10)
                            {
                                GameManger.player.startInvenIndex = equipList.Count - 10;
                            }
                        }

                    }
                    else
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                        }

                        else if (GameManger.worldMap.cursor.y >= GameManger.player.inventory.Count)
                        {
                            GameManger.worldMap.cursor.y = GameManger.player.inventory.Count - 1;
                        }
                    }

                }
                else if (GameManger.worldMap.cursor.x >= 1)
                {

                    Wagon currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];
                    currInventory = currWagon.inventory;
                    equipList = currInventory.FindAll(tmp => tmp.type == 1);

                    if (GameManger.player.wagonList == null || GameManger.player.wagonList.Count == 0) return;

                    //구매창
                    if (equipList.Count > 10)
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                            currWagon.startInvenIndex -= 1;
                            if (currWagon.startInvenIndex < 0)
                            {
                                currWagon.startInvenIndex = 0;
                            }
                        }
                        else if (GameManger.worldMap.cursor.y >= 10)
                        {
                            GameManger.worldMap.cursor.y = 9;
                            currWagon.startInvenIndex += 1;
                            if (equipList.Count - currWagon.startInvenIndex < 10)
                            {
                                currWagon.startInvenIndex = equipList.Count - 10;
                            }
                        }

                    }
                    else
                    {
                        if (GameManger.worldMap.cursor.y < 0)
                        {
                            GameManger.worldMap.cursor.y = 0;
                        }
                        else if (GameManger.worldMap.cursor.y >= equipList.Count)
                        {
                            GameManger.worldMap.cursor.y = equipList.Count - 1;
                        }
                    }
                }
            }
            else
            {
                GameManger.worldMap.cursor.y = GameManger.worldMap.cursor.y + WorldMap._AXIS_MATRIX_Y[GameManger.player.direction];
                if (GameManger.worldMap.cursor.y < 0)
                {
                    GameManger.worldMap.cursor.y = 2;
                }
                else if (GameManger.worldMap.cursor.y > 2)
                {
                    GameManger.worldMap.cursor.y = 0;
                }
            }
        }
        public override void PressNoEvent()
        {
            if (!GameManger.currField.isMenu)
            {
                GameManger.worldMap.isInventory = false;
                GameManger.worldMap.isEquip = false;

                GameManger.currField.isMenu = true;
                GameManger.currField.StopEnemies();
                GameManger.currField.ReturnSelfToForest().createTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                GameManger.player.startInvenIndex = 0;
                GameManger.worldMap.cursor = new Coordinate(0, 0);
            }

            else
            {
                if (GameManger.worldMap.isInventory)
                {
                    GameManger.worldMap.isEquip = false;
                    GameManger.worldMap.isInventory = false;
                    GameManger.player.startInvenIndex = 0;
                    GameManger.worldMap.cursor = new Coordinate(0, 0);
                    return;
                }
                if (GameManger.worldMap.isEquip)
                {
                    GameManger.worldMap.isEquip = false;
                    GameManger.worldMap.isInventory = false;
                    GameManger.player.startInvenIndex = 0;
                    GameManger.worldMap.cursor = new Coordinate(0, 0);
                    return;
                }

                GameManger.currField.isMenu = false;
                GameManger.currField.PlayEnemies();
                GameManger.currField.ReturnSelfToForest().createTimer.Change(100, 10000);
            }

        }

        public override bool PressYesEvent()
        {

            if (!GameManger.currField.isMenu)
            {
                if (!GameManger.player.isMeleeDelay)
                {
                    GameManger.player.isMeleeDelay = true;
                    GameManger.player.meleeDelay = new Timer(GameManger.player.DelayMeleeTimer, null, 3000, 0);

                    int nextX = GameManger.player.Hold(GameManger.player.GetNextX(GameManger.player.direction), FieldBase._FIELD_SIZE);
                    int nextY = GameManger.player.Hold(GameManger.player.GetNextY(GameManger.player.direction), FieldBase._FIELD_SIZE);

                    GameManger.player.Effects.Add(new Effect(nextX, nextY, 0));


                    //적 공격
                    Enemy currEnemy = GameManger.currField.FindEnemiesAt(nextX, nextY);
                    if (currEnemy != null)
                    {
                        if (currEnemy.isLive)
                        {
                            currEnemy.hitPoint -= currEnemy.hitPoint;
                            if (currEnemy.hitPoint <= 0)
                            {
                                GameManger.currField.RemoveEnemy(nextX, nextY);
                                return true;
                            }
                        }
                        return false;
                    }


                    //벽 공격
                    if (GameManger.currField.fieldInfo[nextY, nextX] == FieldBase.field_info.tree)
                    {
                        //공구 있을때만 파괴 가능
                        Items breakTool = GameManger.player.inventory.Find(x => x.itemId == 1);

                        if (GameManger.player.inventory.Find(x => x.itemId == 1) != null)
                        {
                            breakTool.count -= 1;
                            if (breakTool.count <= 0)
                            {
                                GameManger.player.inventory.Remove(breakTool);
                            }
                            GameManger.currField.fieldInfo[nextY, nextX] = 0;
                            return false;
                        }
                    }

                    //함정 공격
                    if (GameManger.currField.fieldInfo[nextY, nextX] == FieldBase.field_info.mud)
                    {
                        //공구 있을때만 파괴 가능
                        Items breakTool = GameManger.player.inventory.Find(x => x.itemId == 1);

                        if (GameManger.player.inventory.Find(x => x.itemId == 1) != null)
                        {
                            breakTool.count -= 1;
                            if (breakTool.count <= 0)
                            {
                                GameManger.player.inventory.Remove(breakTool);
                            }
                            GameManger.currField.fieldInfo[nextY, nextX] = 0;
                            return false;
                        }
                    }

                }
            }

            if (GameManger.currField.isMenu)
            {
                if (!GameManger.worldMap.isInventory && !GameManger.worldMap.isEquip && GameManger.worldMap.cursor.y == 1)
                {
                    GameManger.worldMap.isMinimap = !GameManger.worldMap.isMinimap;

                    GameManger.currField.isMenu = false;
                    return false;
                }

                if (!GameManger.worldMap.isInventory && !GameManger.worldMap.isEquip && GameManger.worldMap.cursor.y == 0)
                {
                    GameManger.worldMap.isInventory = true;
                    GameManger.player.startInvenIndex = 0;
                    for (int i = 0; i < GameManger.player.wagonList.Count; i++)
                    {
                        GameManger.player.wagonList[i].startInvenIndex = 0;
                    }
                    GameManger.worldMap.cursor = new Coordinate(0, 0);
                    return false;
                }

                if (!GameManger.worldMap.isInventory && !GameManger.worldMap.isEquip && GameManger.worldMap.cursor.y == 2)
                {
                    GameManger.worldMap.isEquip = true;
                    GameManger.player.startInvenIndex = 0;
                    for (int i = 0; i < GameManger.player.wagonList.Count; i++)
                    {
                        GameManger.player.wagonList[i].startInvenIndex = 0;
                    }
                    GameManger.worldMap.cursor = new Coordinate(0, 0);
                    return false;
                }

                if (GameManger.worldMap.isInventory)
                {
                    List<Items> currInventory;
                    Items item;
                    Wagon currWagon = null;

                    if (GameManger.worldMap.cursor.x == 0)
                    {
                        currInventory = GameManger.player.inventory;
                        if (GameManger.worldMap.cursor.y + GameManger.player.startInvenIndex >= currInventory.Count)
                        {
                            return false; 
                        }
                        if (GameManger.worldMap.cursor.y + GameManger.player.startInvenIndex < 0)
                        {
                            return false; 
                        }
                        item = currInventory[GameManger.worldMap.cursor.y + GameManger.player.startInvenIndex];
                    }
                    else
                    {
                        currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];
                        currInventory = currWagon.inventory;
                        item = currInventory[GameManger.worldMap.cursor.y + currWagon.startInvenIndex];
                    }

                    GameManger.player.use = GameManger.currField;

                    if (GameManger.player.use.UseItem(item, currWagon))
                    {

                    }
                    return false;
                }


                if (GameManger.worldMap.isEquip)
                {
                    List<Items> equipList;
                    List<Items> currInventory;
                    Items item;
                    Wagon currWagon = null;

                    if (GameManger.worldMap.cursor.x == 0)
                    {
                        currInventory = GameManger.player.inventory;
                        equipList = currInventory.FindAll(tmp => tmp.type == 1);
                        if (equipList == null || equipList.Count == 0)
                        {
                            return false;
                        }
                        item = equipList[GameManger.worldMap.cursor.y + GameManger.player.startInvenIndex];
                    }
                    else
                    {
                        currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];
                        currInventory = currWagon.inventory;
                        equipList = currInventory.FindAll(tmp => tmp.type == 1);
                        if (equipList == null || equipList.Count == 0)
                        {
                            return false;
                        }
                        item = equipList[GameManger.worldMap.cursor.y + currWagon.startInvenIndex];
                    }

                    GameManger.player.equip = GameManger.currField;

                    if (GameManger.player.equip.EquipItem(item, currWagon))
                    {

                    }
                    return false;
                }
            }
            return false;
        }

        public override string[] ConvertMapToString(ref string[] line)
        {
            for (int y = 0; y < GameManger.buffer._BUFFER_SIZE; y++)
            {
                line[y] = "";
            }

            for (int y = 0; y < FieldBase._FIELD_SIZE; y++)
            {
                line[y] = "";
                for (int x = 0; x < FieldBase._FIELD_SIZE; x++)
                {
                    if (GameManger.player.Effects != null && GameManger.player.Effects.Count > 0)
                    {
                        List<Effect> currEffect;

                        currEffect = GameManger.player.Effects.FindAll(effect => (effect.Axis2D.x == x && effect.Axis2D.y == y));
                        Effect firstEffect = currEffect.FirstOrDefault();
                        if (firstEffect != null)
                        {
                            line[y] += ".11." + Effect._EFFECT_STRING[firstEffect.type] + ".";
                            if (firstEffect.type != 4)
                            {
                                GameManger.player.Effects.Remove(firstEffect);
                            }
                            continue;
                        }
                        else
                        {
                        }
                    }

                    if (fogInfo[y, x] == 1)
                    {
                        //적 2순위
                        Enemy tmp = FindEnemiesAt(x, y);
                        if (tmp != null)
                        {
                            if (tmp.isLive)
                            {
                                if (tmp.hitPoint < Enemy._ENEMY_HITPOINT_MAX / 20)
                                {
                                    line[y] += ".7.";
                                }
                                else if (tmp.hitPoint <= Enemy._ENEMY_HITPOINT_MAX)
                                {
                                    line[y] += ".1.";
                                }
                                else
                                {
                                    line[y] += ".0.";
                                }
                                line[y] += "적.";
                            }
                            else if (!tmp.isLive)
                            {
                                line[y] += ".1.？.";
                            }
                            continue;
                        }
                    }

                    //플레이어 3순위
                    if (GameManger.player.Axis2D.x == x && GameManger.player.Axis2D.y == y)
                    {
                        if (GameManger.player.isLive)
                        {
                            if (GameManger.player.hitPoint < 20)
                            {
                                line[y] += ".6.";

                            }
                            else if (GameManger.player.hitPoint < 50)
                            {
                                line[y] += ".5.";

                            }
                            else if (GameManger.player.hitPoint < 200)
                            {
                                line[y] += ".4.";

                            }
                            else if (GameManger.player.hitPoint < int.MaxValue)
                            {
                                line[y] += ".0.";

                            }

                            switch (GameManger.player.direction)
                            {
                                case 0:
                                    line[y] += "▶.";
                                    break;
                                case 1:
                                    line[y] += "◀.";
                                    break;
                                case 2:
                                    line[y] += "▲.";
                                    break;
                                case 3:
                                    line[y] += "▼.";
                                    break;
                                case 4:
                                    line[y] += "！.";
                                    break;
                                default:
                                    line[y] += "나.";
                                    break;
                            }

                        }
                        else
                        {
                            line[y] += ".3.Π.";
                        }
                        continue;
                    }

                    //마차 4순위
                    Wagon wagon = GameManger.player.wagonList.Find(tmp => tmp.Axis2D.x == x && tmp.Axis2D.y == y);
                    if (wagon != null)
                    {
                        if (wagon.inventory.Count > 0)
                        {
                            if (wagon.hitPoint <= Wagon._WAGON_HITPOINT_MAX / 2)
                            {
                                line[y] += ".11.ㅇ.";
                            }
                            else
                            {
                                line[y] += ".2.ㅇ.";
                            }
                        }
                        else
                        {
                            if (wagon.hitPoint <= Wagon._WAGON_HITPOINT_MAX / 2)
                            {
                                line[y] += ".9.ㅇ.";
                            }
                            else
                            {
                                line[y] += ".3.ㅇ.";
                            }
                        }
                        continue;
                    }


                    if (fogInfo[y, x] == 0)
                    {
                        line[y] += ".8.　.";
                    }
                    else
                    {
                        if (fogInfo[y, x] == 1)
                        {
                            if (fieldInfo[y, x] == FieldBase.field_info.portal)
                            {
                                line[y] += ".2.";
                            }
                            else
                            {
                                line[y] += ".0.";

                            }
                        }
                        else if (fogInfo[y, x] > 0 && fogInfo[y, x] < 1)
                        {
                            if (fieldInfo[y, x] == FieldBase.field_info.portal)
                            {
                                line[y] += ".12.";
                            }
                            else
                            {
                                line[y] += ".6.";
                            }

                        }

                        //맵정보 마지막
                        switch (fieldInfo[y, x])
                        {
                            case FieldBase.field_info.empty:
                                line[y] += "ㅁ.";
                                break;
                            case FieldBase.field_info.mud:
                                line[y] += "＊.";
                                break;
                            case FieldBase.field_info.tree:
                                line[y] += "〓.";
                                break;
                            case FieldBase.field_info.portal:
                                bool isTownConnect = false;
                                if (x == _FIELD_SIZE - 1) isTownConnect = portals[0].isTown;
                                else if (x == 0) isTownConnect = portals[1].isTown;
                                else if (y == 0) isTownConnect = portals[2].isTown;
                                else if (y == _FIELD_SIZE - 1) isTownConnect = portals[3].isTown;

                                if (isTownConnect)
                                {
                                    line[y] += "문.";
                                }
                                else
                                {
                                    line[y] += "길.";
                                }
                                break;
                            case FieldBase.field_info.road:
                                line[y] += "□.";
                                break;
                            case FieldBase.field_info.wall:
                                line[y] += "■.";
                                break;
                        }
                    }
                }
            }

            line[FieldBase._FIELD_SIZE + 1] += $"체력{GameManger.player.hitPoint,3}/{Player._PLAYER_HITPOINT_MAX,3}";
            line[FieldBase._FIELD_SIZE + 1] += $"\t시야{FogView.GetCurrTimeLight(GameManger.player.light),2}";
            line[FieldBase._FIELD_SIZE + 1] += $"\tDay {GameManger.worldMap.daySum,-4}";
            if (GameManger.worldMap.day == 0)
            {
                line[FieldBase._FIELD_SIZE + 1] += "(낮)";
            }
            else
            {
                line[FieldBase._FIELD_SIZE + 1] += "(밤)";
            }
            line[FieldBase._FIELD_SIZE + 2] += $"장비) {GameManger.player.weapon.name,-4}";
            line[FieldBase._FIELD_SIZE + 3] += $"골드) {GameManger.player.gold,10} gold";
            line[FieldBase._FIELD_SIZE + 4] += $"현재 좌표 ({GameManger.currFieldPos.x,2}, {GameManger.currFieldPos.y,2})";

            for (int y = 0; y < GameManger.buffer._BUFFER_SIZE; y++)
            {
                if (line[y].Length < FieldBase._FIELD_SIZE)
                {
                    for (int i = line[y].Length; i < FieldBase._FIELD_SIZE; i++)
                    {
                        line[y] += "　";
                    }
                }
            }

            if (!GameManger.currField.isMenu)
            {
                if (GameManger.worldMap.isMinimap)
                {
                    for (int y = 0; y < WorldMap._MAP_SIZE; y++)
                    {
                        for (int x = 0; x < WorldMap._MAP_SIZE; x++)
                        {
                            string[] tmp = GameManger.worldMap.map[y, x].MakeRoomToString();
                            line[y * 3] += tmp[0];
                            line[y * 3 + 1] += tmp[1];
                            line[y * 3 + 2] += tmp[2];
                        }
                    }
                }
            }
            else
            {
                if (GameManger.worldMap.isInventory)
                {
                    for (int y = 0; y < 15; y++)
                    {
                        //구매창
                        if (y == GameManger.worldMap.cursor.y)
                        {
                            line[y] += "▶";
                        }
                        else
                        {
                            line[y] += "　";
                        }

                        if (GameManger.worldMap.cursor.x == 0)
                        {
                            if (y + GameManger.player.startInvenIndex < GameManger.player.inventory.Count && y < 10)
                            {
                                Items item = GameManger.player.inventory[y + GameManger.player.startInvenIndex];

                                if (item.type == 0)
                                {
                                    line[y] += ".4.";
                                }
                                else
                                {
                                    line[y] += ".6.";
                                }

                                line[y] += $"{y + GameManger.player.startInvenIndex + 1,2}" + ")";
                                line[y] += $"{item.name,-6}";
                                line[y] += $"{item.price,-6:N0} ";
                                line[y] += $"{item.count,-3}";
                                if (item.type == 2)
                                {
                                    line[y] += $"{(int)(item.quality * 100),3}" + "%";
                                }
                                else
                                {
                                    line[y] += "∞";
                                }
                                line[y] += ".\t\t\t\t\t";
                            }
                        }
                        else if (GameManger.worldMap.cursor.x >= 1)
                        {
                            if (GameManger.player.wagonList == null || GameManger.player.wagonList.Count == 0) continue;
                            Wagon currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];

                            List<Items> currInventory = currWagon.inventory;

                            if (y + currWagon.startInvenIndex < currInventory.Count && y < 10)
                            {
                                Items item = currInventory[y + currWagon.startInvenIndex];
                                if (item.type == 0)
                                {
                                    line[y] += ".4.";
                                }
                                else if (item.type == 1)
                                {
                                    line[y] += ".6.";
                                }
                                else
                                {
                                    line[y] += ".8.";
                                }

                                line[y] += $"{y + currWagon.startInvenIndex + 1,2}" + ")";
                                line[y] += $"{item.name,-6}";
                                line[y] += $"{item.price,-6:N0} ";
                                line[y] += $"{item.count,-3}";
                                if (item.type == 2)
                                {
                                    line[y] += $"{(int)(item.quality * 100),3}" + "%";
                                }
                                else
                                {
                                    line[y] += "∞";
                                }
                                line[y] += ".\t\t\t\t\t";
                            }
                        }
                    }

                }
                else if (GameManger.worldMap.isEquip)
                {
                    for (int y = 0; y < 15; y++)
                    {
                        //구매창
                        if (y == GameManger.worldMap.cursor.y)
                        {
                            line[y] += "▶";
                        }
                        else
                        {
                            line[y] += "　";
                        }

                        if (GameManger.worldMap.cursor.x == 0)
                        {
                            List<Items> equipList = GameManger.player.inventory.FindAll(tmp => tmp.type == 1);

                            if (y + GameManger.player.startInvenIndex < equipList.Count && y < 10)
                            {
                                Items item = equipList[y + GameManger.player.startInvenIndex];
                                if (item.type == 0)
                                {
                                    line[y] += ".4.";
                                }
                                else if (item.type == 1)
                                {
                                    line[y] += ".8.";
                                }
                                else
                                {
                                    line[y] += ".10.";
                                }
                                line[y] += $"{y + GameManger.player.startInvenIndex + 1,2}" + ")";
                                line[y] += $"{item.name,-6} ";
                                line[y] += $"{item.weaponStr,4} ";
                                line[y] += $"{item.weaponDelay,5} ms";
                                line[y] += ".\t\t\t\t\t";
                            }
                        }
                        else if (GameManger.worldMap.cursor.x >= 1)
                        {
                            if (GameManger.player.wagonList == null || GameManger.player.wagonList.Count == 0) continue;
                            Wagon currWagon = GameManger.player.wagonList[GameManger.worldMap.cursor.x - 1];

                            List<Items> equipList = currWagon.inventory.FindAll(tmp => tmp.type == 1);

                            if (y + currWagon.startInvenIndex < equipList.Count && y < 10)
                            {
                                Items item = equipList[y + currWagon.startInvenIndex];

                                line[y] += $"{y + currWagon.startInvenIndex + 1,2}" + ")";
                                line[y] += $"{item.name,-6}";
                                line[y] += "\t\t\t\t\t";
                            }
                        }
                    }

                }
                else
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (y == GameManger.worldMap.cursor.y)
                        {
                            line[y] += "▶";
                        }
                        else
                        {
                            line[y] += "　";
                        }
                    }

                    line[0] += "인벤토리\t\t\t\t";
                    if (GameManger.worldMap.isMinimap)
                    {
                        line[1] += "미니맵 off\t\t\t";
                    }
                    else
                    {
                        line[1] += "미니맵 on\t\t\t";
                    }
                    line[2] += "장비\t\t\t\t\t\t";
                }

            }

            for (int y = 0; y < GameManger.buffer._BUFFER_SIZE; y++)
            {
                line[y] += "\t\t\t\t\t\t\t\t\t";
            }
            return line;
        }

    }
}
