using NUnit.Framework;

namespace Ascii.Tests
{
    [TestFixture]
    public class AngleTests
    {
        [Test]
        [TestCase(0.0,'V')]
        [TestCase(1,'V')]
        [TestCase(2,'V')]
        [TestCase(3,'V')]
        [TestCase(4,'V')]
        [TestCase(5,'V')]
        [TestCase(6,'V')]
        public void TestAngle(double angle, char expectedChar)
        {
            Assert.AreEqual(expectedChar,Map.CalculateArrow(angle));
        }

        [TestCase(3, 'V')]
        public void TestAngle2(double angle, char expectedChar)
        {
            Assert.AreEqual(expectedChar,Map.CalculateArrow(angle));
        }




    }
}
