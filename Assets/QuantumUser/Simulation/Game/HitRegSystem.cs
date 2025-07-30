namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class HitRegSystem : SystemMainThread
    {
        public override void Update(Frame frame)
        {
            var e1 = frame.Global->Ronin1;
            var e2 = frame.Global->Ronin2;
            
            // try a context approach

            var c1 = SetupContext(frame, e1);
            var c2 = SetupContext(frame, e2);

            var c1Result = GetCombatResult(frame, c1, c2);
            var c2Result = GetCombatResult(frame, c2, c1);
            
            ParseCombatResult(frame, c1Result);
            ParseCombatResult(frame, c2Result);
        }

        private void ParseCombatResult(Frame frame, CombatResult result)
        {
            switch (result.Type)
            {
                case CombatResultType.Hit:
                    frame.Signals.OnHit(result.Attacker, result.AttackState);
                    frame.Signals.OnReceivedHit(result.Defender, result.AttackState);
                    break;
                
                case CombatResultType.Deflected:
                    frame.Signals.OnDeflected(result.Attacker, result.AttackState);
                    frame.Signals.OnReceivedDeflected(result.Defender, result.AttackState);
                    break;
                
                case CombatResultType.Clashed:
                    frame.Signals.OnClashed(result.Attacker, result.AttackState);
                    break;
            }
        }

        private CombatContext SetupContext(Frame frame, EntityRef entity)
        {
            var context = new CombatContext()
            {
                Entity = entity,
            };

            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);

            var roninState = frame.FindAsset(ronin->CurrentState);
            context.IsAttacking = roninState is AttackStateBase;
            if (context.IsAttacking)
            {
                var attackState = roninState as AttackStateBase;
                context.IsAttackActive = attackState.IsActive(frame, entity);
                context.AttackState = attackState;
            }
            
            var saberState = frame.FindAsset(saber->CurrentState);
            context.IsDeflecting = saberState is Holding;
            if (context.IsDeflecting)
            {
                context.SaberState = saberState;
            }

            return context;
        }

        private CombatResult GetCombatResult(Frame frame, CombatContext cAttacker, CombatContext cDefender)
        {
            var result = new CombatResult()
            {
                Type = CombatResultType.Whiff,
                Attacker = cAttacker.Entity,
                Defender = cDefender.Entity
            };

            var attRonin = frame.Unsafe.GetPointer<RoninData>(cAttacker.Entity);
            var defRonin = frame.Unsafe.GetPointer<RoninData>(cDefender.Entity);

            if (!cAttacker.IsAttacking)
                return result;

            if (attRonin->HasHit)
                return result;
            
            var attAttackState = frame.FindAsset(cAttacker.AttackState);

            if (!cAttacker.IsAttackActive)
                return result;

            if (!IsInRange(frame, attRonin, defRonin, attAttackState))
                return result;

            result.Type = CombatResultType.Hit;
            result.AttackState = attAttackState;
            attRonin->HasHit = true;
            
            var defSaberState = frame.FindAsset(cDefender.SaberState);
            var defSaber = frame.Unsafe.GetPointer<SaberData>(cDefender.Entity);

            if (defSaberState.BlockType == SaberBlockType.Deflect)
            {
                bool deflected = false;
                
                switch (attAttackState.Height)
                {
                    case AttackHeight.High:
                        if (defRonin->TargetingSign == defRonin->FacingSign)
                        {
                            deflected = defSaber->Direction.Id == SaberDirection.FwHigh;
                        }
                        else
                        {
                            deflected = defSaber->Direction.Id == SaberDirection.BwHigh;
                        }
                        
                        break;
                    
                    case AttackHeight.Mid:
                        if (defRonin->TargetingSign == defRonin->FacingSign)
                        {
                            deflected = defSaber->Direction.Id == SaberDirection.FwMid;
                        }
                        else
                        {
                            deflected = defSaber->Direction.Id == SaberDirection.BwMid;
                        }

                        break;
                    
                    case AttackHeight.Low:
                        if (defRonin->TargetingSign == defRonin->FacingSign)
                        {
                            deflected = defSaber->Direction.Id == SaberDirection.FwLow;
                        }
                        else
                        {
                            deflected = defSaber->Direction.Id == SaberDirection.BwLow;
                        }

                        break;
                }

                if (deflected)
                {
                    result.Type = CombatResultType.Deflected;
                }

                return result;
            }

            if (cDefender.IsAttacking)
            {
                var defAttackState = frame.FindAsset(cDefender.AttackState);

                if (defAttackState.IsActive(frame, cDefender.Entity) && 
                    IsInRange(frame, defRonin, attRonin, defAttackState))
                {
                    if (attAttackState.Height == defAttackState.Height)
                    {
                        result.Type = CombatResultType.Clashed;
                    }
                }
            }
            
            return result;
        }

        private bool IsInRange(Frame frame, RoninData* attacker, RoninData* defender, AttackStateBase attack)
        {
            // this does not take into account if defender is behind attacker or not, might want to use actual hitboxes instead
            return FPVector2.Distance(attacker->Position, defender->Position) < attack.Range;
        }
    }
}
