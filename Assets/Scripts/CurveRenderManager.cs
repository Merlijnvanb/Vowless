using System;
using UnityEngine;
using System.Collections.Generic;

public class CurveRenderManager : MonoBehaviour
{
    public int CurveSampleRate = 48;
    public int TransientPoolSize = 10;

    public Transform PersistentParent;
    public Transform TransientParent;
    
    public AnimationData Data; // just prototyping stuff for now
    public GameObject RendererPrefab;
    
    private Dictionary<CurveID, CurveRenderer> pRenderers;
    private CurveRenderer[] tRenderers;

    void Start()
    {
        pRenderers = new Dictionary<CurveID, CurveRenderer>();
        tRenderers = new CurveRenderer[TransientPoolSize];
        
        foreach (var value in Enum.GetValues(typeof(CurveID)))
        {
            var obj = Instantiate(RendererPrefab, PersistentParent);
            obj.name = value.ToString();
            
            var renderer = obj.GetComponent<CurveRenderer>();
            renderer.Initialize(CurveSampleRate);
            pRenderers.Add((CurveID)value, renderer);
            
            Debug.Log("Added " + value + " to renderers");
        }

        for (int i = 0; i < TransientPoolSize; i++)
        {
            var obj = Instantiate(RendererPrefab, TransientParent);
            obj.name = "transient[" + i + "]";
            
            var renderer = obj.GetComponent<CurveRenderer>();
            renderer.Initialize(CurveSampleRate);
            tRenderers[i] = renderer;
            
            obj.SetActive(false);
        }

        foreach (var curve in Data.Frames[0].Persistent)
        {
            pRenderers[curve.ID].GetComponent<CurveRenderer>().UpdateMesh(curve);
        }
    }
}
