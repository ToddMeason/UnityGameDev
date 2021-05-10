using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    #region Variables
    private float lifeTime = 5;

    private Material mat;
    private Color originalColor;
    private float fadePercent;
    private float deathTime;
    private bool fading;
    #endregion

    #region Builtin Methods
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        originalColor = mat.color;
        deathTime = Time.time + lifeTime;

        StartCoroutine("Fade");
    }

    #endregion

    #region Coroutines
    IEnumerator Fade()
    {
        while (true)
        {
            yield return new WaitForSeconds(.2f);

            if (fading)
            {
                fadePercent += Time.deltaTime;
                mat.color = Color.Lerp(originalColor, Color.clear, fadePercent);

                if (fadePercent >= 1)
                {
                    Destroy(gameObject);
                }
            }
            else {
                if(Time.time > deathTime)
                {
                    fading = true;
                }
            }
        }
    }

    void OnTriggerEnter(Collider c)//Detecting ground collision
    {
        if(c.tag == "Ground")
        {
            GetComponent<Rigidbody>().Sleep();
        }
    }
    #endregion
}
