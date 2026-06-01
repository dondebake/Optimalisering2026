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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Core/Forms/CompartimenteringForm.cs 3     27/06/08 16:14 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using OptimaliseRing.General;

namespace OptimaliseRing.Core
{
   public partial class CompartimenteringForm : Form
   {
      private SortedList         m_DijkringDelen;
      private Profile.Ini        m_Profile;
      private Profile.Ini        m_Language;
      private Berekening         m_Berekening;
      private ApplicationError   m_ApplicationError;

      /// <summary>
      /// Maak een nieuwe instantiatie van de  <see cref="T:BerekeningenForm"/> class.
      /// </summary>
      public CompartimenteringForm(
         Profile.Ini profile
         , Profile.Ini languageProfile
         , Berekening berekening
         , ApplicationError applicationError)
      {
         m_Profile = profile;
         m_Language = languageProfile;
         m_Berekening = berekening;
         m_ApplicationError = applicationError;

         InitializeComponent();

         m_DijkringDelen = m_Berekening.OptimaliseRingDB.CompartimenteringsDijken();
      }

      /// <summary>
      /// Initializes this instance.
      /// </summary>
      private void Initialize()
      {
         CultureInfo cultureInfo = new CultureInfo(m_Profile.GetValue("OptimaliseRing", "Taal", "en-GB"));

         this.dgvCompartimenteren.Columns.Clear();

         DataGridViewCheckBoxColumn kolomCompartimenteren = new DataGridViewCheckBoxColumn();
         kolomCompartimenteren.Name = "Compartimenteren";
         kolomCompartimenteren.HeaderText = m_Language.GetValue("Captions:" + cultureInfo.Name, "Compartimenteren").ToString();
         kolomCompartimenteren.SortMode = DataGridViewColumnSortMode.NotSortable;
         kolomCompartimenteren.ReadOnly = false;
         kolomCompartimenteren.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
         this.dgvCompartimenteren.Columns.Add(kolomCompartimenteren);

         DataGridViewTextBoxColumn kolomDijkring = new DataGridViewTextBoxColumn();
         kolomDijkring.Name = "Dijkring";
         kolomDijkring.HeaderText = m_Language.GetValue("Captions:" + cultureInfo.Name, "Dijkring").ToString();
         kolomDijkring.SortMode = DataGridViewColumnSortMode.NotSortable;
         kolomDijkring.ReadOnly = true;
         kolomDijkring.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
         this.dgvCompartimenteren.Columns.Add(kolomDijkring);

         DataGridViewTextBoxColumn kolomDijkringNaam = new DataGridViewTextBoxColumn();
         kolomDijkringNaam.Name = "Naam";
         kolomDijkringNaam.HeaderText = m_Language.GetValue("Captions:" + cultureInfo.Name, "Naam").ToString();
         kolomDijkringNaam.SortMode = DataGridViewColumnSortMode.NotSortable;
         kolomDijkringNaam.ReadOnly = true;
         kolomDijkringNaam.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
         this.dgvCompartimenteren.Columns.Add(kolomDijkringNaam);

         this.dgvCompartimenteren.Rows.Clear();

         for (int i = 0; i < m_DijkringDelen.Count; i++)
         {
            SortedList dijkringDeel = (SortedList)m_DijkringDelen.GetByIndex(i);

            this.dgvCompartimenteren.Rows.Add(new DataGridViewRow());

            this.dgvCompartimenteren["Dijkring", i].Value = dijkringDeel["Dijkring"].ToString();
            this.dgvCompartimenteren["Naam", i].Value = m_Berekening.OptimaliseRingDB.DijkringNaam(dijkringDeel["Dijkring"].ToString());
            bool compartimenteren = m_Profile.GetValue("Compartimenteren", dijkringDeel["Dijkring"].ToString(), false);
            this.dgvCompartimenteren["Compartimenteren", i].Value = compartimenteren;
         }

      }

      /// <summary>
      /// Handles the Click event of the btnOK control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
      private void OnBtnOKClick(object sender, EventArgs e)
      {
         // Wijzigingen opslaan
         for (int i = 0; i < this.dgvCompartimenteren.RowCount; i++)
         {
            m_Profile.SetValue("Compartimenteren",
              (string)this.dgvCompartimenteren.Rows[i].Cells["dijkring"].Value,
              (bool)this.dgvCompartimenteren.Rows[i].Cells["Compartimenteren"].Value);
         }

         // Doorgeven dat er op OK is geklikt
         this.DialogResult = DialogResult.OK;
      }

      /// <summary>
      /// Handles the Click event of the btnCancel control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
      private void OnBtnCancelClick(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
      }

      /// <summary>
      /// Handles the CellValueChanged event of the dgvCompartimenteren control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
      private void DgvCompartimenterenCellValueChanged(object sender, DataGridViewCellEventArgs e)
      {
         try
         {
            this.btnOK.Enabled = true;
         }
         catch (Exception appex)
         {
            m_ApplicationError.Raise(1004, new object[] { appex.Message });
         }
      }

      /// <summary>
      /// Handles the Load event of the CompartimenteringForm control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
      private void OnCompartimenteringFormLoad(object sender, EventArgs e)
      {
         try
         {
            Initialize();
            this.btnOK.Enabled = false;
         }
         catch (Exception appex)
         {
            m_ApplicationError.Raise(1004, new object[] { appex.Message });
         }
      }

      /// <summary>
      /// Handles the CellEnter event of the dgvCompartimenteren control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
      private void OnDgvCompartimenterenCellEnter(object sender, DataGridViewCellEventArgs e)
      {
         try
         {
            if (e.ColumnIndex == 0)
            {
               this.btnOK.Enabled = true;
            }
         }
         catch (Exception appex)
         {
            m_ApplicationError.Raise(1004, new object[] { appex.Message });
         }
      }

   }
}
