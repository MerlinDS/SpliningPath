// <copyright file="EditMode.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/27/2017 10:15</date>
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using SpliningPath.Editor.Configuration;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SpliningPath.Editor.SceneViewEditing
{
    /// <summary>
    /// EditMode
    /// </summary>
    [InitializeOnLoad]
    public class EditMode
    {
        //================================       Public Setup       =================================
        public enum SceneViewEditMode : int
        {
            None = -1,
            Spline = 0,
            EveryPoint,
            OnePoint
        }

        public static OnEditModeStopFunc onEditModeEndDelegate;
        public static OnEditModeStartFunc onEditModeStartDelegate;

        public delegate void OnEditModeStopFunc(UnityEditor.Editor editor);

        public delegate void OnEditModeStartFunc(UnityEditor.Editor editor, SceneViewEditMode mode);

        //================================          Style           =================================
        private static GUIStyle _buttonStyle;

        private static GUIStyle _commandStyle;
        private static readonly GUIContent _icon = EditorGUIUtility.IconContent("EditCollider");

        private static readonly GUIContent[] _modeIcons =
        {
//                EditorGUIUtility.IconContent("EditCollider", "|Show all points of current cruve."),
            EditorGUIUtility.IconContent("ClothInspector.SelectTool",
                "|Show only spline, without points and normals"),
            EditorGUIUtility.IconContent("ClothInspector.ViewValue", "|Show all points of current cruve."),
            EditorGUIUtility.IconContent("sv_icon_dot1_sml", "|Show a one point to edit.")
        };

        //================================    Systems properties    =================================
        private static SceneViewEditMode _editMode;
        private static Tool _editTool;

        private static int _ownerId;

        private static EditingContoller _contoller;
        //================================      Public methods      =================================
        /// <inheritdoc />
        public static int ownerId
        {
            get { return _ownerId; }
            set
            {
                if(_ownerId == value)return;
                _ownerId = value;
                if (SPSettings.Current.DebugMode)
                    Debug.Log("Set ownerId: " + value);
            }
        }

        public static Tool editTool
        {
            get { return _editTool; }
            set
            {
                if(_editMode == SceneViewEditMode.None || _editTool == value)return;
                if (SPSettings.Current.DebugMode)Debug.Log("Set editTool: " + value);
                _editTool = value;
                SceneView.RepaintAll();
            }
        }

        public static SceneViewEditMode editMode
        {
            get { return _editMode; }
            set { _editMode = value; }
        }

        public static void QuitEditMode()
        {
            ownerId = 0;
            _editMode = SceneViewEditMode.None;
            _contoller.EndEditing();
        }

        public static void DoEditModeInspectorModeButton(UnityEditor.Editor caller)
        {
            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle((GUIStyle) "Button");
                _buttonStyle.padding = new RectOffset(0, 0, 0, 0);
                _buttonStyle.margin = new RectOffset(10, 10, 0, 0);
            }
            DetectMainToolChange();
            Rect controlRect = EditorGUILayout.GetControlRect(true, 23f);
            Rect position = new Rect(controlRect.xMin + EditorGUIUtility.labelWidth, controlRect.yMin, 33f, 23f);
            int instanceId = caller.GetInstanceID();
            bool flag = editMode != SceneViewEditMode.None && ownerId == instanceId;
            EditorGUI.BeginChangeCheck();
            bool editing = GUI.Toggle(position, flag, _icon, _buttonStyle);
            if (!EditorGUI.EndChangeCheck())
                return;
            ChangeEditMode(editing ? SceneViewEditMode.Spline : SceneViewEditMode.None, caller);
        }

        public static void DoInspectorToolbar(UnityEditor.Editor caller)
        {
            if (_commandStyle == null)
            {
                _commandStyle = "Command";
                _commandStyle.margin = new RectOffset(0, 0, 0, 0);
            }
            DetectMainToolChange();
            Rect controlRect = EditorGUILayout.GetControlRect(true, 23f);
                _commandStyle.padding = new RectOffset(0, 0, 0, 0);
            Rect position = new Rect(controlRect.xMin + EditorGUIUtility.labelWidth + 40f,
                controlRect.yMin - 24f, EditorGUIUtility.currentViewWidth, 23f);
            int selection = (int)editMode;
            EditorGUI.BeginChangeCheck();
            selection = GUI.Toolbar(position, selection, _modeIcons, _commandStyle);
            if (!EditorGUI.EndChangeCheck())
                return;
            ChangeEditMode((SceneViewEditMode) selection, caller);
        }

        public static void DoInspectorControls(UnityEditor.Editor caller)
        {
            int instanceId = caller.GetInstanceID();
            if(editMode != SceneViewEditMode.None && ownerId != instanceId)return;
            _contoller.OnInspectorGUI();
        }
        //================================ Private|Protected methods ================================
        static EditMode()
        {
            _ownerId = 0;
            _editMode = SceneViewEditMode.None;
            _editTool = Tool.Move;
            //Initialize editors
            _contoller = new EditingContoller(new Dictionary<SceneViewEditMode, Type>
            {
                {SceneViewEditMode.Spline, typeof(SplineEditor)},
                {SceneViewEditMode.EveryPoint, typeof(EveryPointsEditor)},
                {SceneViewEditMode.OnePoint, typeof(OnePointEditor)}
            });
        }

        public static void ChangeEditMode(SceneViewEditMode mode, UnityEditor.Editor caller)
        {
            UnityEditor.Editor objectFromInstanceId =
                InternalEditorUtility.GetObjectFromInstanceID(ownerId) as UnityEditor.Editor;
            if (SPSettings.Current.DebugMode)
            {
                Debug.LogFormat("ChangeEditMode from {0} to {1} ", editMode, mode);
            }
            editMode = mode;
            ownerId = mode == SceneViewEditMode.None ? 0 : caller.GetInstanceID();
            _contoller.EndEditing();
            if (onEditModeEndDelegate != null)
                onEditModeEndDelegate(objectFromInstanceId);

            if (editMode != SceneViewEditMode.None)
            {
                _contoller.StartEditing(mode, caller);
                if(onEditModeStartDelegate != null)
                    onEditModeStartDelegate(caller, editMode);
            }
            SceneView.RepaintAll();
        }

        private static void DetectMainToolChange()
        {
            if (editMode == SceneViewEditMode.None) return;
            Tool current = Tools.current;
            if (current != Tool.Move && current != Tool.Rect && current != Tool.View)
            {
                if(SPSettings.Current.DebugMode)
                    Debug.LogWarning("This tool does not implement yet! See help...");
                current = Tool.None;
                Tools.current = editTool;
            }
            editTool = current;
        }
    }
}