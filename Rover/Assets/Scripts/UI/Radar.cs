using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarObject 
{
    public Image icon {get; set;}
    public GameObject owner {get; set;}
}

public class Radar : MonoBehaviour
{
    #region Variables
    public float arrowOffset;//need to set base on arrow icon used and + 45 for the camera offset for the player
    public Transform playerPos;
    Transform sweepTransform;
    float rotationSpeed;
    float mapScale = 2f;
    [SerializeField] Transform target;
    [SerializeField] GameObject arrow;

    public static List<RadarObject> radObjects = new List<RadarObject>();

    #endregion

    #region Builtin Methods

    private void Awake() {
        sweepTransform = transform.Find("Sweep");
        rotationSpeed = 180f;
        playerPos = FindObjectOfType<Player>().transform;
        target = FindObjectOfType<MainObjective>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        sweepTransform.eulerAngles -= new Vector3(0, 0, rotationSpeed * Time.deltaTime);
        DrawRadarDots();
        LookAtObj();
    }

    #endregion

    #region Custom Methods

    void LookAtObj()
    {
        Vector3 dir = target.transform.InverseTransformDirection(target.position - playerPos.transform.position);
        float angle = Mathf.Atan2(-dir.x, dir.z) * Mathf.Rad2Deg + arrowOffset;
        arrow.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    void DrawRadarDots() 
    {
        foreach(RadarObject ro in radObjects) 
        {
            Vector3 radarPos = (ro.owner.transform.position - playerPos.position);
            float distToObject = Vector3.Distance(playerPos.position, ro.owner.transform.position) * mapScale;
            float deltaY = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg + 45; //put  - playerPos.eulerAngles.y at the end to track player rotation
            radarPos.x = distToObject * Mathf.Cos(deltaY * Mathf.Deg2Rad) * -1;
            radarPos.z = distToObject * Mathf.Sin(deltaY * Mathf.Deg2Rad);

            ro.icon.transform.SetParent(this.transform);
            ro.icon.transform.position = new Vector3(radarPos.x, radarPos.z, 0) + this.transform.position;
        }
    }

    public static void RegisterRadarObject(GameObject o, Image i) 
    {
        Image image = Instantiate(i);
        radObjects.Add(new RadarObject() {owner = o, icon = image}); 
    }

    public static void RemoveRadarObject(GameObject o) 
    {
        List<RadarObject> newList = new List<RadarObject>();
        for (int i = 0; i < radObjects.Count; i++) 
        {
            if (radObjects[i].owner == o) 
            {
                Destroy(radObjects[i].icon);
                continue;
            }
            else 
            {
                newList.Add(radObjects[i]);
            }
        } 

        radObjects.RemoveRange(0, radObjects.Count);
        radObjects.AddRange(newList);
    }

    #endregion
}
