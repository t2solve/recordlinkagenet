using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core.Data
{
    public class DataRow : IEnumerable<DataCell>
    {
        private DataTableFeather parent = null;
        public Dictionary<string, DataCell> Data = new Dictionary<string, DataCell>();

        public DataRow(DataTableFeather dataTableFeatherParent)
        {
            parent = dataTableFeatherParent;
        }

        public string GetHashValueForColumList(List<string> colNames)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string colName in colNames)
            {
                if (Data.ContainsKey(colName))
                {
                    sb.Append(Data[colName].Value);
                }
                else Trace.WriteLine("warning 29389283  colname " + colName + " not found, will be ignored");
            }
            Trace.WriteLine("debug: hash string double: " + sb.ToString());
            return HashString(sb.ToString());
        }

        private string HashString(string text, string salt = "")
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }

        public IEnumerator<DataCell> GetEnumerator()
        {
            return Data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}