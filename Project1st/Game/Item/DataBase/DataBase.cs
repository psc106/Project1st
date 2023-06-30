using Project1st.Game.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1st.Game.DataBase
{
    public class DB
    {
        public Dictionary<int, Items> database;

        public DB()
        {
            database = new Dictionary<int, Items>();

        }

        public void PutData()
        {

            //기본 사용도구 0~3 4
            database.Add(0, new Items(0, 0, "포션　　　", 100, int.MaxValue, 1));
            database.Add(1, new Items(1, 0, "공구　　　", 30, int.MaxValue, 3));
            database.Add(2, new Items(2, 0, "등불　　　", 70, int.MaxValue, 5));
            database.Add(3, new Items(3, 0, "마차　　　", 1000, int.MaxValue, -100));

            //장비 4~9 6
            database.Add(4, new Items(4, 1, "대검　　　", 1050, 10, 10));
            database.Add(5, new Items(5, 1, "창　　　　", 400, 10, 6));
            database.Add(6, new Items(6, 1, "도끼　　　", 600, 10, 8));
            database.Add(7, new Items(7, 1, "단검　　　", 150, 10, 2));
            database.Add(8, new Items(8, 1, "망치　　　", 540, 10, 20));
            database.Add(9, new Items(9, 1, "검　　　　", 600, 10, 4));

            //사치품? 10 ~50 41
            database.Add(10, new Items(10, 2, "책　　　　", 250, 1, 8));
            database.Add(11, new Items(11, 2, "사과　　　", 12, 1, 6));
            database.Add(12, new Items(12, 2, "갑옷　　　", 301, 1, 20));
            database.Add(13, new Items(13, 2, "투구　　　", 120, 1, 8));
            database.Add(14, new Items(14, 2, "장갑　　　", 82, 1, 5));
            database.Add(15, new Items(15, 2, "모자　　　", 70, 1, 4));
            database.Add(16, new Items(16, 2, "망토　　　", 340, 1, 14));
            database.Add(17, new Items(17, 2, "향신료　　", 152, 1, 3));
            database.Add(18, new Items(18, 2, "소금　　　", 60, 1, 3));
            database.Add(19, new Items(19, 2, "수박　　　", 14, 1, 30));
            database.Add(20, new Items(20, 2, "감자　　　", 11, 1, 18));
            database.Add(21, new Items(21, 2, "마법석　　", 1507, 1, 50));
            database.Add(22, new Items(22, 2, "허리띠　　", 60, 1, 5));
            database.Add(23, new Items(23, 2, "목걸이　　", 105, 1, 2));
            database.Add(24, new Items(24, 2, "반지　　　", 100, 1, 3));
            database.Add(25, new Items(25, 2, "귀걸이　　", 60, 1, 2));
            database.Add(26, new Items(26, 2, "망원경　　", 200, 1, 7));
            database.Add(27, new Items(27, 2, "용비늘　　", 400, 1, 12));
            database.Add(28, new Items(28, 2, "초콜렛　　", 15, 1, 4));
            database.Add(29, new Items(29, 2, "사탕　　　", 10, 1, 4));
            database.Add(30, new Items(30, 2, "만년필　　", 80, 1, 2));
            database.Add(31, new Items(31, 2, "다이아몬드", 450, 1, 1));
            database.Add(32, new Items(32, 2, "사파이어　", 301, 1, 1));
            database.Add(33, new Items(33, 2, "루비　　　", 350, 1, 1));
            database.Add(34, new Items(34, 2, "신발　　　", 20, 1, 3));
            database.Add(35, new Items(35, 2, "구슬　　　", 100, 1, 3));
            database.Add(36, new Items(36, 2, "우산　　　", 50, 1, 6));
            database.Add(37, new Items(37, 2, "석탄　　　", 123, 1, 22));
            database.Add(38, new Items(38, 2, "배　　　　", 14, 1, 20));
            database.Add(39, new Items(39, 2, "철광석　　", 100, 1, 10));
            database.Add(40, new Items(40, 2, "생선　　　", 40, 1, 8));
            database.Add(41, new Items(41, 2, "붓　　　　", 14, 1, 2));
            database.Add(42, new Items(42, 2, "물감　　　", 30, 1, 5));
            database.Add(43, new Items(43, 2, "캔버스　　", 20, 1, 2));
            database.Add(44, new Items(44, 2, "비누　　　", 77, 1, 4));
            database.Add(45, new Items(45, 2, "무　　　　", 33, 1, 12));
            database.Add(46, new Items(46, 2, "가방　　　", 127, 1, 8));
            database.Add(47, new Items(47, 2, "천　　　　", 13, 1, 2));
            database.Add(48, new Items(48, 2, "보약　　　", 3005, 1, 6));
            database.Add(49, new Items(49, 2, "가면　　　", 123, 1, 4));
            database.Add(50, new Items(50, 2, "화장품　　", 236, 1, 2));

            database.Add(100, new Items(100, 3, "땅문서　　", 100000, int.MaxValue, 0));

        }
    }
}
