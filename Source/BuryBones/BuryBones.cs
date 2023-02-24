using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BuryBones
{
    /// <summary>
    /// Bury Bones mod, this mod allows you to bury corpses in an 'unmarked' grave... basically where they stand.
    /// </summary>
    public class BuryBones : Mod
    {
        /// <summary>
        /// The mod settings, singleton.
        /// </summary>
        public BuryBonesSettings Settings {get; private set;}

        /// <summary>
        /// The instance singleton.
        /// </summary>
        public static BuryBones Instance { get; private set; } = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuryBones"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public BuryBones(ModContentPack content) : base(content)
        {
            Settings = GetSettings<BuryBonesSettings>();
            Instance = this;

            // Patch the ReverseDesignatorDatabase to add our designator.
            var harmony = new HarmonyLib.Harmony("com.codingale.burybones");
            harmony.PatchAll();
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <returns>The settings.</returns>
        public override string SettingsCategory()
        {
            return "BuryBones".Translate();
        }

        /// <summary>
        /// Do the settings window.
        /// </summary>
        /// <param name="inRect">The in rect.</param>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
        }


        /// <summary>
        /// A debug logger that checks if debug mode is enabled.
        /// </summary>
        public static void DebugLog(string message)
        {
            if (!Instance.Settings.Debug) return;
            
            Log.Message(message);
        }
    }
}
