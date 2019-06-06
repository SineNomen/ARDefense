using Sojourn.ARDefense.Interfaces;
using Sojourn.PicnicIOC;
using Sojourn.Extensions;
using Sojourn.Utility;
using Sojourn.Interfaces;
using UnityEngine;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using TMPro;

namespace Sojourn.ARDefense.Components {
	public enum eModalOption {
		OK,
		Cancel
	}
	[RequireComponent(typeof(CanvasGroup))]
	public class OKCancelModal : MonoBehaviour {
		[SerializeField]
		private ButtonGroup _okButton = null;
		[SerializeField]
		private ButtonGroup _cancelButton = null;
		[SerializeField]
		private TMP_Text _title = null;
		[SerializeField]
		private TMP_Text _bodyText = null;

		private CanvasGroup _canvasGroup;
		private IPromise<eModalOption> _promise;

		[AutoInject]
		private ILevelManager _levelManager = null;
		[AutoInject]
		private IPersistentDataManager _saveDataManager = null;

		private void Awake() {
			_canvasGroup = GetComponent<CanvasGroup>();
			_canvasGroup.HideInstant();
		}

		private void OnOk() {
			_canvasGroup.Hide(0.15f)
			.Then(() => {
				_promise.Resolve(eModalOption.OK);
			});
		}

		private void OnCancel() {
			_canvasGroup.Hide(0.15f)
			.Then(() => {
				_promise.Resolve(eModalOption.Cancel);
			});
		}

		public void Setup(string title, string bodyText, string okText = "OK", string cancelText = "Cancel") {
			_title.text = title;
			_bodyText.text = bodyText;
			_okButton.Text.text = okText;
			_cancelButton.Text.text = cancelText;
			_okButton.Button.onClick.AddListener(OnOk);
			if (cancelText == null) {
				_cancelButton.gameObject.SetActive(false);
			} else {
				_cancelButton.Button.onClick.AddListener(OnCancel);
			}
		}

		public IPromise<eModalOption> DoModal() {
			_promise = new Promise<eModalOption>();
			_canvasGroup.Show(0.15f);
			return _promise;
		}
	}
}