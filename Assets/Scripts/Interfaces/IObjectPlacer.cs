using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using TMPro;

public interface IObjectPlacer {
	IPromise<GameObject> PlaceObjectOnGround(GameObject prefab);
	IPromise<T> PlaceObjectOnGround<T>(GameObject prefab) where T : Component;
}
