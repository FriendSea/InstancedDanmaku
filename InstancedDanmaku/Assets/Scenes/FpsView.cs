using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsView : MonoBehaviour
{
	[SerializeField]
	TMPro.TextMeshProUGUI text;

	float currentTime = 0;
	int currentFrame = 0;
	int currentFixedFrame = 0;

	private void Update()
	{
		currentTime += Time.deltaTime;
		currentFrame++;
		if (currentTime >= 1f)
		{
			text.SetText("{0} : {1}", currentFrame, currentFixedFrame);
			currentTime = 0;
			currentFrame = 0;
			currentFixedFrame = 0;
		}
	}

	private void FixedUpdate()
	{
		currentFixedFrame++;
	}
}
