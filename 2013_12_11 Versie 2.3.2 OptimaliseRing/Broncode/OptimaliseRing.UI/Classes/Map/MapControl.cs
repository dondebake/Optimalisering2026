using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

using OptimaliseRing.General;
using OptimaliseRing.Core;

using MapObjectsLT2;

namespace OptimaliseRing.UI
{


  /// <summary>
  ///
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="mode"></param>
  public delegate void PointerModeChangedEventHandler(object sender, PointerMode mode);

  /// <summary>
  /// Geeft een event als weergave wordt gewijzigd
  /// </summary>
  /// <param name="sender">object</param>
  /// <param name="viewMode">ViewMode</param>
  public delegate void ViewChangedEventHandler(object sender, ViewMode viewMode);

  /// <summary>
  /// Geeft een event als er op een van de custombuttons wordt geklikt.
  /// </summary>
  public delegate void CustomMenuButtonClickEventHandler(object sender, string buttonName);

  /// <summary>
  /// Map control
  /// </summary>
  [ToolboxBitmap(typeof(MapControl), "OptimaliseRing.bmp"),
   Description("OptimaliseRing.General Map Control")]
  public partial class MapControl : UserControl
  {
    private PointerMode m_CurrentMode;

    private OptimaliseRing.Profile.Profile m_Profile;
    private Berekening m_Berekening;

    private bool m_MapTipsShow;
    private MapTips m_MapTips = null;
    private string m_KaartlagenMap = null;
    private ViewMode m_ViewMode = ViewMode.Overstromingskans;

    /// <summary>
    /// OnPointerModeChanged
    /// </summary>
    public event PointerModeChangedEventHandler OnPointerModeChanged;

    /// <summary>
    /// OnViewModeChanged
    /// </summary>
    public event ViewChangedEventHandler OnViewModeChanged;

    /// <summary>
    /// Occurs when [on custom menu button click].
    /// </summary>
    public event CustomMenuButtonClickEventHandler OnCustomMenuButtonClick;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MapControl"/> class.
    /// </summary>
    public MapControl()
    {
      InitializeComponent();

      // weergavelijst
      cmbWeergave.Items.Clear();
      cmbWeergave.Items.Add(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Overstromingskans", "Overstromingskans"));
      cmbWeergave.Items.Add(ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "Overschrijdingskans", "Overschrijdingskans"));
      cmbWeergave.SelectedIndex = 0;
      cmbWeergave.SelectedIndexChanged += new EventHandler(OnCmbWeergaveSelectedIndexChanged);

      DoubleBuffered = true;

      m_CurrentMode = PointerMode.Select;

      m_MapTipsShow = false;

      m_MapTips = new MapTips(Canvas, mapTipsTimer, mapTipsPanel, mapTipsLabel);

    }

    /// <summary>
    /// Handles the SelectedIndexChanged event of the cmbWeergave control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void OnCmbWeergaveSelectedIndexChanged(object sender, EventArgs e)
    {
      if (OnViewModeChanged != null)
      {
        switch (cmbWeergave.SelectedIndex)
        {
          case 0:
            m_ViewMode = ViewMode.Overstromingskans;
            break;

          case 1:
            m_ViewMode = ViewMode.Overschrijdingskans;
            break;

          default:
            m_ViewMode = ViewMode.Overstromingskans;
            break;

        }
        OnViewModeChanged(this, m_ViewMode);
      }
    }

    /// <summary>
    /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
    /// </summary>
    /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
    public override void Refresh()
    {
      Canvas.Refresh();
    }

    /// <summary>
    /// Sets a value indicating whether [map tips show].
    /// </summary>
    /// <value><c>true</c> if [map tips show]; otherwise, <c>false</c>.</value>
    public bool MapTipsShow
    {
      set
      {
        m_MapTipsShow = value;
      }
    }

    /// <summary>
    /// Gets or sets the kaartlagen map.
    /// </summary>
    /// <value>The kaartlagen map.</value>
    public string KaartlagenMap
    {
      set { m_KaartlagenMap = value; }
      get { return m_KaartlagenMap; }
    }

    /// <summary>
    /// Sets the map tips layer.
    /// </summary>
    /// <value>The map tips layer.</value>
    public MapLayer MapTipsInfoLayer
    {
      set
      {
        MapLayer layer = (MapLayer)value;
        if (layer.Valid)
        {
          if (m_MapTips != null)
          {
            m_MapTips.MapInfoLayer = layer;
          }
        }
      }
    }

