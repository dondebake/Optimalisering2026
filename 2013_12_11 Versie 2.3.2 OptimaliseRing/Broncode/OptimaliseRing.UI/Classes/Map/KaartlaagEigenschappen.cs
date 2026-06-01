#region Copyright -------------------------------------------------------
// Copyright © 2008, Rijkswaterstaat/DWW & ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : Calamiteitenmodel Noordzeekanaal-Amsterdam-Rijnkanaal
//
// Author(s)  : Johan Ansink, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Classes/Map/KaartlaagEigenschappen.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using MapObjectsLT2;

namespace OptimaliseRing.UI
{
   /// <summary>
   ///
   /// </summary>
   public partial class KaartlaagEigenschappenForm :  Form
   {
      private OptimaliseRing.Profile.Profile m_Profile;
      private MapLayer m_MapLayer = null;
      private ImageLayer m_ImageLayer = null;
      private Color m_ForeColor;

      /// <summary>
      /// Initializes a new instance of the <see cref="T:KaartlaagEigenschappenForm"/> class.
      /// </summary>
      /// <param name="profile">The profile.</param>
      /// <param name="layer">The layer.</param>
      public KaartlaagEigenschappenForm(OptimaliseRing.Profile.Profile profile, Object layer)
      {
         InitializeComponent();

         this.m_Profile = profile;

         if (layer is MapLayer)
         {
            this.m_MapLayer = (MapLayer)layer;
         }
         else
         {
            this.m_ImageLayer = (ImageLayer)layer;
         }

         Initialiseer();

         this.mapControl.ToolbarVisible = false;

      }

      private void InitializeMarkerStyles()
      {
         this.cboMarkerStyle.Items.Clear();
         bool doorgaan = true;

         CultureInfo cultureInfo = ThisAppCulture.Instance;

         while (doorgaan)
         {
            string entry = "MarkerStyle" + (this.cboMarkerStyle.Items.Count + 1).ToString();
            string item = this.m_Profile.GetValue("MarkerStyles:" + cultureInfo.Name, entry, "NO_MORE_STYLES");
            if (item.Length > 0)
            {
               if (string.Compare(item, "NO_MORE_STYLES") == 0)
               {
                  doorgaan = false;
               }
               else
               {
                  this.cboMarkerStyle.Items.Add(item);
               }
            }
         }

         if ((this.cboMarkerStyle.Items.Count > 0) && (this.m_MapLayer.Symbol.Style < this.cboMarkerStyle.Items.Count))
         {
            if (this.m_MapLayer != null)
            {
               this.cboMarkerStyle.SelectedIndex = this.m_MapLayer.Symbol.Style;
            }
         }

         DisplayLayer();
      }

      private void InitializeFillStyles()
      {
         CultureInfo cultureInfo = ThisAppCulture.Instance;

         this.cboVulstijl.Items.Clear();
         bool doorgaan = true;

         while (doorgaan)
         {
            string entry = "FillStyle" + (this.cboVulstijl.Items.Count + 1).ToString();
            string item = this.m_Profile.GetValue("FillStyles:" + cultureInfo.Name, entry, "NO_MORE_STYLES");
            if (item.Length > 0)
            {
               if (string.Compare(item, "NO_MORE_STYLES") == 0)
               {
                  doorgaan = false;
               }
               else
               {
                  this.cboVulstijl.Items.Add(item);
               }
            }
         }
         // TODO plaatje als layer!!!
         if ((this.cboVulstijl.Items.Count > 0) && (this.m_MapLayer.Symbol.Style < this.cboVulstijl.Items.Count))
         {
            if (this.m_MapLayer != null)
            {
               this.cboVulstijl.SelectedIndex = this.m_MapLayer.Symbol.Style;
            }
         }

         DisplayLayer();
      }

      private void DisplayLayer()
      {
         // Alle layers weggooien
         this.mapControl.Canvas.Layers.Clear();
         // De gegeven layer toevoegen
         if (this.m_MapLayer != null)
         {
            this.mapControl.AddLayer(this.m_MapLayer);
         }
         else if (this.m_ImageLayer != null)
         {
            this.mapControl.AddLayer(this.m_ImageLayer);
         }
      }

