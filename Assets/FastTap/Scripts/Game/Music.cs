using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//处理音乐
public class Music : MonoBehaviour {
    static Music _instance;

    public static Music instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Music>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    void Awake()
    {

        //此脚本永不消毁，并且每次进入初始场景时进行判断，若存在重复的则销毁  
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != _instance)
        {
            Destroy(gameObject);
        }

    }
}
