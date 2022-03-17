using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	[System.Serializable]
    public class BulletSpawner
    {
		[SerializeField]
		BulletModel bulletModel;
		[SerializeField]
		public Color[] colors = new Color[] { Color.red };
		[SerializeField]
		public int ways = 1;
		[SerializeField]
		public float angle;
		[SerializeField]
		public float angleDelta = 30;
		[SerializeField]
		public float rotateSpeed = 0;
		[SerializeField]
		public int span = 5;
		[SerializeField]
		public int count = 0;
		[SerializeField]
		public int loopLength = 0;
		[SerializeReference, BulletBehaviourSelector]
		IBulletBehaviour behaviour = new DefaultBehaviour();
		[SerializeField]
		UnityEngine.Events.UnityEvent onFire;

		[field: System.NonSerialized]
		public Vector3 Position { get; set; }
		[field: System.NonSerialized]
		public Quaternion Rotation { get; set; } = Quaternion.identity;

		public BulletModel Model => bulletModel;
		public IBulletBehaviour Behaviour => behaviour;

		public Danmaku DanmakuInstance { get; set; } = null;

		int currentFrame;
		public void Update()
		{
			if (span <= 1)
				Fire();
			else if (currentFrame % span == 0)
				Fire();

			currentFrame++;
			if (currentFrame == loopLength)
			{
				currentFrame = 0;
				colorIndex = 0;
				currentCount = 0;
			}
		}

		int colorIndex = 0;
		int currentCount = 0;
		void Fire()
		{
			if (count > 0 && currentCount >= count) return;

			for (int i = 0; i < ways; i++)
			{
				float totalWidth = angleDelta * (ways - 1);
				float ang = angle - totalWidth / 2f + i * angleDelta;
				(DanmakuInstance ?? Danmaku.Instance).AddBullet(bulletModel, Position, Rotation * Quaternion.Euler(ang, 90f, 0), colors[colorIndex % colors.Length], behaviour);
			}

			colorIndex++;
			currentCount++;
			angle += rotateSpeed;
			if (angle > 360f) angle -= 360f;

			onFire?.Invoke();
		}
	}
}
