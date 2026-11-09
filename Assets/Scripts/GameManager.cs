using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    AsyncOperation preload; //声明预加载
    bool isOver = false;  //设定游戏结束的初始值为否
    public GameObject clearMes;  //游戏通关消息
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(NextScenePreload()); //进入场景开始预加载下一关
    }

    // Update is called once per frame
    void Update()
    {
        if(clearMes.activeSelf)  //如果通关消息被激活
        {
            if(Input.GetKey(KeyCode.Escape)) //且玩家输入esc
            {
                Application.Quit(); //退出游戏
            }
            else if(Input.GetKey(KeyCode.R))  //或输入R
            {
                SceneManager.LoadScene(0);  //重新从第0个场景开始
            }
        }
    }

    public IEnumerator NextScenePreload()  //预加载
    {
        yield return null;
        int sceneID = SceneManager.GetActiveScene().buildIndex+1; //得到现场景编号
        if(sceneID<SceneManager.sceneCountInBuildSettings)  //如果现场景编号小于总场景数，则加载下一场景且暂不触发切换场景
        {
            preload = SceneManager.LoadSceneAsync(sceneID);
            preload.allowSceneActivation = false;
        }
        else                                                //否则，场景切换结束
        {
            isOver = true;
        }
    }

    public IEnumerator LoadNextScene()  //切换下一场景
    {
        if(!isOver)                     //如果场景切换没有结束，则等待2秒后切换
        {
            yield return new WaitForSeconds(2);
            preload.allowSceneActivation = true;
        }
        else                            //否则，激活通关消息
        {
            clearMes.SetActive(true);
        }
    }

    public IEnumerator RestartScene() //重启场景
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
