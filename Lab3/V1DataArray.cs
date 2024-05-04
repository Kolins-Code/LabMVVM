using System.Collections;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab3
{
    
    public class V1DataArray : V1Data
    {
        private int index = 0;

        public double[] grid { get; set; }

        public double[][] field { get; set; }

        public string ThisString
        {
            get
            {
                return ToLongString(DataStat.FORMAT);
            }
        }

        public V1DataArray(string key, DateTime date) : base(key, date)
        {
            grid = new double[0];
            field = new double[0][];
        }

        public V1DataArray(string key, DateTime date, double[] x, DataStat.FValues F) : base(key, date)
        {
            grid = new double[x.Length];
            x.CopyTo(grid, 0);
            field = new double[grid.Length][];

            for (int i = 0; i < grid.Length; i++)
            {
                double[] field_element = new double[2];
                F(grid[i], ref field_element[0], ref field_element[1]);
                field[i] = field_element;
            }

        }

        public V1DataArray(string key, DateTime date, double[] x, double[,] f) : base(key, date)
        {
            grid = new double[x.Length];
            x.CopyTo(grid, 0);
            field = new double[grid.Length][];

            for (int i = 0; i < grid.Length; i++)
            {
                double[] field_element = new double[2];
                field_element[0] = f[i, 0];
                field_element[1] = f[i, 1];
                field[i] = field_element;
            }

        }

        public V1DataArray(string key, DateTime date, int nX, double xL, double xR, DataStat.FValues F) : base(key, date)
        {
            double shift = (xR - xL) / (nX - 1);

            grid = new double[nX];
            field = new double[nX][];

            double current = xL;
            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = current;

                double[] field_element = new double[2];
                F(current, ref field_element[0], ref field_element[1]);
                field[i] = field_element;

                current += shift;
            }
        }

        public double[] this[int key]
        {

            get
            {
                double[] result = new double[grid.Length];

                for (int i = 0; result.Length > i; i++)
                {
                    result[i] = field[i][key];
                }

                return result;
            }
        }

        public V1DataList DataList
        {
            get
            {
                V1DataList list = new V1DataList(key, time, grid, recombine);
                index = 0;
                return list;
            }
        }

        private DataItem recombine(double x)
        {

            DataItem item = new DataItem(x, field[index][0], field[index][1]);
            index++;
            return item;
        }

        public override double MaxDistance
        {
            get
            {
                if (grid.Length == 0)
                {
                    return 0;
                }
                double max = grid[0];
                foreach (double point in grid)
                {
                    if (max < point)
                    {
                        max = point;
                    }
                }

                double min = grid[0];
                foreach (double point in grid)
                {
                    if (min > point)
                    {
                        min = point;
                    }
                }

                return max - min;
            }
        }

        public override string ToString()
        {
            return "\nV1DataArray: key - " + key + ", time - " + time + ", maxDistance - " + MaxDistance + ", length of grid - " + grid.Length;
        }

        public override string ToLongString(string format)
        {
            string result = ToString() + "\n grid:";

            for (int i = 0; i < grid.Length; i++)
            {
                result += string.Format("\n {0:" + format + "} [{1:" + format + "}, {2:" + format + "}]", grid[i], field[i][0], field[i][1]);
            }

            return result;
        }

        public override IEnumerator<DataItem> GetEnumerator()
        {
            return new Enumerator(grid, field);
        }

        private class Enumerator : IEnumerator<DataItem>
        {
            private List<DataItem> data { get; set; }
            private int position = -1;

            public Enumerator(double[] grid, double[][] field)
            {
                data = new List<DataItem>();
                

                for (int i = 0; i < grid.Length; i++)
                {
                    data.Add(new DataItem(grid[i], field[i][0], field[i][1]));
                }
            }
            public DataItem Current
            {
                get
                {
                    try
                    {
                        return data[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                position++;
                return position < data.Count();
            }

            public void Reset()
            {
                position = -1;
            }
        }

        public bool Save(string filename)
        {
            var status = false;
            
            FileStream file = null;
            try
            {
                file = new FileStream(filename, FileMode.Create, FileAccess.Write);
                BinaryWriter writer = new BinaryWriter(file);
                writer.Write(key);
                writer.Write(time.ToString());
                writer.Write(grid.Length);
                for (int i = 0; i < grid.Length; i++)
                {
                    writer.Write(grid[i]);
                    writer.Write(field[i][0]);
                    writer.Write(field[i][1]);
                }
                
                writer.Close();
                status = true;
            } 
            catch (Exception ex)
            {
                throw;
                Console.WriteLine(ex.Message);
            } 
            finally
            {
                if (file != null)
                    file.Close();
            }

            return status;
        }



        public static bool Load(string filename, ref V1DataArray array)
        {
            var status = false;
            FileStream file = null;

            try
            {
                file = new FileStream(filename, FileMode.Open);
                BinaryReader reader = new BinaryReader(file);

                string key = reader.ReadString();
                DateTime time = DateTime.Parse(reader.ReadString());

                int length = reader.ReadInt32();

                double[] x = new double[length];
                double[,] f = new double[length, 2];
                

                for (int i = 0; i < length; i++) 
                {
                    x[i] = reader.ReadDouble();
                    f[i, 0] = reader.ReadDouble();
                    f[i, 1] = reader.ReadDouble();
                }
                reader.Close();

                array = new V1DataArray(key, time, x, f);
                status = true;
            }
            catch (Exception ex)
            {
                throw;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }



            return status;
        }
    }
}
