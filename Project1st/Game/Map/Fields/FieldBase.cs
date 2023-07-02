using Project1st.Game.Core;
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
    //필드 베이스 클래스
    public class FieldBase : IUseItem, IEquipItem
    {
        public static readonly int _FIELD_SIZE = 15;

        public enum field_info : byte
        {
            road = 0, empty, mud, tree, wall, portal, error = 90
        }
        public enum field_type : byte
        {
           nothing = 0, forest, town, battle, ending
        }

        //0-길 1-빈땅 2-진창 3-나무 4-포탈
        public Portal[] portals;
        public field_info[,] fieldInfo;

        public field_type type;
        public bool isFog;
        public bool isCurrField;
        public bool isMenu;
        public bool isWin = false;

        public FieldBase()
        {
            isMenu = false;
            type = field_type.nothing;
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
            type = field_type.nothing;

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



        //입력 처리
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
        //

        //입장
        public virtual void InitEnter()
        {
        }

        //퇴장
        public virtual void Exit()
        {
        }

        #region virtual 메서드


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
        public virtual void RemoveEnemy(int x, int y)
        {
        }
        public virtual Enemy FindEnemiesAt(int x, int y)
        {
            return null;
        }
       
        public virtual void StopEnemies()
        {
        }
        public virtual void PlayEnemies()
        {
        }
        public virtual void PlayEnemies(int time)
        {
        }

        #endregion

        public field_info GetElementAt(int x, int y)
        {
            if (fieldInfo == null) return field_info.error;
            return fieldInfo[y, x];
        }

        public Battle ReturnSelfToBattle()
        {
            if(type == field_type.battle) return (Battle)this;
            return null;
        }

        public Forest ReturnSelfToForest()
        {

            if (type == field_type.forest) return (Forest)this;
            return null;
        }

        //플레이어의 아이템 사용
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
                        GameManger.player.light += 2;
                        GameManger.player.walk = 100;
                        return true;
                    }
                }
            }
            return false;
        }

        //플레이어의 장비 변경.
        //장비 사용시 이전 장비 삭제
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

        //각 필드마다 다른 형태로 변환해서 출력한다.
        public virtual string[] ConvertMapToString(ref string[] strings)
        {
            strings = null;
            return null;
        }


        //미니맵용 스트링 output
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
                if (!this.isCurrField && this.type == field_type.forest)
                {
                    line[1] += "□";
                }
                else if (!this.isCurrField && this.type == field_type.town)
                {
                    line[1] += "☆";
                }
                else if (this.isCurrField && this.type == field_type.forest)
                {
                    line[1] += "■";
                }
                else if (this.isCurrField && this.type == field_type.town)
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


    //포탈
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
