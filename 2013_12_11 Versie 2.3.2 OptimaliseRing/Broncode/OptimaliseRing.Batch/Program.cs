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
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.Batch/Program.cs 1     16/06/08 10:23 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;
using System.Resources;

using OptimaliseRing;
using OptimaliseRing.General;

namespace OptimaliseRing.Batch
{
   /// <summary>
   /// Hoofdprogramma OptimaliseRingBatch
   /// </summary>
   public static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      public static void Main()
      {
         // Default taal is engels
         CultureInfo cultureInfo = ThisAppCulture.Instance;

         ThisAppErr.Instance.CultureInfo = cultureInfo;
         ThisAppErr.Instance.Profile = ThisAppProfile.Instance;

         System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnCurrentDomainUnhandledException);
         Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(OnApplicationThreadException);

         try
         {
            SingleApplication.Run(new BatchForm());
         }

         catch (ApplicationErrorException ex)
         {

            ThisAppErr.Instance.Display(null, ex);
         }

         catch (Exception ex)
         {
            ThisAppErr.Instance.Display(null, ex);
         }

      }

      /// <summary>
      /// Handles the UnhandledException event of the CurrentDomain control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
      private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
      {
         Exception ex = (Exception)e.ExceptionObject;
         HandleAllUnknownErrors(sender.ToString(), ex);
      }

      /// <summary>
      /// Handles the ThreadException event of the Application control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.Threading.ThreadExceptionEventArgs"/> instance containing the event data.</param>
      private static void OnApplicationThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
      {
         HandleAllUnknownErrors(sender.ToString(), e.Exception);
      }

      /// <summary>
      /// Handles all unknown errors.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="ex">The ex.</param>
      private static void HandleAllUnknownErrors(string sender, Exception ex)
      {
         MessageBox.Show("Error occured: (continuing after error...)\n" + ex.ToString(), "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

   }
}
