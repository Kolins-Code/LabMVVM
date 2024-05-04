using System.Xml.Linq;

namespace Lab3
{
    public class V1MainCollection: System.Collections.ObjectModel.ObservableCollection<V1Data>
    {
        public double average
        {
            get
            {

                double query = (from data in this 
                                from element in data 
                                select Math.Sqrt(element.y1 * element.y1 + element.y2 * element.y2)).Average();
                return query;
            }
        }

        public DataItem? differenceElement
        {
            get
            {
               

                double max = (from data in this
                              from element in data
                              select Math.Abs(Math.Sqrt(element.y1 * element.y1 + element.y2 * element.y2) - average)).Max();

                DataItem? query = (from data in this
                                   from element in data
                                   where Math.Abs(Math.Sqrt(element.y1 * element.y1 + element.y2 * element.y2) - average) == max
                                   select element).First();

                return query;

            }
        }

        public IEnumerable<double> matches
        {
            get
            {
               

                IEnumerable<double> repeats = from first in this
                                              from second in this
                                              from firstElement in first
                                              from secondElement in second
                                              where first != second && firstElement.x == secondElement.x
                                              select firstElement.x;

                IEnumerable<double> sorted = from element in repeats
                                             orderby element
                                             select element;

                var groups = from element in sorted
                             group element by element;

                var query = from data in groups
                            select data.First();

                return query;
            }
        }

        public IEnumerable<double> match_two
        {
            get
            {
                IEnumerable<double> repeats = from first in this
                                              from second in this
                                              from firstElement in first
                                              from secondElement in second
                                              where first != second && firstElement.x == secondElement.x
                                              select firstElement.x;

                IEnumerable<double> sorted = from element in repeats
                                             orderby element
                                             select element;

                var groups = from element in sorted
                             group element by element;

                var query = from data in groups
                            where data.Count() == 2
                            select data.First();

                return query;
            }
        }

        public bool Contains(string key)
        {
            bool isPresented = false;
            foreach (V1Data element in this)
            {
                if (element.key == key)
                {
                    isPresented = true;
                    break;
                }
            }

            return isPresented;
        }
        
        public new bool Add(V1Data v1Data)
        {
            bool contains = Contains(v1Data.key);
            if (!contains)
            {
                base.Add(v1Data);
            }
            return !contains;
        }

        private void arrayGenerate(double x, ref double y1, ref double y2)
        {
            y1 = 1 / (x + 1);
            y2 = -y1;
        }

        private DataItem listGenerate(double x)
        {
            DataItem item = new DataItem(x, 1 / (x + 1), -1 / (x + 1));
            return item;
        }

        public V1MainCollection(int nV1DataArray, int nV1DataList)
        {
           
            for (int i = 0; i < nV1DataArray; i++)
            {
                Add(new V1DataArray("Array" + i.ToString(), DateTime.Now, 10, 0, 9, arrayGenerate));
            }

            double[] x = new double[nV1DataList];
            x[0] = -15;
            for (int i = 1; i <  x.Length; i++)
            {
                x[i] = x[i - 1] + 1;
            }

            for (int i = 0; i < nV1DataList; i++)
            {
                Add(new V1DataList("List" + i.ToString(), DateTime.Now, x, listGenerate));
            }
        }

        public string ToLongString(string format)
        {
            string result = "VMainCollection\n";

            foreach (V1Data element in this)
            {
                result += element.ToLongString(format);
            }

            return result;
        }

        public override string ToString()
        {
            string result = "VMainCollection\n";

            foreach (V1Data element in this)
            {
                result += element.ToString();
            }

            return result;
        }

        public V1MainCollection()
        { 
        }

       
    }
 
}
