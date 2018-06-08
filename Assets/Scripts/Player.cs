using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string Name;
    public Color Color;

    void Start()
    {
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material.color = Color;
        }
    }

    void Update()
    {

    }

    void OnMouseDown()
    {

    }
}
