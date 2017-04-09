// <copyright file="ColorExtention.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/26/2017 9:33</date>

using System;
using System.Text;
using UnityEngine;

namespace SpliningPath.Editor.Utils
{
    /// <summary>
    /// ColorExtention
    /// </summary>
    public static class ColorExtention
    {
        //================================       Public Setup       =================================

        //================================    Systems properties    =================================

        //================================      Public methods      =================================
        public static string ToHex(this Color color)
        {
            return ToHex((Color32)color);
        }

        public static string ToHex(this Color32 color)
        {
            return new StringBuilder()
                .AppendFormat("{0:x2}", color.r)
                .AppendFormat("{0:x2}", color.g)
                .AppendFormat("{0:x2}", color.b)
                .AppendFormat("{0:x2}", color.a)
                .ToString();
        }

        public static Color32 FromHex(this string hex)
        {
            int chars = hex.Length;
            byte[] bytes = new byte[chars / 2];
            for (int i = 0; i < chars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);
        }
        //================================ Private|Protected methods ================================

    }
}