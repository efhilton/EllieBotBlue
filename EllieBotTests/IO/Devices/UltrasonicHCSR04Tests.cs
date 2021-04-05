using NUnit.Framework;

namespace Tests {

    [TestFixture()]
    public class UltrasonicHCSR04Tests {

        [TestCase(160729, 20)]
        [TestCase(810875, 10)]
        [TestCase(842022, 10)]
        public void TestCalculateDistanceInCm(long tof, double cm) {
            double actual = UltrasonicHCSR04.CalculateDistanceInCm(tof);
            Assert.That(actual, Is.EqualTo(cm));
        }
    }
}
