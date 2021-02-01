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
                        if (RaceUtility.PawnUsesChildren(pawn) && AgeStage.IsYoungerThan(pawn ,AgeStage.Teenager))
                        {
                            if(pawn.story.bodyType == null)
                                pawn.story.bodyType = ((pawn.gender == Gender.Female) ? BodyTypeDefOf.Female : BodyTypeDefOf.Male);
                            
                            Growing_Comp comp = pawn.TryGetComp<Growing_Comp>();
                            comp?.Initialize(true);
                            ChildrenUtility.ChangeBodyType(pawn, true, false);
                            ChildrenUtility.ChangeChildBackstory(pawn);
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
    // Handy for more readable code when age-checking
    public static class AgeStage
    {
        public const int Baby = 0;
        public const int Toddler = 1;
        public const int Child = 2;
        public const int Teenager = 3;
        public const int Adult = 4;


        public static int GetAgeStage(Pawn pawn)
        {
            int currentLifeStageIndex = pawn.ageTracker.CurLifeStageIndex;
            
            
            //clamp index between baby and adult
            //may cause incompatibilities with mods that add more life stages
            if (currentLifeStageIndex < AgeStage.Baby)
                return AgeStage.Baby;
            
            if(currentLifeStageIndex > AgeStage.Adult)
                 return AgeStage.Adult;

            return currentLifeStageIndex;

        }

        public static bool IsAgeStage(Pawn pawn, int lifeStage)
        {
            return GetAgeStage(pawn) == lifeStage;
        }

        public static bool IsYoungerThan(Pawn pawn, int lifeStage)
        {
            return GetAgeStage(pawn) < lifeStage;
        }

        public static bool IsOlderThan(Pawn pawn, int lifeStage)
        {
            return !IsAgeStage(pawn, lifeStage) && !IsYoungerThan(pawn, lifeStage);
        }

        public static LifeStageAge GetLifeStageAge(Pawn pawn, int ageStage)
        {
            return GetLifeStageAge(pawn.def.race, ageStage);
        }

        public static LifeStageAge GetPreviousLifeStageAge(Pawn pawn)
        {
            return GetLifeStageAge(pawn, GetAgeStage(pawn) - 1);
        }
        public static LifeStageAge GetCurrentLifeStageAge(Pawn pawn)
        {
            return GetLifeStageAge(pawn, GetAgeStage(pawn));
        }
        public static LifeStageAge GetNextLifeStageAge(Pawn pawn)
        {
            return GetLifeStageAge(pawn, GetAgeStage(pawn) + 1);
        }
        public static LifeStageAge GetLifeStageAge(RaceProperties raceProps, int ageStage)
        {
            return raceProps.lifeStageAges[ageStage];
        }
    }


}