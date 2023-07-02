﻿using Project1st.Game.Core;
using Project1st.Game.GameObject;
using Project1st.Game.Interface;
using Project1st.Game.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Map.Fields
{

    public class FieldBase : IUseItem, IEquipItem
    {
        public static readonly int _FIELD_SIZE = 15;

        public enum field_info : byte
        {
            road = 0, empty, mud, tree, wall, portal, error = 90
        }

        //0-길 1-빈땅 2-진창 3-나무 4-포탈
        public Portal[] portals;
        public field_info[,] fieldInfo;

        public int type;
        public bool isFog;
        public bool isCurrField;
        public bool isMenu;
        public bool isWin = false;

        public FieldBase()
        {
            isMenu = false;
            type = 0;
            portals = new Portal[4];
            fieldInfo = new field_info[_FIELD_SIZE, _FIELD_SIZE];

            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fieldInfo[y, x] = field_info.empty;
                }
            }
        }


        public FieldBase(int flag)
        {
            isMenu = false;
            type = 0;

            portals = new Portal[4];
            fieldInfo = new field_info[_FIELD_SIZE, _FIELD_SIZE];
            int bit = 1;

            for (int y = 0; y < _FIELD_SIZE; y++)
            {
                for (int x = 0; x < _FIELD_SIZE; x++)
                {
                    fieldInfo[y, x] = field_info.empty;
                }
            }

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


        public virtual void CreateDoor(int index)
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
        }

        public virtual bool PressYesEvent()
        {
            return false;
        }
        public virtual void PressNoEvent()
        {
        }
        public virtual bool PressMoveEvent()
        {
            return false;
        }
        public virtual void PressMenuEvent()
        {
        }



        public virtual void InitEnter()
        {
        }

        public virtual void Exit()
        {
        }



        public virtual float[,] GetFogInfo()
        {
            return null;
        }
        public virtual float GetFogInfo(int x, int y)
        {
            return 1;
        }
        public virtual void SetFogInfo(int x, int y, float info)
        {
        }


        public virtual List<Enemy> GetEnemies()
        {
            return null;
        }
        public virtual Timer GetCreateTimer()
        {
            return null;
        }
        public virtual void SetCreateTimer(Timer timer)
        {
        }
        public virtual void CreateEnemy(object obj)
        {
        }

        public field_info GetElementAt(int x, int y)
        {
            if (fieldInfo == null) return field_info.error;
            return fieldInfo[y, x];
        }
        public virtual void RemoveEnemy(int index)
        {
        }
        public virtual void RemoveEnemy(int x, int y)
        {
        }
        public virtual Enemy FindEnemiesAt(int x, int y)
        {
            return null;
        }
        public virtual Player FindPlayerAt(int x, int y)
        {
            return null;
        }
        public virtual void StopEnemies()
        {
        }
        public virtual void PlayEnemies()
        {
        }

        public Battle ReturnSelfToBattle()
        {
            if(type == 3) return (Battle)this;
            return null;
        }

        public Forest ReturnSelfToForest()
        {

            if (type == 1) return (Forest)this;
            return null;
        }

        public bool UseItem(Items item, Wagon currWagon)
        {
            if (item.type == 0)
            {
                //포션
                if (item.itemId == 0)
                {
                    if (GameManger.player.hitPoint == Player._PLAYER_HITPOINT_MAX)
                    {
                        return false;
                    }
                    else
                    {
                        item.count -= 1;
                        if (item.count == 0)
                        {
                            if (!GameManger.player.inventory.Remove(item))
                            currWagon.inventory.Remove(item);
                        }
                        GameManger.player.hitPoint += 20;
                        if (GameManger.player.hitPoint > Player._PLAYER_HITPOINT_MAX)
                        {
                            GameManger.player.hitPoint = Player._PLAYER_HITPOINT_MAX;
                        }
                        return true;
                    }
                }

                //공구
                if (item.itemId == 1)
                {
                    
                    if (currWagon.hitPoint == Wagon._WAGON_HITPOINT_MAX)
                    {
                        return false;
                    }
                    else
                    {
                        item.count -= 1;
                        if (item.count == 0)
                        {
                            if (!GameManger.player.inventory.Remove(item))
                                currWagon.inventory.Remove(item);
                        }
                        currWagon.hitPoint += 40;
                        if (currWagon.hitPoint > Wagon._WAGON_HITPOINT_MAX)
                        {
                            currWagon.hitPoint = Wagon._WAGON_HITPOINT_MAX;
                        }
                        return true;
                    }
                }

                //등불
                if (item.itemId == 2)
                {
                    if (GameManger.player.walk > 0)
                    {
                        return false;
                    }
                    else
                    {
                        item.count -= 1;
                        if (item.count == 0)
                        {
                            if (!GameManger.player.inventory.Remove(item))
                                currWagon.inventory.Remove(item);
                        }
                        GameManger.player.light += 3;
                        GameManger.player.walk = 100;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool EquipItem(Items item, Wagon currWagon)
        {
            if (item.type == 1)
            {
                GameManger.player.weapon = item;

                item.count -= 1;
                if (item.count == 0)
                {
                    if (!GameManger.player.inventory.Remove(item))
                        currWagon.inventory.Remove(item);
                }
            }
            return false;
        }
        public virtual string[] ConvertMapToString(ref string[] strings)
        {
            strings = null;
            return null;
        }

        public string[] MakeRoomToString()
        {
            string[] line = new string[3];

            for (int y = 0; y < line.Length; y++)
            {
                line[y] = "";

            }

            if (!this.isFog && this.portals[2] != null)
            {
                line[0] += "　↓　";
            }
            else
            {
                line[0] += "　　　";
            }

            if (!this.isFog && this.portals[1] != null)
            {
                line[1] += "→";
            }
            else
            {
                line[1] += "　";
            }

            if (!this.isFog)
            {
                if (!this.isCurrField && this.type == 1)
                {
                    line[1] += "□";
                }
                else if (!this.isCurrField && this.type == 2)
                {
                    line[1] += "☆";
                }
                else if (this.isCurrField && this.type == 1)
                {
                    line[1] += "■";
                }
                else if (this.isCurrField && this.type == 2)
                {
                    line[1] += "★";
                }
            }
            else
            {
                line[1] += "　";
            }

            if (!this.isFog && this.portals[0] != null)
            {
                line[1] += "←";
            }
            else
            {
                line[1] += "　";
            }
            if (!this.isFog && this.portals[3] != null)
            {
                line[2] += "　↑　";
            }
            else
            {
                line[2] += "　　　";
            }

            return line;
        }
    }


    public class Portal
    {
        public Coordinate axis;
        public bool isTown;

        public Portal()
        {
            axis = new Coordinate();
            isTown = false;
        }
        public Portal(int x, int y)
        {
            axis = new Coordinate(x, y);
            isTown = false;
        }
    }

}
