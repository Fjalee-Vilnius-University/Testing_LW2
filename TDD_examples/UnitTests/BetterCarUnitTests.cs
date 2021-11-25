using NUnit.Framework;
using TDD_examples;

namespace UnitTests
{
    public class BetterCarUnitTests
    {
        [Test]
        public void Start_startsEngine_correctly()
        {
            var fakeEngine = new FakeEngine();
            var car = new BetterCar(fakeEngine);

            car.Start();

            Assert.IsTrue(fakeEngine.IsStarted());
        }
    }
}
