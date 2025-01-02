using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public string sceneToLoad = "";
    public float timeToLoadScene = 0.5f;
    public bool isLoading = false;
    public void LoadDashboardAfter() => sceneToLoad = "Dashboard";
    public void LoadRegisterAfter() => sceneToLoad = "Register";
    public void LoadSignInAfter() => sceneToLoad = "SignIn";
    public void LoadTutorialAfter(float time)
    {
        sceneToLoad = "Tutorial";
        timeToLoadScene = time;
    }

    Coroutine coroutine;
    
    public void Update()
    {
        if (sceneToLoad.Length > 0 && !isLoading)
        {
            coroutine = StartCoroutine(StartAfterDelay(timeToLoadScene));
        }
    }

    // Coroutine để khởi động sau một thời gian chờ
    IEnumerator StartAfterDelay(float delay)
    {
        isLoading = true;

        // Đợi trong khoảng thời gian delay
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneToLoad);
        sceneToLoad = "";
        timeToLoadScene = 0;

        isLoading = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
