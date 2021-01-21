using HarmonyLib;
using System.Linq;
using Verse;

namespace RimWorldChildren
{
    [StaticConstructorOnStartup]
    public static class Initializer
    {
        static Initializer()
        {

            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef.race != null && thingDef.race.Humanlike && thingDef.race.lifeStageAges.Count == 5)
                {
                    thingDef.comps.Add(new CompProperties_Growing());         
                }
            }

            ChildrenBase.ModWIP_ON = ChildrenUtility.IsModOn("[NL] Facial Animation - WIP");
            ChildrenBase.ModCSL_ON = ChildrenUtility.IsModOn("Children, school and learning");
            ChildrenBase.ModRJW_ON = ChildrenUtility.IsModOn("RimJobWorld");
        }
    }

    //[HarmonyPatch(typeof(Game), "InitNewGame")]
    //static class Patch_Game_InitNewGame
    //{
    //    static void Postfix()
    //    {
    //        Hamony_Showhair.Showhair_Init();
    //    }
    //}

    //[HarmonyPatch(typeof(SavedGameLoaderNow), "LoadGameFromSaveFileNow")]
    //static class Patch_SavedGameLoader_LoadGameFromSaveFileNow
    //{
    //    [HarmonyPriority(Priority.Last)]
    //    static void Postfix()
    //    {
    //        if (!ChildrenBase.OncePerGame)
    //        {
    //            Log.Message("=============BNC Once Per Game=============");
    //            foreach (Map map in Current.Game.Maps)
    //            {
    //                foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
    //                {
    //                    if (ChildrenUtility.GetAgeStage(pawn) < AgeStage.Teenager) ChildrenUtility.ChangeBodyType(pawn, true, true);
    //                }
    //            }
    //            ChildrenBase.OncePerGame.Value = true;
    //        }
    //    }

    //}
}
