using Project1st.Game.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Core
{
    public class Core
    {
        public Core()
        {
            GameManger.Instance();

            GameManger.player.Axis2D.x = 1;
            GameManger.player.Axis2D.y = 1;

            //그리기 타이머
            Timer timer_Print = new Timer(PrintMap, null, 100, 100);

        }

        public void PrintTimer(object obj)
        {
            if (GameManger.map == null) { return; }

            GameManger.buffer.SetBuffer(/*.Tostring()*/null);
            GameManger.buffer.PrintBuffer();
        }

        public void Start()
        {

        }

        public void MapToStringArray(ref string[] line)
        {
            for (int y = 0; y < Field.ROOM_SIZE; y++)
            {
                line[y] = "";
                for (int x = 0; x < Room.ROOM_SIZE; x++)
                {
                    //이펙트 1순위
                    if (AttackEffect != null && AttackEffect.Count > 0)
                    {
                        MyEffect currEffect = AttackEffect.FindAll(effect => (effect.X == x && effect.Y == y)).FirstOrDefault();
                        if (currEffect != null)
                        {
                            line[y] += ".8." + MyEffect.ATTACK_STRING[currEffect.type] + ".";
                            AttackEffect.Remove(currEffect);
                            continue;
                        }
                    }

                    //적 2순위
                    Enemy tmp = Utility.currRoom.FindEnemiesAt(x, y);
                    if (tmp != null)
                    {
                        if (tmp.isLive)
                        {
                            switch (tmp.hitPoint)
                            {
                                case 1:
                                    line[y] += ".7.";
                                    break;
                                case 2:
                                    line[y] += ".1.";
                                    break;
                                default:
                                    line[y] += ".0.";
                                    break;
                            }
                            if (tmp.randMoveCount > 0)
                            {
                                line[y] += "？.";
                            }
                            else
                            {
                                line[y] += "적.";
                            }
                        }
                        else if (!tmp.isLive)
                        {
                            line[y] += ".3.봉.";
                        }
                        continue;
                    }

                    //NPC 3순위
                    NonPlayableCharacter npc = Utility.currRoom.FindNPCAt(x, y);
                    if (npc != null)
                    {
                        if (npc.isLive)
                        {
                            if (0 <= npc.hitPoint && npc.hitPoint < 10)
                            {
                                line[y] += ".9.";
                            }
                            else if (10 <= npc.hitPoint && npc.hitPoint <= 20)
                            {
                                line[y] += ".10.";
                            }
                            else
                            {
                                line[y] += ".11.";
                            }
                            line[y] += "★.";
                        }
                        else if (!tmp.isLive)
                        {
                            line[y] += ".3.봉.";
                        }
                        continue;
                    }

                    //플레이어 4순위
                    if (Utility.player.X == x && Utility.player.Y == y)
                    {
                        if (Utility.player.isLive)
                        {
                            switch (Utility.player.hitPoint)
                            {
                                case 1:
                                    line[y] += ".6.";
                                    break;
                                case 2:
                                    line[y] += ".5.";
                                    break;
                                case 3:
                                    line[y] += ".4.";
                                    break;
                                default:
                                    line[y] += ".0.";
                                    break;
                            }
                            switch (direction)
                            {
                                case 0:
                                    line[y] += "▶.";
                                    break;
                                case 1:
                                    line[y] += "◀.";
                                    break;
                                case 2:
                                    line[y] += "▲.";
                                    break;
                                case 3:
                                    line[y] += "▼.";
                                    break;
                                case 4:
                                    line[y] += "！.";
                                    break;
                                default:
                                    line[y] += "나.";
                                    break;
                            }

                        }
                        else
                        {
                            line[y] += ".3.Π.";
                        }
                        continue;
                    }

                    //맵정보 마지막
                    switch (Utility.currRoom.roomInfomation[y, x])
                    {
                        case 0:
                            line[y] += ".0.　.";
                            break;
                        case 1:
                            line[y] += ".0.＊.";
                            break;
                        case 2:
                            line[y] += ".0.〓.";
                            break;
                        case 3:
                            line[y] += ".2.문.";
                            break;
                    }

                }
            }


            for (int y = 0; y < map.field.GetLength(0); y++)
            {
                for (int x = 0; x < map.field.GetLength(1); x++)
                {
                    string[] tmp = map.field[y, x].MakeRoomToString();

                    line[y * 3] += tmp[0];
                    line[y * 3 + 1] += tmp[1];
                    line[y * 3 + 2] += tmp[2];
                }
            }

        }

        public void PrintMap(Object obj)
        {
            MapToStringArray(ref line);
            if (!buffer.isWork)
            {
                buffer.SetBuffer((string[])obj);
                buffer.PrintBuffer();
            }
            else
            {
                Console.Clear();
            }
        }
    }
}
