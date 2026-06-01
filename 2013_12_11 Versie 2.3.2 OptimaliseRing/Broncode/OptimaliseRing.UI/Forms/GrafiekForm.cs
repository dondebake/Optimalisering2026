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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Forms/GrafiekForm.cs 2     18/06/08 14:11 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

using OptimaliseRing.General;
using OptimaliseRing.Core;

using CenterSpace.NMath.Core;

using Steema.TeeChart;
using OptimaliseRing.Aimms;
using System.Collections.Generic;

namespace OptimaliseRing.UI.Forms
{
  /// <summary>
  ///
  /// </summary>
  public partial class GrafiekForm : Form
  {
    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:GrafiekForm"/> class.
    /// </summary>
    /// <param name="kansen">The kansen.</param>
    public GrafiekForm(Berekening berekening, Kansen kansen)
    {
      InitializeComponent();

      this.Top = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Top", "0").ToString());
      this.Left = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Left", "0").ToString());
      this.Width = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Width", "640").ToString());
      this.Height = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Height", "480").ToString());

      this.DrawAimms(kansen, berekening);

    }

    /// <summary>
    /// Gets the tee chart.
    /// </summary>
    /// <value>The tee chart.</value>
    public Steema.TeeChart.TChart TeeChart
    {
      get
      {
        return this.tchGrafiek;
      }
    }

    /// <summary>
    /// Gets the kleuren lijst.
    /// </summary>
    /// <param name="n">The n.</param>
    /// <returns></returns>
    private SortedList<int, Color> GetKleurenLijst(int n)
    {
      SortedList<int, Color> retval = new SortedList<int, Color>();

      for (int i = 1; i <= n; i++)
      {
        // x = 2i-1/2n
        double x = (2.0 * Convert.ToDouble(i) - 1.0) / (2.0 * Convert.ToDouble(n));

        //f(x) rood/groen/blauw
        int red = Convert.ToInt32((0.0 <= x && x < 0.2 ? (0.5 + 2.5 * x) : 0.2 <= x && x < 0.75 ? 1 : (1 - (x - 0.75) / 0.25)) * 255.0);
        int green = Convert.ToInt32((0.0 <= x && x < 0.2 ? 0.0 : 0.2 <= x && x < 0.575 ? ((x - 0.2) / 0.375) : 1.0) * 255.0);
        int blue = Convert.ToInt32((0.0 <= x && x < 0.575 ? 0 : 0.575 <= x && x < 0.75 ? (((x - 0.575) / 0.175) * (3 / 4) * 0.575) : 0.75 <= x && x < 0.8125 ? 0.75 : (0.75 - ((x - 0.8125) / 0.25))) * 255.0);

        Console.WriteLine(string.Format("R:{0,10} G:{1,10} B:{2,10}   [x:{3}]", Math.Max(0, Math.Min(255, red)), Math.Max(0, Math.Min(255, green)), Math.Max(0, Math.Min(255, blue)), x));
        retval.Add(i, Color.FromArgb(Math.Max(0, Math.Min(255, red)), Math.Max(0, Math.Min(255, green)), Math.Max(0, Math.Min(255, blue))));
      }

      return retval;
    }

