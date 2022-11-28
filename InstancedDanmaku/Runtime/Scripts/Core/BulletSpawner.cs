using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	[System.Serializable]
	public class BulletSpawner
	{
		[SerializeField]
		public int loopLength = 0;
		[SerializeField]
		public FlexibleValue startSpeed;
		[SerializeField]
		public FlexibleValue ways = new FlexibleValue(1);
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
		[SerializeReference, BulletBehaviourSelector]
		IBulletBehaviour behaviour = new DefaultBehaviour();
		[SerializeField]
		BulletModel bulletModel;
		[SerializeField]
		public Color[] colors = new Color[] { Color.red };
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

			for (int i = 0; i < ways.GetInt(currentFrame); i++)
			{
				float totalWidth = angleDelta * (ways.GetInt(currentFrame) - 1);
				float ang = angle - totalWidth / 2f + i * angleDelta;
				var rot = Rotation * Quaternion.Euler(ang, 90f, 0);
				(DanmakuInstance ?? Danmaku.Instance).AddBullet(bulletModel, Position, rot, colors[colorIndex % colors.Length], behaviour, rot * Vector3.forward * startSpeed.GetValue(currentFrame));
			}

			colorIndex++;
			currentCount++;
			angle += rotateSpeed;
			if (angle > 360f) angle -= 360f;

			onFire?.Invoke();
		}

		IEnumerable<Quaternion> GetRotations()
		{
			for (int i = 0; i < ways.GetInt(currentFrame); i++)
			{
				float totalWidth = angleDelta * (ways.GetInt(currentFrame) - 1);
				float ang = angle - totalWidth / 2f + i * angleDelta;
				yield return Rotation * Quaternion.Euler(ang, 90f, 0);
			}
		}

		public void DrawGizmos()
		{
#if UNITY_EDITOR
			/*
			Gizmos.color = Color.red;

			foreach(var ang in GetRotations())
			{
				var bullet = new Bullet(Position, ang, ang * Vector3.forward * startSpeed.GetValue(0), Vector4.zero, Behaviour);
				var beforepos = Position;
				for (int i = 0; i < 1000; i++)
				{
					bullet.Update();
					if (!bullet.Active) break;

					Gizmos.DrawLine(beforepos, bullet.position);
					beforepos = bullet.position;
				}
			}
			*/
#endif
		}
	}
}
