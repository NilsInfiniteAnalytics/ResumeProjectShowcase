namespace ClassLibrary.Interfaces
{
    public interface IUnitsConverterService
    {
        public double[] ConvertArray(double[] values, string fromUnit, string toUnit);
    }
}