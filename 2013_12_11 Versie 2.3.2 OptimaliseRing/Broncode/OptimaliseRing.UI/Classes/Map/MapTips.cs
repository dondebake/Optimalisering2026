#region Copyright -------------------------------------------------------
// Copyright © 2007, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.UI/Classes/Map/MapTips.cs 2     18/06/08 14:11 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using OptimaliseRing.Core;

using MapObjectsLT2;

namespace OptimaliseRing.UI
{
   public class DijkringInfo
   {
      private string m_Naam;
      private string m_Nummer;
      private string m_Deel;
      private string m_Compartnummer;
      private double m_Afstand;

      public DijkringInfo(string aNaam, string aNummer, string aDeel, string aCompartnummer, double aAfstand)
      {
         m_Naam = aNaam;
         m_Nummer = aNummer;
         m_Deel = aDeel;
         m_Compartnummer = aCompartnummer;
         m_Afstand = aAfstand;
      }

      public string Naam
      {
         get { return m_Naam; }
         set { m_Naam = value; }
      }

      public string Nummer
      {
         get { return m_Nummer; }
         set { m_Nummer = value; }
      }

      public string Deel
      {
         get { return m_Deel; }
         set { m_Deel = value; }
      }

      public string Compartnummer
      {
         get { return m_Compartnummer; }
         set { m_Compartnummer = value; }
      }

      public double Afstand
      {
         get { return m_Afstand; }
         set { m_Afstand = value; }
      }

   }

   public class MapTips
   {
      private Single m_X;                   // current x position
      private Single m_Y;                   // current y position
      private Single m_LastX;               // x position when timer starts
      private Single m_LastY;               // y position when timer starts
      private AxMapObjectsLT2.AxMap m_Map;
      private Timer m_Timer;
      private Panel m_Panel;
      private Label m_Tooltip;
      private Berekening m_Berekening;

      private MapLayer m_LookupLayer;       // layer om gebied te bepalen
      private MapLayer m_InfoLayer;         // layer om informatie op te zoeken
      private String m_FieldName;           // field to get ToolTip text from

      public MapTips(AxMapObjectsLT2.AxMap map, Timer timer, Panel panel, Label label)
      {
         m_Map = map;
         m_Timer = timer;
         m_Timer.Enabled = false;
         m_Panel = panel;
         m_Tooltip = label;

         m_Panel.Visible = false;
         m_Panel.BackColor = Color.FromKnownColor(KnownColor.Info);

         m_Tooltip.ForeColor = Color.FromKnownColor(KnownColor.InfoText);
         m_Tooltip.AutoSize = true;
         m_Tooltip.BackColor = Color.Transparent;

         m_Timer.Tick += new EventHandler(OnTimerTick);
      }

      /// <summary>
      /// Handles the Tick event of the timer control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
      private void OnTimerTick(object sender, EventArgs e)
      {
         SetLabelInfo();
      }

      /// <summary>
      /// Sets the tool tip.
      /// </summary>
      /// <value>The tool tip.</value>
      public string ToolTip
      {
         set
         {
            m_Tooltip.Text = value;
         }
      }

      /// <summary>
      /// Sets the map info layer.
      /// </summary>
      /// <value>The map info layer.</value>
      public MapLayer MapInfoLayer
      {
         set
         {
            m_InfoLayer = value;
         }
      }

      /// <summary>
      /// Sets the map lookup layer.
      /// </summary>
      /// <value>The map lookup layer.</value>
      public MapLayer MapLookupLayer
      {
         set { m_LookupLayer = value; }
      }

      /// <summary>
      /// Sets the map layer field.
      /// </summary>
      /// <value>The map layer field.</value>
      public String MapLayerField
      {
         set
         {
            m_FieldName = value;
         }
      }

      public Berekening Berekening
      {
         get { return m_Berekening; }
         set { m_Berekening = value; }
      }

      /// <summary>
      /// MouseMove event
      /// </summary>
      /// <param name="x">The x.</param>
      /// <param name="y">The y.</param>
      public void MouseMove(Single x, Single y)
      {
         m_X = x;
         m_Y = y;
         if (!m_Timer.Enabled)
         {
            if (!(x == m_LastX && y == m_LastY))
            {
               // start the timer
               m_LastX = x;
               m_LastY = y;
               m_Timer.Enabled = true;
            }
         }
         else
         {
            m_Panel.Visible = false;
         }
      }

