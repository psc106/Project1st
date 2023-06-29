using Project1st.Game.Core;
using Project1st.Game.Map.Fields;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.GameObject
{
    public class Effect : MoveObject
    {
        public static string[] effectString = { "／", "€", "Δ", "⇔", "˚" };
        public int type;
        public bool isRemove;

        public Timer bulletTimer;

        public Effect()
        {
            type = 0;
        }

        public Effect(int type)
        {
            this.type = type;
        }

        //근접 공격
        public Effect(int x, int y, int type)
        {
            Axis2D.x = x;
            Axis2D.y = y;
            this.type = type;
        }

        //원거리 공격
        public Effect(int x, int y, int type, int direction)
        {
            Axis2D.x = x;
            Axis2D.y = y;
            this.type = 4;
            this.direction = direction;
            bulletTimer = new Timer(SetBulletTimer, null, 1, 150);
            isRemove = false;
        }

        public void SetBulletTimer(object obj)
        {
            int nextX = this.GetNextX(direction);
            int nextY = this.GetNextY(direction);

            if (nextX == -1 || nextY == -1 || nextX == FieldBase._FIELD_SIZE || nextY == FieldBase._FIELD_SIZE||
                GameManger.currField.GetElementAt(nextX, nextY) == FieldBase.field_info.wall)
            {
                bulletTimer.Dispose();
                GameManger.player.bulletCount -= 1;
                isRemove = true;
                return;
            }

            if (GameManger.currField.GetElementAt(nextX, nextY) != FieldBase.field_info.wall &&
                            GameManger.currField.FindEnemiesAt(nextX, nextY) == null)
            {
                this.Axis2D.x = nextX;
                this.Axis2D.y = nextY;
                return;
            }

            Enemy currEnemy = GameManger.currField.FindEnemiesAt(nextX, nextY);
            if (currEnemy != null)
            {
                if (currEnemy.isLive)
                {
                    currEnemy.hitPoint -= GameManger.player.attckPoint;
                    bulletTimer.Dispose();
                    GameManger.player.bulletCount -= 1;
                    isRemove = true;

                    if (currEnemy.hitPoint <= 0)
                    {
                        GameManger.currField.RemoveEnemy(nextX, nextY);

                        if (GameManger.currField.GetEnemies().Count == 0)
                        {
                            GameManger.currField.ReturnSelfToBattle().isWin = true;

                            GameManger.currField.ReturnSelfToBattle().beforePlayerInfo.hitPoint = GameManger.player.hitPoint;
                            GameManger.currField.StopEnemies();

                            //Utility.currRoom.enemyTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                            GameManger.player = GameManger.currField.ReturnSelfToBattle().beforePlayerInfo;
                            GameManger.currField = GameManger.currField.ReturnSelfToBattle().beforeFieldInfo;
                            if (GameManger.currField.type == 1)
                            {
                                GameManger.currField.PlayEnemies();
                                GameManger.currField.SetCreateTimer(new Timer(GameManger.currField.CreateEnemy, null, 100, 10000));
                            }
                            GameManger.currField.isFog = false;
                            GameManger.currField.isCurrField = true;

                            GameManger.player.RemoveFog();
                        }
                    }
                }
            }
        }
    }

}
