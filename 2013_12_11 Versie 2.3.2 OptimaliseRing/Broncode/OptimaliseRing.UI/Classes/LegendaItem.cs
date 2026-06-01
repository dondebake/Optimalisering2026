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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Classes/LegendaItem.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Text;

namespace OptimaliseRing.UI
{
  /// <summary>
  /// Legenda item class
  /// </summary>
  public class LegendaItem
  {
    #region Instance Variables -----------------------------------------------

    private double min;               // Minimum waarde
    private double max;               // Maximum waarde
    private Color colorColor;         // Legenda kleur
    private UInt32  uInt32Color;      // Legenda kleur

    #endregion Instance Variables --------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:LegendaItem"/> class.
    /// </summary>
    public LegendaItem()
    {
    }

    #endregion Constructors --------------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Minimum waarde
    /// </summary>
    /// <value>Minimum waarde</value>
    public double Min
    {
      get { return this.min; }
      set { this.min = value; }
    }

    /// <summary>
    /// Maximum waarde
    /// </summary>
    /// <value>Maximum waarde</value>
    public double Max
    {
      get { return this.max; }
      set { this.max = value; }
    }

    /// <summary>
    /// Legendatekst
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
      get
      {
        string t = "";
        if (this.min == 1.0)
        {
          t = string.Format("> 1/{0}", this.max);
        }
        else if (this.max == Double.MaxValue )
        {
          t = string.Format("< 1/{0}", this.min);
        }
        else
        {
          t = string.Format("1/{0} - 1/{1}", this.max, this.min);
        }
        return t;
      }
    }

    /// <summary>
    /// Legendakleur
    /// </summary>
    /// <value>Legendakleur</value>
    public Color Color
    {
      get { return this.colorColor; }
      set
      {
        this.colorColor = value;
        this.uInt32Color = ColorToUInt32(this.colorColor);
      }
    }

    /// <summary>
    /// Legendakleur
    /// </summary>
    /// <value>Legendakleur</value>
    public UInt32 UInt32Color
    {
      get { return this.uInt32Color; }
      set
      {
        this.uInt32Color = value;
        this.colorColor = UInt32ToColor(this.uInt32Color);
      }
    }

    #endregion Properties ----------------------------------------------------

    #region Member functions -------------------------------------------------

    #endregion Member functions ----------------------------------------------

    /// <summary>
    /// Converteer een UInt32 kleur naar een System.Drawing.Color
    /// </summary>
    /// <param name="color">The UInt32color.</param>
    /// <returns>System.Drawing.Color</returns>
    private Color UInt32ToColor(UInt32 color)
    {
      string hex = color.ToString("X6");

      byte a = 255;
      byte b = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
      byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
      byte r = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

      Color.FromArgb(a, r, g, b);

      return Color.FromArgb(a, r, g, b);
    }

    /// <summary>
    /// Converteer een System.Drawing.Color naar UInt32 kleur
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns></returns>
    private UInt32 ColorToUInt32(Color color)
    {
      string hex = ToHex(color);

      UInt32 kleur = uint.Parse(hex, System.Globalization.NumberStyles.HexNumber);

      return kleur;
    }

    /// <summary>
    /// Converteer naar hexadecimaal
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns></returns>
    private string ToHex(Color color)
    {
      return string.Format("{0:X2}{1:X2}{2:X2}", color.B, color.G, color.R);
    }


  }
}
