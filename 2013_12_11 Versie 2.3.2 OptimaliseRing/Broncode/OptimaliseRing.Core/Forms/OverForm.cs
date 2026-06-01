#region Copyright -------------------------------------------------------
// Copyright © 2006, Rijkswaterstaat/RIZA & ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : 1142.10.00 Implementatie KBA-model
//              OptimaliseRing - Economische optimalisatie van veiligheidsniveaus van dijkringen
//
// Author(s)  : Johan Ansink, HKV lijn in water
//              Matthijs Duits, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Core/Forms/OverForm.cs 3     1/07/08 9:10 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace OptimaliseRing.Core
{
  public partial class OverForm : Form
  {
    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:OverForm"/> class.
    /// </summary>
    public OverForm(Profile.Ini profile)
    {
      InitializeComponent();

      // Description
      CultureInfo cultureInfo = new CultureInfo(profile.GetValue("OptimaliseRing", "Taal", "en-GB"));

      // Versie
      Version mgrVersion = Assembly.GetExecutingAssembly().GetName().Version;

      this.uxWhatLabel.Text = Application.ProductName;
      string email = profile.GetValue("OptimaliseRing", "EMAILinfo", "jarl.kind@deltares.nl");

      if (cultureInfo.Name == "nl-NL")
      {
        this.lblVersie.Text += mgrVersion.Major + "." + mgrVersion.Minor + "." + mgrVersion.Build;
        this.lblDescription.Text = "Economische optimalisatie van veiligheidsniveaus van dijkringen";
        this.lblOntwikkeld.Text = "Ontwikkeld door ®HKV lijn in water i.o.v. Deltares";
        this.lblInformatie.Text = "Voor meer informatie : " + email;
      }
      else if (cultureInfo.Name == "en-GB")
      {
        this.lblVersie.Text += mgrVersion.Major + "." + mgrVersion.Minor + "." + mgrVersion.Build;
        this.lblDescription.Text = "Economic optimization of safety standards for dike rings";
        this.lblOntwikkeld.Text = "Developed by ®HKV consultants i.c.w. Deltares";
        this.lblInformatie.Text = "For more details : " + email;
      }
      else
      {
        this.lblVersie.Text += mgrVersion.Major + "." + mgrVersion.Minor + "." + mgrVersion.Build;
        this.lblDescription.Text = "Economische optimalisatie van veiligheidsniveaus van dijkringen";
        this.lblOntwikkeld.Text = "Ontwikkeld door ®HKV lijn in water i.o.v. Deltares";
        this.lblInformatie.Text = "Voor meer informatie : " + email;
      }
      this.lblCopyrightLabel.Text = this.AssemblyCopyright;
    }

    /// <summary>
    /// Handles the Click event of the uxOkButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void uxOkButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Gets the assembly copyright.
    /// </summary>
    /// <value>The assembly copyright.</value>
    private string AssemblyCopyright
    {
      get
      {
        // Get all Copyright attributes on this assembly
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        // If there aren't any Copyright attributes, return an empty string
        if (attributes.Length == 0)
          return "";
        // If there is a Copyright attribute, return its value
        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
    }
  }
}
