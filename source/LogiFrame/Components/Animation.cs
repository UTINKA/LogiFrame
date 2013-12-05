﻿// Animation.cs
// 
// LogiFrame rendering library.
// Copyright (C) 2013 Tim Potze
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace LogiFrame.Components
{
    /// <summary>
    /// Represents a drawable animation.
    /// </summary>
    public class Animation : Picture
    {
        #region Fields

        private bool _autoInterval = true;
        private Bytemap[] _bytemaps;
        private int _frame;
        private int _interval;
        private bool _run;
        private Thread _thread;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the time in miliseconds each frame lasts.
        /// </summary>
        public int Interval
        {
            get { return _interval; }
            set
            {
                if (!AutoInterval)
                    _interval = value;
                CheckThreadRunning();
            }
        }

        /// <summary>
        /// Gets or sets whether this LogiFrame.Components.Animation should
        /// automatically calculate its Interval.
        /// </summary>
        public bool AutoInterval
        {
            get { return _autoInterval; }
            set
            {
                if (_autoInterval == value)
                    return;

                _autoInterval = value;

                if (value)
                    _interval = GetFrameDuration();
            }
        }

        /// <summary>
        /// Gets or sets the animated System.Drawing.Image to be rendered.
        /// </summary>
        public override Image Image
        {
            get { return base.Image; }
            set
            {
                if (base.Image == value)
                    return;

                IsRendering = true;
                base.Image = value;
                IsRendering = false;

                RenderAnimation();
                HasChanged = true;
                CheckThreadRunning();
            }
        }

        /// <summary>
        /// Gets or sets the LogiFrame.ConversionMethod to be used to render the animation.
        /// </summary>
        public override ConversionMethod ConversionMethod
        {
            get { return base.ConversionMethod; }
            set
            {
                if (base.ConversionMethod == value)
                    return;

                IsRendering = true;
                base.ConversionMethod = value;
                IsRendering = false;

                RenderAnimation();
                HasChanged = true;
                CheckThreadRunning();
            }
        }

        /// <summary>
        ///    Gets or sets the 0-based frame index to be rendered.
        /// </summary>
        public int Frame
        {
            get { return _frame; }
            set
            {
                if (_frame == value)
                    return;

                if (value >= FrameCount)
                    value = 0;

                if (value < 0)
                    value = FrameCount - 1;

                _frame = value;
                HasChanged = true;
            }
        }

        /// <summary>
        /// Gets the number of frames available in this animation.
        /// </summary>
        public int FrameCount
        {
            get
            {
                if (_bytemaps == null)
                    return 0;

                return _bytemaps.Length;
            }
        }

        /// <summary>
        /// Gets or sets whether the animation should automatically cycle trough its frames.
        /// </summary>
        public bool Run
        {
            get { return _run; }
            set
            {
                if (_run == value)
                    return;

                _run = value;

                CheckThreadRunning();
            }
        }

        #endregion

        #region Methods

        protected override Bytemap Render()
        {
            //Return current frame
            if (_frame < 0 || _bytemaps == null || _frame >= _bytemaps.Length)
                return null;

            return _bytemaps[_frame];
        }

        protected override void DisposeComponent()
        {
            _run = false;
            Image.Dispose();
        }

        /// <summary>
        /// Checks whether the thread is still running and restarts it if necessary.
        /// </summary>
        private void CheckThreadRunning()
        {
            //Check if the thread is running
            if (!Disposed && Run && _thread == null)
            {
                _thread = new Thread(AnimationThread);
                _thread.Start();
            }
        }

        /// <summary>
        /// Renders and stores every individual frame of this LogiFrame.Components.Animation.
        /// </summary>
        private void RenderAnimation()
        {
            if (Disposed)
                throw new ObjectDisposedException("Resource was disposed.");

            //If no image is set, don't render anything.
            if (Image == null)
            {
                _bytemaps = null;
                return;
            }

            //Calculate frame dimensions
            FrameDimension dimension = new FrameDimension(Image.FrameDimensionsList[0]);

            // Get numer of frames
            int frames = Image.GetFrameCount(dimension);

            //Create bytemap for each frame
            _bytemaps = new Bytemap[frames];

            //Render bytemaps
            for (int i = 0; i < frames; i++)
            {
                Image.SelectActiveFrame(dimension, i);
                _bytemaps[i] = Bytemap.FromBitmap((Bitmap) Image, ConversionMethod);
            }

            //calculate interval
            if (AutoInterval)
                _interval = GetFrameDuration();

            //check current frame
            if (_frame < 0 || _frame >= frames)
                _frame = 0;
        }

        /// <summary>
        /// Gets the frame duration of the Image from libgdiplus.
        /// </summary>
        /// <returns>The frame duration from libgdiplus</returns>
        private int GetFrameDuration()
        {
            try
            {
                PropertyItem item = Image.GetPropertyItem(0x5100); // 0x5100 is the FrameDelay in libgdiplus
                // Time is in 1/100th of a second
                return (item.Value[0] + item.Value[1]*256)*10;
            }
            catch (Exception)
            {
                return 200;
            }
        }

        /// <summary>
        /// Cycles trough all the frames on the set Interval.
        /// </summary>
        private void AnimationThread()
        {
            while (!Disposed && Run && Interval > 0)
            {
                Frame++;
                Thread.Sleep(Interval);
            }
            _thread = null;
        }

        #endregion
    }
}