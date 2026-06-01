#region Copyright -------------------------------------------------------
// Copyright © 2008, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Johan Ansink, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/General/Xml.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Xml;

namespace OptimaliseRing.General
{
  sealed public class Xml
  {
    public static string ReadElement(XmlReader xmlReader, string name)
    {
      string element = "";
      xmlReader.Read();
      if (string.Compare(xmlReader.Name, name, true) == 0)
      {
        element = xmlReader.ReadString();
      }
      return element;
    }

  }
}
