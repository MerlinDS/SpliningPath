// <copyright file="Spline.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/26/2017 8:20</date>

using System;
using UnityEngine;

namespace SpliningPath.Scripts.Core
{
    /// <summary>
    /// Spline - class of the
    /// <see cref="!:https://en.wikipedia.org/wiki/Composite_B%C3%A9zier_curve">Composite Bézier curve</see>.
    /// It manipulates with an array of points to represent the Bézier spline.
    /// <para>
    ///     By default, it will construct a simple straight line
    ///     that can be extended with the editor to a more difficult type of spline.
    /// </para>
    /// <seealso cref="!:https://en.wikipedia.org/wiki/B%C3%A9zier_curve">
    ///     Bezier Cubiq on wikipedia.org
    /// </seealso>
    /// </summary>
    public sealed class Spline : ScriptableObject
    {
        /// <summary>
        /// The length of the vector, count of coordinates for tree dimensional vector.
        /// </summary>
        public const int VLen = 3;
        /// <summary>
        /// The length of the segment, count of points (vector) in segment.
        /// </summary>
        public const int SLen = 4;
        /// <summary>
        /// The concatenation divider for segments of the spline.
        /// </summary>
        private const int Concatenator = 3;
        //================================    Systems properties    =================================
        /// <summary>
        /// Spline segments concationation factor.
        /// <para>
        ///     This parameter calculates by formula:
        ///     <code>float sf = 1.0F / count</code>
        /// </para>
        /// <para>
        ///     Total segments count equals: total count of points in the spline minus one
        ///     and divide to <see cref="Concatenator"/>
        ///     <code>int count = (int) ((_points.arraySize / VLen) - 1.0F) / Concatenator</code>
        /// </para>
        /// </summary>
        /*[SerializeField] *//*[HideInInspector]*/ private float _sf = 1.0F;
        /// <summary>
        /// The array of evaluation points. Set by <see cref="Evaluate">evaluation function</see>
        /// and represents a segment of current spline.
        /// </summary>
        [SerializeField][HideInInspector]
        private float[] _p = new float[SLen * VLen];
        /// <summary>
        /// The array of flags of the points, it correlates to array of points.
        /// <seealso cref="PointInfo"/>
        /// </summary>
        [SerializeField][HideInInspector]
        private PointInfo[] _flags =
        {
            PointInfo.Reference | PointInfo.Linear,
            PointInfo.Control | PointInfo.Linear,
            PointInfo.Control | PointInfo.Linear,
            PointInfo.Reference | PointInfo.Linear
        };

        ///  <summary>
        ///  The array of points that belongs to the spline:
        ///  <para>
        ///      [reference point A] [contol point Ar] [contol point Bl] [reference point B] [contol point Br]...
        ///      [contol point Nl][reference point N]
        ///  </para>
        ///  <para>
        ///      Where: A,B,...N - names of the points, l - left side, r - right side
        ///  </para>
        ///  <para>
        ///  Control points for <see cref="PointInfo.Linear"/> segment will be ignored.
        ///  </para>
        ///
        /// <seealso cref="PointInfo"/>
        /// <seealso cref="!:https://en.wikipedia.org/wiki/B%C3%A9zier_curve">
        ///     Bezier Cubiq on wikipedia.org
        /// </seealso>
        ///  </summary>
        /// TODO: Try to make readonly
        [SerializeField] [HideInInspector]
        private float[] _points =
        {
            1.0F, 0.0F, 0.0F,
            0.5F, 0.0F, 0.0F,
            -0.5F, 0.0F, 0.0F,
            -1.0F, 0.0F, 0.0F
        };
        //================================      Public methods      =================================
        public int SegmentsCount
        {
            // ReSharper disable once PossibleLossOfFraction
            get { return (int) (_points.Length / VLen - 1.0F) / Concatenator; }
        }
        /// <summary>
        ///   <para>Evaluate the curve at t.</para>
        /// </summary>
        /// <param name="time">The t within the spline you want to evaluate.</param>
        /// <returns>
        ///   <para>The value of the spline, at the point in t specified.</para>
        /// </returns>
        public Vector3 Evaluate(float time)
        {
            //t clamping
            if (time < 0.0F) time = 0.0F;
            else if (time > 1.0F) time = 1.0F;
            //Calculate a segment index in the spline and a segment time
            int index = time > 0.0F ?(int)Math.Ceiling(time / _sf) - 1 : 0;
            time = (time - index * _sf) / _sf;
            index = index * Concatenator;
            //Collect the evaluation points
            for (int i = 0; i < _p.Length; i++)
                _p[i] = _points[index + i];
            //Evaluation of result depending on type of the first reference point in segment
            float x, y, z;
            if ((_flags[index] & PointInfo.Linear) == PointInfo.Linear)
                LinearInterpolation(time, out x, out y, out z);
            else
                CubicInterpolation(time, out x, out y, out z);
            return new Vector3(x, y, z);
        }
        //TODO Add manipulation methods (Get point, Move point, ect.)
        //================================ Private|Protected methods ================================
        /// <summary>
        /// Linearly interpolates between two reference points of the segment.
        /// Evaluate the point by linear bezier formula.
        /// <code>
        ///     F(t) = (1 - t) * A + t * B, 0 <![CDATA[<= t <=1]]>
        /// </code>
        /// <para>
        ///     Where:
        ///     A, B - reference poinsts of the curve (segment).
        /// </para>
        /// </summary>
        /// <param name="t">The t within the curve that need to be evaluate.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void LinearInterpolation(float t, out float x, out float y, out  float z)
        {
            float t0 = 1 - t;
            //linear curve equation
            x = t0 * _p[0] + t * _p[9];
            y = t0 * _p[1] + t * _p[10];
            z = t0 * _p[2] + t * _p[11];
        }

        /// <summary>
        /// Evaluate the point by cubic bezier formula.
        /// <code>
        ///     F(t) = pow(1 - t, 3)*A + 3*pow(1 - t, 2)*t*Ar + 3*(1 - t)*pow(t, 2)*Bl + pow(t, 3)*B 0 <![CDATA[<= t <=1]]>
        /// </code>
        /// <para>
        ///     Where:
        ///     A, B - reference poinsts of the curve (segment).
        ///     Ar, Bl - controls poinsts of the curve (segment). (l - left, r - right)
        /// </para>
        /// </summary>
        /// <param name="t">The t within the curve that need to be evaluated.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void CubicInterpolation(float t, out float x, out float y, out  float z)
        {
            float t0 = 1 - t;
            float t2 = t * t;//t pow 2
            float t3 = t2 * t;//t pow 3
            float t02 = t0 * t0;//t0 pow 2
            float t03 = t02 * t0;//t0 pow 3
            //Cubic curve equation
            x = t03 * _p[0] + 3.0F * t02 * t * _p[3] + 3.0F * t0 * t2 * _p[6] + t3 * _p[9];
            y = t03 * _p[1] + 3.0F * t02 * t * _p[4] + 3.0F * t0 * t2 * _p[7] + t3 * _p[10];
            z = t03 * _p[2] + 3.0F * t02 * t * _p[5] + 3.0F * t0 * t2 * _p[8] + t3 * _p[11];

        }
    }
}