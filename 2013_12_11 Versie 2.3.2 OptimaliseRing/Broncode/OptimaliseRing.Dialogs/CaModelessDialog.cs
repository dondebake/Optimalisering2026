//---------------------------------------------------------------------------
// Dialog Workshop for .NET Framework
// Copyright (c) COMPONENTAGE Software,
// all rights reserved
//
// http://www.componentage.com
//
// All files included into Dialog Workshop.NET source code package,
// remain COMPONENTAGE's exclusive property. Regardless of
// any modifications that you make, you may not distribute
// or publish any source code files.
//---------------------------------------------------------------------------

using System;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;

using ComponentAge.PlatformServices;

namespace ComponentAge.Dialogs
{
    //[Description(DialogConsts.strDialogClosedEventArgs_TypeDesc)]
    public class DialogClosedEventArgs : System.EventArgs
    {
        // fields
        private DialogResult _res = DialogResult.None;

        // constructor
        //[Description(DialogConsts.strDialogClosedEventArgs_Ctor)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DialogClosedEventArgs (DialogResult res)
        {
            _res = res;
        }

        // properties
        //[Description(DialogConsts.strDialogClosedEventArgs_DialogResult)]
        public DialogResult DialogResult { get {return _res;} }
    }

    //[Description(DialogConsts.strDialogClosedEventHandler_TypeDesc)]
    public delegate void DialogClosedEventHandler (object sender, DialogClosedEventArgs e);

    [ToolboxItem(false)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("Represents modeless view options of the dialog box.")]
    internal class CaModelessViewOptions
    {
        private Control _parent = null;
        private int _x = 0;
        private int _y = 0;
        private DockStyle _dock = DockStyle.None;
        private bool _insideForm;

        internal CaModelessViewOptions ()
        {
        }

        internal void Reset()
        {
            Parent = null;
            Left = 0;
            Top = 0;
            Dock = DockStyle.None;
            InsideMode = false;
        }

        [DefaultValue(null)]
        [Description("The parent of the dialog window.")]
        public Control Parent { get {return _parent;} set {_parent = value;} }
        [DefaultValue(0)]
        [Description("Gets or sets the x-coordinate of the left edge of the dialog box window.")]
        public int Left { get {return _x;} set {_x = value;} }
        [DefaultValue(0)]
        [Description("Gets or sets the x-coordinate of the top edge of the dialog box window.")]
        public int Top { get {return _y;} set {_y = value;} }
        [Description("The docking location of the control, indicating which borders are docked to the container.")]
        [DefaultValue(DockStyle.None)]
        public DockStyle Dock { get {return _dock;} set {_dock = value;} }
        [Description("If True, dialog is inside WinForm instance.")]
        [DefaultValue(false)]
        public bool InsideMode { get { return _insideForm; } set { _insideForm = value; } }
    }



    [Description("Special class provided for displaying dialog boxes as modeless windows.")]
    [LicenseProvider(typeof(LicUuidLicenseProvider))]
    [Guid("D34B6ED7-1D5E-4241-8D20-D102AE352312")]
    [System.Drawing.ToolboxBitmap(typeof(CaModelessDialog))]
	public class CaModelessDialog : Component
	{
        // member fields
        private ICommonDialog _dlg;
        private Thread _d;
        private DialogResult _result = DialogResult.None;
        private double _t = 0.0;
        private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private System.Threading.Timer _timer1;
        private bool _autoClose = true;
        private bool _autoClosePopups = true;
        private Thread _currThread = null;
        private CaModelessViewOptions _modelessViewInfo = new CaModelessViewOptions();
        private UserControl _modelessParent = new UserControl();
        private UserControl _ctrlInDialogThread = null;

        // constructor
		public CaModelessDialog(ICommonDialog dlg)
		{
            _dlg = dlg;
            _modelessParent.Name = "modelessParent";
        }

        public CaModelessDialog()
        {
            _modelessParent.Name = "modelessParent";
        }

        public CaModelessDialog(System.ComponentModel.IContainer container)
        {
            container.Add(this);
            _modelessParent.Name = "modelessParent";
        }

        // methods
        public void OnModelessParentResize(object sender, System.EventArgs e)
        {
            if (_ctrlInDialogThread != null && _ctrlInDialogThread.Created)
            {
                _ctrlInDialogThread.Invoke(new EventHandler(OnResize), new object[] { null, null });
            }
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            if (_modelessViewInfo.InsideMode)
            {
                _ctrlInDialogThread.Width = _modelessParent.Width - 4;
                _ctrlInDialogThread.Height = _modelessParent.Height - 4;
                Win32.SetWindowPos(_dlg.Handle, 0, -2, -2, 0, 0, Win32.SWP_FRAMECHANGED | Win32.SWP_NOSIZE | Win32.SWP_NOZORDER);
            }
        }

