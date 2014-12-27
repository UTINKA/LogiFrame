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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace LogiFrame.Components
{
    /// <summary>
    ///     Represents a drawable text label.
    /// </summary>
    public class Label : Component
    {
        #region Fields

        private readonly List<CacheItem> _cache = new List<CacheItem>();
        private bool _autoSize;
        private Font _font = new Font("Arial", 7);
        private Alignment _horizontalAlignment = Alignment.Left;
        private string _text;
        private Alignment _verticalAlignment = Alignment.Top;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the text this LogiFrame.Components.Label should draw.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                if (!SwapProperty(ref _text, value, false, false)) return;

                if (AutoSize) MeasureText(true);
                AlignText();
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Gets or sets the System.Drawing.Font this LogiFrame.Components.Label should draw with.
        /// </summary>
        public Font Font
        {
            get { return _font; }
            set
            {
                if (!SwapProperty(ref _font, value, false)) return;

                if (AutoSize) MeasureText(true);
                AlignText();
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Gets or sets whether this LogiFrame.Components.Label should automatically
        ///     resize to fit the Text.
        /// </summary>
        public bool AutoSize
        {
            get { return _autoSize; }
            set
            {
                if (!SwapProperty(ref _autoSize, value, false)) return;

                if (AutoSize) MeasureText(true);
                AlignText();
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Gets or sets the LogiFrame.Size of this LogiFrame.Components.Label.
        /// </summary>
        public override Size Size
        {
            get { return base.Size; }
            set
            {
                if (!AutoSize)
                    base.Size = value;
                AlignText();
            }
        }

        /// <summary>
        ///     Gets or sets the vertical LogiFrame.Component.Alignment of this LogiFrame.Components.Label.
        /// </summary>
        public Alignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set
            {
                if (!SwapProperty(ref _verticalAlignment, value, false)) return;
                AlignText();
            }
        }

        /// <summary>
        ///     Gets or sets the horizontal LogiFrame.Component.Alignment of this LogiFrame.Components.Label.
        /// </summary>
        public Alignment HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set
            {
                if (!SwapProperty(ref _horizontalAlignment, value, false)) return;
                AlignText();
            }
        }

        /// <summary>
        ///     Gets or sets whether the label should cache all rendered texts.
        /// </summary>
        public bool UseCache { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Clears all cache items from the Label's cache.
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }

        protected override Bytemap Render()
        {
            if (UseCache)
            {
                CacheItem cacheItem = _cache.FirstOrDefault(c => c.Text == Text && c.Font.Equals(Font));
                if (cacheItem != null)
                {
                    if (AutoSize)
                        Size = cacheItem.Bytemap.Size;
                    return cacheItem.Bytemap;
                }
            }

            var bmp = new Bitmap(Size.Width, Size.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            g.DrawString(Text, Font, Brushes.Black, new Point(0, 0));

            Bytemap bymp = Bytemap.FromBitmap(bmp);
            if (UseCache)
                _cache.Add(new CacheItem {Bytemap = bymp, Font = Font.Clone() as Font, Text = Text});

            return bymp;
        }

        private void MeasureText(bool silent)
        {
            if (silent) IsRendering = true;
            SizeF strSize = Graphics.FromImage(new Bitmap(1, 1)).MeasureString(Text, Font);
            base.Size.Set((int) Math.Ceiling(strSize.Width), (int) Math.Ceiling(strSize.Height));
            if (silent) IsRendering = false;
        }

        private void AlignText()
        {
            int x = 0;
            int y = 0;

            switch (HorizontalAlignment)
            {
                case Alignment.Left:
                    x = 0;
                    break;
                case Alignment.Middle:
                    x = -Size.Width/2;
                    break;
                case Alignment.Right:
                    x = -Size.Width;
                    break;
            }

            switch (VerticalAlignment)
            {
                case Alignment.Top:
                    y = 0;
                    break;
                case Alignment.Center:
                    y = -Size.Height/2;
                    break;
                case Alignment.Bottom:
                    y = -Size.Height;
                    break;
            }

            RenderOffset.Set(x, y);
        }

        #endregion

        #region Subclasses

        private class CacheItem
        {
            public string Text { get; set; }
            public Font Font { get; set; }
            public Bytemap Bytemap { get; set; }
        }

        #endregion
    }
}