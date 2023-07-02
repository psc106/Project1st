using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.GameObject
{
    public class ObjectBase
    {
        public Coordinate Axis2D;

        public ObjectBase()
        {
            Init();
        }

        public virtual void Init()
        {
            Axis2D = new Coordinate();
        }
    }

    public class MoveObject : ObjectBase
    {
        public int ID;
        public bool isLive;

        protected readonly int[] _OBJECT_AXIS_MATRIX_X = { 1, -1, 0, 0, 0, 0 };
        protected readonly int[] _OBJECT_AXIS_MATRIX_Y = { 0, 0, -1, 1, 0, 0 };

        public int hitPoint;
        public int attckPoint;

        public int direction { get; set; }

        public MoveObject() 
        {
            Init();
        }

        public override void Init()
        {
            base.Init();
            direction = 5;
            isLive = true;
        }


        public int GetNextX(int direction)
        {
            return Axis2D.x + _OBJECT_AXIS_MATRIX_X[direction];
        }

        public int GetNextY(int direction)
        {
            return Axis2D.y + _OBJECT_AXIS_MATRIX_Y[direction];
        }
        public int Hold(int axis, int size)
        {
            if (axis < 0)
            {
                axis = 0;
            }
            else if (axis >= size)
            {
                axis = size - 1;
            }
            return axis;
        }

        public bool MoveAndHold(int direction, int XSize, int YSize)
        {
            Axis2D.x += _OBJECT_AXIS_MATRIX_X[direction];
            Axis2D.y += _OBJECT_AXIS_MATRIX_Y[direction];

            return HoldX(XSize) || HoldY(YSize);
        }


        public bool HoldX(int XSize)
        {
            if (Axis2D.x < 0)
            {
                Axis2D.x = 0;
                return true;
            }
            else if (Axis2D.x >= XSize)
            {
                Axis2D.x = XSize - 1;
                return true;
            }
            return false;

        }
        public bool HoldY(int YSize)
        {
            if (Axis2D.y < 0)
            {
                Axis2D.y = 0;
                return true;
            }
            else if (Axis2D.y >= YSize)
            {
                Axis2D.y = YSize - 1;
                return true;
            }
            return false;
        }

        public void HoldXY(ref int x, ref int y, int sizeX, int sizeY)
        {
            if (x < 0)
            {
                x = 0;
            }
            else if (x >= sizeX)
            {
                x = sizeX - 1;
            }
            if (y < 0)
            {
                y = 0;
            }
            else if (y >= sizeY)
            {
                y = sizeY - 1;
            }

        }
        public void Teleport(int XSize, int Ysize)
        {
            if (Axis2D.x == 0)
            {
                Axis2D.x = XSize - 1;
            }
            else if (Axis2D.x == XSize - 1)
            {
                Axis2D.x = 0;
            }
            else if (Axis2D.y == 0)
            {
                Axis2D.y = Ysize - 1;
            }
            else if (Axis2D.y == Ysize - 1)
            {
                Axis2D.y = 0;
            }
        }
    }
}
