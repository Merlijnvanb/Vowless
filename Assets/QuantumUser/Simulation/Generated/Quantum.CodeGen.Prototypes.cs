// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
//
// If you need to extend the classes generated to add
// fields or methods to them, please create partial
// declarations in another file.
// </auto-generated>
#pragma warning disable 0109
#pragma warning disable 1591


namespace Quantum.Prototypes {
  using Photon.Deterministic;
  using Quantum;
  using Quantum.Core;
  using Quantum.Collections;
  using Quantum.Inspector;
  using Quantum.Physics2D;
  using Quantum.Physics3D;
  using Byte = System.Byte;
  using SByte = System.SByte;
  using Int16 = System.Int16;
  using UInt16 = System.UInt16;
  using Int32 = System.Int32;
  using UInt32 = System.UInt32;
  using Int64 = System.Int64;
  using UInt64 = System.UInt64;
  using Boolean = System.Boolean;
  using String = System.String;
  using Object = System.Object;
  using FlagsAttribute = System.FlagsAttribute;
  using SerializableAttribute = System.SerializableAttribute;
  using MethodImplAttribute = System.Runtime.CompilerServices.MethodImplAttribute;
  using MethodImplOptions = System.Runtime.CompilerServices.MethodImplOptions;
  using FieldOffsetAttribute = System.Runtime.InteropServices.FieldOffsetAttribute;
  using StructLayoutAttribute = System.Runtime.InteropServices.StructLayoutAttribute;
  using LayoutKind = System.Runtime.InteropServices.LayoutKind;
  #if QUANTUM_UNITY //;
  using TooltipAttribute = UnityEngine.TooltipAttribute;
  using HeaderAttribute = UnityEngine.HeaderAttribute;
  using SpaceAttribute = UnityEngine.SpaceAttribute;
  using RangeAttribute = UnityEngine.RangeAttribute;
  using HideInInspectorAttribute = UnityEngine.HideInInspector;
  using PreserveAttribute = UnityEngine.Scripting.PreserveAttribute;
  using FormerlySerializedAsAttribute = UnityEngine.Serialization.FormerlySerializedAsAttribute;
  using MovedFromAttribute = UnityEngine.Scripting.APIUpdating.MovedFromAttribute;
  using CreateAssetMenu = UnityEngine.CreateAssetMenuAttribute;
  using RuntimeInitializeOnLoadMethodAttribute = UnityEngine.RuntimeInitializeOnLoadMethodAttribute;
  #endif //;
  
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Input))]
  public unsafe partial class InputPrototype : StructPrototype {
    public FPVector2 MoveDir;
    public FPVector2 LookDir;
    public Button Block;
    public Button Attack;
    public Button Turn;
    partial void MaterializeUser(Frame frame, ref Quantum.Input result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.Input result, in PrototypeMaterializationContext context = default) {
        result.MoveDir = this.MoveDir;
        result.LookDir = this.LookDir;
        result.Block = this.Block;
        result.Attack = this.Attack;
        result.Turn = this.Turn;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.PlayerData))]
  public unsafe partial class PlayerDataPrototype : ComponentPrototype<Quantum.PlayerData> {
    public PlayerRef PlayerRef;
    [ArrayLengthAttribute(60)]
    public Quantum.Prototypes.InputPrototype[] InputHistory = new Quantum.Prototypes.InputPrototype[60];
    public Int32 InputHeadIndex;
    partial void MaterializeUser(Frame frame, ref Quantum.PlayerData result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.PlayerData component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.PlayerData result, in PrototypeMaterializationContext context = default) {
        result.PlayerRef = this.PlayerRef;
        for (int i = 0, count = PrototypeValidator.CheckLength(InputHistory, 60, in context); i < count; ++i) {
          this.InputHistory[i].Materialize(frame, ref *result.InputHistory.GetPointer(i), in context);
        }
        result.InputHeadIndex = this.InputHeadIndex;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.RoninData))]
  public unsafe partial class RoninDataPrototype : ComponentPrototype<Quantum.RoninData> {
    public AssetRef<RoninConstants> Constants;
    public FPVector2 Position;
    public Int32 FacingSign;
    public AssetRef<RoninStateBase> CurrentState;
    public Int32 StateFrame;
    partial void MaterializeUser(Frame frame, ref Quantum.RoninData result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.RoninData component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.RoninData result, in PrototypeMaterializationContext context = default) {
        result.Constants = this.Constants;
        result.Position = this.Position;
        result.FacingSign = this.FacingSign;
        result.CurrentState = this.CurrentState;
        result.StateFrame = this.StateFrame;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.SaberData))]
  public unsafe partial class SaberDataPrototype : ComponentPrototype<Quantum.SaberData> {
    public AssetRef<SaberConstants> Constants;
    public FPVector2 Direction;
    public AssetRef<SaberStateBase> CurrentState;
    public Int32 StateFrame;
    partial void MaterializeUser(Frame frame, ref Quantum.SaberData result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.SaberData component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.SaberData result, in PrototypeMaterializationContext context = default) {
        result.Constants = this.Constants;
        result.Direction = this.Direction;
        result.CurrentState = this.CurrentState;
        result.StateFrame = this.StateFrame;
        MaterializeUser(frame, ref result, in context);
    }
  }
}
#pragma warning restore 0109
#pragma warning restore 1591
