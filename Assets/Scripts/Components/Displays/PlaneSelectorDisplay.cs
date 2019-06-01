using Sojourn.ARDefense.Interfaces;
using UnityEngine;
using UnityEngine.UI;

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
		public Button ChooseButton { get => _chooseButton; }

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