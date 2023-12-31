﻿using Project1st.Game.Core;
using Project1st.Game.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.Map.Fields
{
    public class Ending : FieldBase
    {
        int flag;
        public Ending()
        {
            this.type = FieldBase.field_type.ending;
        }
        public Ending(int flag) 
        {
            this.type = FieldBase.field_type.ending;
            this.flag = flag;

            GameManger.currField = this;
        }


        public override string[] ConvertMapToString(ref string[] line)
        {
            //패배
            if (flag == 1)
            {
                line[0] = "실망입니다\t\t\t\t\t\t\t\t\t\t\t\t\t";
                line[1] = "당신은 죽었습니다\t\t\t\t\t\t\t\t\t\t\t\t";
                for (int i = 2; i < BufferPrinter._BUFFER_SIZE; i++)
                {
                    line[i] = "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";

                }
            }

            //승리
            else if (flag == 2)
            {
                line[0] = "축하합니다\t\t\t\t\t\t\t\t\t\t\t\t\t";
                line[1] = "당신은 자수성가 했습니다\t\t\t\t\t\t\t\t\t\t\t\t";
                for (int i = 2; i < BufferPrinter._BUFFER_SIZE; i++)
                {
                    line[i] = "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";
                }
            }

            return line;
        }


    }
}
