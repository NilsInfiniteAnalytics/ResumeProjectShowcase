using ClassLibrary.Interfaces;

namespace ClassLibrary.Modules.UnitsConverterService
{
    public class UnitsConverterService : IUnitsConverterService
    {
        public double[] ConvertArray(double[] values, string fromUnit, string toUnit)
        {
            try
            {
                double[] convertedValues = UnitsConverter.Convert.ArrayFrom(values, fromUnit).ToSIMD(toUnit);
                return convertedValues;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
