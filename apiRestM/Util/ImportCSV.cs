using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace apiRestM.Util
{
    public class ImportCSV
    {
        public static List<T> GetList<T>(byte[] aByteArray) where T : new()
        {
            List<T> result = new List<T>();
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            Dictionary<int, PropertyInfo> propertyOrder = new Dictionary<int, PropertyInfo>();

            StreamReader streamReader = openCSV(aByteArray);

            //The first line must have the object properties
            List<string> firstLine = streamReader.ReadLine().Split(',').ToList();

            foreach (string property in firstLine)
            {
                int index = properties.FindIndex(x => x.Name.Equals(property));
                if (index >= 0)
                    propertyOrder.Add(index, properties[index]);
                else
                    throw new Exception(String.Format("The column {0} was not found in the object {1}", property, typeof(T)));
            }


            //Cicle until there are no lines left on the file
            while (streamReader.Peek() > 0)
            {
                T proxy = new T();
                string[] currentLine = streamReader.ReadLine().Split(',');
                int i = 0;
                foreach (int key in propertyOrder.Keys)
                {
                    convertValue(propertyOrder[key], currentLine[i++], ref proxy);
                }

                result.Add(proxy);
            }

            return result;
        }

        public static DataTable GetDataTable(byte[] aByteArray)
        {
            DataTable result = new DataTable();
            StreamReader streamReader = openCSV(aByteArray);

            //The first line must have the column names
            List<string> columns = streamReader.ReadLine().Split(',').ToList();
            foreach (string column in columns)
            {
                if (!string.IsNullOrEmpty(column))
                    result.Columns.Add(column);
            }

            //Cicle until there is no lines left on the file
            while (streamReader.Peek() > 0)
            {
                DataRow row = result.NewRow();
                string lineHolder = streamReader.ReadLine();
                if (string.IsNullOrEmpty(lineHolder.Replace(",", string.Empty)))
                    continue;
                string[] currentLine = lineHolder.Split(',');
                for (int i = 0; (i < currentLine.Length && i < row.ItemArray.Length); i++)
                {
                    row[columns[i]] = currentLine[i];
                }
                result.Rows.Add(row);
            }

            return result;
        }

        private static StreamReader openCSV(byte[] aByteArray)
        {
            MemoryStream memoryStream = new MemoryStream(aByteArray);
            return new StreamReader(memoryStream);
        }

        private static void convertValue<T>(PropertyInfo aPropertyType, string aValue, ref T aObject) where T : new()
        {
            if (String.IsNullOrEmpty(aValue.Trim()))
            {
                aPropertyType.SetValue(aObject, null);
                return;
            }

            #region Nullable type
            Type type = aPropertyType.PropertyType;
            if (type.Name == "Nullable`1")
                type = type.GetGenericArguments()[0];
            #endregion

            //Create a conversion based on the property type
            switch (type.Name)
            {
                case "DateTime":
                    DateTime date = Convert.ToDateTime(aValue);
                    aPropertyType.SetValue(aObject, date);
                    break;
                case "String":
                case "string":
                    aPropertyType.SetValue(aObject, aValue);
                    break;
                case "Decimal":
                case "decimal":
                    aPropertyType.SetValue(aObject, Convert.ToDecimal(aValue));
                    break;
                case "Double":
                case "double":
                case "Float":
                case "float":
                    aPropertyType.SetValue(aObject, Convert.ToDouble(aValue));
                    break;
                case "Int32":
                case "int":
                    aPropertyType.SetValue(aObject, Convert.ToInt32(aValue));
                    break;

                default:
                    throw new Exception(String.Format("The type {0} is not yet supported", aPropertyType.PropertyType.Name));
            }
        }
    }
}