      /// <summary>
      /// Set the caption
      /// </summary>
      /// <param name="text">The text.</param>
      private void Show(String text)
      {
         m_Tooltip.Text = text;
         m_Tooltip.Left = 50;
         m_Tooltip.Top = 0;
         m_Tooltip.TextAlign = ContentAlignment.MiddleLeft;
         m_Tooltip.BorderStyle = BorderStyle.FixedSingle;

         // position the panel

         // Bepaal left, middel of right
         int x = Convert.ToInt32(m_X - m_Map.Left);

         if (x >= 0 && x < m_Map.Width / 3)
         {
            m_Panel.Left = Convert.ToInt32(m_Map.Left + m_X);
         }
         else if (x >= m_Map.Width / 3 && x < m_Map.Width / 3 * 2)
         {
            m_Panel.Left = Convert.ToInt32(m_Map.Left + m_X - (m_Tooltip.Width / 2));
         }
         else
         {
            m_Panel.Left = Convert.ToInt32(m_Map.Left + m_X - m_Tooltip.Width);
         }

         m_Panel.Top = Convert.ToInt32(m_Map.Top + m_Y + Cursor.Current.Size.Height);
         m_Panel.Width = m_Tooltip.Width;
         m_Panel.Height = m_Tooltip.Height;

         m_Panel.Visible = true;
      }

