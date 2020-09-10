using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SpiderSolitaire
{
    class Game
    {
        internal IList<List<Card>> GameCollection;
        internal IDictionary<string, bool> GameStates;
        internal List<Move> currentMoves;

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
            currentMoves = new List<Move>();

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
                offStack.Add(sortedCards[i]);
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

        internal bool Solve()
        {
            var possMoves = Move.FindMoves(GameCollection);
            foreach (var move in possMoves)
            {
                currentMoves.Add(move);
                GameCollection = move.ApplyMove(GameCollection);
                this.DisplayDetails("Applied: ", move, GameCollection);
                if (!this.DuplicateState(GameCollection))
                {
                    if (move.interimMove)
                        this.StoreState(GameCollection);
                    if (this.Solve())
                        return true;
                }

                if (isSolved)
                    return true;

                GameCollection = move.ReverseMove(GameCollection);
                currentMoves.RemoveAt(currentMoves.Count - 1);
            }
            return isSolved;
        }

        private void DisplayDetails(String action, Move move, IList<List<Card>> gameCollection)
        {
            Console.WriteLine(action + move.cardNumber + " from source " + move.srcColumn + " to " + move.destColumn);

            int max = 0;
            for (int i = 0; i < gameCollection.Count - 1; i++)
                max = gameCollection[i].Count > max ? gameCollection[i].Count : max;

            for (int j = 0; j < max; j++)
            {
                for (int i = 0; i < gameCollection.Count - 1; i++)
                {
                    if (gameCollection[i].Count <= j)
                        Console.Write("\t");
                    else if (gameCollection[i][j].Shown)
                        Console.Write("\t" + gameCollection[i][j].Value);
                    else if (!gameCollection[i][j].Shown)
                        Console.Write("\t-");
                }
                Console.WriteLine();
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
