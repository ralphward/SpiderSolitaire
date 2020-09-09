using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SpiderSolitaire
{
    class Game
    {
        internal IList<List<Card>> GameCollection;
        internal IDictionary<string, bool> GameStates;

        internal bool isSolved
        {
            get
            {
                int numLeft = 0;
                foreach (var stack in GameCollection)
                    numLeft += stack.Count;
                return (numLeft == 0);

            }
        }

        public Game(IList<Card> sortedCards, IDictionary<string, bool> gameStates)
        {
            GameCollection = new List<List<Card>>();
            GameStates = gameStates;
            List<Card> offStack = new List<Card>();

            int[] stackNum = {6, 6, 6, 6, 5, 5, 5, 5, 5, 5};

            int cardNum = 0;
            for (int i = 0; i < stackNum.Length; i++)
            {
                List<Card> c = new List<Card>();

                for (int j = 0; j < stackNum[i]; j++)
                {
                    c.Add(sortedCards[cardNum]);
                    cardNum++;
                }
                GameCollection.Add(c);
            }
            for (int i = cardNum; i < sortedCards.Count; i++)
            {
                offStack.Add(sortedCards[cardNum]);
            }
            GameCollection.Add(offStack);
        }

        internal void Uncover()
        {
            foreach (var list in GameCollection)
            {
                list.Last().Shown = true;
            }
            GameCollection.Last().Last().Shown = false;
        }

        internal List<Move> Solve(List<Move> currentMoves)
        {
            Console.WriteLine(currentMoves.Count);
            var possMoves = Move.FindMoves(GameCollection);
            if (possMoves.Count == 0)
            {
                // no more legal moves - check if we've won here and return true if we have!
                return currentMoves;
            }
            else
            {
                foreach (var move in possMoves)
                {
                    currentMoves.Add(move);
                    GameCollection = move.ApplyMove(GameCollection);

                    if (!this.DuplicateState(GameCollection))
                    {
                        if (move.interimMove)
                            this.StoreState(GameCollection);
                        Solve(currentMoves);
                    }

                    if (isSolved)
                        return currentMoves;

                    GameCollection = move.ReverseMove(GameCollection);
                    currentMoves.RemoveAt(currentMoves.Count - 1);
                }
                
                return currentMoves;
            }

        }

        private void StoreState(IList<List<Card>> gameCollection)
        {
            string rawData = "";
            foreach (var stack in gameCollection)
            {
                foreach (var card in stack)
                {
                    rawData += card.Value.ToString() + card.Suit.ToString() + card.Shown.ToString();
                }

            }
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                GameStates.Add(builder.ToString(), true);
            }

        }

        private bool DuplicateState(IList<List<Card>> gameCollection)
        {
            if (GameStates.Count == 0)
            {
                return false;
            }

            string rawData = "";
            foreach (var stack in gameCollection)
            {
                foreach (var card in stack)
                {
                    rawData += card.Value.ToString() + card.Suit.ToString() + card.Shown.ToString();
                }

            }
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                if (GameStates.ContainsKey(builder.ToString()))
                    return true;
                else
                    return false;
            }
        }
    }
}
