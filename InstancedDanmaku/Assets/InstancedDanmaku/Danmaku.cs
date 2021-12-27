using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System.Linq;

namespace InstancedDanmaku
{
	public struct Bullet
	{
		public IBulletBehaviour behaviour;
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 velocity;
		public int CurrentFrame { get; private set; }
		public bool Active { get; private set; }
		internal bool Used { get; set; }

		internal Bullet(Vector3 position, Quaternion rotation, Vector3 velocity, IBulletBehaviour behaviour)
		{
			this.behaviour = behaviour;
			this.position = position;
			this.rotation = rotation;
			this.velocity = velocity;
			Used = true;
			Active = true;
			CurrentFrame = 0;
		}

		internal void Update()
		{
			if (!Active) return;
			behaviour.UpdateBullet(ref this);
			position += velocity;
			CurrentFrame++;
		}
		public void Destroy() => Active = false;
	}

	public interface IBulletBehaviour
	{
		void UpdateBullet(ref Bullet bullet);
	}

	class BulletGroup
	{
		const int MAX_BULLETS = 1023;

		BulletModel model;

		Bullet[] bullets;
		Matrix4x4[] matricies;
		Vector4[] colors;
		internal Stack<int> Unused { get; }

		internal BulletGroup(BulletModel model)
		{
			this.model = model;

			bullets = new Bullet[MAX_BULLETS];
			matricies = new Matrix4x4[MAX_BULLETS];
			colors = new Vector4[MAX_BULLETS];
			Unused = new Stack<int>(Enumerable.Range(0, MAX_BULLETS));
		}

		internal void AddNewBullet(Vector3 position, Quaternion rotation, Vector3 velocity, Color color, IBulletBehaviour behaviour)
		{
			var index = Unused.Pop();
			bullets[index] = new Bullet(position, rotation, velocity, behaviour);
			colors[index] = new Vector4(color.r, color.g, color.b, color.a);
		}

		internal void UpdateBullets()
		{
			for (int i = 0; i < bullets.Length; i++)
			{
				matricies[i] = bullets[i].Active ? Matrix4x4.TRS(bullets[i].position, bullets[i].rotation, model.scale) : Matrix4x4.zero;

				bullets[i].Update();
				if(!bullets[i].Active && bullets[i].Used)
				{
					bullets[i].Used = false;
					Unused.Push(i);
				}
			}
		}

		MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
		internal void Render()
		{
			propertyBlock.Clear();
			propertyBlock.SetVectorArray("_Color", colors);
			Graphics.DrawMeshInstanced(model.mesh, 0, model.material, matricies, MAX_BULLETS, propertyBlock);
		}
	}

	public class Danmaku
	{
		class DefaultBehaviour : IBulletBehaviour
		{
			public void UpdateBullet(ref Bullet bullet)
			{
				if (bullet.CurrentFrame > 200)
					bullet.Destroy();
			}
		}

		DefaultBehaviour defaultBehaviour = new DefaultBehaviour();

		static Danmaku _instance = null;
		public static Danmaku Instance => _instance ?? (_instance = new Danmaku());

		Dictionary<BulletModel, BulletGroup> groups = new Dictionary<BulletModel, BulletGroup>();

		public void AddBullet(BulletModel model, Vector3 position, Quaternion rotation, Vector3 velocity, Color color)
		{
			if (!groups.ContainsKey(model))
				groups.Add(model, new BulletGroup(model));

			groups[model].AddNewBullet(position, rotation, velocity, color, defaultBehaviour);
		}

		public void Update()
		{
			foreach (var group in groups)
			{
				group.Value.UpdateBullets();
			}
		}

		public void Render()
		{
			foreach (var group in groups)
			{
				group.Value.Render();
			}
		}
	}
}
