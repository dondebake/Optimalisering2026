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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Forms/LegendaForm.cs 2     18/06/08 14:11 Ansink $
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

using OptimaliseRing.General;
using OptimaliseRing.Core;
using System.Threading;
using System.Globalization;

namespace OptimaliseRing.UI
{
  /// <summary>
  ///
  /// </summary>
  public partial class LegendaForm : Form
  {
    private MainForm m_Berekening;
    private int m_Jaar;

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:LegendaForm"/> class.
    /// </summary>
    /// <param name="mainForm">Pointer naar het hoofdformulier.</param>
    public LegendaForm(MainForm mainForm)
    {
      this.m_Berekening = mainForm;
      InitializeComponent();

      this.Top = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Top", "0").ToString());
      this.Left = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Left", "0").ToString());

      if (this.cmbLegendaKeuze.Items.Count == 0)
      {
        LoadLegendas();
      }

      Draw();
    }

    private void LoadLegendas()
    {
      bool stop = false;
      this.cmbLegendaKeuze.Items.Clear();
      ThisAppLegendaItems2.Instance.Clear();
      int legendaIndex = 0;

      CultureInfo cultureInfo = new CultureInfo(ThisAppProfile.Instance.GetValue("OptimaliseRing", "Taal", "en-GB"));

      while (!stop)
      {
        string section = string.Format("LegendaItems.{0}", legendaIndex);
        string entry = string.Format("Legenda:{0}", cultureInfo.Name);
        string item = ThisAppProfile.Instance.GetValue(section, entry, "NO_MORE_LEGENDASECTIONS");

        stop = item == "NO_MORE_LEGENDASECTIONS";
        if (!stop)
        {
          this.InitializeLegendaItems(legendaIndex++);
          this.cmbLegendaKeuze.Items.Add(item);
        }
      }

      int legendaType = ConvertString.ToInt32(
        ThisAppProfile.Instance.GetValue("OptimaliseRing", "LegendaType", 0.ToString()));

      if (this.cmbLegendaKeuze.Items.Count > legendaType)
      {
        this.cmbLegendaKeuze.SelectedIndex = legendaType;
      }
    }

    /// <summary>
    /// Sets the zicht jaar.
    /// </summary>
    /// <value>The zicht jaar.</value>
    public int Jaar
    {
      set
      {
        this.m_Jaar = value;
        this.Draw();
      }
    }

    private string Section
    {
      get
      {
        return string.Format("LegendaItems.{0}", this.cmbLegendaKeuze.SelectedIndex);
      }
    }

    private SortedList AppLegendaItems
    {
      get
      {
        return ThisAppLegendaItems2.Instance[this.cmbLegendaKeuze.SelectedIndex];
      }
    }

    /// <summary>
    /// Initialiseer the legenda items.
    /// </summary>
    private void InitializeLegendaItems(int index)
    {

      SortedList legendaItems = new SortedList();
      if (ThisAppLegendaItems2.Instance.Count -1 > index)
      {
        legendaItems = ThisAppLegendaItems2.Instance[index];
        legendaItems.Clear();
      }

      bool doorgaan = true;
      double minimum = 1.0;
      while (doorgaan)
      {
        string entry = "LegendaItem" + (legendaItems.Count + 1).ToString();
        string section = string.Format("LegendaItems.{0}", index);
        string item = ThisAppProfile.Instance.GetValue(section, entry, "NO_MORE_LEGENDAITEMS");
        if (item.Length > 0)
        {
          if (string.Compare(item, "NO_MORE_LEGENDAITEMS") == 0)
          {
            doorgaan = false;
          }
          else
          {
            string[] items = item.Split("|".ToCharArray());

            if (items.Length > 0)
            {
              LegendaItem legendaItem = new LegendaItem();

              legendaItem.Min = minimum;
              legendaItem.Max = ConvertString.ToDouble(items[0].ToString());

              if (legendaItem.Max == 0.0)
              {
                legendaItem.Max = Double.MaxValue;
              }
              int a = ConvertString.ToInt32(items[1].ToString());
              int r = ConvertString.ToInt32(items[2].ToString());
              int g = ConvertString.ToInt32(items[3].ToString());
              int b = ConvertString.ToInt32(items[4].ToString());

              legendaItem.Color = Color.FromArgb(a, r, g, b);

              legendaItems.Add(legendaItems.Count, legendaItem);
              minimum = legendaItem.Max;
            }
          }
        }
      }
      ThisAppLegendaItems2.Instance.Add(index, legendaItems);
    }

    /// <summary>
    /// Saves this instance.
    /// </summary>
    public void Save()
    {
      for (int i = 0; i < this.AppLegendaItems.Count; i++)
      {
        LegendaItem legendaItem = (LegendaItem)this.AppLegendaItems.GetByIndex(i);

        string item = "0";
        // Maximum bewaren
        if (legendaItem.Max != Double.MaxValue)
        {
          item = legendaItem.Max.ToString();
        }

        // ook nog de kleur bewaren
        item += "|" + legendaItem.Color.A.ToString()
             + "|" + legendaItem.Color.R.ToString()
             + "|" + legendaItem.Color.G.ToString()
             + "|" + legendaItem.Color.B.ToString();

        string entry = "LegendaItem" + (i + 1).ToString();
        ThisAppProfile.Instance.SetValue(this.Section, entry, item);
      }

      // Opnieuw tekenen van de legenda
      Draw();
    }

