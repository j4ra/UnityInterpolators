/*
 * Based on video by Sebastian Lague: https://www.youtube.com/watch?v=n_RHttAaRCk
 * Complete project on his github
 * https://github.com/SebLague/Curve-Editor
 * 
 * 
 * ©2018 - Jaroslav Nejedlý
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    public class Bezier2DCreator : MonoBehaviour
    {
        [HideInInspector]
        public Bezier2D path;

        /// <summary>
        /// Creates new Bezier2D path at current position
        /// </summary>
        public void CreatePath()
        {
            path = new Bezier2D(transform.position);
        }
    }
}
