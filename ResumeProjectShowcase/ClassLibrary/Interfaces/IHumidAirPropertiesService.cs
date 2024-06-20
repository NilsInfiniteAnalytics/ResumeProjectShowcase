namespace ClassLibrary.Interfaces;
public interface IHumidAirPropertiesService
{
    public double[] CalculateDryAirMolarDensity(double[] temperature, double[] pressure);

    public double[] CalculateDryAirMolarEnthalpy(double[] temperature, double[] pressure);

    public double[] CalculateDryAirMolarInternalEnergy(double[] temperature, double[] pressure);

    public double[] CalculateDryAirIsochoricSpecificHeat(double[] temperature, double[] pressure);

    public double[] CalculateDryAirIsobaricSpecificHeat(double[] temperature, double[] pressure);
}