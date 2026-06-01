#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Connection.cs 3     31-03-09 10:30 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Aimms;

namespace OptimaliseRing.Aimms
{

  /// <summary>
  /// Connection
  /// </summary>
  public class Connection : IDisposable
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Project
    /// </summary>
    private Project m_project = null;
    /// <summary>
    /// Projectpath
    /// </summary>
    private string m_projectpath;
    /// <summary>
    /// Connectionstate
    /// </summary>
    private bool m_isConnected;
    /// <summary>
    /// Ini met foutmeldingen
    /// </summary>
    private Hkv.Profile.Ini m_appIni;

    private string m_language;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets the projectpath.
    /// </summary>
    /// <value>The projectpath.</value>
    public string Projectpath
    {
      get { return this.m_projectpath; }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is connected.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is connected; otherwise, <c>false</c>.
    /// </value>
    public bool IsConnected
    {
      get { return this.m_isConnected; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Connection"/> class.
    /// </summary>
    /// <param name="projectpath">The projectpath.</param>
    public Connection(string projectpath, string language)
    {
      // == optimalisering.ini
      this.m_appIni = new Hkv.Profile.Ini();
      this.m_language = language;

      this.m_isConnected = false;

      if (System.IO.File.Exists(projectpath))
      {
        this.m_projectpath = projectpath;
        this.m_project = new Project();
        this.m_project.StartupMode = AIMMS_STARTUPMODE.STARTUP_HIDDEN;
        this.Open();
      }
      else
      {
        // Aimms project niet aanwezig
        string description = this.m_appIni.GetValue("ERROR" + this.m_language, "Error.0001", "No Aimms project file.");
        throw new ApplicationException(description);
      }
    }

    #endregion Constructors --------------------------------------------------

    #region Event handling ---------------------------------------------------
    #endregion Event handling ------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Opens this instance.
    /// </summary>
    /// <returns></returns>
    private bool Open()
    {
      try
      {
        if (this.m_project != null)
        {
          if (this.m_isConnected == false)
          {
            if (this.m_project.ProjectOpen(this.m_projectpath, 0) == 1)
            {
              this.m_project.GetControl(-1);
              this.m_isConnected = true;
              return true;
            }
            else
            {
              this.m_project = null;
            }
          }
        }
      }
      catch (Exception appex)
      {
        // Kan geen verbinding met Aimms maken, mogelijke oorzaken:
        // - niet geinstalleerd
        // - geen licentie
        // - projectbestand niet goed
        // - project al open op een andere locatie

        //string description = this.m_appIni.GetValue("ERROR" + this.m_language, "Error.0001", "No Aimms project file");
        //throw new ApplicationException(description);
      }

      this.m_isConnected = false;
      return false;
    }

    /// <summary>
    /// Runs the specified input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    public Output Run(Input input)
    {
      Output output = null;

      if (this.m_project != null && this.m_isConnected == true)
      {
        if (input != null)
        {
          object[] nothing = null;

          // Alle benodigde variabelen zetten om te rekenen.
          if (input.RegisterVariabelen(this.m_project))
          {

            // Start de berekening       Algorithm_wrapper
            if (this.m_project.Run("Solve_selected", ref nothing) == 0)
            {

              output = new Output();
              output.GetResults(this.m_project, input.Projectvariabelen.Segments);
            }
            else
            {
              // Kan Algorithm_wrapper niet starten in Aimms
              string description = this.m_appIni.GetValue("ERROR" + this.m_language, "Error.0001", "Can't start Aimms calculation.");
              throw new ApplicationException(description);
            }
          }
          else
          {
            // Kan inputvariabelen invullen
            string description = this.m_appIni.GetValue("ERROR" + this.m_language, "Error.0001", "Can't assign inputvariables.");
            throw new ApplicationException(description);
          }
        }
      }

      return output;
    }

    /// <summary>
    /// Closes this instance.
    /// </summary>
    private void Close()
    {
      if (this.m_project != null)
      {
        this.m_project.ReleaseControl();
        this.m_project.ProjectClose();
        this.m_project = null;
      }
      this.m_isConnected = false;
    }

    #endregion Member functions ----------------------------------------------

    #region IDisposable Members ----------------------------------------------

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    void IDisposable.Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the specified disposing.
    /// </summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Close();
      }
    }

    #endregion Disposable Members --------------------------------------------
  }
}
