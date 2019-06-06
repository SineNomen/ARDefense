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
	public class PlayerHUD : MonoBehaviour, IPlayerHUD {
		[SerializeField]
		private TMP_Text _scoreText = null;
		[SerializeField]
		private Toggle[] _weaponToggles = null;

		private CanvasGroup _canvasGroup;
		private int _dropshipScore = 50;
		private int _tankScore = 10;
		private int _fighterScore = 20;
		private int _currentScore = 0;

		[AutoInject]
		private ILevelManager _levelManager = null;
		[AutoInject]
		private IPlayer _player = null;

		private void Awake() {
			Container.Register<IPlayerHUD>(this).AsSingleton();
			_canvasGroup = GetComponent<CanvasGroup>();
			HideInstant();
		}

		private void Start() {
			Container.AutoInject(this);
			SetScore();
			_levelManager.OnLevelStarted += OnLevelStarted;
			_levelManager.OnEnemyKilled += OnEnemyKilled;

			for (int i = 0; i < _weaponToggles.Length; i++) {
				int index = i;
				_weaponToggles[index].onValueChanged.AddListener((isOn) => {
					if (isOn) {
						OnWeaponToggle(index);
					}
				});
			}
		}

		private void OnWeaponToggle(int i) {
			_player.SetCurrentWeapon(i);
		}

		private void OnLevelStarted() {
			_currentScore = 0;
			//setup the weapon toggles
			for (int i = 0; i < _weaponToggles.Length; i++) {
				if (i < _player.WeaponList.Capacity) {
					_weaponToggles[i].gameObject.SetActive(true);
					Transform parent = _weaponToggles[i].transform;
					GameObject prefab = _player.WeaponList[i].UIIcon;
					GameObject obj = Instantiate(prefab, parent);
					RectTransform rect = obj.transform as RectTransform;
					rect.anchoredPosition = Vector3.zero;
				} else {
					_weaponToggles[i].gameObject.SetActive(false);
				}
			}
			// _weaponToggles[0].isOn = true;
		}

		private void OnEnemyKilled(GameObject go) {
			Dropship dropship = go.GetComponent<Dropship>();
			Fighter fighter = go.GetComponent<Fighter>();
			Tank tank = go.GetComponent<Tank>();
			if (dropship != null) {
				_currentScore += _dropshipScore;
			} else if (fighter != null) {
				_currentScore += _fighterScore;
			} else if (tank != null) {
				_currentScore += _tankScore;
			}
			SetScore();
		}

		public void ShowInstant() { _canvasGroup.ShowInstant(); }
		public void HideInstant() { _canvasGroup.HideInstant(); }

		public IPromise Show() { return _canvasGroup.Show(0.1f); }
		public IPromise Hide() { return _canvasGroup.Hide(0.1f); }

		private void SetScore() {
			_scoreText.text = _currentScore.ToString("N0");
		}
	}
}