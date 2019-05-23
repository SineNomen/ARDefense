using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using GoogleARCore;
using GoogleARCore.Examples.Common;

namespace Sojourn.ARDefense.Interfaces {
	public interface IObjectPlacer {
		IPromise<GameObject> PlaceObjectOnPlane(GameObject prefab, DetectedPlane plane = null);
		IPromise<T> PlaceObjectOnPlane<T>(GameObject prefab, DetectedPlane plane = null) where T : Component;
	}
}