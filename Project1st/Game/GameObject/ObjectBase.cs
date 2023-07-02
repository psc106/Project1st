using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.GameObject
{
    //모든 object들은 이를 상속한다.
    public class ObjectBase
    {
        //좌표 저장
        public Coordinate axis;

        public ObjectBase()
        {
            Init();
        }

        public virtual void Init()
        {
            axis = new Coordinate();
        }
    }


    //모든 이동하는 오브젝트는 이를 상속한다.
    //플레이어, 적, 이펙트, 마차
    public class MoveObject : ObjectBase
    {
        //좌표 변화값 - direction과 연관
        protected readonly int[] _OBJECT_AXIS_MATRIX_X = { 1, -1, 0, 0, 0, 0 };
        protected readonly int[] _OBJECT_AXIS_MATRIX_Y = { 0, 0, -1, 1, 0, 0 };
       
        //id. 현재는 안쓰인다.
        public int ID;

        //활성화 유/무
        public bool isLive;

        //체력, 공격력
        public int hitPoint;
        public int attckPoint;

        //방향
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
            return axis.x + _OBJECT_AXIS_MATRIX_X[direction];
        }

        public int GetNextY(int direction)
        {
            return axis.y + _OBJECT_AXIS_MATRIX_Y[direction];
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
            axis.x += _OBJECT_AXIS_MATRIX_X[direction];
            axis.y += _OBJECT_AXIS_MATRIX_Y[direction];

            return HoldX(XSize) || HoldY(YSize);
        }


        public bool HoldX(int XSize)
        {
            if (axis.x < 0)
            {
                axis.x = 0;
                return true;
            }
            else if (axis.x >= XSize)
            {
                axis.x = XSize - 1;
                return true;
            }
            return false;

        }
        public bool HoldY(int YSize)
        {
            if (axis.y < 0)
            {
                axis.y = 0;
                return true;
            }
            else if (axis.y >= YSize)
            {
                axis.y = YSize - 1;
                return true;
            }
            return false;
        }


        public void Teleport(int XSize, int Ysize)
        {
            if (axis.x == 0)
            {
                axis.x = XSize - 1;
            }
            else if (axis.x == XSize - 1)
            {
                axis.x = 0;
            }
            else if (axis.y == 0)
            {
                axis.y = Ysize - 1;
            }
            else if (axis.y == Ysize - 1)
            {
                axis.y = 0;
            }
        }
    }
}
