using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnStart : MonoBehaviour
{
    public bool m_UseSceneIndex = true;

    public int m_SceneIndex = 1;
    public string m_SceneName = "NetStart";
    
    public LoadSceneMode m_LoadSceneMode = LoadSceneMode.Single;
    
    void Start()
    {
        if (m_UseSceneIndex)
        {
            SceneManager.LoadScene(m_SceneIndex, m_LoadSceneMode);
        }
        else
        {
            SceneManager.LoadScene(m_SceneName, m_LoadSceneMode);
        }
    }
}
