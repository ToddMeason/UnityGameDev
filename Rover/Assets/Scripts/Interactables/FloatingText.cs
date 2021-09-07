using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class FloatingText : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
