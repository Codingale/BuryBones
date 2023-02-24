using UnityEngine;

using RimWorld;
using Verse;

namespace BuryBones
{
    public class Designator_BuryCorpse : Designator
    {

        public override int DraggableDimensions => 2;

        protected override DesignationDef Designation => BuryBonesDefOf.BuryBonesDesignator;

        public Designator_BuryCorpse()
        {
            defaultDesc = "BuryBones.DesignatorBuryCorpseDesc".Translate();
            defaultLabel = "BuryBones.DesignatorBuryCorpse".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/BuryCorpse", true);
            useMouseIcon = true;
            soundSucceeded = SoundDefOf.Designate_Haul;
            soundFailed = SoundDefOf.Designate_Failed;

            showReverseDesignatorDisabledReason = true;
        }

        public override AcceptanceReport CanDesignateThing(Thing thing)
        {
            // We can only designate corpses.
            if (!(thing is Corpse corpse)) return false;

            // If the settings has only bury skeltons turned on we need to check if it's dessicated.
            if (BuryBones.Instance.Settings.SkeletonOnly && !corpse.IsDessicated()) return false;

            // We don't want to mark it if it's already marked.
            if (corpse.Map.designationManager.DesignationOn(corpse, Designation) != null) return false;
            
            // If we don't need to check, we'll just report true.
            return true;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            // We can only designate cells that have a corpse on them.
            if (!loc.InBounds(Map)) // This check was in Rimworld's source, so I'm keeping it here. Might not be needed.
                return AcceptanceReport.WasRejected;
            if (!DebugSettings.godMode && loc.Fogged(Map)) // This was too, but I doubt there'd be a corpse you can't see.
                return AcceptanceReport.WasRejected;
            
            HasBuriableCorpse(loc, out AcceptanceReport result);
            return result;
        }

        public override void DesignateSingleCell(IntVec3 loc)
        {
            DesignateThing(HasBuriableCorpse(loc, out AcceptanceReport _));
        }

        private Thing HasBuriableCorpse(IntVec3 loc, out AcceptanceReport result)
        {
            // Initialize the result.
            result = new AcceptanceReport("BuryBones.NoCorpse".Translate());

            // Loop over the things in this turf and see if there is a corpse that can be buried.
            foreach (Thing thing in loc.GetItems(Map))
            {
                if (CanDesignateThing(thing))
                {
                    result = AcceptanceReport.WasAccepted;

                    return thing; // We found a corpse, so we can stop looking.
                }
            }

            return null;
        }

        public override void DesignateThing(Thing thing)
        {
            if (DebugSettings.godMode)
                thing.Destroy(DestroyMode.Vanish);
            else
                Map.designationManager.AddDesignation(new Designation((LocalTargetInfo)thing, Designation));
        }
    }
}
