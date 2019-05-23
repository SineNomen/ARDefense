using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using GoogleARCore;
using GoogleARCore.Examples.Common;
//`Mat hack, shouldn't need components
using Sojourn.ARDefense.Components;

namespace Sojourn.ARDefense.Interfaces {
	public interface IPlaneSelector {
		float MinGroundArea { get; }
		IPromise<DetectedPlane> SelectPlane();
	}
}