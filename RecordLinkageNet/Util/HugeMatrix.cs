using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RecordLinkageNet.Core
{
    //! class based on answer of https://stackoverflow.com/a/55489819
    //! without transpose funtionality
    public class HugeMatrix<T> : IDisposable
        where T : struct
    {
       
        private IntPtr pointer = IntPtr.Zero;
        private ulong nRows = 0;
        private ulong nColumns = 0;
        private ulong b_element_size = 0;
        private ulong b_row_size = 0;
        private ulong b_size = 0;
        private bool disposed = false;


        public HugeMatrix()
            : this(0, 0)
        {
        }

        public HugeMatrix(ulong nrows, ulong ncols)
        {
            if (nrows < 0)
                throw new ArgumentException("The number of rows can not be negative");
            if (ncols < 0)
                throw new ArgumentException("The number of columns can not be negative");
            nRows = nrows;
            nColumns = ncols;
            b_element_size = (ulong)(Marshal.SizeOf(typeof(T)));
            b_row_size = (ulong)nColumns * b_element_size;
            b_size = (ulong)nRows * b_row_size;
            pointer = Marshal.AllocHGlobal((IntPtr)b_size);

            //TODO zero the memory
            //Marshal.Copy(new byte[b_size], 0, pointer,(int) b_size);

            disposed = false;
        }

        public IntPtr Pointer
        {
            get { return pointer; }
        }

        public ulong NRows
        {
            get { return nRows; }
        }

        public ulong NColumns
        {
            get { return nColumns; }
        }

        public HugeMatrix(T[,] matrix)
            : this((ulong)matrix.GetLength(0), (ulong)matrix.GetLength(1))
        {
            ulong nrows = (ulong)matrix.GetLength(0);
            ulong ncols = (ulong)matrix.GetLength(1);
            for (ulong i1 = 0; i1 < nrows; i1++)
                for (ulong i2 = 0; i2 < ncols; i2++)
                    this[i1, i2] = matrix[i1, i2];
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Marshal.FreeHGlobal(pointer);
                nRows = 0;
                nColumns = 0;
                b_element_size = 0;
                b_row_size = 0;
                b_size = 0;
                pointer = IntPtr.Zero;
                disposed = true;
            }
        }

        public T this[ulong i_row, ulong i_col]
        {
            get
            {
                IntPtr p = GetAddress(i_row, i_col);
                return (T)Marshal.PtrToStructure(p, typeof(T));
            }
            set
            {
                IntPtr p = GetAddress(i_row, i_col);
                Marshal.StructureToPtr(value, p, true);
            }
        }

        private IntPtr GetAddress(ulong i_row, ulong i_col)
        {
            if (disposed)
                throw new ObjectDisposedException("Can't access the matrix once it has been disposed");
            if (i_row < 0)
                throw new IndexOutOfRangeException("Negative row indices are not allowed");
            if (i_row >= NRows)
                throw new IndexOutOfRangeException("Row index is out of bounds of this matrix");
            if (i_col < 0)
                throw new IndexOutOfRangeException("Negative column indices are not allowed");
            if (i_col >= NColumns)
                throw new IndexOutOfRangeException("Column index is out of bounds of this matrix");
            ulong i1 = i_row;
            ulong i2 = i_col;
            ulong p_row = (ulong)pointer + b_row_size * (ulong)i1;
            IntPtr p = (IntPtr)(p_row + b_element_size * (ulong)i2);
            return p;
        }
    }

}
