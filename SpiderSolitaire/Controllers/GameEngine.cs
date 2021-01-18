using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace SpiderSolitaire
{
    public class GameEngine
    {
        private const int NUM_DECKS = 8;
        private const int NUM_SUITS = 1;

        IList<Card> cards;
        private Game gm;

        public GameEngine()
        {
            cards = new List<Card>();
        }

        public void init()
        {
            GenerateCards();
            cards.Shuffle();
            gm = new Game(cards, new Dictionary<string, bool>());
            gm.Uncover();
            if (gm.Solve())
                Console.WriteLine("Solved in " + gm.currentMoves.Count);
            else
                Console.WriteLine("Unable to solve without interim moves!");

        }

        private void GenerateCards()
        {
            for (int j = 0; j < NUM_DECKS; j++)
            {
                for (int k = 0; k < NUM_SUITS; k++)
                {
                    for (int i = 0; i < 13; i++)
                    {
                        Card c = new Card() { Value = i, Suit = k, Shown = false };
                        cards.Add(c);
                    }
                }
            }
        }

        public static bool InvalidState(IList<List<Card>> gameCollection)
        {
            int total = 0;
            int card_total = 0;
            foreach (var lst in gameCollection) { 
                foreach (var card in lst) {
                    total += card.Value;
                    card_total++;
                }
            }
            if (total % 78 != 0)
                return true;
            if (card_total % 13 != 0)
                return true;
            return false;
        }
    }

}
