using SpiderSolitaire;
using Xunit;

namespace SolitaireTestSuite
{
    public class TestCard
    {
        [Fact]
        public void TestCardEquality()
        {
            Card c = new Card() { Value = 0, Suit = 1, Shown = false };
            Card c1 = new Card() { Value = 0, Suit = 1, Shown = false };

            Assert.Equal(c, c1);

        }

        [Fact]
        public void TestStack()
        {
            Card c = new Card() { Value = 0, Suit = 1, Shown = true };
            Card c1 = new Card() { Value = 1, Suit = 1, Shown = true };

            Assert.True(c.Stackable(c1));
            Assert.False(c1.Stackable(c));

            c = new Card() { Value = 0, Suit = 2, Shown = true };
            c1 = new Card() { Value = 1, Suit = 1, Shown = true };

            Assert.True(c.Stackable(c1));
            Assert.False(c1.Stackable(c));
        }
        
        [Fact]
        public void testPickup()
        {
            Card c = new Card() { Value = 0, Suit = 2, Shown = true };
            Card c1 = new Card() { Value = 1, Suit = 1, Shown = true };

            Assert.False(c1.canPickup(c));

            c = new Card() { Value = 0, Suit = 2, Shown = true };
            c1 = new Card() { Value = 1, Suit = 2, Shown = true };

            Assert.True(c1.canPickup(c));
        }
    }
}
