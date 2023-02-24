using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BuryBones
{
	public class BuryBonesSettings : ModSettings
	{
		public bool SoilOnlu = true;
		public bool SkeletonOnly = false;
		public bool Debug = false;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.SoilOnlu, "BuryBones.SoilOnly", true, false); // Should we only bury in soil? Or can we bury on any turf
			Scribe_Values.Look<bool>(ref this.SkeletonOnly, "BuryBones.SkeletonOnly", false, false); // Only bury skeletons, or any corpse?
			Scribe_Values.Look<bool>(ref this.Debug, "BuryBones.Debug", false, false); // Debug messages.
		}
		
		public void DoWindowContents(Rect inRect)
		{
			Listing_Standard listingStandard = new Listing_Standard();
			listingStandard.Begin(inRect);
			listingStandard.CheckboxLabeled("BuryBones.SoilOnly".Translate(), ref this.SoilOnlu, "BuryBones.SoilOnlyDesc".Translate());
			listingStandard.CheckboxLabeled("BuryBones.SkeletonOnly".Translate(), ref this.SkeletonOnly, "BuryBones.SkeletonOnlyDesc".Translate());
			listingStandard.CheckboxLabeled("BuryBones.Debug".Translate(), ref this.Debug, "BuryBones.DebugDesc".Translate());
			listingStandard.End();
		}
	}
}
