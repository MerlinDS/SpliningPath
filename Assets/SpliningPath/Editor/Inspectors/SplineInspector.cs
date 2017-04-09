// <copyright file="SplineInspector.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/26/2017 9:36</date>

using System.IO;
using SpliningPath.Editor.Configuration;
using SpliningPath.Editor.SceneViewEditing;
using SpliningPath.Editor.Utils;
using SpliningPath.Scripts.Core;
using UnityEditor;
using UnityEngine;

namespace SpliningPath.Editor.Inspectors
{
    /// <summary>
    /// SplineInspector
    /// </summary>
    [CustomEditor(typeof(Spline))]
    public class SplineInspector : UnityEditor.Editor
    {
        //================================       Public Setup       =================================
        //================================    Systems properties    =================================
        protected SplineBridge Spline;
        //================================      Public methods      =================================
        [MenuItem("Assets/Create/Splining Path/Spline")]
        private static void CreateAsset()
        {
            Spline asset = CreateInstance<Spline>();
            var name = string.Concat(SPSettings.Current.AssetPrefix, asset.GetInstanceID());
            var path = "Assets/" + SPSettings.Current.AssetFolder;
            if (!Directory.Exists(path))
            {
                Debug.LogWarningFormat("Path {0} is invalide! An asset will be created in the root folder.\n" +
                   "Fix path in plugin settings to avoid this error in future: Edit->Spline Path settings->Asset Folder", path);
                path = "Assets/";
            }

            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".asset");
            //Create new asset
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            //Save asset
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            EditMode.DoEditModeInspectorModeButton(this);
            EditMode.DoInspectorToolbar(this);

            Spline.Update();
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < Spline.Count; i++)
            {
                var pointType = Spline.GetPointInfo(i);
                string lable = PointInfo.Reference.ToString();
                EditorGUI.indentLevel = 0;
                if ((pointType & PointInfo.Control) == PointInfo.Control)
                {
                    EditorGUI.indentLevel = 1;
                    lable = PointInfo.Control.ToString();
                }
                Spline[i] = EditorGUILayout.Vector3Field(lable, Spline[i]);
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = indent;
            Spline.ApplyModifiedProperties();
            EditorGUILayout.Space();
            EditMode.DoInspectorControls(this);

        }
        //================================ Private|Protected methods ================================

        protected virtual void OnEnable()
        {
            Spline = new SplineBridge(serializedObject);
            /*EditMode.onEditModeStartDelegate += OnEditModeStart;
            EditMode.onEditModeEndDelegate += OnEditModeEnd;*/
        }

        protected virtual void OnDisable()
        {
            Spline = null;
            if (EditMode.editMode != EditMode.SceneViewEditMode.None)
                EditMode.QuitEditMode();
            // ReSharper disable once DelegateSubtraction
            EditMode.onEditModeEndDelegate -= OnEditModeEnd;
            // ReSharper disable once DelegateSubtraction
            EditMode.onEditModeStartDelegate -= OnEditModeStart;
        }
        private void OnEditModeStart(UnityEditor.Editor editor, EditMode.SceneViewEditMode mode)
        {
            if (mode == EditMode.SceneViewEditMode.None || editor != this)return;
        }

        protected void OnEditModeEnd(UnityEditor.Editor editor)
        {
            if (editor != this)return;

        }
    }
}