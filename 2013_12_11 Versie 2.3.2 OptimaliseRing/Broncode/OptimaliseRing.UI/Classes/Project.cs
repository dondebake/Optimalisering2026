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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Classes/Project.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Windows.Forms;

using OptimaliseRing.Core;

namespace OptimaliseRing.UI
{
   /// <summary>Handles Project <see cref="Project.Modified"/> events.</summary>
   public delegate void ProjectModifiedEventHandler(object sender, EventArgs e);

   /// <summary>
   /// Represents an OptimaliseRing project
   /// </summary>
   public class Project
   {
      private string m_ProjectFile;
      private string m_ProjectMap;
      private bool m_IsDirty;
      private bool m_SuspendDirtyCheck;

      /// <summary>
      /// Maak een nieuwe instantiatie van de  <see cref="T:Project"/> class.
      /// </summary>
      public Project()
      {
         this.m_IsDirty = false;
         this.m_SuspendDirtyCheck = false;
         this.m_ProjectMap = OptimaliseRing.General.MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "InstellingenMap", ""));
      }

      #region Properties

      /// <summary>
      /// Gets or sets the name of an OptimaliseRing project
      /// </summary>
      /// <value>The projectFile.</value>
      public string ProjectFile
      {
         get { return this.m_ProjectFile; }
         set { this.m_ProjectFile = value; }
      }

      #endregion

      #region 'Dirty' flag handling

      /// <summary>Raised when the project <see cref="IsDirty"/> state changes from <see langword="false"/> to <see langword="true"/>.</summary>
      public event ProjectModifiedEventHandler Modified;

      private void ContentsChanged(object sender, EventArgs e)
      {
         IsDirty = true;
      }

      /// <summary>Gets or sets a value indicating whether the contents of this project have been modified.</summary>
      /// <remarks>If a project is marked as 'dirty' then the GUI will ask to user if they wish to save the project before loading another, or exiting.</remarks>
      public bool IsDirty
      {
         get { return this.m_IsDirty; }

         set
         {
            if (!this.m_SuspendDirtyCheck)
            {
               if (!this.m_IsDirty && value)
               {
                  this.m_IsDirty = true;
                  if (Modified != null)
                  {
                     Modified(this, EventArgs.Empty);
                  }
               }
               else
               {
                  this.m_IsDirty = value;
               }
            }
         }
      }

      /// <summary>
      /// Gets or sets a value indicating whether <see cref="IsDirty"/> is updated when a project property is modifed.
      /// </summary>
      /// <value>
      /// 	<see langword="true"/>, if changes to project properties should <b>not</b> update the value of <see cref="IsDirty"/>; otherwise, <see langword="false"/>.
      /// </value>
      /// <remarks>The default value of this property is <see langword="false"/>, however it is set to <see langword="true"/> during <see cref="Read"/> so a newly loaded project is not flagged as 'dirty'</remarks>
      public bool SuspendDirtyCheck
      {
         get { return this.m_SuspendDirtyCheck; }
         set { this.m_SuspendDirtyCheck = value; }
      }

      #endregion

      /// <summary>
      /// Clears the project.
      /// </summary>
      public void Clear()
      {
         IsDirty = false;
         ProjectFile = "";
      }

      #region Read from Disk

      /// <summary>
      /// Reads an OptimaliseRing project file from disk.
      /// </summary>
      /// <param name="filename">The filename.</param>
      public void Read(string filename, Instellingen instellingen)
      {
         Clear();

         ProjectFile = Path.GetFullPath(filename);

         try
         {
            instellingen.Read(ProjectFile);
         }
         catch (Exception ex)
         {
            //Clear the project to ensure everything is back to default state
            Clear();
            ThisAppErr.Instance.Raise (3, new object[] {ex.Message , filename});
         }
      }

      #endregion

      #region Write to Disk

      /// <summary>
      /// Writes an OptimaliseRing project to a disk file.
      /// </summary>
      /// <param name="filename">The filename.</param>
      public void Write(string filename, Instellingen instellingen)
      {
         //save the previous project file location.
         //If an error occurs during serialization, we we need to restore this...
         string oldProjectFile = ProjectFile;

         //Let the project know where it is being stored. This is used when deriving
         //pathnames relative to the project file
         ProjectFile = Path.GetFullPath(filename);

         try
         {
            instellingen.Write(ProjectFile, Application.ProductName, Application.ProductVersion);
         }
         catch (Exception ex)
         {
            //ouch, something went horribly wrong!
            //restore the original filename
            ProjectFile = oldProjectFile;
            ThisAppErr.Instance.Raise (4, new object [] {ex.Message , filename});
         }
         IsDirty = false;
      }

      #endregion
   }
}