    /// <summary>
    /// Draws this instance.
    /// </summary>
    public void Draw()
    {
      if (this.AppLegendaItems != null)
      {
        this.BackColor = Color.White;
        if (this.AppLegendaItems.Count > 0)
        {
          this.Height = (this.AppLegendaItems.Count + 2) * 16 + this.uxButtonPanel.Height + 28 + this.toolStrip1.Height;
        }
        Bitmap bitmap = CreateBitmap();
        this.picLegend.Image = bitmap;

        this.picLegend.Dock = DockStyle.Fill;
      }
    }

    /// <summary>
    /// Creates the bitmap.
    /// </summary>
    /// <returns></returns>
    private Bitmap CreateBitmap()
    {
      Bitmap bitmap = new Bitmap(this.Width, this.Height);

      Graphics graphics = Graphics.FromImage(bitmap);
      DrawLegenda(graphics, new System.Drawing.Point(0, 0));
      graphics.Dispose();
      return bitmap;
    }

    /// <summary>
    /// Draw the legenda
    /// </summary>
    private void DrawLegenda(Graphics graphics, System.Drawing.Point pt)
    {
      float rectangleWidth = 14;

      Font font = new Font(new FontFamily("Verdana"), 8, FontStyle.Bold);

      // Save the GraphicsState.
      GraphicsState graphicsState = graphics.Save();

      // Set the PageUnit to Millimeter because all of our measurements are in millimeter.
      graphics.PageUnit = GraphicsUnit.Millimeter;

      // Set the PageScale to 1, so an millimeter will represent a true millimeter.
      graphics.PageScale = 1;

      SolidBrush brushBG = new SolidBrush(Color.White);
      SolidBrush brushFG = new SolidBrush(Color.Black);

      float xStart = pt.X;
      float yStart = pt.Y;

      PointF pointf = new PointF();

      float fTextHeight = graphics.MeasureString("O", font).Height;

      // Vaste tekst
      pointf.X = xStart + 2;
      pointf.Y = yStart + fTextHeight / 2;

      ViewMode weergave = m_Berekening.mapControlMain.Weergave;

      switch (weergave)
      {
        case ViewMode.Overstromingskans:
          graphics.DrawString(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "OptimaleOverstromingskans", "OptimaleOverstromingskans"), font, brushFG, pointf);
          break;
        case ViewMode.Overschrijdingskans:
          graphics.DrawString(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "OptimaleOverschrijdingskans", "OptimaleOverschrijdingskans"), font, brushFG, pointf);
          break;
        default:
          graphics.DrawString(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "OptimaleOverstromingskans", "OptimaleOverstromingskans"), font, brushFG, pointf);
          break;
      }

      pointf.Y += fTextHeight;

      graphics.DrawString(string.Format("in {0} [1/{1}]", this.m_Jaar, ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Jaar", "Jaar")), font, brushFG, pointf);
      pointf.Y += fTextHeight;

      // Legendaitems
      xStart = pointf.X;
      yStart = pointf.Y;
      for (int i = this.AppLegendaItems.Count - 1; i >= 0; i--)
      {
        LegendaItem legendaItem = (LegendaItem)this.AppLegendaItems.GetByIndex(i);

        SolidBrush brushColor = new SolidBrush(legendaItem.Color);

        pointf.X = xStart + rectangleWidth;
        pointf.Y = yStart + fTextHeight / 2;

        graphics.DrawString(legendaItem.Text, font, brushFG, pointf);

        graphics.FillRectangle(brushColor, xStart + 2, yStart + +fTextHeight / 2, rectangleWidth - 4, fTextHeight);

        yStart += fTextHeight;
      }

      // Restore the GraphicsState.
      graphics.Restore(graphicsState);
    }

    /// <summary>
    /// Handles the FormClosing event of the LegendaForm control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
    private void OnLegendaFormFormClosing(object sender, FormClosingEventArgs e)
    {
      ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Top", this.Top.ToString());
      ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Left", this.Left.ToString());

      if (e.CloseReason == CloseReason.UserClosing)
      {
        this.m_Berekening.ShowLegenda(false);
      }
    }

    /// <summary>
    /// Handles the Click event of the btnBewerken control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnBtnBewerkenClick(object sender, EventArgs e)
    {
      try
      {
        OptimaliseRing.UI.Forms.LegendaItemsForm legendaItemsForm = new OptimaliseRing.UI.Forms.LegendaItemsForm(this.cmbLegendaKeuze.SelectedIndex);
        legendaItemsForm.Icon = this.Icon;
        if (legendaItemsForm.ShowDialog(this) == DialogResult.OK)
        {
          this.Save();
          this.m_Berekening.mapControlMain.Refresh();
        }
      }
      catch (Exception appex)
      {
        ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
      }
    }

    private void OnCmbLegendaKeuzeSelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        if (this.cmbLegendaKeuze.SelectedIndex > -1)
        {
          ThisAppProfile.Instance.SetValue("OptimaliseRing", "LegendaType"
            , this.cmbLegendaKeuze.SelectedIndex.ToString());

          this.m_Berekening.mapControlMain.Refresh();
        }
        Draw();
      }
      catch (Exception)
      {

        throw;
      }
    }

  }
}
