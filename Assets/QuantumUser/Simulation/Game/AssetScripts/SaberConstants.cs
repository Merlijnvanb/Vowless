namespace Quantum
{
    using Photon.Deterministic;
    using System.Collections.Generic;
    using UnityEngine;

    public class SaberConstants : AssetObject
    {
        [System.Serializable]
        public struct StateData
        {
            public Holding Holding;
            public Attacking Attacking;
        }

        public StateData States;
        
        public Dictionary<SaberDirection, FPVector2> Directions;
        [SerializeField] private SaberDirectionData[] _directions;
        
        public FP InputMagnitudeThreshold;

        public void InitData()
        {
            Directions = new Dictionary<SaberDirection, FPVector2>();

            foreach (var data in _directions)
            {
                Directions.Add(data.Id, data.Vector.Normalized);
            }
        }
    }
}