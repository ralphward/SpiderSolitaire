using System;
using System.Diagnostics.CodeAnalysis;

namespace SpiderSolitaire
{
    public class Card : IEquatable<Card>
    {
        public int Value;

        public int Suit;

        public bool Shown;

        public bool Equals([AllowNull] Card other)
        {
            return (other.Suit == this.Suit && other.Value == this.Value);
        }

        public bool Stackable(Card other)
        {
            return (this.Value + 1 == other.Value && this.Shown && other.Shown);
        }

        public bool canPickup(Card child)
        {
            if (this.Shown && this.Value - 1 == child.Value && this.Suit == child.Suit)
            {
                return true;
            }
            return false;

        }
    }
}
