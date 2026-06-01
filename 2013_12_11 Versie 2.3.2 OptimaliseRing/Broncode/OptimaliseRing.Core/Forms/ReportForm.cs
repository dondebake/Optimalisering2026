using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Globalization;
using System.Threading;

using OptimaliseRing.Core;
using OptimaliseRing.General;

namespace OptimaliseRing.Core
{
   public partial class ReportForm :  Form
   {
      private Berekening m_Berekening;
      private Profile.Ini m_Profile;
      private Profile.Ini m_Language;
      private CultureInfo m_CultureInfo;

      /// <summary>
      /// Maak een nieuwe instantiatie van de  <see cref="T:ReportForm"/> class.
      /// </summary>
      public ReportForm(Profile.Ini profile, Profile.Ini language, Berekening berekening)
      {
         m_Profile = profile;
         m_Language = language;
         m_Berekening = berekening;
         m_CultureInfo = new CultureInfo(m_Profile.GetValue("OptimaliseRing", "Taal", "en-GB"));

         InitializeComponent();


         StrategieReport();
         KansenReport();

      }

      #region Properties -------------------------------------------------------

      /// <summary>
      /// Report viewer kosten.
      /// </summary>
      /// <value>The report viewer kosten.</value>
      public ReportViewer ReportViewerStrategie
      {
         get { return this.reportViewerStrategie; }
      }

      /// <summary>
      /// Report viewer kansen.
      /// </summary>
      /// <value>The report viewer kansen.</value>
      public ReportViewer ReportViewerKansen
      {
         get { return this.reportViewerKansen; }
      }

      #endregion Properties

      /// <summary>
      /// Strategiereport.
      /// </summary>
      public void StrategieReport()
      {
         if (m_Berekening.StrategieList.Count > 0)
         {
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = m_Berekening.StrategieList;

            ReportDataSource myDataSource = new ReportDataSource();
            myDataSource.Name = "OptimaliseRing_Core_Strategie";
            myDataSource.Value = bindingSource;// this.StrategieBindingSource;

            this.reportViewerStrategie.Clear();
            this.reportViewerStrategie.Reset();


            this.reportViewerStrategie.LocalReport.DataSources.Clear();
            this.reportViewerStrategie.LocalReport.DataSources.Add(myDataSource);
            this.reportViewerStrategie.Name = "reportViewerStrategie";

            this.reportViewerStrategie.LocalReport.ReportEmbeddedResource = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "ReportEmbeddedResourceStrategie", "ReportEmbeddedResourceStrategie").ToString();
            //ReportPageSettings reportPageSettings = this.reportViewerStrategie.LocalReport.GetDefaultPageSettings();

            SetStrategieReportParameters(m_Berekening.ZichtJaar);

            bindingSource.DataSource = m_Berekening.StrategieList;

            this.reportViewerStrategie.LocalReport.Refresh();

            // Process and render the report
            this.reportViewerStrategie.RefreshReport();

         }
      }

      /// <summary>
      /// Kansenreport.
      /// </summary>
      public void KansenReport()
      {
         if (m_Berekening.KansenList.Count > 0)
         {

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = m_Berekening.KansenList;

            ReportDataSource myDataSource = new ReportDataSource();
            myDataSource.Name = "OptimaliseRing_Core_Kansen";
            myDataSource.Value = bindingSource;

            this.ReportViewerKansen.Clear();
            this.ReportViewerKansen.Reset();

            this.ReportViewerKansen.LocalReport.DataSources.Clear();
            this.ReportViewerKansen.LocalReport.DataSources.Add(myDataSource);
            this.ReportViewerKansen.Name = "reportViewerKansen";

            this.ReportViewerKansen.LocalReport.ReportEmbeddedResource = m_Language.GetValue("Captions:" + m_CultureInfo.Name, "ReportEmbeddedResourceKansen", "ReportEmbeddedResourceKansen").ToString();
            SetKansenReportParameters(m_Berekening.ZichtJaar, m_Berekening.OptimaleOverstromingskansenJaar, m_Berekening.VeiligheidsnormTekst);

            bindingSource.DataSource = m_Berekening.KansenList;

            this.ReportViewerKansen.LocalReport.Refresh();

            // Process and render the report
            this.ReportViewerKansen.RefreshReport();

         }
      }

