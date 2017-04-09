// <copyright file="SplineContent.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/27/2017 19:04</date>

using SpliningPath.Scripts.Core;
using UnityEngine;

namespace SpliningPath.Editor.Utils
{
    /// <summary>
    /// SplineContent
    /// </summary>
    public class SplineContent
    {
        //================================       Public Setup       =================================
        /// <inheritdoc />
        public readonly Vector3 P0;
        public readonly Vector3 P1;
        public readonly Vector3 P2;
        public readonly Vector3 P3;
        public readonly PointInfo Info;

        public readonly int Index;
        public readonly int ParentId;
        public readonly int InternalHash;
        //================================    Systems properties    =================================

        //================================      Public methods      =================================
        /// <inheritdoc />
        public SplineContent(Vector3 p0, PointInfo info, int index, int parentId)
        {
            P0 = p0;
            Info = info;
            Index = index;
            ParentId = parentId;
            InternalHash = parentId + 1 + index;
        }

        public SplineContent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,
            PointInfo info, int index, int parentId) : this (p0, info, index, parentId)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }
        //================================ Private|Protected methods ================================
    }
}