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
        //플레이어가 팔때
        public float currRate;
        public float mainRate;

        public int keepTurn;
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
        }
    }
}
