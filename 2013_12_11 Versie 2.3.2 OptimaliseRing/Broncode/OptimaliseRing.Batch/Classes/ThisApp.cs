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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Batch/Classes/ThisApp.cs 2     18/06/08 14:11 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
using System.Globalization;

using OptimaliseRing.General;

namespace OptimaliseRing.Batch
{
   #region ThisAppLanguage
   /// <summary>
   /// Application language profile
   /// </summary>
   public sealed class ThisAppLanguage
   {
      private static volatile OptimaliseRing.Profile.Ini m_Instance;
      private static object m_SyncRoot = new Object();

      /// <summary>
      /// Maak een nieuwe instantiatie van de  <see cref="T:ThisAppLanguage"/> class.
      /// </summary>
      private ThisAppLanguage() { }

      /// <summary>
      /// Gets the instance of ThisAppLanguage
      /// </summary>
      public static OptimaliseRing.Profile.Ini Instance
      {
         get
         {
            if (m_Instance == null)
            {
               lock (m_SyncRoot)
               {
                  if (m_Instance == null)
                     m_Instance = new OptimaliseRing.Profile.Ini(Path.Combine(Application.StartupPath, "Language.ini"));
               }
            }

            return m_Instance;
         }
      }
   }
   #endregion

   #region ThisAppProfile
   /// <summary>
   /// Application profile
   /// </summary>
   public sealed class ThisAppProfile
   {
      private static volatile OptimaliseRing.Profile.Ini m_Instance;
      private static object m_SyncRoot = new Object();

      /// <summary>
      /// Maak een nieuwe instantiatie van de  <see cref="T:ThisAppProfile"/> class.
      /// </summary>
      private ThisAppProfile() { }

      /// <summary>
      /// Gets the instance of ApplicationError
      /// </summary>
      public static OptimaliseRing.Profile.Ini Instance
      {
         get
         {
            if (m_Instance == null)
            {
               lock (m_SyncRoot)
               {
                  if (m_Instance == null)
                  {
                     m_Instance = new OptimaliseRing.Profile.Ini(Path.Combine(Application.StartupPath, "OptimaliseRing.ini"));
                  }
               }
            }

            return m_Instance;
         }
      }
   }
   #endregion

   #region ThisAppCulture
   /// <summary>
   /// Application error
   /// </summary>
   public sealed class ThisAppCulture
   {
      private static volatile CultureInfo m_Instance;
      private static object m_SyncRoot = new Object();

      /// <summary>
      /// Maak een nieuwe instantiatie van de  <see cref="T:ThisAppCulture"/> class.
      /// </summary>
      private ThisAppCulture() { }

      /// <summary>
      /// Gets the instance of ApplicationError
      /// </summary>
      public static CultureInfo Instance
      {
         get
         {
            if (m_Instance == null)
            {
               lock (m_SyncRoot)
               {
                  if (m_Instance == null)
                     m_Instance = new CultureInfo(ThisAppProfile.Instance.GetValue("OptimaliseRing", "Taal", "en-GB"));
               }
            }

            return m_Instance;
         }
      }
   }
   #endregion

   #region ThisAppErr
   /// <summary>
   /// Application error
   /// </summary>
   public sealed class ThisAppErr
   {
      private static volatile OptimaliseRing.General.ApplicationError m_Instance;
      private static object m_SyncRoot = new Object();

      /// <summary>
      /// Maak een nieuwe instantiatie van de  <see cref="T:ThisAppErr"/> class.
      /// </summary>
      private ThisAppErr() { }

      /// <summary>
      /// Gets the instance of ApplicationError
      /// </summary>
      public static OptimaliseRing.General.ApplicationError Instance
      {
         get
         {
            if (m_Instance == null)
            {
               lock (m_SyncRoot)
               {
                  if (m_Instance == null)
                     m_Instance = new OptimaliseRing.General.ApplicationError();
               }
            }

            return m_Instance;
         }
      }
   }
   #endregion

   #region ThisAppWorkingDirectory
   /// <summary>
   /// Application working directory
   /// </summary>
   public sealed class ThisAppWorkingDirectory
   {
      private static volatile WorkingDirectory m_Instance;
      private static object m_SyncRoot = new Object();

      /// <summary>
      /// Maak een nieuwe instantiatie van de  <see cref="T:ThisAppWorkingDirectory"/> class.
      /// </summary>
      private ThisAppWorkingDirectory() { }

      /// <summary>
      /// Gets the instance of ApplicationError
      /// </summary>
      public static WorkingDirectory Instance
      {
         get
         {
            if (m_Instance == null)
            {
               lock (m_SyncRoot)
               {
                  if (m_Instance == null)
                     m_Instance = new WorkingDirectory();
               }
            }

            return m_Instance;
         }
      }

      /// <summary>
      ///
      /// </summary>
      public sealed class WorkingDirectory
      {
         string m_BatchMap = MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "BatchMap", @".\Batch"));

         /// <summary>
         /// Gets or sets the batchMap.
         /// </summary>
         /// <value>The batchMap.</value>
         public string BatchMap
         {
            get { return m_BatchMap; }
            set
            {
               m_BatchMap = value;
            }
         }


         /// <summary>
         /// Maak een nieuwe instantiatie van de  <see cref="T:WorkingDirectory"/> class.
         /// </summary>
         public WorkingDirectory()
         {
            if (!Directory.Exists(this.BatchMap))
            {
               // map bestaat niet, maak een map batchMap onder de programmadirectory
               string tmp = System.IO.Path.Combine(MyPath.ProgramDirectoryName(), "Batch");
               if (!Directory.Exists(tmp))
               {
                  Directory.CreateDirectory(tmp);
               }
               this.BatchMap = tmp;
            }
            ThisAppProfile.Instance.SetValue("OptimaliseRing", "BatchMap", MyPath.RelativeName(this.m_BatchMap));
         }
      }
   }
   #endregion
}