        internal void InitDialog (object sender, System.EventArgs e)
        {
            //MessageBox.Show(_modelessViewInfo.Parent.Name);
            //_modelessViewInfo.Parent.CreateControl();
            _modelessViewInfo.Parent.Invoke(new EventHandler(InvokeInitDialog), new object[] { null, null });

            if (_ctrlInDialogThread.InvokeRequired)
            {
                _ctrlInDialogThread.Invoke(new EventHandler(SetParent2), new object[] { });
            }
            else
            {
                Win32.SetParent(_dlg.Handle, _ctrlInDialogThread.Handle);
            }
            Win32.SetWindowLong(_dlg.Handle, Win32.GWL_STYLE, Win32.WS_CHILD | Win32.WS_CLIPCHILDREN);
            Win32.SetWindowPos(_dlg.Handle, 0, -2, -2, 0, 0, Win32.SWP_FRAMECHANGED | Win32.SWP_NOSIZE | Win32.SWP_NOZORDER);


            if (_ctrlInDialogThread.InvokeRequired)
            {
                _ctrlInDialogThread.Invoke(new EventHandler(ResizeControlInDialogThread), new object[] { });
            }
            else
            {
                ResizeControlInDialogThread(null, null);
            }

            _modelessParent.Invoke(new EventHandler(InvokeInitDialog2), new object[] { null, null });
        }

        private void InvokeInitDialog2(object sender, EventArgs e)
        {
            _modelessParent.Width = _ctrlInDialogThread.Width;
            _modelessParent.Height = _ctrlInDialogThread.Height;
        }

        private void InvokeInitDialog(object sender, System.EventArgs e)
        {
            Win32.RECT r = new Win32.RECT(0, 0, 0, 0);
            Win32.GetClientRect(_dlg.Handle, ref r);

            _modelessParent.Left = _modelessViewInfo.Left;
            _modelessParent.Top = _modelessViewInfo.Top;
            _modelessParent.Width = r.right - r.left - 4;
            _modelessParent.Height = r.bottom - r.top - 4;

            _ctrlInDialogThread.Parent = _modelessParent;
        }

        [Description("Shows the dialog box specified in the Dialog property as modeless window.")]
        public void Show ()
        {
            _modelessViewInfo.Parent = null;
            _modelessViewInfo.InsideMode = false;

            _result = DialogResult.None;

            _dlg.IsModeless = true;
            _dlg.InitDialog -= new System.EventHandler(InitDialog);
            _dlg.ModelessDialog = this;

            _currThread = Thread.CurrentThread;

            if (_d == null || !_d.IsAlive)
            {
                _d = new Thread(new ThreadStart(Start));
                _d.IsBackground = true;
#if DOTNET2
                _d.SetApartmentState(ApartmentState.STA);
#else
                _d.ApartmentState = ApartmentState.STA;
#endif
                _d.Start();
            }
            else
                Win32.BringWindowToTop(_dlg.Handle);
        }
        [Description("Shows the dialog box specified in the Dialog property as modeless view inside other form.")]
        public void Show (Control parent)
        {
            Show(parent, 0, 0);
        }

