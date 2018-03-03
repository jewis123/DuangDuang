using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadLock : MonoBehaviour {
	void Start () {
        //创建Lock预制体
        GameObject locks = (GameObject)Instantiate(Resources.Load("Lock"), transform);

        //给Lock预制体下的所有按钮添加点击事件
        for (int i = 0; i < locks.transform.childCount; i++)
        {
            //如果子物体所在index具有解锁标志
            if (PlayerPrefs.HasKey(i.ToString()))
            {
                locks.transform.GetChild(i).gameObject.SetActive(false);
            }
            locks.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(Camera.main.transform.GetComponent<UIManger>().UnLock);
        }
    }
}
