using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ComponentAge.Dialogs;

namespace OptimaliseRing.Core
{
   /// <summary>
   /// DiamantDialog
   /// </summary>
   public sealed class OptimaliseRingDialog
   {
      private static string m_StartFolder = string.Empty;

      #region Member functions -------------------------------------------------

      /// <summary>
      /// Selecteer 1 of meer bestaande bestanden
      /// </summary>
      /// <param name="okCaption">De tekst op de OK knop.</param>
      /// <param name="titel">Titel van de dialoog</param>
      /// <param name="filter">Filter van de dialoog.</param>
      /// <param name="defaultExt">Default extensie van de dialoog.</param>
      /// <param name="padEntry">Naam van de entry in de ini-file.</param>
      /// <returns>Namen van de geselecteerde bestanden</returns>
      public static string[] OpenMulti(
         string okCaption
         , string titel
         , string filter
         , string defaultExt
         , string initialDirectory
         )
      {
         string [] namen = null;

         using (CaOpenFileDialog fileDialog = new CaOpenFileDialog())
         {
           fileDialog.DlgItemsCaptions.OK = okCaption;
           fileDialog.DlgItemsCaptions.Cancel = "&Annuleren";
           fileDialog.DlgItemsCaptions.LookIn = "Kijk in";
           fileDialog.DlgItemsCaptions.FileName = "Bestand";
           fileDialog.DlgItemsCaptions.FilesOfType = "Open als type";
           fileDialog.DefaultExt = defaultExt;
           fileDialog.CheckFileExists = true;
           fileDialog.DereferenceLinks = true;
           fileDialog.Filter = filter;
           fileDialog.FilterIndex = 6;
           fileDialog.Multiselect = true;
           fileDialog.Title = titel;
           fileDialog.ValidateNames = true;
           fileDialog.InitialDirectory = initialDirectory;
           if (!string.IsNullOrEmpty(initialDirectory))
             fileDialog.InitialDirectory = initialDirectory;
           if (fileDialog.ShowDialog() == DialogResult.OK)
             namen = fileDialog.FileNames;
         }

         return namen;
      }

      /// <summary>
      /// Selecteer 1 bestaand bestand
      /// </summary>
      /// <param name="okCaption">De tekst op de OK knop.</param>
      /// <param name="cancelCaption">The cancel caption.</param>
      /// <param name="lookInCaption">The look in caption.</param>
      /// <param name="fileNameCaption">The file name caption.</param>
      /// <param name="filesOfTypeCaption">The files of type caption.</param>
      /// <param name="titel">Titel van de dialoog</param>
      /// <param name="filter">Filter van de dialoog.</param>
      /// <param name="defaultExt">Default extensie van de dialoog.</param>
      /// <param name="initialDirectory">The initial directory.</param>
      /// <returns>Naam van het geselecteerde bestand</returns>
      public static string Open(
         string okCaption
         , string cancelCaption
         , string lookInCaption
         , string fileNameCaption
         , string filesOfTypeCaption
         , string titel
         , string filter
         , string defaultExt
         , string initialDirectory
         )
      {
         string naam = null;

         CaOpenFileDialog fileDialog = new CaOpenFileDialog();

         fileDialog.DlgItemsCaptions.OK = okCaption;
         fileDialog.DlgItemsCaptions.Cancel = cancelCaption;
         fileDialog.DlgItemsCaptions.LookIn = lookInCaption;
         fileDialog.DlgItemsCaptions.FileName = fileNameCaption;
         fileDialog.DlgItemsCaptions.FilesOfType = filesOfTypeCaption;

         fileDialog.DefaultExt = defaultExt;
         fileDialog.CheckFileExists = true;
         fileDialog.DereferenceLinks = true;
         fileDialog.Filter = filter;
         fileDialog.FilterIndex = 6;
         fileDialog.Multiselect = false ;
         fileDialog.Title = titel;
         fileDialog.ValidateNames = true;
         fileDialog.InitialDirectory = initialDirectory;

         if (fileDialog.ShowDialog() == DialogResult.OK)
         {
            naam = fileDialog.FileName;
         }
         return naam;
      }

      /// <summary>
      /// Selecteer een nieuwe naam
      /// </summary>
      /// <param name="okCaption">De tekst op de OK knop.</param>
      /// <param name="cancelCaption">The cancel caption.</param>
      /// <param name="lookInCaption">The look in caption.</param>
      /// <param name="fileNameCaption">The file name caption.</param>
      /// <param name="filesOfTypeCaption">The files of type caption.</param>
      /// <param name="titel">Titel van de dialoog</param>
      /// <param name="filter">Filter van de dialoog.</param>
      /// <param name="defaultExt">Default extensie van de dialoog.</param>
      /// <param name="initialDirectory">The initial directory.</param>
      /// <returns>Naam van het geselecteerde bestand</returns>
      public static string Save(
         string okCaption
         , string cancelCaption
         , string lookInCaption
         , string fileNameCaption
         , string filesOfTypeCaption
         , string titel
         , string filter
         , string defaultExt
         , string initialDirectory
         )
      {
         string naam = string.Empty;

         CaSaveFileDialog fileDialog = new CaSaveFileDialog();

         fileDialog.DlgItemsCaptions.OK = okCaption;
         fileDialog.DlgItemsCaptions.Cancel = cancelCaption;
         fileDialog.DlgItemsCaptions.LookIn = lookInCaption;
         fileDialog.DlgItemsCaptions.FileName = fileNameCaption;
         fileDialog.DlgItemsCaptions.FilesOfType = filesOfTypeCaption;

         fileDialog.DefaultExt = defaultExt;
         fileDialog.Filter = filter;
         fileDialog.Title = titel;
         fileDialog.CheckFileExists = false;
         fileDialog.InitialDirectory = initialDirectory;

         if (fileDialog.ShowDialog() == DialogResult.OK)
         {
            naam = fileDialog.FileName;
         }
         return naam;
      }

      /// <summary>
      /// Kies een folder
      /// </summary>
      /// <returns>Naam van de gekozen folder</returns>
      public static string Folder(string title, string statusText, string startFolder, bool showNewFolderButton)
      {
         m_StartFolder = startFolder;
         string naam = string.Empty;

         CaFolderDialog folderDialog = new CaFolderDialog();
         folderDialog.NewUserInterface = true;
         folderDialog.ShowNewFolderButton = false;
         folderDialog.Title = title;
         folderDialog.StatusText = statusText;
         folderDialog.ShowNewFolderButton = showNewFolderButton;
         folderDialog.Show += new EventHandler(folderDialog_Show);

         //if (folderDialog.ShowDialog() != DialogResult.Cancel)
         //{
         //  naam = "cancelled";
         //}
         if (folderDialog.ShowDialog() == DialogResult.OK)
         {
            naam = folderDialog.SelectedFolder;
         }
         return naam;
      }

      static void folderDialog_Show(object sender, EventArgs e)
      {
         CaFolderDialog caFolderDialog = sender as CaFolderDialog;
         caFolderDialog.SelectedFolder = m_StartFolder;
      }

      #endregion Member functions ----------------------------------------------
   }
}
