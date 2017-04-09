// <copyright file="BezierInterpolation.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>04/04/2017 22:03</date>

using System;
using UnityEngine;

namespace SpliningPath.Editor.Utils
{
    /// <summary>
    /// BezierInterpolation
    /// </summary>
    public static class BezierInterpolation
    {

        public static Vector3 Linear(Vector3 a, Vector3 b, float t)
        {
            return (1 - t) * a + t * b;
        }

        public static Vector3 Quadratic(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return (float)Math.Pow(1 - t, 2) * a + 2 * t * (1 - t) * b + (float)Math.Pow(t, 2) * c;
        }

        public static Vector3 Cubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            return (float)Math.Pow(1 - t, 3) * a + 3 * t * (float)Math.Pow(1 - t, 2) * b
                + 3 * (float)Math.Pow(t, 2) * c + (float)Math.Pow(t, 3) * d;
        }
    }
}