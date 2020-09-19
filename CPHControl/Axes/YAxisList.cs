using System;
using System.Collections.Generic;
using OpenTK;

namespace CPHControl
{
    /// <summary>
    /// List containing all the YAxes.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{global::CPHControl.YAxis}" />
    public class YAxisList : List<YAxis>
    {
        /// <summary>
        /// The reference to the TextInstance Object
        /// </summary>
        internal TextInstance _textInstance;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YAxisList"/> class.
        /// </summary>
        /// <param name="TextInstance">The text instance.</param>
        public YAxisList(TextInstance TextInstance)
        {
            _textInstance = TextInstance;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Adds a <see cref="YAxis"/> with the specified title to the list.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public int Add(string title)
        {
            YAxis axis = new YAxis(title, _textInstance, Count);
            Add(axis);

            return Count - 1;
        }

        /// <summary>
        /// Returns the index of an axis referenced by name.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public int IndexOf(string title)
        {
            int index = 0;
            foreach (YAxis axis in this)
            {
                if (String.Compare(axis.Title.Text, title, true) == 0)
                    return index;
                index++;
            }

            return -1;
        }

        /// <summary>
        /// Draws all the YAxes
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="chart">The chart.</param>
        public void Draw(GraphPane pane, Chart chart)
        {
            foreach (YAxis axis in this)
            {
                axis.Draw(pane, chart);
            }
        }

        /// <summary>
        /// Sets up the transformation matrices of each YAxis.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="zoomPanState">State of the zoom pan.</param>
        /// <param name="tempXAxis">The temporary x axis.</param>
        internal void SetUpScales(GraphPane pane, ZoomPanState zoomPanState, XAxis tempXAxis)
        {
            foreach (YAxis axis in this)
            {
                Matrix4 translationToCorner = Matrix4.CreateTranslation(-1, -1, 0);                             //Default Translation, damit (0,0) im unteren linken Eck ist
                Matrix4 translationToScaleMin = Matrix4.CreateTranslation(-(float)tempXAxis.Scale.Min, -(float)axis.Scale.Min, 0);  //Skalierung der X Achse
                Matrix4 scale = Matrix4.CreateScale((float)(2 / (tempXAxis.Scale.Max - tempXAxis.Scale.Min)),
                                                    (float)(2 / (axis.Scale.Max - axis.Scale.Min)),
                                                    1);

                axis.TraFo = translationToScaleMin * scale * translationToCorner;//store the result into its axis
            }
        }
        #endregion
    }
}
