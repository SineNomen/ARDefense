using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using AOFL.Promises.V1.Interfaces;
using GoogleARCore;
using GoogleARCore.Examples.Common;
//`Mat hack, shouldn't need components
using Sojourn.ARDefense.Components;

namespace Sojourn.ARDefense.Interfaces {
	public interface IGameManager {
		float MinGroundArea { get; }
		DetectedPlane GroundPlane { get; }
		DetectedPlaneVisualizer GroundPlaneVisualizer { get; }
		ARCoreSession ArSession { get; }
		Camera DeviceCamera { get; }
		IPromise StartNewGame();
	}
}