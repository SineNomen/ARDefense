using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Sojourn.ARDefense.Components {
	//curently no dfferent from Enemy, but some patterns are meant for a specific type of enemy
	public class Fighter : SimpleEnemy {
		[SerializeField]
		private Transform _model = null;
		[SerializeField]
		private float _modelSpeedScale = 50.0f;
		private static int _createCount = 0;

		public Transform Model { get => _model; }
		private Vector3 _lastVelocity;

		private int _velCapacity = 50;
		private Queue<System.Tuple<Vector3, float>> _velocityQueue;

		protected override void Awake() {
			base.Awake();
			gameObject.name = string.Format("Fighter #{0}", _createCount);
			_createCount++;
			_velocityQueue = new Queue<System.Tuple<Vector3, float>>(_velCapacity);
		}

		private void Update() {
			// Model.transform.rotation
			if (_velocityQueue.Count == _velCapacity) { _velocityQueue.Dequeue(); }
			_velocityQueue.Enqueue(new System.Tuple<Vector3, float>(Body.velocity, Time.deltaTime));
			Vector3 sum = Vector3.zero;
			float time = 0.0f;
			if (_velocityQueue.Count == _velCapacity) {
				foreach (System.Tuple<Vector3, float> element in _velocityQueue) {
					sum += element.Item1;
					time += element.Item2;
				}
			}
			//world acc, need to convert to local
			Vector3 acc = (sum / _velocityQueue.Count) / time;
			Quaternion lerp = Quaternion.Lerp(_model.localRotation, Quaternion.AngleAxis(-acc.magnitude * _modelSpeedScale, Vector3.forward), Time.deltaTime);
			_model.localRotation = lerp;

			Debug.LogFormat("Acceleration: {0}, forward: {1}, mag: {2}", acc, transform.forward, acc.magnitude);
		}
	}
}