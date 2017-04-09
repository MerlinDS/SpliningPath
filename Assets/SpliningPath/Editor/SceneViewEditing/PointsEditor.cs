// <copyright file="PointsEditor.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/27/2017 16:19</date>

using System;
using System.Collections.Generic;
using System.Linq;
using SpliningPath.Editor.Utils;
using SpliningPath.Scripts.Core;
using UnityEditor;
using UnityEngine;

namespace SpliningPath.Editor.SceneViewEditing
{
    /// <summary>
    /// PointsEditor
    /// </summary>
    public class PointsEditor : SplineEditor
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
            if (SplineGUILayout.PointTypeControls(Spline.GetPoint(_selection), out info, Spline.Count))
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
            SplineContent[] points = FilterPoints(_selection);

            SplineHandles.DrawSegments(Spline.GetSegments(), _selection);
            SplineHandles.DrawNormals(FilterNormals(_selection), -1);

            if(points.Length == 0)return;
            var selection = Array.FindIndex(points, p => p.Index == _selection);
            //Draw points
            EditorGUI.BeginChangeCheck();
            selection = SplineHandles.PointsHandle(selection, points, EditMode.editTool == Tool.Rect);
            if (EditorGUI.EndChangeCheck())_selection = points[selection].Index;
            //Move points
            if (_selection < 0 || EditMode.editTool != Tool.Rect)return;
            Repaint();
            EditorGUI.BeginChangeCheck();
            Vector3 position = Spline[_selection];
            position = Handles.PositionHandle(position, Quaternion.identity);
            if(!EditorGUI.EndChangeCheck())return;
            MovePoint(_selection, position);
            Spline.ApplyModifiedProperties();
        }

        protected virtual SplineContent[] FilterNormals(int selection)
        {
            return _selection < 0 ? new SplineContent[0] : Spline.GetNormals(selection);
        }

        protected virtual SplineContent[] FilterPoints(int selection)
        {
            if (selection < 0)return Spline.GetPoints();

            PointInfo right;
            PointInfo left = Spline.GetPointInfo(selection);
            List<int> indexes = Spline.GetPointsIndexes().ToList();

            if ((left & PointInfo.Reference) == PointInfo.Reference)
            {
                //Detect previous segment type
                right = selection - 2 < 0 ? PointInfo.Linear : Spline.GetPointInfo(selection - 2);
                if((right & PointInfo.Linear) != PointInfo.Linear && selection > 0) indexes.Add(selection - 1);
                if ((left & PointInfo.Linear) != PointInfo.Linear && selection < Spline.Count - 1)
                    indexes.Add(selection + 1);
            }
            else
            {
                indexes.Add(selection);
                int reference = Spline.GetReferenceIndex(selection);
                if (reference > 0 && reference < Spline.Count - 1)
                {
                    int opposite = reference + (reference - selection);
                    right = Spline.GetPointInfo(opposite);
                    if ((right & PointInfo.Linear) != PointInfo.Linear) indexes.Add(opposite);
                }
            }

            indexes.Sort();
            return Spline.GetPoints(indexes.ToArray());
        }

        private void MovePoint(int selection, Vector3 position)
        {
            Spline[selection] = position;
        }

    }
}