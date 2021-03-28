using NUnit.Framework;

namespace EllieBot.IO.Tests {

    [TestFixture()]
    public class PwmControllerTest {

        [TestCase(1.0, 100)]
        [TestCase(0.05, 0)]
        [TestCase(0.06, 10)]
        [TestCase(0.1, 10)]
        [TestCase(0.12, 10)]
        [TestCase(0.14, 10)]
        [TestCase(0.04, 0)]
        [TestCase(0.01, 0)]
        [TestCase(-1.0, -100)]
        [TestCase(-0.05, 0)]
        [TestCase(-0.1, -10)]
        [TestCase(-0.12, -10)]
        [TestCase(-0.14, -10)]
        [TestCase(-0.04, 0)]
        [TestCase(-0.01, 0)]
        public void TestScaleMotorInput(double input, int expected) {
            Assert.That(PwmController.ScaleMotorInput(input), Is.EqualTo(expected));
        }
    }
}
