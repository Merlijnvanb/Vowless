using System;
using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;

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

    [Header("DebugStrand")] 
    public bool DebugStrandEnabled;
    public int DebugStrandIterations;
    public float DebugStrandStepSize;

    [Header("DebugPlane")] 
    public bool DebugPlaneEnabled;
    public Vector2 DebugPlaneSize;
    public int2 DebugPlaneResolution;

    [Header("Timing")] 
    public float ScrollSpeed = 1f;
    public Vector3 ScrollDirection;

    [Header("Scaling")] 
    public float SpiralScalar = 1f;
    public float CurlScalar = 1f;

    [Header("Curl")] 
    public CurlMethod UsedCurlMethod;
    public float Epsilon = 0.001f;
    public float NoiseScale = 1f;

    [Header("Potential")] 
    public Vector3x3 PotentialOffsets;
    public float PotentialOffsetScalar = 1f;

    [Header("Spiral Sink")] 
    public Transform FocalPoint;
    public Vector3 SpiralAxis;
    public float VortexStrength;
    public float SinkStrength;
    public float MinRadius;
    

    void OnDrawGizmos()
    {
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
