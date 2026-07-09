using System;
using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class AmbientForce : MonoBehaviour
{
    [System.Serializable]
    public struct Vector3x3
    {
        public Vector3 X;
        public Vector3 Y;
        public Vector3 Z;
    }

    public enum CurlMethod
    {
        FiniteDifference,
        CrossProduct
    }
    
    [TitleGroup("Spiral Sink")] public Transform FocalPoint;
    [TitleGroup("Spiral Sink")] public Vector3 SpiralAxis;
    [TitleGroup("Spiral Sink")] public float VortexStrength;
    [TitleGroup("Spiral Sink")] public float SinkStrength;
    [TitleGroup("Spiral Sink")] public float MinRadius;
    [TitleGroup("Spiral Sink")] public float SpiralScalar = 1f;

    [TitleGroup("Curl")] public CurlMethod UsedCurlMethod;
    [TitleGroup("Curl")] public float CurlScalar = 1f;
    [TitleGroup("Curl")] public float NoiseScale = 1f;
    [TitleGroup("Curl")] public float Epsilon = 0.001f;

    [TitleGroup("Curl/Potential")] public Vector3x3 PotentialOffsets;
    [TitleGroup("Curl/Potential")] public float PotentialOffsetScalar = 1f;

    [TitleGroup("Curl/Scroll")] public float ScrollSpeed = 1f;
    [TitleGroup("Curl/Scroll")] public Vector3 ScrollDirection;

    [TitleGroup("Debug")]
    [TitleGroup("Debug/Particle")] public bool DebugParticleEnabled;
    [TitleGroup("Debug/Particle")] public float DebugParticleLifeTime;
    [TitleGroup("Debug/Particle")] public float DebugParticleStepSize;
    [TitleGroup("Debug/Particle"), Button("Spawn Debug Particle")]
    public void SpawnParticle() => SpawnDebugParticle();

    [TitleGroup("Debug/Strand")] public bool DebugStrandEnabled;
    [TitleGroup("Debug/Strand")] public int DebugStrandIterations;
    [TitleGroup("Debug/Strand")] public float DebugStrandStepSize;

    [TitleGroup("Debug/Plane")] public bool DebugPlaneEnabled;
    [TitleGroup("Debug/Plane")] public Vector2 DebugPlaneSize;
    [TitleGroup("Debug/Plane")] public int2 DebugPlaneResolution;

    private float debugParticleLife;
    private Vector3 debugParticlePosition;
    private List<Vector3> debugParticlePosList;


    void OnDrawGizmos()
    {
        if (DebugParticleEnabled && debugParticlePosList != null)
        {
            if (debugParticleLife > 0)
            {
                var ambientVector = SampleAmbient(debugParticlePosition);
                var delta = ambientVector * DebugParticleStepSize;
                debugParticlePosition += delta;
                debugParticlePosList.Add(debugParticlePosition);

                debugParticleLife -= Time.deltaTime;
            }
            
            for (int i = 0; i < debugParticlePosList.Count; i++)
            {
                Gizmos.DrawSphere(debugParticlePosList[i], 0.01f);

                if (i >= debugParticlePosList.Count - 1) 
                    break;
                    
                Gizmos.DrawLine(debugParticlePosList[i], debugParticlePosList[i + 1]);
            }
        }
        
        if (DebugStrandEnabled)
        {
            var strand = transform.position;
            Gizmos.DrawSphere(strand, 0.01f);
            var ambientVector = SampleAmbient(strand);
            Gizmos.DrawLine(strand, strand + ambientVector);

            for (int i = 0; i < DebugStrandIterations; i++)
            {
                strand += ambientVector * DebugStrandStepSize;
                
                Gizmos.DrawSphere(strand, 0.01f);
                ambientVector = SampleAmbient(strand);
                Gizmos.DrawLine(strand, strand + ambientVector);
            }
        }
        
        if (DebugPlaneEnabled)
        {
            var planeCenter = transform.position;
            var planeRight = transform.right;
            var planeUp = transform.up;

            for (int i = 0; i < DebugPlaneResolution.x; i++)
            {
                for (int j = 0; j < DebugPlaneResolution.y; j++)
                {
                    var vector = planeCenter;
                    vector += planeRight * ((float)i / (DebugPlaneResolution.x - 1) * DebugPlaneSize.x -
                                            DebugPlaneSize.x / 2);
                    vector += planeUp * ((float)j / (DebugPlaneResolution.y - 1) * DebugPlaneSize.y -
                                         DebugPlaneSize.y / 2);

                    Gizmos.DrawSphere(vector, 0.01f);

                    var ambientVector = SampleAmbient(vector);
                    Gizmos.DrawLine(vector, vector + ambientVector);
                }
            }
        }
    }

    private void SpawnDebugParticle()
    {
        if (debugParticlePosList == null)
            debugParticlePosList = new List<Vector3>();

        debugParticlePosList.Clear();
        debugParticleLife = DebugParticleLifeTime;
        debugParticlePosition = transform.position;
        DebugParticleEnabled = true;
    }
    
    public Vector3 SampleAmbient(Vector3 point)
    {
        var scrollVector = ScrollDirection.normalized * (ScrollSpeed * Time.time);
        var pScrolled = point + scrollVector;
        
        return SampleSpiral(point) * SpiralScalar + SampleCurl(pScrolled) * CurlScalar;
    }

    private Vector3 SampleSpiral(Vector3 point)
    {
        var axis = SpiralAxis.normalized;
        
        var offset = point - FocalPoint.position;
        var radialOffset = offset - Vector3.Dot(offset, axis) * axis;
        var radius = Mathf.Max(radialOffset.magnitude, MinRadius);
        var radialDir = radialOffset / radius;
        var tangentialDir = Vector3.Cross(axis, radialDir);
        var distance = Mathf.Max(offset.magnitude, MinRadius);
        
        var sinkVel = (-SinkStrength / distance) * (offset / distance);
        var vortexVel = (VortexStrength / radius) * tangentialDir;
        
        return (sinkVel + vortexVel);
    }

    private Vector3 SampleCurl(Vector3 point)
    {
        var curl = Vector3.zero;
        var pScaled = point * NoiseScale;
        
        switch (UsedCurlMethod)
        {
            case CurlMethod.FiniteDifference:
                
                var dNdx = GetPotential(pScaled + new Vector3(Epsilon, 0f, 0f)) - GetPotential(pScaled - new Vector3(Epsilon, 0f, 0f));
                var dNdy = GetPotential(pScaled + new Vector3(0f, Epsilon, 0f)) - GetPotential(pScaled - new Vector3(0f, Epsilon, 0f));
                var dNdz = GetPotential(pScaled + new Vector3(0f, 0f, Epsilon)) - GetPotential(pScaled - new Vector3(0f, 0f, Epsilon));

                curl.x = dNdy.z - dNdz.y;
                curl.y = dNdz.x - dNdx.z;
                curl.z = dNdx.y - dNdy.x;
                curl *= 300f;
                
                break;
            
            case CurlMethod.CrossProduct:
                noise.snoise(pScaled + PotentialOffsets.X * PotentialOffsetScalar, out float3 gradF);
                noise.snoise(pScaled + PotentialOffsets.Y * PotentialOffsetScalar, out float3 gradG);

                curl = Vector3.Cross(gradF, gradG).normalized;
                
                break;
        }

        return curl;
    }

    private Vector3 GetPotential(Vector3 point)
    {
        var potential = new Vector3();

        potential.x = noise.snoise(point + PotentialOffsets.X * PotentialOffsetScalar);
        potential.y = noise.snoise(point + PotentialOffsets.Y * PotentialOffsetScalar);
        potential.z = noise.snoise(point + PotentialOffsets.Z * PotentialOffsetScalar);
        
        return potential;
    }
}
