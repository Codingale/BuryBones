using System.Collections.Generic;
using System.Linq;

using RimWorld;

using Verse;
using Verse.AI;

namespace BuryBones
{
    public class WorkGiver_BuryCorpse : WorkGiver_Scanner
    {
        protected DesignationDef Designation
        {
            get
            {
                return BuryBonesDefOf.BuryBonesDesignator;
            }
        }

        protected JobDef BuryCorpse
        {
            get
            {
                return BuryBonesDefOf.BuryBones;
            }
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            foreach (Designation designation in pawn.Map.designationManager.SpawnedDesignationsOfDef(this.Designation))
            {
                yield return designation.target.Thing;
            }
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            BuryBones.DebugLog("Checking if we should skip the workgiver");
            List<Thing> things = pawn.Map.designationManager.SpawnedDesignationsOfDef(this.Designation).Select(d => d.target.Thing).ToList();

            // If there are no corpses designated to bury, we don't need to do anything.
            if (things.Count == 0) return true;

            // Double check there's at least one dessicated corpse we can bury
            if (BuryBones.Instance.Settings.SkeletonOnly && !things.Any(t => t.IsDessicated())) return true;

            BuryBones.DebugLog("We have work to do, returning false");
            return false;
        }

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            return pawn.CanReserve((LocalTargetInfo)thing) && pawn.Map.designationManager.DesignationOn(thing, this.Designation) != null;
        }

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            if (BuryBones.Instance.Settings.SkeletonOnly && !thing.IsDessicated() && !forced)
            {
                BuryBones.DebugLog($"JobOnThing({thing}, {forced}), thing is not dessicated, returning null");
                return null;
            }

            BuryBones.DebugLog($"JobOnThing({thing}, {forced}), BuryBones.Instance.Settings.BuryBonesSoil={BuryBones.Instance.Settings.SoilOnlu}");
            // Find a nearby tile that is considered to be soil.
            IntVec3 cell = IntVec3.Invalid;

            if (BuryBones.Instance.Settings.SoilOnlu)
            {
                IEnumerable<IntVec3> cells = GenRadial.RadialCellsAround(thing.Position, 32, true);

                foreach (IntVec3 c in cells)
                {
                    BuryBones.DebugLog($"Checking cell {c}: {c.GetTerrain(thing.Map).defName}, IsSoil={c.GetTerrain(thing.Map).IsSoil}");
                    if (c.InBounds(thing.Map) && c.GetTerrain(thing.Map).IsSoil)
                    {
                        cell = c;
                        break;
                    }
                }
            }
            else
            {
                cell = thing.Position;
            }

            if (cell == IntVec3.Invalid)
            {
                BuryBones.DebugLog($"JobOnThing({thing}, {forced}), cell is invalid, falling back to thing.Position");
                cell = thing.Position;
            }

            return JobMaker.MakeJob(BuryCorpse, (LocalTargetInfo)thing, (LocalTargetInfo)cell);
        }

    }
}