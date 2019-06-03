using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
//`Mat hack, shouldn't need components
using Sojourn.ARDefense.Components;

namespace Sojourn.ARDefense.Interfaces {
	public interface IGameManager {
		ARSession ArSession { get; }
		ARRaycastManager RaycastManager { get; }
		ARReferencePointManager PointManager { get; }
		ARPlaneManager PlaneManager { get; }
		ARSessionOrigin Origin { get; }
		Transform WorldParent { get; }

		float CameraHeight { get; }
		Camera DeviceCamera { get; }
	}
}