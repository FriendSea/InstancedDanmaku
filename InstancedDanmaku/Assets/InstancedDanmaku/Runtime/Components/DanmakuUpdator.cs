using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	public class DanmakuUpdator : MonoBehaviour
	{
		private void FixedUpdate()
		{
			if (DanmakuSettings.Instance.useFixedUpdate)
				Danmaku.Instance.Update();
		}

		private void Update()
		{
			if (!DanmakuSettings.Instance.useFixedUpdate)
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
