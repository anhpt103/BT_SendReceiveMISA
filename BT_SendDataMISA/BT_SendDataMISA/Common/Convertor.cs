using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace BT_SendDataMISA.Common
{
    static public class Convertor
    {
        static public string ObjectToJson(object obj, out string value)
        {
            value = "";
            try
            {
                value = JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }
        static public string JsonToObject<T>(object data, out T obj) where T : class
        {
            obj = null;

            try
            {
                obj = JsonConvert.DeserializeObject<T>(Convert.ToString(data));
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }
        static public string JsonToObject(object data, Type type, out object obj)
        {
            obj = null;

            try
            {
                obj = JsonConvert.DeserializeObject(Convert.ToString(data), type);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }
        public static string ObjectToXML(Object obj, out string xml)
        {
            return ObjectToXML(obj, "", "", out xml);
        }
        public static string ObjectToXML(Object obj, string prefixXML, string namespaceXML, out string xml)
        {
            return ObjectToXML(obj, new List<string> { prefixXML }, new List<string> { namespaceXML }, out xml);
        }
        public static string ObjectToXML(Object obj, List<string> prefixXML, List<string> namespaceXML, out string xml)
        {
            string msg = "";
            xml = "";
            try
            {
                using (var sw = new Utf8StringWriter())
                {
                    using (var xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true }))
                    {
                        xw.WriteStartDocument(true); // that bool parameter is called "standalone"
                        var namespaces = new XmlSerializerNamespaces();
                        for (int i = 0; i < prefixXML.Count; i++)
                            namespaces.Add(prefixXML[i], namespaceXML[i]);

                        var xmlSerializer = new XmlSerializer(obj.GetType());
                        xmlSerializer.Serialize(xw, obj, namespaces);

                        xml = sw.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }
        public static string XMLToObject<T>(String xml, out T obj)
        {
            string msg = "";
            obj = default(T);

            try
            {
                using (TextReader reader = new StringReader(xml))
                {
                    obj = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        public static string ObjectToString<T>(T objectData, out string value) where T : class
        {
            return ObjectToString<T>(false, objectData, out value);
        }
        /// <summary>
        /// Convert object to string. String trong định dạng XML hoặc Json
        /// </summary>
        /// <param name="isXML">true: string trả về trong định dạng XML; false: string trả về trong định dạng Json</param>
        /// <param name="objectData"></param>
        /// <param name="value"></param>
        /// <returns>string khác rỗng là lỗi</returns>
        public static string ObjectToString<T>(bool isXML, T objectData, out string value) where T : class
        {
            string msg = "";

            value = null;
            if (isXML) msg = ObjectToXML(objectData, out value);
            else msg = ObjectToJson(objectData, out value);

            return msg;
        }
        public static string StringToObject<T>(string stringData, out T obj) where T : class
        {
            return StringToObject<T>(false, stringData, out obj);
        }
        /// <summary>
        /// Convert string to object. String trong định dạng XML hoặc Json
        /// </summary>
        /// <param name="isXML">true: string trong định dạng XML; false: string trong định dạng Json</param>
        /// <param name="stringData"></param>
        /// <param name="obj"></param>
        /// <returns>string khác rỗng là lỗi</returns>
        public static string StringToObject<T>(bool isXML, string stringData, out T obj) where T : class
        {
            string msg = "";

            obj = null;
            if (isXML) msg = XMLToObject(stringData, out obj);
            else msg = JsonToObject(stringData, out obj);

            return msg;
        }

        /// <summary>
        /// Serialize và chuyển sang Base64 1 object bất kỳ (object cần có thuộc tính Serializable)
        /// </summary>
        /// <param name="obj">object cần chuyển sang Base64</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ObjectToBase64String(object obj, out string value)
        {
            string msg = "";
            value = null;

            if (obj == null) return "Error: Object is null @ObjectToBase64String";

            try
            {
                value = ObjectToBase64String(obj);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }
        public static string ObjectToBase64String(object obj)
        {
            if (obj is byte[]) return Convert.ToBase64String((byte[])obj);

            return Convert.ToBase64String(ObjectToBytes(obj));
        }
        public static byte[] ObjectToBytes(object obj)
        {
            if (obj == null) return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserialize từ Base64 về object 
        /// </summary>
        /// <param name="base64">Chuỗi cần chuyển về object</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Base64StringToObject(string base64, out object value)
        {
            string msg = "";
            value = null;

            try
            {
                value = Base64StringToObject(base64);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }
        public static object Base64StringToObject(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            return BytesToObject(bytes);
        }
        public static object BytesToObject(byte[] bytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream memStream = new MemoryStream())
            {
                memStream.Write(bytes, 0, bytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                object obj = bf.Deserialize(memStream);
                return obj;
            }
        }

        public static string Base64StringToBytes(string base64, out byte[] value)
        {
            string msg = "";
            value = null;

            try
            {
                value = Convert.FromBase64String(base64);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }
        public static string Base64StringToFile(string base64Content, string filePath)
        {
            string msg = "";
            try
            {
                byte[] content = Convert.FromBase64String(base64Content);
                File.WriteAllBytes(filePath, content);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }

        public static string GetBytes(string data, out byte[] bytes)
        {
            string msg = "";
            bytes = null;

            try
            {
                bytes = Encoding.UTF8.GetBytes(data);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }
        public static string GetString(byte[] bytes, out string data)
        {
            string msg = "";
            data = null;

            try
            {
                data = Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return msg;
        }

        static public string StringToMD5(string s, out string value)
        {
            string msg = "";
            value = null;
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                msg = BytesToMD5(bytes, out value);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }
        static public string BytesToMD5(byte[] bytes, out string value)
        {
            string msg = "";
            value = null;
            try
            {
                MD5 md5 = MD5.Create();
                byte[] hash = md5.ComputeHash(bytes);

                // step 2, convert byte array to hex string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                value = sb.ToString();
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }

        static public string StringToGuid(string guid, out Guid value)
        {
            return ObjectToGuid(guid, out value);
        }
        static public string ObjectToGuid(object guid, out Guid value)
        {
            value = Guid.Empty;
            try
            {
                value = new Guid(guid.ToString());
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }
        static public Guid ToGuid(this object obj, Guid defaultValue)
        {
            try
            {
                return Guid.Parse(obj.ToString());
            }
            catch
            {
                return defaultValue;
            }
        }

        static public string StringToNumber(string s, out long value)
        {
            value = 0;
            try
            {
                value = long.Parse(s);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }

        static public string StringToNumber(string s, out decimal value)
        {
            value = 0;
            try
            {
                value = decimal.Parse(s);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }

        static public long ToLong(this object obj, long defaultvalue)
        {
            try
            {
                return Convert.ToInt64(obj);
            }
            catch
            {
                return defaultvalue;
            }
        }

        static public string StringToDouble(string s, out double value)
        {
            value = 0;
            try
            {
                value = double.Parse(s);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }
        static public double ToDouble(this object obj, double defaultvalue)
        {
            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return defaultvalue;
            }
        }

        static public string StringToNumber(string s, out int value)
        {
            value = 0;
            try
            {
                value = int.Parse(s);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }
        static public int ToNumber(this object obj, int defaultvalue)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return defaultvalue;
            }
        }

        static public string StringToDatetime(string s, out double value)
        {
            value = 0;
            try
            {
                value = double.Parse(s);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }
        static public string StringToDatetime(string s, out DateTime value)
        {
            return StringToDatetime(s, null, out value);
        }

        static public string StringToDatetime(string s, string cultureInfo, out DateTime value)
        {
            value = DateTime.MinValue;
            try
            {
                if (cultureInfo != null) value = DateTime.Parse(s, new CultureInfo(cultureInfo, true));
                else value = DateTime.Parse(s);

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }


        static public string ToString(object obj, string defaultvalue)
        {
            try
            {
                return Convert.ToString(obj);
            }
            catch
            {
                return defaultvalue;
            }
        }
        static public bool ToBoolean(this object obj, bool defaultvalue)
        {
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return defaultvalue;
            }
        }


        /// <summary>
        /// Convert 1 DataTable to 1 list of objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string DataTableToList<T>(DataTable table, out List<T> list)
        {
            list = null;
            if (table == null) return "Error: table is null @DataTableToList";

            list = new List<T>();

            string msg = "";
            foreach (DataRow row in table.Rows)
            {
                T item = default(T);
                msg = RowToObject<T>(row, out item);
                if (msg.Length == 0 && item == null) msg = "Error: item is null @DataTableToList";

                if (msg.Length > 0)
                {
                    list.Clear();
                    list = null;
                    return msg;
                }

                list.Add(item);
            }

            return msg;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string RowToObject<T>(DataRow row, out T value)
        {
            value = default(T);

            if (row == null) return "Error: row is null @RowToObject";

            string columnName = "";
            try
            {
                value = Activator.CreateInstance<T>();
                Type type = value.GetType();
                foreach (DataColumn column in row.Table.Columns)
                {
                    columnName = column.ColumnName;
                    PropertyInfo prop = type.GetProperty(columnName);
                    if (prop == null) continue;

                    object cell = row[columnName];
                    if (cell != DBNull.Value) prop.SetValue(value, prop.PropertyType.Name == "Int32" ? int.Parse(cell.ToString()) : cell, null);
                }
            }
            catch (Exception ex)
            {
                return ex.ToString() + " @" + columnName;
            }
            return "";
        }


        public static DataTable ToDataTable<T>(this IList<T> list, DataTable defaultvalue = null)
        {
            string msg = ListToDataTable(list, out DataTable dt);
            if (msg.Length > 0) return defaultvalue;
            return dt;

        }
        public static string ListToDataTable<T>(IList<T> list, out DataTable dt)
        {
            dt = null;
            try
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
                dt = new DataTable();
                for (int i = 0; i < props.Count; i++) dt.Columns.Add(props[i].Name, props[i].PropertyType);

                object[] values = new object[props.Count];
                foreach (T item in list)
                {
                    for (int i = 0; i < values.Length; i++) values[i] = props[i].GetValue(item);

                    dt.Rows.Add(values);
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        static public string CopyObjectPropertyData<T1, T2>(T1 obj1, T2 obj2, string exceptProperties = null) where T1 : class
        {
            string msg = "";
            string pName = "";

            string[] arrExceptProperties = null;
            if (!string.IsNullOrWhiteSpace(exceptProperties)) arrExceptProperties = exceptProperties.Split('|');

            try
            {
                var ps1 = typeof(T1).GetProperties();
                foreach (var p1 in ps1)
                {
                    pName = p1.Name;
                    if (arrExceptProperties != null && arrExceptProperties.Contains(pName)) continue;

                    var p2 = typeof(T2).GetProperty(pName);
                    if (p2 != null && p2.CanWrite) p2.SetValue(obj2, p1.GetValue(obj1, null), null);
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString() + " @" + pName;
            }
            return msg;
        }
        /// <summary>
        /// Copy các item ở list1 sang list2 (phần tử ở list2 có thể có ít property hơn phần tử ở list1, nhưng tên các property phải giống nhau) 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static string ListToList<T1, T2>(IList<T1> list1, out List<T2> list2) where T1 : class
        {
            list2 = null;
            try
            {
                list2 = new List<T2>();

                foreach (T1 i1 in list1)
                {
                    T2 i2 = Activator.CreateInstance<T2>();
                    CopyObjectPropertyData(i1, i2);
                    list2.Add(i2);
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }


        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;
            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) dest.Write(bytes, 0, cnt);
        }

        public static string ObjectToZippedBase64(object obj, out string zippedBase64)
        {
            string msg = "";
            zippedBase64 = null;

            try
            {
                byte[] zippedData = null;
                msg = Zip(obj, out zippedData);
                if (msg.Length > 0) return msg;

                zippedBase64 = Convert.ToBase64String(zippedData);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }
        public static string ZippedBase64ToObject(string zippedBase64, out object obj)
        {
            string msg = "";
            obj = null;

            try
            {
                byte[] zippedData = Convert.FromBase64String(zippedBase64);

                byte[] bytes = null;
                msg = Unzip(zippedData, out bytes);
                if (msg.Length > 0) return msg;

                obj = BytesToObject(bytes);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }

        public static string Zip(string str, out byte[] zippedData)
        {
            string msg = "";
            zippedData = null;

            try
            {
                if (string.IsNullOrWhiteSpace(str)) return "input string is null or empty";

                var bytes = Encoding.UTF8.GetBytes(str);
                return Zip(bytes, out zippedData);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }
        public static string Zip(object obj, out byte[] zippedData)
        {
            string msg = "";
            zippedData = null;

            try
            {
                if (obj == null) return "object is null";

                byte[] bytes = ObjectToBytes(obj);
                return Zip(bytes, out zippedData);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }
        public static string Zip(byte[] data, out byte[] zippedData)
        {
            string msg = "";
            zippedData = null;

            try
            {
                if (data == null) return "data is null";

                using (var msi = new MemoryStream(data))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        //msi.CopyTo(gs);
                        CopyTo(msi, gs);
                    }

                    zippedData = mso.ToArray();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }

        public static string Unzip(byte[] bytes, out string unzippedData)
        {
            string msg = "";
            unzippedData = null;

            try
            {
                byte[] unzippedBytes = null;
                msg = Unzip(bytes, out unzippedBytes);
                if (msg.Length > 0) return msg;

                unzippedData = Encoding.UTF8.GetString(unzippedBytes);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }
        public static string Unzip(byte[] bytes, out object obj)
        {
            string msg = "";
            obj = null;

            try
            {
                byte[] unzippedBytes = null;
                msg = Unzip(bytes, out unzippedBytes);
                if (msg.Length > 0) return msg;

                obj = BytesToObject(unzippedBytes);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }
        public static string Unzip(byte[] bytes, out byte[] unzippedBytes)
        {
            string msg = "";
            unzippedBytes = null;

            try
            {
                using (var msi = new MemoryStream(bytes))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        //gs.CopyTo(mso);
                        CopyTo(gs, mso);
                    }

                    unzippedBytes = mso.ToArray();
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            return msg;
        }

        /// <summary>
        /// Chuyển string từ tiếng việt có dấu thành không dấu
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="type">0 thì binh thuong, 1 tolower, 2 To upper </param>
        /// <returns></returns>
        static string StringRemoveMarkVietnamese(this string input, int type = 0)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = input.Normalize(NormalizationForm.FormD);
            var result = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            if (type == 1) return result.ToLower();
            else if (type == 2) return result.ToUpper();
            else return result;
        }
        public static string StringRemoveMarkVietnameseToLower(this string input)
        {
            return StringRemoveMarkVietnamese(input, type: 1);
        }
        public static string StringRemoveMarkVietnameseToUpper(this string input)
        {
            return StringRemoveMarkVietnamese(input, type: 2);
        }
    }
}
