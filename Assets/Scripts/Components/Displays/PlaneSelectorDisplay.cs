using Sojourn.ARDefense.Interfaces;
using Sojourn.ARDefense.Components;
using Sojourn.Extensions;
using Sojourn.Utility;
using Sojourn.PicnicIOC;
using UnityEngine;
using UnityEngine.UI;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;

public delegate void PlaneSelectorCallback();

namespace Sojourn.ARDefense.Components {

	[RequireComponent(typeof(CanvasGroup))]
	public class PlaneSelectorDisplay : SimpleDisplay, IDisplay {
		[SerializeField]
		private Button _chooseButton = null;
		[SerializeField]
		private Button _cancelButton = null;

		public PlaneSelectorCallback OnChooseButton { get; set; }
		public PlaneSelectorCallback OnCancelButton { get; set; }

		protected override void Start() {
			base.Start();
			_chooseButton.onClick.AddListener(() => {
				if (OnChooseButton != null) { OnChooseButton(); }
			});
			_cancelButton.onClick.AddListener(() => {
				if (OnChooseButton != null) { OnChooseButton(); }
			});
		}
	}
}