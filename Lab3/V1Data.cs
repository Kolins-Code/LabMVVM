using System.Collections;
using System.Xml;

namespace Lab3;


public abstract class V1Data : IEnumerable<DataItem>
{
    public string key { get; set; }
    public DateTime time { get; set; }
    public abstract double MaxDistance { get; }

    public V1Data(string key, DateTime time)
    {
        this.key = key;
        this.time = time;
    }

    public abstract string ToLongString(string format);

    public override abstract string ToString();

    public abstract IEnumerator<DataItem> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}