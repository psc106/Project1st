using Project1st.Game.GameObject;
using Project1st.Game.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.Interface
{
    public interface IUseItem
    {
        bool UseItem(Items item, Wagon obj);
    }
}
