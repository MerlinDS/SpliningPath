// <copyright file="EditingController.cs" company="Near Fancy">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Andrew Salomatin</author>
// <date>03/26/2017 11:01</date>

using SpliningPath.Scripts.Core;
using UnityEngine;

namespace SpliningPath.Scripts.Controllers
{
    /// <summary>
    /// EditingController - helper class for spline editing.
    /// Will be added automaticaly
    /// </summary>
    public class EditingController : MonoBehaviour
    {
        //================================       Public Setup       =================================
        [SerializeField][HideInInspector]
        public Spline Spline;
        //================================    Systems properties    =================================
        //================================      Public methods      =================================

        //================================ Private|Protected methods ================================
        private void Awake()
        {
            Debug.Log("On Awake");
        }
    }
}