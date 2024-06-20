using ClassLibrary.Interfaces;
using HumidAirPropertiesLib;

namespace ClassLibrary.Modules.HumidAirPropertiesService
{
    public class HumidAirPropertiesService : IHumidAirPropertiesService
    {
        public double[] CalculateDryAirMolarDensity(double[] temperature, double[] pressure)
        {
            double[] result = new double[temperature.Length];
            for (int i = 0; i < temperature.Length; i++)
            {
                result[i] = DryAirPropertiesCalculator.CalculateDryAirMolarDensity(temperature[i], pressure: pressure[i]);
            }
            return result;
        }

        public double[] CalculateDryAirMolarEnthalpy(double[] temperature, double[] pressure)
        {
            double[] result = new double[temperature.Length];
            for (int i = 0; i < temperature.Length; i++)
            {
                result[i] = DryAirPropertiesCalculator.CalculateDryAirMolarEnthalpy(temperature[i], pressure: pressure[i]);
            }
            return result;
        }

        public double[] CalculateDryAirMolarInternalEnergy(double[] temperature, double[] pressure)
        {
            double[] result = new double[temperature.Length];
            for (int i = 0; i < temperature.Length; i++)
            {
                result[i] = DryAirPropertiesCalculator.CalculateDryAirMolarInternalEnergy(temperature[i], pressure: pressure[i]);
            }
            return result;
        }

        public double[] CalculateDryAirIsochoricSpecificHeat(double[] temperature, double[] pressure)
        {
            double[] result = new double[temperature.Length];
            for (int i = 0; i < temperature.Length; i++)
            {
                result[i] = DryAirPropertiesCalculator.CalculateDryAirIsochoricSpecificHeat(temperature[i], pressure: pressure[i]);
            }
            return result;
        }

        public double[] CalculateDryAirIsobaricSpecificHeat(double[] temperature, double[] pressure)
        {
            double[] result = new double[temperature.Length];
            for (int i = 0; i < temperature.Length; i++)
            {
                result[i] = DryAirPropertiesCalculator.CalculateDryAirIsobaricSpecificHeat(temperature[i], pressure: pressure[i]);
            }
            return result;
        }
    }
}
