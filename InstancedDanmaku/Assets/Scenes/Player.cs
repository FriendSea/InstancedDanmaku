using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField]
	float speed = 0.1f;

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
