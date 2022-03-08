using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [HideInInspector]
    public bool hasAlreadyPlayed;
    [HideInInspector]
    public int highScore;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void load(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }
}
