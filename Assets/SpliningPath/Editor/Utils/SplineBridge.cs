﻿// <copyright file="SplineBridge.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/26/2017 0:55</date>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SpliningPath.Scripts.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpliningPath.Editor.Utils
{
    /// <summary>
    /// SplineBridge
    /// </summary>
    public class SplineBridge
    {
        #region Conts

        private const string CoordinatesField = "_points";
        private const string FlagsField = "_flags";
        private const string SfField = "_sf";

        #endregion

        #region ReadonlyParameters

        private readonly Spline _spline;
        private readonly SerializedProperty _sf;
        private readonly SerializedProperty _flags;
        private readonly SerializedProperty _coodrinates;
        private readonly SerializedObject _serializedObject;

        #endregion

        #region Paramenters

        public int InstanceId
        {
            get { return _spline.GetInstanceID(); }
        }

        public Vector3 this[int i]
        {
            get
            {
                return new Vector3(
                    _coodrinates.GetArrayElementAtIndex(i * Spline.VLen).floatValue,
                    _coodrinates.GetArrayElementAtIndex(i * Spline.VLen + 1).floatValue,
                    _coodrinates.GetArrayElementAtIndex(i * Spline.VLen + 2).floatValue);
            }
            set
            {
                _coodrinates.GetArrayElementAtIndex(i * Spline.VLen).floatValue = value.x;
                _coodrinates.GetArrayElementAtIndex(i * Spline.VLen + 1).floatValue = value.y;
                _coodrinates.GetArrayElementAtIndex(i * Spline.VLen + 2).floatValue = value.z;
            }
        }

        public int Count
        {
            get { return _coodrinates.arraySize / Spline.VLen; }
        }

        public int SegmentsCount
        {
            get { return _spline.SegmentsCount; }
        }

        public Vector3 Midpoint
        {
            get { return (this[Count - 1] + this[0]) * 0.5F; }
            set
            {
                var delta = value - (this[Count - 1] + this[0]) * 0.5F;
                var n = Count;
                for (var i = 0; i < n; ++i)
                    this[i] += delta;
            }
        }

        #endregion

        public PointInfo GetPointInfo(int index)
        {
            return (PointInfo) _flags.GetArrayElementAtIndex(index).intValue;
        }

        public void SetPointInfo(int index, PointInfo info)
        {
            _flags.GetArrayElementAtIndex(index).intValue = (int) info;
            if ((info & PointInfo.Control) == PointInfo.Control || index >= Count)
                return;
            //Normalize control points
            int p0 = index + 1;//Control
            int p1 = index + 2;//Control
            int p3 = index + 3;//Next ref point of the segment
            Vector3 pos0 = this[index];
            Vector3 pos1 = this[p3];

            RemoveFlags(p0, PointInfo.Linear, PointInfo.Quadratic, PointInfo.Cubiq);
            RemoveFlags(p1, PointInfo.Linear, PointInfo.Quadratic, PointInfo.Cubiq);

            bool isLiner = (info & PointInfo.Linear) == PointInfo.Linear;
            bool isCubic = (info & PointInfo.Cubiq) == PointInfo.Cubiq;
            bool isQuadratic = (info & PointInfo.Quadratic) == PointInfo.Quadratic;
            Vector3 midpoint;
            if (isLiner)
            {
                AddFlags(p0, PointInfo.Linear);
                AddFlags(p1, PointInfo.Linear);
                midpoint = (pos1 - pos0) * 0.5F;
                this[p0] = pos0 + midpoint;
                this[p1] = pos1 - midpoint;
            }
            else if (isQuadratic)
            {
                AddFlags(p0, PointInfo.Quadratic);
                AddFlags(p1, PointInfo.Quadratic);
                midpoint = (this[p1] - this[p0]) * 0.5F;
                this[p0] += midpoint;
                this[p1] -= midpoint;
            }
            else if (isCubic)
            {
                AddFlags(p0, PointInfo.Cubiq);
                AddFlags(p1, PointInfo.Cubiq);
                midpoint = (this[p0] - pos0) * 0.5F;
                this[p0] = pos0 + midpoint;
                midpoint = (this[p1] - pos1) * 0.5F;
                this[p1] = pos1 + midpoint;
            }
        }

        private void RemoveFlags(int index, params PointInfo[] flags)
        {
            PointInfo current = (PointInfo)_flags.GetArrayElementAtIndex(index).intValue;
            foreach (PointInfo flag in flags)
            {
                if((current & flag) != flag)continue;
                current = current ^ flag;
            }
            _flags.GetArrayElementAtIndex(index).intValue = (int)current;
        }

        private void AddFlags(int index, PointInfo flags)
        {
            PointInfo current = (PointInfo)_flags.GetArrayElementAtIndex(index).intValue;
            _flags.GetArrayElementAtIndex(index).intValue = (int)(current | flags);
        }

        /// <inheritdoc cref="SerializedObject.Update"/>
        public void Update()
        {
            _serializedObject.Update();
        }

        /// <inheritdoc cref="SerializedObject.ApplyModifiedProperties"/>
        public bool ApplyModifiedProperties()
        {
            return _serializedObject.ApplyModifiedProperties();
        }

        public bool PointInfoHas(int index, PointInfo type)
        {
            if (Count <= index) return false;
            var info = (PointInfo) _flags.GetArrayElementAtIndex(index).intValue;
            return (info & type) == type;
        }

        public int GetPointHash(int index)
        {
            return GetHashCode() + index;
        }

        public int GetPointIndex(int hash)
        {
            int count = Count;
            int hashCode = GetHashCode();
            for (int i = 0; i < count; i++)
            {
                if (hashCode + i != hash) continue;
                return i;
            }
            return -1;
        }

        public SplineContent[] GetPoints(PointInfo filter = Scripts.Core.PointInfo.Reference)
        {
            int n = Count;
            List<SplineContent> result = new List<SplineContent>();
            for (int i = 0; i < n; i++)
            {
                PointInfo info = GetPointInfo(i);
                if ((info & filter) == 0) continue;
                SplineContent content = new SplineContent(this[i], info, i, _spline.GetInstanceID());
                result.Add(content);
            }
            return result.ToArray();
        }

        public SplineContent[] GetPoints(int[] indexes)
        {
            int n = indexes.Length;
            SplineContent[] result = new SplineContent[indexes.Length];
            for (int i = 0; i < n; i++)
            {
                int index = indexes[i];
                if (index < 0 || index >= Count) continue;
                PointInfo info = GetPointInfo(index);

                SplineContent content = new SplineContent(this[index], info, i, _spline.GetInstanceID());
                result[i] = content;
            }
            return result.ToArray();
        }

        public SplineContent GetPoint(int index)
        {
            if (index < 0 || index >= Count) return null;
            PointInfo info = GetPointInfo(index);

            SplineContent content = new SplineContent(this[index], info, index, _spline.GetInstanceID());
            return content;
        }

        public SplineContent[] GetSegments(PointInfo filter =
            PointInfo.Linear | PointInfo.Quadratic | PointInfo.Cubiq)
        {
            int n = SegmentsCount;
            List<SplineContent> result = new List<SplineContent>();
            for (int i = 0; i < n; i++)
            {
                int index = i * Spline.VLen * Spline.SLen;
                PointInfo info = GetPointInfo(index);
                if ((info & filter) == 0) continue;
                SplineContent content = new SplineContent(
                    this[index], this[index + 1], this[index + 2], this[index + 3],
                    info, i, _spline.GetInstanceID());
                result.Add(content);
            }
            return result.ToArray();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _serializedObject.targetObject.name;
        }


        #region Custructing

        public SplineBridge([NotNull] SerializedObject serializedObject)
        {
            if (serializedObject == null) throw new ArgumentNullException("serializedObject");

            if (serializedObject.isEditingMultipleObjects)
                throw new ArgumentException("Bridge can peocessed only single object", "serializedObject");

            serializedObject = GetSerializedSpline(serializedObject);
            _coodrinates = serializedObject.FindProperty(CoordinatesField);
            _flags = serializedObject.FindProperty(FlagsField);
            _sf = serializedObject.FindProperty(SfField);
            _serializedObject = serializedObject;
            _spline = (Spline) _serializedObject.targetObject;
        }

        public static SerializedObject GetSerializedSpline(SerializedObject serializedObject)
        {
            if (serializedObject == null) throw new ArgumentNullException("serializedObject");
            var type = typeof(Spline);
            if (serializedObject.targetObject.GetType() == type)
                return serializedObject;

            var f = GetSplineField(serializedObject, type);
            return f == null ? null : new SerializedObject(f.objectReferenceValue);
        }

        public static Object GetUnserializedSpline(SerializedObject serializedObject)
        {
            if (serializedObject == null) throw new ArgumentNullException("serializedObject");
            var type = typeof(Spline);
            if (serializedObject.targetObject.GetType() == type)
                return serializedObject.targetObject;

            var f = GetSplineField(serializedObject, type);
            return f == null ? null : f.objectReferenceValue;
        }

        private static SerializedProperty GetSplineField(SerializedObject serializedObject, Type type)
        {
            var fields = serializedObject.targetObject.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (from field in fields
                where field.FieldType == type
                select
                serializedObject.FindProperty(field.Name)).FirstOrDefault();
        }

        #endregion
    }
}