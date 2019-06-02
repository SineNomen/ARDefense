using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using System.Collections.Generic;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	public class VisualPlane : MonoBehaviour {
		public static List<VisualPlane> PlaneList { get; } = new List<VisualPlane>();

		public static void DestroyAll() {
			foreach (VisualPlane plane in PlaneList) {
				Destroy(plane.gameObject);
			}
		}

		private void Awake() {
			PlaneList.Add(this);
		}
		private void OnDestroy() {
			PlaneList.Remove(this);
		}
	}
}