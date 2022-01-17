using System.Collections;
using System.Collections.Generic;
using InstancedDanmaku;
using UnityEngine;

public class Player : MonoBehaviour, InstancedDanmaku.IBulletCollider
{
	[SerializeField]
	float speed = 0.1f;
	[SerializeField]
	GameObject hitEffect;

	public bool DeleteBullet => true;

	public void Collide(Bullet bullet)
	{
		if (hitEffect == null) return;
		var effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
		Destroy(effect, 0.5f);
	}

	private void FixedUpdate()
	{
		transform.position +=
			new Vector3(
				Input.GetAxisRaw("Horizontal"),
				Input.GetAxisRaw("Vertical"),
				0
			) * speed;
	}
}
