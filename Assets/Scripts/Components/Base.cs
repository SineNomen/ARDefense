using Sojourn.ARDefense.ScriptableObjects;
using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;

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

		private void OnEnable() {
			Debug.LogError("++Base Enabled");
		}
		private void OnDisable() {
			Debug.LogError("--Base Disabled");
		}
		private void OnDestroy() {
			Debug.LogError("<< Base Destroyed >>");
		}
	}
}