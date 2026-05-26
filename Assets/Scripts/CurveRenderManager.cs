using System;
using UnityEngine;
using System.Collections.Generic;

public class CurveRenderManager : MonoBehaviour
{
    public static int CurveSampleSize = 48;
    
    public AnimationData Data; // just prototyping stuff for now
    public GameObject RendererPrefab;
    
    private Dictionary<CurveID, GameObject> renderers = new Dictionary<CurveID, GameObject>();

    void Start()
    {
        foreach (var value in Enum.GetValues(typeof(CurveID)))
        {
            var obj = Instantiate(RendererPrefab, transform);
            obj.name = value.ToString();
            //obj.GetComponent<CurveRenderer>().Initialize(CurveSampleSize);
            renderers.Add((CurveID)value, obj);
            
            Debug.Log("Added " + value + " to renderers");
        }

        foreach (var curve in Data.Frames[0].Persistent)
        {
            renderers[curve.ID].GetComponent<CurveRenderer>().CurrentCurve = curve;
        }
    }
}
