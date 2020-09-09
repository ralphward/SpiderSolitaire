using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SpiderSolitaire
{
    class Move
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

            for (int j = 0; j < Game.Count - 2; j++)
            {
                for (int i = Game[j].Count -1; i >= 0; i--)
                {
                    if (i != Game[j].Count - 1)
                        if (!Game[j][i].canPickup(Game[j][i + 1]))
                            break;

                    for (int l = 0; l < Game.Count - 2; l++)
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
            bool interimMove = true;
            if (srcDepth > 0 && !Game[src][srcDepth - 1].Shown)
            {
                interimMove = false;
            }
            // low quality move - move to an empty column from a stacked position
            if (srcDepth > 0 && Game[src][srcDepth].Stackable(Game[src][srcDepth - 1]))
                _weight = 0;
            // clears a spot - highest priority move - interim move because it reveals nothing new on the board
            if (srcDepth == 0 && Game[dest].Count > 0)
                _weight = 8;
            // Moving to an empty column - not an ideal move
            else if (Game[dest].Count == 0)
            {
                if (Game[src][srcDepth - 1].Shown)
                    _weight = 1;
                else
                    _weight = 2;
            }
            else if (Game[src][srcDepth - 1].Shown)
            {
                // already stacked - reveals nothing new
                if (Game[src][srcDepth].Suit == Game[src][srcDepth - 1].Suit)
                    _weight = 3;
                else
                    _weight = 4;
            }
            else
            {
                if (Game[src][srcDepth].Suit == Game[dest].Last().Suit)
                    _weight = 6;
                else
                    _weight = 5;
            }

            return Tuple.Create<int, bool>(_weight, interimMove);
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
                }
                return game;
            }

            numMoved = 0;
            for (int i = cardNumber; i < game[srcColumn].Count; i++)
            {
                game[destColumn].Add(game[srcColumn][i]);
                numMoved++;
            }

            while (numMoved > 0)
            {
                game[srcColumn].RemoveAt(game[srcColumn].Count - 1);
                numMoved--;
            }

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
            for (int i = game[destColumn].Count - numMoved - 1; i < game[destColumn].Count - 1; i++)
                game[srcColumn].Add(game[destColumn][i]);

            for (int i = 0; i < numMoved; i++)
                game[destColumn].RemoveAt(game[destColumn].Count - 1);

            return restoreSets(game);
        }

        internal IList<List<Card>> removeSets(IList<List<Card>> game, int column)
        {
            int stack = 0;
            while (game[column].Last().Value == stack && stack < 13)
                stack++;
            if (stack == 13)
            {
                suitRemoved = game[column].Last().Suit;
                game[column].RemoveRange(game[column].Count - 13, 13);
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
