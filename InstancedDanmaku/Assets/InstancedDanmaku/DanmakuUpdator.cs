using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	public class DanmakuUpdator : MonoBehaviour
	{
		private void FixedUpdate()
		{
			Danmaku.Instance.Update();
		}

		private void Update()
		{
			Danmaku.Instance.Render();
		}
	}
}