      /// <summary>
      /// Handles the Click event of the buttonOK control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
      private void OnButtonOKClick(object sender, EventArgs e)
      {
         DialogResult = DialogResult.OK;
      }

      /// <summary>
      /// Handles the Click event of the buttonCancel control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
      private void OnButtonCancelClick(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
      }

      private void OnCboLijnstijlSelectedIndexChanged(object sender, EventArgs e)
      {
         if (this.m_MapLayer != null)
         {
            // Verander de kleur van het symbool van de enige layer in de map
            this.m_MapLayer.Symbol.Style = Convert.ToInt16(this.cboLijnstijl.SelectedIndex);
            // Beeld de kaart opnieuw af
            DisplayLayer();
         }
      }


      /// <summary>
      /// Handles the SelectedColorChanged event of the cboKleur control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
      private void OnCboKleurSelectedColorChanged(object sender, EventArgs e)
      {
         if (this.m_MapLayer != null)
         {
            // Verander de kleur van het symbool van de enige layer in de map
            this.m_MapLayer.Symbol.Color = OptimaliseRing.UI.ColorConverter.ColorToUInt32(this.cboKleur.Color);
            // Beeld de kaart opnieuw af
            DisplayLayer();
         }
      }

      private void OnCboVulstijlSelectedIndexChanged(object sender, EventArgs e)
      {
         if (this.m_MapLayer != null)
         {
            // Verander de vulstijl van het symbool van de enige layer in de map
            this.m_MapLayer.Symbol.Style = Convert.ToInt16(this.cboVulstijl.SelectedIndex);
            // Beeld de kaart opnieuw af
            DisplayLayer();
         }
      }

      private void OnCboMarkerStyleSelectedIndexChanged(object sender, EventArgs e)
      {
         if (this.m_MapLayer != null)
         {
            // Verander de markerstyle van het symbool van de enige layer in de map
            this.m_MapLayer.Symbol.Style = Convert.ToInt16(this.cboMarkerStyle.SelectedIndex);
            // Beeld de kaart opnieuw af
            DisplayLayer();
         }
      }

      private void OnCboLijndikteSelectedIndexChanged(object sender, EventArgs e)
      {
         if (this.m_MapLayer != null)
         {
            // Verander de vulstijl van het symbool van de enige layer in de map
            this.m_MapLayer.Symbol.Size = Convert.ToInt16(this.cboLijndikte.SelectedIndex + 1);
            // Beeld de kaart opnieuw af
            DisplayLayer();
         }
      }

      private void InitializeLineStyles()
      {
         this.cboLijnstijl.Items.Clear();

         this.cboLijnstijl.Items.Add("—————————————————————————————");
         this.cboLijnstijl.Items.Add("— — — — — — — — — — — — — — — — — — — —");
         this.cboLijnstijl.Items.Add("· · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · · ·");
         this.cboLijnstijl.Items.Add("— · — · — · — · — · — · — · — · — · — · — · — · — · — ·");
         this.cboLijnstijl.Items.Add("— · · — · · — · · — · · — · · — · · — · · — · · — · · — · ·");

         if ((this.cboLijnstijl.Items.Count > 0) && (this.m_MapLayer.Symbol.Style < this.cboLijnstijl.Items.Count))
         {
            if (this.m_MapLayer != null)
            {
               this.cboLijnstijl.SelectedIndex = this.m_MapLayer.Symbol.Style;
            }
         }

      }
      private void InitializeLineWidths()
      {
         this.cboLijndikte.Items.Clear();

         this.cboLijndikte.Items.Add("1 pt");
         this.cboLijndikte.Items.Add("2 pt");
         this.cboLijndikte.Items.Add("3 pt");
         this.cboLijndikte.Items.Add("4 pt");
         this.cboLijndikte.Items.Add("5 pt");

         if ((this.cboLijndikte.Items.Count > 0) && (this.m_MapLayer.Symbol.Size < this.cboLijndikte.Items.Count))
         {
            if (this.m_MapLayer != null)
            {
               this.cboLijndikte.SelectedIndex = this.m_MapLayer.Symbol.Size - 1;
            }
         }
      }

