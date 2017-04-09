// <copyright file="SplineGUILayout.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/30/2017 21:19</date>

using System;
using System.Text;
using SpliningPath.Scripts.Core;
using UnityEditor;
using UnityEngine;

namespace SpliningPath.Editor.Utils
{
    /// <summary>
    /// SplineGUILayout
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class SplineGUILayout
    {
        public enum CreationType
        {
            None = -1,
            AddLeft = 0,
            AddRight,
            Remove
        }
        //================================       Public Setup       =================================

        //================================    Systems properties    =================================
        private static readonly GUIContent[] _creationControls =
        {
            EditorGUIUtility.IconContent("Toolbar Plus",
                "|Add point to left"),
            EditorGUIUtility.IconContent("Toolbar Plus More",
                "|Add point to right"),
            EditorGUIUtility.IconContent("Toolbar Minus",
                "|Remove current point")

        };

        private static readonly GUIContent[] _typeControls =
        {
            new GUIContent("L", "|Linear"),
            new GUIContent("Q", "|Quadratick"),
            new GUIContent("C", "|Cubic")
        };

        private static readonly GUIContent _magnetButton = new GUIContent("M", "|Magnett");

        private static GUIStyle ButtonStyle
        {
            get
            {
                if (_buttonStyle == null)
                {
                    _buttonStyle = new GUIStyle("Button");
                    _buttonStyle.padding = new RectOffset(0, 0, 0, 0);
                    _buttonStyle.margin = new RectOffset(10, 10, 0, 0);
                }
                return _buttonStyle;
            }
        }

        private static GUIStyle _buttonStyle;
        //================================      Public methods      =================================

        //================================ Private|Protected methods ================================

        public static bool PointField(SplineBridge spline, int index)
        {
            Vector3 pos = spline[index];
            PointInfo info = spline.GetPointInfo(index);
            StringBuilder sb = new StringBuilder();
            if((info & PointInfo.Reference) == PointInfo.Reference)
                sb.Append(PointInfo.Reference);
            else
                sb.Append(PointInfo.Control);
            sb.Append(" (").Append(index).Append(')');
            //Draw position field
            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = EditorGUILayout.Vector3Field(new GUIContent(sb.ToString(), "Selected point of the spline"), pos);
            bool positionChanged = EditorGUI.EndChangeCheck();
            if (positionChanged)
            {
                //TODO:Move sticky points
                spline[index] = newPosition;
            }
            return positionChanged;
        }

        public static CreationType PointCreationControls(PointInfo point)
        {
            if((point & PointInfo.Control) == PointInfo.Control)return CreationType.None;
            Rect controlsRect = EditorGUILayout.GetControlRect(false, 23f);
            Rect position = new Rect(controlsRect.xMin, controlsRect.yMin,
                EditorGUIUtility.currentViewWidth / 4, 23f);
            return (CreationType)GUI.Toolbar(position, -1, _creationControls, ButtonStyle);
        }

        public static bool PointTypeControls(SplineContent point, out PointInfo info)
        {
            Rect controlsRect = EditorGUILayout.GetControlRect(false, 23f);
            Rect position = new Rect(controlsRect.xMin, controlsRect.yMin,
                EditorGUIUtility.currentViewWidth / 4, 23f);

            //Parse flags to bools
            var isRef = (point.Info & PointInfo.Reference) == PointInfo.Reference;
            var isMagnet = (point.Info & PointInfo.Sticky) == PointInfo.Sticky;
            var typeSelection = (point.Info & PointInfo.Cubiq) == PointInfo.Cubiq
                ? 2 : (point.Info & PointInfo.Quadratic) == PointInfo.Quadratic ? 1 : 0;

            bool needToUpdate = false;
            //Parse bools to flags
            info = isRef ? PointInfo.Reference : PointInfo.Control;
            //Draw controls
            if (isRef)
            {
                EditorGUI.BeginChangeCheck();
                typeSelection = GUI.Toolbar(position, typeSelection, _typeControls, ButtonStyle);
                needToUpdate = EditorGUI.EndChangeCheck();
                position.x = position.x + EditorGUIUtility.currentViewWidth / 4 + 10F;
            }
            position.width = 23f;

            EditorGUI.BeginChangeCheck();
            isMagnet = GUI.Toggle(position, isMagnet, _magnetButton, ButtonStyle);
            if (EditorGUI.EndChangeCheck()) needToUpdate = true;
            //build new flags
            if(isMagnet)info |= PointInfo.Sticky;
            info |= typeSelection == 0 ? PointInfo.Linear :
                typeSelection == 1 ? PointInfo.Quadratic : PointInfo.Cubiq;
            return needToUpdate;
        }
    }
}