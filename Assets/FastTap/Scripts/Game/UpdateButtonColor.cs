using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// 处理开始游戏事件和更改开始按钮的图片底色
/// This script is attached to CanvasStart/StartButotn GameObject.
/// </summary>
public class UpdateButtonColor : MonoBehaviorHelper {

	public Image image;

	void OnEnable()
	{
		StartCoroutine (ChangeColor());
	}

	IEnumerator ChangeColor(){
		while (true) {
			Color32 c = colorManager.sprite.color;

			image.color = new Color32(c.r,c.g,c.b,c.a);

			yield return 0;
		}
	}



}
