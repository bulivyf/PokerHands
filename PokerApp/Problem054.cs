using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectEulerSolutions
{
    class Problem054
    {
        private enum PLAYER { ONE, TWO };

        //public  void Main()
        //{
        //    new Problem054().DoProblem();
        //}

        public void DoProblem()
        {
            List<String> plays = GetPlaysFromFile("../../../PokerApp/Data/p54_poker.txt");

            var enumlist = SubDivideList(plays, 250);
            var strlist = enumlist.ToList();

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var task1 = Task.Factory.StartNew(() => Play(strlist[0]));
            var task2 = Task.Factory.StartNew(() => Play(strlist[1]));
            var task3 = Task.Factory.StartNew(() => Play(strlist[2]));
            var task4 = Task.Factory.StartNew(() => Play(strlist[3]));
            Task.WaitAll(task1, task2, task3, task4);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine("Elapsed: " + elapsedMs);

            Console.WriteLine(task1.Result.Player1 + " " + task1.Result.Player2);
            Console.WriteLine(task2.Result.Player1 + " " + task2.Result.Player2);
            Console.WriteLine(task3.Result.Player1 + " " + task3.Result.Player2);
            Console.WriteLine(task4.Result.Player1 + " " + task4.Result.Player2);
            int player1Wins = task1.Result.Player1
                + task2.Result.Player1
                + task3.Result.Player1
                + task4.Result.Player1;
            int player2Wins = task1.Result.Player2
                 + task2.Result.Player2
                 + task3.Result.Player2
                 + task4.Result.Player2;
            Console.WriteLine(player1Wins + " <=> " + player2Wins);
            Console.Write("Done");
        }

        private PlayerWinCount Play(List<string> playList)
        {
            PlayerWinCount winCounter = new PlayerWinCount();
            foreach (string play in playList)
            {
                PLAYER winner = GetWinnerFromHand(play);
                switch (winner)
                {
                    case PLAYER.ONE:
                        winCounter.Player1++;
                        break;
                    case PLAYER.TWO:
                        winCounter.Player2++;
                        break;
                }
            }
            return winCounter;
        }

        private PLAYER GetWinnerFromHand(String hand)
        {
            List<string> cards = hand.Split(' ').ToList();
            var playerHands = SubDivideList(cards, 5).ToList();
            Hand player1Hand = new Hand(playerHands[0].ToArray());
            Hand player2Hand = new Hand(playerHands[1].ToArray());
            return player1Hand.IsBetterThan(player2Hand) ? PLAYER.ONE : PLAYER.TWO;
        }

        public IEnumerable<List<T>> SubDivideList<T>(IEnumerable<T> enumerable, int count)
        {
            int index = 0;
            return from l in enumerable
                   group l by index++ / count
                into l
                   select l.ToList();
        }

        private List<string> GetPlaysFromFile(string v)
        {
            List<string> lines = new List<String>();
            StreamReader f = new StreamReader(v);
            while (!f.EndOfStream)
            {
                lines.Add(f.ReadLine());
            }
            return lines;
        }

        class PlayerWinCount
        {
            int player1 = 0;
            int player2 = 0;

            public int Player2 { get => player2; set => player2 = value; }
            public int Player1 { get => player1; set => player1 = value; }
        }

    }
}