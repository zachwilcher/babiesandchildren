using HugsLib;
using Verse;

namespace BabiesAndChildren
{
    public class ChildrenBase : ModBase
    {
        public static ChildrenBase Instance { get; private set; }
        
        public override string ModIdentifier
        {
            get { return "Babies_and_Children"; }
        }

        //Children School and Learning
        public static bool ModCSL_ON = false;
        //RimJobWorld
        public static bool ModRJW_ON = false;
        //Facial Animation - WIP
        public static bool ModWIP_ON = false;
        //Android tiers
        public static bool ModAT_ON = false;
        //Dress Patients
        public static bool ModDressPatients_ON = false;
        
        private ChildrenBase()
        {
            Instance = this;
        }

        public override void DefsLoaded()
        {
            CLog.Message("Adding CompProperties_Growing to races.");
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef.race != null && thingDef.race.Humanlike && thingDef.race.lifeStageAges.Count == 5)
                {
                    thingDef.comps.Add(new CompProperties_Growing());         
                }
            }

        }

        public override void MapLoaded(Map map)
        {
            if (map == Current.Game.CurrentMap)
            {
                if (!BnCSettings.OncePerGame)
                {
                    CLog.Message("Changing Body types of all children.");
                    foreach (Map tmap in Current.Game.Maps)
                    {
                        foreach (Pawn pawn in tmap.mapPawns.AllPawnsSpawned)
                        {
                            if (ChildrenUtility.RaceUsesChildren(pawn) && ChildrenUtility.GetAgeStage(pawn) < AgeStage.Teenager)
                            {
                                ChildrenUtility.ChangeBodyType(pawn, true, false);
                                ChildrenUtility.ChangeChildBackstory(pawn);
                            }
                        }
                    }
                    BnCSettings.OncePerGame = true;
                }
                if (BnCSettings.GestationPeriodDays_Enable)
                {
                    foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
                    {
                        if (thingDef.race != null && thingDef.race.Humanlike && thingDef.race.lifeStageAges.Count == 5)
                        {
                            thingDef.race.gestationPeriodDays = (float)BnCSettings.GestationPeriodDays;
               
                        }
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

        public static bool Valid(int ageStage)
        {
            return (ageStage <= Adult) && (ageStage >= Baby);
        }
    }


}