      /// <summary>
      /// Sets the label info.
      /// </summary>
      public void SetLabelInfo()
      {
         if (m_X == m_LastX && m_Y == m_LastY)
         {
            MapObjectsLT2.Point pt = m_Map.ToMapPoint(m_X, m_Y);

            // Mouse didn't move
            m_Timer.Enabled = false;

            if (m_InfoLayer != null)
            {

               //// Bepaal van de polygonlayer (lookuplayer) op welke dijkring(nummer) is geklikt

               // Eerst gegevens uit de lookuplayer halen
               MapObjectsLT2.Recordset polyRecs = m_LookupLayer.SearchShape(pt, SearchMethodConstants.moAreaIntersect, "");
               if (polyRecs.Count == 1)
               {
                  // Tekstobject aanmaken
                  StringBuilder tooltiptekst = new StringBuilder();

                  // Haal van de linelayer (infoLayer) alle overeenkomstige records met hetzelfde dijkringnummer
                  string dijkringnaam = polyRecs.Fields.Item(m_FieldName).Value.ToString();
                  string dijkringnummer = polyRecs.Fields.Item("DIJKRINGNU").Value.ToString();

                  MapObjectsLT2.Recordset recs = m_InfoLayer.SearchExpression("Dijkringnu='"
                    + dijkringnummer + "'");

                  // Welke lijnen hebben we gevonden
                  recs.MoveFirst();

                  // Reset minimale afstand
                  double minAfstand = 100000000D;

                  // informatie opslaan
                  DijkringInfo info = null;

                  while (!recs.EOF)
                  {
                     object objShape = recs.Fields.Item("Shape").Value;
                     MapObjectsLT2.Line objLine = (MapObjectsLT2.Line)objShape;
                     double currentafstand = objLine.DistanceTo(pt);

                     // Bepaal de line die het dichts bij de muispunt zit
                     if (minAfstand > currentafstand)
                     {
                        info = new DijkringInfo(recs.Fields.Item(m_FieldName).Value.ToString()
                          , recs.Fields.Item("DIJKRINGNU").Value.ToString()
                          , recs.Fields.Item("DIJKRINGDE").Value.ToString()
                          , recs.Fields.Item("COMPARTDEE").Value.ToString()
                          , currentafstand);

                        minAfstand = currentafstand;
                     }

                     recs.MoveNext();
                  }

                  if (info != null)
                  {

                     // Controleer of er een berekening bestaat voor deze dijkringnummer (deel+comp)
                     if (info.Compartnummer.Length > 0)
                     {
                        // is verdeeld in compartimenten?
                        if (m_Berekening.Instellingen.Compartimentering.ContainsKey(info.Nummer))
                        {
                           List<Compartimenteringsdijk> compartimenteringsdijk = m_Berekening.Instellingen.Compartimentering[info.Nummer];

                           foreach (Compartimenteringsdijk dijk in compartimenteringsdijk)
                           {
                              if (dijk.Dijkdeel == info.Compartnummer)
                              {
                                 info.Nummer = dijk.DijkId;
                              }
                           }
                        }
                     }

                     // Ophalen gegevens uit berekeninglijst en kansenlijst
                     DijkringDeel dijkringdeelObject = Berekening.GetDijkringdeelById(info.Nummer, Convert.ToInt32(info.Deel));

                     if (dijkringdeelObject != null)
                     {
                       double optOverstromingskans = dijkringdeelObject.Kansen.OptimaleOverstromingskansInOptimaleOverstromingskansJaar;
                       double optOverschrijdingskans = dijkringdeelObject.Kansen.OptimaleOverschrijdingskansInOptimaleOverstromingskansJaar;

                       long len = Math.Max(
                            ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Dijkring").ToString().Length
                          , ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Deel").ToString().Length);

                       tooltiptekst.Append(string.Format("{0,-" + len.ToString() + "}: {1}", ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Dijkring").ToString(), dijkringdeelObject.DijkringNaam) + Environment.NewLine);

                       // Ook deelnaam weergeven als ongelijk aan dijkringnaam
                       if (string.Compare(dijkringdeelObject.DijkringNaam, dijkringdeelObject.Naam) != 0)
                       {
                         tooltiptekst.Append(string.Format("{0,-" + len.ToString() + "}: {1}", ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Deel").ToString(), dijkringdeelObject.Naam) + Environment.NewLine);
                       }

                       tooltiptekst.Append(Environment.NewLine);

                       string inJaar = string.Format(" in {0}", m_Berekening.OptimaleOverstromingskansenJaar);

                       len = Math.Max(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "OptimaleOverstromingskans").ToString().Length,
                         ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "OptimaleOverschrijdingskans").ToString().Length);

                       string format = !double.IsInfinity(optOverstromingskans)
                         ? "{0,-" + (len + inJaar.Length).ToString() + "}: 1/{1:F0} " + ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Jaar").ToString()
                         : "{0,-" + (len + inJaar.Length).ToString() + "}: - ";

                       tooltiptekst.Append(string.Format(format
                         , ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "OptimaleOverstromingskans").ToString()
                         + inJaar
                         , optOverstromingskans)
                         + Environment.NewLine);

                       format = !double.IsInfinity(optOverschrijdingskans)
                         ? "{0,-" + (len + inJaar.Length).ToString() + "}: 1/{1:F0} " + ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Jaar").ToString()
                         : "{0,-" + (len + inJaar.Length).ToString() + "}: - ";

                       tooltiptekst.Append(string.Format(format
                         , ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "OptimaleOverschrijdingskans").ToString()
                         + inJaar
                         , optOverschrijdingskans)
                         + Environment.NewLine);

                       tooltiptekst.Append(Environment.NewLine);
                       tooltiptekst.Append(string.Format(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "AantalTrajecten").ToString() + ": {0} ", dijkringdeelObject.Trajecten.Count) + Environment.NewLine);

                       if (dijkringdeelObject.Trajecten.Count > 0)
                       {
                         tooltiptekst.Append(string.Format("{0,-19:0} {1,"
                             + (ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "EersteInvestering").ToString().Length + 1).ToString()
                             + "} {2,6} {3,10} {4,12}",
                          ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Trajectnaam").ToString(),
                          ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Investering").ToString(),
                          ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Jaar").ToString(),
                          ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Hoogte").ToString(),
                          ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Kosten").ToString())
                          + Environment.NewLine);

                         foreach (DijkringTraject dijkringTraject in dijkringdeelObject.Trajecten)
                         {
                           if (dijkringTraject.Investeringen.Count > 0)
                           {
                             tooltiptekst.Append(string.Format("{0,-19:0} {1,"
                               + (ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "EersteInvestering").ToString().Length + 1).ToString()
                               + "} {2,6:F0} {3,6:F0}[cm] {4,8:F1}[M€]",
                               dijkringTraject.Naam.Substring(0, Math.Min(dijkringTraject.Naam.Length, 19)),
                               ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "EersteInvestering").ToString(),
                               dijkringTraject.Investeringen[0].Jaar,
                               dijkringTraject.Investeringen[0].Hoogte,
                               dijkringTraject.Investeringen[0].Kosten)
                               + Environment.NewLine);
                           }
                           else
                           {
                             tooltiptekst.Append(string.Format("{0,-19:0} {1,"
                               + (ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "EersteInvestering").ToString().Length + 1).ToString()
                               + "} {2,6} {3,10:F0} {4,12}",
                               dijkringTraject.Naam.Substring(0, Math.Min(dijkringTraject.Naam.Length, 19)),
                               ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "EersteInvestering").ToString(),
                               "-",
                               "-",
                               "-")
                               + Environment.NewLine);
                           }
                         }
                       }
                     }
                     else
                     {
                       // Geen gegevens bekend over dit gebied, toon waarde uit shapefile
                       tooltiptekst.Append(string.Format("{0}", dijkringnaam));
                     }
                  }

                  // Toon tooltiptekst
                  Show(tooltiptekst.ToString());

               }
               else
               {
                  // Er is niet op een polyobject geklikt
               }
            }
         }
         else
         {
            // start over at the current location
            m_LastX = m_X;
            m_LastY = m_Y;
         }
      }
   }
}
