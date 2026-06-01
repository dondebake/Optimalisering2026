#region Copyright -------------------------------------------------------
// Copyright © 2008, Rijkswaterstaat/Waterdienst & ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : 1142.50.00 Batch OptimalisatieRing
//              OptimaliseRing - Economische optimalisatie van veiligheidsniveaus van dijkringen
//
// Author(s)  : Johan Ansink, HKV lijn in water
//              Rolf Waterman, HKV lijn in water
//              Matthijs Duits, HKV lijn in water
//
#endregion

using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using OptimaliseRing.General;
using OptimaliseRing.Core;

namespace OptimaliseRing.UI.Forms
{
  /// <summary>
  /// InstellingenForm
  /// </summary>
  public partial class InstellingenForm : Form
  {
    #region Instance Variables -----------------------------------------------

    /// <summary>
    /// Vlag voor 'dirty' instellingen
    /// </summary>
    private Boolean m_IsDirty;

    /// <summary>
    /// Project
    /// </summary>
    private Project m_Project;

    /// <summary>
    /// Project Filename
    /// </summary>
    private String m_ProjectFilename;

    /// <summary>
    /// Instellingenmap
    /// </summary>
    private string m_InstellingenMap;

    /// <summary>
    /// berekening
    /// </summary>
    private Berekening m_Berekening;

    /// <summary>
    /// Aantal jaren berekenen
    /// </summary>
    private int m_Z;

    #endregion Instance Variables --------------------------------------------

    #region Constructors -----------------------------------------------------

    /// <summary>
    /// Maak een nieuwe instantiatie van de  <see cref="T:InstellingenForm"/> class.
    /// </summary>
    public InstellingenForm(Berekening berekening)
    {
      m_Berekening = berekening;

      InitializeComponent();

      m_ProjectFilename = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "UntitledProjectName", "UntitledProjectName").ToString();


      m_Project = new Project();

