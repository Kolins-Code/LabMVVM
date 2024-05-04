namespace Lab3;     

public class V1DataList : V1Data
{
    private int index = 0;

    public List<DataItem> data { get; set; }

    public string ThisString 
    {
        get
        {
            return ToLongString(DataStat.FORMAT);
        }
    }

    public V1DataList(string key, DateTime date) : base(key, date)
    {
        data = new List<DataItem>();        
    }

    public V1DataList(string key, DateTime date, double[] x, DataStat.FDI F) : base(key, date)
    {
        data = new List<DataItem>();

        List<double> xAdded = new List<double>();

        for (int i = 0; i < x.Length; i++)
        {
            if (!xAdded.Contains(x[i])) {
                data.Add(F(x[i]));
                xAdded.Add(x[i]);
            }
        }
    }

    public override double MaxDistance
    {
        get
        {
            if (data.Count == 0)
            {
                return 0;
            }
            double max = data[0].x;
            foreach (DataItem point in data)
            {
                if (max < point.x)
                {
                    max = point.x;
                }
            }

            double min = data[0].x;
            foreach (DataItem point in data)
            {
                if (min > point.x)
                {
                    min = point.x;
                }
            }

            return max - min;
        }
    }

    public static explicit operator V1DataArray(V1DataList source)
    {
        double[] x = new double[source.data.Count];
        for(int i = 0; i < source.data.Count; i++)
        {
            x[i] = source.data[i].x;
        }

        V1DataArray array = new V1DataArray(source.key, source.time, x, source.recombine);
        source.index = 0;
        return array;
    }

    private void recombine(double x, ref double y1, ref double y2)
    {
        y1 = data[index].y1;
        y2 = data[index].y2;
        index++;
    }

    public override string ToString()
    {
        return "\nV1DataList: key - " + key + ", time - " + time + ", maxDistance - " + MaxDistance + ", length of data - " + data.Count;
    }

    public override string ToLongString(string format)
    {
        string result = ToString() + "\n data:";

        foreach (DataItem point in data)
        {
            result += string.Format("\n {0:" + format + "} [{1:" + format + "}, {2:" + format + "}]", point.x, point.y1, point.y2);
        }

        return result;
    }

    public override IEnumerator<DataItem> GetEnumerator()
    {
        return data.GetEnumerator();
    }

    
}