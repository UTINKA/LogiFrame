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
using LogiFrame.Drawing;

namespace LogiFrame
{
    public class FrameControl
    {
        private int _height;
        private bool _isInvalidated;
        private bool _isLayoutInit;
        private bool _isPerformingLayout;
        private int _layoutSuspendCount;
        private IMergeMethod _mergeMethod = MergeMethods.Override;
        private bool _visible = true;
        private int _width;
        private int _x;
        private int _y;
        public FrameControl Parent { get; private set; }
        public MonochromeBitmap Bitmap { get; private set; }

        public virtual IMergeMethod MergeMethod
        {
            get { return _mergeMethod ?? MergeMethods.Override; }
            set
            {
                _mergeMethod = value;
                Parent?.Invalidate();
            }
        }

        public virtual Point Location
        {
            get { return new Point(_x, _y); }
            set
            {
                _x = value.X;
                _y = value.Y;
                Parent?.Invalidate();
            }
        }

        public virtual Size Size
        {
            get { return new Size(Width, Height); }
            set { SetBounds(_x, _y, value.Width, value.Height); }
        }

        public virtual int Width
        {
            get { return _width; }
            set { SetBounds(_x, _y, value, _height); }
        }

        public virtual int Height
        {
            get { return _height; }
            set { SetBounds(_x, _y, _width, value); }
        }

        public virtual bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value) return;
                _visible = value;
                OnVisibleChanged();
                Invalidate();
            }
        }

        public event EventHandler VisibleChanged;
        public event EventHandler<FramePaintEventArgs> Paint;
        public event EventHandler<ButtonEventArgs> ButtonDown;
        public event EventHandler<ButtonEventArgs> ButtonUp;

        protected void SetBounds(int x, int y, int width, int height)
        {
            SetBounds(x, y, width, height, false);
        }

        protected virtual void SetBounds(int x, int y, int width, int height, bool preventInvalidation)
        {
            if (width < 1) width = 1;
            if (height < 1) height = 1;

            _x = x;
            _y = y;
            _width = width;
            _height = height;

            Bitmap = new MonochromeBitmap(Width, Height);

            if (!preventInvalidation)
                Invalidate();
        }

        public virtual void AssignParent(FrameControl value)
        {
            Parent = value;
            InitLayout();
        }

        public virtual void Invalidate()
        {
            _isInvalidated = true;
            if (!_isPerformingLayout && _layoutSuspendCount == 0)
                Parent?.Invalidate();
        }

        /// <summary>
        ///     Suspends the usual layout logic.
        /// </summary>
        public virtual void SuspendLayout()
        {
            _layoutSuspendCount++;
        }

        /// <summary>
        ///     Resumes the usual layout logic.
        /// </summary>
        /// <param name="performLayout">true to execute pending layout requests; otherwise, false.</param>
        public virtual void ResumeLayout(bool performLayout)
        {
            if (_layoutSuspendCount > 0)
            {
                _layoutSuspendCount--;

                if (_isInvalidated && performLayout)
                    Invalidate();
            }
        }

        /// <summary>
        ///     Resumes the usual layout logic.
        /// </summary>
        public virtual void ResumeLayout()
        {
            ResumeLayout(true);
        }

        public virtual void PerformLayout()
        {
            if (_isInvalidated && _isLayoutInit && _layoutSuspendCount == 0)
            {
                _isPerformingLayout = true;
                Bitmap.Reset();

                if (Visible)
                    OnPaint(new FramePaintEventArgs(Bitmap));

                _isInvalidated = false;
                _isPerformingLayout = false;
            }
        }

        /// <summary>
        ///     Called after the control has been added to another container.
        /// </summary>
        protected virtual void InitLayout()
        {
            if (_isLayoutInit)
            {
                Invalidate();
                return;
            }

            _isLayoutInit = true;

            if (_width == 0 || _height == 0)
                SetBounds(0, 0, 1, 1);
            else
                Bitmap = new MonochromeBitmap(Width, Height);

            Invalidate();
        }

        protected virtual void OnPaint(FramePaintEventArgs e)
        {
            Paint?.Invoke(this, e);
        }

        protected virtual void OnButtonDown(ButtonEventArgs e)
        {
            ButtonDown?.Invoke(this, e);
        }

        protected virtual void OnButtonUp(ButtonEventArgs e)
        {
            ButtonUp?.Invoke(this, e);
        }

        protected virtual void OnVisibleChanged()
        {
            VisibleChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        public bool HandleButtonDown(int button)
        {
            var args = new ButtonEventArgs(button);
            if (!args.PreventPropagation)
                OnButtonDown(args);
            return args.PreventPropagation;
        }

        public bool HandleButtonUp(int button)
        {
            var args = new ButtonEventArgs(button);
            if (!args.PreventPropagation)
                OnButtonUp(args);
            return args.PreventPropagation;
        }
    }
}