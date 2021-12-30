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
		public Vector4 color;
		public int CurrentFrame { get; private set; }
		public bool Active { get; private set; }
		internal bool Used { get; set; }

		internal Bullet(Vector3 position, Quaternion rotation, Vector3 velocity, Vector4 color, IBulletBehaviour behaviour)
		{
			this.behaviour = behaviour;
			this.position = position;
			this.rotation = rotation;
			this.velocity = velocity;
			this.color = color;
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

	public interface IBulletCollider
	{
		void Collide(Bullet bullet);
		bool DeleteBullet { get; }
	}

	class BulletGroup : System.IDisposable
	{
		const int MAX_BULLETS = 1023;

		BulletModel model;

		Bullet[] bullets;
		Matrix4x4[] matricies;
		Vector4[] colors;
		NativeArray<SpherecastCommand> raycastCommands;
		NativeArray<RaycastHit> raycastHits;
		internal Stack<int> Unused { get; }

		internal bool HasEmpty => Unused.Count > 0;

		internal BulletGroup(BulletModel model)
		{
			this.model = model;

			bullets = new Bullet[MAX_BULLETS];
			matricies = new Matrix4x4[MAX_BULLETS];
			colors = new Vector4[MAX_BULLETS];
			raycastCommands = new NativeArray<SpherecastCommand>(MAX_BULLETS, Allocator.Persistent);
			raycastHits = new NativeArray<RaycastHit>(MAX_BULLETS, Allocator.Persistent);
			Unused = new Stack<int>(Enumerable.Range(0, MAX_BULLETS));
		}

		internal void AddNewBullet(Vector3 position, Quaternion rotation, Color color, IBulletBehaviour behaviour)
		{
			var index = Unused.Pop();
			bullets[index] = new Bullet(position, rotation, Vector3.zero, new Vector4(color.r, color.g, color.b, color.a), behaviour);
			//colors[index] = new Vector4(color.r, color.g, color.b, color.a);
		}

		//JobHandle jobHandle;
		internal void UpdateBullets()
		{
			//if (!jobHandle.IsCompleted)
			//	jobHandle.Complete();
			SpherecastCommand.ScheduleBatch(raycastCommands, raycastHits, 20).Complete();

			var camDir = Camera.main.transform.forward.normalized;

			for (int i = 0; i < bullets.Length; i++)
			{

				if (raycastHits[i].collider != null)
				{
					var delete = false;
					foreach(var target in raycastHits[i].collider.GetComponentsInChildren<IBulletCollider>())
					{
						target.Collide(bullets[i]);
						delete |= target.DeleteBullet;
					}
					if (delete)
						bullets[i].Destroy();
				}

				matricies[i] = bullets[i].Active ? Matrix4x4.TRS(bullets[i].position, bullets[i].rotation, model.scale) : Matrix4x4.zero;

				bullets[i].Update();
				colors[i] = bullets[i].color;

				if(!bullets[i].Active && bullets[i].Used)
				{
					bullets[i].Used = false;
					Unused.Push(i);

					if (model.vanishEffect)
						Danmaku.Instance.AddBullet(DanmakuSettings.Instance.vanishEffect, bullets[i].position, Quaternion.identity, bullets[i].color, DanmakuSettings.Instance.vanishBulletBehaviour);
				}

				raycastCommands[i] = bullets[i].Used ? new SpherecastCommand(bullets[i].position - camDir, model.radius, camDir, 2f) : new SpherecastCommand();

				//jobHandle = SpherecastCommand.ScheduleBatch(raycastCommands, raycastHits, 20);
			}
		}

		MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
		internal void Render()
		{
			propertyBlock.Clear();
			propertyBlock.SetVectorArray("_Color", colors);
			propertyBlock.SetTexture("_MainTex", model.texture);
			Graphics.DrawMeshInstanced(model.mesh, 0, model.material, matricies, MAX_BULLETS, propertyBlock);
		}

		internal void DrawGizmos()
		{
			Gizmos.color = Color.green;
			foreach (var bul in bullets)
				if (bul.Active)
					Gizmos.DrawWireSphere(bul.position, model.radius);
		}

		public void Dispose()
		{
			raycastCommands.Dispose();
			raycastHits.Dispose();
		}
	}

	[System.Serializable]
	class DefaultBehaviour : IBulletBehaviour
	{
		[SerializeField]
		float speed = 1f;
		[SerializeField]
		int lifeTime = 600;

		public bool VanishEffect => true;

		public void UpdateBullet(ref Bullet bullet)
		{
			if (bullet.CurrentFrame == 0)
				bullet.velocity = bullet.rotation * Vector3.forward * speed;
			if (bullet.CurrentFrame > lifeTime)
				bullet.Destroy();
		}
	}

	[System.Serializable]
	class VanishEffectBehaviour : IBulletBehaviour
	{
		[SerializeField]
		int lifeTime = 30;

		public bool VanishEffect => false;

		public void UpdateBullet(ref Bullet bullet)
		{
			var col = bullet.color;
			col.w = 0.99f - (float)bullet.CurrentFrame / lifeTime;
			bullet.color = col;
			if (bullet.CurrentFrame > lifeTime)
				bullet.Destroy();
		}
	}

	public class Danmaku : System.IDisposable
	{
		static Danmaku _instance = null;
		public static Danmaku Instance => _instance ?? (_instance = new Danmaku());

		Dictionary<BulletModel, List<BulletGroup>> groups = new Dictionary<BulletModel, List<BulletGroup>>();

		public void AddBullet(BulletModel model, Vector3 position, Quaternion rotation, Color color, IBulletBehaviour behaviour)
		{
			if (!groups.ContainsKey(model))
				groups.Add(model, new List<BulletGroup>());

			BulletGroup group = null;
			foreach (var g in groups[model])
				if (g.HasEmpty) group = g;
			if (group == null)
			{
				group = new BulletGroup(model);
				groups[model].Add(group);
			}

			group.AddNewBullet(position, rotation, color, behaviour);
		}

		public void Update()
		{
			for(int i= 0;i < groups.Count;i++)
			{
				foreach (var g in groups.ElementAt(i).Value)
					g.UpdateBullets();
			}
		}

		public void Render()
		{
			foreach (var group in groups)
			{
				foreach (var g in group.Value)
					g.Render();
			}
		}

		internal void DrawGizmos()
		{
			foreach (var group in groups)
			{
				foreach (var g in group.Value)
					g.DrawGizmos();
			}
		}

		public void Dispose()
		{
			foreach (var group in groups)
			{
				foreach (var g in group.Value)
					g.Dispose();
				group.Value.Clear();
			}
			groups.Clear();
		}
	}
}
