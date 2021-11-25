using NUnit.Framework;
using TDD_examples;

namespace UnitTests
{
    public class CarUnitTests
    {
        [Test]
        public void Start_startsEngine_correctly()
        {
            var car = new Car();
            car.Start();
            // Can't assert anything here
        }
    }
}
