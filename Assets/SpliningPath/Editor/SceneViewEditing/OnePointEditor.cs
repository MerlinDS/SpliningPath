// <copyright file="OnePointEditor.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/27/2017 16:19</date>

using System;
using SpliningPath.Editor.Utils;
using SpliningPath.Scripts.Core;
using UnityEditor;
using UnityEngine;

namespace SpliningPath.Editor.SceneViewEditing
{
    /// <summary>
    /// OnePointEditor
    /// </summary>
    public class OnePointEditor : SplineEditor
    {
        //================================       Public Setup       =================================

        //================================    Systems properties    =================================
        private int _selection = -1;
        //================================      Public methods      =================================
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Ope point mode: Editing a one point.", MessageType.Info);
            if(_selection < 0)return;
            Spline.Update();
            var needToUpdate = SplineGUILayout.PointField(Spline, _selection);
            var create = SplineGUILayout.PointCreationControls(Spline.GetPointInfo(_selection));

            PointInfo info;
            if (SplineGUILayout.PointTypeControls(Spline.GetPoint(_selection), out info))
            {
                Spline.SetPointInfo(_selection, info);
                needToUpdate = true;
            }

            if (create != SplineGUILayout.CreationType.None)
            {
                //TODO: Add creation methods
            }
            if(!needToUpdate)return;
            Spline.ApplyModifiedProperties();
            SceneView.RepaintAll();
        }

        //================================ Private|Protected methods ================================

        protected override void OnSceneGUI()
        {
            if (EditMode.editTool == Tool.Move)
            {
                base.OnSceneGUI();
                return;
            }
            Spline.Update();
            SplineHandles.DrawSegments(Spline.GetSegments());

            SplineContent[] points = Spline.GetPoints(PointInfo.Reference | PointInfo.Control);
            if(points.Length == 0)return;
            var selection = Array.FindIndex(points, p => p.Index == _selection);
            //Draw points
            EditorGUI.BeginChangeCheck();
            selection = SplineHandles.PointsHandle(selection, points, EditMode.editTool == Tool.Rect);
            if (EditorGUI.EndChangeCheck())_selection = points[selection].Index;
            //Move points
            if (_selection < 0 || EditMode.editTool != Tool.Rect)return;
            if (points.Length <= _selection)
            {
                Debug.LogError("Something goes wrong");
                return;
            }
            Repaint();
            EditorGUI.BeginChangeCheck();
            Vector3 position = Handles.PositionHandle(points[_selection].P0, Quaternion.identity);
            if(!EditorGUI.EndChangeCheck())return;
            MovePoint(points[_selection], position);
            Spline.ApplyModifiedProperties();
        }

        private void MovePoint(SplineContent point, Vector3 position)
        {
            Spline[point.Index] = position;
            if((point.Info & PointInfo.Control) == PointInfo.Control)
            {
                if ((point.Info & PointInfo.Cubiq) != PointInfo.Quadratic)
                {
                    //TODO: Search for pair
                }
            }
            /*if ((point.Info & GetPointInfo.Reference | GetPointInfo.Sticky) != 0)
            {

                return;
            }*/
        }

    }
}