    /// <summary>
    /// Sets the map tips lookup layer.
    /// </summary>
    /// <value>The map tips lookup layer.</value>
    public MapLayer MapTipsLookupLayer
    {
      set
      {
        MapLayer layer = (MapLayer)value;
        if (layer.Valid)
        {
          if (m_MapTips != null)
          {
            m_MapTips.MapLookupLayer = layer;
          }
        }
      }
    }

    /// <summary>
    /// Sets the map tips layer field.
    /// </summary>
    /// <value>The map tips layer field.</value>
    public string MapTipsLayerField
    {
      set
      {
        if (m_MapTips != null)
        {
          m_MapTips.MapLayerField = value;
        }
      }
    }

    /// <summary>
    /// Layers the type.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    public LayerTypeConstants LayerType(int index)
    {
      LayerTypeConstants type = LayerTypeConstants.moMapLayer;

      if (Canvas.Layers != null)
      {
        if ((index >= 0) && (index < Canvas.Layers.Count))
        {
          Object objectLayer = Canvas.Layers.Item(index);
          if (objectLayer is MapLayer)
          {
            type = LayerTypeConstants.moMapLayer;
          }
          else if (objectLayer is ImageLayer)
          {
            type = LayerTypeConstants.moImageLayer;
          }
        }
      }
      return type;
    }

    /// <summary>
    /// Loads the shapes.
    /// </summary>
    public void LoadShapes()
    {
      if (m_Profile != null)
      {
        bool doorgaan = true;

        int i = 1;
        while (doorgaan)
        {
          string entry = "AchtergrondShape" + i.ToString();
          string shape = m_Profile.GetValue("AchtergrondShapes", entry, "NO_MORE");
          if (shape.Length > 0)
          {
            if (string.Compare(shape, "NO_MORE") == 0)
            {
              doorgaan = false;
            }
            else
            {
              string[] items = shape.Split("|".ToCharArray());

              if (items.Length > 0)
              {
                string fileName = OptimaliseRing.General.MyPath.AbsoluteName(items[0]);
                if (File.Exists(fileName))
                {
                  Boolean visible = OptimaliseRing.General.ConvertString.ToBoolean(items[1].ToString()); ;

                  // bepaal de extensie
                  string extension = Path.GetExtension(fileName);

                  // Add shapefile
                  // *.shp
                  if (string.Compare(extension, ".shp", true) == 0)
                  {
                    int a = OptimaliseRing.General.ConvertString.ToInt32(items[2].ToString());
                    int r = OptimaliseRing.General.ConvertString.ToInt32(items[3].ToString());
                    int g = OptimaliseRing.General.ConvertString.ToInt32(items[4].ToString());
                    int b = OptimaliseRing.General.ConvertString.ToInt32(items[5].ToString());

                    Color rgb = Color.FromArgb(a, r, g, b);

                    Int16 style = OptimaliseRing.General.ConvertString.ToInt16(items[6].ToString());

                    Int16 lineWidth = 1;
                    if (items.Length > 7)
                    {
                      lineWidth = OptimaliseRing.General.ConvertString.ToInt16(items[7].ToString());
                    }
                    AddLayer(fileName, visible, rgb, style, lineWidth);
                  }
                  else
                  {
                    // Add image layer
                    // *.tif;*.bmp;*.jpg;*.jpeg;*.tff;*.tiff
                    AddImageLayer(fileName, visible);
                  }
                }
                else
                {
                  throw new System.Exception(string.Format("Background shape file {0}\ndoes not exist", fileName));
                }
              }
            }
            i++;
          }
        }
      }
    }

    /// <summary>
    /// Sets the viewport.
    /// </summary>
    public void SetViewport()
    {
      string viewport = m_Profile.GetValue("OptimaliseRing", "Viewport", "");
      if (viewport.Length > 0)
      {
        string[] items = viewport.Split("|".ToCharArray());

        if (items.Length == 4)
        {
          ZoomToExtent();

          MapObjectsLT2.Rectangle rectangle = new MapObjectsLT2.Rectangle();

          rectangle.Left = OptimaliseRing.General.ConvertString.ToDouble(items[0]);
          rectangle.Right = OptimaliseRing.General.ConvertString.ToDouble(items[1]);
          rectangle.Top = OptimaliseRing.General.ConvertString.ToDouble(items[2]);
          rectangle.Bottom = OptimaliseRing.General.ConvertString.ToDouble(items[3]);

          Canvas.Extent = rectangle;
        }
      }
      else
      {
        ZoomToExtent();
      }
    }

