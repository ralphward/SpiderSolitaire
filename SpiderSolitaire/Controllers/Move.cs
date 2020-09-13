using System;
using System.Collections.Generic;
using System.Linq;


namespace SpiderSolitaire
{
    public class Move
    {
        // roughly how good the move is
        internal int weight;

        // column a card is coming from, -1 for a deal
        internal int srcColumn;
        internal int destColumn;

        // The index of the card in the source column
        internal int cardNumber;

        // boolean showing if a new card was revealed on this move
        internal bool interimMove;

        // number of cards moved
        private int numMoved;

        // number representing the suit of the cards removed. -1 if no suit removed
        private int suitRemoved;

        public static IList<Move> FindMoves(IList<List<Card>> Game)
        {
            IList<Move> moveCollection = new List<Move>();
            if (Game.Last().Count > 0)
                moveCollection.Add(new Move() { weight = 0, srcColumn = -1, destColumn = -1, cardNumber = -1, interimMove = true} );

            for (int j = 0; j < Game.Count - 1; j++)
            {
                for (int i = Game[j].Count -1; i >= 0; i--)
                {
                    if (i != Game[j].Count - 1)
                        if (!Game[j][i].canPickup(Game[j][i + 1]))
                            break;

                    for (int l = 0; l < Game.Count - 1; l++)
                    {
                        if (Game[l].Count == 0 || Game[j][i].Stackable(Game[l].Last()))
                        {
                            Tuple<int, bool> responseWeight = Move.WeighMove(Game, j, l, i);
                            
                            moveCollection.Add(new Move() { 
                                weight = responseWeight.Item1, 
                                srcColumn = j, 
                                destColumn = l, 
                                cardNumber = i, 
                                interimMove = responseWeight.Item2
                            });
                        }
                    }
                }
            }

            return moveCollection.OrderByDescending(o => o.weight).ToList();
        }

        private static Tuple<int, bool> WeighMove(IList<List<Card>> Game, int src, int dest, int srcDepth )
        {
            int _weight = 0;
            if (srcDepth > 0 && !Game[src][srcDepth - 1].Shown)
            {
                //reveals something new - this is a high priority move
                return Tuple.Create<int, bool>(50, false);
            }
            // low quality move - move to an empty column from a stacked position
            if (srcDepth > 0 && Game[src][srcDepth].Stackable(Game[src][srcDepth - 1]))
                _weight = 1;
            // clears a spot - highest priority move
            else if (srcDepth == 0 && Game[dest].Count > 0)
                _weight = 45;
            // almost certainly pointless move - moving from and empty colum to an empty column
            else if (srcDepth == 0 && Game[dest].Count == 0)
                _weight = 0;
            else
                _weight = (Game[src].Count - srcDepth) + Game[dest].Count;

            return Tuple.Create<int, bool>(_weight, true);
        }

        public IList<List<Card>> ApplyMove(IList<List<Card>> game)
        {
            suitRemoved = -1;

            if (srcColumn == -1)
            {
                for (int i = 0; i < game.Count - 1; i++)
                {
                    game[i].Add(game.Last().Last());
                    game.Last().RemoveAt(game.Last().Count - 1);
                    game[i].Last().Shown = true;
                }
                return game;
            }

            numMoved = 0;
            for (int i = cardNumber; i < game[srcColumn].Count; i++)
            {
                game[destColumn].Add(game[srcColumn][i]);
                numMoved++;
            }

            for (int i = 0; i < numMoved; i++)
                game[srcColumn].RemoveAt(game[srcColumn].Count - 1);

            if (!this.interimMove)
                game[srcColumn].Last().Shown = true;

            return removeSets(game, destColumn);
        }

        internal IList<List<Card>> ReverseMove(IList<List<Card>> game)
        {
            game = restoreSets(game);
            if (srcColumn == -1)
            {
                for (int i = game.Count - 2; i >= 0; i--)
                {
                    game.Last().Add(game[i].Last());
                    game[i].RemoveAt(game[i].Count - 1);
                }
                return game;
            }
            for (int i = game[destColumn].Count - numMoved; i < game[destColumn].Count; i++)
                game[srcColumn].Add(game[destColumn][i]);

            for (int i = 0; i < numMoved; i++)
                game[destColumn].RemoveAt(game[destColumn].Count - 1);

            return game;
        }

        internal IList<List<Card>> removeSets(IList<List<Card>> game, int column)
        {
            bool stack = true;
            int depth = game[column].Count - 1;
            int _suit = game[column].Last().Suit;
            if (depth >= 12)
            {
                for (int i = 0; i < 13; i++)
                {
                    if (game[column][depth - i].Value == i && game[column][depth - i].Shown && game[column][depth - i].Suit == _suit)
                    {
                        stack = true;
                    }
                    else
                    {
                        stack = false;
                        break;
                    }
                }

                if (stack)
                {
                    suitRemoved = game[column].Last().Suit;
                    game[column].RemoveRange(game[column].Count - 13, 13);
                    if (game[column].Count > 0 )
                        game[column].Last().Shown = true;
                }
            }

            return game;
        }

        internal IList<List<Card>> restoreSets(IList<List<Card>> game)
        {
            if (suitRemoved > -1)
                for (int i = 0; i < 14; i++)
                    game[destColumn].Add(new Card() { Value = i, Shown = true, Suit = suitRemoved });
            return game;
        }

    }
}
