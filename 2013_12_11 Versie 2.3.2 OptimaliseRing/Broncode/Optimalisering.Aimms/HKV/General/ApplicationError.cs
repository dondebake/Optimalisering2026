#region Copyright -------------------------------------------------------
// Copyright © 2005 HKV lijn in water, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : Hkv.General
//
// Author(s)  : Abe Hoekstra, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/Optimalisering.Aimms/HKV/General/ApplicationError.cs 3     31-03-09 10:30 Waterman $
// $NoKeywords: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;

namespace Hkv.General
{
  /// <summary>
  /// Class for handling errors in an application
  /// </summary>
  public class ApplicationError:IDisposable
  {
    /// <summary>
    /// Ini-file object
    /// </summary>
    Hkv.Profile.Ini _iniFile = new Hkv.Profile.Ini();
    /// <summary>
    /// Naam van de sectie waaruit de foutmeldingen worden gelezen
    /// </summary>
    String _sectionNameForErrors = "Errors";
    String _sectionNameForMessages = "Messages";
    String _sectionNameForQuestions = "Questions";

    static int _instanceCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ApplicationError"/> class.
    /// </summary>
    public ApplicationError()
    {
      _instanceCount++;
    }
    /// <summary>
    /// Releases unmanaged resources and performs other cleanup operations before the
    /// <see cref="T:Hkv.General.ApplicationError"/> is reclaimed by garbage collection.
    /// </summary>
    ~ApplicationError()
    {
      _instanceCount--;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ApplicationError"/> class.
    /// </summary>
    /// <param name="inifile">The inifile.</param>
    public ApplicationError(Hkv.Profile.Ini inifile)
    {
      this._iniFile = inifile;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ApplicationError"/> class.
    /// </summary>
    /// <param name="sectionName">Name of the section in the inifile conatining the error messages.</param>
    /// <param name="inifile">The inifile.</param>
    public ApplicationError(String sectionName, Hkv.Profile.Ini inifile)
    {
      this._sectionNameForErrors = sectionName;
      this._iniFile = inifile;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ApplicationError"/> class.
    /// </summary>
    /// <param name="sectionName">Name of the section conatining the error messages.</param>
    public ApplicationError(String sectionName)
    {
      this._sectionNameForErrors = sectionName;
    }

    /// <summary>
    /// Gets or sets the name of the section conatining the error messages.
    /// </summary>
    /// <value>The name of the section.</value>
    public String SectionName
    {
      get
      {
        return this._sectionNameForErrors;
      }
      set
      {
        this._sectionNameForErrors = value;
      }
    }

    /// <summary>
    /// Throw an exception met een eigen foutmelding
    /// </summary>
    /// <param name="errorNumber">Nummer van de fout</param>
    /// <param name="paramaterArray">Object-array met 0 1 of meer parameters die gesubstitueerd worden in de foutmelding, mag ook null zijn.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
    public void Raise(int errorNumber, object[] paramaterArray)
    {
      // Bepaal de naam van de sectie in de ini-file
      String section = _sectionNameForErrors + "-" + Thread.CurrentThread.CurrentUICulture.ToString();
      if (!this._iniFile.HasSection(section))
      {
        section = _sectionNameForErrors;
      }
      // Haal de omschrijving van de foutmelding uit de ini-file
      String key = "err." + String.Format(Thread.CurrentThread.CurrentUICulture, "{0:0000}", errorNumber);

      object iniWaarde = this._iniFile.GetValue(section, key);

      String description = "Onbekende fout:" + key;
      if (iniWaarde != null)
      {
        description = iniWaarde.ToString();
        description = SubstituteParameters(description, paramaterArray);
        // Even de new-line karakters veranderen in echte new-line's
        description = description.Replace("\\n", "\n");
      }

      throw new Hkv.General.ApplicationErrorException(description);
    }

    /// <summary>
    /// Gets the errortext
    /// </summary>
    /// <param name="errorNumber">Nummer van de fout</param>
    /// <param name="paramaterArray">Object-array met 0 1 of meer parameters die gesubstitueerd worden in de foutmelding, mag ook null zijn.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
    public string GetErrorText(int errorNumber, object[] paramaterArray)
    {
      // Bepaal de naam van de sectie in de ini-file
      String section = _sectionNameForErrors + "-" + Thread.CurrentThread.CurrentUICulture.ToString();
      if (!this._iniFile.HasSection(section))
      {
        section = _sectionNameForErrors;
      }
      // Haal de omschrijving van de foutmelding uit de ini-file
      String key = "err." + String.Format(Thread.CurrentThread.CurrentUICulture, "{0:0000}", errorNumber);

      object iniWaarde = this._iniFile.GetValue(section, key);

      String description = "Onbekende fout:" + key;
      if (iniWaarde != null)
      {
        description = iniWaarde.ToString();
        description = SubstituteParameters(description, paramaterArray);
        // Even de new-line karakters veranderen in echte new-line's
        description = description.Replace("\\n", "\n");
      }

      return description;
    }

    /// <summary>
    /// Gets the message text.
    /// </summary>
    /// <param name="msgNumber">The MSG number.</param>
    /// <param name="paramaterArray">The paramater array.</param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
    public string GetMessageText(int msgNumber, object[] paramaterArray)
    {
      // Bepaal de naam van de sectie in de ini-file
      String section = _sectionNameForMessages + "-" + Thread.CurrentThread.CurrentUICulture.ToString();
      if (!this._iniFile.HasSection(section))
      {
        section = _sectionNameForMessages;
      }
      // Haal de omschrijving van de foutmelding uit de ini-file
      String key = "msg." + String.Format(Thread.CurrentThread.CurrentUICulture, "{0:0000}", msgNumber);

      object iniWaarde = this._iniFile.GetValue(section, key);

      String description = "Onbekende message:" + key;
      if (iniWaarde != null)
      {
        description = iniWaarde.ToString();
        description = SubstituteParameters(description, paramaterArray);
        // Even de new-line karakters veranderen in echte new-line's
        description = description.Replace("\\n", "\n");
      }

      return description;
    }

    /// <summary>
    /// Toon de opgetreden fout middels een MessageBox
    /// </summary>
    /// <param name="owner">Displays the message box in front of this object</param>
    /// <param name="e">De ge-catch-te exeption</param>
    public static void Display(IWin32Window owner, Exception e)
    {
      if (e != null)
      {
        MessageBox.Show(owner, e.Message, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, GetMessageboxOptions(owner));

        //MessageBox.Show(owner, e.StackTrace, Application.ProductName,
        //    MessageBoxButtons.OK, MessageBoxIcon.Error,
        //    MessageBoxDefaultButton.Button1, GetMessageboxOptions(owner));
      }
    }

    /// <summary>
    /// Displays the specified owner.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="message">The message.</param>
    public static void Display(IWin32Window owner, string message)
    {
      if (message.Length > 0)
      {
        MessageBox.Show(owner, message, Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, GetMessageboxOptions(owner));
      }
    }

    /// <summary>
    /// Displays the error.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="errorNumber">The error number.</param>
    /// <param name="paramaterArray">The paramater array.</param>
    public void DisplayError(IWin32Window owner, int errorNumber, object[] paramaterArray)
    {
      // Bepaal de naam van de sectie in de ini-file
      String section = _sectionNameForErrors + "-" + Thread.CurrentThread.CurrentUICulture.ToString();
      if (!this._iniFile.HasSection(section))
      {
        section = _sectionNameForErrors;
      }
      // Haal de omschrijving van de foutmelding uit de ini-file
      String key = "err." + String.Format(Thread.CurrentThread.CurrentUICulture, "{0:0000}", errorNumber);

      object iniWaarde = this._iniFile.GetValue(section, key);

      String description = "Onbekende error:" + key;
      if (iniWaarde != null)
      {
        description = iniWaarde.ToString();
        description = SubstituteParameters(description, paramaterArray);
        // Even de new-line karakters veranderen in echte new-line's
        description = description.Replace("\\n", "\n");
      }

      MessageBox.Show(owner, description, Application.ProductName,
                 MessageBoxButtons.OK, MessageBoxIcon.Warning,
                 MessageBoxDefaultButton.Button1, GetMessageboxOptions(owner));

    }

    /// <summary>
    /// Displays the message.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="msgNumber">The MSG number.</param>
    /// <param name="paramaterArray">The paramater array.</param>
    public void DisplayMessage(IWin32Window owner, int msgNumber, object[] paramaterArray)
    {
      // Bepaal de naam van de sectie in de ini-file
      String section = _sectionNameForMessages + "-" + Thread.CurrentThread.CurrentUICulture.ToString();
      if (!this._iniFile.HasSection(section))
      {
        section = _sectionNameForMessages;
      }
      // Haal de omschrijving van de foutmelding uit de ini-file
      String key = "msg." + String.Format(Thread.CurrentThread.CurrentUICulture, "{0:0000}", msgNumber);

      object iniWaarde = this._iniFile.GetValue(section, key);

      String description = "Onbekende message:" + key;
      if (iniWaarde != null)
      {
        description = iniWaarde.ToString();
        description = SubstituteParameters(description, paramaterArray);
        // Even de new-line karakters veranderen in echte new-line's
        description = description.Replace("\\n", "\n");
      }

      MessageBox.Show(owner, description, Application.ProductName,
                 MessageBoxButtons.OK, MessageBoxIcon.Information,
                 MessageBoxDefaultButton.Button1, GetMessageboxOptions(owner));

    }

    /// <summary>
    /// Displays the question.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="questionNumber">The question number.</param>
    /// <param name="paramaterArray">The paramater array.</param>
    /// <returns></returns>
    public DialogResult DisplayQuestion(IWin32Window owner, int questionNumber, object[] paramaterArray)
    {
      // Bepaal de naam van de sectie in de ini-file
      String section = _sectionNameForQuestions + "-" + Thread.CurrentThread.CurrentUICulture.ToString();
      if (!this._iniFile.HasSection(section))
      {
        section = _sectionNameForQuestions;
      }
      // Haal de omschrijving van de foutmelding uit de ini-file
      String key = "question." + String.Format(Thread.CurrentThread.CurrentUICulture, "{0:0000}", questionNumber);

      object iniWaarde = this._iniFile.GetValue(section, key);

      String description = "Unknown question:" + key;
      if (iniWaarde != null)
      {
        description = iniWaarde.ToString();
        description = SubstituteParameters(description, paramaterArray);
        // Even de new-line karakters veranderen in echte new-line's
        description = description.Replace("\\n", "\n");
      }

      return MessageBox.Show(owner, description, Application.ProductName,
                 MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button2, GetMessageboxOptions(owner));

    }

    /// <summary>
    /// Substitutes the parameters.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <param name="paramaterArray">The paramater array.</param>
    /// <returns></returns>
    private static String SubstituteParameters(String description, object[] paramaterArray)
    {
      if (description != null & paramaterArray != null)
      {
        // De parameters invullen in de string
        for (int i = 0; i < paramaterArray.Length; i++)
        {
          description = description.Replace("%" + String.Format(Thread.CurrentThread.CurrentUICulture
            , "{0:#}", i + 1), paramaterArray[i].ToString());
        }
      }
      return description;
    }

    /// <summary>
    /// Gets the messagebox options.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <returns></returns>
    private static MessageBoxOptions GetMessageboxOptions(IWin32Window owner)
    {
      // To display a message box correctly for cultures that use a right to left reading order,
      // the RightAlign and RtlReading members of the MessageBoxOptions enumeration must be passed
      // to the Show method. Examine the System.Windows.Forms.Control.RightToLeft property of the
      // containing control to determine whether to use a right to left reading order.
      // If the value of the RightToLeft property is System.Windows.Forms.RightToLeft.Inherit,
      // which is the default, examine the RightToLeft property of the parent control, and so on,
      // until a value of Yes or No is found.
      MessageBoxOptions options = (MessageBoxOptions)0;
      Control someControl = owner as Control;
      if (someControl != null)
      {
        RightToLeft rightToLeftValue = someControl.RightToLeft;
        Control parent = someControl;

        while ((rightToLeftValue == RightToLeft.Inherit) &&
           (parent != null))
        {
          parent = parent.Parent;
          rightToLeftValue = parent.RightToLeft;
        }

        if (rightToLeftValue == RightToLeft.Yes)
        {
          options = MessageBoxOptions.RtlReading |
             MessageBoxOptions.RightAlign;
        }
      }
      return options;
    }

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      //throw new Exception("The method or operation is not implemented.");
    }

    #endregion
  }

  /// <summary>
  /// Exception class for ApplicationError class
  /// </summary>
  [Serializable()]
  public class ApplicationErrorException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ApplicationErrorException"/> class.
    /// </summary>
    public ApplicationErrorException()
    {
      // Add any type-specific logic, and supply the default message.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ApplicationErrorException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public ApplicationErrorException(string message)
      : base(message)
    {
      // Add any type-specific logic.
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ApplicationErrorException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ApplicationErrorException(string message, Exception innerException)
      :
       base(message, innerException)
    {
      // Add any type-specific logic for inner exceptions.
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ApplicationErrorException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
    /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
    /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
    protected ApplicationErrorException(SerializationInfo info,
       StreamingContext context)
      : base(info, context)
    {
      // Implement type-specific serialization constructor logic.
    }
  }


}
