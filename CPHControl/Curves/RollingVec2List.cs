using OpenTK;
using System;

namespace CPHControl
{
    /// <summary>
    /// Circular Buffer storing Vec2 Objects. Is used to store the data of each <see cref="CurveItem"/>
    /// </summary>
    public class RollingVec2List
    {

        #region Fields

        /// <summary>
        /// An array of Vector2 objects that acts as the underlying buffer.
        /// </summary>
        protected Vector2[] _mBuffer;

        /// <summary>
        /// The index of the previously enqueued item. -1 if buffer is empty.
        /// </summary>
        protected int _headIdx;

        /// <summary>
        /// The index of the next item to be dequeued. -1 if buffer is empty.
        /// </summary>
        protected int _tailIdx;

        /// <summary>
        /// indicates if there are new points to draw
        /// </summary>
        protected bool _drawState;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an empty buffer with the specified capacity.
        /// </summary>
        /// <param name="capacity">Number of elements in the rolling list.  This number
        /// cannot be changed once the RollingVec2List is constructed.</param>
        public RollingVec2List(int capacity)
        {
            _mBuffer = new Vector2[capacity];
            _headIdx = _tailIdx = -1;
            HasUndrawnPoint = false;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets the capacity of the rolling buffer.
        /// </summary>
        public int Capacity
        {
            get { return _mBuffer.Length; }
        }

        /// <summary>
        /// Gets the count of items within the rolling buffer. Note that this may be less than
        /// the capacity.
        /// </summary>
        public int Count
        {
            get
            {
                if (_headIdx == -1)
                    return 0;

                if (_headIdx > _tailIdx)
                    return (_headIdx - _tailIdx) + 1;

                if (_tailIdx > _headIdx)
                    return (_mBuffer.Length - _tailIdx) + _headIdx + 1;

                return 1;
            }
        }

        /// <summary>
        /// Gets a bolean that indicates if the buffer is empty.
        /// Alternatively you can test Count==0.
        /// </summary>
        public bool IsEmpty
        {
            get { return _headIdx == -1; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector2" /> at the specified index in the buffer.
        /// </summary>
        /// <remarks>
        /// Index must be within the current size of the buffer, e.g., the set
        /// method will not expand the buffer even if <see cref="Capacity" /> is available
        /// </remarks>
        public Vector2 this[int index]
        {
            get
            {
                if (index >= Count || index < 0)
                    throw new ArgumentOutOfRangeException();

                index += _tailIdx;
                if (index >= _mBuffer.Length)
                    index -= _mBuffer.Length;

                return _mBuffer[index];
            }
            set
            {
                if (index >= Count || index < 0)
                    throw new ArgumentOutOfRangeException();

                index += _tailIdx;
                if (index >= _mBuffer.Length)
                    index -= _mBuffer.Length;

                _mBuffer[index] = value;
            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has an undrawn point.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has an undrawn point; otherwise, <c>false</c>.
        /// </value>
        public bool HasUndrawnPoint
        {
            get
            {
                return _drawState;
            }
            set
            {
                _drawState = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to copy the whole array
        /// </summary>
        /// <returns>The whole buffer in an ordered fashion, from tail to head.</returns>
        public Vector2[] TailToHeadCopy()
        {
            Vector2[] _mBufferSorted = new Vector2[_mBuffer.Length];

            if (_tailIdx > _headIdx)
            {
                Array.Copy(_mBuffer, _tailIdx, _mBufferSorted, 0, _mBuffer.Length - _tailIdx);
                Array.Copy(_mBuffer, 0, _mBufferSorted, _mBuffer.Length - _tailIdx, _headIdx + 1);
            }
            else
            {
                _mBufferSorted = _mBuffer;
            }
            return _mBufferSorted;
        }

        /// <summary>
        /// Clear the buffer of all <see cref="Vector2"/> objects.
        /// Note that the <see cref="Capacity" /> remains unchanged.
        /// </summary>
        public void Clear()
        {
            _headIdx = _tailIdx = -1;
        }

        /// <summary>
        /// Calculate that the next index in the buffer that should receive a new data point.
        /// Note that this method actually advances the buffer, so a datapoint should be
        /// added at _mBuffer[_headIdx].
        /// </summary>
        /// <returns>The index position of the new head element</returns>
        private int GetNextIndex()
        {
            if (_headIdx == -1)
            {   // buffer is currently empty.
                _headIdx = _tailIdx = 0;
            }
            else
            {
                // Determine the index to write to.
                if (++_headIdx == _mBuffer.Length)
                {   // Wrap around.
                    _headIdx = 0;
                }

                if (_headIdx == _tailIdx)
                {   // Buffer overflow. Increment tailIdx.
                    if (++_tailIdx == _mBuffer.Length)
                    {   // Wrap around.
                        _tailIdx = 0;
                    }
                }
            }

            return _headIdx;
        }

        /// <summary>
        /// Add a <see cref="Vector2"/> onto the head of the queue,
        /// overwriting old values if the buffer is full.
        /// </summary>
        /// <param name="item">The <see cref="Vector2" /> to be added.</param>
        public void Add(Vector2 item)
        {
            _mBuffer[GetNextIndex()] = item;
            _drawState = true;
        }

        /// <summary>
        /// Appends a point to the end of the list.  The data are passed in as two <see cref="Double" />
        /// types.
        /// </summary>
        /// <param name="x">The <see cref="Double" /> value containing the X data to be added.</param>
        /// <param name="y">The <see cref="Double" /> value containing the Y data to be added.</param>
        /// <returns>The ordinal position (zero-based), at which the new point was added.</returns>
        public void Add(double x, double y)
        {
            int index = GetNextIndex();
            _mBuffer[index].X = (float)x;
            _mBuffer[index].Y = (float)y;
            _drawState = true;
        }

        /// <summary>
        /// Remove an old item from the tail of the queue.
        /// </summary>
        /// <returns>The removed item. Throws an <see cref="InvalidOperationException" />
        /// if the buffer was empty. 
        /// Check the buffer's length (<see cref="Count" />) or the <see cref="IsEmpty" />
        /// property to avoid exceptions.</returns>
        public Vector2 Remove()
        {
            if (_tailIdx == -1)
            {   // buffer is currently empty.
                throw new InvalidOperationException("buffer is empty.");
            }

            Vector2 o = _mBuffer[_tailIdx];

            if (_tailIdx == _headIdx)
            {   // The buffer is now empty.
                _headIdx = _tailIdx = -1;
                return o;
            }

            if (++_tailIdx == _mBuffer.Length)
            {   // Wrap around.
                _tailIdx = 0;
            }

            return o;
        }

        /// <summary>
        /// Remove the <see cref="Vector2" /> at the specified index
        /// </summary>
        /// <remarks>
        /// All items in the queue that lie after <paramref name="index"/> will
        /// be shifted back by one, and the queue will be one item shorter.
        /// </remarks>
        /// <param name="index">The ordinal position of the item to be removed.
        /// Throws an <see cref="ArgumentOutOfRangeException" /> if index is less than
        /// zero or greater than or equal to <see cref="Count" />
        /// </param>
        public void RemoveAt(int index)
        {
            int count = this.Count;

            if (index >= count || index < 0)
                throw new ArgumentOutOfRangeException();

            // shift all the items that lie after index back by 1
            for (int i = index + _tailIdx; i < _tailIdx + count - 1; i++)
            {
                i = (i >= _mBuffer.Length) ? 0 : i;
                int j = i + 1;
                j = (j >= _mBuffer.Length) ? 0 : j;
                _mBuffer[i] = _mBuffer[j];
            }

            // Remove the item from the head (it's been duplicated already)
            Pop();
        }

        /// <summary>
        /// Remove a range of <see cref="Vector2" /> objects starting at the specified index
        /// </summary>
        /// <remarks>
        /// All items in the queue that lie after <paramref name="index"/> will
        /// be shifted back, and the queue will be <paramref name="count" /> items shorter.
        /// </remarks>
        /// <param name="index">The ordinal position of the item to be removed.
        /// Throws an <see cref="ArgumentOutOfRangeException" /> if index is less than
        /// zero or greater than or equal to <see cref="Count" />
        /// </param>
        /// <param name="count">The number of items to be removed.  Throws an
        /// <see cref="ArgumentOutOfRangeException" /> if <paramref name="count" /> is less than zero
        /// or greater than the total available items in the queue</param>
        public void RemoveRange(int index, int count)
        {
            int totalCount = this.Count;

            if (index >= totalCount || index < 0 || count < 0 || count > totalCount)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < count; i++)
                this.RemoveAt(index);
        }

        /// <summary>
        /// Pop an item off the head of the queue.
        /// </summary>
        /// <returns>The popped item. Throws an exception if the buffer was empty.</returns>
        public Vector2 Pop()
        {
            if (_tailIdx == -1)
            {   // buffer is currently empty.
                throw new InvalidOperationException("buffer is empty.");
            }

            Vector2 o = _mBuffer[_headIdx];

            if (_tailIdx == _headIdx)
            {   // The buffer is now empty.
                _headIdx = _tailIdx = -1;
                return o;
            }

            if (--_headIdx == -1)
            {   // Wrap around.
                _headIdx = _mBuffer.Length - 1;
            }

            return o;
        }

        /// <summary>
        /// Peek at the <see cref="Vector2" /> item at the head of the queue.
        /// </summary>
        /// <returns>The <see cref="Vector2" /> item at the head of the queue.
        /// Throws an <see cref="InvalidOperationException" /> if the buffer was empty.
        /// </returns>
        public Vector2 Peek()
        {
            if (_headIdx == -1)
            {   // buffer is currently empty.

                //throw new InvalidOperationException("buffer is empty.");
                return _mBuffer[0];
            }

            return _mBuffer[_headIdx];
        }


        #endregion

    }
}