    /// <summary>
    /// Saves the shapes.
    /// </summary>
    /// <param name="profile">The profile.</param>
    public void SaveShapes(OptimaliseRing.Profile.Profile profile)
    {
      int aantal = Canvas.Layers.Count;

      // Bewaar een aantal atributen van de achtergrondlayers in de ini-file

      string item = "";
      for (int i = 0; i < Canvas.Layers.Count; i++)
      {
        switch (LayerType(i))
        {
          case LayerTypeConstants.moMapLayer:
            MapLayer mapLayer = (MapLayer)Canvas.Layers.Item(i);
            item = OptimaliseRing.General.MyPath.RelativeName(mapLayer.File) + "|" + mapLayer.Visible.ToString();
            if (string.Compare(mapLayer.Tag, "ACHTERGROND", true) == 0)
            {
              // als de layer een zgn. maplayer is, dan ook nog de kleur en stijl bewaren
              Color color = OptimaliseRing.UI.ColorConverter.UInt32ToColor(mapLayer.Symbol.Color);
              if (mapLayer.LayerType == LayerTypeConstants.moMapLayer)
              {
                item += "|" + color.A.ToString() + "|" + color.R.ToString() + "|" + color.G.ToString() + "|" + color.B.ToString();
                item += "|" + mapLayer.Symbol.Style;
                item += "|" + mapLayer.Symbol.Size;
              }
              string key = "AchtergrondShape" + aantal.ToString();
              profile.SetValue("AchtergrondShapes", key, item);
            }
            break;
          case LayerTypeConstants.moImageLayer:
            ImageLayer imageLayer = (ImageLayer)Canvas.Layers.Item(i);
            if (string.Compare(imageLayer.Tag, "ACHTERGROND", true) == 0)
            {
              item = OptimaliseRing.General.MyPath.RelativeName(imageLayer.File) + "|" + imageLayer.Visible.ToString();

              string key = "AchtergrondShape" + aantal.ToString();
              profile.SetValue("AchtergrondShapes", key, item);
            }
            break;
        }
        aantal--;
      }


      // Verwijder de rest
      bool doorgaan = true;
      int ii = 1;
      while (doorgaan)
      {
        string entry = "AchtergrondShape" + ii.ToString();
        string shape = m_Profile.GetValue("AchtergrondShapes", entry, "NO_MORE");
        if (shape.Length > 0)
        {
          if (string.Compare(shape, "NO_MORE") == 0)
          {
            doorgaan = false;
          }
          else
          {
            if (ii > Canvas.Layers.Count)
            {
              m_Profile.RemoveEntry("AchtergrondShapes", entry);
            }
          }
        }
        ii++;
      }

      MapObjectsLT2.Rectangle rectangle = Canvas.Extent;

      string viewport = rectangle.Left.ToString("F2") + "|" +
        rectangle.Right.ToString("F2") + "|" +
        rectangle.Top.ToString("F2") + "|" +
        rectangle.Bottom.ToString("F2");

      profile.SetValue("OptimaliseRing", "Viewport", viewport);
    }

