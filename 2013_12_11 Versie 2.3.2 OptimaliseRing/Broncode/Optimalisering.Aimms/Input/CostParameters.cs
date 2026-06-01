#region Copyright -------------------------------------------------------
// Copyright © 2009, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace OptimaliseRing.Aimms
{
  /// <summary>
  /// ExponentialCostParameters
  /// </summary>
  [Serializable]
  public class ExponentialCostParameters
  {

    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Exp_fixed
    /// var: C
    /// eenheid: EUR
    /// domein: l (segments)
    /// </summary>
    private double m_exp_fixed;
    /// <summary>
    /// Exp_linear
    /// var: b
    /// eenheid: EUR/cm
    /// domein: l (segments)
    /// </summary>
    private double m_exp_linear;
    ///// <summary>
    ///// Exp_power
    ///// var: lambda
    ///// eenheid: 1/cm
    ///// domein: l (segments)
    ///// </summary>
    //private double m_exp_power;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or sets the Exp_fixed.
    /// </summary>
    /// <value>
    /// Exp_fixed
    /// var: C
    /// eenheid: EUR
    /// domein: l (segments)
    /// </value>
    public double Exp_fixed
    {
      get { return this.m_exp_fixed; }
      set { this.m_exp_fixed = value; }
    }

    /// <summary>
    /// Gets or sets the Exp_linear.
    /// </summary>
    /// <value>
    /// Exp_linear
    /// var: b
    /// eenheid: EUR/cm
    /// domein: l (segments)
    /// </value>
    public double Exp_linear
    {
      get { return this.m_exp_linear; }
      set { this.m_exp_linear = value; }
    }

    ///// <summary>
    ///// Gets or sets the Exp_power.
    ///// </summary>
    ///// <value>
    ///// Exp_power
    ///// var: lambda
    ///// eenheid: 1/cm
    ///// domein: l (segments)
    ///// </value>
    //public double Exp_power
    //{
    //  get { return this.m_exp_power; }
    //  set { this.m_exp_power = value; }
    //}

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialCostParameters"/> class.
    /// </summary>
    public ExponentialCostParameters()
    {
      this.m_exp_fixed = 0.0;
      this.m_exp_linear = 0.0;
      //this.m_exp_power = 0.0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialCostParameters"/> class.
    /// </summary>
    /// <param name="exp_fixed">The exp_fixed.</param>
    /// <param name="exp_linear">The exp_linear.</param>
    public ExponentialCostParameters(double exp_fixed, double exp_linear)
    {
      this.m_exp_fixed = exp_fixed;
      this.m_exp_linear = exp_linear;
      //this.m_exp_power = exp_power;
    }

    #endregion Constructors --------------------------------------------------

  }

  /// <summary>
  /// Quadratic cost parameters
  /// </summary>
  [Serializable]
  public class QuadraticCostParameters
  {

    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Q_fixed
    /// var: c
    /// eenheid: EUR
    /// domein: l (segments)
    /// </summary>
    private double m_q_fixed;
    /// <summary>
    /// Q_linear
    /// var: b
    /// eenheid: EUR/cm
    /// domein: l (segments)
    /// </summary>
    private double m_q_linear;
    /// <summary>
    /// Q_quad
    /// var: a
    /// eenheid: EUR/cm^2
    /// domein: l (segments)
    /// </summary>
    private double m_q_quad;

    #endregion Instance Variables --------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets or sets the Q_fixed.
    /// </summary>
    /// <value>
    /// Q_fixed
    /// var: c
    /// eenheid: EUR
    /// domein: l (segments)
    /// </value>
    public double Q_fixed
    {
      get { return this.m_q_fixed; }
      set { this.m_q_fixed = value; }
    }

    /// <summary>
    /// Gets or sets the Q_linear.
    /// </summary>
    /// <value>
    /// Q_linear
    /// var: b
    /// eenheid: EUR/cm
    /// domein: l (segments)
    /// </value>
    public double Q_linear
    {
      get { return this.m_q_linear; }
      set { this.m_q_linear = value; }
    }

    /// <summary>
    /// Gets or sets the Q_quad.
    /// </summary>
    /// <value>
    /// Q_quad
    /// var: a
    /// eenheid: EUR/cm^2
    /// domein: l (segments)
    /// </value>
    public double Q_quad
    {
      get { return this.m_q_quad; }
      set { this.m_q_quad = value; }
    }

    #endregion Properties ----------------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="QuadraticCostParameters"/> class.
    /// </summary>
    public QuadraticCostParameters() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuadraticCostParameters"/> class.
    /// </summary>
    /// <param name="q_fixed">The q_fixed.</param>
    /// <param name="q_linear">The q_linear.</param>
    /// <param name="q_quad">The q_quad.</param>
    public QuadraticCostParameters(double q_fixed, double q_linear, double q_quad)
    {
      this.m_q_fixed = q_fixed;
      this.m_q_linear = q_linear;
      this.m_q_quad = q_quad;
    }

    #endregion Constructors --------------------------------------------------

  }
}
