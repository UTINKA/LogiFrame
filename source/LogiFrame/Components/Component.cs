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
using System.ComponentModel;

namespace LogiFrame.Components
{
    /// <summary>
    ///     An abstract base class that provides functionality for the LogiFrame.Frame class.
    /// </summary>
    [TypeConverter(typeof (SimpleExpandableObjectConverter))]
    public abstract class Component : IDisposable
    {
        #region Fields

        private Bytemap _bytemap;
        private bool _hasChanged;
        private Location _location = new Location();
        private Location _renderOffset = new Location();
        private Size _size = new Size();

        private bool _topEffect;
        private bool _transparent;
        private bool _visible = true;

        #endregion

        #region Constructor/Deconstructor

        /// <summary>
        ///     Initializes a new instance of the abstract LogiFrame.Components.Component class.
        /// </summary>
        internal Component()
        {
            _location.Changed += location_Changed;
            _size.Changed += size_Changed;
        }

        /// <summary>
        ///     Releases all resources used by LogiFrame.Components.Comonent.
        /// </summary>
        ~Component()
        {
            Dispose();
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when a property has changed or the LogiFrame.Components.Component needs to be refreshed.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        ///     Occurs when the location of the LogiFrame.Components.Component has changed.
        /// </summary>
        public event EventHandler LocationChanged;

        /// <summary>
        ///     Occurs when this LogiFrame.Components.Component has been disposed.
        /// </summary>
        public event EventHandler Disposed;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the LogiFrame.Location this LogiFrame.Components.Component should
        ///     be rendered at within the parrent LogiFrame.Components.Container.
        /// </summary>
        public virtual Location Location
        {
            get { return _location; }
            set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("Resource was disposed.");

                _location.Changed -= location_Changed;

                if (SwapProperty(ref _location, value))
                    OnLocationChanged(EventArgs.Empty);

                _location.Changed += location_Changed;
            }
        }

        /// <summary>
        ///     Gets the exact LogiFrame.Location this LogiFrame.Components.Component should
        ///     be rendered at within the parrent LogiFrame.Components.Container.
        /// </summary>
        public Location RenderLocation
        {
            get { return _location + _renderOffset; }
        }

        /// <summary>
        ///     Gets or sets the offset from the actual Location this LogiFrame.Components.Component
        ///     should be rendered at within the parrent LogiFrame.Components.Container.
        /// </summary>
        protected Location RenderOffset
        {
            get { return _renderOffset; }
            set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("Resource was disposed.");

                _renderOffset.Changed -= location_Changed;

                if (SwapProperty(ref _renderOffset, value))
                    OnLocationChanged(EventArgs.Empty);

                _renderOffset.Changed += location_Changed;
            }
        }

