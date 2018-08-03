using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HD = ProjectEulerSolutions.Hand;


namespace UnitTestProjectEuler
{
    [TestClass]
    public class HandUnitTest
    {
        [TestMethod]
        public void TestHighFiveCardSequence()
        {
            HD hand = new HD("AS JS KS QS TS".Split(' '));
            Assert.IsTrue(hand.IsFiveConsecutiveValues());
        }

        [TestMethod]
        public void TestLowFiveCardSequence()
        {
            HD hand = new HD("6S 5S 4S 3S 2S".Split(' '));
            Assert.IsTrue(hand.IsFiveConsecutiveValues());
        }

        [TestMethod]
        public void TestIsRoyalFlush()
        {
            String[] hands = new String[]{ "TS JS QS KS AS", "AC JC KC QC TC", "AH TH JH QH KH", "AD KD QD JD TD" };
            foreach (String refHand in hands)
            {
                HD hand = new HD(refHand.Split(' '));
                Assert.IsTrue(hand.IsRoyalFlush());
                Assert.IsTrue(hand.GetRank() == HD.HAND.ROYALFLUSH);
            }

            String[] badhands = new String[] { "TS JS 9S KS QS", "AC JC 7C QC TC", "5H TH JH QH KH", "6D 7D 8D 9D TD" };
            foreach (String refHand in badhands)
            {
                HD hand = new HD(refHand.Split(' '));
                Assert.IsFalse(hand.IsRoyalFlush());
                Assert.IsFalse(hand.GetRank() == HD.HAND.ROYALFLUSH);
            }

        }

        [TestMethod]
        public void TestIsStraightFlush()
        {
            String[] hands = new String[] { "3S 4S 5S 6S 7S", "8C 7C 6C 5C 4C", "JH TH 9H QH KH", "QD JD TD 9D 8D" };
            foreach (String refHand in hands)
            {
                HD hand = new HD(refHand.Split(' '));
                Assert.IsTrue(hand.IsStraightFlush());
                Assert.IsTrue(hand.GetRank() == HD.HAND.STRAIGHTFLUSH);
                Assert.IsTrue(hand.GetHighestCardInRank() == hand.orderedUniqueValues.ToArray()[hand.orderedUniqueValues.Count-1]);

            }
        }

        [TestMethod]
        public void TestIsFourOfAkind()
        {
            HD hand = new HD("TD TH TS TC KC".Split(' '));
            Assert.IsTrue(hand.IsFourOfAkind());
            Assert.IsTrue(hand.GetRank() == HD.HAND.FOUROFAKIND);
            Assert.IsTrue(hand.GetHighestCardInRank() == HD.CARD._T);//hand.orderedUniqueValues.ToArray()[0]);
        }

        [TestMethod]
        public void TestIsFullHouse()
        {
            HD hand = new HD("7C 7H 7S TD TC".Split(' '));
            Assert.IsTrue(hand.IsFullHouse());
            Assert.IsTrue(hand.GetRank() == HD.HAND.FULLHOUSE);
            Assert.IsTrue(hand.GetHighestCardInRank() == HD.CARD._7);
        }

        [TestMethod]
        public void TestIsFlush()
        {
            HD hand = new HD("JS 9S 4S TS KS".Split(' '));
            Assert.IsTrue(hand.IsFlush());
            Assert.IsTrue(hand.GetRank() == HD.HAND.FLUSH);
            Assert.IsTrue(hand.GetHighestCardInRank() == HD.CARD._K);
        }

        [TestMethod]
        public void TestIsStraight()
        {
            HD hand = new HD("JS 9C QS TS KD".Split(' '));
            Assert.IsTrue(hand.IsStraight());
            Assert.IsTrue(hand.GetRank() == HD.HAND.STRAIGHT);
            Assert.IsTrue(hand.GetHighestCardInRank() == HD.CARD._K);
        }

        [TestMethod]
        public void TestIsThreeOfAKind()
        {
            HD hand = new HD("5S 5C KS 5D QD".Split(' '));
            Assert.IsTrue(hand.IsThreeOfAKind());
            Assert.IsTrue(hand.GetRank() == HD.HAND.THREEOFAKIND);
            Assert.IsTrue(hand.GetHighestCardInRank() == HD.CARD._5);
        }

        [TestMethod]
        public void TestIsTwoPairs()
        {
            HD hand = new HD("5H 5D KS 7S KD".Split(' '));
            Assert.IsTrue(hand.IsTwoPairs());
            Assert.IsTrue(hand.GetRank() == HD.HAND.TWOPAIRS);
            Assert.IsTrue(hand.GetHighestCardInRank() == HD.CARD._K);

        }

        [TestMethod]
        public void TestIsOnePair()
        {
            // 
            HD hand = new HD("5H 5C 6S 7S KD".Split(' '));
            Assert.IsTrue(hand.IsOnePair());
            Assert.IsTrue(hand.GetRank() == HD.HAND.ONEPAIR);
            Assert.IsTrue(hand.GetHighestCardInRank() == HD.CARD._5);

            hand = new HD("4D 9H AS TC QH".Split(' '));
            Assert.IsFalse(hand.IsOnePair());
            Assert.IsFalse(hand.GetRank() == HD.HAND.ONEPAIR);
        }


    }
}
