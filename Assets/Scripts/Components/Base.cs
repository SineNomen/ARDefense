using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using UnityEngine;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(SimpleKillable))]
	[RequireComponent(typeof(Rigidbody))]
	public class Base : MonoBehaviour {
		[SerializeField]
		private eKillableTeam _team = eKillableTeam.Player1;
		[SerializeField]

		public eKillableTeam Team { get => _team; set => _team = value; }
		public Rigidbody Body { get; private set; }
		public Transform Transform { get => this.transform; }
		public Vector3 CenterPosition { get => this.transform.position + Vector3.up * 0.5f; }

		private void Start() {
			Container.AutoInject(this);
			Body = GetComponent<Rigidbody>();
		}
	}
}