        /// <summary>
        ///     Gets or sets the LogiFrame.Size of this LogiFrame.Components.Component.
        /// </summary>
        public virtual Size Size
        {
            get { return _size; }
            set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("Resource was disposed.");

                _size.Changed -= size_Changed;
                SwapProperty(ref _size, value);
                _size.Changed += size_Changed;
            }
        }

        /// <summary>
        ///     Gets or sets whether this LogiFrame.Components.Component should have Bytemap.TopEffect enabled when rendered.
        /// </summary>
        public bool TopEffect
        {
            get { return _topEffect; }
            set { SwapProperty(ref _topEffect, value); }
        }

        /// <summary>
        ///     Gets or sets whether this LogiFrame.Components.Component should have Bytemap.Transparent enabled when rendered.
        /// </summary>
        public bool Transparent
        {
            get { return _transparent; }
            set { SwapProperty(ref _transparent, value); }
        }

        /// <summary>
        ///     Gets or sets whether this LogiFrame.Components.Component should be visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { SwapProperty(ref _visible, value); }
        }

        /// <summary>
        ///     Gets or sets(protected) whether this LogiFrame.Component is in the process of rendering itself.
        ///     When IsRendering is True, the component won't be calling listeners of Changed when properties a refresh is
        ///     requested.
        /// </summary>
        public bool IsRendering { get; protected set; }

        /// <summary>
        ///     Gets or sets the object that contains data about the Component.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        ///     Gets whether this LogiFrame.Components.Component has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Gets the rendered LogiFrame.Bytemap of this LogiFrame.Components.Component.
        /// </summary>
        public Bytemap Bytemap
        {
            get
            {
                if (!_hasChanged)
                    return Visible ? _bytemap : Bytemap.Empty;

                Refresh(false);
                _hasChanged = false;
                return Visible ? _bytemap : Bytemap.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets the parent LogiFrame.Components.Component.
        /// </summary>
        public Component ParentComponent { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Releases all resources used by LogiFrame.Components.Comonent.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            DisposeComponent();
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Refreshes the LogiFrame.Components.Component.Bytemap and renders it if nececcary.
        /// </summary>
        /// <param name="forceRefresh">
        ///     Forces the LogiFrame.Components.Component.Bytemap being rerendered even if it hasn't changed
        ///     when True.
        /// </param>
        public virtual void Refresh(bool forceRefresh)
        {
            if (IsDisposed)
                throw new ObjectDisposedException("Resource was disposed.");

            if (!forceRefresh && !_hasChanged)
                return;

            IsRendering = true;

            //Debug.WriteLine("[LogiFrame] Rendering " + this + " : parent=" + ParentComponent);

            _bytemap = Size.Width == 0 || Size.Height == 0 ? new Bytemap(1, 1) : (Render() ?? new Bytemap(1, 1));
            _bytemap.Transparent = Transparent;
            _bytemap.TopEffect = TopEffect;

            IsRendering = false;
        }

        /// <summary>
        ///     Refreshes the LogiFrame.Components.Component.Bytemap and renders it if nececcary.
        /// </summary>
        public void Refresh()
        {
            Refresh(false);
        }

        /// <summary>
        ///     Swaps property with given value.
        /// </summary>
        /// <param name="field">The value of the field.</param>
        /// <param name="value">The value to swap it with.</param>
        /// <returns>Whether the field's value has changed.</returns>
        protected bool SwapProperty<T>(ref T field, T value)
        {
            return SwapProperty(ref field, value, false, true);
        }

        /// <summary>
        ///     Swaps property with given value.
        /// </summary>
        /// <param name="field">The value of the field.</param>
        /// <param name="value">The value to swap it with.</param>
        /// <param name="allowNull">Whether null values are allowed.</param>
        /// <returns>Whether the field's value has changed.</returns>
        protected bool SwapProperty<T>(ref T field, T value, bool allowNull)
        {
            return SwapProperty(ref field, value, allowNull, true);
        }

        /// <summary>
        ///     Swaps property with given value.
        /// </summary>
        /// <param name="field">The value of the field.</param>
        /// <param name="value">The value to swap it with.</param>
        /// <param name="allowNull">Whether null values are allowed.</param>
        /// <param name="reportChange">Whether OnChanged should be called if the proporty has been swapped with the value.</param>
        /// <returns>Whether the field's value has changed.</returns>
        protected bool SwapProperty<T>(ref T field, T value, bool allowNull, bool reportChange)
        {
            if (value == null && !allowNull)
                throw new NullReferenceException("Property cannot be set to null.");

            if (field != null && field.Equals(value))
                return false;

            field = value;
            if (reportChange)
                OnChanged(EventArgs.Empty);

            return true;
        }

        /// <summary>
        ///     Finds the parent LogiFrame.Components.Component of the given type.
        /// </summary>
        /// <typeparam name="T">Type to find.</typeparam>
        /// <returns>The partent LogiFrame.Components.Component of the given type. Returns null if not found.</returns>
        public T GetParentComponent<T>() where T : Component
        {
            Component c = this;
            while ((c = c.ParentComponent) != null)
                if (c is T)
                    return c as T;
            return null;
        }

        /// <summary>
        ///     Called when the location has changed.
        /// </summary>
        /// <param name="e">Contains information about the event.</param>
        public virtual void OnLocationChanged(EventArgs e)
        {
            if (LocationChanged != null)
                LocationChanged(this, e);
        }

        /// <summary>
        ///     Called when the component has changed.
        /// </summary>
        /// <param name="e">Contains information about the event.</param>
        public virtual void OnChanged(EventArgs e)
        {
            _hasChanged = true;

            if (!IsRendering && Changed != null)
                Changed(this, e);
        }

        /// <summary>
        ///     Stub for child components. This overridable method can be used to dispose resources.
        /// </summary>
        protected virtual void DisposeComponent()
        {
            //Stub
        }

        /// <summary>
        ///     Renders all grahpics of this LogiFrame.Components.Component.
        /// </summary>
        /// <returns>The rendered LogiFrame.Bytemap.</returns>
        protected abstract Bytemap Render();

        /// <summary>
        ///     Listens to Size.Changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void size_Changed(object sender, EventArgs e)
        {
            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        ///     Listens to Location.Changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void location_Changed(object sender, EventArgs e)
        {
            if (!IsDisposed && LocationChanged != null)
                LocationChanged(sender, e);
        }

        #endregion
    }
}