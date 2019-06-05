using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Sojourn.ARDefense.Components {
	//curently no dfferent from Enemy, but some patterns are meant for a specific type of enemy
	[RequireComponent(typeof(Collider))]
	public class Fighter : SimpleEnemy {
		[SerializeField]
		private Transform _model = null;

		private Collider _collider = null;
		private static int _createCount = 0;

		public Transform Model { get => _model; }
		public Vector3 Size { get => _collider.bounds.size; }

		protected override void Awake() {
			base.Awake();
			gameObject.name = string.Format("Fighter #{0}", _createCount);
			_collider = GetComponent<Collider>();
			_createCount++;
		}

	}
}