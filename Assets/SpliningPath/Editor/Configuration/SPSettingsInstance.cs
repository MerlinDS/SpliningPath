using System.Collections.Generic;
using UnityEngine;

namespace SpliningPath.Editor.Configuration
{
    // ReSharper disable once InconsistentNaming
    public partial class SPSettings
    {
        [Tooltip("Prefix for name of new asset.")]
        public string AssetPrefix = "Spline";
        [Tooltip("Where to store created assets (Assets/...)")]
        public string AssetFolder;

        [Header("View setting:")]

        [Tooltip("Size of spline reference points in scene view")]
        [Range(0.01F, 1F)]
        public float ReferenceSize = 0.01F;
        [Tooltip("Size of spline control points in scene view")]
        [Range(0.01F, 1F)]
        public float ControlSize = 0.01F;
        [Tooltip("Weight of spline lines in sceen view")]
        [Range(0.01F, 5F)]
        public float LineWeight = 1.5F;

        [Tooltip("Turn off|on internal logs")]
        public bool DebugMode;

        [Header("Color palette setting")]
        [Tooltip("Selection color")]
        public Color Selection = Color.green;
        [Tooltip("Colors of normals lines in scene view")]
        public Color Normals = Color.magenta;
        [Tooltip("Colors of lines in scene view")]
        public Color[] LinesColors =
        {
            Color.cyan,
            Color.yellow,
            Color.white,
            Color.blue,
            Color.red,
            Color.black
        };

        private int _lastReserved;
        private Dictionary<int, int> _rerserved;

        private void Initialize()
        {
            _lastReserved = -1;
            _rerserved = new Dictionary<int, int>();
        }

        public Color GetLineColor(int instanceId)
        {
            int colorIndex;
            if (!_rerserved.TryGetValue(instanceId, out colorIndex))
            {
                if (++_lastReserved >= LinesColors.Length)
                    _lastReserved = 0;
                _rerserved.Add(instanceId, _lastReserved);
                colorIndex = _lastReserved;
            }
            return LinesColors[colorIndex];
        }

    }
}