using System;
using System.Text;
using System.Runtime.InteropServices;

namespace ComponentAge.Dialogs.Interop
{
    public class UnicodeCharBuffer : CharBuffer
    {
        // Methods
        public UnicodeCharBuffer(int size)
        {
            this.buffer = new char[size];
        }

        public override IntPtr AllocCoTaskMem()
        {
            IntPtr ptr1 = Marshal.AllocCoTaskMem(this.buffer.Length * 2);
            Marshal.Copy(this.buffer, 0, ptr1, this.buffer.Length);
            return ptr1;
        }

        public override string GetString()
        {
            int num1 = this.offset;
            while ((num1 < this.buffer.Length) && (this.buffer[num1] != '\0'))
            {
                num1++;
            }
            string text1 = new string(this.buffer, this.offset, num1 - this.offset);
            if (num1 < this.buffer.Length)
            {
                num1++;
            }
            this.offset = num1;
            return text1;
        }

        public override void PutCoTaskMem(IntPtr ptr)
        {
            Marshal.Copy(ptr, this.buffer, 0, this.buffer.Length);
            this.offset = 0;
        }

        public override void PutString(string s)
        {
            int num1 = Math.Min(s.Length, this.buffer.Length - this.offset);
            s.CopyTo(0, this.buffer, this.offset, num1);
            this.offset += num1;
            if (this.offset < this.buffer.Length)
            {
                this.buffer[this.offset++] = '\0';
            }
        }

        // Fields
        internal char[] buffer;
        internal int offset;
    }
}
