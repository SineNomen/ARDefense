using UnityEngine;
using System.Collections.Generic;
using AOFL.Promises.V1.Interfaces;

namespace Sojourn.ARDefense.Interfaces {
	public interface IReticule {
		List<GameObject> VisibleObjects { get; }
		GameObject TrackedObject { get; }
		IPromise ShowHighlight(float time = 0.1f);
		IPromise HideHighlight(float time = 0.1f);
		IPromise Unload();
		IPromise Reload();
		void SetReload(float val);
	}
}