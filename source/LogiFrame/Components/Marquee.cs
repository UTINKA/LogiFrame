﻿// LogiFrame
// Copyright (C) 2014 Tim Potze
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>

using System;
using System.Drawing;
using System.Linq;

namespace LogiFrame.Components
{
    /// <summary>
    ///     Represents a scrolling text.
    /// </summary>
    public class Marquee : Container
    {
        #region Fields

        private readonly Label _label = new Label {AutoSize = true};
        private readonly ComponentCollection<Marquee> _syncedMarquees = new ComponentCollection<Marquee>();
        private readonly Timer _timer = new Timer();
        private int _endSteps;
        private int _step;
        private int _stepSize = 1;
        private bool _vertical;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the LogiFrame.Component.Marquee class.
        /// </summary>
        public Marquee()
        {
            _syncedMarquees.ComponentAdded += (sender, args) => { args.Component.Disposed += Component_Disposed; };
            _timer.Tick += (sender, args) => { Step += StepSize; };
            Components.Add(_label);
            Components.Add(_timer);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets thetext this LogiFrame.Component.Marquee should draw.
        /// </summary>
        public string Text
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        /// <summary>
        ///     Gets or sets the System.Drawing.Font this LogiFrame.Component.Marquee should draw with.
        /// </summary>
        public Font Font
        {
            get { return _label.Font; }
            set { _label.Font = value; }
        }

        /// <summary>
        ///     Gets or sets whether the label should cache all rendered texts.
        /// </summary>
        public bool UseCache
        {
            get { return _label.UseCache; }
            set { _label.UseCache = value; }
        }

        /// <summary>
        ///     Gets or sets the time in miliseconds each frame lasts.
        /// </summary>
        public int Interval
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        /// <summary>
        ///     Gets or sets whether the text should move.
        /// </summary>
        public bool Run
        {
            get { return _timer.Enabled; }
            set { _timer.Enabled = value; }
        }

        /// <summary>
        ///     Gets or sets whether the text should move vertically. If false, the text moves horizontally
        /// </summary>
        public bool Vertical
        {
            get { return _vertical; }
            set { SwapProperty(ref _vertical, value); }
        }

        /// <summary>
        ///     Gets or sets the number of pixels shifted each interval.
        /// </summary>
        public int StepSize
        {
            get { return _stepSize; }
            set
            {
                if (value <= 0)
                    throw new IndexOutOfRangeException("Marquee.StepSize must at least contain a value of 1 or higher.");

                SwapProperty(ref _stepSize, value);
            }
        }

        /// <summary>
        ///     Gets a list to fill with LogiFrame.Components.Marquee objects whose steps to sync with this
        ///     LogiFrame.Components.Marquee.
        /// </summary>
        public ComponentCollection<Marquee> SyncedMarquees
        {
            get { return _syncedMarquees; }
        }

        /// <summary>
        ///     Gets or sets the LogiFrame.Components.MarqueeStyle to use.
        /// </summary>
        public MarqueeStyle MarqueeStyle { get; set; }

        /// <summary>
        ///     Gets or sets the amount of steps for the marquee to stop at the preset moments.
        /// </summary>
        public int EndSteps
        {
            get { return _endSteps; }
            set { SwapProperty(ref _endSteps, value); }
        }

        /// <summary>
        ///     Gets the amount of steps it takes this LogiFrame.Components.Marquee to rotate.
        /// </summary>
        public int Steps
        {
            get
            {
                switch (MarqueeStyle)
                {
                    case MarqueeStyle.Loop:
                        return EndSteps + Size.Width + _label.Size.Width*2;
                    case MarqueeStyle.Visibility:
                        return EndSteps*2 +
                               (_label.Size.Width - Size.Width > 0 ? _label.Size.Width - Size.Width : 0);
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the current step of this LogiFrame.Components.Marquee.
        /// </summary>
        public int Step
        {
            get { return _step; }
            set
            {
                if (!SwapProperty(ref _step, value, false, false)) return;
                int ms = _syncedMarquees.Select(m => m.Steps).Concat(new[] {Steps}).Max();

                _step %= ms;
                while (_step < 0) _step += ms;
                int step = _step >= Steps ? Steps - 1 : _step;

                foreach (Marquee marquee in _syncedMarquees)
                    marquee.Step = Math.Min(_step, marquee.Steps - 1);

                switch (MarqueeStyle)
                {
                    case MarqueeStyle.Loop:
                        if (Vertical)
                        {
                            int y = Size.Width - step;
                            if (step > Size.Width) y += Math.Min(step - Size.Width, EndSteps);
                            _label.Location.Set(0, y);
                        }
                        else
                        {
                            int x = Size.Width - step;
                            if (step > Size.Width) x += Math.Min(step - Size.Width, EndSteps);
                            _label.Location.Set(x, 0);
                        }
                        break;
                    case MarqueeStyle.Visibility:
                        if (Steps == EndSteps*2)
                        {
                            _label.Location.Set(0, 0);
                            break;
                        }
                        if (Vertical)
                        {
                            int y = -step + Math.Min(step, EndSteps);
                            int h = _label.Size.Height - Size.Height > 0 ? _label.Size.Height - Size.Height : 0;
                            if (step > h + EndSteps) y += Math.Min(step - (h + EndSteps), EndSteps);
                            _label.Location.Set(0, y);
                        }
                        else
                        {
                            int x = -step + Math.Min(step, EndSteps);
                            int w = _label.Size.Width - Size.Width > 0 ? _label.Size.Width - Size.Width : 0;
                            if (step > w + EndSteps) x += Math.Min(step - (w + EndSteps), EndSteps);
                            _label.Location.Set(x, 0);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Clears all items from the Label's cache.
        /// </summary>
        public void ClearCache()
        {
            _label.ClearCache();
        }


        private void Component_Disposed(object sender, EventArgs e)
        {
            var m = sender as Marquee;
            m.Disposed -= Component_Disposed;
            _syncedMarquees.Remove(m);
        }

        #endregion
    }
}