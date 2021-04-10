using NUnit.Framework;

namespace Tests {

    [TestFixture()]
    public class UltrasonicHCSR04Tests {

        [TestCase(160729, 0.803645)]
        [TestCase(810875, 4.054375)]
        [TestCase(842022, 4.21011)]
        public void TestCalculateDistanceInCm(long tof, double cm) {
            double actual = UltrasonicHCSR04.CalculateDistanceInCm(tof, 1);
            Assert.That(actual, Is.EqualTo(cm));
        }
    }
}
