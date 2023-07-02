using Project1st.Game.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.Item
{
    public class PriceRate
    {
        //현재 시세 비율
        public float currRate;
        //기본 시세 비율
        public float mainRate;

        //시세 유지 시간
        public int keepTurn;
        //다음 시세 상태
        public state nextState;

        public enum state
        {
            keep = 0, bigDown, down, up, bigUp
        }

        public PriceRate()
        {
            this.mainRate = 1;
            this.currRate = this.mainRate;
            this.keepTurn = GameManger.random.Next(1, 4);
            this.nextState = state.keep;
        }

        public PriceRate(float rate)
        {
            this.mainRate = rate;
            this.currRate = this.mainRate;
            this.keepTurn = GameManger.random.Next(1, 4);
            this.nextState = state.keep;
        }

        //매 일 keepturn을 감소시키고 시간이 되면 시세를 변경한다. 
        public void ChangePriceRate()
        {
            if (keepTurn == 0)
            {
                switch (nextState)
                {
                    case state.bigDown:
                        this.currRate = mainRate - GameManger.random.Next(70, 90) * 0.01f;
                        break;
                    case state.down:
                        this.currRate = mainRate - GameManger.random.Next(10, 25) * 0.01f;
                        break;
                    case state.keep:
                        break;
                    case state.up:
                        this.currRate = mainRate + GameManger.random.Next(10, 25) * 0.01f;
                        break;
                    case state.bigUp:
                        this.currRate = mainRate + GameManger.random.Next(80, 150) * 0.01f;
                        break;
                }
                if (currRate < 0) currRate = 0;

                this.keepTurn = GameManger.random.Next(1, 3);
                int next = GameManger.random.Next(0, 101);
                if (next < 5)
                {
                    nextState = state.bigDown;
                }
                else if (next < 5 + 10)
                {
                    nextState = state.down;
                }
                else if (next < 5 + 10 + 70)
                {
                    nextState = state.keep;
                }
                else if (next < 5 + 10 + 70 + 10)
                {
                    nextState = state.up;
                }
                else if (next <= 5 + 10 + 70 + 10 + 5)
                {
                    nextState = state.bigUp;
                }
            }
            else
            {
                keepTurn -= 1;
            }
        }//[ChangePriceRate] end
    }
}
