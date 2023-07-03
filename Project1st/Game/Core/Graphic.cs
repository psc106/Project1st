using Project1st.Game.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project1st.Game.Core
{
    public class BufferPrinter
    {
        //버퍼 크기
        public static readonly int _BUFFER_SIZE = 45;

        //밖에서 전달해준 값을 저장할 버퍼
        private string[] buffer;

        //만약 종료할 일이 있다면 이 Timer 변수를 dispose한다.
        public Timer printTimer;

        //동작중인지 체크할 변수
        public bool isWork { private set; get; }

        //색깔
        private enum Color
        {
            WHITE = 0, RED, GREEN, BLUE,
            YELLOW = 4, CYAN, DARK_GRAY, DARK_RED,
            DARK_MAGENTA = 8, DARK_BLUE, DARK_CYAN, DARK_GREEN, 
            DARK_YELLOW = 12, BLACK=99
        }

        //생성자
        public BufferPrinter()
        {
            isWork = false;
            buffer = new string[_BUFFER_SIZE];
        }

        //먼저 저장한다.
        public void SetBuffer(string[] map)
        {
            buffer = map;
        }

        //출력 시작
        public void PrintBuffer()
        {
            //앞에서 이미 실행중이면 종료
            if (isWork) { return; }
            //제대로 준비가 안되있으면 종료
            if (buffer == null) { return; }

            //실행 전 처리
            isWork = true;

            //1행씩 출력한다.
            for (int y = 0; y < buffer.Length; y++)
            {
                //만약 해당 행이 null일 경우 해당 행은 넘긴다.
                if (buffer[y] == null) { break; }

                //행 마다 커서 강제 이동
                Console.SetCursorPosition(0, y);
                
                //색을 넣기위해 split한다.
                string[] splitString = buffer[y].Split('.');

                for (int i = 0; i < splitString.Length; i++)
                {
                    //split할 경우
                    //1번째 : 공백또는 색이 없는 글자로 판단하고 이를 출력한다.
                    if (i % 3 == 0)
                    {
                        Console.Write(splitString[i]);
                    }
                    //2번째 : 색 정보를 저장한 정수를 가져오고 이에 따라서 색을 지정해준다.
                    else if (i % 3 == 1)
                    {
                        Color color = (Color)int.Parse(splitString[i]);
                        switch (color)
                        {
                            //0
                            case Color.WHITE:
                                Console.ForegroundColor = ConsoleColor.White;
                                break;
                            //1
                            case Color.RED:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            //2
                            case Color.GREEN:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            //3
                            case Color.BLUE:
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            //4
                            case Color.YELLOW:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            //5
                            case Color.CYAN:
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                            //6
                            case Color.DARK_GRAY:
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                break;
                            //7
                            case Color.DARK_RED:
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                break;
                            //8
                            case Color.DARK_MAGENTA:
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                break;
                            //9
                            case Color.DARK_BLUE:
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                break;
                            //10
                            case Color.DARK_CYAN:
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                break;
                            //11
                            case Color.DARK_GREEN:
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                break;
                            //12
                            case Color.DARK_YELLOW:
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                break;

                            //99?
                            case Color.BLACK:
                                Console.ForegroundColor = ConsoleColor.Black;
                                break;
                            default:
                                break;
                        }   //[switch] end 컬러체크
                    }

                    //3번째 : 색을 넣을 문자열을 출력해준다. 그후 초기화한다.
                    else if (i % 3 == 2)
                    {
                        Console.Write(splitString[i]);
                        Console.ResetColor();
                    }


                }// 1행 출력 종료

            }//모든 행 출력 종료

            //테스트 코드
          /*  Console.SetCursorPosition(50, 40);
            Console.WriteLine(WorldMap.testPos.x+", "+ WorldMap.testPos.y);*/

            //실행 종료
            isWork = false;
        }

    }
}
