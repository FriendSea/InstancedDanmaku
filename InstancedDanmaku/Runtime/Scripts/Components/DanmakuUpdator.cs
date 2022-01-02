using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	public class DanmakuUpdator : MonoBehaviour
	{
		[SerializeField]
		DanmakuSettings danmakuSettings;

		private void Awake()
		{
			DanmakuSettings.Current = danmakuSettings;
		}

		private void FixedUpdate()
		{
			if (DanmakuSettings.Current.useFixedUpdate)
				Danmaku.Instance.Update();
		}

		private void Update()
		{
			if (!DanmakuSettings.Current.useFixedUpdate)
				Danmaku.Instance.Update();
			Danmaku.Instance.Render();
		}

		private void OnDrawGizmos()
		{
			Danmaku.Instance.DrawGizmos();
		}

		private void OnDestroy()
		{
			Danmaku.Instance.Dispose();
		}
	}
}
