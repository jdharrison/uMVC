﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace uMVC
{
	public abstract class View : MonoBehaviour
	{
		private readonly Dictionary<string, List<Action>> _listeners = new Dictionary<string, List<Action>>();

		[SerializeField] private bool _showOnStart;
		[SerializeField] private string _containerId;
		[SerializeField] private UnityEvent _onShow;
		[SerializeField] private UnityEvent _onHide;

		public bool Active { get; private set; }

		private bool _loaded;

		public string ContainerId
		{
			get { return _containerId; }
		}

		private void Awake()
		{
			gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			Unload(false);
		}

		public IEnumerator Load()
		{
			if (Active)
				Unload(false);

			yield return Setup();
			Active = true;

			if (_showOnStart)
				Show();
		}

		public void Unload(bool destroy = true)
		{
			if (!Active)
				return;

			Active = false;
			ClearListeners();
			Cleanup();

			if (destroy)
				Destroy(gameObject);
		}

		public void Show()
		{
			if (!Active)
				throw new InvalidOperationException("[uMVC] Attempting to show view but it has not been loaded");

			if (gameObject.activeSelf)
				return;

			gameObject.SetActive(true);
			_onShow.Invoke();
		}

		public void Hide()
		{
			Hide(false);
		}

		// ReSharper disable once MethodOverloadWithOptionalParameter
		public void Hide(bool instant = false)
		{
			if (!Active)
				throw new InvalidOperationException("[uMVC] Attemping to hide view but it has not been loaded");

			if (!gameObject.activeSelf)
				return;

			if (!instant)
			{
				_onHide.Invoke();
				StartCoroutine(DelayedHide());
			}
			else
				gameObject.SetActive(false);
		}

		private IEnumerator DelayedHide()
		{
			yield return new WaitForSeconds(5);
			gameObject.SetActive(false);
		}

		protected abstract IEnumerator Setup();
		protected abstract void Cleanup();

		public void Notify(string type)
		{
			GetListeners(type).ForEach(callback => { callback(); });
		}

		public void AddListener(string type, Action response)
		{
			GetListeners(type).Add(response);
		}

		public void RemoveListener(string type, Action response)
		{
			GetListeners(type).Remove(response);
		}

		public void ClearListeners(string type)
		{
			if (_listeners.ContainsKey(type))
				_listeners[type].Clear();
		}

		public void ClearListeners()
		{
			_listeners.Clear();
		}

		private List<Action> GetListeners(string type)
		{
			if (!_listeners.ContainsKey(type))
				_listeners[type] = new List<Action>();

			return _listeners[type];
		}
	}
}