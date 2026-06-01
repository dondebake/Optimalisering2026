#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/Input/Input.cs 3     31-03-09 10:30 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Aimms;

namespace OptimaliseRing.Aimms
{
  /// <summary>
  /// Input parameters voor Aimms project
  /// </summary>
  [Serializable]
  public class Input
  {
    #region Instance Variables -----------------------------------------------


    /// <summary>
    /// Projectvariabelen
    /// </summary>
    private Projectvariabelen m_projectvariabelen;
    /// <summary>
    /// Settings declarations (Tab Settings)
    /// </summary>
    private Settings m_settings;
    /// <summary>
    /// Discretization declarations (Tab Discretization)
    /// </summary>
    private Discretization m_discretization;
    /// <summary>
    /// Solve declarations (Tab Solve)
    /// </summary>
    private Solve m_solve;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets the projectvariabelen.
    /// </summary>
    /// <value>The projectvariabelen.</value>
    public Projectvariabelen Projectvariabelen
    {
      get { return this.m_projectvariabelen; }
    }

    /// <summary>
    /// Gets the settings.
    /// </summary>
    /// <value>The settings.</value>
    public Settings Settings
    {
      get { return this.m_settings; }
    }

    /// <summary>
    /// Gets the discretization.
    /// </summary>
    /// <value>The discretization.</value>
    public Discretization Discretization
    {
      get { return this.m_discretization; }
    }

    /// <summary>
    /// Gets the solve.
    /// </summary>
    /// <value>The solve.</value>
    public Solve Solve
    {
      get { return this.m_solve; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Input"/> class.
    /// </summary>
    public Input(Projectvariabelen projectvariabelen)
    {
      this.m_projectvariabelen = projectvariabelen;
      this.m_settings = new Settings();
      this.m_discretization = new Discretization();
      this.m_solve = new Solve();
    }

    #endregion Constructors --------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Registers the variabelen.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <returns></returns>
    public bool RegisterVariabelen(Project project)
    {
      if (project != null)
      {
        if (this.m_projectvariabelen.RegisterVariabelen(project))
        {
          if (this.m_settings.RegisterVariabelen(project))
          {
            if (this.m_discretization.RegisterVariabelen(project))
            {
              if (this.m_solve.RegisterVariabelen(project))
              {
                return true;
              }
            }
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Saves this instance.
    /// </summary>
    public void Save()
    {
    }

    #endregion Member functions ----------------------------------------------

  }
}