      /// <summary>
      /// Zet kansen report parameters.
      /// </summary>
      /// <param name="zichtJaar">ZichtJaar.</param>
      private void SetStrategieReportParameters(int zichtJaar)
      {
         // Zet de report parameters
         ReportParameter reportParameterZichtJaar = new ReportParameter("ZichtJaar", zichtJaar.ToString());
         this.reportViewerStrategie.LocalReport.SetParameters(new ReportParameter[] { reportParameterZichtJaar });
      }

      /// <summary>
      /// Zet kansen report parameters.
      /// </summary>
      /// <param name="zichtJaar">ZichtJaar.</param>
      /// <param name="optimaleOverstromingskansenJaar">The optimale overstromingskansen jaar.</param>
      /// <param name="restrictieTekst">The restrictie tekst.</param>
     private void SetKansenReportParameters(int zichtJaar, int optimaleOverstromingskansenJaar, string restrictieTekst)
      {
         // Zet de report parameters
         ReportParameter reportParameterZichtJaar = new ReportParameter("ZichtJaar", zichtJaar.ToString());
         ReportParameter reportParameterOptimaleOverstromingskansenJaar = new ReportParameter("OptimaleOverstromingskansenJaar", optimaleOverstromingskansenJaar.ToString());
         ReportParameter reportParameterRestrictie = new ReportParameter("Restrictie", restrictieTekst);

         this.reportViewerKansen.LocalReport.SetParameters(new ReportParameter[] { reportParameterZichtJaar, reportParameterOptimaleOverstromingskansenJaar, reportParameterRestrictie });
      }

      /// <summary>
      /// Handles the Click event of the btnOK control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
      private void OnBtnOKClick(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.OK;
      }

      /// <summary>
      /// Handles the Click event of the btnCancel control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
      private void BtnCancelClick(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
      }

      /// <summary>
      /// Handles the FormClosing event of the ReportForm control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
      private void OnReportFormFormClosing(object sender, FormClosingEventArgs e)
      {
         m_Profile.SetValue("Formulieren", this.Name + "Top", this.Top.ToString());
         m_Profile.SetValue("Formulieren", this.Name + "Left", this.Left.ToString());
         m_Profile.SetValue("Formulieren", this.Name + "Width", this.Width.ToString());
         m_Profile.SetValue("Formulieren", this.Name + "Height", this.Height.ToString());
      }

      /// <summary>
      /// Tabs the control_ selection changed.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="oldPage">The old page.</param>
      /// <param name="newPage">The new page.</param>
      private void TabControlSelectionChanged(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
      {
         if (newPage.Name == "tabPageKansen")
         {
            if (this.reportViewerKansen.LocalReport.ReportEmbeddedResource == null)
            {

               this.KansenReport();
            }
            else
            {
               this.reportViewerKansen.RefreshReport();
            }
         }
         else
         {
            if (this.reportViewerStrategie.LocalReport.ReportEmbeddedResource == null)
            {
               this.StrategieReport();
            }
            else
            {
               this.reportViewerStrategie.RefreshReport();
            }
         }
      }

      private void OnReportFormLoad(object sender, EventArgs e)
      {
         this.reportViewerKansen.RefreshReport();
         this.ReportViewerStrategie.RefreshReport();

         this.Top = ConvertString.ToInt32(m_Profile.GetValue("Formulieren", this.Name + "Top", "0").ToString());
         this.Left = ConvertString.ToInt32(m_Profile.GetValue("Formulieren", this.Name + "Left", "0").ToString());
         this.Width = ConvertString.ToInt32(m_Profile.GetValue("Formulieren", this.Name + "Width", "640").ToString());
         this.Height = ConvertString.ToInt32(m_Profile.GetValue("Formulieren", this.Name + "Height", "480").ToString());
      }
   }
}
