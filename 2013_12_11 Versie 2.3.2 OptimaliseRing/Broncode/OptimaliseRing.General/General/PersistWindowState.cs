#region Copyright -------------------------------------------------------
// Copyright © 2005 HKV lijn in water, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : OptimaliseRing.General
//
// Author(s)  : Abe Hoekstra, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/General/PersistWindowState.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion

using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing;

namespace OptimaliseRing.General
{
    /// <summary>
    /// Class allows to keep last window state in Registry
    /// and restore it when form is loaded.
    ///
    ///  Using:
    ///  1. Add class member to the owner form:
    ///
    ///  private PersistWindowState persistState;
    ///
    ///  2. Create it in the form constructor:
    ///
    ///  persistState = new PersistWindowState("Software\\MyCompany\\MyProgram", this);
    ///
    /// </summary>
    public class PersistWindowState
    {
        #region Members

        private Form    _ownerForm;       // reference to owner form
        private string _registryPath;      // path in Registry where state information is kept

        // Form state parameters:
        private int normalLeft;
        private int normalTop;
        private int normalWidth;
        private int normalHeight;

        // FormWindowState is enumeration from System.Windows.Forms Namespace
        // Contains 3 members: Maximized, Minimized and Normal.
        private FormWindowState windowState = FormWindowState.Normal;

        // if allowSaveMinimized is true, form closed in minimal state
        // is loaded next time in minimal state.
        private bool allowSaveMinimized = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="sRegPath"></param>
        /// <param name="owner"></param>
        public PersistWindowState(string path, Form owner)
        {
            if (path == null ||
                path.Length == 0)
            {
                _registryPath = "Software\\Unknown";
            }
            else
            {
                _registryPath = path;
            }

            if (!_registryPath.EndsWith("\\"))
                _registryPath += "\\";

            _registryPath += "MainForm";

            _ownerForm = owner;

            // subscribe to parent form's events

            _ownerForm.Closing += OnClosing;
            _ownerForm.Resize += OnResize;
            _ownerForm.Move += OnMove;
            _ownerForm.Load += OnLoad;

            // get initial width and height in case form is never resized
            normalWidth = _ownerForm.Width;
            normalHeight = _ownerForm.Height;
        }

        #endregion

        #region Properties

        /// <summary>
        /// AllowSaveMinimized property (default value false)
        /// </summary>
        public bool AllowSaveMinimized
        {
            get
            {
                return allowSaveMinimized;
            }
            set
            {
                allowSaveMinimized = value;
            }
        }

        #endregion

        #region Event Handlers


        /// <summary>
        /// Parent form is resized.
        /// Keep current size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnResize(object sender, System.EventArgs e)
        {
            // save width and height
            if (_ownerForm.WindowState == FormWindowState.Normal)
            {
                normalWidth = _ownerForm.Width;
                normalHeight = _ownerForm.Height;
            }
        }

        /// <summary>
        /// Parent form is moved.
        /// Keep current window position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMove(object sender, System.EventArgs e)
        {
            // save position
            if (_ownerForm.WindowState == FormWindowState.Normal)
            {
                normalLeft = _ownerForm.Left;
                normalTop = _ownerForm.Top;
            }

            // save state
            windowState = _ownerForm.WindowState;
        }

        /// <summary>
        /// Parent form is closing.
        /// Keep last state in Registry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // save position, size and state
            RegistryKey key = Registry.CurrentUser.CreateSubKey(_registryPath);
            key.SetValue("Left", normalLeft);
            key.SetValue("Top", normalTop);
            key.SetValue("Width", normalWidth);
            key.SetValue("Height", normalHeight);

            // check if we are allowed to save the state as minimized (not normally)
            if (!allowSaveMinimized)
            {
                if (windowState == FormWindowState.Minimized)
                    windowState = FormWindowState.Normal;
            }

            key.SetValue("WindowState", (int)windowState);
        }

        /// <summary>
        /// Parent form is loaded.
        /// Read last state from Registry and set it to form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoad(object sender, System.EventArgs e)
        {
            // attempt to read state from registry
            RegistryKey key = Registry.CurrentUser.OpenSubKey(_registryPath);
            if (key != null)
            {
                int left = (int)key.GetValue("Left", _ownerForm.Left);
                int top = (int)key.GetValue("Top", _ownerForm.Top);
                int width = (int)key.GetValue("Width", _ownerForm.Width);
                int height = (int)key.GetValue("Height", _ownerForm.Height);
                FormWindowState windowState = (FormWindowState)key.GetValue("WindowState", (int)_ownerForm.WindowState);

                _ownerForm.Location = new Point(left, top);
                _ownerForm.Size = new Size(width, height);
                _ownerForm.WindowState = windowState;
            }
        }

        #endregion

    }
}
