using System;
using UnityEngine;
using System.Collections.Generic;

public struct RenderContainer
{
    public Dictionary<CurveID, FrameCurveContainer> Persistent;
    public List<FrameCurveContainer> Transient;
    //public List<FrameCurveContainer> Guide;
}

public class CurveRenderManager : MonoBehaviour
{
    public int CurveSampleRate = 48;
    public int TransientPoolSize = 10;

    public Transform PersistentParent;
    public Transform TransientParent;
    
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
            renderer.Disable();
            tRenderers[i] = renderer;
        }
    }

    public void RenderFrame(RenderContainer container)
    {
        if (pRenderers == null || tRenderers == null)
            return;
        
        foreach (var kvp in pRenderers)
        {
            if (container.Persistent.TryGetValue(kvp.Key, out var curve))
                kvp.Value.UpdateMesh(curve);
            else
                kvp.Value.Disable();
        }
        
        if (container.Transient.Count >= TransientPoolSize)
        {
            Debug.LogWarning("Transient pool size exceeded");
            return;
        }

        for (int i = 0; i < TransientPoolSize; i++)
        {
            if (i < container.Transient.Count)
                tRenderers[i].UpdateMesh(container.Transient[i]);
            else
                tRenderers[i].Disable();
        }
    }
}
