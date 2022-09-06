using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    public class Candidate
    {
        public IDataView data { get; set; } = null;

        private long dataCount = -1; //TODO check is really needed ? 

        public Candidate(IDataView data)
        {
            this.data = data;
        }

        public bool CheckDataHasColWithName(string name)
        {
            return CheckColNameExistsIn(name);
        }

        private bool CheckColNameExistsIn(string name)
        {
            bool success = false;
            DataViewSchema.Column? col = GetColumnByName(name);
            if (col != null)
            {
                success = true;
            }
            return success;
        }

        public long GetAmountRows()
        {
            return CountAmountRows(data);
        }

        private long CountAmountRows(IDataView dat)
        {
            if (dat != null)
            {
                if (dataCount == -1)//we do it only one time
                {
                    long counter = 0;
                    //we have to iterate , desgined for streaming 
                    DataViewSchema columns = dat.Schema;
                    if (columns.Count > 0)
                    {
                        //found here
                        using (DataViewRowCursor cursor = dat.GetRowCursor(columns))
                        {

                            while (cursor.MoveNext())
                            {
                                counter += 1;
                            }
                        }
                        if (counter > 0)
                        {
                            dataCount = counter;
                            return counter;
                        }
                    }
                    else Trace.WriteLine("warning 35353 no columns in dataview");

                }//we already counted
                else
                {
                    return dataCount;
                }
            }
            return 0;
        }

        public DataViewSchema.Column? GetColumnByName(string name)
        {
            DataViewSchema.Column? column = null;
            IDataView? view = this.data;

            if (view != null)
            {
                DataViewSchema schema = view.Schema;

                if (schema != null)
                {
                    column = schema.GetColumnOrNull(name);
                }
            }

            return column;
        }
    }
}
