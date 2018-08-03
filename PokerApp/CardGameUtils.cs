using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectEulerSolutions
{
    public class Hand
    {
        #region ATTRIBUTES
        public enum HAND { CARDVALUE = 0, ONEPAIR = 1, TWOPAIRS = 2, THREEOFAKIND = 4, STRAIGHT = 8, FLUSH = 16, FULLHOUSE = 32, FOUROFAKIND = 64, STRAIGHTFLUSH = 128, ROYALFLUSH = 256 };
        public enum CARD { NONE = 0, _2 = 2, _3 = 3, _4 = 4, _5 = 5, _6 = 6, _7 = 7, _8 = 8, _9 = 9, _T = 10, _J = 11, _Q = 12, _K = 13, _A = 14 };
        public enum SUIT { D = 1, H = 2, C = 4, S = 8 };

        private const int numCards = 5;
        private string[] refHand;
        public CARD[] fiveCards = new CARD[numCards];
        public SUIT[] fiveSuits = new SUIT[numCards];

        public List<CARD> allCardValuesInOrder;
        public List<CARD> orderedUniqueValues;
        List<IGrouping<string, CARD>> stackedValues;
        List<IGrouping<string, SUIT>> stackedSuits;

        CARD highestCardInRank;
        HAND bestRankForHand;
        #endregion


        #region CONSTRUCTOR
        public Hand(string[] fiveCardHand)
        {
            if (fiveCardHand.Count() != numCards)
                throw new IllegalNumberOfCards();
            refHand = fiveCardHand;
            allCardValuesInOrder = GetStandardCardValues();

            fiveCards = (from v in GetCardValues(fiveCardHand.ToList()) select GetEnumFromString("_" + v)).ToArray();
            fiveSuits = (from s in GetCardSuits(fiveCardHand.ToList()) select (SUIT)Enum.Parse(typeof(SUIT), s.ToString())).ToArray();

            stackedValues = fiveCards.GroupBy(t => t.ToString()).OrderByDescending(u => u.Count()).ToList();
            stackedSuits = fiveSuits.GroupBy(t => t.ToString()).OrderByDescending(u => u.Count()).ToList();

            orderedUniqueValues = new List<CARD>(fiveCards.Distinct().ToList());
            orderedUniqueValues.Sort();

            highestCardInRank = orderedUniqueValues.Last();
            bestRankForHand = FindBestHand();
        }
        #endregion


        #region Poker card rankings
        // Royal Flush: Ten, Jack, Queen, King, Ace, in same suit.
        // TS JS QS KS AS
        public bool IsRoyalFlush()
        {
            bool isFound = false;
            if (GetLowestCard() == CARD._T && IsFlush() && IsFiveConsecutiveValues())
                isFound = true;
            return isFound;
        }

        // Straight Flush: All cards are consecutive values of same suit.
        // 3S 4S 5S 6S 7S
        public bool IsStraightFlush()
        {
            bool isFound = false;
            if (IsFlush() && IsFiveConsecutiveValues())
            {
                isFound = true;
                highestCardInRank = GetHighestCard();
            }
            return isFound;
        }

        // Four of a Kind: Four cards of the same value.
        // Not impl: Each of the four in a different suit.
        // TD TH TS TC KC
        public bool IsFourOfAkind()
        {
            bool isFound = false;
            if (stackedValues[0].Count() == 4)
            {
                isFound = true;
                highestCardInRank = stackedValues[0].FirstOrDefault();
            }
            return isFound;
        }

        // Full House: Three of a (value) kind and a pair.
        // 7C 7H 7S TD TC
        public bool IsFullHouse()
        {
            bool isFound = false;
            if (stackedValues.Count() == 2)
            {
                isFound = true;
                highestCardInRank = stackedValues[0].FirstOrDefault();
            }
            return isFound;
        }

        // Flush: All cards of the same suit.
        // JS 9S QS TS KS
        public bool IsFlush()
        {
            bool isFound = false;
            if (stackedSuits.Count == 1)
            {
                isFound = true;
                highestCardInRank = GetHighestCard();
            }
            return isFound;
        }

        // Straight: All cards are consecutive values.
        // JS 9C QS TS KD
        public bool IsStraight()
        {
            bool isFound = false;
            if (IsFiveConsecutiveValues())
            {
                isFound = true;
                highestCardInRank = GetHighestCard();
            }
            return isFound;
        }

        // Three of a Kind: Three cards of the same value.
        // Not Impl: plus Two unmatched cards.
        // 5S 5C KS 5D QD
        public bool IsThreeOfAKind()
        {
            bool isFound = false;
            if (stackedValues[0].Count() == 3
                //&& stackedSuits.Count() == 3
                )
            {
                isFound = true;
                highestCardInRank = stackedValues[0].FirstOrDefault();
            }
            return isFound;
        }

        // Two Pairs: Two different (value) pairs.
        // 5H 5D KS 7S KD
        public bool IsTwoPairs()
        {
            bool isFound = false;
            if (stackedSuits.Count() > 1 && stackedValues[0].Count() == 2 && stackedValues[1].Count() == 2)
            {
                isFound = true;
                highestCardInRank = stackedValues[1].FirstOrDefault(); // Options: _5 or _K
            }
            return isFound;
        }

        // One Pair: Two cards of the same value.
        // and three unmatched cards.
        // 5H 5C 6S 7S KD
        public bool IsOnePair()
        {
            bool isFound = false;
            if (stackedValues.Count == 4)
            {
                isFound = true;
                highestCardInRank = stackedValues[0].FirstOrDefault();
            }
            return isFound;
        }

        public bool IsFiveConsecutiveValues()
        {
            int cardSeqMatchCnt = 1;
            var firstCardName = GetLowestCard();
            int idxFirstCard = allCardValuesInOrder.IndexOf(firstCardName);

            for (int idx = 1; idx < numCards; idx++)
            {
                if (idx >= orderedUniqueValues.Count() || idxFirstCard + idx >= allCardValuesInOrder.Count())
                    break;
                CARD cardInHand = (CARD)Enum.Parse(typeof(CARD), orderedUniqueValues[idx].ToString());
                CARD refCardInSeq = (CARD)Enum.Parse(typeof(CARD), allCardValuesInOrder[idxFirstCard + idx].ToString());
                if (cardInHand.Equals(refCardInSeq))
                    cardSeqMatchCnt++;
            }
            return cardSeqMatchCnt == numCards;
        }
        #endregion


        #region PUBLIC METHODS
        public bool IsBetterThan(Hand player2Hand)
        {
            HAND comparisonHandRanking = player2Hand.GetRank();
            CARD comparisonHiCardInRank = player2Hand.GetHighestCardInRank();

            bool isCurrentRankBest = false;

            if (bestRankForHand != comparisonHandRanking)
            {
                isCurrentRankBest = bestRankForHand > comparisonHandRanking;
                //if (isCurrentRankBest)
                //    DisplayHandRanking(player2Hand, comparisonHandRanking, comparisonHiCardInRank, isCurrentRankBest);
            }
            else
            {
                isCurrentRankBest = highestCardInRank > comparisonHiCardInRank;
                //if (isCurrentRankBest)
                //    DisplayCardRanking(player2Hand, comparisonHandRanking, comparisonHiCardInRank, isCurrentRankBest);
            }

            return isCurrentRankBest;
        }


        public string GetHand()
        {
            string hand = "";
            string storedHand = "";
            for (int cnt = 0; cnt < numCards; cnt++)
            {
                hand += " " + fiveCards[cnt].ToString()[1] + fiveSuits[cnt];
                storedHand += " " + refHand[cnt];
            }
            hand = hand.TrimEnd();
            storedHand = storedHand.TrimEnd();

            if (!storedHand.Equals(hand))
                Console.WriteLine("!!!: '" + storedHand + "' != '" + hand + "':!!!");
            return hand;
        }
        #endregion


        #region PRIVATE METHODS
        private HAND FindBestHand()
        {
            HAND bestHand = HAND.CARDVALUE;

            if (IsRoyalFlush())
                bestHand = HAND.ROYALFLUSH;
            else if (IsStraightFlush())
                bestHand = HAND.STRAIGHTFLUSH;
            else if (IsFourOfAkind())
                bestHand = HAND.FOUROFAKIND;
            else if (IsFullHouse())
                bestHand = HAND.FULLHOUSE;
            else if (IsFlush())
                bestHand = HAND.FLUSH;
            else if (IsStraight())
                bestHand = HAND.STRAIGHT;
            else if (IsThreeOfAKind())
                bestHand = HAND.THREEOFAKIND;
            else if (IsTwoPairs())
                bestHand = HAND.TWOPAIRS;
            else if (IsOnePair())
                bestHand = HAND.ONEPAIR;

            return bestHand;
        }

        private  List<CARD> GetStandardCardValues()
        {
            return ((CARD[])Enum.GetValues(typeof(CARD))).ToList();
        }

        private  string GetCardStringNameFromValue(char enumRhsValue)
        {
            return Enum.GetName(typeof(CARD), enumRhsValue);
        }
        #endregion


        #region UTILITIES
        public  int GetEnumIndex<T>(T val)
        {
            int index = -1;
            int itemIndex = 0;
            foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if (item.Equals(val))
                {
                    index = itemIndex + 1;
                    break;
                }
                itemIndex++;
            }
            return index;
        }

        public  CARD GetEnumFromValue(char enumRhsValue)
        {
            return GetEnumFromString(GetCardStringNameFromValue(enumRhsValue));
        }

        public  CARD GetEnumFromString(string enumLhsName)
        {
            return (CARD)Enum.Parse(typeof(CARD), enumLhsName);
        }

        public  List<char> GetCardValues(List<string> hand)
        {
            return hand.Select(s => s[0]).ToList();
        }

        public  List<char> GetCardSuits(List<string> hand)
        {
            return hand.Select(s => s[1]).ToList();
        }

        public  IEnumerable<IGrouping<string, char>> GetUniqueSuits(List<string> hand)
        {
            return hand.Select(s => s[1]).GroupBy(t => t.ToString());
        }

        private void DisplayHandRanking(Hand player2Hand, HAND comparisonHandRanking, CARD comparisonHiCardInRank, bool isCurrentRankBest)
        {
            string output = string.Format("{0} [{1,12}:{2,1}] {3,1}{4} [{5,12}:{6,1}] {7}",
                GetHand(), bestRankForHand, highestCardInRank.ToString()[1],
                bestRankForHand > comparisonHandRanking ? ">" :
                (bestRankForHand < comparisonHandRanking ? "<" : "="),
                player2Hand.GetHand(), comparisonHandRanking, comparisonHiCardInRank.ToString()[1],
                isCurrentRankBest ? "WIN" : ""
                );
            Console.WriteLine(output);
        }

        private void DisplayCardRanking(Hand player2Hand, HAND comparisonHandRanking, CARD comparisonHiCardInRank, bool isCurrentRankBest)
        {
            string output = string.Format("{0} [{1,12}:{2,1}] {3,1}{4} [{5,12}:{6,1}] {7}",
                GetHand(), bestRankForHand, highestCardInRank.ToString()[1],
                highestCardInRank > comparisonHiCardInRank ? ">" :
                (highestCardInRank < comparisonHiCardInRank ? "<" : "="),
                player2Hand.GetHand(), comparisonHandRanking, comparisonHiCardInRank.ToString()[1],
                isCurrentRankBest ? "WIN" : ""
                );
            Console.WriteLine(output);
        }
        #endregion


        #region PROPERTIES
        public HAND GetRank()
        {
            return bestRankForHand;
        }

        public CARD GetHighestCardInRank()
        {
            return highestCardInRank;
        }

        private CARD GetLowestCard()
        {
            return orderedUniqueValues.FirstOrDefault();
        }

        private CARD GetHighestCard()
        {
            return orderedUniqueValues.LastOrDefault();
        }
        #endregion

    }

}
