namespace TDD_examples
{
    public class FakeEngine : IEngine
    {
        private bool _isStarted;

        public void Start()
        {
            _isStarted = true;
        }

        public bool IsStarted()
        {
            return _isStarted;
        }
    }
}