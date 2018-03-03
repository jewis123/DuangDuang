using UnityEngine;
using System.Collections;

/// <summary>
/// 绑定到障碍物上
/// 判断是否超出屏幕范围
/// </summary>
public class CheckIfOutOfScreen : MonoBehaviorHelper
{

	public Renderer m_renderer;

	float size = -1;

	void Awake()
	{
		Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;

		#if UNITY_TVOS
		size = width;
		#else
		size = height;
		#endif
	}

	void OnEnable()
	{
		StopAllCoroutines();

		LaunchCoUpdate();
	}


	void OnDisable()
	{
		StopAllCoroutines();
	}

	void LaunchCoUpdate()
	{
		if (!Application.isPlaying)
			return;

		StartCoroutine(CoUpdate());
	}

	void StopCoUpdate()
	{
		gameObject.SetActive (false);
		StopAllCoroutines();
	}
		
	/// <summary>
	/// Verify each seconds if the obstacle is out of screen.
	/// </summary>
	IEnumerator CoUpdate()
	{
		while(true)
		{


			if(IsBehind())
			{
				
				break;
			}

			yield return new WaitForSeconds(1);;
		}

	
		StopCoUpdate();
	}

	/// <summary>
	/// 检查是否脱离视域
	/// </summary>
	bool IsBehind()
	{
		if (playerTransform == null)
			return true;

		Vector3 forward = transform.TransformDirection(Vector3.up);
		Vector3 toOther = playerTransform.position - transform.position;
		if (Vector3.Dot (forward, toOther) > size / 1.8f)
			return true;

		return false;
	}

}
