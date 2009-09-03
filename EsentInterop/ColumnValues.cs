﻿//-----------------------------------------------------------------------
// <copyright file="ColumnValues.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Microsoft.Isam.Esent.Interop
{
    /// <summary>
    /// Base class for objects that represent a column value to be set.
    /// </summary>
    public abstract class ColumnValue
    {
        /// <summary>
        /// Initializes a new instance of the ColumnValue class.
        /// </summary>
        protected ColumnValue()
        {
            this.ItagSequence = 1;    
        }

        /// <summary>
        /// Gets or sets the columnid to be set.
        /// </summary>
        public JET_COLUMNID Columnid { get; set; }

        /// <summary>
        /// Gets or sets column retrieval options.
        /// </summary>
        public SetColumnGrbit SetGrbit { get; set; }

        /// <summary>
        /// Gets or sets the column itag sequence.
        /// </summary>
        public int ItagSequence { get; set; }

        /// <summary>
        /// Gets the error generated by setting this column.
        /// </summary>
        public JET_err Error { get; internal set; }

        internal abstract unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i);

        internal NATIVE_SETCOLUMN MakeNativeSetColumn()
        {
            return new NATIVE_SETCOLUMN
            {
                columnid = this.Columnid.Value,
                grbit = (uint) this.SetGrbit,
                itagSequence = 1,
            };
        }

        internal unsafe int SetColumns(
            JET_SESID sesid,
            JET_TABLEID tableid,
            ColumnValue[] columnValues,
            NATIVE_SETCOLUMN* nativeColumns,
            int i,
            void* buffer,
            int bufferSize,
            bool hasValue)
        {
            Debug.Assert(this == columnValues[i], "SetColumns should be called on the current object");
            NATIVE_SETCOLUMN setcolumn = this.MakeNativeSetColumn();

            if (hasValue)
            {
                setcolumn.cbData = checked((uint)bufferSize);
                setcolumn.pvData = new IntPtr(buffer);
                if (0 == bufferSize)
                {
                    setcolumn.grbit |= (uint)SetColumnGrbit.ZeroLength;
                }
            }

            nativeColumns[i] = setcolumn;

            int err = i == columnValues.Length - 1
                          ? Api.Impl.JetSetColumns(sesid, tableid, nativeColumns, columnValues.Length)
                          : columnValues[i + 1].SetColumns(sesid, tableid, columnValues, nativeColumns, i + 1);

            this.Error = (JET_err) nativeColumns[i].err;
            return err;
        }
    }

    /// <summary>
    /// Set a column of a struct type (e.g. Int32/Guid).
    /// </summary>
    /// <typeparam name="T">Type to set.</typeparam>
    public abstract class ColumnValueOfStruct<T> : ColumnValue where T : struct
    {
        /// <summary>
        /// Gets or sets the value to retrieve.
        /// </summary>
        public T? Value { get; set; }

        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }

    /// <summary>
    /// A <see cref="bool"/> column value.
    /// </summary>
    public class BoolColumnValue : ColumnValueOfStruct<bool>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            byte data = this.Value.GetValueOrDefault() ? (byte)0xFF : (byte)0x00;
            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, &data, sizeof(byte), this.Value.HasValue);
        }
    }

    /// <summary>
    /// A <see cref="Byte"/> column value.
    /// </summary>
    public class ByteColumnValue : ColumnValueOfStruct<byte>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            var data = this.Value.GetValueOrDefault();
            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, &data, sizeof(byte), this.Value.HasValue);
        }
    }

    /// <summary>
    /// An <see cref="Int16"/> column value.
    /// </summary>
    public class Int16ColumnValue : ColumnValueOfStruct<short>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            var data = this.Value.GetValueOrDefault();
            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, &data, sizeof(short), this.Value.HasValue);
        }
    }

    /// <summary>
    /// A <see cref="UInt16"/> column value.
    /// </summary>
    public class UInt16ColumnValue : ColumnValueOfStruct<ushort>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            var data = this.Value.GetValueOrDefault();
            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, &data, sizeof(ushort), this.Value.HasValue);
        }
    }

    /// <summary>
    /// An <see cref="Int32"/> column value.
    /// </summary>
    public class Int32ColumnValue : ColumnValueOfStruct<int>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            var data = this.Value.GetValueOrDefault();
            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, &data, sizeof(int), this.Value.HasValue);
        }
    }

    /// <summary>
    /// A <see cref="UInt32"/> column value.
    /// </summary>
    public class UInt32ColumnValue : ColumnValueOfStruct<uint>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            var data = this.Value.GetValueOrDefault();
            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, &data, sizeof(uint), this.Value.HasValue);
        }
    }

    /// <summary>
    /// An <see cref="Int64"/> column value.
    /// </summary>
    public class Int64ColumnValue : ColumnValueOfStruct<long>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            var data = this.Value.GetValueOrDefault();
            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, &data, sizeof(long), this.Value.HasValue);
        }
    }

    /// <summary>
    /// A <see cref="UInt64"/> column value.
    /// </summary>
    public class UInt64ColumnValue : ColumnValueOfStruct<ulong>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            var data = this.Value.GetValueOrDefault();
            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, &data, sizeof(ulong), this.Value.HasValue);
        }
    }

    /// <summary>
    /// A <see cref="float"/> column value.
    /// </summary>
    public class FloatColumnValue : ColumnValueOfStruct<float>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            var data = this.Value.GetValueOrDefault();
            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, &data, sizeof(float), this.Value.HasValue);
        }
    }

    /// <summary>
    /// A <see cref="double"/> column value.
    /// </summary>
    public class DoubleColumnValue : ColumnValueOfStruct<double>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            var data = this.Value.GetValueOrDefault();
            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, &data, sizeof(double), this.Value.HasValue);
        }
    }

    /// <summary>
    /// A <see cref="Guid"/> column value.
    /// </summary>
    public class DateTimeColumnValue : ColumnValueOfStruct<DateTime>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            var data = this.Value.GetValueOrDefault().ToOADate();
            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, &data, sizeof(double), this.Value.HasValue);
        }
    }

    /// <summary>
    /// A <see cref="Guid"/> column value.
    /// </summary>
    public class GuidColumnValue : ColumnValueOfStruct<Guid>
    {
        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            byte[] data = this.Value.GetValueOrDefault().ToByteArray();
            fixed (void* buffer = data)
            {
                return this.SetColumns(
                    sesid, tableid, columnValues, nativeColumns, i, buffer, data.Length, this.Value.HasValue);
            }
        }
    }

    /// <summary>
    /// A Unicode string column value.
    /// </summary>
    public class StringColumnValue : ColumnValue
    {
        /// <summary>
        /// Gets or sets the value of the column. Use <see cref="Api.SetColumns"/> to update a
        /// record with the column value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            return this.Value;
        }

        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            if (null != this.Value)
            {
                fixed (void* buffer = this.Value)
                {
                    return this.SetColumns(
                        sesid, tableid, columnValues, nativeColumns, i, buffer, this.Value.Length * sizeof(char), true);
                }
            }

            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, null, 0, false);
        }
    }

    /// <summary>
    /// A byte array column value.
    /// </summary>
    public class BytesColumnValue : ColumnValue
    {
        /// <summary>
        /// Gets or sets the value of the column. Use <see cref="Api.SetColumns"/> to update a
        /// record with the column value.
        /// </summary>
        public byte[] Value { get; set; }

        internal override unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i)
        {
            if (null != this.Value)
            {
                fixed (void* buffer = this.Value)
                {
                    return this.SetColumns(
                        sesid, tableid, columnValues, nativeColumns, i, buffer, this.Value.Length, true);
                }
            }

            return this.SetColumns(sesid, tableid, columnValues, nativeColumns, i, null, 0, false);
        }
    }
}