#region Copyright -------------------------------------------------------
// Copyright © 2005 HKV lijn in water, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : OptimaliseRing.General
//
// Author(s)  : Abe Hoekstra, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/DataAccess/MyDataSet.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OptimaliseRing.DataAccess
{
  [Serializable()]
  public class MyDataSet : DataSet
  {
    public MyDataSet()
      : base()
    {
    }
    protected MyDataSet(SerializationInfo info,
       StreamingContext context)
      : base(info, context)
    {
      // Implement type-specific serialization constructor logic.
    }
    /// <summary>
    /// Converts this dataset to sorted list.
    /// </summary>
    /// <returns></returns>
    public SortedList ToSortedList()
    {
      SortedList sl = new SortedList();

      if (this.Tables.Count > 0)
      {
        foreach (DataRow dr in this.Tables[0].Rows)
        {
          SortedList row = new SortedList();
          for (int i = 0; i < this.Tables[0].Columns.Count; i++)
          {
            row.Add(this.Tables[0].Columns[i].Caption, dr[i]);
          }
          sl.Add(sl.Count, row);
        }
      }
      return sl;
    }
    /// <summary>
    /// Converts this dataset to array list.
    /// </summary>
    /// <returns></returns>
    public ArrayList ToArrayList()
    {
      ArrayList sl = new ArrayList();

      if (this.Tables.Count > 0)
      {
        foreach (DataRow dr in this.Tables[0].Rows)
        {
          SortedList row = new SortedList();
          for (int i = 0; i < this.Tables[0].Columns.Count; i++)
          {
            row.Add(this.Tables[0].Columns[i].Caption, dr[i]);
          }
          sl.Add(row);
        }
      }
      return sl;
    }
  }
}
