using UnityEngine;
using System.Collections;

/// <summary>
///挂载于"AmbiantLightBackground" 使图片适应屏幕尺寸
/// </summary>
public class ResizeBackgroundSize : MonoBehaviour 
{
	void Start()
	{
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		if (sr == null) return;

		transform.localScale = new Vector3(1,1,1);

		float width = sr.sprite.bounds.size.x;
		float height = sr.sprite.bounds.size.y;

		var worldScreenHeight = Camera.main.orthographicSize * 2.0f;
		var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

		transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);
	}
}
