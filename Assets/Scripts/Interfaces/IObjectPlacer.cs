using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using UnityEngine.XR.ARFoundation;

namespace Sojourn.ARDefense.Interfaces {
	public interface IObjectPlacer {
		IPromise<GameObject> PlaceObjectOnPlane(GameObject prefab, ARPlane plane = null);
		IPromise<T> PlaceObjectOnPlane<T>(GameObject prefab, ARPlane plane = null) where T : Component;
	}
}