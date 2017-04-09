// <copyright file="PointInfo.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/26/2017 9:23</date>

using System;

namespace SpliningPath.Scripts.Core
{
    /// <summary>
    /// PointInfo
    /// </summary>
    [Flags]
    public enum PointInfo : uint
    {
        Reference = 0x1,
        Control = 0x2,
        /// <summary>
        ///
        /// </summary>
        Linear = 0x4,
        Cubiq = 0x8,
        Quadratic = 0x10,
        //TODO:Rename
        Sticky = 0x20
    }
}