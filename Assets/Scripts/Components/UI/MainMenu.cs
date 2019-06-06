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
using System;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

namespace Sojourn.ARDefense.Components {
	[RequireComponent(typeof(CanvasGroup))]
	public class MainMenu : MonoBehaviour, IPlayerHUD {
		[SerializeField]
		private Button _newGameButton = null;
		[SerializeField]
		private TMP_Text _highScoreText = null;

		private CanvasGroup _canvasGroup;

		[AutoInject]
		private ILevelManager _levelManager = null;

		private void Awake() {
			Container.Register<IPlayerHUD>(this).AsSingleton();
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		private void Start() {
			Container.AutoInject(this);
			SetScore(0);
			_newGameButton.onClick.AddListener(OnNewGame);
		}

		private void OnNewGame() {
			Debug.Log("Starting new game");
			Utilities.PromiseGroup(
				Hide(),
				_levelManager.SetupLevel()
			)
			.Then(_levelManager.StartLevel);
		}

		public void ShowInstant() { _canvasGroup.ShowInstant(); }
		public void HideInstant() { _canvasGroup.HideInstant(); }

		public IPromise Show() { return _canvasGroup.Show(0.1f); }
		public IPromise Hide() { return _canvasGroup.Hide(0.1f); }

		private void SetScore(int score) {
			_highScoreText.text = score.ToString("N0");
		}
	}
}