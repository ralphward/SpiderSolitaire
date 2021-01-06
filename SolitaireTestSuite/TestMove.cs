using SpiderSolitaire;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SolitaireTestSuite
{
    public class TestMove
    {

        public List<List<Card>> generateUnplayableGame()
        {
            List<List<Card>> c = new List<List<Card>>();
            c.Add(fillStack(6, nc(10)));
            c.Add(fillStack(6, nc(10)));
            c.Add(fillStack(6, nc(10)));
            c.Add(fillStack(6, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(0, null));

            return c;
        }

        public List<List<Card>> generatePlayableGame()
        {
            List<List<Card>> c = new List<List<Card>>();
            c.Add(fillStack(6, nc(10)));
            c.Add(fillStack(6, nc(10)));
            c.Add(fillStack(6, nc(10)));
            c.Add(fillStack(6, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(5, nc(10)));
            c.Add(fillStack(0, null));

            return c;
        }

        public List<Card> fillStack(int stacksize, Card visibleCard)
        {
            List<Card> c = new List<Card>();
            for (int i = 0; i < stacksize; i++)
                c.Add(nc(1));
            if (visibleCard != null)
                c.Add(visibleCard);

            return c;
        }

        public Card nc(int _value, bool _shown = false, int _suit = 0 )
        {
            return new Card() { Shown = _shown, Suit = _suit, Value = _value };
        }

        [Fact]
        public void TestPossMoves()
        {
            IList<Move> moves = Move.FindMoves(generateUnplayableGame());
            Assert.Equal(moves, new List<Move>());

        }

    }
}
