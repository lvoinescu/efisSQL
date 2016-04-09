/*
    efisSQL - data base management tool
    Copyright (C) 2011  Lucian Voinescu

    This file is part of efisSQL

    efisSQL is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    efisSQL is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with efisSQL. If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Runtime.InteropServices;


namespace DBMS.core
{


   public unsafe class SamHugeString : IDisposable
    {
        private char * buffer;
        private int maxCapacity;
        private IntPtr pointer; 

        private int length = 0;
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public  unsafe SamHugeString(int maxCapacity)
        {
            if (buffer == null)
            {
                pointer = Marshal.AllocHGlobal(sizeof(char)*maxCapacity);
                buffer = (char*)pointer;
            }
            length = 0;
            this.maxCapacity = maxCapacity;
        }
        public unsafe SamHugeString Append(char c)
        {
            Marshal.Copy(new char[] { c },0, new IntPtr( (long)this.pointer.ToInt64() + sizeof(char)*length),  1);
            length++;
            return this;
        }

        public SamHugeString Append(char[] buffer, int start, int len)
        {
            long adreess =  this.pointer.ToInt64() + sizeof(char)*length;
            Marshal.Copy(buffer, start, new IntPtr(adreess), len);
            this.length += len;
            return this;
        }


        public SamHugeString Append(char[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
                this.buffer[this.length + i] = buffer[i];
            this.length += buffer.Length;
            return this;
        }

        public SamHugeString Append(string buffer)
        {
        	char[] aux =   buffer.ToCharArray();
            Marshal.Copy(aux, 0, new IntPtr((long)this.pointer.ToInt64() + sizeof(char)*length), aux.Length);
            this.length += buffer.Length;
            return this;
        }

        public void Clear()
        {
            this.length = 0;
        }

        public override string ToString()
        {
            return Marshal.PtrToStringAuto(pointer, length);
        }


       public void Dispose()
       {
           Marshal.FreeHGlobal(pointer);
           buffer = null;
           GC.Collect();
       }

    }
}
