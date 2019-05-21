using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using AOFL.Promises.V1.Core;
using AOFL.Promises.V1.Interfaces;

namespace Extensions {
	public static class TweenExtensions {
		public static IPromise ToPromise(this Tween tween) {
			Promise p = new Promise();
			tween.OnComplete(p.Resolve);
			return p;
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
	}

	public static class FloatExtensions {

		public static Tween CountTo(this float f, float time) {
			// return cg.DOFade(1.0f, time);
			return null;
		}
	}
}