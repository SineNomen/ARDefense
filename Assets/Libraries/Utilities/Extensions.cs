using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Sojourn.Extensions {
	public static class MonoBehaviourExtensions {
		public static IPromise StartCoroutineAsPromise(this MonoBehaviour mb, IEnumerator co) {
			Promise p = new Promise();
			mb.StartCoroutine(mb.Runner(co, p));
			return p;
		}

		public static IEnumerator Runner(this MonoBehaviour mb, IEnumerator co, IPromise p) {
			yield return mb.StartCoroutine(co);
			p.Resolve();
		}

		public static IPromise<T> StartCoroutineAsPromise<T>(this MonoBehaviour mb, IEnumerator co) {
			Promise<T> p = new Promise<T>();
			mb.StartCoroutine(mb.Runner<T>(co, p));
			return p;
		}

		public static IEnumerator Runner<T>(this MonoBehaviour mb, IEnumerator co, IPromise<T> p) {
			T ret = default(T);
			while (co.MoveNext()) {
				ret = (T)co.Current;
				yield return default(T);
			}
			p.Resolve(ret);
		}
	}

	public static class TweenExtensions {
		public static IPromise ToPromise(this Tween tween) {
			Promise p = new Promise();
			tween.OnComplete(p.Resolve);
			return p;
		}
	}

	public static class TransformExtensions {
		public static void SetPose(this Transform tr, Pose pose) {
			tr.position = pose.position;
			tr.rotation = pose.rotation;
		}
	}

	public static class CanvasGroupExtensions {
		public static void SetBlocking(this CanvasGroup cg, bool block) {
			cg.interactable = block;
			cg.blocksRaycasts = block;
		}

		public static Tween ShowTween(this CanvasGroup cg, float time) {
			cg.SetBlocking(true);
			return cg.DOFade(1.0f, time);
		}

		public static Tween HideTween(this CanvasGroup cg, float time) {
			return cg.DOFade(0.0f, time).OnComplete(() => {
				cg.SetBlocking(false);
			});
		}

		public static IPromise Show(this CanvasGroup cg, float time) { return cg.ShowTween(time).ToPromise(); }
		public static IPromise Hide(this CanvasGroup cg, float time) { return cg.HideTween(time).ToPromise(); }

		public static void ShowInstant(this CanvasGroup cg) { cg.SetBlocking(true); cg.alpha = 1.0f; }
		public static void HideInstant(this CanvasGroup cg) { cg.SetBlocking(false); cg.alpha = 0.0f; }
	}
}