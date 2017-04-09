// <copyright file="EveryPointsEditor.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/27/2017 16:22</date>

using SpliningPath.Editor.Utils;
using SpliningPath.Scripts.Core;

namespace SpliningPath.Editor.SceneViewEditing
{
    /// <summary>
    /// EveryPointsEditor
    /// </summary>
    public class EveryPointsEditor : PointsEditor
    {
        //================================       Public Setup       =================================

        //================================    Systems properties    =================================
        //================================      Public methods      =================================
        //EditorGUILayout.HelpBox("Every points mode: Editing a all point.", MessageType.Info);
        //================================ Private|Protected methods ================================
        /// <inheritdoc />
        protected override SplineContent[] FilterPoints(int selection)
        {
            return Spline.GetPoints(PointInfo.Reference | PointInfo.Cubiq | PointInfo.Quadratic);
        }

        protected override SplineContent[] FilterNormals(int selection)
        {
            return Spline.GetNormals();
        }
    }
}