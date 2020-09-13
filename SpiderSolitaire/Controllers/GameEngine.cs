using System;
using System.Collections.Generic;

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
            gm = new Game(cards, new Dictionary<string,bool>());
            gm.Uncover();
            if (gm.Solve())
            {
                Console.WriteLine("Solved!");
                
            }
            else
                Console.WriteLine("Couldn't solve it!");

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
    }

}
