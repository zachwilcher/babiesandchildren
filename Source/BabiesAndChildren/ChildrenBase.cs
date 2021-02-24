using BabiesAndChildren.api;
using BabiesAndChildren.Tools;
using HugsLib;
using RimWorld;
using Verse;

namespace BabiesAndChildren
{
    public class ChildrenBase : ModBase
    {
        public static ChildrenBase Instance { get; private set; }
        
        public override string ModIdentifier => "Babies_and_Children";

        //Children School and Learning
        public static bool ModCSL_ON = false;
        //RimJobWorld
        public static bool ModRimJobWorld_ON = false;
        //Facial Animation - WIP
        public static bool ModFacialAnimation_ON = false;
        //Android tiers
        public static bool ModAndroid_Tiers_ON = false;
        //Dress Patients
        public static bool ModDressPatients_ON = false;

        private ChildrenBase()
        {
            Instance = this;
        }

        public override void DefsLoaded()
        {
            RaceUtility.ClearCache();
            CLog.Message("Adding CompProperties_Growing to races.");
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (RaceUtility.ThingUsesChildren(thingDef))
                {
                    thingDef.comps.Add(new CompProperties_Growing());         
                }
            }

        }

        public static void ReinitializeChildren(Map map)
        {
                CLog.Message("Resetting body types of all children.");
                    foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
                    {
                        if (RaceUtility.PawnUsesChildren(pawn) && AgeStages.IsYoungerThan(pawn ,AgeStages.Teenager))
                        {
                            if(pawn.story.bodyType == null)
                                pawn.story.bodyType = ((pawn.gender == Gender.Female) ? BodyTypeDefOf.Female : BodyTypeDefOf.Male);
                            
                            Growing_Comp comp = pawn.TryGetComp<Growing_Comp>();
                            comp?.Initialize(true);
                            ChildrenUtility.ChangeBodyType(pawn, true, false);
                            StoryUtility.ChangeChildBackstory(pawn);
                        }
                    }
            
        }

        public override void MapLoaded(Map map)
        {
            if (map != Current.Game.CurrentMap) return;
            if (!BnCSettings.OncePerGame)
            {
                foreach (Map tmap in Current.Game.Maps)
                {
                    ReinitializeChildren(tmap);
                }
                BnCSettings.OncePerGame = true;
            }
            if (BnCSettings.GestationPeriodDays_Enable)
            {
                foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
                {
                    if (RaceUtility.ThingUsesChildren(thingDef))
                    {
                        thingDef.race.gestationPeriodDays = BnCSettings.GestationPeriodDays;
                    }
                }
            }
        }
    }


}