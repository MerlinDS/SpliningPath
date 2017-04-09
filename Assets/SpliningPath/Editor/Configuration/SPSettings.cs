// <copyright file="SPSettings.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/26/2017 9:31</date>

using System.Collections.Generic;
using System.Text;
using SpliningPath.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace SpliningPath.Editor.Configuration
{
    /// <summary>
    /// SPSettings
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class SPSettings : ScriptableObject
    {
        //================================       Public Setup       =================================
        private const char Delimenter = '|';
        private static SPSettings _instance;
        private static SerializedObject _scriptable;
        //================================    Systems properties    =================================

        //================================      Public methods      =================================
        public static SPSettings Current {
            get
            {
                if (_instance == null)
                    LoadSettings();
                return _instance;
            }
            private set
            {
                if(_instance == value)return;
                if(_instance != null)DestroyImmediate(_instance);
                _instance = value;
                if (_scriptable == null) return;
                _scriptable.Dispose();
                _scriptable = null;
            }
        }

        public static SerializedObject Serialized
        {
            get
            {
                if (_scriptable == null)
                    _scriptable = new SerializedObject(Current);
                return _scriptable;
            }
        }

        public static void RestoreDefault()
        {
            Current = CreateInstance<SPSettings>();
            SaveSettings();
        }

        public static void LoadSettings()
        {
            var settings = CreateInstance<SPSettings>();
            var timestamp = EditorPrefs.GetFloat("SP_timestamp");
            if (timestamp <= 0)
            {
                Current = settings;
                SaveSettings();
                return;
            }
            settings.DebugMode = EditorPrefs.GetBool("SP_DebugMode");
            settings.AssetPrefix = EditorPrefs.GetString("SP_AssetPrefix");
            settings.AssetFolder = EditorPrefs.GetString("SP_AssetFolder");
            settings.ReferenceSize = EditorPrefs.GetFloat("SP_ReferenceSize");
            settings.ControlSize = EditorPrefs.GetFloat("SP_ControlSize");
            settings.LineWeight = EditorPrefs.GetFloat("SP_LineWeight");
            //Convert colors data to Colors
            string colors = EditorPrefs.GetString("SP_Colors");
            string[] hexes = colors.Split(Delimenter);
            settings.LinesColors = new Color[hexes.Length - 2];
            for (int i = 0; i < hexes.Length; i++)
            {
                if (i == 0)
                {
                    settings.Selection = hexes[i].FromHex();
                    continue;
                }
                if (i == 1)
                {
                    settings.Normals = hexes[i].FromHex();
                    continue;
                }
                settings.LinesColors[i - 2] = hexes[i].FromHex();
            }
            settings.Initialize();
            Current = settings;
        }

        public static void SaveSettings()
        {
            if (_instance == null)
            {
                Debug.LogError("Instance of the setting was not created!");
                return;
            }
            EditorPrefs.SetBool("SP_DebugMode", _instance.DebugMode);
            EditorPrefs.SetString("SP_AssetPrefix", _instance.AssetPrefix);
            EditorPrefs.SetString("SP_AssetFolder", _instance.AssetFolder);
            EditorPrefs.SetFloat("SP_ReferenceSize", _instance.ReferenceSize);
            EditorPrefs.SetFloat("SP_ControlSize", _instance.ControlSize);
            EditorPrefs.SetFloat("SP_LineWeight", _instance.LineWeight);
            //Convert Colors to color data
            StringBuilder sb = new StringBuilder()
                .Append(_instance.Selection.ToHex()).Append(Delimenter)
                .Append(_instance.Normals.ToHex()).Append(Delimenter);
            var n = _instance.LinesColors.Length;
            for (int i = 0; i < n; ++i)
            {
                sb.Append(_instance.LinesColors[i].ToHex());
                if (i < n - 1) sb.Append(Delimenter);
            }
            EditorPrefs.SetString("SP_Colors", sb.ToString());
            EditorPrefs.SetFloat("SP_timestamp", 1);//TODO: Add valid timestamp
        }

        //================================ Private|Protected methods ================================
    }
}