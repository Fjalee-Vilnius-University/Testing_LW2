namespace TDD_examples
{
    public class Program
    {
        private static object _testOutputDir;

        public static object DirectoryTool { get; private set; }

        static void Main()
        {

        }

        static void SingleResponsability_CreateGarage()
        {
            BetterCar petrolCar = new BetterCar(new PetrolEngine());
            BetterCar electricCar = new BetterCar(new ElectricEngine());
            BetterCar jetCar = new BetterCar(new JetCar());
        }

    }
}
