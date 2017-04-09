// <copyright file="EveryPointsEditor.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/27/2017 16:22</date>

using SpliningPath.Editor.Utils;
using UnityEditor;

namespace SpliningPath.Editor.SceneViewEditing
{
    /// <summary>
    /// EveryPointsEditor
    /// </summary>
    public class EveryPointsEditor : UnityEditor.Editor
    {
        //================================       Public Setup       =================================

        //================================    Systems properties    =================================
        private SplineBridge _spline;
        //================================      Public methods      =================================
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Every points mode: Editing a all point.", MessageType.Info);
        }

        //================================ Private|Protected methods ================================
        /// <inheritdoc />
        protected void OnEnable()
        {
            _spline= new SplineBridge(serializedObject);
        }

        /// <inheritdoc />
        protected void OnDisable()
        {
            _spline = null;
        }

        private void OnSceneGUI()
        {
        }
    }
}