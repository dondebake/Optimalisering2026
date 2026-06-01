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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Forms/MainForm.cs 7     28-04-09 14:02 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Resources;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using Microsoft.Reporting.WinForms;

using OptimaliseRing.General;
using OptimaliseRing.Core;
using OptimaliseRing.UI.Forms;

using MapObjectsLT2;


namespace OptimaliseRing.UI
{
  /// <summary>
  ///
  /// </summary>
  public partial class MainForm : Form, IStatus
  {
    #region Instance Variables -----------------------------------------------

    private LegendaForm m_LegendaForm = null;
    private Berekening m_Berekening;

    #endregion Instance Variables --------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:MainForm"/> class.
    /// </summary>
    public MainForm()
    {
      InitializeComponent();
    }

    #endregion Constructors --------------------------------------------------

    #region Menu -------------------------------------------------------------

    #region Bestand Menu -----------------------------------------------------

    /// <summary>
    /// Handles the Click event of the menuBestandWerkmap control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnMenuBestandWerkmapClick(object sender, EventArgs e)
    {
      string werkMap = OptimaliseRingDialog.Folder(
         Application.ProductName + " - " + ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name
         , "FolderBrowserDialogTitle", "")
         , ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FolderBrowserDialogDescription", "")
         , ThisAppWorkingDirectory.Instance.Werkmap
         , false
         );

      if (!string.IsNullOrEmpty(werkMap))
      {
        ThisAppProfile.Instance.SetValue("OptimaliseRing", "Werkmap", MyPath.RelativeName(werkMap));
        ThisAppWorkingDirectory.Instance.Werkmap = werkMap;
      }
    }

    /// <summary>
    /// Handles the Click event of the exitToolStripMenuItem control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnMenuBestandExitClick(object sender, EventArgs e)
    {
      BestandExit();
    }

    #endregion

    #region Bewerken Menu ----------------------------------------------------

    /// <summary>
    /// Handles the Click event of the menuBewerken control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void MenuBewerkenClick(object sender, EventArgs e)
    {
      if (sender == this.menuBewerkenSelectie)
      {
        this.mapControlMain.CurrentMode = PointerMode.Select;
      }
      else if (sender == this.menuBewerkenZoomIn)
      {
        this.mapControlMain.CurrentMode = PointerMode.ZoomIn;
      }
      else if (sender == this.menuBewerkenZoomUit)
      {
        this.mapControlMain.CurrentMode = PointerMode.ZoomOut;
      }
      else if (sender == this.menuBewerkenZoomGebied)
      {
        this.mapControlMain.CurrentMode = PointerMode.ZoomArea;
      }
      else if (sender == this.menuBewerkenSchuif)
      {
        this.mapControlMain.CurrentMode = PointerMode.Drag;
      }
      else if (sender == this.menuBewerkenCentreer)
      {
        this.mapControlMain.CurrentMode = PointerMode.Center;
      }
      else if (sender == this.menuBewerkenWereld)
      {
        this.mapControlMain.ZoomToExtent();
      }
      else if (sender == menuBewerkenKaartlagen)
      {
        this.mapControlMain.Kaartlagen();
        string kaartlagenMap = MyPath.RelativeName(this.mapControlMain.KaartlagenMap);
        ThisAppProfile.Instance.SetValue("OptimaliseRing", "AchtergrondShapes", kaartlagenMap);
      }
      else if (sender == this.mnuBewerkenLegenda)
      {
        ShowLegenda(this.mnuBewerkenLegenda.Checked);
      }
      else if (sender == this.menuBewerkenMapTips)
      {
        ShowMapTips(this.menuBewerkenMapTips.Checked);
      }
      else
      {
        this.mapControlMain.CurrentMode = PointerMode.Select;
      }

      OnPointerModeChanged(this, this.mapControlMain.CurrentMode);
    }

    #endregion

    #region Rekenen Menu

    /// <summary>
    /// Handles the Click event of the menuRekenenBerekeningen control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnMenuRekenenBerekeningenClick(object sender, EventArgs e)
    {
      try
      {
        StartBerekeningen();
      }
      catch (Exception appex)
      {
        ThisAppErr.Instance.Display(this, appex);
      }
      finally
      {
        this.mapControlMain.Canvas.Refresh();
      }
    }


