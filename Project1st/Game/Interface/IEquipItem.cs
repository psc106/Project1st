using Project1st.Game.GameObject;
using Project1st.Game.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.Interface
{
    public interface IEquipItem
    {
        bool EquipItem(Items item, Wagon obj);
    }
}
