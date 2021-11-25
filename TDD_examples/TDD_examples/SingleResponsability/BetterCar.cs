namespace TDD_examples
{
    public class BetterCar : ICar
    {
        private IEngine _engine;

        public BetterCar(IEngine engine)
        {
            _engine = engine;
        }

        public void Start()
        {
            SwitchToStartGear();
            BreaksOn();
            _engine.Start();
        }

        private void BreaksOn() { }
        public void SwitchToStartGear() { }
    }
}