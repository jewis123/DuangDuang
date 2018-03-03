using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
///处理背景色
/// </summary>
public class ColorManager : MonoBehaviour 
{
	
	/// <summary>
	/// The sprite.
	/// </summary>
	public SpriteRenderer sprite;
	/// <summary>
	/// The user interface.
	/// </summary>

	
	/// <summary>
	/// The color of the time change.
	/// </summary>
	public float timeChangeColor = 10;

	void OnEnable()
	{
		StartCoroutine(ChangeColor ());
	}

	void OnDisable(){
		StopAllCoroutines ();
	} 

	//当前色
	/// <summary>
	/// The color.
	/// </summary>
	public Color color;

	
	IEnumerator ChangeColor(){

		while (true) {

			Color colorTemp =  Utils.GetRandomColor();

			StartCoroutine(DoLerp(sprite.color, colorTemp, 1f));

			yield return new WaitForSeconds (timeChangeColor);
		}

	}

	
	/// <summary>
	/// Dos the lerp.
	/// </summary>
	/// <returns>The lerp.</returns>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	/// <param name="time">Time.</param>
	public IEnumerator DoLerp(Color from, Color to, float time)
	{
		float timer = 0;
		while (timer <= time)
		{
			timer += Time.deltaTime;
			sprite.color = Color.Lerp(from, to, timer / time);
			yield return null;
		}
		sprite.color = to;
	}



}
