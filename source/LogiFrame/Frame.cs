﻿// LogiFrame rendering library.
// Copyright (C) 2014 Tim Potze
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>. 

using System;
using System.Threading;
using LogiFrame.Components;

namespace LogiFrame
{
    /// <summary>
    /// Represents the framework.
    /// </summary>
    public sealed class Frame : Container
    {
        #region Fields

        private LgLcd.LgLcdBitmap160X43X1 _bitmap;
        private int _buttonState;
        private LgLcd.LgLcdConnectContext _connection;
        private LgLcd.LgLcdOpenContext _openContext;

        #endregion

        #region Constructor/Deconstructor

        /// <summary>
        /// Initializes a new instance of the LogiFrame.Frame class.
        /// </summary>
        /// <param name="applicationName">A string that contains the 'friendly name' of the application.</param>
        public Frame(string applicationName)
            : this(applicationName, false, false, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogiFrame.Frame class.
        /// </summary>
        /// <param name="applicationName">A string that contains the 'friendly name' of the application.</param>
        /// <param name="isAutostartable"> Whether true application can be started by LCDMon or not.</param>
        public Frame(string applicationName, bool isAutostartable)
            : this(applicationName, isAutostartable, false, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogiFrame.Frame class.
        /// </summary>
        /// <param name="applicationName">A string that contains the 'friendly name' of the application.</param>
        /// <param name="isAutostartable"> Whether true application can be started by LCDMon or not.</param>
        /// <param name="isPersistent">Whether connection is regular.</param>
        public Frame(string applicationName, bool isAutostartable, bool isPersistent)
            : this(applicationName, isAutostartable, isPersistent, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogiFrame.Frame class.
        /// </summary>
        /// <param name="applicationName">A string that contains the 'friendly name' of the application.</param>
        /// <param name="isAutostartable"> Whether true application can be started by LCDMon or not.</param>
        /// <param name="isPersistent">Whether connection is regular.</param>
        /// <param name="allowConfiguration">Whether the application is configurable via LCDmon.</param>
        public Frame(string applicationName, bool isAutostartable, bool isPersistent, bool allowConfiguration)
            : this(applicationName, isAutostartable, isPersistent, allowConfiguration, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogiFrame.Frame class.
        /// </summary>
        /// <param name="applicationName">A string that contains the 'friendly name' of the application.</param>
        /// <param name="isAutostartable"> Whether true application can be started by LCDMon or not.</param>
        /// <param name="isPersistent">Whether connection is regular.</param>
        /// <param name="allowConfiguration">Whether the application is configurable via LCDmon.</param>
        /// <param name="simulate">Whether LogiFrame should start in simulation mode.</param>
        public Frame(string applicationName, bool isAutostartable, bool isPersistent, bool allowConfiguration,
            bool simulate)
        {
            //Check for LgLcd.dll file
            if (!System.IO.File.Exists("LgLcd.dll"))
                throw new DllNotFoundException("Could not find LgLcd.dll.");

            //Initialize connection and store properties
            _connection.AppFriendlyName = ApplicationName = applicationName;
            _connection.IsAutostartable = IsAutostartable = isAutostartable;
            _connection.IsPersistent = IsPersistent = isPersistent;
            Simulate = simulate;

            //Configuration callback
            AllowConfiguration = allowConfiguration;
            if (AllowConfiguration)
                _connection.OnConfigure.configCallback = lgLcd_onConfigureCB;

            //Set default updatepriority
            UpdatePriority = UpdatePriority.Normal;

            //Start simulation thread
            if (simulate)
                new Thread(() => Simulation.Start(this)){Name="LogiFrame simulation thread"}.Start();

            //Initialize main container
            Size = new Size((int) LgLcd.LglcdBmpWidth, (int) LgLcd.LglcdBmpHeight);
            Changed += mainContainer_ComponentChanged;

            //Store connection
            LgLcd.lgLcdInit();
            int connectionResponse = LgLcd.lgLcdConnect(ref _connection);

            //Check if a connection is set or throw an Exception
            if (connectionResponse != Win32Error.ERROR_SUCCESS && !simulate)
                throw new ConnectionException(Win32Error.ToString(connectionResponse));

            //Open connection
            _openContext.Connection = _connection.Connection;
            _openContext.OnSoftbuttonsChanged.SoftbuttonsChangedCallback = lgLcd_onSoftButtonsCB;
            _openContext.Index = 0;

            LgLcd.lgLcdOpen(ref _openContext);

            //Store bitmap format
            _bitmap.hdr = new LgLcd.LgLcdBitmapHeader {Format = LgLcd.LglcdBmpFormat160X43X1};

            //Send empty bytemap
            UpdateScreen(null);
        }

        /// <summary>
        /// Releases all resources used by LogiFrame.Frame
        /// </summary>
        ~Frame()
        {
            Dispose();
        }

        #endregion

        #region Events

        /// <summary>
        /// Represents the method that handles LogiFrame.Frame.ButtonDown 
        /// and LogiFrame.Frame.Buttonup.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A LogiFrame.ButtonEventArgs that contains the event data.</param>
        public delegate void ButtonEventHandler(object sender, ButtonEventArgs e);

        /// <summary>
        /// Represents the method that handles a LogiFrame.Frame.FramePush.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A LogiFrame.PushingEventArgs that contains the event data.</param>
        public delegate void PushingEventHandler(object sender, PushingEventArgs e);

        /// <summary>
        /// Occurs when a button is being pressed.
        /// </summary>
        public event ButtonEventHandler ButtonDown;

        /// <summary>
        /// Occurs when a button is being released.
        /// </summary>
        public event ButtonEventHandler ButtonUp;

        /// <summary>
        /// Occurs before a frame is being pushed to the display.
        /// </summary>
        public event PushingEventHandler Pushing;

        /// <summary>
        /// Occurs after the frame has been closed or disposed
        /// </summary>
        public event EventHandler FrameClosed;

        /// <summary>
        /// Occurs when the 'configure' button has been pressed in LCDmon.
        /// </summary>
        public event EventHandler Configure;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a string that contains the 'friendly name' of the application.
        /// This name is presented to the user whenever a list of applications is shown.
        /// </summary>
        public string ApplicationName { get; private set; }

        /// <summary>
        /// Gets whether application can be started by LCDMon or not.
        /// </summary>
        public bool IsAutostartable { get; private set; }

        /// <summary>
        /// Gets whether connection is temporary or whether it is
        /// a regular connection that should be added to the list.
        /// </summary>
        public bool IsPersistent { get; private set; }

        /// <summary>
        /// Gets whether the 'configure' option is being shown in LCDmon.
        /// </summary>
        public bool AllowConfiguration { get; private set; }

        /// <summary>
        /// Gets whether LogiFrame.Frame is simulating the LCD display on-screen.
        /// </summary>
        public bool Simulate { get; private set; }

        /// <summary>
        /// Gets or sets the priority for the forthcoming LCD updates.
        /// </summary>
        public UpdatePriority UpdatePriority { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources used by LogiFrame.Frame
        /// </summary>
        public new void Dispose()
        {
            if (Disposed) return;

            base.Dispose();

            OnFrameClosed(EventArgs.Empty);

            //Cannot de-initialize LgLcd from LgLcd-thread.
            //As a precausion disposing resources from another thread
            new Thread(() =>
            {
                LgLcd.lgLcdClose(_openContext.Device);
                LgLcd.lgLcdDisconnect(_connection.Connection);
                LgLcd.lgLcdDeInit();
            }){Name="LogiFrame disposal thread"}.Start();
        }

        /// <summary>
        /// Waits untill the LogiFrame.Frame was disposed.
        /// </summary>
        public void WaitForClose()
        {
            while (!Disposed)
                Thread.Sleep(2000);
        }

        /// <summary>
        /// Called when a button has been released.
        /// </summary>
        /// <param name="e">Contains information about the event.</param>
        public void OnButtonDown(ButtonEventArgs e)
        {
            if (ButtonDown != null)
                ButtonDown(this, e);
        }

        /// <summary>
        /// Called when a button has been pressed.
        /// </summary>
        /// <param name="e">Contains information about the event.</param>
        public void OnButtonUp(ButtonEventArgs e)
        {
            if (ButtonUp != null)
                ButtonUp(this, e);
        }

        /// <summary>
        /// Called when a frame is being pushed.
        /// </summary>
        /// <param name="e">Contains information about the event.</param>
        public void OnPushing(PushingEventArgs e)
        {
            if (Pushing != null)
                Pushing(this, e);
        }

        /// <summary>
        /// Called when the frame is being closed.
        /// </summary>
        /// <param name="e">Contains information about the event.</param>
        public void OnFrameClosed(EventArgs e)
        {
            if (FrameClosed != null)
                FrameClosed(this, e);
        }

        /// <summary>
        /// Called when the configuration button had been pressed.
        /// </summary>
        /// <param name="e">Contains information about the event.</param>
        public void OnConfigure(EventArgs e)
        {
            if (Configure != null)
                Configure(this, e);
        }

        /// <summary>
        /// Emulates a button being pushed.
        /// </summary>
        public void PerformButtonDown(int button)
        {
            int power = (int) Math.Pow(2, button);

            if ((_buttonState & power) == power)
                return;

            _buttonState = power | _buttonState;

            OnButtonDown(new ButtonEventArgs(button));
        }

        /// <summary>
        /// Emulates a button being released.
        /// </summary>
        public void PerformButtonUp(int button)
        {
            int power = (int) Math.Pow(2, button);

            if ((_buttonState & power) == 0)
                return;

            _buttonState = _buttonState ^ power;

            OnButtonUp(new ButtonEventArgs(button));
        }

        /// <summary>
        /// Pushes the given <paramref name="bytemap"/> to the display.
        /// </summary>
        /// <param name="bytemap">The LogiFrame.Bytemap to push.</param>
        private void UpdateScreen(Bytemap bytemap)
        {
            PushingEventArgs e = new PushingEventArgs(bytemap);
            OnPushing(e);

            if (e.PreventPush) return;

            _bitmap.pixels = bytemap == null ? new byte[LgLcd.LglcdBmpWidth*LgLcd.LglcdBmpHeight] : bytemap.Data;
            LgLcd.lgLcdUpdateBitmap(_openContext.Device, ref _bitmap, (uint) UpdatePriority);
        }

        /// <summary>
        /// Listener for LgLcd.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="dwButtons"></param>
        /// <param name="pContext"></param>
        /// <returns></returns>
        private int lgLcd_onSoftButtonsCB(int device, int dwButtons, IntPtr pContext)
        {
            for (int i = 0, b = 1; i < 4; i++, b *= 2)
                if ((_buttonState & b) == 0 && (dwButtons & b) == b)
                    OnButtonDown(new ButtonEventArgs(i));
                else if ((_buttonState & b) == b && (dwButtons & b) == 0)
                    OnButtonUp(new ButtonEventArgs(i));

            _buttonState = dwButtons;
            return 1;
        }

        /// <summary>
        /// Listener for LgLcd.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pContext"></param>
        /// <returns></returns>
        private int lgLcd_onConfigureCB(int connection, IntPtr pContext)
        {
            OnConfigure(EventArgs.Empty);
            return 1;
        }

        /// <summary>
        /// Listener for LgLcd.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainContainer_ComponentChanged(object sender, EventArgs e)
        {
            if (Disposed || sender == null)
                return;

            if (Size.Width != LgLcd.LglcdBmpWidth || Size.Height != LgLcd.LglcdBmpHeight)
                throw new InvalidOperationException("The size of the LogiFrame.Frame container may not be changed.");

            var component = sender as Component;
            if (component != null) UpdateScreen(component.Bytemap);
        }

        #endregion
    }
}