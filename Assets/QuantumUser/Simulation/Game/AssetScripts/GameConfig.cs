namespace Quantum
{
    using Photon.Deterministic;

    public class GameConfig : AssetObject // Immutable global data
    {
        public AssetRef<EntityPrototype> BaseRonin;
        
        public AssetRef<RoninConstants> BaseRoninConstants;
        public AssetRef<RoninStateBase> StartingRoninState;
        
        public AssetRef<SaberConstants> BaseSaberConstants;
        public AssetRef<SaberStateBase> StartingSaberState;
        
        public FP StageWidth = 10;
        public FP MaxDistance = 5;
        public FP StartDistance = 2;

        public int PreBattleTimeFrames = 180;
        public int BattleTimeFrames = 0;
        public int PostBattleTimeFrames = 300;
    }
}
