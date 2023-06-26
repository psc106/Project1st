using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.GameObject
{
    public class MainObject
    {
        public Coordinate Axis2D;

        public MainObject()
        {
            Init();
        }

        public virtual void Init()
        {
            Axis2D = new Coordinate();
        }
    }

    public class MoveObject : MainObject
    {
        public int ID;
        public bool isLive;

        protected int[] AXIS_X = { 1, -1, 0, 0, 0, 0 };
        protected int[] AXIS_Y = { 0, 0, -1, 1, 0, 0 };

        public int hitPoint;
        public int direction { get; set; }

        public MoveObject() 
        {
            Init();
        }

        public override void Init()
        {
            base.Init();
            direction = 4;
            isLive = true;
        }


        public int GetNextX(int direction)
        {
            return Axis2D.x + AXIS_X[direction];
        }

        public int GetNextY(int direction)
        {
            return Axis2D.y + AXIS_Y[direction];
        }

        public bool MoveAndHold(int direction, int XSize, int YSize)
        {
            Axis2D.x += AXIS_X[direction];
            Axis2D.y += AXIS_Y[direction];

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