        [Description("Shows the dialog box specified in the Dialog property as modeless view inside other form.")]
        public void Show (Control parent, int left, int top)
        {
            if (_d != null && _d.IsAlive)
            {
                _d.Abort();
                _d = null;
            }

            if (_dlg == null)
            {
                throw new Exception("Dialog property not specified in the CaModelessDialog component.");
            }

            if (_dlg is CaFileDialogBase)
            {
                CaFileDialogBase fileDlg = _dlg as CaFileDialogBase;
                //fileDlg.EnableSizing = false;
            }

            _modelessViewInfo.InsideMode = true;
            _modelessViewInfo.Parent = parent;
            _modelessViewInfo.Left = left;
            _modelessViewInfo.Top = top;

            _result = DialogResult.None;

            _modelessParent.Visible = true;
            _modelessParent.Parent = parent;
            _dlg.ModelessParentControl = _modelessParent;
            _dlg.IsModeless = true;
            _dlg.InitDialog += new System.EventHandler(InitDialog);
            _dlg.ModelessDialog = this;
            _currThread = Thread.CurrentThread;

            if (_d == null || !_d.IsAlive)
            {
                _d = new Thread(new ThreadStart(Start));
                _d.IsBackground = true;
#if DOTNET2
                _d.SetApartmentState(ApartmentState.STA);
#else
                _d.ApartmentState = ApartmentState.STA;
#endif
                _d.Start();
            }
            else
                Win32.BringWindowToTop(_dlg.Handle);
        }
        [Description("Hides the dialog box specified in the Dialog property.")]
        public void Hide ()
        {
            if (AutoClosePopups)
            {
                bool closed = true;
                while (closed)
                {
                    closed = false;
                    IntPtr popup = Win32.GetWindow(_dlg.Handle, Win32.GW_ENABLEDPOPUP);
                    if (popup != _dlg.Handle && popup != IntPtr.Zero)
                    {
                        Win32.SendMessageA(popup, Win32.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        closed = true;
                    }
                }
            }
            _dlg.CloseDialog(DialogResult.Cancel);
        }

        private void SetParent (object sender, System.EventArgs e)
        {
            _ctrlInDialogThread.Parent = _modelessParent;
        }

        private void SetParent2(object sender, System.EventArgs e)
        {
            Win32.SetParent(_dlg.Handle, _ctrlInDialogThread.Handle);
        }

        private void ResizeControlInDialogThread(object sender, System.EventArgs e)
        {
            Win32.RECT rect = new Win32.RECT(0, 0, 0, 0);
            Win32.GetWindowRect(_dlg.Handle, ref rect);
            _ctrlInDialogThread.Width = rect.right - rect.left - 4;
            _ctrlInDialogThread.Height = rect.bottom - rect.top - 4;
        }

        internal void Start ()
        {
            if (_modelessViewInfo.Parent != null)
            {
                _ctrlInDialogThread = new UserControl();
                _ctrlInDialogThread.Name = "ctrlInDialogThread";
            }

            Application.DoEvents();

            if (Timeout > 0.0)
            {
                _timer.Interval = (int)(Timeout * 1000.0);
                _timer.Tick += new System.EventHandler(OnTimer);
                _timer.Start();
            }
            if (_autoClose)
            {
                _timer1 = new System.Threading.Timer(new TimerCallback(OnTimer1), null, 0, 100);
            }

            _result = _dlg.ShowDialog();

            if (Site != null && Site.Component is Control)
            {
                Control c = (Control)(Site.Component);
                c.Invoke(new System.EventHandler(OnCloseDialog), new object[] {null, null});
            }
            else
            {
                OnCloseDialog(null, null);
            }

            if (_ctrlInDialogThread.InvokeRequired)
            {
                _ctrlInDialogThread.Invoke(new EventHandler(HideControlInDialogThread), new object[] { });
            }
            else
            {
                HideControlInDialogThread(null, null);
            }
        }
        private void HideControlInDialogThread(object sender, System.EventArgs e)
        {
            if (_ctrlInDialogThread != null)
            {
                _ctrlInDialogThread.Visible = false;
                _ctrlInDialogThread.Parent = null;
            }
        }
        internal void OnCloseDialog (object sender, System.EventArgs e)
        {
            _dlg.ModelessParentControl = null;
            _dlg.ModelessDialog = null;
            _dlg.IsModeless = false;

            if (_modelessParent.InvokeRequired)
            {
                _modelessParent.Invoke(new EventHandler(HideModelessParent), new object[] { });
            }
            else
            {
                HideModelessParent(null, null);
            }

            _modelessViewInfo.Reset();

            if (DialogClosed != null)
                DialogClosed(this, new DialogClosedEventArgs(_result));
        }
        private void HideModelessParent(object sender, System.EventArgs e)
        {
            _modelessParent.Visible = false;

            if (_modelessParent.Parent != null)
            {
                Control parent = _modelessParent.Parent;
                parent.Controls.Remove(_modelessParent);
                _modelessParent.Parent = null;
            }
        }
        internal void OnTimer (object sender, System.EventArgs e)
        {
            _timer.Enabled = false;
            Hide();
        }
        internal void OnTimer1 (object obj)
        {
            if ((_currThread.ThreadState & ThreadState.Stopped) != 0)
            {
                _timer1.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                _timer1.Dispose();
                _timer1 = null;
                Hide();
            }
        }

        // events
        [Description("Occurs when modeless dialog box is closed. DialogResult property shows how the dialog was closed (via OK or Cancel, etc. button).")]
        public event DialogClosedEventHandler DialogClosed;

        // properties
        [Description("Gets or sets the dialog box which should be displayed in the modeless mode.")]
        public ICommonDialog Dialog  {get {return _dlg;} set {_dlg = value;} }
        [Description("Gets last DialogResult value returned from the dialog box.")]
        [Browsable(false)]
        public DialogResult DialogResult  {get {return _result;} }
        [DefaultValue(0.0)]
        [Description("If not zero, gets or sets a value indicating when modeless dialog should be closed automatically with Cancel modal result, without waiting for user input.")]
        public double Timeout { get {return _t;} set {_t = value;} }
        [Description("If True, the modeless dialog is closed when parent thread (or dialog component's owner form) is terminated/closed.")]
        [DefaultValue(true)]
        public bool AutoCloseOnExit { get {return _autoClose;} set {_autoClose = value;} }
        [Description("If True, all enabled popup windows owned by the dialog are closed when Hide() method is calling or when Timeout expired.")]
        [DefaultValue(true)]
        public bool AutoClosePopups { get {return _autoClosePopups;} set {_autoClosePopups = value;} }
        [Description("Gets the control which is actual parent of the dialog box window in the modeless view.")]
        public UserControl ModelessViewSite { get {return _modelessParent;} }
    }

}
