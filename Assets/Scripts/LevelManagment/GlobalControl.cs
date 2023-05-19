using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For tracking the levels complete and timePassed across scenes 
/// </summary>

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;
    public int levelsCleared;
    public float timePassed;
    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

    }
}