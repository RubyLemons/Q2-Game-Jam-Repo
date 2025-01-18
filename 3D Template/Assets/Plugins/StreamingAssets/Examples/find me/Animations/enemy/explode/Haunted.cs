using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haunted : MonoBehaviour
{
    [Tooltip("Time to live (ms)")] [SerializeField] float ttl = 2500;

    void Start()
    {
        StartCoroutine(Tks.SetTimeout(() => Destroy(gameObject), ttl));
    }
}
