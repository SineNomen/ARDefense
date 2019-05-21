// using UnityEngine;
// using AOFL.Promises.V1.Core;
// using AOFL.Promises.V1.Interfaces;
// using System.Collections;
// using System.Collections.Generic;

// public class ConditionPromise {
// 	private float _period;
// 	private System.Func<bool> _condition;
// 	private Promise _promise;

// 	public ConditionPromise(System.Func<bool> c, float p = 0.1f) {
// 		_period = p;
// 		_condition = c;
// 	}

// 	public IPromise Do() {
// 		_promise = new Promise();
// 		StartCoroutine(CheckCondition());
// 		return _promise;
// 	}

// 	private IEnumerator CheckCondition() {
// 		while (_condition() == false) {
// 			yield return new WaitForSeconds(_period);
// 		}
// 		_promise.Resolve();
// 	}
// }
