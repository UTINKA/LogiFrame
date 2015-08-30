// LogiFrame
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
using System.Collections;
using System.Collections.Generic;

namespace LogiFrame
{
    public class FrameTabPageCollection : IList<FrameTabPage>, ICloneable
    {
        private readonly List<FrameTabPage> _controls = new List<FrameTabPage>();

        #region Implementation of ICloneable

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var o = new FrameTabPageCollection();
            foreach (var e in this) o.Add(e);
            return o;
        }

        #endregion

        public event EventHandler<ValueEventArgs<FrameTabPage>> ItemAdded;
        public event EventHandler<ValueEventArgs<FrameTabPage>> ItemRemoved;

        protected virtual void OnItemAdded(ValueEventArgs<FrameTabPage> e)
        {
            ItemAdded?.Invoke(this, e);
        }

        protected virtual void OnItemRemoved(ValueEventArgs<FrameTabPage> e)
        {
            ItemRemoved?.Invoke(this, e);
        }

        #region Implementation of IEnumerable

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<FrameTabPage> GetEnumerator()
        {
            return _controls.GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<FrameControl>

        /// <summary>
        ///     Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
        ///     read-only.
        /// </exception>
        public void Add(FrameTabPage item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _controls.Add(item);
            OnItemAdded(new ValueEventArgs<FrameTabPage>(item));
        }

        /// <summary>
        ///     Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
        ///     read-only.
        /// </exception>
        public void Clear()
        {
            foreach (var i in this)
                OnItemRemoved(new ValueEventArgs<FrameTabPage>(i));
            _controls.Clear();
        }

        /// <summary>
        ///     Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <returns>
        ///     true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />;
        ///     otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public bool Contains(FrameTabPage item)
        {
            return _controls.Contains(item);
        }

        /// <summary>
        ///     Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an
        ///     <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied
        ///     from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have
        ///     zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///     The number of elements in the source
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from
        ///     <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(FrameTabPage[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException(
                    "The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.",
                    nameof(arrayIndex));

            var i = arrayIndex;
            foreach (var el in this)
                array[i++] = el;
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <returns>
        ///     true if <paramref name="item" /> was successfully removed from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if
        ///     <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
        ///     read-only.
        /// </exception>
        public bool Remove(FrameTabPage item)
        {
            if (item == null)
                return false;

            if (_controls.Remove(item))
            {
                OnItemRemoved(new ValueEventArgs<FrameTabPage>(item));
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <returns>
        ///     The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public int Count => _controls.Count;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly => false;

        #endregion

        #region Implementation of IList<FrameControl>

        /// <summary>
        ///     Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <returns>
        ///     The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public int IndexOf(FrameTabPage item)
        {
            return _controls.IndexOf(item);
        }

        /// <summary>
        ///     Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is not a valid index in the
        ///     <see cref="T:System.Collections.Generic.IList`1" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        public void Insert(int index, FrameTabPage item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _controls.Insert(index, item);
            OnItemAdded(new ValueEventArgs<FrameTabPage>(item));
        }

        /// <summary>
        ///     Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is not a valid index in the
        ///     <see cref="T:System.Collections.Generic.IList`1" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        public void RemoveAt(int index)
        {
            var element = this[index];
            if (element == null) return;
            OnItemRemoved(new ValueEventArgs<FrameTabPage>(element));
            _controls.RemoveAt(index);
        }

        /// <summary>
        ///     Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        ///     The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is not a valid index in the
        ///     <see cref="T:System.Collections.Generic.IList`1" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The property is set and the
        ///     <see cref="T:System.Collections.Generic.IList`1" /> is read-only.
        /// </exception>
        public FrameTabPage this[int index]
        {
            get { return _controls[index]; }
            set
            {
                if (value == null) throw new Exception();

                if (_controls[index] == value)
                    return;

                OnItemRemoved(new ValueEventArgs<FrameTabPage>(_controls[index]));
                OnItemAdded(new ValueEventArgs<FrameTabPage>(value));
                _controls[index] = value;
            }
        }

        #endregion
    }
}