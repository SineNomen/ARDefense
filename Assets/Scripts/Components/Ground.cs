using UnityEngine.XR.ARFoundation;
using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	//The game surface. This will be placed on the chosen ARPlane. All the action takes place here
	public class Ground : MonoBehaviour {
		public ARPlane Plane { get; set; }
		public Transform Transform { get => this.transform; }
		public Vector3 Center { get => this.transform.position; }
		public float Radius {
			get => this.transform.localScale.x;
			set => this.transform.localScale = new Vector3(value, 0.1f, value);
		}

		public Vector3 GetPositionAt(float radianAngle, float distance, float height = 0.0f) {
			if (distance > Radius) {
				Debug.LogErrorFormat("Ground.GetPositionAt: Distance {0} is greater than Radius {1}", distance, Radius);
			}
			Vector2 offset = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle)) * distance;
			Vector3 pos = Center + new Vector3(offset.x, height, offset.y);
			// Debug.LogFormat("GetPositionAt [{0}, {1}], offset: {2} pos: {3}", radianAngle, distance, offset, pos);
			return pos;
		}
	}
}