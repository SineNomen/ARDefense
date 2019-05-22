using UnityEngine;
using AOFL.Promises.V1.Interfaces;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public interface IGameManager {
	float MinGroundArea { get; }
	DetectedPlane GroundPlane { get; }
	DetectedPlaneVisualizer GroundPlaneVisualizer { get; }
	ARCoreSession ArSession { get; }
	Camera DeviceCamera { get; }
	IPromise StartNewGame();
}
