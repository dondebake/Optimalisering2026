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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/DataAccess/MyOleDb.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OptimaliseRing.DataAccess
{
  /// <summary>
  /// Base class for handling OleDb Databases
  /// </summary>
  public class MyOleDB : IDisposable
  {
    /// <summary>
    /// Name of the database
    /// </summary>
    private String _filename;
    /// <summary>
    /// an OleDbConnection object
    /// </summary>
    private OleDbConnection _conOleDb;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MyOleDB"/> class.
    /// </summary>
    public MyOleDB()
    {
    }

    /// <summary>
    /// Releases unmanaged resources and performs other cleanup operations before the
    /// <see cref="T:OptimaliseRing.DataAccess.MyOleDB"/> is reclaimed by garbage collection.
    /// </summary>
    ~MyOleDB()
    {
      // Finalizer calls Dispose(false)
      Dispose(false);
    }

    /// <summary>
    /// Gets the filename.
    /// </summary>
    /// <value>The filename.</value>
    public String Filename
    {
      get
      {
        return _filename;
      }
    }

    public void ExecuteNonQuery(string sql)
    {
      OleDbCommand od = new OleDbCommand(sql, _conOleDb);
      od.ExecuteNonQuery();
    }
    /// <summary>
    /// Gets the data set.
    /// </summary>
    /// <param name="sql">The SQL string</param>
    /// <returns></returns>
    public MyDataSet GetDataSet(string sql)
    {
      OleDbDataAdapter ad = new OleDbDataAdapter(sql, _conOleDb);

      //Console.WriteLine(sql);

      MyDataSet dstemp = new MyDataSet();
      dstemp.Locale = CultureInfo.InvariantCulture;

      ad.Fill(dstemp, "General");

      return dstemp;
    }

    public SortedList GetTableInfo(String tableName)
    {
      SortedList slInfo = new SortedList();

      OleDbCommand cmd = new OleDbCommand();

      cmd.Connection = _conOleDb;
      cmd.CommandText = "SELECT * from " + tableName;

      // KeyInfo: the query returns column and primary key information.
      OleDbDataReader myReader = cmd.ExecuteReader(CommandBehavior.KeyInfo);

      DataTable schemaTablex = myReader.GetSchemaTable();

      //For each field in the table...
      foreach (DataRow myField in schemaTablex.Rows)
      {
        SortedList slField = new SortedList();

        // Een aantal veldeigenschappen opslaan, dit kan naar behoefte
        // worden uitgebreid met andere eigenschappen uit myField
        slField.Add("Datatype", myField["Datatype"]);
        slField.Add("ColumnSize", myField["ColumnSize"]);

        slInfo.Add(myField["ColumnName"].ToString(), slField);
      }
      return slInfo;

    }

    /// <summary>
    /// Tables the exists.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <returns></returns>
    public bool TableExists(string tableName)
    {

      DataTable schemaTable = _conOleDb.GetOleDbSchemaTable(
                  OleDbSchemaGuid.Tables,
                  new object[] { null, null, null, "TABLE" });

      foreach (DataRow row in schemaTable.Rows)
      {
        if (String.Compare(tableName, row["TABLE_NAME"].ToString(), true, System.Threading.Thread.CurrentThread.CurrentCulture) == 0)
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void Close()
    {
      if (_conOleDb != null)
      {
        if (_conOleDb.State == ConnectionState.Open)
        {
          _conOleDb.Close();
          _conOleDb = null;
        }
      }
    }

    /// <summary>
    /// Opens the specified filename.
    /// </summary>
    /// <param name="filename">The filename.</param>
    ///
    /// <param name="connectionString">The connection string.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "OptimaliseRing.DataAccess.MyOleDBErrorException.#ctor(System.String)")]
    public virtual void Open(string filename, string connectionString)
    {
      if (File.Exists(filename))
      {
        if (_conOleDb != null)
        {
          // Er is al een connectie met een database
          // Kijk even of het dezelfde is.
          if (String.Compare(_filename, filename, true, System.Threading.Thread.CurrentThread.CurrentCulture) != 0)
          {
            // Het is niet dezelfde, oude eerst even sluiten
            this.Close();
          }
        }

        if (_conOleDb == null)
        {
          _filename = filename;
          _conOleDb = new OleDbConnection(connectionString + ";Data Source=" + filename);
          _conOleDb.Open();
        }
      }
      else
      {
        // File does not exists
        throw new MyOleDBErrorException("File does not exists");

      }
    }


    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
      ;
    }
    /// <summary>
    /// Disposes the specified disposing.
    /// </summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        // free managed resources
        if (_conOleDb != null)
        {
          _conOleDb.Dispose();
          _conOleDb = null;
        }
      }
      //// free native resources if there are any.
      //if (nativeResource != IntPtr.Zero)
      //{
      //  Marshal.FreeHGlobal(nativeResource);
      //  nativeResource = IntPtr.Zero;
      //}
    }

    #endregion
  }


  /// <summary>
  /// Exception class for OleDb class
  /// </summary>
  [Serializable()]
  public class MyOleDBErrorException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MyOleDBErrorException"/> class.
    /// </summary>
    public MyOleDBErrorException()
    {
      // Add any type-specific logic, and supply the default message.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MyOleDBErrorException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public MyOleDBErrorException(string message)
      : base(message)
    {
      // Add any type-specific logic.
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MyOleDBErrorException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public MyOleDBErrorException(string message, Exception innerException)
      :
       base(message, innerException)
    {
      // Add any type-specific logic for inner exceptions.
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MyOleDBErrorException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
    /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
    protected MyOleDBErrorException(SerializationInfo info,
       StreamingContext context)
      : base(info, context)
    {
      // Implement type-specific serialization constructor logic.
    }
  }
}
