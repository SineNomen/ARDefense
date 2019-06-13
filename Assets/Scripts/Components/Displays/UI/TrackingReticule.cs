using UnityEngine;
using UnityEngine.UI;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.ARDefense.Interfaces;
using AOFL.Promises.V1.Interfaces;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using System.Linq;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(AudioSource))]
	public class TrackingReticule : SimpleReticule, IReticule {
		[SerializeField]
		protected float _trackingTime = 2.0f;
		[SerializeField]
		protected CanvasGroup _lockGroup = null;
		[SerializeField]
		protected AudioClip _trackedClip = null;
		[SerializeField]
		protected AudioClip _lockedClip = null;
		//mark an object as tracked when we've been looking at it long enough
		private GameObject _trackedObject = null;
		private float _trackTime = 0.0f;
		protected AudioSource _source;

		protected override void Awake() {
			base.Awake();
			_source = GetComponent<AudioSource>();
		}

		protected override void Update() {
			List<GameObject> newObjects = new List<GameObject>();
			Ray ray = _gameManager.DeviceCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
			//everything BUT the player
			int mask = ~LayerMask.GetMask("Player");
			RaycastHit[] hits = Physics.SphereCastAll(ray, _radius, _targettingRange, mask);
			if (hits.Length > 0) {
				foreach (RaycastHit hit in hits) {
					// Debug.LogFormat("Looking at: {0}", hit.transform.name);
					newObjects.Add(hit.transform.gameObject);
				}
			}

			//Objects that were targeted, but not anymore
			foreach (GameObject obj in _targetedObjects.Except(newObjects)) {
				//`Mat Broadcast message
				if (obj != null) {
					obj.BroadcastMessage("OnUntargeted", SendMessageOptions.DontRequireReceiver);
				}
			}
			//The newly targeted obejcts
			foreach (GameObject obj in newObjects.Except(_targetedObjects)) {
				//`Mat Broadcast message
				if (obj != null) {
					obj.BroadcastMessage("OnTargeted", SendMessageOptions.DontRequireReceiver);
					if (_trackedObject == null && IsEnemy(obj)) {
						_trackedObject = obj;
						Debug.LogWarningFormat("Tracked: {0}", _trackedObject);
						_highlightGroup.Show(0.1f);
						_trackTime = Time.time + _trackingTime;
					}
				}
			}
			_targetedObjects = newObjects;
			if (_trackedObject != null) {
				if (!_targetedObjects.Contains(_trackedObject)) {
					_trackedObject = null;
					LockedObject = null;
					_highlightGroup.Hide(0.1f);
					_lockGroup.Hide(0.1f);
				} else {
					if (LockedObject == null && Time.time > _trackTime) {
						LockedObject = _trackedObject;
						_highlightGroup.Hide(0.1f);
						_lockGroup.Show(0.1f);
						Debug.LogErrorFormat("Locked: {0}", LockedObject);
					}
				}
			}
		}

		private bool IsEnemy(GameObject obj) {
			IKillable killable = obj.GetComponent<IKillable>();
			return killable != null && killable.Team != eKillableTeam.Player1;
		}
	}
}