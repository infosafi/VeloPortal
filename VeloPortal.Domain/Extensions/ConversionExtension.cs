using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Reflection;

namespace VeloPortal.Domain.Extensions
{
    public static class ConversionExtension
    {
        public static List<T> DataTableToList<T>(this DataTable dataTable) where T : new()
        {
            var dataList = new List<T>();

            //Define what attributes to be read from the class
            const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

            //Read Attribute Names and Types
            var objFieldNames = typeof(T).GetProperties(flags).Cast<System.Reflection.PropertyInfo>().
                Select(item => new
                {
                    Name = item.Name,
                    Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
                }).ToList();

            //Read Datatable column names and types
            var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
                Select(item => new
                {
                    Name = item.ColumnName,
                    Type = item.DataType
                }).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var classObj = new T();

                foreach (var dtField in dtlFieldNames)
                {
                    System.Reflection.PropertyInfo propertyInfos = classObj.GetType().GetProperty(dtField.Name);

                    var field = objFieldNames.Find(x => x.Name == dtField.Name);

                    if (field != null)
                    {

                        if (propertyInfos.PropertyType == typeof(DateTime))
                        {
                            propertyInfos.SetValue
                            (classObj, convertToDateTime(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(Nullable<DateTime>))
                        {
                            propertyInfos.SetValue
                            (classObj, convertToDateTime(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(int))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToInt(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(long))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToLong(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(decimal))
                        {
                            propertyInfos.SetValue
                            (classObj, ConvertToDecimal(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(String))
                        {
                            if (dataRow[dtField.Name].GetType() == typeof(DateTime))
                            {
                                propertyInfos.SetValue
                                (classObj, ConvertToDateString(dataRow[dtField.Name]), null);
                            }
                            else
                            {
                                propertyInfos.SetValue
                                (classObj, ConvertToString(dataRow[dtField.Name]), null);
                            }
                        }
                        else
                        {
                            propertyInfos.SetValue
                                (classObj, Convert.ChangeType(dataRow[dtField.Name], propertyInfos.PropertyType), null);

                        }
                    }
                }
                dataList.Add(classObj);
            }
            return dataList;
        }

        //public static IEnumerable<T> DataTableToIEnurable<T>(this DataTable dataTable) where T : new()
        //{
        //    var dataList = new List<T>();

        //    //Define what attributes to be read from the class
        //    const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

        //    //Read Attribute Names and Types
        //    var objFieldNames = typeof(T).GetProperties(flags).Cast<System.Reflection.PropertyInfo>().
        //        Select(item => new
        //        {
        //            Name = item.Name,
        //            Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
        //        }).ToList();

        //    //Read Datatable column names and types
        //    var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
        //        Select(item => new
        //        {
        //            Name = item.ColumnName,
        //            Type = item.DataType
        //        }).ToList();

        //    foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
        //    {
        //        var classObj = new T();

        //        foreach (var dtField in dtlFieldNames)
        //        {
        //            System.Reflection.PropertyInfo propertyInfos = classObj.GetType().GetProperty(dtField.Name);

        //            var field = objFieldNames.Find(x => x.Name == dtField.Name);

        //            if (field != null)
        //            {

        //                if (propertyInfos.PropertyType == typeof(DateTime))
        //                {
        //                    propertyInfos.SetValue
        //                    (classObj, convertToDateTime(dataRow[dtField.Name]), null);
        //                }
        //                else if (propertyInfos.PropertyType == typeof(Nullable<DateTime>))
        //                {
        //                    propertyInfos.SetValue
        //                    (classObj, convertToDateTime(dataRow[dtField.Name]), null);
        //                }
        //                else if (propertyInfos.PropertyType == typeof(int))
        //                {
        //                    propertyInfos.SetValue
        //                    (classObj, ConvertToInt(dataRow[dtField.Name]), null);
        //                }
        //                else if (propertyInfos.PropertyType == typeof(long))
        //                {
        //                    propertyInfos.SetValue
        //                    (classObj, ConvertToLong(dataRow[dtField.Name]), null);
        //                }
        //                else if (propertyInfos.PropertyType == typeof(decimal))
        //                {
        //                    propertyInfos.SetValue
        //                    (classObj, ConvertToDecimal(dataRow[dtField.Name]), null);
        //                }
        //                else if (propertyInfos.PropertyType == typeof(String))
        //                {
        //                    if (dataRow[dtField.Name].GetType() == typeof(DateTime))
        //                    {
        //                        propertyInfos.SetValue
        //                        (classObj, ConvertToDateString(dataRow[dtField.Name]), null);
        //                    }
        //                    else
        //                    {
        //                        propertyInfos.SetValue
        //                        (classObj, ConvertToString(dataRow[dtField.Name]), null);
        //                    }
        //                }
        //                else
        //                {
        //                    propertyInfos.SetValue
        //                        (classObj, Convert.ChangeType(dataRow[dtField.Name], propertyInfos.PropertyType), null);

        //                }
        //            }
        //        }
        //        dataList.Add(classObj);
        //    }
        //    return dataList;
        //}
        public static List<dynamic> DataTableToDynamicList(this DataTable dataTable)
        {


            var dynamicDt = new List<dynamic>();
            if (dataTable == null)
            {
                return dynamicDt;
            }
            if (dataTable.Rows.Count == 0)
            {
                return dynamicDt;
            }
            foreach (DataRow row in dataTable.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dataTable.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
            }
            return dynamicDt;


        }

        public static DataTable ListToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        public static DataTable DynamicToToDataTable(IEnumerable<dynamic> dynamicList)
        {
            DataTable table = new DataTable();

            if (dynamicList == null)
                return table;

            foreach (var item in dynamicList)
            {
                // Convert dynamic to dictionary
                var expandoDict = (IDictionary<string, object>)item;

                // Initialize columns based on the first item
                if (table.Columns.Count == 0)
                {
                    foreach (var key in expandoDict.Keys)
                    {
                        table.Columns.Add(key);
                    }
                }

                // Create a new row
                var row = table.NewRow();
                foreach (var key in expandoDict.Keys)
                {
                    row[key] = expandoDict[key] ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }
        public static DataTable? GetDataTableFromObjects(object[] objects)
        {

            if (objects != null && objects.Length > 0)
            {

                Type t = objects[0].GetType();

                DataTable dt = new DataTable(t.Name);

                foreach (PropertyInfo pi in t.GetProperties())
                {

                    dt.Columns.Add(new DataColumn(pi.Name));

                }

                foreach (var o in objects)
                {

                    DataRow dr = dt.NewRow();

                    foreach (DataColumn dc in dt.Columns)
                    {

                        dr[dc.ColumnName] = o.GetType().GetProperty(dc.ColumnName).GetValue(o, null);

                    }

                    dt.Rows.Add(dr);

                }

                return dt;

            }

            return null;

        }
        public static DataTable? DynamicToDataTable(dynamic objects)
        {


            if (objects != null && objects.Count > 0)
            {

                Type t = objects[0].GetType();

                DataTable dt = new DataTable(t.Name);

                foreach (PropertyInfo pi in t.GetProperties())
                {

                    dt.Columns.Add(new DataColumn(pi.Name));

                }

                foreach (var o in objects)
                {

                    DataRow dr = dt.NewRow();

                    foreach (DataColumn dc in dt.Columns)
                    {

                        dr[dc.ColumnName] = o.GetType().GetProperty(dc.ColumnName).GetValue(o, null);

                    }

                    dt.Rows.Add(dr);

                }

                return dt;

            }

            return null;
        }

        public static List<T> DynamicToClassListConverter<T>(IEnumerable<dynamic> dynamicList) where T : new()
        {
            var resultList = new List<T>();

            foreach (var dynamicObj in dynamicList)
            {
                var obj = new T();
                var objProperties = obj.GetType().GetProperties();

                if (dynamicObj is ExpandoObject)
                {
                    var dynamicProperties = (IDictionary<string, object>)dynamicObj;

                    foreach (var property in objProperties)
                    {
                        if (dynamicProperties.ContainsKey(property.Name))
                        {
                            // Set the property value
                            property.SetValue(obj, Convert.ChangeType(dynamicProperties[property.Name], property.PropertyType));
                        }
                    }
                }

                resultList.Add(obj);
            }

            return resultList;
        }

        private static string? ConvertToDateString(object date)
        {
            if (date == null)
                return string.Empty;

            return date == null ? string.Empty : Convert.ToDateTime(date).ConvertDate();
        }

        private static string? ConvertToString(object value)
        {
            return Convert.ToString(ReturnEmptyIfNull(value));
        }

        private static int ConvertToInt(object value)
        {
            return Convert.ToInt32(ReturnZeroIfNull(value));
        }

        private static long ConvertToLong(object value)
        {
            return Convert.ToInt64(ReturnZeroIfNull(value));
        }

        private static decimal ConvertToDecimal(object value)
        {
            return Convert.ToDecimal(ReturnZeroIfNull(value));
        }

        private static DateTime convertToDateTime(object date)
        {
            return Convert.ToDateTime(ReturnDateTimeMinIfNull(date));
        }

        public static string? ConvertDate(this DateTime datetTime, bool excludeHoursAndMinutes = false)
        {
            if (datetTime != DateTime.MinValue)
            {
                if (excludeHoursAndMinutes)
                    return datetTime.ToString("yyyy-MM-dd");
                return datetTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            return null;
        }
        public static object ReturnEmptyIfNull(this object value)
        {
            if (value == DBNull.Value)
                return string.Empty;
            if (value == null)
                return string.Empty;
            return value;
        }
        public static object ReturnZeroIfNull(this object value)
        {
            if (value == DBNull.Value)
                return 0;
            if (value == null)
                return 0;
            return value;
        }
        public static object ReturnDateTimeMinIfNull(this object value)
        {
            if (value == DBNull.Value)
                return DateTime.MinValue;
            if (value == null)
                return DateTime.MinValue;
            return value;
        }

    }
}
