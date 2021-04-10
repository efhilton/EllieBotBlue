using NUnit.Framework;

namespace Tests {

    [TestFixture()]
    public class UltrasonicHCSR04Tests {

        [TestCase(160729, 16072900)]
        [TestCase(810875, 81087500)]
        [TestCase(842022, 84202200)]
        public void TestCalculateDistanceInCm(long tof, double cm) {
            double actual = UltrasonicHCSR04.CalculateDistanceInCm(tof, 2.0, 1);
            Assert.That(actual, Is.EqualTo(cm));
        }
    }
}