      /// <summary>
      /// Initialiseer
      /// </summary>
      private void Initialiseer()
      {
         if (this.m_MapLayer != null)
         {
            InitializeFillStyles();
            InitializeMarkerStyles();
            InitializeLineWidths();
            InitializeLineStyles();

            this.lblKleur.Visible = true;
            this.cboKleur.Visible = true;

            // Als het een map layer is, dan de gebruiker de mogelijkheid
            // geven om de symboolkleur en vulstijl van de map aan te passen
            switch (this.m_MapLayer.ShapeType)
            {
               case ShapeTypeConstants.moShapeTypePoint:
               case ShapeTypeConstants.moShapeTypeMultipoint:

                  this.lblLijnstijl.Visible = false;
                  this.cboLijnstijl.Visible = false;
                  this.lblVulstijl.Visible = false;
                  this.cboVulstijl.Visible = false;
                  this.cboLijndikte.Visible = false;
                  this.lblLijndikte.Visible = false;
                  this.lblMarkerStyle.Visible = true;
                  this.cboMarkerStyle.Visible = true;

                  this.cboMarkerStyle.SelectedIndex = this.m_MapLayer.Symbol.Style;
                  break;

               case ShapeTypeConstants.moShapeTypeLine:

                  this.lblLijnstijl.Visible = true;
                  this.cboLijnstijl.Visible = true;
                  this.lblVulstijl.Visible = false;
                  this.cboVulstijl.Visible = false;
                  this.cboLijndikte.Visible = true;
                  this.lblLijndikte.Visible = true;
                  this.lblMarkerStyle.Visible = false;
                  this.cboMarkerStyle.Visible = false;

                  this.cboLijnstijl.SelectedIndex = this.m_MapLayer.Symbol.Style;
                  this.cboLijndikte.SelectedIndex = Math.Max(0, this.m_MapLayer.Symbol.Size - 1);
                  break;

               case ShapeTypeConstants.moShapeTypePolygon:
               case ShapeTypeConstants.moShapeTypeRectangle:
               case ShapeTypeConstants.moShapeTypeEllipse:

                  this.lblLijnstijl.Visible = false;
                  this.cboLijnstijl.Visible = false;
                  this.lblVulstijl.Visible = true;
                  this.cboVulstijl.Visible = true;
                  this.cboLijndikte.Visible = true;
                  this.lblLijndikte.Visible = true;

                  this.lblMarkerStyle.Visible = false;
                  this.cboMarkerStyle.Visible = false;

                  this.cboVulstijl.SelectedIndex = this.m_MapLayer.Symbol.Style;
                  this.cboLijndikte.SelectedIndex = Math.Max(0, this.m_MapLayer.Symbol.Size - 1);
                  break;
            }

            this.filenameLabel.Text = this.m_MapLayer.Name;
            this.m_ForeColor = OptimaliseRing.UI.ColorConverter.UInt32ToColor(m_MapLayer.Symbol.Color);
            this.cboKleur.Color = this.m_ForeColor;
         }

         // Image layer alleen tonen
         if (this.m_ImageLayer != null)
         {
            this.lblLijnstijl.Visible = false;
            this.cboLijnstijl.Visible = false;
            this.lblKleur.Visible = false;
            this.cboKleur.Visible = false;
            this.lblVulstijl.Visible = false;
            this.cboVulstijl.Visible = false;
            this.lblMarkerStyle.Visible = false;
            this.cboMarkerStyle.Visible = false;
            this.cboLijndikte.Visible = false;
            this.lblLijndikte.Visible = false;

            this.filenameLabel.Text = this.m_ImageLayer.Name;

            DisplayLayer();
         }
      }

   }
}