    /// <summary>
    /// Draws the aimms.
    /// </summary>
    /// <param name="kansen">The kansen.</param>
    /// <param name="berekening">The berekening.</param>
    private void DrawAimms(Kansen kansen, Berekening berekening)
    {

      if (kansen.MatrixData != null)
      {
        CultureInfo cultureInfo = ThisAppCulture.Instance;

        DijkringDeel dijkringDeel = berekening.GetDijkringDeel(kansen);

        // rekenen met parameteronzekerheid?
        bool rekenenMetParameteronzekerheid = berekening.Scenarioparameters.Count > 0;

        if (dijkringDeel != null)
        {

          double maxJaar = ConvertString.ToDouble(ThisAppProfile.Instance.GetValue("Parameters", "JaartalFiguur").ToString());

          this.tchGrafiek.Aspect.View3D = false;

          this.tchGrafiek.Header.AdjustFrame = false;
          this.tchGrafiek.Header.Lines = new string[3];
          if (kansen.Deel.Length > 0)
          {
            this.tchGrafiek.Header.Lines[0] = string.Format("{0} - {1} - {2}", dijkringDeel.DijkringId, dijkringDeel.DijkringNaam, dijkringDeel.DeelNummer.ToString());
          }
          else
          {
            this.tchGrafiek.Header.Lines[0] = string.Format("{0} - {1}", dijkringDeel.DijkringId, dijkringDeel.DijkringNaam);
          }
          this.tchGrafiek.Legend.Visible = true;

          this.tchGrafiek.Axes.Bottom.Title.Text = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "AxisJaar", "AxisJaar").ToString();

          this.tchGrafiek.Axes.Left.Title.Text = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Overstromingskans").ToString();
          this.tchGrafiek.Axes.Left.Labels.ValueFormat = "#,#0.###00";

          this.tchGrafiek.Axes.Bottom.Labels.ValueFormat = "####";

          // Data toevoegen
          this.tchGrafiek.Series.Clear();

          bool investeringenAanwezig = false;

          // *******************************************************************************************************

          SortedList<double, double> p_Voorloop = new SortedList<double, double>();

          if (dijkringDeel.Trajecten.Count == 1)
          {
            // geen investering, lijn Pmidden niet zichtbaar
            investeringenAanwezig = dijkringDeel.Trajecten[0].Investeringen.Count > 0;
          }
          if (dijkringDeel.Trajecten.Count > 1)
          {
            SortedList<int, Color> kleurenLijst = GetKleurenLijst(dijkringDeel.Trajecten.Count);

            // Meerdere trajecten aanwezig (P PER TRAJECT)
            for (int i = 0; i < dijkringDeel.Trajecten.Count; i++)
            {
              // Ophalen traject
              DijkringTraject traject = (DijkringTraject)dijkringDeel.Trajecten[i];

              // geen investering, lijn Pmidden niet zichtbaar
              investeringenAanwezig = investeringenAanwezig || traject.Investeringen.Count > 0;

              Steema.TeeChart.Styles.Line line = new Steema.TeeChart.Styles.Line();
              this.tchGrafiek.Series.Add(line);

              if (kleurenLijst.ContainsKey(i + 1))
              {
                line.Color = kleurenLijst[i + 1];
              }

              // zet legendatekst
              this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Title = traject.Naam;

              line.Title = this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Title;
              line.LinePen.Width = 2;

              // Voorloopjaren ook afdrukken
              for (int vljaar = berekening.Kansjaar; vljaar <= berekening.ZichtJaar; vljaar++)
              {
                double x = vljaar;
                double y = 0.0;

                //if (rekenenMetParameteronzekerheid)
                //{


                //  // Eerste scenario gebruiken voor bepalen van de overstromingskans
                //  OptimaliseRing.Core.DijkringDeel.TrajectOnzekerheid trajectOnzekerheid
                //    = dijkringDeel.GetTrajectOnzekerheid(traject
                //    , berekening.Scenarioparameters[0].KlimaatScenarioEnFysischMaxAfvoer
                //    , vljaar
                //    , berekening.Kansjaar
                //    , berekening.FactorKans);
                //  y = trajectOnzekerheid.Initial_flood_probability;
                //}
                //else
                //{
                  // Overstromingskans bij rekenen zonder scenario's met meerdere trajecten
                  double overstromingskans = traject.P0Overstromingskans
                    * NMathFunctions.Exp(traject.AlphaOverstromingskans * traject.Eta
                    * (vljaar - berekening.Kansjaar));

                  y = overstromingskans * berekening.FactorKans * traject.Factor;
                //}

                if (p_Voorloop.ContainsKey(x))
                { p_Voorloop[x] = Math.Max(p_Voorloop[x], y); }
                else
                { p_Voorloop.Add(x, y); }

                //this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x - 1, y);
                this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x, y);
              }

              // ophalen matrixdata per traject
              for (int row = 0; row < kansen.MatrixData.Rows; row++)
              {
                // Jaartal
                double x = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.JAAR)];
                double y = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.AANTAL)
                  + dijkringDeel.Trajecten.IndexOf(traject)];
                this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x, y);
              }
            }
          }

          // *******************************************************************************************************
          // werkelijk (P) Maximale p over alle trajecten
          Steema.TeeChart.Styles.Line lineWerkelijk = new Steema.TeeChart.Styles.Line();
          this.tchGrafiek.Series.Add(lineWerkelijk);

          lineWerkelijk.Color = Color.Green;
          lineWerkelijk.Title = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "PWERKELIJK", "PWERKELIJK");
          lineWerkelijk.LinePen.Width = 2;

          if (dijkringDeel.Trajecten.Count == 1)
          {
            // Voorloopjaren ook afdrukken
            for (int vljaar = berekening.Kansjaar; vljaar <= berekening.ZichtJaar; vljaar++)
            {
              double x = vljaar;
              double y = 0.0;

              //dijkringDeel.Trajecten[0].P0Overschrijdingskans

              //if (rekenenMetParameteronzekerheid)
              //{
              //  // Eerste scenario gebruiken voor bepalen van de overstromingskans
              //  OptimaliseRing.Core.DijkringDeel.TrajectOnzekerheid trajectOnzekerheid
              //    = dijkringDeel.GetTrajectOnzekerheid(dijkringDeel.Trajecten[0]
              //    , berekening.Scenarioparameters[0].KlimaatScenarioEnFysischMaxAfvoer
              //    , vljaar
              //    , berekening.Kansjaar
              //    , berekening.FactorKans);
              //  y = trajectOnzekerheid.Initial_flood_probability;
              //}
              //else
              //{
                // Overstromingskans bij rekenen zonder scenario's met 1 traject
                double overstromingskans = dijkringDeel.Trajecten[0].P0Overstromingskans
                  * NMathFunctions.Exp(dijkringDeel.Trajecten[0].AlphaOverstromingskans
                  * dijkringDeel.Trajecten[0].Eta * (vljaar - berekening.Kansjaar));

                y = overstromingskans * berekening.FactorKans * dijkringDeel.Trajecten[0].Factor;
              //}

              //this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x - 1, y);
              this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x, y);
            }
          }
          else
          {
            foreach (KeyValuePair<double, double> keyValuePair in p_Voorloop)
            {
              //this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(keyValuePair.Key - 1, keyValuePair.Value);
              this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(keyValuePair.Key, keyValuePair.Value);
            }
          }

          for (int row = 0; row < kansen.MatrixData.Rows; row++)
          {
            double x = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.JAAR)];
            double y = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.P)];

            if (!Double.IsNaN(y))
            {
              if (!Double.IsInfinity(y))
              {
                this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x, y);
              }
            }
          }

          // *******************************************************************************************************
          // wettelijke norm overstromingskans (PWET)
          Steema.TeeChart.Styles.Line lineWettelijk = new Steema.TeeChart.Styles.Line();
          this.tchGrafiek.Series.Add(lineWettelijk);

          lineWettelijk.Color = Color.Black;
          lineWettelijk.Title = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "PWET", "PWET");
          lineWettelijk.LinePen.Style = DashStyle.Solid;
          lineWettelijk.LinePen.Width = 2;

          // Voorloopjaren ook afdrukken
          for (int vljaar = berekening.Kansjaar; vljaar < berekening.ZichtJaar; vljaar++)
          {
            double x = vljaar;
            double y = (double)kansen.MatrixData[0, Convert.ToInt32(MatrixKolom.PWET)];

            this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x, y);
          }

          for (int row = 0; row < kansen.MatrixData.Rows; row++)
          {
            double x = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.JAAR)];
            double y = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.PWET)];

            if (!Double.IsNaN(y))
            {
              if (!Double.IsInfinity(y))
              {
                this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x, y);
              }
            }
          }

          // *******************************************************************************************************
          // wettelijke norm overstromingskans (PRestrictie)
          if (berekening.Veiligheidsnorm == 2)
          {
            Steema.TeeChart.Styles.Line lineRestrictie = new Steema.TeeChart.Styles.Line();
            this.tchGrafiek.Series.Add(lineRestrictie);

            lineRestrictie.Color = Color.Purple;
            lineRestrictie.Title = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "PRESTRICTIE", "PRESTRICTIE");
            lineRestrictie.LinePen.Style = DashStyle.Solid;
            lineRestrictie.LinePen.Width = 2;

            // Voorloopjaren ook afdrukken
            for (int vljaar = berekening.Kansjaar; vljaar < berekening.ZichtJaar; vljaar++)
            {
              double x = vljaar;
              double y = (double)kansen.MatrixData[0, Convert.ToInt32(MatrixKolom.PRESTRICTIE)];

              this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x, y);
            }

            for (int row = 0; row < kansen.MatrixData.Rows; row++)
            {
              double x = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.JAAR)];
              double y = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.PRESTRICTIE)];

              if (!Double.IsNaN(y))
              {
                if (!Double.IsInfinity(y))
                {
                  this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x, y);
                }
              }
            }
          }

          // *******************************************************************************************************

          // Als er geen investeringen bij alle trajecten aanwezig zijn dan lijn P-midden niet tonen.
          if (investeringenAanwezig)
          {

            // midden van de optimale overstromingskans (PMIDDEN)
            Steema.TeeChart.Styles.Line lineMidden = new Steema.TeeChart.Styles.Line();
            this.tchGrafiek.Series.Add(lineMidden);

            lineMidden.Color = Color.Blue;
            lineMidden.Title = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "PMIDDEN", "PMIDDEN");
            lineMidden.LinePen.Style = DashStyle.Dot;
            lineMidden.LinePen.Width = 2;

            for (int row = 0; row < kansen.MatrixData.Rows; row++)
            {
              double x = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.JAAR)];
              double y = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.PMIDDEN)];

              if (System.Convert.ToInt32(x) >= berekening.ZichtJaar)
              {
                if (!Double.IsNaN(y))
                {
                  if (!Double.IsInfinity(y))
                  {
                    this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x, y);
                  }
                }
              }
            }

            if (rekenenMetParameteronzekerheid)
            {
              // *******************************************************************************************************
              // minimale midden van de optimale overstromingskans over de scenario's(PMIDDENMIN)
              Steema.TeeChart.Styles.Line lineMiddenMin = new Steema.TeeChart.Styles.Line();
              this.tchGrafiek.Series.Add(lineMiddenMin);

              lineMiddenMin.Color = Color.Brown;
              lineMiddenMin.Title = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "PMIDMIN", "PMIDMIN");
              lineMiddenMin.LinePen.Style = DashStyle.Dot;
              lineMiddenMin.LinePen.Width = 1;

              for (int row = 0; row < kansen.MatrixData.Rows; row++)
              {
                double x = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.JAAR)];
                double y = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.PMIDMIN)];

                if (System.Convert.ToInt32(x) >= berekening.ZichtJaar)
                {
                  if (!Double.IsNaN(y))
                  {
                    if (!Double.IsInfinity(y) && y != double.MaxValue)
                    {
                      this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x, y);
                    }
                  }
                }
              }

              // *******************************************************************************************************
              // maximale midden van de optimale overstromingskans over de scenario's(PMIDDENMAX)
              Steema.TeeChart.Styles.Line lineMiddenMax = new Steema.TeeChart.Styles.Line();
              this.tchGrafiek.Series.Add(lineMiddenMax);

              lineMiddenMax.Color = Color.Blue;
              lineMiddenMax.Title = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "PMIDMAX", "PMIDMAX");
              lineMiddenMax.LinePen.Style = DashStyle.Dot;
              lineMiddenMax.LinePen.Width = 1;

              //lineMiddenMax.ShowInLegend = false;

              for (int row = 0; row < kansen.MatrixData.Rows; row++)
              {
                double x = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.JAAR)];
                double y = (double)kansen.MatrixData[row, Convert.ToInt32(MatrixKolom.PMIDMAX)];

                if (System.Convert.ToInt32(x) >= berekening.ZichtJaar)
                {
                  if (!Double.IsNaN(y))
                  {
                    if (!Double.IsInfinity(y) && y != double.MinValue)
                    {
                      this.tchGrafiek.Series[this.tchGrafiek.Series.Count - 1].Add(x, y);
                    }
                  }
                }
              }
            }
          }

          // *******************************************************************************************************
          // As schalen
          SchaalAs(this.tchGrafiek.Axes.Left, 8);
          this.tchGrafiek.Axes.Left.Minimum = 0;

          this.tchGrafiek.Axes.Left.Increment = 1E-8;

          this.tchGrafiek.Axes.Bottom.Automatic = false;
          this.tchGrafiek.Axes.Bottom.Increment = 1;
          this.tchGrafiek.Axes.Bottom.Minimum = berekening.Kansjaar;
          this.tchGrafiek.Axes.Bottom.Maximum = maxJaar;

        }
      }
    }


    /// <summary>
    /// Schaals as.
    /// </summary>
    /// <param name="axis">The axis.</param>
    /// <param name="minNSteps">The min N steps.</param>
    private static void SchaalAs(Axis axis, long minNSteps)
    {
      double axMin;
      double axMax;
      double axStep;
      long maxLabelLength;

      if (axis.Horizontal)
      {
        AsSchalen(axis.MinXValue, axis.MaxXValue, minNSteps, 1.1, 1.1, out axMin, out axMax, out axStep, out maxLabelLength);
      }
      else
      {
        AsSchalen(axis.MinYValue, axis.MaxYValue, minNSteps, 1.1, 1.1, out axMin, out axMax, out axStep, out maxLabelLength);
      }
      axis.Automatic = false;
      axis.SetMinMax(axMin, axMax);
      axis.Increment = axStep;
      axis.Labels.CustomSize = (int)maxLabelLength * axis.Labels.Font.Size;
    }

    /// <summary>
    /// Bepaal 'nette' schalen voor het tekenen van een as.
    /// </summary>
    /// <param name="minimum">Minimum waarde die binnen de as moet vallen.</param>
    /// <param name="maximum">Maximum waarde die binnen de as moet vallen.</param>
    /// <param name="minNSteps">The min N steps.</param>
    /// <param name="minFactor">Factor waarmee de as aan de onderkant wordt verlengd.</param>
    /// <param name="maxFactor">Factor waarmee de as aan de bovenkant wordt verlengd.</param>
    /// <param name="axMinimum">'Nette' minimum waarde voor het tekenen van de as.</param>
    /// <param name="axMaximum">'Nette' maximum waarde voor het tekenen van de as.</param>
    /// <param name="axStep">'Nette' stap voor het tekenen van de as.</param>
    /// <param name="maxLabelLength">De lengte van het langste label.</param>
    private static void AsSchalen(double minimum, double maximum, long minNSteps
      , double minFactor, double maxFactor
      , out double axMinimum, out double axMaximum, out double axStep, out long maxLabelLength)
    {
      double diff = maximum - minimum;
      long richting;

      if (diff != 0)
      {
        richting = (long)(diff / Math.Abs(diff));
      }
      else
      {
        richting = 1;
        diff = 0.0000000001;
      }
      axMinimum = maximum - diff * minFactor;
      axMaximum = minimum + diff * maxFactor;
      diff = axMaximum - axMinimum;

      axStep = diff / minNSteps;

      double logDiff = Math.Log(Math.Abs(axStep)) / Math.Log(10);
      long factor = (int)Math.Floor(logDiff) + 1;
      axStep = axStep / Math.Pow(10, factor);
      if (axStep * richting >= 1)
      {
        axStep = richting;
      }
      else if (axStep * richting >= 0.5)
      {
        axStep = 0.5 * richting;
      }
      else if (axStep * richting >= 0.2)
      {
        axStep = 0.2 * richting;
      }
      else
      {
        axStep = 0.1 * richting;
      }
      axStep = axStep * Math.Pow(10, factor);

      axMinimum = (int)Math.Floor((axMinimum / axStep)) * axStep;
      axMaximum = (int)Math.Floor((axMaximum / axStep + 0.999)) * axStep;

      if (axMinimum == axMaximum)
      {
        axMaximum = axMinimum + 0.01 * axMinimum * richting;
        axMaximum = axMinimum - 0.01 * axMinimum * richting;
        axStep = Math.Abs(axMaximum - axMinimum);

      }

      maxLabelLength = Math.Max(axMinimum.ToString().Length, axMaximum.ToString().Length);
      maxLabelLength = Math.Max(maxLabelLength, axStep.ToString().Length);
    }

    /// <summary>
    /// Handles the Click event of the btnEdit control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void BtnEditClick(object sender, EventArgs e)
    {
      this.tchGrafiek.ShowEditor();
    }

    /// <summary>
    /// Handles the Click event of the btnCopy control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnCopyClick(object sender, EventArgs e)
    {
      try
      {
        // Prepare the drawing for copying to file or clipboard

        // Make the panel color white
        Color oldColor = this.tchGrafiek.Chart.Panel.Color;
        this.tchGrafiek.Chart.Panel.Color = Color.White;

        // Remove the inner and outer bevels
        Steema.TeeChart.Drawing.BevelStyles bevelInner = this.tchGrafiek.Chart.Panel.Bevel.Inner;
        Steema.TeeChart.Drawing.BevelStyles bevelOuter = this.tchGrafiek.Chart.Panel.Bevel.Outer;
        this.tchGrafiek.Chart.Panel.Bevel.Inner = Steema.TeeChart.Drawing.BevelStyles.None;
        this.tchGrafiek.Chart.Panel.Bevel.Outer = Steema.TeeChart.Drawing.BevelStyles.None;

        // Repaint the picture
        this.tchGrafiek.Refresh();

        // Show the export dialog to the user
        this.tchGrafiek.Chart.Export.ShowExportDialog();

        // Restore original values for color and bevel
        this.tchGrafiek.Chart.Panel.Color = oldColor;
        this.tchGrafiek.Chart.Panel.Bevel.Inner = bevelInner;
        this.tchGrafiek.Chart.Panel.Bevel.Outer = bevelOuter;

        // Repaint the picture
        this.tchGrafiek.Refresh();

      }
      catch (Exception appex)
      {
        ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
      }

    }

    /// <summary>
    /// Handles the Click event of the btnPrint control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnPrintClick(object sender, EventArgs e)
    {
      this.tchGrafiek.Printer.Landscape = true;
      this.tchGrafiek.Printer.Preview();
    }

    /// <summary>
    /// Handles the FormClosing event of the GrafiekForm control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
    private void OnGrafiekFormFormClosing(object sender, FormClosingEventArgs e)
    {
      ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Top", this.Top.ToString());
      ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Left", this.Left.ToString());
      ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Width", this.Width.ToString());
      ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Height", this.Height.ToString());
    }
  }
}
