using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CPHControl
{
    /// <summary>
    /// This is a list of the <see cref="CurveItem"/>s contained in the <see cref="global::CPHControl"/>.
    /// It contains some additional List methods and Drawing methods for the items.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{global::CPHControl.CurveItem}" />
    public class CurveList : List<CurveItem>
    {
        /// <summary>
        /// The reference to the <see cref="TextInstance"/>
        /// </summary>
        internal TextInstance _textInstance;

        /// <summary>
        /// The reference to the <see cref="GraphPane"/>
        /// </summary>
        protected GraphPane _pane;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveList"/> class.
        /// </summary>
        /// <param name="pane">The <see cref="GraphPane"/>.</param>
        /// <param name="TextInstance">The <see cref="TextInstance"/>.</param>
        public CurveList(GraphPane pane, TextInstance TextInstance)
        {
            _textInstance = TextInstance;
            _pane = pane;
        }

        #region IEnumerable Methods
        /// <summary>
        /// Iterates backward thorugh the list
        /// </summary>
        /// <value>
        /// The <see cref="CurveItem"/> matching the index.
        /// </value>
        public IEnumerable<CurveItem> Backward
        {
            get
            {
                for (int i = this.Count - 1; i >= 0; i--)
                    yield return this[i];
            }
        }
        /// <summary>
        /// Iterates forward thorugh the list
        /// </summary>
        /// <value>
        /// The <see cref="CurveItem"/> matching the index.
        /// </value>
        public IEnumerable<CurveItem> Forward
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                    yield return this[i];
            }
        }

        #endregion

        #region List Methods
        /// <summary>
        /// Gets the <see cref="CurveItem"/> with the specified label.
        /// </summary>
        /// <value>
        /// The <see cref="CurveItem"/>.
        /// </value>
        /// <param name="label">The label.</param>
        /// <returns>the <see cref="CurveItem"/> matching the name</returns>
        public CurveItem this[string label]
        {
            get
            {
                int index = IndexOf(label);
                if (index >= 0)
                    return (this[index]);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the Index of the <see cref="CurveItem"/> 
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns>The Index of the <see cref="CurveItem"/> matching the name</returns>
        public int IndexOf(string label)
        {
            int index = 0;
            foreach (CurveItem p in this)
            {
                if (String.Compare(p._label, label, true) == 0)
                    return index;
                index++;
            }

            return -1;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws all the <see cref="CurveItem"/>
        /// </summary>
        /// <param name="pane">The pane.</param>
        public void Draw(GraphPane pane)
        {
            GL.LineWidth(1.25f);
            for (int i = this.Count - 1; i >= 0; i--)
            {
                this[i].Draw(pane);
            }
            GL.LineWidth(1.0f);
        }

        /// <summary>
        /// Gets the maximum x value contained in the <see cref="CurveItem"/>s. 
        /// This is used for Auto Scrolling.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <returns></returns>
        public double GetXMax(GraphPane pane)
        {
            double tXMax, tXTemp;
            tXMax = double.MinValue;

            foreach (CurveItem curve in this)
            {
                tXTemp = curve._rollingV2List.Peek().X;

                if (tXMax < tXTemp)
                    tXMax = tXTemp;
            }
            return tXMax;
        }
        #endregion
    }
}
