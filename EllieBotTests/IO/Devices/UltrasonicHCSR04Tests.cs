using NUnit.Framework;

namespace Tests {

    [TestFixture()]
    public class UltrasonicHCSR04Tests {

        [TestCase(160729, 275.65023500000001d)]
        [TestCase(810875, 1390.650625d)]
        [TestCase(842022, 1444.06773d)]
        public void TestCalculateDistanceInCm(long tof, double cm) {
            double actual = UltrasonicHCSR04.CalculateDistanceInCm(tof);
            Assert.That(actual, Is.EqualTo(cm));
        }
    }
}
