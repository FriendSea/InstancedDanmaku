using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
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

		public BulletModel Model { get; }

		Bullet[] bullets;
		Matrix4x4[] matricies;
		Vector4[] colors;
		NativeArray<SpherecastCommand> raycastCommands;
		NativeArray<RaycastHit> raycastHits;
		internal Stack<int> Unused { get; }

		internal bool HasEmpty => Unused.Count > 0;

		internal BulletGroup(BulletModel model)
		{
			this.Model = model;

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
		}

		List<IBulletCollider> collisionTargets = new List<IBulletCollider>();
		//JobHandle jobHandle;
		internal void UpdateBullets()
		{
			//if (!jobHandle.IsCompleted)
			//	jobHandle.Complete();
			SpherecastCommand.ScheduleBatch(raycastCommands, raycastHits, 20).Complete();

			var camDir = Camera.main.transform.forward.normalized;

			for (int i = 0; i < bullets.Length; i++)
			{
				if(raycastCommands[i].radius > 0)
				{
					if (raycastHits[i].collider != null)
					{
						var delete = false;
						raycastHits[i].collider.GetComponentsInChildren<IBulletCollider>(collisionTargets);
						if (collisionTargets.Count <= 0)
							delete = true;
						else foreach (var target in collisionTargets)
						{
							target.Collide(bullets[i]);
							delete |= target.DeleteBullet;
						}
						if (delete)
							bullets[i].Destroy();
					}
				}

				matricies[i] = bullets[i].Active ? Matrix4x4.TRS(bullets[i].position, bullets[i].rotation, Model.Scale) : Matrix4x4.zero;

				bullets[i].Update();
				colors[i] = bullets[i].color;

				if(!bullets[i].Active && bullets[i].Used)
				{
					bullets[i].Used = false;
					Unused.Push(i);

					if (Model.VanishEffect)
						Danmaku.Instance.AddBullet(DanmakuSettings.Instance.vanishEffect, bullets[i].position, Quaternion.identity, bullets[i].color, DanmakuSettings.Instance.vanishBulletBehaviour);
				}

				raycastCommands[i] = bullets[i].Used ? new SpherecastCommand(bullets[i].position - camDir, Model.Radius, camDir, 2f, DanmakuSettings.Instance.collisionMask) : new SpherecastCommand();

				//jobHandle = SpherecastCommand.ScheduleBatch(raycastCommands, raycastHits, 20);
			}
		}

		MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
		internal void Render()
		{
			propertyBlock.Clear();
			propertyBlock.SetVectorArray("_Color", colors);
			propertyBlock.SetTexture("_MainTex", Model.Texture);
			Graphics.DrawMeshInstanced(Model.Mesh, 0, Model.Material, matricies, MAX_BULLETS, propertyBlock);
		}

		internal void DrawGizmos()
		{
			Gizmos.color = Color.green;
			foreach (var bul in bullets)
				if (bul.Active)
					Gizmos.DrawWireSphere(bul.position, Model.Radius);
		}

		public void Dispose()
		{
			raycastCommands.Dispose();
			raycastHits.Dispose();
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

		List<BulletGroup> groups = new List<BulletGroup>();

		public void AddBullet(BulletModel model, Vector3 position, Quaternion rotation, Color color, IBulletBehaviour behaviour)
		{
			BulletGroup group = null;
			foreach(var g in groups)
			{
				if (g.Model != model) continue;
				if (!g.HasEmpty) continue;
				group = g;
				break;
			}
			if(group == null)
			{
				group = new BulletGroup(model);
				groups.Add(group);
			}

			group.AddNewBullet(position, rotation, color, behaviour);
		}

		public void Update()
		{
			for (int i = 0; i < groups.Count; i++)
				groups[i].UpdateBullets();
		}

		public void Render()
		{
			foreach (var g in groups)
				g.Render();
		}

		internal void DrawGizmos()
		{
			foreach (var g in groups)
				g.DrawGizmos();
		}

		public void Dispose()
		{
			foreach (var group in groups)
				group.Dispose();
			groups.Clear();
		}

		public static void Reset()
		{
			Instance.Dispose();
			_instance = null;
		}
	}
}
