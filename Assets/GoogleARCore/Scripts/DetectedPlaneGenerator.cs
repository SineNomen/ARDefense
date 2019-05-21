//-----------------------------------------------------------------------
// <copyright file="DetectedPlaneGenerator.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.Common {
	using System.Collections.Generic;
	using GoogleARCore;
	using UnityEngine;

	/// <summary>
	/// Manages the visualization of detected planes in the scene.
	/// </summary>
	public class DetectedPlaneGenerator : MonoBehaviour {
		/// <summary>
		/// A prefab for tracking and visualizing detected planes.
		/// </summary>
		public GameObject DetectedPlanePrefab;

		/// <summary>
		/// A list to hold new planes ARCore began tracking in the current frame. This object is
		/// used across the application to avoid per-frame allocations.
		/// </summary>
		private List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();
		private HashSet<DetectedPlane> m_Planes = new HashSet<DetectedPlane>();

		public DetectedPlane BiggestPlane { get; private set; }
		public DetectedPlaneVisualizer BiggestPlaneVisualizer { get; private set; }

		/// <summary>
		/// The Unity Update method.
		/// </summary>
		public void Update() {
			// Check that motion tracking is tracking.
			if (Session.Status != SessionStatus.Tracking) {
				return;
			}

			// Iterate over planes found in this frame and instantiate corresponding GameObjects to
			// visualize them.
			int count = m_Planes.Count;
			Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);
			for (int i = 0; i < m_NewPlanes.Count; i++) {
				DetectedPlane plane = m_NewPlanes[i];
				m_Planes.Add(plane);
				Debug.LogError("Creating new plane");
				// Instantiate a plane visualization prefab and set it to track the new plane. The
				// transform is set to the origin with an identity rotation since the mesh for our
				// prefab is updated in Unity World coordinates.
				GameObject planeObject =
					Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
				planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(plane);
				planeObject.gameObject.name = string.Format("Plane #{0}", m_Planes.Count - 1);

				if (BiggestPlane == null || plane.Area > BiggestPlane.Area) {
					BiggestPlane = plane;
					BiggestPlaneVisualizer = planeObject.GetComponent<DetectedPlaneVisualizer>();
				}
			}

			if (count != m_Planes.Count) {
				Debug.LogFormat("Biggest plane: {0}[{1}]", BiggestPlaneVisualizer.gameObject.name, BiggestPlane.Area);
			}
		}
	}
}
