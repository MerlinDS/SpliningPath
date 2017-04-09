// <copyright file="SplineHandles.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/27/2017 16:05</date>

using System;
using SpliningPath.Editor.Configuration;
using SpliningPath.Scripts.Core;
using UnityEditor;
using UnityEngine;

namespace SpliningPath.Editor.Utils
{
    /// <summary>
    /// SplineHandles
    /// </summary>
    public static class SplineHandles
    {
        private static readonly int PointSelectionHash = "PointSelectionHash".GetHashCode();
        //================================       Public Setup       =================================

        //================================    Systems properties    =================================

        //================================      Public methods      =================================
        public static void DrawSegments(SplineContent[] contents, int selection)
        {
            foreach (SplineContent content in contents)
            {
                Color color = SPSettings.Current.GetLineColor(content.ParentId);
                if (content.IsSelected(selection)) color = SPSettings.Current.Selection;
                DrawSegment(content, color);
            }
        }

        public static void DrawNormals(SplineContent[] contents, int selection)
        {
            foreach (SplineContent content in contents)
            {
                Color color = SPSettings.Current.Normals;
                if (content.IsSelected(selection)) color = SPSettings.Current.Selection;
                DrawSegment(content, color, true);
            }
        }

        public static void DrawSegment(SplineContent segment, Color color, bool dotted = false)
        {
            float weight = SPSettings.Current.LineWeight;
            var current = Handles.color;
            Handles.color = color;
            if ((segment.Info & PointInfo.Linear) == PointInfo.Linear)
            {
                if(dotted) Handles.DrawDottedLine(segment.P0, segment.P3, 5F);
                else Handles.DrawLine(segment.P0, segment.P3);
            }
            else
            {
                Handles.DrawBezier(segment.P0, segment.P3, segment.P1,
                    segment.P2, color, null, weight);
            }
            Handles.color = current;
        }

        /// <summary>
        /// Handle points interation
        /// </summary>
        /// <param name="selected">Selected point</param>
        /// <param name="contents">Available poinst for selection</param>
        /// <param name="selectable">Are points can be selected</param>
        /// <returns></returns>
        public static int PointsHandle(int selected, SplineContent[] contents, bool selectable)
        {
            var current = Handles.color;
            int controlId = GUIUtility.GetControlID(PointSelectionHash, FocusType.Passive);
            int i, n = contents.Length;
            SplineContent content;
            switch (Event.current.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    selected = Array.FindIndex(contents, c => c.InternalHash == HandleUtility.nearestControl);
                    if (selected < 0 || Event.current.button != 0) break;
                    GUIUtility.hotControl = controlId;
                    GUI.changed = true;
                    EditorGUIUtility.SetWantsMouseJumping(1);
                    Event.current.Use();
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlId)
                    {
                        GUIUtility.hotControl = 0;
                        EditorGUIUtility.SetWantsMouseJumping(0);
                        Event.current.Use();
                    }
                    break;
                case EventType.Layout:
                    if(!selectable)break;
                    for (i = 0; i < n; i++)
                    {
                        content = contents[i];
                        HandleUtility.AddControl(content.InternalHash,
                            HandleUtility.DistanceToCircle(content.P0, 0.16F));
                    }
                    break;
                case EventType.Repaint:
                    for (i = 0; i < n; i++)
                    {
                        content = contents[i];
                        if ((content.Info & PointInfo.Control) == PointInfo.Control)
                            Handles.color = SPSettings.Current.Normals;
                        if (selected == i)
                            Handles.color = SPSettings.Current.Selection;
                        Handles.DotHandleCap(content.InternalHash, content.P0, Quaternion.identity,
                            SPSettings.Current.ControlSize, EventType.Repaint);
                        Handles.color = current;
                    }
                    break;
            }
            //Clean
            Handles.color = current;
            return selected;
        }
        //================================ Private|Protected methods ================================
    }
}