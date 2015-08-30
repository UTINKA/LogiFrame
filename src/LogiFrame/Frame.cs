﻿// LogiFrame
// Copyright 2015 Tim Potze
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using LogiFrame.Drawing;
using LogiFrame.Internal;

namespace LogiFrame
{
    /// <summary>
    /// </summary>
    public class Frame : ContainerFrameControl
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly LgLcd.ConnectContext _connection;
        private readonly int _device;
        // Must keep the open context to prevent the button change delegate from being GCed.
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly LgLcd.OpenContext _openContext;
        private int _oldButtons;

        public Frame(string name, bool canAutoStart, bool isPersistent, bool allowConfiguration)
        {
            UpdatePriority = UpdatePriority.Normal;

            _connection.AppFriendlyName = name;
            _connection.IsAutostartable = canAutoStart;
            _connection.IsPersistent = isPersistent;

            if (allowConfiguration)
                _connection.OnConfigure.ConfigCallback = (connection, pContext) => 1;

            UnmanagedLibrariesLoader.Load();
            LgLcd.Init();

            var connectionResponse = LgLcd.Connect(ref _connection);

            if (connectionResponse != 0)
                throw new ConnectionException(connectionResponse);

            _openContext = new LgLcd.OpenContext
            {
                Connection = _connection.Connection,
                Index = 0,
                OnSoftButtonsChanged =
                {
                    Callback = (device, buttons, context) =>
                    {
                        for (var button = 0; button < 4; button++)
                        {
                            var buttonIdentifier = 1 << button;
                            if ((buttons & buttonIdentifier) > (_oldButtons & buttonIdentifier))
                                HandleButtonDown(button);
                            else if ((buttons & buttonIdentifier) < (_oldButtons & buttonIdentifier))
                                HandleButtonUp(button);
                        }
                        _oldButtons = buttons;
                        return 1;
                    }
                }
            };

            LgLcd.Open(ref _openContext);
            _device = _openContext.Device;

            SetBounds(0, 0, DefaultSize.Width, DefaultSize.Height);
            InitLayout();
        }

        public static Size DefaultSize { get; } = new Size((int) LgLcd.BitmapWidth, (int) LgLcd.BitmapHeight);
        public UpdatePriority UpdatePriority { get; set; }
        public event EventHandler Configure;
        public event EventHandler<RenderedEventArgs> Rendered;

        public void PushToForeground(bool toggle)
        {
            ThrowIfDisposed();

            LgLcd.SetAsLCDForegroundApp(_device, toggle ? 1 : 0);
        }

        #region Overrides of FrameControl

        public override void Invalidate()
        {
            base.Invalidate();
            PerformLayout();
        }

        #endregion

        #region Overrides of ContainerFrameControl

        protected override void OnPaint(FramePaintEventArgs e)
        {
            base.OnPaint(e);
            Push(e.Bitmap);
        }

        #endregion

        #region Overrides of FrameControl

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:LogiFrame.FrameControl"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        protected override void Dispose(bool disposing)
        {
            LgLcd.Close(_device);
            base.Dispose(disposing);
            _cancellationTokenSource.Cancel();
        }

        #endregion

        public async Task WaitForCloseAsync()
        {
            ThrowIfDisposed();

            while (!IsDisposed)
            {
                try
                {
                    await Task.Delay(60000, _cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        public void WaitForClose()
        {
            WaitForCloseAsync().Wait();
        }

        protected virtual void OnConfigure()
        {
            Configure?.Invoke(this, EventArgs.Empty);
        }

        private void Push(MonochromeBitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));

            var render = new MonochromeBitmap(bitmap, (int) LgLcd.BitmapWidth, (int) LgLcd.BitmapHeight);
            var lgBitmap = new LgLcd.Bitmap160X43X1
            {
                Header = {Format = LgLcd.BitmapFormat160X43X1},
                Pixels = render.Data
            };

            LgLcd.UpdateBitmap(_device, ref lgBitmap, (uint) UpdatePriority);
            OnRendered(new RenderedEventArgs(render));
        }

        protected virtual void OnRendered(RenderedEventArgs e)
        {
            Rendered?.Invoke(this, e);
        }

        public virtual bool IsButtonDown(int key)
        {
            ThrowIfDisposed();

            return (_oldButtons & (1 << key)) != 0;
        }
    }
}