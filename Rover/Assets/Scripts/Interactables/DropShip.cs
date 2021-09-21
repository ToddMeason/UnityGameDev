using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DropShip : ChangeLevelInteractable
{
    [SerializeField]private float speed = 5;
    [SerializeField]private Transform target;

    private void OnEnable()
    {
        target = FindObjectOfType<MainObjective>().transform;
        StartCoroutine(Move());
    }

    public override void Interact()
    {
        InvokeBeforeSceneChange();
        SceneManager.LoadScene("ShipHub");
        InvokeOnSceneChange();
    }

    IEnumerator Move()
    {
        int count = 0;//just to make sure no infinte loop
        while (transform.position != target.position || count > 100)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
            count++;
            yield return null;
        }
        yield return null;
    }
}
