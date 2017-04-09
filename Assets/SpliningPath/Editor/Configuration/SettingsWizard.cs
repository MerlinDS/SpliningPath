// <copyright file="SettingsWizard.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/26/2017 9:29</date>

using UnityEditor;
using UnityEngine;

namespace SpliningPath.Editor.Configuration
{
    /// <summary>
    /// SettingsWizard
    /// </summary>
    public class SettingsWizard : ScriptableWizard
    {
        //================================       Public Setup       =================================

        //================================    Systems properties    =================================

        //================================      Public methods      =================================

        //================================ Private|Protected methods ================================
        [MenuItem ("Edit/Splining Path settings ...")]
        private static void CreateWizard ()
        {
            SettingsWizard wizard = DisplayWizard<SettingsWizard>("Splining Path settings", "Save", "Reset");
            wizard.minSize = new Vector2(300, 300);
        }

        /// <inheritdoc />
        protected override bool DrawWizardGUI()
        {
            var result = base.DrawWizardGUI();
            var settings = SPSettings.Serialized;
            EditorGUILayout.PropertyField(settings.FindProperty("AssetPrefix"));
            EditorGUILayout.PropertyField(settings.FindProperty("AssetFolder"));
            EditorGUILayout.PropertyField(settings.FindProperty("DebugMode"));
            EditorGUILayout.PropertyField(settings.FindProperty("ReferenceSize"));
            EditorGUILayout.PropertyField(settings.FindProperty("ControlSize"));
            EditorGUILayout.PropertyField(settings.FindProperty("LineWeight"));
            EditorGUILayout.PropertyField(settings.FindProperty("Selection"));
            EditorGUILayout.PropertyField(settings.FindProperty("Normals"));
            EditorGUILayout.PropertyField(settings.FindProperty("LinesColors"), true);
            return result;
        }

        private void OnWizardUpdate () {
            helpString = "Please change setting for Splining Path plugin!";
        }

        private void OnWizardCreate()
        {
            var settings = SPSettings.Serialized;
            settings.ApplyModifiedProperties();
            SPSettings.SaveSettings();
            Debug.Log("SplinePath settings was changed and saved.");
        }

        private void OnWizardOtherButton ()
        {
            SPSettings.RestoreDefault();
            Debug.Log("SplinePath settings was restored to default.");
        }
    }
}