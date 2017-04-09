// <copyright file="SplinePropertyDrawer.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/26/2017 8:22</date>

using SpliningPath.Scripts.Controllers;
using SpliningPath.Scripts.Core;
using UnityEditor;
using UnityEngine;

namespace SpliningPath.Editor.Inspectors
{
    /// <summary>
    /// SplinePropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(Spline))]
    public class SplinePropertyDrawer : PropertyDrawer
    {
        //================================       Public Setup       =================================

        //================================    Systems properties    =================================
        private UnityEditor.Editor _editor;
        //================================      Public methods      =================================
        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndProperty();
            if(!Application.isEditor || Application.isPlaying)return;
            var spline = (Spline) property.objectReferenceValue;
            MonoBehaviour component = (MonoBehaviour) property.serializedObject.targetObject;
            var editorComponent = component.GetComponent<EditingController>();
            if (spline == null)
            {
                if(editorComponent != null)
                    Object.DestroyImmediate(editorComponent);
                return;
            }

            if(editorComponent == null)
            {
                editorComponent = component.gameObject.AddComponent<EditingController>();
                editorComponent.hideFlags = HideFlags.DontSave;
            }
            editorComponent.Spline = spline;
        }
        //================================ Private|Protected methods ================================
    }
}