using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	[CreateAssetMenu(fileName = "New Bullet Model", menuName = "FriendSea/BulletModel")]
	public class BulletModel : ScriptableObject
	{
		[SerializeField]
		Mesh mesh;
		[SerializeField]
		Texture2D texture;
		[SerializeField]
		Material material;
		[SerializeField]
		int priority;
		[SerializeField]
		Vector3 scale = Vector3.one;
		[SerializeField]
		float radius = 0.1f;
		[SerializeField]
		bool vanishEffect = true;

		public Mesh Mesh => mesh;
		public Texture2D Texture => texture;
		public Vector3 Scale => scale;
		public float Radius => radius;
		public bool VanishEffect => vanishEffect;

		[System.NonSerialized]
		Material _material = null;
		public Material Material { get
			{
				if(_material == null)
				{
					_material = new Material(material);
					_material.renderQueue += priority;
				}
				return _material;
			} }
	}
}
