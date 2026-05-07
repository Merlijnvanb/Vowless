using UnityEngine;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;

public class BodyMaterialUpdater : MonoBehaviour
{
    [SerializeField] private Material baseMaterial;

    [SerializeField] private List<MeshRenderer> renderers;

    [SerializeField] private Transform BladeStart;
    [SerializeField] private Transform BladeEnd;

    private Material mat;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mat = Instantiate(baseMaterial);
        
        foreach (var renderer in renderers)
        {
            renderer.sharedMaterial = mat;
        }
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetVector("_BladeStart", BladeStart.position);
        mat.SetVector("_BladeEnd", BladeEnd.position);
    }
}
