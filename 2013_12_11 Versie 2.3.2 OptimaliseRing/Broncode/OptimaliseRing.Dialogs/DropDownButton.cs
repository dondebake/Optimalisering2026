using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

#if DOTNET2
using System.Windows.Forms.VisualStyles;
#endif


namespace ComponentAge.Dialogs
{
	internal class DropDownButton : System.Windows.Forms.Button
	{
        // member fields
        private ContextMenu _buttonMenu = null;
#if DOTNET2
        private PushButtonState _buttonState = PushButtonState.Normal;
#else
        private ButtonState _buttonState = ButtonState.Normal;
#endif

#if DOTNET2
        private ContextMenuStrip _buttonMenuStrip;
#endif

        // constructor
		public DropDownButton() : base()
		{
            Text = "OK";
		}

        // overrides
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

#if DOTNET2
            if (_buttonMenuStrip != null)
            {
                _buttonMenuStrip.Close();
            }
#endif
        }

        protected override void OnClick(EventArgs e)
        {
#if DOTNET2
            if (_buttonMenuStrip != null && _buttonMenuStrip.Visible)
            {
                return;
            }
#endif

            base.OnClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            bool menuShown = false;

#if DOTNET2
            if (_buttonMenu != null && _buttonMenuStrip == null)
            {
                if (e.X > ClientSize.Width - 13)
                {
                    _buttonMenu.Show(this, new Point(0, ClientSize.Height));
                    menuShown = true;
                }
            }
            else if (_buttonMenuStrip != null)
            {
                if (e.X > ClientSize.Width - 13)
                {
                    _buttonMenuStrip.Show(this, new Point(0, ClientSize.Height));
                    menuShown = true;
                }
            }
#else
            if (e.X > ClientSize.Width - 13 && _buttonMenu != null)
            {
                _buttonMenu.Show(this, new Point(0, ClientSize.Height));
                menuShown = true;
            }
#endif

            if (!menuShown)
            {
#if DOTNET2
            _buttonState = PushButtonState.Pressed;
#else
                _buttonState = ButtonState.Pushed;
#endif
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
#if DOTNET2
            _buttonState = PushButtonState.Normal;
#else
            _buttonState = ButtonState.Normal;
#endif

            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
#if DOTNET2
            _buttonState = PushButtonState.Normal;
#else
            _buttonState = ButtonState.Normal;
#endif

            base.OnMouseUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
#if DOTNET2
            _buttonState = PushButtonState.Pressed;
#else
            _buttonState = ButtonState.Pushed;
#endif

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
#if DOTNET2
            _buttonState = PushButtonState.Normal;
#else
            _buttonState = ButtonState.Normal;
#endif

            base.OnKeyUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Focused)
            {
#if !DOTNET2
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, SystemColors.ControlText, ButtonBorderStyle.Solid);
#endif

                Rectangle r1 = new Rectangle(e.ClipRectangle.Location, e.ClipRectangle.Size);

#if DOTNET2
                ButtonRenderer.DrawButton(e.Graphics, r1, _buttonState);
#else
                r1.Width = r1.Width - 2;
                r1.Height = r1.Height - 2;
                r1.X++;
                r1.Y++;

                ControlPaint.DrawButton(e.Graphics, r1, _buttonState);
#endif

                Rectangle focuRect = new Rectangle();
                focuRect.X = 4;
                focuRect.Y = 4;
                focuRect.Height = ClientSize.Height - 8;
                focuRect.Width = ClientSize.Width - 21;
                ControlPaint.DrawFocusRectangle(e.Graphics, focuRect, SystemColors.WindowText, SystemColors.Control);
            }
            else
            {
#if DOTNET2
                ButtonRenderer.DrawButton(e.Graphics, e.ClipRectangle, _buttonState);
#else
                ControlPaint.DrawButton(e.Graphics, e.ClipRectangle, _buttonState);
#endif
            }

#if DOTNET2
            if (_buttonMenu != null || _buttonMenuStrip != null)
#else
            if (_buttonMenu != null)
#endif
            {
                int x = ClientSize.Width - 11;
                int y = ClientSize.Height / 2 - 2;

                e.Graphics.DrawLine(new Pen(SystemColors.ControlDark), x - 4, 1, x - 4, ClientSize.Height - 2);

                GraphicsPath path = new GraphicsPath();
                path.AddLine(x, y, x + 7, y);
                path.AddLine(x + 7, y, x + 3, y + 4);
                path.AddLine(x + 3, y + 4, x, y);
                e.Graphics.FillPath(new SolidBrush(this.ForeColor), path);
            }

            SizeF textSize = e.Graphics.MeasureString(Text, Font, ClientSize.Width - 15, StringFormat.GenericDefault);
            int textX = (ClientSize.Width - 15 - (int)textSize.Width) / 2;
            if (textX <= 0)
            {
                textX = 2;
            }

            int textY = (ClientSize.Height - (int)textSize.Height) / 2;

#if DOTNET2
            if (_buttonState == PushButtonState.Pressed)
#else
            if (_buttonState == ButtonState.Pushed)
#endif
            {
                textX++;
                textY++;
            }

            e.Graphics.DrawString(Text, Font, SystemBrushes.ControlText, textX, textY);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

#if DOTNET2
            _buttonState = PushButtonState.Normal;
#else
            _buttonState = ButtonState.Normal;
#endif

            Refresh();

        }

        // methods
        public void Reset()
        {
#if DOTNET2
            _buttonState = PushButtonState.Normal;
#else
            _buttonState = ButtonState.Normal;
#endif
            Refresh();
        }

        // properties
        public ContextMenu DropDownMenu
        {
            get
            {
                return _buttonMenu;
            }
            set
            {
                _buttonMenu = value;
            }
        }

#if DOTNET2
        public ContextMenuStrip DropDownMenuStrip
        {
            get
            {
                return _buttonMenuStrip;
            }
            set
            {
                _buttonMenuStrip = value;
            }
        }
#endif
	}
}
