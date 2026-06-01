#region Copyright -------------------------------------------------------
// Copyright © 2008, Rijkswaterstaat/Waterdienst & ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : 1142.50.00 Batch OptimalisatieRing
//              OptimaliseRing - Economische optimalisatie van veiligheidsniveaus van dijkringen
//
// Author(s)  : Johan Ansink, HKV lijn in water
//              Matthijs Duits, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Classes/ThisApp.cs 2     18/06/08 14:11 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
using System.Globalization;

using OptimaliseRing.General;
using OptimaliseRing.UI.Forms;
using System.Collections.Generic;

namespace OptimaliseRing.UI
{
  #region ThisAppLanguage
  /// <summary>
  /// Application language profile
  /// </summary>
  public sealed class ThisAppLanguage
  {
    private static volatile OptimaliseRing.Profile.Ini m_Instance;
    private static object m_SyncRoot = new Object();

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:ThisAppLanguage"/> class.
    /// </summary>
    private ThisAppLanguage() { }

    /// <summary>
    /// Gets the instance of ThisAppLanguage
    /// </summary>
    public static OptimaliseRing.Profile.Ini Instance
    {
      get
      {
        if (m_Instance == null)
        {
          lock (m_SyncRoot)
          {
            if (m_Instance == null)
              m_Instance = new OptimaliseRing.Profile.Ini(Path.Combine(Application.StartupPath, "Language.ini"));
          }
        }

        return m_Instance;
      }
    }
  }
  #endregion

  #region ThisAppProfile
  /// <summary>
  /// Application profile
  /// </summary>
  public sealed class ThisAppProfile
  {
    private static volatile OptimaliseRing.Profile.Ini m_Instance;
    private static object m_SyncRoot = new Object();

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:ThisAppProfile"/> class.
    /// </summary>
    private ThisAppProfile() { }

    /// <summary>
    /// Gets the instance of ThisAppProfile
    /// </summary>
    public static OptimaliseRing.Profile.Ini Instance
    {
      get
      {
        if (m_Instance == null)
        {
          lock (m_SyncRoot)
          {
            if (m_Instance == null)
              m_Instance = new OptimaliseRing.Profile.Ini();
          }
        }

        return m_Instance;
      }
    }
  }
  #endregion

  #region ThisAppCulture
  /// <summary>
  /// Application error
  /// </summary>
  public sealed class ThisAppCulture
  {
    private static volatile CultureInfo m_Instance;
    private static object m_SyncRoot = new Object();

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:ThisAppCulture"/> class.
    /// </summary>
    private ThisAppCulture() { }

    /// <summary>
    /// Gets the instance of ApplicationError
    /// </summary>
    public static CultureInfo Instance
    {
      get
      {
        if (m_Instance == null)
        {
          lock (m_SyncRoot)
          {
            if (m_Instance == null)
              m_Instance = new CultureInfo(ThisAppProfile.Instance.GetValue("OptimaliseRing", "Taal", "en-GB"));
          }
        }

        return m_Instance;
      }
    }
  }
  #endregion

  #region ThisAppErr
  /// <summary>
  /// Application error
  /// </summary>
  public sealed class ThisAppErr
  {
    private static volatile OptimaliseRing.General.ApplicationError m_Instance;
    private static object m_SyncRoot = new Object();

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:ThisAppErr"/> class.
    /// </summary>
    private ThisAppErr() { }

    /// <summary>
    /// Gets the instance of ApplicationError
    /// </summary>
    public static OptimaliseRing.General.ApplicationError Instance
    {
      get
      {
        if (m_Instance == null)
        {
          lock (m_SyncRoot)
          {
            if (m_Instance == null)
              m_Instance = new OptimaliseRing.General.ApplicationError();
          }
        }

        return m_Instance;
      }
    }
  }
  #endregion

  #region ThisAppWorkingDirectory
  /// <summary>
  /// Application working directory
  /// </summary>
  public sealed class ThisAppWorkingDirectory
  {
    private static volatile WorkingDirectory m_Instance;
    private static object m_SyncRoot = new Object();

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:ThisAppWorkingDirectory"/> class.
    /// </summary>
    private ThisAppWorkingDirectory() { }

    /// <summary>
    /// Gets the instance of ApplicationError
    /// </summary>
    public static WorkingDirectory Instance
    {
      get
      {
        if (m_Instance == null)
        {
          lock (m_SyncRoot)
          {
            if (m_Instance == null)
              m_Instance = new WorkingDirectory();
          }
        }

        return m_Instance;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public sealed class WorkingDirectory
    {
      private string m_Werkmap = MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "Werkmap", @".\Werkmap"));

      /// <summary>
      /// Gets or sets the werkmap.
      /// </summary>
      /// <value>The werkmap.</value>
      public string Werkmap
      {
        get { return m_Werkmap; }
        set
        {
          m_Werkmap = value;
        }
      }


      /// <summary>
      /// Maak een nieuwe instantiatie van de  <see cref="T:WorkingDirectory"/> class.
      /// </summary>
      public WorkingDirectory()
      {
        if (!Directory.Exists(this.Werkmap))
        {
          // map bestaat niet, maak een map werkmap onder de programmadirectory
          string tmp = System.IO.Path.Combine(MyPath.ProgramDirectoryName(), "Werkmap");
          if (!Directory.Exists(tmp))
          {
            Directory.CreateDirectory(tmp);
          }
          this.Werkmap = tmp;
        }
        ThisAppProfile.Instance.SetValue("OptimaliseRing", "Werkmap", MyPath.RelativeName(this.m_Werkmap));
      }
    }
  }
  #endregion

  #region ThisAppLegendaItems
  /// <summary>
  /// Application Legenda items
  /// </summary>
  public sealed class ThisAppLegendaItems2
  {
    private static volatile SortedList<int,SortedList> m_Instance;
    private static object m_SyncRoot = new Object();

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:ThisAppLegendaItems"/> class.
    /// </summary>
    private ThisAppLegendaItems2() { }

    /// <summary>
    /// Gets the instance of Berekeningen
    /// </summary>
    public static SortedList<int, SortedList> Instance
    {
      get
      {
        if (m_Instance == null)
        {
          lock (m_SyncRoot)
          {
            if (m_Instance == null)
            {
              m_Instance = new SortedList<int, SortedList>();
            }
          }
        }

        return m_Instance;
      }
    }
  }
  #endregion
}
