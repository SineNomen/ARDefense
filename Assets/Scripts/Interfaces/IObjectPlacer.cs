using UnityEngine;
using AOFL.Promises.V1.Interfaces;

namespace Sojourn.ARDefense.Interfaces {
	public interface IObjectPlacer {
		IPromise<GameObject> PlaceObjectOnGround(GameObject prefab);
		IPromise<T> PlaceObjectOnGround<T>(GameObject prefab) where T : Component;
	}
}