    /// <summary>
    /// Add a new layer to the map.  Overloaded.  If you pass it a filename,
    /// that file will be loaded.  Otherwise, a file selection box will
    /// be presented to the user for them to select a file to load.
    /// </summary>
    public void AddLayer()
    {
      string oKCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "ShapeOKCaption", "ShapeOKCaption").ToString();
      string cancelCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "CancelCaption", "CancelCaption").ToString();
      string lookInCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "LookInCaption", "LookInCaption").ToString();
      string fileNameCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FileNameCaption", "FileNameCaption").ToString();
      string filesOfTypeCaption = ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "FilesOfTypeCaption", "FilesOfTypeCaption").ToString();

      string fileName = OptimaliseRingDialog.Open(oKCaption, cancelCaption, lookInCaption, fileNameCaption, filesOfTypeCaption
         , ThisAppLanguage.Instance.GetValue("Captions:" + ThisAppCulture.Instance.Name, "ShapeOpenTitle", "ShapeOpenTitle").ToString()
         , "ESRI Shapefiles (*.shp)|*.shp|Image Files (*.tif;*.bmp;*.jpg;*.jpeg;*.tff;*.tiff)|*.tif;*.bmp;*.jpg;*.jpeg;*.tff;*.tiff"
         , "shp"
         , MyPath.AbsoluteName(ThisAppProfile.Instance.GetValue("OptimaliseRing", "AchtergrondShapes", "")));

      if (!string.IsNullOrEmpty(fileName))
      {
        if (fileName.Length > 0)
        {
          m_KaartlagenMap = Path.GetDirectoryName(fileName);
          AddLayer(fileName);
          ThisAppProfile.Instance.SetValue("OptimaliseRing", "AchtergrondShapes", MyPath.RelativeName(Path.GetDirectoryName(fileName)));
        }
      }
    }

    /// <summary>
    /// Adds the layer.
    /// </summary>
    /// <param name="layer">The layer.</param>
    public void AddLayer(Object layer)
    {
      Cursor = Cursors.AppStarting;
      Canvas.Layers.Add(layer);
      Cursor = Cursors.Default;
    }

    /// <summary>
    /// Adds the layer.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    public void AddLayer(string fileName)
    {
      Cursor = Cursors.AppStarting;

      string extension = Path.GetExtension(fileName);
      if (string.Compare(extension, ".shp", true) == 0)
      {
        MapLayer layer = new MapLayerClass();

        //layer.Name = Path.GetFileNameWithoutExtension(fileName);

        layer.File = fileName;
        layer.Tag = "ACHTERGROND";
        layer.Visible = true;

        layer.Symbol.Color = OptimaliseRing.UI.ColorConverter.ColorToUInt32(Color.FromArgb(255, 12, 112, 212));

        if (layer.Valid)
        {
          if (!Canvas.Layers.Add(layer))
          {
            throw new System.Exception(string.Format("Background shape file {0}\ncannot be added", fileName));
          }
        }
      }
      else
      {
        ImageLayer imageLayer = new ImageLayerClass();

        imageLayer.Name = Path.GetFileNameWithoutExtension(fileName);
        imageLayer.File = fileName;
        imageLayer.Tag = "ACHTERGROND";

        if (imageLayer.Valid)
        {
          if (!Canvas.Layers.Add(imageLayer))
          {
            throw new System.Exception(string.Format("Background image {0}\ncannot be added", fileName));
          }
        }
      }

      Cursor = Cursors.Default;
    }

    /// <summary>
    /// Adds the image layer.
    /// </summary>
    /// <param name="imageFile">The image file.</param>
    /// <param name="visible">if set to <c>true</c> [visible].</param>
    public void AddImageLayer(string imageFile, Boolean visible)
    {
      Cursor = Cursors.AppStarting;


      ImageLayer imageLayer = new ImageLayerClass();

      imageLayer.Name = Path.GetFileNameWithoutExtension(imageFile);
      imageLayer.File = imageFile;
      imageLayer.Visible = visible;
      imageLayer.Tag = "ACHTERGROND";

      if (imageLayer.Valid)
      {
        if (!Canvas.Layers.Add(imageLayer))
        {
          throw new System.Exception(string.Format("Background image {0}\ncannot be added", imageFile));
        }
      }
      Cursor = Cursors.Default;
    }

    /// <summary>
    /// Adds the layer.
    /// </summary>
    /// <param name="shapeFile">The shape file.</param>
    /// <param name="visible">if set to <c>true</c> [visible].</param>
    /// <param name="color">The color.</param>
    /// <param name="style">The style.</param>
    /// <param name="lineWidth">Width of the line.</param>
    public void AddLayer(string shapeFile, Boolean visible, Color color, Int16 style, Int16 lineWidth)
    {
      Cursor = Cursors.AppStarting;

      MapLayer layer = new MapLayerClass();

      layer.Name = Path.GetFileNameWithoutExtension(shapeFile);
      layer.File = shapeFile;
      layer.Tag = "ACHTERGROND";
      layer.Symbol.Color = OptimaliseRing.UI.ColorConverter.ColorToUInt32(color);
      layer.Symbol.Size = lineWidth;
      layer.Symbol.Style = style;
      layer.Visible = visible;

      if (!Canvas.Layers.Add(layer))
      {
        throw new System.Exception(string.Format("Achtergrondshape file {1}\ncannot be added", shapeFile));
      }

      Cursor = Cursors.Default;
    }

    /// <summary>
    /// Canvas_s the mouse down event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void CanvasMouseDownEvent(object sender, AxMapObjectsLT2._DMapEvents_MouseDownEvent e)
    {
      MapObjectsLT2.Rectangle rectangle;
      switch (m_CurrentMode)
      {
        case PointerMode.Select:
          Cursor.Current = Cursors.Arrow;
          break;

        case PointerMode.ZoomIn:
          Cursor.Current = Cursors.Cross;
          rectangle = Canvas.Extent;
          rectangle.ScaleRectangle(1.0 / 1.5);
          Canvas.Extent = rectangle;
          break;

        case PointerMode.ZoomOut:
          rectangle = Canvas.Extent;
          rectangle.ScaleRectangle(1.5);
          Canvas.Extent = rectangle;
          break;

        case PointerMode.ZoomArea:
          Canvas.Extent = Canvas.TrackRectangle();
          break;

        case PointerMode.Drag:
          Canvas.Pan();
          break;

        case PointerMode.World:
          ZoomToExtent();
          break;

        case PointerMode.Center:

          MapObjectsLT2.Point wP = Canvas.ToMapPoint(e.x, e.y);
          Canvas.CenterAt(wP.X, wP.Y);
          break;

      }

      if (OnPointerModeChanged != null)
      {
        OnPointerModeChanged(this, m_CurrentMode);
      }

    }

    /// <summary>
    /// Zooms to extent.
    /// </summary>
    public void ZoomToExtent()
    {
      MapObjectsLT2.Rectangle rectangle = null;

      for (int i = 0; i < Canvas.Layers.Count; i++)
      {
        switch (LayerType(i))
        {
          case LayerTypeConstants.moMapLayer:
            MapLayer mapLayer = (MapLayer)Canvas.Layers.Item(i);
            // haal  de rechthoekinfo van layer
            rectangle = mapLayer.Extent;
            break;
          case LayerTypeConstants.moImageLayer:
            ImageLayer imageLayer = (ImageLayer)Canvas.Layers.Item(i);
            // haal  de rechthoekinfo van layer
            rectangle = imageLayer.Extent;
            break;
        }

        // maak de rechthoek aan alle kanten 5 procent groter
        double width = Math.Max(25000, rectangle.Right - rectangle.Left);
        rectangle.Left = rectangle.Left - 0.05 * width;
        rectangle.Right = rectangle.Right + 0.05 * width;

        double height = Math.Max(25000, rectangle.Top - rectangle.Bottom);
        rectangle.Bottom = rectangle.Bottom - 0.05 * height;
        rectangle.Top = rectangle.Top + 0.05 * height;

        // geef het zichtbare gedeelte deze maat
        Canvas.Extent = rectangle;
      }
    }

    /// <summary>
    /// Gets or sets the current mode.
    /// </summary>
    /// <value>The current mode.</value>
    public PointerMode CurrentMode
    {
      get
      {
        return m_CurrentMode;
      }
      set
      {
        m_CurrentMode = value;

        uxSelectButton.Checked = false;
        uxZoomInButton.Checked = false;
        uxZoomOutButton.Checked = false;
        uzZoomAreaButton.Checked = false;
        uxDragButton.Checked = false;
        uxCenterButton.Checked = false;
        uxWorldButton.Checked = false;

        Canvas.MousePointer = MousePointerConstants.moDefault;

        switch (m_CurrentMode)
        {
          case PointerMode.Select:
            uxSelectButton.Checked = true;
            break;

          case PointerMode.ZoomIn:
            uxZoomInButton.Checked = true;
            Canvas.MousePointer = MousePointerConstants.moZoomIn;
            break;

          case PointerMode.ZoomOut:
            uxZoomOutButton.Checked = true;
            Canvas.MousePointer = MousePointerConstants.moZoomOut;
            break;

          case PointerMode.ZoomArea:
            uzZoomAreaButton.Checked = true;
            Canvas.MousePointer = MousePointerConstants.moZoom;
            break;

          case PointerMode.Drag:
            uxDragButton.Checked = true;
            Canvas.MousePointer = MousePointerConstants.moPan;
            break;

          case PointerMode.Center:
            uxCenterButton.Checked = true;
            Canvas.MousePointer = MousePointerConstants.moCross;
            break;

          case PointerMode.World:
            uxWorldButton.Checked = true;
            Canvas.MousePointer = MousePointerConstants.moDefault;
            break;

          default:
            break;
        }

      }
    }

    /// <summary>
    /// Handles the Click event of the tscMap control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    private void TscMapClick(object sender, EventArgs e)
    {
      if (sender == uxSelectButton)
      {
        CurrentMode = PointerMode.Select;
      }
      else if (sender == uxZoomInButton)
      {
        CurrentMode = PointerMode.ZoomIn;
      }
      else if (sender == uxZoomOutButton)
      {
        CurrentMode = PointerMode.ZoomOut;
      }
      else if (sender == uzZoomAreaButton)
      {
        CurrentMode = PointerMode.ZoomArea;
      }
      else if (sender == uxDragButton)
      {
        CurrentMode = PointerMode.Drag;
      }
      else if (sender == uxCenterButton)
      {
        CurrentMode = PointerMode.Center;
      }
      else if (sender == uxWorldButton)
      {
        CurrentMode = PointerMode.World;
        ZoomToExtent();
        CurrentMode = PointerMode.Select;
      }
      else if (sender == uxKaartlagenButton)
      {
        Kaartlagen();
      }
      else if (sender == uxPrintButton)
      {
        Print();
      }
      else if (sender == uxResultaatButton)
      {
        if (OnCustomMenuButtonClick != null)
        {
          OnCustomMenuButtonClick(sender, uxResultaatButton.Name);
        }
      }

      if (OnPointerModeChanged != null)
      {
        OnPointerModeChanged(this, CurrentMode);
      }
    }

    /// <summary>
    /// Kaartlagens this instance.
    /// </summary>
    public void Kaartlagen()
    {
      KaartlagenForm kaartlagenForm = new KaartlagenForm(this);

      if (kaartlagenForm.ShowDialog(this) == DialogResult.OK)
      {
        Refresh();
      }
      kaartlagenForm.Dispose();
    }

    /// <summary>
    /// Prints this instance.
    /// </summary>
    public void Print()
    {
      //Canvas.OutputMap(hDC);
    }

    private void CanvasMouseMoveEvent(object sender, AxMapObjectsLT2._DMapEvents_MouseMoveEvent e)
    {
      MapObjectsLT2.Point mapPoint = Canvas.ToMapPoint(e.x, e.y);

      uxCoordinatesLabel.Text = string.Format("x = {0:F2}, y = {1:F2}", mapPoint.X, mapPoint.Y);
      if (m_MapTipsShow)
      {
        m_MapTips.MouseMove((float)e.x, (float)e.y);
      }
    }

    /// <summary>
    /// Sets a value indicating whether [toolbar visible].
    /// </summary>
    /// <value><c>true</c> if [toolbar visible]; otherwise, <c>false</c>.</value>
    public Boolean ToolbarVisible
    {
      set
      {
        uxTopoToolStrip.Visible = value;
      }
    }

    /// <summary>
    /// Gets or sets the profile.
    /// </summary>
    /// <value>The profile.</value>
    public OptimaliseRing.Profile.Profile Profile
    {
      get { return m_Profile; }
      set { m_Profile = value; }
    }

    /// <summary>
    /// Sets the berekening.
    /// </summary>
    /// <value>The berekening.</value>
    public Berekening Berekening
    {
      get { return m_Berekening; }
      set
      {
        m_Berekening = value;
        if (m_MapTips != null)
        {
          m_MapTips.Berekening = m_Berekening;
        }
      }
    }

    /// <summary>
    /// Sets Of er een berekening/ resultaat in het geheugen is geladen
    /// </summary>
    /// <value><c>true</c> if [berekening aanwezig]; otherwise, <c>false</c>.</value>
    public bool BerekeningAanwezig
    {
      set
      {
        this.uxResultaatButton.Enabled = value;
      }
    }

    /// <summary>
    /// This routine converts map units to device units (pixels suitable for GDI calls)
    /// </summary>
    /// <param name="x">the x coordinate to convert in map units</param>
    /// <param name="y">the y coordinate to convert in map units</param>
    /// <param name="xPos">x coordinate in pixels</param>
    /// <param name="yPos">y coordinate in pixels</param>
    public void FromMapPoint(double x, double y, ref double xPos, ref double yPos)
    {
      xPos = Canvas.Left + ((x - Canvas.Extent.Left) / Canvas.Extent.Width * Canvas.Width);
      yPos = Canvas.Bottom - ((y - Canvas.Extent.Bottom) / Canvas.Extent.Height * Canvas.Height);
    }

    /// <summary>
    /// Gets the weergave.
    /// </summary>
    /// <value>The weergave.</value>
    public ViewMode Weergave
    {
      get { return m_ViewMode; }
    }

  }
}
