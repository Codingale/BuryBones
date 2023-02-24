using System.Collections.Generic;

using RimWorld;

using Verse;
using Verse.AI;

namespace BuryBones
{
    public class JobDriver_BuryBones : JobDriver
    {
        private const TargetIndex HaulableCorpse = TargetIndex.A;
        private const TargetIndex TargetCell = TargetIndex.B;

        //Working vars
        private bool forbiddenInitially;

        public override void ExposeData()
        {
            base.ExposeData();


            Scribe_Values.Look(ref forbiddenInitially, "forbiddenInitially");
        }

        public override void Notify_Starting()
        {
            base.Notify_Starting();

            if (TargetThingA != null)
                forbiddenInitially = TargetThingA.IsForbidden(pawn);
            else
                forbiddenInitially = false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(HaulableCorpse);
            this.FailOnBurningImmobile(HaulableCorpse);

            if (!forbiddenInitially)
                this.FailOnForbidden(HaulableCorpse);

            Toil toilGoto = Toils_Goto.GotoThing(HaulableCorpse, PathEndMode.ClosestTouch)
                .FailOnSomeonePhysicallyInteracting(HaulableCorpse);
            yield return toilGoto;

            pawn.CurJob.count = 1;

            Toil toilHaul = Toils_Haul.StartCarryThing(HaulableCorpse);
            yield return toilHaul;

            Toil moveTo = Toils_Goto.GotoCell(TargetCell, PathEndMode.ClosestTouch);
            yield return moveTo;

            // Bury the corpse
            Toil Wait = new Toil
            {
                initAction = delegate
                {
                    pawn.pather.StopDead();
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 500 // 5 seconds
            };
            Wait.WithProgressBarToilDelay(TargetIndex.A);
            Wait.FailOnCannotTouch(TargetCell, PathEndMode.ClosestTouch);

            yield return Wait;

            // Drop the corpse
            Toil Drop = Toils_Haul.PlaceHauledThingInCell(TargetCell, Wait, false);
            yield return Drop;

            // Remove the corpse
            Toil Remove = new Toil
            {
                initAction = delegate
                {
                    TargetThingA?.Destroy();
                }
            };
            yield return Remove;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }
    }
}