      m_InstellingenMap = MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "InstellingenMap", "./Instellingen"));

      InitializeComboBox(cboEconomischScenario, m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM EconomischScenario").ToSortedList());
      InitializeComboBox(cboParametersKostenfunctie, m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM ParametersKostenfunctie").ToSortedList());
      InitializeComboBox(cboScenarioVoorHoeveelheidSchade, m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM ScenarioVoorHoeveelheidSchade").ToSortedList());
      InitializeComboBox(cboKlimaatScenarioEnFysischMaxAfvoer, m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM Klimaat_AftoppenAfvoer").ToSortedList());
      InitializeComboBox(cboRamingVoorSlachtoffers, m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM RamingVoorSlachtoffers").ToSortedList());
      InitializeComboBox(cboSchadeFunctie, m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM SchadeFunctie").ToSortedList());
      InitializeComboBox(cboTypeKostenfunctie, m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM TypeKostenfunctie").ToSortedList());
      InitializeComboBox(cboDijkringspecifiekeFactorSchade, m_Berekening.OptimaliseRingDB.GetDataset("SELECT * FROM DijkringspecifiekeFactorSchade").ToSortedList());

      cboVeiligheidsnorm.Items.Clear();
      cboVeiligheidsnorm.Items.Add(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name
        , "UVeiligheidsnormKeuze1", "UVeiligheidsnormKeuze1"));
      cboVeiligheidsnorm.Items.Add(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name
        , "UVeiligheidsnormKeuze2", "UVeiligheidsnormKeuze2"));
      cboVeiligheidsnorm.SelectedIndex = 0;

      // lees laatst gekozen uit de inifile
      m_Berekening.Instellingen.Load();

      m_Berekening.Instellingen.LoadKeringPercentages();

      // ophalen z-waarde
      this.m_Z = Convert.ToInt32(ThisAppProfile.Instance.GetValue("Parameters", "Z"));

      Initialize();

      this.nudOptimaleOverstromingskansenJaar.Value = Math.Max(this.nudOptimaleOverstromingskansenJaar.Value, this.nudZichtJaar.Value);
      this.nudOptimaleOverstromingskansenJaar.Minimum = this.nudZichtJaar.Value;
      this.nudOptimaleOverstromingskansenJaar.Maximum = this.nudZichtJaar.Value + this.m_Z;

      this.nudDiscontovoetInvesteringen.Enabled = !this.chkParametersonzekerheid.Checked;
      this.nudDiscontovoetSchade.Enabled = !this.chkParametersonzekerheid.Checked;
      this.cboKlimaatScenarioEnFysischMaxAfvoer.Enabled = !this.chkParametersonzekerheid.Checked;
      this.cboEconomischScenario.Enabled = !this.chkParametersonzekerheid.Checked;
    }


    #endregion Constructors --------------------------------------------------

    #region Properties -------------------------------------------------------

    /// <summary>
    /// Gets a value indicating whether this instance is dirty.
    /// </summary>
    /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
    public Boolean IsDirty
    {
      get { return m_IsDirty; }
    }

    /// <summary>
    /// Gets the project filename.
    /// </summary>
    /// <value>The project filename.</value>
    public string ProjectFilename
    {
      get { return m_ProjectFilename; }
    }

    #endregion Properties ----------------------------------------------------

    #region Member functions -------------------------------------------------

    /// <summary>
    /// Loads the instellingen.
    /// </summary>
    public void Initialize()
    {
      nudZichtJaar.Text = m_Berekening.Instellingen.ZichtJaar.ToString();
      nudOptimaleOverstromingskansenJaar.Text = m_Berekening.Instellingen.OptimaleOverstromingskansenJaar.ToString();
      nudDiscontovoetSchade.Text = m_Berekening.Instellingen.DiscontovoetSchade.ToString("F1");
      nudDiscontovoetInvesteringen.Text = m_Berekening.Instellingen.DiscontovoetInvesteringen.ToString("F1");
      //cboEconomischScenario.SelectedIndex = m_Berekening.Instellingen.EconomischScenario - 1;
      cboEconomischScenario.SelectedIndex = TestMaximaleComboIndex(cboEconomischScenario, m_Berekening.Instellingen.EconomischScenario);
      nudBedragPerInwoner.Text = m_Berekening.Instellingen.BedragPerInwoner.ToString();
      nudBedragPerDodelijkSlachtoffer.Text = m_Berekening.Instellingen.BedragPerDodelijkSlachtoffer.ToString();
      nudBedragPerGetroffene.Text = m_Berekening.Instellingen.BedragPerGetroffene.ToString();
      nudAversiefactor.Text = m_Berekening.Instellingen.Aversiefactor.ToString("F2");
      nudBeleidsfactorOverstromingsSchade.Text = m_Berekening.Instellingen.BeleidsfactorOverstromingsschade.ToString("F2");
      nudAanpassingsfactorOverstromingsSchade.Text = m_Berekening.Instellingen.AanpassingsfactorOverstromingsschade.ToString("F2");
      //cboParametersKostenfunctie.SelectedIndex = m_Berekening.Instellingen.ParametersKostenfunctie - 1;
      cboParametersKostenfunctie.SelectedIndex = TestMaximaleComboIndex(cboParametersKostenfunctie, m_Berekening.Instellingen.ParametersKostenfunctie);
      //cboScenarioVoorHoeveelheidSchade.SelectedIndex = m_Berekening.Instellingen.ScenarioVoorHoeveelheidSchade - 1;
      cboScenarioVoorHoeveelheidSchade.SelectedIndex = TestMaximaleComboIndex(cboScenarioVoorHoeveelheidSchade, m_Berekening.Instellingen.ScenarioVoorHoeveelheidSchade);
      //cboKlimaatScenarioEnFysischMaxAfvoer.SelectedIndex = m_Berekening.Instellingen.KlimaatScenarioEnFysischMaxAfvoer - 1;
      cboKlimaatScenarioEnFysischMaxAfvoer.SelectedIndex = TestMaximaleComboIndex(cboKlimaatScenarioEnFysischMaxAfvoer, m_Berekening.Instellingen.KlimaatScenarioEnFysischMaxAfvoer);
      //cboTypeKostenfunctie.SelectedIndex = m_Berekening.Instellingen.TypeKostenfunctie - 1;
      cboTypeKostenfunctie.SelectedIndex = TestMaximaleComboIndex(cboTypeKostenfunctie, m_Berekening.Instellingen.TypeKostenfunctie);
      //cboVeiligheidsnorm.SelectedIndex = m_Berekening.Instellingen.Veiligheidsnorm - 1;
      cboVeiligheidsnorm.SelectedIndex = TestMaximaleComboIndex(cboVeiligheidsnorm, m_Berekening.Instellingen.Veiligheidsnorm);
      //cboDijkringspecifiekeFactorSchade.SelectedIndex = m_Berekening.Instellingen.DijkringspecifiekeFactorSchade - 1;
      cboDijkringspecifiekeFactorSchade.SelectedIndex = TestMaximaleComboIndex(cboDijkringspecifiekeFactorSchade, m_Berekening.Instellingen.DijkringspecifiekeFactorSchade);
      //cboRamingVoorSlachtoffers.SelectedIndex = m_Berekening.Instellingen.RamingVoorSlachtoffers - 1;
      cboRamingVoorSlachtoffers.SelectedIndex = TestMaximaleComboIndex(cboRamingVoorSlachtoffers, m_Berekening.Instellingen.RamingVoorSlachtoffers);
      //cboSchadeFunctie.SelectedIndex = m_Berekening.Instellingen.SchadeFunctie - 1;
      cboSchadeFunctie.SelectedIndex = TestMaximaleComboIndex(cboSchadeFunctie, m_Berekening.Instellingen.SchadeFunctie);
      nudFactorKosten.Text = m_Berekening.Instellingen.FactorKosten.ToString("F2");
      nudFactorKans.Text = m_Berekening.Instellingen.FactorKans.ToString("F2");
      nudFactorGroeiSchade.Text = m_Berekening.Instellingen.FactorGroeiSchade.ToString("F2");
      chkParametersonzekerheid.Checked = m_Berekening.Instellingen.Parametersonzekerheid;
      m_IsDirty = false;
    }

    /// <summary>
    /// Zet conmobox op de waarde uit optimalisering.ini tenzij de index niet bestaat
    /// </summary>
    /// <param name="cbo"></param>
    /// <param name="iniKeuze"></param>
    /// <returns>keuze</returns>
    private int TestMaximaleComboIndex(ComboBox cbo, Object iniKeuze)
    {
      int keuze;
      if (cbo.Items.Count >= (int)iniKeuze)
      {
        keuze = (int)iniKeuze - 1;
      }
      else
      {
        keuze = 0;
      }
      return keuze;
    }


    /// <summary>
    /// Saves the instellingen.
    /// </summary>
    public void SaveInstellingen()
    {
      // Save values
      this.m_Berekening.Instellingen.ZichtJaar = ConvertString.ToInt32(nudZichtJaar.Text);
      this.m_Berekening.Instellingen.OptimaleOverstromingskansenJaar = ConvertString.ToInt32(nudOptimaleOverstromingskansenJaar.Text);
      this.m_Berekening.Instellingen.DiscontovoetSchade = ConvertString.ToDouble(nudDiscontovoetSchade.Text);
      this.m_Berekening.Instellingen.DiscontovoetInvesteringen = ConvertString.ToDouble(nudDiscontovoetInvesteringen.Text);
      this.m_Berekening.Instellingen.EconomischScenario = this.cboEconomischScenario.SelectedIndex + 1;
      this.m_Berekening.Instellingen.BedragPerInwoner = ConvertString.ToInt32(nudBedragPerInwoner.Text);
      this.m_Berekening.Instellingen.BedragPerDodelijkSlachtoffer = ConvertString.ToInt32(nudBedragPerDodelijkSlachtoffer.Text);
      this.m_Berekening.Instellingen.BedragPerGetroffene = ConvertString.ToInt32(nudBedragPerGetroffene.Text);
      this.m_Berekening.Instellingen.Aversiefactor = ConvertString.ToDouble(nudAversiefactor.Text);
      this.m_Berekening.Instellingen.BeleidsfactorOverstromingsschade = ConvertString.ToDouble(nudBeleidsfactorOverstromingsSchade.Text);
      this.m_Berekening.Instellingen.AanpassingsfactorOverstromingsschade = ConvertString.ToDouble(nudAanpassingsfactorOverstromingsSchade.Text);
      this.m_Berekening.Instellingen.ParametersKostenfunctie = this.cboParametersKostenfunctie.SelectedIndex + 1;
      this.m_Berekening.Instellingen.ScenarioVoorHoeveelheidSchade = this.cboScenarioVoorHoeveelheidSchade.SelectedIndex + 1;
      this.m_Berekening.Instellingen.KlimaatScenarioEnFysischMaxAfvoer = this.cboKlimaatScenarioEnFysischMaxAfvoer.SelectedIndex + 1;
      this.m_Berekening.Instellingen.TypeKostenfunctie = this.cboTypeKostenfunctie.SelectedIndex + 1;
      this.m_Berekening.Instellingen.Veiligheidsnorm = this.cboVeiligheidsnorm.SelectedIndex + 1;
      this.m_Berekening.Instellingen.DijkringspecifiekeFactorSchade = this.cboDijkringspecifiekeFactorSchade.SelectedIndex + 1;
      this.m_Berekening.Instellingen.FactorKans = ConvertString.ToDouble(nudFactorKans.Text);
      this.m_Berekening.Instellingen.FactorKosten = ConvertString.ToDouble(nudFactorKosten.Text);
      this.m_Berekening.Instellingen.FactorGroeiSchade = ConvertString.ToDouble(nudFactorGroeiSchade.Text);
      this.m_Berekening.Instellingen.RamingVoorSlachtoffers = this.cboRamingVoorSlachtoffers.SelectedIndex + 1;
      this.m_Berekening.Instellingen.SchadeFunctie = this.cboSchadeFunctie.SelectedIndex + 1;
      this.m_Berekening.Instellingen.Parametersonzekerheid = this.chkParametersonzekerheid.Checked && this.m_Berekening.Instellingen.Scenarioparameters.Count > 0;
    }

    /// <summary>
    /// Handles the Click event of the btnOK control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void btnOK_Click(object sender, EventArgs e)
    {
      try
      {
        if (QueryContinueOpen())
        {
          SaveInstellingen();
          // Bewaar laatste instellingen ook in de inifile
          m_Berekening.Instellingen.Save();
          m_Berekening.Instellingen.SaveKeringPercentages();

          DialogResult = DialogResult.OK;
        }
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the Click event of the btnCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void btnCancel_Click(object sender, EventArgs e)
    {
      try
      {
        DialogResult = DialogResult.Cancel;
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Initialiseer the combo box.
    /// </summary>
    /// <param name="comboBox">The combo box.</param>
    /// <param name="entry">The entry.</param>
    private void InitializeComboBox(ComboBox comboBox, SortedList records)
    {
      for (int i = 0; i < records.Count; i++)
      {
        SortedList record = (SortedList)records.GetByIndex(i);

        string columnName = "Naam";
        if (ThisAppCulture.Instance.Name == "en-GB")
        {
          columnName = "Name";
        }
        comboBox.Items.Add(record[columnName].ToString());
      }
      if (comboBox.Items.Count > 0)
      {
        comboBox.SelectedIndex = 0;
      }
    }

    /// <summary>
    /// Values the changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void ValueChanged(object sender, EventArgs e)
    {
      try
      {
        m_IsDirty = true;
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    #region Bestand Menu

    /// <summary>
    /// Handles the Click event of the menuBestandOpenen control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void menuBestandOpenen_Click(object sender, EventArgs e)
    {
      try
      {
        FileOpen();
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the Click event of the mnuBestandSave control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void mnuBestandSave_Click(object sender, EventArgs e)
    {
      try
      {
        SaveOrSaveAs();
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the Click event of the menuBestandSaveAs control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void menuBestandSaveAs_Click(object sender, EventArgs e)
    {
      try
      {
        FileSaveAs();
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    #endregion

    /// <summary>
    /// Enables/disables the Save and SaveAs menu items.
    /// </summary>
    /// <param name="bEnable"><b>true</b> for enabling the menu items, <b>false</b> for disabling.</param>
    private void EnableMenuItems(bool bEnable)
    {
      menuBestandSave.Enabled = bEnable;
      menuBestandSaveAs.Enabled = bEnable;
    }

    #region File -------------------------------------------------------------

    /// <summary>
    /// File open.
    /// </summary>
    private void FileOpen()
    {
      if (QueryContinueOpen())
      {
        string oKCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FileOKCaption", "FileOKCaption").ToString();
        string cancelCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "CancelCaption", "CancelCaption").ToString();
        string lookInCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "LookInCaption", "LookInCaption").ToString();
        string fileNameCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FileNameCaption", "FileNameCaption").ToString();
        string filesOfTypeCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FilesOfTypeCaption", "FilesOfTypeCaption").ToString();

        string fileName = OptimaliseRingDialog.Open(oKCaption, cancelCaption, lookInCaption, fileNameCaption, filesOfTypeCaption
        , ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FileOpenTitle", "FileOpenTitle").ToString()
        , ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FileOpenFilter", "FileOpenFilter").ToString()
        , "xml"
        , m_InstellingenMap);

        if (!string.IsNullOrEmpty(fileName))
        {
          m_InstellingenMap = Path.GetDirectoryName(fileName);
          ThisAppProfile.Instance.SetValue("OptimaliseRing", "InstellingenMap", MyPath.RelativeName(m_InstellingenMap));

          // Test of dit een instellingen bestand is
          if (InstellingenBestand(fileName))
          {
            FileOpen(fileName);
          }
          else
          {
            ThisAppErr.Instance.DisplayMessage(this, 4, new object[] { fileName });
            // Try again
            FileOpen();
          }
        }
      }
    }

    /// <summary>
    /// Test of dit xml bestand een instellingen xml bestand is
    /// </summary>
    /// <param name="bestand">Naam van het bestand.</param>
    /// <returns>true als dit een instellingen bestand is</returns>
    private bool InstellingenBestand(string bestand)
    {
      bool instellingenBestand = false;

      XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
      xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
      xmlReaderSettings.IgnoreWhitespace = true;
      xmlReaderSettings.IgnoreComments = true;

      using (XmlReader xmlReader = XmlReader.Create(bestand, xmlReaderSettings))
      {
        // Lees <Instellingen>
        while (xmlReader.Read())
        {
          if ((xmlReader.Name == "Instellingen") && xmlReader.IsStartElement())
          {
            instellingenBestand = true;
            break;
          }
        }
      }
      return instellingenBestand;
    }


    /// <summary>
    /// File open.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    private void FileOpen(string fileName)
    {
      bool bFailed = true;

      try
      {
        m_Project.Read(fileName, m_Berekening.Instellingen);
        m_ProjectFilename = fileName;
        Initialize();
        EnableMenuItems(true);

        bFailed = false;
      }
      catch (ApplicationException appex)
      {
        ThisAppErr.Instance.Raise(1004, new object[] { appex.Message });
      }

      if (bFailed)
      {
        m_Project.Clear();
      }
    }

    /// <summary>
    /// Queries the continue open.
    /// </summary>
    /// <returns></returns>
    private bool QueryContinueOpen()
    {
      bool continueOpen = true;

      if (m_Project.IsDirty)
      {
        switch (PromptToSave())
        {
          case DialogResult.Yes:
            SaveOrSaveAs();
            break;
          case DialogResult.No:
            break;
          case DialogResult.Cancel:
            continueOpen = false;
            break;
        }
      }

      return continueOpen;
    }

    /// <summary>
    /// Prompts to save.
    /// </summary>
    /// <returns></returns>
    private DialogResult PromptToSave()
    {
      return ThisAppErr.Instance.DisplayQuestion(this, 3, new string[] { m_ProjectFilename });
    }

    /// <summary>
    /// Saves the or save as.
    /// </summary>
    private void SaveOrSaveAs()
    {
      if (m_ProjectFilename == ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "UntitledProjectName", "UntitledProjectName").ToString())
      {
        FileSaveAs();
      }
      else
      {
        FileSave(m_ProjectFilename);
      }
    }

    /// <summary>
    /// Files the save.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    private void FileSave(string fileName)
    {
      try
      {
        SaveInstellingen();
        m_Project.Write(fileName, m_Berekening.Instellingen);
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, ex.InnerException.Message, "Save",
          MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        FileSaveAs();
      }
    }

    /// <summary>
    /// Files the save as.
    /// </summary>
    private void FileSaveAs()
    {
      string oKCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FileSaveOkCaption", "FileSaveOkCaption").ToString();
      string cancelCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "CancelCaption", "CancelCaption").ToString();
      string lookInCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "LookInCaption", "LookInCaption").ToString();
      string fileNameCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FileNameCaption", "FileNameCaption").ToString();
      string filesOfTypeCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FilesSaveOfTypeCaption", "FilesSaveOfTypeCaption").ToString();

      string fileName = OptimaliseRingDialog.Save(oKCaption, cancelCaption, lookInCaption, fileNameCaption, filesOfTypeCaption
         , ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FileSaveTitle", "FileSaveTitle").ToString()
         , ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FileSaveFilter", "FileSaveFilter").ToString()
         , "xml"
         , m_InstellingenMap);
      if (!string.IsNullOrEmpty(fileName))
      {
        FileSave(fileName);

        m_ProjectFilename = fileName;

        m_InstellingenMap = Path.GetDirectoryName(fileName);
        ThisAppProfile.Instance.SetValue("OptimaliseRing", "InstellingenMap", MyPath.RelativeName(m_InstellingenMap));

        EnableMenuItems(true);
      }
    }

    /// <summary>
    /// Handles the CheckedChanged event of the chkParametersonzekerheid control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void chkParametersonzekerheid_CheckedChanged(object sender, EventArgs e)
    {
      try
      {
        this.nudDiscontovoetInvesteringen.Enabled = !this.chkParametersonzekerheid.Checked;
        this.nudDiscontovoetSchade.Enabled = !this.chkParametersonzekerheid.Checked;
        this.cboKlimaatScenarioEnFysischMaxAfvoer.Enabled = !this.chkParametersonzekerheid.Checked;
        this.cboEconomischScenario.Enabled = !this.chkParametersonzekerheid.Checked;
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    #endregion File ----------------------------------------------------------

    private void nudZichtJaar_ValueChanged(object sender, EventArgs e)
    {
      try
      {
        this.nudOptimaleOverstromingskansenJaar.Minimum = (sender as NumericUpDown).Value;
        this.nudOptimaleOverstromingskansenJaar.Maximum = (sender as NumericUpDown).Value + this.m_Z;
        this.nudOptimaleOverstromingskansenJaar.Value = Math.Max(this.nudOptimaleOverstromingskansenJaar.Value, (sender as NumericUpDown).Value);
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }
    }

    /// <summary>
    /// Handles the Click event of the btnParameterScenarios control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void btnParameterScenarios_Click(object sender, EventArgs e)
    {
      try
      {
        using (ParameterOnzekerheidForm parameterOnzekerheidForm = new ParameterOnzekerheidForm(
          this.m_Berekening))
        {
          if (parameterOnzekerheidForm.ShowDialog(this) == DialogResult.OK)
          {
            // Opslaan ?
            Console.WriteLine("");
          }
        }
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }

    }

    #endregion Member functions ----------------------------------------------

    private void btnBkeringen_Click(object sender, EventArgs e)
    {
      try
      {
        using (KeringenForm keringenform = new KeringenForm(
          this.m_Berekening))
        {
          if (keringenform.ShowDialog(this) == DialogResult.OK)
          {
            // Opslaan ?
            Console.WriteLine("");
          }
        }
      }
      catch (Exception appex)
      {
        Hkv.General.ApplicationError.Display(this, appex);
      }

    }



  }
}
