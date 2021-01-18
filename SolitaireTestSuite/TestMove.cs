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

        public Card snc(int _value, bool _shown = true, int _suit = 0)
        {
            return nc(_value, _shown, _suit);
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

        [Fact]
        public void TestBug()
        {
            Move MoveR = new Move()
            {
                weight = 1,
                srcColumn = 5,
                destColumn = 6,
                cardNumber = 6,
                interimMove = true,
                suitRemoved = 0
            };

            IList<List<Card>> board = new List<List<Card>>();

            Move PlayMove = new Move()
            {
               weight = -1,
               srcColumn = 5,
               destColumn = 9,
               cardNumber = 6,
               interimMove = true,
               suitRemoved = -1
            };
            board.Add(new List<Card> { snc(12), snc(11) });
            board.Add(new List<Card> { snc(6), snc(5), snc(4), snc(3), snc(2), snc(1), snc(6), snc(8), snc(7), snc(6), snc(5), snc(4), snc(3), snc(2)});
            board.Add(new List<Card> { nc(7),nc(5),nc(11),nc(1),nc(7), snc(8 ), snc(12), snc(10), snc(9 ), snc(8 ), snc(12), snc(0 ) });
            board.Add(new List<Card> { snc(12), snc(11), snc(10), snc(9 ), snc(8 ), snc(7 ), snc(6 ), snc(12),});
            board.Add(new List<Card> { nc(2), nc(2), nc(7), nc(5), snc(5), snc(4), snc(3), snc(7), snc(12), snc(11), snc(10), snc(9), snc(1), snc(0), snc(8), snc(0) });
            board.Add(new List<Card> { nc(6), nc(0), nc(10), nc(9), snc(10), snc(3) });
            board.Add(new List<Card> { snc(6), snc(0) });
            board.Add(new List<Card> { nc(9), nc(1), nc(11), nc(11), snc(8), snc(10), snc(9), snc(0) });
            board.Add(new List<Card> { nc(11), nc(4), nc(12), nc(2), snc(1), snc(0), snc(10), snc(6), snc(5), snc(4), snc(3), snc(2), snc(1), snc(4), snc(3), snc(2), snc(9), snc(8), snc(7) });
            board.Add(new List<Card> { nc(5), snc(1), snc(4), snc(3) });
            board.Add(new List<Card> { });

            Assert.False(GameEngine.InvalidState(board));
            Game.DisplayDetails("Pre reverse apply: ", MoveR, board);

            board = MoveR.ReverseMove(board);
            Game.DisplayDetails("Reversed move: ", MoveR, board);

            Assert.False(GameEngine.InvalidState(board));
            board = PlayMove.ApplyMove(board);
            Assert.False(GameEngine.InvalidState(board));
        }

    }
}
