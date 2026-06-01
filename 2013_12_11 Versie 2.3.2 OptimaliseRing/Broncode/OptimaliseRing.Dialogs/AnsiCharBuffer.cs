using System;
using System.Text;
using System.Runtime.InteropServices;

namespace ComponentAge.Dialogs.Interop
{
    public class AnsiCharBuffer : CharBuffer
    {
        // Methods
        public AnsiCharBuffer(int size)
        {
            this.buffer = new byte[size];
        }
        public override IntPtr AllocCoTaskMem()
        {
            IntPtr ptr1 = Marshal.AllocCoTaskMem(this.buffer.Length);
            Marshal.Copy(this.buffer, 0, ptr1, this.buffer.Length);
            return ptr1;
        }
        public override string GetString()
        {
            int num1 = this.offset;
            while ((num1 < this.buffer.Length) && (this.buffer[num1] != 0))
            {
                num1++;
            }
            string text1 = Encoding.Default.GetString(this.buffer, this.offset, num1 - this.offset);
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
            byte[] buffer1 = Encoding.Default.GetBytes(s);
            int num1 = Math.Min(buffer1.Length, this.buffer.Length - this.offset);
            Array.Copy(buffer1, 0, this.buffer, this.offset, num1);
            this.offset += num1;
            if (this.offset < this.buffer.Length)
            {
                this.buffer[this.offset++] = 0;
            }
        }

        // Fields
        internal byte[] buffer;
        internal int offset;
    }
}
