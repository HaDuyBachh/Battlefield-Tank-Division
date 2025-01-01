using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialControl : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FindAnyObjectByType<SceneControl>().LoadDashboardAfter();
        }    
    }
}
