// <copyright file="EditingContoller.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/27/2017 11:42</date>

using System;
using System.Collections.Generic;
using System.Reflection;
using SpliningPath.Editor.Configuration;
using SpliningPath.Editor.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpliningPath.Editor.SceneViewEditing
{
    /// <summary>
    /// EditingContoller
    /// </summary>
    public sealed class EditingContoller
    {
        //================================       Public Setup       =================================

        //================================    Systems properties    =================================
        // ReSharper disable once InconsistentNaming
        private MethodInfo _onSceneGUIMethodInfo;

        private UnityEditor.Editor _editor;
        private UnityEditor.Editor _caller;
        private readonly Dictionary<EditMode.SceneViewEditMode, Type> _editors;
        //================================      Public methods      =================================
        /// <inheritdoc />
        public EditingContoller(Dictionary<EditMode.SceneViewEditMode, Type> editors)
        {
            _editors = editors;
        }

        public void StartEditing(EditMode.SceneViewEditMode mode, UnityEditor.Editor caller)
        {
            if (mode == EditMode.SceneViewEditMode.None || _caller == caller) return;
            Type editorType;
            if (!_editors.TryGetValue(mode, out editorType))
            {
                if (SPSettings.Current.DebugMode)
                    Debug.LogErrorFormat("Editor for mode {0} was not found!", mode);
                return;
            }
            _caller = caller;
            _editor = UnityEditor.Editor.CreateEditor(
                SplineBridge.GetUnserializedSpline(caller.serializedObject) , editorType);
            editorType = _editor.GetType();
            _onSceneGUIMethodInfo = editorType.GetMethod("OnSceneGUI",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod);
            if (_onSceneGUIMethodInfo == null)
            {
                Debug.LogErrorFormat(
                    "Editor {0} has no method OnSceneGUI for view updating. Editing can not be processed.",
                    _editor.GetType().Name);
                _editor = null;
                _caller = null;
                return;
            }

            SceneView.onSceneGUIDelegate += OnSceneGuiDelegate;
            Tools.hidden = true;
            if (SPSettings.Current.DebugMode)
            {
                Debug.LogFormat("Editor {0} enabled for editing {1}",
                    _editor.GetType().Name, _editor.target);
            }
        }

        public void EndEditing()
        {
            // ReSharper disable once DelegateSubtraction
            SceneView.onSceneGUIDelegate -= OnSceneGuiDelegate;
            if (_editor == null) return;
            if (SPSettings.Current.DebugMode)
            {
                Debug.LogFormat("Editor {0} disabled and release {1}",
                    _editor.GetType().Name, _editor.target);
            }
            Object.DestroyImmediate(_editor);
            Tools.hidden = false;
            _onSceneGUIMethodInfo = null;
            _editor = null;
            _caller = null;
        }

        //================================ Private|Protected methods ================================
        private void OnSceneGuiDelegate(SceneView sceneView)
        {
            _onSceneGUIMethodInfo.Invoke(_editor, new object[0]);
        }

        // ReSharper disable once InconsistentNaming
        public void OnInspectorGUI()
        {
            if(_editor == null)return;
            GUILayout.Box(String.Empty, GUILayout.ExpandWidth(true), GUILayout.Height(1));
            _editor.OnInspectorGUI();
        }
    }
}