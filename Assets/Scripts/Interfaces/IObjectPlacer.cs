using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using UnityEngine.XR.ARFoundation;
using Sojourn.ARDefense.Components;

namespace Sojourn.ARDefense.Interfaces {
	public interface IObjectPlacer {
		IPromise<GameObject> PlaceObject(GameObject prefab, Ground ground);
		IPromise<T> PlaceObject<T>(GameObject prefab, Ground ground) where T : Component;
	}
}