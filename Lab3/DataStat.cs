namespace Lab3;

public static class DataStat
{
    public const string FORMAT = "f4";
    
    public delegate DataItem FDI(double x);

    public delegate void FValues(double x, ref double y1, ref double y2);
}