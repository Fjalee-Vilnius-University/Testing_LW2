namespace TDD_examples
{
    public class Car
    {
        private Engine _engine = new Engine();

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