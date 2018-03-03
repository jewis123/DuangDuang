using UnityEngine;
using System.Collections;

//TVOS平台适配
public class DesactivateForSomePlatforms : MonoBehaviour 
{
	void Awake()
	{
		#if UNITY_TVOS
		gameObject.SetActive(false);
		#else
		#endif
	}
}
