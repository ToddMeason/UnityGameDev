using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 5.0f;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
