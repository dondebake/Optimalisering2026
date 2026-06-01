using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace OptimaliseRing.General
{
  /// <summary>
  /// Static methods for reading XML schema
  /// </summary>
  class ExcelReader
  {
    /// <summary>
    /// Main static method which imports XML spreadsheet into DataTable
    /// </summary>
    /// <param name="ExcelXmlFile">Imported file</param>
    /// <returns>dataTable result</returns>
    public static DataTable ReadExcelXML(string ExcelXmlFile)
    {
      DataTable dt = new DataTable();
      XmlDocument xc = new XmlDocument();
      xc.Load(ExcelXmlFile);
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(xc.NameTable);
      nsmgr.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
      nsmgr.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
      nsmgr.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");

      XmlElement xe = (XmlElement)xc.DocumentElement.SelectSingleNode("//ss:Worksheet/ss:Table", nsmgr);
      if (xe == null)
        return null;
      XmlNodeList xl = xe.SelectNodes("ss:Row", nsmgr);
      int Row = -1, Col = 0;
      Dictionary<int, string> cols = new Dictionary<int, string>();
      foreach (XmlElement xi in xl)
      {
        XmlNodeList xcells = xi.SelectNodes("ss:Cell", nsmgr);
        Col = 0;
        foreach (XmlElement xcell in xcells)
        {
          if (Row == -1)
          {
            dt.Columns.Add(xcell.InnerText);
            cols[Col++] = xcell.InnerText;
          }
          else
          {
            if (xcell.Attributes["ss:Index"] != null)
            {
              int idx = int.Parse(xcell.Attributes["ss:Index"].InnerText);
              Col = idx - 1;
            }

            SetCol(dt, Row, (string)cols[Col++], xcell.InnerText, typeof(string));
          }
        }
        Row++;
      }
      return dt;
    }

    /// <summary>
    /// Adds row to datatable, manages System.DBNull and so
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="AcceptChanges"></param>
    /// <returns></returns>
    public static int AddRow(DataTable dt, bool AcceptChanges)
    {
      object[] Values = new object[dt.Columns.Count];
      for (int Column = 0; Column < dt.Columns.Count; Column++)
      {
        if (!dt.Columns[Column].AllowDBNull)
        {
          if (dt.Columns[Column].DefaultValue != null &&
              dt.Columns[Column].DefaultValue != System.DBNull.Value)
          {
            Values[Column] = dt.Columns[Column].DefaultValue;
          }
        }
      }
      dt.Rows.Add(Values);
      if (AcceptChanges)
      {
        dt.AcceptChanges();
      }
      return dt.Rows.Count - 1;
    }

    /// <summary>
    /// Sets data into datatable in safe manner of row index
    /// </summary>
    /// <param name="dt">DataTable to set</param>
    /// <param name="Row">Ordinal row index</param>
    /// <param name="ColumnName">name of column to set</param>
    /// <param name="Value">non/typed value to set</param>
    /// <param name="TypeOfValue">Becase Value can be null we must know datatype to manage default values</param>
    /// <returns></returns>
    public static DataColumn SetCol(DataTable dt, int Row, string ColumnName,
                                    object Value, System.Type TypeOfValue)
    {
      if (dt == null || ColumnName == null || ColumnName == "")
        return null;

      if (Value == null)
        Value = System.DBNull.Value;

      int nIndex = -1;
      DataColumn dcol = null;
      if (dt.Columns.Contains(ColumnName))
      {
        dcol = dt.Columns[ColumnName];
      }
      else
      {
        dcol = dt.Columns.Add(ColumnName, TypeOfValue);

      }
      if (dcol.ReadOnly)
        dcol.ReadOnly = false;

      nIndex = dcol.Ordinal;
      //new empty row appended
      if (dt.Rows.Count == Row && Row >= 0)
      {
        AddRow(dt, false);
      }
      //one row
      if (Row >= 0)
      {
        dt.Rows[Row][nIndex] = Value;
      }
      else if (Row == -1)
      { //all rows
        try
        {
          for (Row = 0; Row < dt.Rows.Count; Row++)
          {
            if (dt.Rows[Row].RowState == DataRowState.Deleted)
            {
              continue;
            }
            dt.Rows[Row][nIndex] = Value;
          }
        }
        catch (Exception)
        {
        }
      }

      return dcol;
    }

  }

}