    /// <summary>
    /// Called when [menu rekenen resultaten click].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void OnMenuRekenenResultatenClick(object sender, EventArgs e)
    {
      try
      {
        using (ResultatenForm resultatenForm = new ResultatenForm())
        {
          resultatenForm.Icon = this.Icon;

          if (resultatenForm.ShowDialog(this) == DialogResult.OK)
          {
            if (resultatenForm.BerekeningenMap != null)
            {
              m_Berekening.BerekeningenMap = resultatenForm.BerekeningenMap;

              try
              {
                if (Directory.Exists(m_Berekening.BerekeningenMap))
                {
                  // Lees gebruikte instellingen
                  m_Berekening.Instellingen.Read(Path.Combine(m_Berekening.BerekeningenMap, "Instellingen.xml"));
                  int optimaleOverstromingskansenJaar = resultatenForm.OptimaleOverstromingskansenJaar;
                  ResultatenUitvoer(false, optimaleOverstromingskansenJaar, true);
                }
              }
              catch
              {
                ThisAppErr.Instance.Raise(2, null);
              }
            }
          }

          this.mapControlMain.BerekeningAanwezig = Directory.Exists(m_Berekening.BerekeningenMap);

          if (this.m_LegendaForm != null)
          {
            m_LegendaForm.Jaar = m_Berekening.OptimaleOverstromingskansenJaar;
          }

          this.mapControlMain.Canvas.Refresh();
        }
      }
      catch (Exception appex)
      {
        ThisAppErr.Instance.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the Click event of the menuRekenencompartimentering control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnMenuRekenencompartimenteringClick(object sender, EventArgs e)
    {
      try
      {
        using (CompartimenteringForm compartimenteringForm = new CompartimenteringForm(
           ThisAppProfile.Instance, ThisAppLanguage.Instance, m_Berekening, ThisAppErr.Instance))
        {
          compartimenteringForm.Text = Application.ProductName + " - " + ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Compartimentering", "Compartimentering");
          if (compartimenteringForm.ShowDialog() == DialogResult.OK)
          {
            Refresh();
          }
        }
      }
      catch (Exception appex)
      {
        ThisAppErr.Instance.Display(this, appex);
      }
    }


    #endregion

    #region Help Menu

    /// <summary>
    /// Handles the Click event of the handleidingToolStripMenuItem control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnMnuHelpHandleidingClick(object sender, EventArgs e)
    {
      try
      {
        string handleiding = MyPath.AbsoluteName(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Handleiding", "Handleiding"));
        if (File.Exists(handleiding))
        {
          // Show
          Process myProcess = new Process();
          myProcess.StartInfo.FileName = handleiding;
          myProcess.StartInfo.Verb = "open";
          myProcess.StartInfo.CreateNoWindow = false;
          myProcess.Start();
          myProcess.Dispose();
        }
        else
        {
          ThisAppErr.Instance.Raise(1001, new object[] { handleiding });
        }
      }
      catch (Exception appex)
      {
        ThisAppErr.Instance.Display(this, appex);
      }

    }

    /// <summary>
    /// Handles the Click event of the mnuHelpOver control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnMnuHelpOverClick(object sender, EventArgs e)
    {
      using (OverForm overForm = new OverForm(ThisAppProfile.Instance))
      {
        overForm.Icon = this.Icon;
        overForm.Text = Application.ProductName + " - " + ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Over", "Over");
        overForm.ShowDialog(this);
      }
    }

    #endregion

    #endregion Menu ----------------------------------------------------------

    #region Event handling ---------------------------------------------------

    /// <summary>
    /// Called when [main form load].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void OnMainFormLoad(object sender, EventArgs e)
    {
      try
      {
        Initialize();

        this.Top = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Top", "0"));
        this.Left = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Left", "0"));
        this.Width = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Width", "640"));
        this.Height = ConvertString.ToInt32(ThisAppProfile.Instance.GetValue("Formulieren", this.Name + "Height", "480"));

        ShowLegenda(ConvertString.ToBoolean(ThisAppProfile.Instance.GetValue("OptimaliseRing", "Legenda", "False")));
        ShowMapTips(ConvertString.ToBoolean(ThisAppProfile.Instance.GetValue("OptimaliseRing", "MapTips", "False")));

        string kaartlagenMap = MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "AchtergrondShapes", ""));

        if (Directory.Exists(kaartlagenMap))
        {
          this.mapControlMain.KaartlagenMap = kaartlagenMap;
        }
      }
      catch (Exception appex)
      {
        ThisAppErr.Instance.Display(this, appex);
        this.Close();
      }
    }

    /// <summary>
    /// after layer draw event
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void OnAfterLayerDraw(object sender, AxMapObjectsLT2._DMapEvents_AfterLayerDrawEvent e)
    {
      // Alleen als de laatste layer is getekend
      if (e.index == 0)
      {
        // Teken de Systeem shapes
        DrawSysteemShapes();

        // Teken de dijkringen
        DrawDijkringen();
      }
    }

    /// <summary>
    /// pointer mode changed on map
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="mode">The mode.</param>
    private void OnPointerModeChanged(object sender, PointerMode mode)
    {
      this.menuBewerkenSelectie.Checked = false;
      this.menuBewerkenZoomIn.Checked = false;
      this.menuBewerkenZoomUit.Checked = false;
      this.menuBewerkenZoomGebied.Checked = false;
      this.menuBewerkenSchuif.Checked = false;
      this.menuBewerkenCentreer.Checked = false;

      switch (mode)
      {
        case PointerMode.Select:
          this.menuBewerkenSelectie.Checked = true;
          break;
        case PointerMode.ZoomIn:
          this.menuBewerkenZoomIn.Checked = true;
          break;
        case PointerMode.ZoomOut:
          this.menuBewerkenZoomUit.Checked = true;
          break;
        case PointerMode.ZoomArea:
          this.menuBewerkenZoomGebied.Checked = true;
          break;
        case PointerMode.Drag:
          this.menuBewerkenSchuif.Checked = true;
          break;
        case PointerMode.Center:
          this.menuBewerkenCentreer.Checked = true;
          break;
        default:
          this.menuBewerkenSelectie.Checked = true;
          break;
      }
    }

    /// <summary>
    /// Maps the control main on view mode changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="viewMode">The view mode.</param>
    private void MapControlMainOnViewModeChanged(object sender, ViewMode viewMode)
    {
      if (this.m_LegendaForm != null)
      {
        this.m_LegendaForm.Draw();
      }
      if (m_Berekening.KansenList.Count > 0)
      {
        this.mapControlMain.Canvas.Refresh();
      }
    }

    /// <summary>
    /// Maps the control main on custom menu button click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="buttonName">Name of the button.</param>
    private void MapControlMainOnCustomMenuButtonClick(object sender, string buttonName)
    {
      try
      {
        if (this.m_Berekening.KansenList.Count > 0)
        {
          ResultatenUitvoer(false, this.m_Berekening.OptimaleOverstromingskansenJaar, false);
        }
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the FormClosing event of the MainForm control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
    private void OnMainFormFormClosing(object sender, FormClosingEventArgs e)
    {
      BestandExit();
    }


    #endregion Event handling ------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Resultatens this instance.
    /// </summary>
    /// <param name="calculated">if set to <c>true</c> [calculated].</param>
    /// <param name="optimaleOverstromingskansenJaar">The optimale overstromingskansen jaar.</param>
    /// <param name="createTabel">if set to <c>true</c> [create tabel].</param>
    private void ResultatenUitvoer(bool calculated, int optimaleOverstromingskansenJaar, bool createTabel)
    {
      using (UitvoerForm uitvoerForm = new UitvoerForm(this, m_Berekening, calculated, optimaleOverstromingskansenJaar, createTabel))
      {
        uitvoerForm.Icon = this.Icon;
        uitvoerForm.ShowDialog(this);
        this.Text = uitvoerForm.Text;
        this.mapControlMain.BerekeningAanwezig = true;
      }
    }

    /// <summary>
    /// Initialiseer this instance.
    /// </summary>
    private void Initialize()
    {
      string databaseName = MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "Database", ""));
      if (File.Exists(databaseName))
      {
        OptimaliseRingDB optimaliseRingDB = new OptimaliseRingDB(ThisAppErr.Instance);
        optimaliseRingDB.Open(databaseName, "Provider=Microsoft.Jet.OLEDB.4.0;User Id=admin;Password=;Mode=12");

        m_Berekening = new Berekening(ThisAppCulture.Instance
           , ThisAppProfile.Instance
           , ThisAppLanguage.Instance
           , optimaliseRingDB
           , ThisAppErr.Instance);

        m_Berekening.Instellingen = new Instellingen(
           ThisAppProfile.Instance
           , ThisAppLanguage.Instance
           , optimaliseRingDB);
        m_Berekening.Instellingen.Load();
        m_Berekening.Instellingen.LoadKeringPercentages();

        m_Berekening.ZichtJaar = m_Berekening.Instellingen.ZichtJaar;
        m_Berekening.OptimaleOverstromingskansenJaar = m_Berekening.Instellingen.OptimaleOverstromingskansenJaar;

        this.mapControlMain.Profile = ThisAppProfile.Instance;
        this.mapControlMain.Berekening = m_Berekening;

        this.mapControlMain.Canvas.AfterLayerDraw += new AxMapObjectsLT2._DMapEvents_AfterLayerDrawEventHandler(OnAfterLayerDraw);
        this.mapControlMain.OnPointerModeChanged += new PointerModeChangedEventHandler(OnPointerModeChanged);
        this.mapControlMain.OnViewModeChanged += new ViewChangedEventHandler(MapControlMainOnViewModeChanged);
        this.mapControlMain.OnCustomMenuButtonClick += new CustomMenuButtonClickEventHandler(MapControlMainOnCustomMenuButtonClick);
        this.mapControlMain.LoadShapes();
        this.mapControlMain.SetViewport();

        // Initialize the MapTip class
        string shapeFileDijkringDelen = MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "ShapeFileDijkringDelen", ""));
        if (File.Exists(shapeFileDijkringDelen))
        {
          MapLayer dijkringenLayer = new MapLayerClass();
          dijkringenLayer.File = shapeFileDijkringDelen;
          this.mapControlMain.MapTipsInfoLayer = dijkringenLayer;
          this.mapControlMain.MapTipsLayerField = "DIJKRING";
        }

        // Initialize the MapTip class
        shapeFileDijkringDelen = MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "ShapeFileDijkringen", ""));
        if (File.Exists(shapeFileDijkringDelen))
        {
          MapLayer dijkringenLayer = new MapLayerClass();
          dijkringenLayer.File = shapeFileDijkringDelen;
          this.mapControlMain.MapTipsLookupLayer = dijkringenLayer;
        }

        this.menuHelpHandleiding.Enabled = true;
        if (ThisAppCulture.Instance.Name == "en-GB")
        {
          this.menuHelpHandleiding.Enabled = false;
        }



      }
      else
      {
        ThisAppErr.Instance.Raise(1000, new object[] { databaseName });
      }

    }

    /// <summary>
    /// Draws the systeem shapes.
    /// </summary>
    private void DrawSysteemShapes()
    {
      bool doorgaan = true;

      int i = 1;
      while (doorgaan)
      {
        string entry = "SysteemShape" + i.ToString();
        string shape = ThisAppProfile.Instance.GetValue("SysteemShapes", entry, "NO_MORE");
        if (shape.Length > 0)
        {
          if (string.Compare(shape, "NO_MORE") == 0)
          {
            doorgaan = false;
          }
          else
          {
            string[] items = shape.Split("|".ToCharArray());

            if (items.Length > 0)
            {
              string fileName = OptimaliseRing.General.MyPath.AbsoluteName(items[0]);
              if (File.Exists(fileName))
              {
                MapLayer mapLayer = new MapLayerClass();
                mapLayer.File = fileName;

                if (mapLayer.ShapeType == ShapeTypeConstants.moShapeTypePolygon)
                {
                  MapObjectsLT2.Recordset recordSet = mapLayer.Records;

                  recordSet.MoveFirst();
                  while (!recordSet.EOF)
                  {
                    MapObjectsLT2.Polygon polygon = (MapObjectsLT2.Polygon)recordSet.Fields.Item("Shape").Value;

                    MapObjectsLT2.Symbol symbol = new MapObjectsLT2.Symbol();

                    symbol.SymbolType = SymbolTypeConstants.moFillSymbol;
                    symbol.Size = 1;

                    // Bepaal kleur en stijl
                    symbol.Color = ColorConverter.ColorToUInt32(Color.Black);
                    symbol.Style = Convert.ToInt16(FillStyleConstants.moTransparentFill);

                    this.mapControlMain.Canvas.DrawShape(polygon, symbol);

                    recordSet.MoveNext();
                  }
                }
                mapLayer = null;
              }
              else
              {
                throw new System.Exception(string.Format("Systeem shape file {0}\ndoes not exist", fileName));
              }
            }
          }
          i++;
        }
      }
    }

    /// <summary>
    /// Draws the dijkringen.
    /// </summary>
    private void DrawDijkringen()
    {
      try
      {
        int legendaType = ConvertString.ToInt32(
          ThisAppProfile.Instance.GetValue("OptimaliseRing", "LegendaType", 0.ToString()));

        Int16 lineWidth = ConvertString.ToInt16(ThisAppProfile.Instance.GetValue("OptimaliseRing", "LineWidth", "1"));

        string shapeFileDijkringDelen = MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "ShapeFileDijkringDelen", ""));
        if (File.Exists(shapeFileDijkringDelen))
        {
          MapLayer dijkringLayer = new MapLayerClass();

          dijkringLayer.File = shapeFileDijkringDelen;

          if (dijkringLayer.ShapeType == ShapeTypeConstants.moShapeTypeLine)
          {
            MapObjectsLT2.Recordset recordSet = dijkringLayer.Records;

            recordSet.MoveFirst();
            while (!recordSet.EOF)
            {
              string id = recordSet.Fields.Item("DIJKRINGNU").Value.ToString();
              string compDeel = recordSet.Fields.Item("COMPARTDEE").Value.ToString();
              int deelNummer = ConvertString.ToInt32(recordSet.Fields.Item("DIJKRINGDE").Value.ToString());

              MapObjectsLT2.Line lines = (MapObjectsLT2.Line)recordSet.Fields.Item("Shape").Value;

              MapObjectsLT2.Symbol symbol = new MapObjectsLT2.Symbol();

              symbol.SymbolType = SymbolTypeConstants.moLineSymbol;
              symbol.Size = lineWidth;

              // Bepaal kleur
              symbol.Color = DijkringDeelKleur(legendaType, id, deelNummer, compDeel);

              symbol.Style = Convert.ToInt16(LineStyleConstants.moSolidLine);

              this.mapControlMain.Canvas.DrawShape(lines, symbol);

              recordSet.MoveNext();
            }
          }
          dijkringLayer = null;
        }

      }
      catch (Exception appex)
      {
        ThisAppErr.Instance.Display(this, appex);
      }
    }

    /// <summary>
    /// Bepaal de kleur van het dijkringdeel afhankelijk van de overstromingskans
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="deelNummer">The deelNummer.</param>
    /// <param name="compDeel">The compartimenteringsdeel.</param>
    /// <returns></returns>
    private uint DijkringDeelKleur(int legendaType, string id, int deelNummer, string compDeel)
    {

      uint kleur = 0;
      if (m_Berekening != null)
      {
        if (compDeel.Length > 0)
        {
          // verdeeld in compartimenten
          if (m_Berekening.Instellingen.Compartimentering.ContainsKey(id))
          {
            List<Compartimenteringsdijk> compartimenteringsdijk = m_Berekening.Instellingen.Compartimentering[id];

            foreach (Compartimenteringsdijk dijk in compartimenteringsdijk)
            {
              if (dijk.Dijkdeel == compDeel)
              {
                id = dijk.DijkId;
              }
            }
          }
        }

        if (m_Berekening.DijkringList.Count > 0)
        {
          Kansen kansen = m_Berekening.GetKansById(id, deelNummer);

          if (kansen != null)
          {
            if (kansen.BepaalKansVoorKleuringOpKaart(this.mapControlMain.Weergave).Length > 2)
            {
              double p = Double.Parse(kansen.BepaalKansVoorKleuringOpKaart(this.mapControlMain.Weergave).Substring(2));
              p = double.IsInfinity(p) ? 1.0 : p;

              for (int j = 0; j < ThisAppLegendaItems2.Instance[legendaType].Count; j++)
              {
                LegendaItem legendaItem = (LegendaItem)ThisAppLegendaItems2.Instance[legendaType].GetByIndex(j);
                if (p >= legendaItem.Min && p <= legendaItem.Max)
                {
                  kleur = legendaItem.UInt32Color;
                  break;
                }
              }
            }
          }
        }
      }
      return kleur;
    }

    /// <summary>
    /// Shows the map tips.
    /// </summary>
    /// <param name="show">if set to <c>true</c> [show].</param>
    public void ShowMapTips(bool show)
    {
      this.mapControlMain.MapTipsShow = show;
      this.menuBewerkenMapTips.Checked = show;
    }

    /// <summary>
    /// Shows the legenda.
    /// </summary>
    /// <param name="show">if set to <c>true</c> [show].</param>
    public void ShowLegenda(bool show)
    {
      if (this.m_LegendaForm == null)
      {
        this.m_LegendaForm = new LegendaForm(this);
      }
      this.m_LegendaForm.Jaar = m_Berekening.OptimaleOverstromingskansenJaar;

      if (show)
      {
        this.m_LegendaForm.Show(this);
      }
      else
      {
        this.m_LegendaForm.Dispose();
        this.m_LegendaForm = null;
      }
      this.mnuBewerkenLegenda.Checked = show;
    }

    /// <summary>
    /// Sets the status label.
    /// </summary>
    /// <param name="tekst">The tekst.</param>
    public void SetStatusLabel(string tekst)
    {
      this.statusLabel.Text = tekst;
      this.statusStripMain.Refresh();
    }

    /// <summary>
    /// Maak de berekeningen.
    /// </summary>
    private void StartBerekeningen()
    {
      try
      {
        using (BerekeningenForm berekeningenForm = new BerekeningenForm(m_Berekening))
        {
          berekeningenForm.Icon = this.Icon;
          if (berekeningenForm.ShowDialog(this) == DialogResult.OK)
          {
            berekeningenForm.Hide();

            using (new WaitCursor())
            {
              int berekeningNummer = 0;

              m_Berekening.BerekeningenMap = berekeningenForm.BerekeningenMap;

              this.mapControlMain.Refresh();

              SetStatusLabel(string.Format(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "StartBerekening").ToString()));

              if (m_Berekening.Start(this, ref berekeningNummer, m_Berekening.TeBerekenenDijkringdelen.Count))
              {
                m_Berekening.StrategieBestand("Strategie.xls");
                m_Berekening.KansenBestand("Kansen.xls");

                Refresh();
                SetStatusLabel("");
                ResultatenUitvoer(true, -1, false);
              }

              this.mapControlMain.Refresh();
            }
          }
          berekeningenForm.Dispose();

          if (m_LegendaForm != null)
          {
            m_LegendaForm.Jaar = m_Berekening.OptimaleOverstromingskansenJaar;
          }
        }
      }
      catch (System.Runtime.InteropServices.COMException)
      {
        SetStatusLabel(string.Format(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Mislukt").ToString()));
        ThisAppErr.Instance.DisplayError(this, 10, null);
      }
      catch (Exception appex)
      {
        SetStatusLabel(string.Format(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Mislukt").ToString()));
        ThisAppErr.Instance.Display(this, appex);
      }
    }

    #endregion

    #region Bestand ----------------------------------------------------------

    /// <summary>
    /// Bestands the exit.
    /// </summary>
    private void BestandExit()
    {
      try
      {
        if (m_Berekening != null)
        {
          if (m_Berekening.OptimaliseRingDB != null)
          {
            m_Berekening.OptimaliseRingDB.Close();
          }
          this.mapControlMain.SaveShapes(ThisAppProfile.Instance);

          ThisAppProfile.Instance.SetValue("OptimaliseRing", "MapTips", this.menuBewerkenMapTips.Checked.ToString());
          ThisAppProfile.Instance.SetValue("OptimaliseRing", "Legenda", this.mnuBewerkenLegenda.Checked.ToString());
          ThisAppProfile.Instance.SetValue("OptimaliseRing", "Werkmap", MyPath.RelativeName(ThisAppWorkingDirectory.Instance.Werkmap));

          if (this.WindowState == FormWindowState.Normal)
          {
            ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Top", this.Top.ToString());
            ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Left", this.Left.ToString());
            ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Width", this.Width.ToString());
            ThisAppProfile.Instance.SetValue("Formulieren", this.Name + "Height", this.Height.ToString());
          }
        }
        this.Dispose();
      }
      catch (Exception appex)
      {
        ThisAppErr.Instance.Display(this, appex);
      }
    }

    #endregion File ----------------------------------------------------------

    #region IStatus Members --------------------------------------------------

    public string Status
    {
      set { SetStatusLabel(value); }
    }

    #endregion
  }
}
