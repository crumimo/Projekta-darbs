using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaytestStuff : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadSceneAsync("SilverForest", LoadSceneMode.Additive);
    }
}
