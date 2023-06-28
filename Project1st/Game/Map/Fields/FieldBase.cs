using Project1st.Game.Core;
using Project1st.Game.GameObject;
using Project1st.Game.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Map.Fields
{

    public class FieldBase
    {
        protected int[] AXIS_X = { 1, -1, 0, 0, 0, 0 };
        protected int[] AXIS_Y = { 0, 0, -1, 1, 0, 0 };

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

        public FieldBase()
        {
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

        public virtual string[] ConvertMapToString(ref string[] strings)
        {
            strings = null;
            return null;
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
                if (!this.isCurrField)
                {
                    line[1] += "□";
                }
                else
                {
                    line[1] += "■";
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

        public virtual bool Move(Coordinate axis)
        {
            return false;
        }
        public virtual bool Move()
        {
            return false;
        }

        public Forest ReturnSelfToForest()
        {
            return (Forest)this;
        }

        public Town ReturnSelfToTown()
        {
            return (Town)this;
        }
    }

    public class City : FieldBase
    {
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
