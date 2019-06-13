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
	public class TrackingReticule : SimpleReticule, IReticule {
		[SerializeField]
		protected float _trackingTime = 3.0f;
		//mark an object as tracked when we've been looking at it long enough

		protected override void Update() {
			List<GameObject> newObjects = new List<GameObject>();
			Ray ray = _gameManager.DeviceCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
			//everything BUT the player
			int mask = ~LayerMask.GetMask("Player");
			RaycastHit[] hits = Physics.SphereCastAll(ray, _radius, Mathf.Infinity, mask);
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
					if (TrackedObject == null) {
						TrackedObject = obj;
					}
				}
			}
			_targetedObjects = newObjects;
			if (TrackedObject != null && !_targetedObjects.Contains(TrackedObject)) {
				TrackedObject = null;
			}
			Debug.LogFormat("Tracked: {0}", TrackedObject);
		}
	}
}