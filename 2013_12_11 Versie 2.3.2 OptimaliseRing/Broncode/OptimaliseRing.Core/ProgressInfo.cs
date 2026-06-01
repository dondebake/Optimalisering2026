using System;
using System.Collections.Generic;
using System.Text;

namespace OptimaliseRing.Core
{
   public class ProgressInfo
   {
      private string m_Status;
      private int m_Progress;

      public string Status
      {
         get { return m_Status; }
         set { m_Status = value; }
      }

      public int Progress
      {
         get { return m_Progress; }
         set { m_Progress = value; }
      }
   }

}
