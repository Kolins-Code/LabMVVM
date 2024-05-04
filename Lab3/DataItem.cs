namespace Lab3;

public struct DataItem
{
    
    
    public double x { get; set; }
    public double y1 { get; set; }
    public double y2 { get; set; }

    public DataItem(double x, double y1, double y2)
    {
        this.x = x;
        this.y1 = y1;
        this.y2 = y2;
    }

    public string ToLongString(string format)
    {
        return string.Format("({0:" + format + "}, {1:" + format + "}) - {2:" + format + "}", y1, y2, x);
    }

    public override string ToString()
    {
        return ToLongString(DataStat.FORMAT);
    }
}