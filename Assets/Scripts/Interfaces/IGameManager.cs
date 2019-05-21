using AOFL.Promises.V1.Interfaces;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public interface IGameManager {
	float MinGroundArea { get; }
	DetectedPlane GroundPlane { get; }
	DetectedPlaneVisualizer GroundPlaneVisualizer { get; }

	IPromise StartNewGame();
}
