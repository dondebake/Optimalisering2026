#region Copyright -------------------------------------------------------
// Copyright © 2005 HKV lijn in water, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : OptimaliseRing.General
//
// Author(s)  : Johan Ansink, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/General/WaitCursor.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Drawing;
using System.Windows.Forms;

namespace OptimaliseRing.General
{
	/// <summary>
	/// Summary description for WaitCursor.
	/// </summary>
  public class WaitCursor : IDisposable
  {
    static private Cursor       _cursorOld = null;
    static private int          _nReferenceCount = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitCursor"/> class.
    /// </summary>
    public WaitCursor()
    {
      ShowWait();
    }

    /// <summary>
    /// Shows the wait cursor
    /// </summary>
    static private void ShowWait()
    {
      if (_nReferenceCount == 0)
      {
        _cursorOld = Cursor.Current;
        Cursor.Current = Cursors.WaitCursor;
      }

      _nReferenceCount += 1;
    }

    /// <summary>
    /// Pops the wait cursor
    /// </summary>
    static private void PopWait()
    {
      if (_nReferenceCount > 0)
      {
        _nReferenceCount -= 1;

        if (_nReferenceCount == 0)
        {
          Cursor.Current = _cursorOld;
          _cursorOld = null;
        }
      }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or
    /// resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      PopWait();
    }
  }

}
