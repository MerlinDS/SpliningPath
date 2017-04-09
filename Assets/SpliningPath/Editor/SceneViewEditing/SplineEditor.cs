// <copyright file="SplineEditor.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/27/2017 12:10</date>

using SpliningPath.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace SpliningPath.Editor.SceneViewEditing
{
    /// <summary>
    /// SplineEditor
    /// </summary>
    public class SplineEditor : UnityEditor.Editor
    {
        //================================       Public Setup       =================================

        //================================    Systems properties    =================================
        protected SplineBridge Spline;
        //================================      Public methods      =================================
        /// <inheritdoc />
        protected virtual void OnEnable()
        {
            Spline= new SplineBridge(serializedObject);
        }

        /// <inheritdoc />
        protected virtual void OnDisable()
        {
            Spline = null;
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("View mode: In this mode, you can only move a whole spline.\n" +
                                    "For editing the spline choose another mode.", MessageType.Info);
        }

        /// <inheritdoc />
        protected virtual void OnSceneGUI()
        {
            Spline.Update();
            SplineHandles.DrawSegments(Spline.GetSegments(), -1);

            EditorGUI.BeginChangeCheck();
            var position = Handles.PositionHandle(Spline.Midpoint, Quaternion.identity);
            if(!EditorGUI.EndChangeCheck())return;
            Spline.Midpoint = position;
            Spline.ApplyModifiedProperties();
            Repaint();
        }

        //================================ Private|Protected methods ================================

    }
}