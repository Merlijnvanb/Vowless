namespace Quantum
{
    using Photon.Deterministic;

    public unsafe partial struct BoxRect
    {
        public FP XMin { get { return Position.X - WidthHeight.X / 2; } }
        public FP XMax { get { return Position.X + WidthHeight.X / 2; } }
        public FP YMin { get { return Position.Y - WidthHeight.Y / 2; } }
        public FP YMax { get { return Position.Y + WidthHeight.Y / 2; } }
        
        public bool Overlaps(BoxRect other)
        {
            var c1 = other.XMax >= XMin;
            var c2 = other.XMin <= XMax;
            var c3 = other.YMax >= YMin;
            var c4 = other.YMin <= YMax;

            return c1 && c2 && c3 && c4;
        }

        public FPVector2 GetOverlapCenter(BoxRect other)
        {
            var overlapMinX = FPMath.Max(XMin, other.XMin);
            var overlapMaxX = FPMath.Min(XMax, other.XMax);
            var overlapMinY = FPMath.Max(YMin, other.YMin);
            var overlapMaxY = FPMath.Min(YMax, other.YMax);
            
            var centerX = (overlapMinX + overlapMaxX) / 2;
            var centerY = (overlapMinY + overlapMaxY) / 2;

            return new FPVector2(centerX, centerY);
        }
    }
}
