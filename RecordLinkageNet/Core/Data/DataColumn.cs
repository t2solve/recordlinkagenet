﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace RecordLinkageNet.Core.Data
{
    public class DataColumn : IEnumerable<DataCell>
    {
        //TODO 
        //TODO avoid public
        public DataTableFeather ParentTable = null;
        public DataCell[] Rows = null; //TODO use init 
        public string Name = "";
        public Type DataType = null;

        private int indexCounter = -1;
        private int rowIndexLimit = -1;

        //public  CompareCondition compareCondition = null;
        public DataColumn(int cellAmount, Type type)
        {
            if (cellAmount < 0)
                throw new ArgumentOutOfRangeException("cellamount");
            if (type == null)
                throw new ArgumentNullException("type");

            //set type
            DataType = type;

            switch (type)
            {
                case Type stringType when stringType == typeof(string):
                    InitStringCells(cellAmount);
                    break;
                default:
                    Trace.WriteLine("warning 2983298398 not implemented type: found :" + type);
                    throw new NotImplementedException();

            }
        }
        public Type GetDataTypeOfCol()
        {
            return DataType;
        }

        public DataCell At(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= rowIndexLimit)
                return null;
            return Rows[rowIndex];
        }

        public bool AppendCell(DataCell cell)
        {
            if (indexCounter == -1)
            {
                Trace.WriteLine("warning 2983928 array not initialisied yes");
                return false;

            }
            if (indexCounter >= rowIndexLimit)
            {
                Trace.WriteLine("error 3f9g235235 array limit reached");
                return false;
            }
            Rows[indexCounter] = cell;
            cell.Id = (uint)indexCounter; // save what id

            //we count
            indexCounter++;

            return true;
        }

        public IEnumerator<DataCell> GetEnumerator()
        {
            return (new List<DataCell>(Rows)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void InitStringCells(int amount)
        {
            Rows = new DataCellString[amount];

            for (uint i = 0; i < Rows.Length; i++)
            {
                Rows[i] = new DataCellString();
                Rows[i].Id = i;
            }
            indexCounter = 0;
            rowIndexLimit = amount;

        }

    }
}
