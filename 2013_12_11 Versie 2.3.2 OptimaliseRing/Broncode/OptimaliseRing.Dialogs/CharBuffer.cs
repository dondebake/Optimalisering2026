using System;
using System.Text;
using System.Runtime.InteropServices;

namespace ComponentAge.Dialogs.Interop
{
    public abstract class CharBuffer
    {
        // Methods
        protected CharBuffer()
        {
        }
        public abstract IntPtr AllocCoTaskMem();
        public static CharBuffer CreateBuffer(int size)
        {
            if (Marshal.SystemDefaultCharSize == 1)
            {
                return new AnsiCharBuffer(size);
            }
            return new UnicodeCharBuffer(size);
        }
        public abstract string GetString();
        public abstract void PutCoTaskMem(IntPtr ptr);
        public abstract void PutString(string s);
    }
}
