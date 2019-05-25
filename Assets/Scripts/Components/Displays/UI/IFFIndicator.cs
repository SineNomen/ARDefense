using UnityEngine;
using UnityEngine.UI;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.ARDefense.Interfaces;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using DG.Tweening;
using TMPro;

namespace Sojourn.ARDefense.Components {
	public enum eIFFCategory {
		Friend,
		Enemy,
		Neutral,
		PointOfInterest
	}
	public class IFFIndicator : MonoBehaviour {
		[SerializeField]
		private GameObject[] _categoryObjects = null;
		[SerializeField]
		private CanvasGroup _group = null;

		public RectTransform Rect { get; set; }
		public eIFFCategory Category { get; set; }
		public bool IsShowing { get; private set; }

		private void Awake() {
			Rect = this.transform as RectTransform;
		}

		public void SetUp(eIFFCategory category) {
			foreach (eIFFCategory c in (eIFFCategory[])System.Enum.GetValues(typeof(eIFFCategory))) {
				if (c == category) {
					_categoryObjects[(int)c].SetActive(true);
				} else {
					_categoryObjects[(int)c].SetActive(false);
				}
			}
			Category = category;
		}

		public IPromise Show() {
			if (IsShowing) { return Promise.Resolved(); }
			IsShowing = true;
			return _group.Show(0.1f);
		}

		public IPromise Hide() {
			if (!IsShowing) { return Promise.Resolved(); }
			IsShowing = false;
			return _group.Hide(0.1f);
		}
	}
}