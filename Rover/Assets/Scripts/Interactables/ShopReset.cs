using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopReset : Interactable
{
    public Transform itemContainer;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public Vector3 distance = new Vector3(-15, 0, 0);
    public float speed = 0.4f;

    private void Start()
    {
        startPosition = itemContainer.position;
        endPosition = itemContainer.position + distance;
    }

    void Update()
    {
        //itemContainer.position = Vector3.Lerp(startPosition, endPosition, 0.5f * Time.deltaTime);
    }

    public override void Interact()
    {
        StartCoroutine(SmoothLerp());
    }

    private IEnumerator SmoothLerp()
    {
        float time = 1;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            itemContainer.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GroundItem[] item = itemContainer.gameObject.GetComponentsInChildren<GroundItem>();
        for (int i = 0; i < item.Length; i++)
        {
            Destroy(item[i].gameObject);
        }

        ShopSlot[] shopSlots = itemContainer.gameObject.GetComponentsInChildren<ShopSlot>();//'restart' script
        for (int i = 0; i < shopSlots.Length; i++)
        {
            shopSlots[i].activated = false;
            shopSlots[i].GenerateItem();
            shopSlots[i].DisplayItem();
        }

        time = 1;
        elapsedTime = 0;

        while (elapsedTime < time)
        {
            itemContainer.position = Vector3.Lerp(endPosition, startPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
