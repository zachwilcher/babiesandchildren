using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorldChildren.Harmony
{
	// Token: 0x0200000A RID: 10
	internal class ChildrenHarmony
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00004824 File Offset: 0x00002A24
		public static void InitModOn()
		{
			try
			{
				ChildrenHarmony.ModCaP_ON = MorePawnUtilities.StartsWithIsModActive("Children and Pr");
				//ChildrenHarmony.ModBaC_ON = MorePawnUtilities.StartsWithIsModActive("Baby and Children");
				ChildrenHarmony.ModAlienChildrenCompatibility_ON = MorePawnUtilities.StartsWithIsModActive("Alien Children Compatibility");
				ChildrenHarmony.ModRoM_Werewolves_ON = MorePawnUtilities.StartsWithIsModActive("Rim of Madness - Werewolves");
				ChildrenHarmony.ModRoM_Vampires_ON = MorePawnUtilities.StartsWithIsModActive("Rim of Madness - Vampires");
				ChildrenHarmony.ModAvali_ON = MorePawnUtilities.StartsWithIsModActive("Avali");
				ChildrenHarmony.ModRationalRomance_ON = MorePawnUtilities.ContainsIsModActive("Rational Romance");
				ChildrenHarmony.ModBirdsAndBees_ON = MorePawnUtilities.StartsWithIsModActive("The Birds and the Bees");
			}
			catch (Exception ex)
			{
				if (Prefs.DevMode)
				{
					ChdLogging.LogError("Cildren: InitModOn: error: " + ex.Message);
				}
			}
		}

		// Token: 0x04000058 RID: 88
		public const string ModVersion = "v1.5.5";

		// Token: 0x04000059 RID: 89
		public const string ModNameCaP = "Children and Pr";

		// Token: 0x0400005A RID: 90
		//public const string ModNameBaC = "Baby and Children";

		// Token: 0x0400005B RID: 91
		public const string ModNameAlienChildrenCompatibility = "Alien Children Compatibility";

		// Token: 0x0400005C RID: 92
		public const string ModNameRoM_Werewolves = "Rim of Madness - Werewolves";

		// Token: 0x0400005D RID: 93
		public const string ModNameRoM_Vampires = "Rim of Madness - Vampires";

		// Token: 0x0400005E RID: 94
		public const string ModNameAvali = "Avali";

		// Token: 0x0400005F RID: 95
		public const string ModNameRationalRomance = "Rational Romance";

		// Token: 0x04000060 RID: 96
		public const string ModNameBirdsAndBees = "The Birds and the Bees";

		// Token: 0x04000061 RID: 97
		public const string ModChildBackstoryDefaultName = "CHDR_ColonyChild";

		// Token: 0x04000062 RID: 98
		public const string ModChildBackstoryEnding = "_Child_Colony_NewbornCSL";

		// Token: 0x04000063 RID: 99
		public static bool ModCaP_ON;

		// Token: 0x04000064 RID: 100
		public static bool ModBaC_ON;

		// Token: 0x04000065 RID: 101
		public static bool ModAlienChildrenCompatibility_ON;

		// Token: 0x04000066 RID: 102
		public static bool ModRoM_Werewolves_ON;

		// Token: 0x04000067 RID: 103
		public static bool ModRoM_Vampires_ON;

		// Token: 0x04000068 RID: 104
		public static bool ModAvali_ON;

		// Token: 0x04000069 RID: 105
		public static bool ModRationalRomance_ON;

		// Token: 0x0400006A RID: 106
		public static bool ModBirdsAndBees_ON;

		// Token: 0x02000023 RID: 35
		[HarmonyPatch(typeof(ListerFilthInHomeArea), "Notify_HomeAreaChanged")]
		public static class Notify_HomeAreaChanged_Patch
		{
			// Token: 0x0600008C RID: 140 RVA: 0x00009228 File Offset: 0x00007428
			[HarmonyPrefix]
			public static void Notify_HomeAreaChanged_Pre(ListerFilthInHomeArea __instance, IntVec3 c)
			{
				try
				{
					if (ChildrenController.Instance.FixBrokenMap.Value)
					{
						List<Map> maps = Find.Maps;
						Map currentMap = Find.CurrentMap;
						if (currentMap.areaManager == null)
						{
							currentMap.areaManager = new AreaManager(currentMap);
							currentMap.areaManager.AddStartingAreas();
							if (Prefs.DevMode)
							{
								CLog.Message("Notify_HomeAreaChanged: Areamanager fixed currentMap");
							}
							ChildrenController.Instance.FixBrokenMap.Value = false;
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x02000024 RID: 36
		[HarmonyPatch(typeof(Bill), "PawnAllowedToStartAnew")]
		public static class Bill_Patch
		{
			// Token: 0x0600008D RID: 141 RVA: 0x000092A8 File Offset: 0x000074A8
			[HarmonyPostfix]
			public static void PawnAllowedToStartAnew(ref bool __result, ref Bill __instance, Pawn p)
			{
				BillController.PawnAllowedToStartAnew(ref __result, ref __instance, p);
			}
		}

		// Token: 0x02000025 RID: 37
		[HarmonyPatch(typeof(ThinkNode_ChancePerHour_Lovin), "MtbHours")]
		public static class ThinkNode_ChancePerHour_Lovin_Patch
		{
			// Token: 0x0600008E RID: 142 RVA: 0x000092B4 File Offset: 0x000074B4
			[HarmonyPostfix]
			public static void ThinkNode_ChancePerHour_Lovin_MtbHoursDone(ThinkNode_ChancePerHour_Lovin __instance, ref float __result, Pawn pawn)
			{
				try
				{
					if (!ChildrenController.Instance.DebugTickDisabled.Value && __result > 100f)
					{
						float num = __result;
						__result *= 0.07f;
						if (Prefs.DevMode)
						{
							CLog.Message("ThinkNode_ChancePerHour_Lovin_MtbHoursDone: Chances modified from " + num.ToString() + " to " + __result.ToString());
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("ThinkNode_ChancePerHour_Lovin_MtbHoursDone: error: " + ex.Message);
					}
				}
			}
		}

		// Token: 0x02000026 RID: 38
		[HarmonyPatch(typeof(Hediff_Pregnant), "Tick")]
		public static class Hediff_Pregnant_PatchMCPrevent
		{
			// Token: 0x0600008F RID: 143 RVA: 0x00009344 File Offset: 0x00007544
			[HarmonyPrefix]
			public static bool Hediff_Pregnant_TickPre(Hediff_Pregnant __instance)
			{
				try
				{
					if (ChildrenController.Instance.NoMiscarriageEnabled.Value && __instance != null && __instance.pawn != null && __instance.pawn.RaceProps.Humanlike)
					{
						__instance.ageTicks++;
						if (__instance.pawn.IsHashIntervalTick(1000))
						{
							if (__instance.pawn.needs.food != null && __instance.pawn.needs.food.CurCategory == HungerCategory.Starving && __instance.pawn.health.hediffSet.HasHediff(HediffDefOf.Malnutrition, false) && (double)__instance.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition, false).Severity > 0.25 && Rand.MTBEventOccurs(0.5f, 60000f, 1000f) && Prefs.DevMode)
							{
								CLog.Message("Children: Hediff_Pregnant: Tick: Skipped miscar food!");
							}
							if (ChildrenHarmony.Hediff_Pregnant_PatchMCPrevent.IsSeverelyWounded(__instance.pawn) && Rand.MTBEventOccurs(0.5f, 60000f, 1000f) && Prefs.DevMode)
							{
								CLog.Message("Children: Hediff_Pregnant: Tick: Skipped miscar health!");
							}
						}
						__instance.Severity += (float)(1.0 / ((double)__instance.pawn.RaceProps.gestationPeriodDays * 60000.0));
						if ((double)__instance.GestationProgress < 1.0)
						{
							return false;
						}
						if (__instance.Visible && PawnUtility.ShouldSendNotificationAbout(__instance.pawn))
						{
							Messages.Message("MessageGaveBirth".Translate(__instance.pawn).CapitalizeFirst(), __instance.pawn, MessageTypeDefOf.PositiveEvent, true);
						}
						Hediff_Pregnant.DoBirthSpawn(__instance.pawn, __instance.father);
						__instance.pawn.health.RemoveHediff(__instance);
						return false;
					}
				}
				catch
				{
				}
				return true;
			}

			// Token: 0x06000090 RID: 144 RVA: 0x00009560 File Offset: 0x00007760
			public static bool IsSeverelyWounded(Pawn pawn)
			{
				float num = 0f;
				List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
				for (int i = 0; i < hediffs.Count; i++)
				{
					if (hediffs[i] is Hediff_Injury && !hediffs[i].IsPermanent())
					{
						num += hediffs[i].Severity;
					}
				}
				List<Hediff_MissingPart> missingPartsCommonAncestors = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
				for (int j = 0; j < missingPartsCommonAncestors.Count; j++)
				{
					if (missingPartsCommonAncestors[j].IsFreshNonSolidExtremity)
					{
						num += missingPartsCommonAncestors[j].Part.def.GetMaxHealth(pawn);
					}
				}
				return (double)num > 38.0 * (double)pawn.RaceProps.baseHealthScale;
			}
		}

		// Token: 0x02000027 RID: 39
		[HarmonyPatch(typeof(Hediff_Pregnant), "Miscarry")]
		public static class Hediff_Pregnant_Patch
		{
			// Token: 0x06000091 RID: 145 RVA: 0x0000962C File Offset: 0x0000782C
			[HarmonyPostfix]
			public static void Hediff_Pregnant_MiscarryDone(Hediff_Pregnant __instance)
			{
				try
				{
					if (__instance != null && __instance.pawn != null && __instance.pawn.IsColonist)
					{
						if (ChildrenHarmony.ModCaP_ON)
						{
							if (Prefs.DevMode)
							{
								CLog.Message("Pregnant: Miscarry-Postfix: Skipped for C&P!");
							}
						}
						else
						{
							if (__instance.Visible)
							{
								if (!__instance.pawn.story.traits.HasTrait(MorePawnUtilities.TraitDefOfMODDED.ChildrenDislikesChildren))
								{
									__instance.pawn.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(MorePawnUtilities.ThoughtDefOfMODDED.ChildrenMiscarriageNegative), null);
								}
								else
								{
									__instance.pawn.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(MorePawnUtilities.ThoughtDefOfMODDED.ChildrenMiscarriagePositive), null);
								}
								if (__instance.father != null)
								{
									if (!__instance.father.story.traits.HasTrait(MorePawnUtilities.TraitDefOfMODDED.ChildrenDislikesChildren))
									{
										__instance.father.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(MorePawnUtilities.ThoughtDefOfMODDED.ChildrenMiscarriageNegativePartner), __instance.pawn);
									}
									else
									{
										__instance.father.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(MorePawnUtilities.ThoughtDefOfMODDED.ChildrenMiscarriagePositivePartner), __instance.pawn);
									}
								}
								if (Prefs.DevMode)
								{
									CLog.Message("Pregnant: Miscarry-Postfix: Done!");
								}
							}
							else if (Prefs.DevMode)
							{
								CLog.Message("Pregnant: Miscarry-Postfix: Done(not visible)!");
							}
							if (__instance.pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.PregnancyDiscovered, false))
							{
								__instance.pawn.health.RemoveHediff(__instance.pawn.health.hediffSet.GetFirstHediffOfDef(MorePawnUtilities.HediffDefOfMODDED.PregnancyDiscovered, false));
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Pregnant: Miscarry: error: " + ex.Message);
					}
				}
			}
		}

		// Token: 0x02000028 RID: 40
		[HarmonyPatch(typeof(HediffComp_Discoverable), "CheckDiscovered")]
		public static class HediffComp_Discoverable_Patch
		{
			// Token: 0x06000092 RID: 146 RVA: 0x0000983C File Offset: 0x00007A3C
			[HarmonyPrefix]
			public static void Hediff_Pregnant_CheckDiscoveredPre(HediffComp_Discoverable __instance)
			{
				try
				{
					if (__instance != null && __instance.CompDisallowVisible() && __instance.parent != null && __instance.parent.CurStage != null && __instance.parent.CurStage.becomeVisible && __instance.Def != null && __instance.Def.defName == HediffDefOf.Pregnant.defName)
					{
						if (ChildrenHarmony.ModCaP_ON)
						{
							if (Prefs.DevMode)
							{
								CLog.Message("Pregnant: CheckDiscovered-Prefix: Skipped for C&P!");
							}
						}
						else
						{
							ChildrenController.PregnancyInitGiveMoods(__instance.Pawn);
							if (Prefs.DevMode)
							{
								CLog.Message("Pregnant: CheckDiscovered-Prefix: Done!");
							}
						}
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x02000029 RID: 41
		[HarmonyPatch(typeof(JobDriver_Lovin), "GenerateRandomMinTicksToNextLovin")]
		public static class JobDriver_Lovin_Patch
		{
			// Token: 0x06000093 RID: 147 RVA: 0x000098F0 File Offset: 0x00007AF0
			[HarmonyPostfix]
			public static void JobDriver_Lovin_Done(JobDriver_Lovin __instance)
			{
				try
				{
					Pawn pawn = (Pawn)Traverse.Create(__instance).Property("Partner", null).GetValue();
					if (Prefs.DevMode)
					{
						CLog.Message("Lovin: GenerateRandomMinTicksToNextLovin: " + __instance);
						CLog.Message("Lovin: GenerateRandomMinTicksToNextLovin: " + pawn);
					}
					MorePawnUtilities.Loved(__instance.pawn, pawn, false);
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Lovin: Tick: error: " + ex.Message);
					}
				}
			}
		}

		// Token: 0x0200002A RID: 42
		[HarmonyPatch(typeof(MemoryThoughtHandler), "TryGainMemory")]
		[HarmonyPatch(new Type[]
		{
			typeof(Thought_Memory),
			typeof(Pawn)
		})]
		public static class MemoryThoughtHandler_Avali_Lovin_Patch
		{
			// Token: 0x06000094 RID: 148 RVA: 0x00009980 File Offset: 0x00007B80
			[HarmonyPostfix]
			public static void MemoryThoughtHandler_TryGainMemory_Done(MemoryThoughtHandler __instance, Thought_Memory newThought, Pawn otherPawn = null)
			{
				try
				{
					if (ChildrenHarmony.ModAvali_ON && ChildrenController.Instance.AvaliChildrenAllowed.Value)
					{
						Pawn pawn = __instance.pawn;
						if (newThought.def == ThoughtDefOf.GotSomeLovin && pawn != null && otherPawn != null && otherPawn.kindDef.race.defName == "Avali")
						{
							if (Prefs.DevMode)
							{
								CLog.Message("Avali-Lovin: TryGainMemory: " + pawn);
								CLog.Message("Avali-Lovin: TryGainMemory: " + otherPawn);
							}
							MorePawnUtilities.Loved(pawn, otherPawn, false);
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Avali-Lovin: TryGainMemory: error: " + ex.Message);
					}
				}
				try
				{
					if (ChildrenHarmony.ModRationalRomance_ON)
					{
						Pawn pawn2 = __instance.pawn;
						if (newThought.def == ThoughtDefOf.GotSomeLovin && pawn2 != null && otherPawn != null)
						{
							bool flag = false;
							try
							{
								if (new StackFrame(4).GetMethod().Module.Assembly.FullName.Contains("Rational Romance"))
								{
									flag = true;
								}
								if (new StackFrame(3).GetMethod().Module.Assembly.FullName.Contains("Rational Romance"))
								{
									flag = true;
								}
								if (new StackFrame(2).GetMethod().Module.Assembly.FullName.Contains("Rational Romance"))
								{
									flag = true;
								}
								if (new StackFrame(1).GetMethod().Module.Assembly.FullName.Contains("Rational Romance"))
								{
									flag = true;
								}
							}
							catch (Exception)
							{
							}
							if (flag)
							{
								if (Prefs.DevMode)
								{
									CLog.Message("Rational-Romance-Lovin detected! (TryGainMemory:GotSomeLovin)");
								}
								if (Prefs.DevMode)
								{
									CLog.Message("Rational-Romance-Lovin: TryGainMemory: " + pawn2);
									CLog.Message("Rational-Romance-Lovin: TryGainMemory: " + otherPawn);
								}
								MorePawnUtilities.Loved(pawn2, otherPawn, false);
							}
						}
					}
				}
				catch (Exception ex2)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Rational-Romance-Lovin: TryGainMemory: error: " + ex2.Message);
					}
				}
			}
		}

		// Token: 0x0200002B RID: 43
		[HarmonyPatch(typeof(PawnGenerator), "GenerateRandomAge")]
		public static class PawnGenerator_Gen_Patch
		{
			// Token: 0x06000095 RID: 149 RVA: 0x00009BC4 File Offset: 0x00007DC4
			[HarmonyPostfix]
			public static void PawnGenerator_Gen_Done(Pawn pawn, PawnGenerationRequest request)
			{
				try
				{
					if (ChildrenController.Instance.RandomBackstoryEnabled.Value)
					{
						int childAge = ChildrenHarmony.PawnUtility_Patch.GetChildAge(pawn, false);
						if ((float)childAge >= ChildrenHarmony.PawnUtility_Patch.GetChildRaceAgeAdult(pawn, false) && request.Faction.IsPlayer && request.KindDef.RaceProps.Humanlike && request.Newborn)
						{
							pawn.ageTracker.AgeBiologicalTicks = (long)((double)childAge * 3600000.0);
							pawn.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
							if (Prefs.DevMode)
							{
								CLog.Message("Children: Birth: GenerateRandomAge: Bio age set to: " + pawn.ageTracker.AgeBiologicalYears.ToString() + " CronAge Set to: " + pawn.ageTracker.AgeChronologicalYears.ToString());
							}
						}
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x0200002C RID: 44
		[HarmonyPatch(typeof(ThoughtWorker), "CurrentState")]
		public static class ThoughtWorker_Patch
		{
			// Token: 0x06000096 RID: 150 RVA: 0x00009CAC File Offset: 0x00007EAC
			[HarmonyPostfix]
			public static void ThoughtWorker_Done(ThoughtWorker __instance, ref ThoughtState __result, Pawn p)
			{
				try
				{
					if (ChildrenHarmony.ModBirdsAndBees_ON && __instance != null && __instance.def != null && p != null)
					{
						if (!ChildrenHarmony.ThoughtWorker_Patch.NeuteredDefCacheSet && __instance.def.defName.ToLower() == "neutered")
						{
							ChildrenHarmony.ThoughtWorker_Patch.NeuteredDefCache = __instance.def;
							ChildrenHarmony.ThoughtWorker_Patch.NeuteredDefCacheSet = true;
						}
						if (ChildrenHarmony.ThoughtWorker_Patch.NeuteredDefCacheSet && __instance.def == ChildrenHarmony.ThoughtWorker_Patch.NeuteredDefCache && __result.Active && __result.StageIndex == 0)
						{
							if (p.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.BirthControlActive, false))
							{
								__result = ThoughtState.Inactive;
								if (__instance.def.invert)
								{
									__result = ((!__result.Active) ? ThoughtState.ActiveAtStage(0) : ThoughtState.Inactive);
								}
							}
							else if (p.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.Postpartum, false))
							{
								__result = ThoughtState.Inactive;
								if (__instance.def.invert)
								{
									__result = ((!__result.Active) ? ThoughtState.ActiveAtStage(0) : ThoughtState.Inactive);
								}
							}
							else if (p.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.OneChildOnly, false))
							{
								__result = ThoughtState.Inactive;
								if (__instance.def.invert)
								{
									__result = ((!__result.Active) ? ThoughtState.ActiveAtStage(0) : ThoughtState.Inactive);
								}
							}
							else if (p.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.NoReproductionLifestage, false))
							{
								__result = ThoughtState.Inactive;
								if (__instance.def.invert)
								{
									__result = ((!__result.Active) ? ThoughtState.ActiveAtStage(0) : ThoughtState.Inactive);
								}
							}
							else if (p.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.NoReproductionLifestageOldAge, false))
							{
								__result = ThoughtState.Inactive;
								if (__instance.def.invert)
								{
									__result = ((!__result.Active) ? ThoughtState.ActiveAtStage(0) : ThoughtState.Inactive);
								}
							}
							else if (p.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.FertilityDisabledDefault, false))
							{
								__result = ThoughtState.Inactive;
								if (__instance.def.invert)
								{
									__result = ((!__result.Active) ? ThoughtState.ActiveAtStage(0) : ThoughtState.Inactive);
								}
							}
							else if (p.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.NoReproductionAndroid, false))
							{
								__result = ThoughtState.Inactive;
								if (__instance.def.invert)
								{
									__result = ((!__result.Active) ? ThoughtState.ActiveAtStage(0) : ThoughtState.Inactive);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Compatibility B&B Neutered + same room debuff: error: " + ex.Message);
					}
				}
			}

			// Token: 0x040000BC RID: 188
			private static ThoughtDef NeuteredDefCache;

			// Token: 0x040000BD RID: 189
			public static bool NeuteredDefCacheSet;
		}

		// Token: 0x0200002D RID: 45
		[HarmonyPatch(typeof(Pawn_AgeTracker), "RecalculateLifeStageIndex")]
		public static class Pawn_AgeTracker_Patch
		{
			// Token: 0x06000098 RID: 152 RVA: 0x00009FB8 File Offset: 0x000081B8
			[HarmonyPostfix]
			public static void RecalculateLifeStageIndex_Done(Pawn_AgeTracker __instance)
			{
				try
				{
					if (__instance != null)
					{
						Pawn pawn = null;
						try
						{
							pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
						}
						catch
						{
						}
						if (ChildrenController.Instance.GetReproductionFromLifeStageEnabled.Value && pawn != null)
						{
							ChildrenController.NoReproductionMod(pawn);
						}
						if (ChildrenController.Instance.GetReproductionFromOldAgeEnabled.Value && pawn != null)
						{
							ChildrenController.NoReproductionModOldAge(pawn);
						}
						if (pawn != null)
						{
							ChildrenController.NoReproductionAndroidCheckAndMod(pawn);
						}
						if (ChildrenHarmony.ModRoM_Werewolves_ON)
						{
							try
							{
								pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
							}
							catch
							{
							}
							if (pawn != null)
							{
								TraitDef traitDef = TraitDef.Named("ROM_Werewolf");
								if (traitDef != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(MorePawnUtilities.TraitDefOfMODDED.ChildrenWerewolfChild) && (double)pawn.ageTracker.AgeBiologicalYearsFloat >= (double)ChildrenController.Instance.WerewolfChildrenMinAgeFirstTransformation.Value)
								{
									Trait trait = new Trait(traitDef, -1, false);
									pawn.story.traits.GainTrait(trait);
									trait = pawn.story.traits.GetTrait(MorePawnUtilities.TraitDefOfMODDED.ChildrenWerewolfChild);
									pawn.story.traits.allTraits.Remove(trait);
								}
							}
						}
						if (!pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.ChildBirthInProgress, false))
						{
							ChildrenHarmony.Pawn_AgeTracker_Patch.CheckWorkingPawn(pawn, true);
						}
						else
						{
							ChildrenHarmony.Pawn_AgeTracker_Patch.CheckWorkingPawn(pawn, false);
						}
						if (pawn != null && pawn.health != null && pawn.ageTracker != null && pawn.ageTracker.AgeBiologicalYearsFloat > (float)Math.Floor((double)pawn.ageTracker.AgeBiologicalYearsFloat) + 0.5f && pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.ChildRenamePending, false))
						{
							pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(MorePawnUtilities.HediffDefOfMODDED.ChildRenamePending, false));
							if (Prefs.DevMode)
							{
								CLog.Message("Children: ChildRenamePending removed!");
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Old or Young reproductive: error: " + ex.Message);
					}
				}
			}

			// Token: 0x06000099 RID: 153 RVA: 0x0000A20C File Offset: 0x0000840C
			public static void CheckWorkingPawn(Pawn p, bool AlertMessage)
			{
				if (p != null && p.skills != null && p.workSettings != null && p.story != null && p.kindDef != null && p.kindDef.race != null)
				{
					try
					{
						if (!ChildrenHarmony.ModCaP_ON || p.kindDef.race.ToString().ToLower() != "human")
						{
							if (ChildrenController.Instance.NoWorkYoungEnabled.Value)
							{
								string text = p.ageTracker.CurLifeStage.defName.ToLower();
								if (text.Contains("baby") || text.Contains("toddler"))
								{
									if (!p.story.traits.HasTrait(MorePawnUtilities.TraitDefOfMODDED.ChildrenToYoungToWork))
									{
										p.story.traits.GainTrait(new Trait(MorePawnUtilities.TraitDefOfMODDED.ChildrenToYoungToWork, 0, false));
										p.skills.Notify_SkillDisablesChanged();
										p.workSettings.EnableAndInitialize();
									}
								}
								else if (p.story.traits.HasTrait(MorePawnUtilities.TraitDefOfMODDED.ChildrenToYoungToWork))
								{
									p.story.traits.allTraits.Remove(p.story.traits.GetTrait(MorePawnUtilities.TraitDefOfMODDED.ChildrenToYoungToWork));
									try
									{
										Traverse.Create(p.story).Field("cachedDisabledWorkTypes").SetValue(null);
									}
									catch
									{
									}
									p.skills.Notify_SkillDisablesChanged();
									MorePawnUtilities.SetPawnSkillsAndPassions(p, MorePawnUtilities.GetMotherKin(p), MorePawnUtilities.GetFatherKin(p), "ChildrenMessageOldEnoughToWork", true, true);
									p.workSettings.EnableAndInitialize();
									if (AlertMessage)
									{
										Find.LetterStack.ReceiveLetter("ChildrenMessageOldEnoughToWork".Translate(p.LabelShort, p.LabelShort, p.LabelShort), "ChildrenMessageOldEnoughToWork".Translate(p.LabelShort, p.LabelShort, p.LabelShort), LetterDefOf.PositiveEvent, p, null, null, null, null);
									}
								}
							}
							else if (p.story.traits.HasTrait(MorePawnUtilities.TraitDefOfMODDED.ChildrenToYoungToWork))
							{
								p.story.traits.allTraits.Remove(p.story.traits.GetTrait(MorePawnUtilities.TraitDefOfMODDED.ChildrenToYoungToWork));
								try
								{
									Traverse.Create(p.story).Field("cachedDisabledWorkTypes").SetValue(null);
								}
								catch
								{
								}
								p.skills.Notify_SkillDisablesChanged();
								MorePawnUtilities.SetPawnSkillsAndPassions(p, MorePawnUtilities.GetMotherKin(p), MorePawnUtilities.GetFatherKin(p), "ChildrenMessageOldEnoughToWork", true, true);
								p.workSettings.EnableAndInitialize();
								if (AlertMessage)
								{
									Find.LetterStack.ReceiveLetter("ChildrenMessageOldEnoughToWork".Translate(p.LabelShort, p.LabelShort, p.LabelShort), "ChildrenMessageOldEnoughToWork".Translate(p.LabelShort, p.LabelShort, p.LabelShort), LetterDefOf.PositiveEvent, p, null, null, null, null);
								}
							}
						}
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// Token: 0x0200002E RID: 46
		[HarmonyPatch(typeof(PawnCapacitiesHandler), "GetLevel")]
		public static class PawnCapacitiesHandler_Patch
		{
			// Token: 0x0600009A RID: 154 RVA: 0x0000A58C File Offset: 0x0000878C
			[HarmonyPostfix]
			public static void GetLevel_Done(PawnCapacitiesHandler __instance, ref float __result, PawnCapacityDef capacity)
			{
				try
				{
					if (capacity == MorePawnUtilities.PawnCapacityDefOf.Fertility && ChildrenHarmony.ModBirdsAndBees_ON && __instance != null)
					{
						string text = "";
						string text2 = "";
						try
						{
							MethodBase method = new StackFrame(2).GetMethod();
							text = method.Name;
							text2 = method.Module.Assembly.FullName;
						}
						catch
						{
						}
						if (text.ToLower() == "lovinlevel" && text2.ToLower().StartsWith("fluffy_birdsandbees"))
						{
							Pawn pawn = null;
							try
							{
								pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
							}
							catch
							{
							}
							if (pawn != null && (pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.BirthControlActive, false) || pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.Postpartum, false) || pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.OneChildOnly, false) || pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.NoReproductionLifestage, false) || pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.NoReproductionLifestageOldAge, false) || pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.FertilityDisabledDefault, false) || pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.NoReproductionAndroid, false)))
							{
								List<BodyPartRecord> allParts = pawn.RaceProps.body.AllParts;
								int i = 0;
								while (i < allParts.Count)
								{
									if (allParts[i].def.defName.ToLower() == "reproductiveorgans")
									{
										if (!pawn.health.hediffSet.PartIsMissing(allParts[i]))
										{
											float num = pawn.health.hediffSet.GetPartHealth(allParts[i]) / 10f;
											if (Prefs.DevMode)
											{
												CLog.Message(string.Concat(new object[]
												{
													"Compatibility B&B LovinLevel>FertilityCapacity: Pawn: ",
													pawn.Name,
													" Changed: ",
													__result,
													" to ",
													num
												}));
											}
											__result = num;
											break;
										}
										break;
									}
									else
									{
										i++;
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Compatibility B&B FertilityCapacity: error: " + ex.Message);
					}
				}
			}
		}

		// Token: 0x0200002F RID: 47
		[HarmonyPatch(typeof(HediffGiver))]
		[HarmonyPatch("TryApply")]
		public class HediffGiver_TryApply
		{
			// Token: 0x0600009B RID: 155 RVA: 0x0000A834 File Offset: 0x00008A34
			[HarmonyPrefix]
			[HarmonyBefore(new string[]
			{
				"Fluffy.BirdsAndBees",
				"Fluffy_BirdsAndBees"
			})]
			[HarmonyPriority(600)]
			private static bool HediffGiver_TryApply_Prefix(HediffGiver __instance, ref bool __result, Pawn pawn)
			{
				try
				{
					if (ChildrenHarmony.ModBirdsAndBees_ON && pawn != null && (pawn.kindDef.race.ToString().ToLower().Contains("lotre_elf") || pawn.kindDef.race.ToString().ToLower().Contains("lotre_halfelf")) && (__instance.hediff.defName == "Impotence" || __instance.hediff.defName == "Menopause"))
					{
						if (Prefs.DevMode)
						{
							CLog.Message(string.Concat(new object[]
							{
								"Elf B&B fix: Pawn: ",
								pawn.Name,
								" Prevented: ",
								__instance.hediff.defName
							}));
						}
						__result = false;
						return false;
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Elf B&B fix TryApply: error: " + ex.Message);
					}
				}
				return true;
			}
		}

		// Token: 0x02000030 RID: 48
		[HarmonyPatch(typeof(TimeAssignmentSelector), "DrawTimeAssignmentSelectorGrid")]
		public static class TimeAssignmentSelector_Patch
		{
			// Token: 0x0600009D RID: 157 RVA: 0x0000A93C File Offset: 0x00008B3C
			[HarmonyPostfix]
			public static void DrawTimeAssignmentSelectorGrid_Done(Rect rect)
			{
				try
				{
					Rect rect2 = rect;
					rect2.xMax = rect2.center.x;
					rect2.yMax = rect2.center.y;
					rect2.x += rect2.width;
					rect2.x += rect2.width;
					rect2.y += rect2.height;
					rect2.height = rect2.height / 3f * 2f;
					ChildrenHarmony.TimeAssignmentSelector_Patch.DrawTimeAssignmentSelectorFor(rect2, MorePawnUtilities.TimeAssignmentDefOf.School);
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Timetable edit in School: error: " + ex.Message);
					}
				}
			}

			// Token: 0x0600009E RID: 158 RVA: 0x0000AA08 File Offset: 0x00008C08
			private static void DrawTimeAssignmentSelectorFor(Rect rect, TimeAssignmentDef ta)
			{
				rect = rect.ContractedBy(2f);
				GUI.DrawTexture(rect, ta.ColorTexture);
				if (Widgets.ButtonInvisible(rect, true))
				{
					TimeAssignmentSelector.selectedAssignment = ta;
					SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
				}
				GUI.color = Color.white;
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.MiddleCenter;
				GUI.color = Color.white;
				Widgets.Label(rect, ta.LabelCap);
				Text.Anchor = TextAnchor.UpperLeft;
				if (TimeAssignmentSelector.selectedAssignment != ta)
				{
					return;
				}
				Widgets.DrawBox(rect, 2);
			}
		}

		// Token: 0x02000031 RID: 49
		[HarmonyPatch(typeof(Hediff))]
		[HarmonyPatch("Label", 1)]
		public static class Hediff_Label_Patch
		{
			// Token: 0x0600009F RID: 159 RVA: 0x0000AA98 File Offset: 0x00008C98
			[HarmonyPostfix]
			public static void Label_Post(Hediff __instance, ref string __result)
			{
				try
				{
					if (ChildrenController.Instance.ShowFatherEnabled.Value && __instance.def != null && __instance.def == HediffDefOf.Pregnant)
					{
						Hediff_Pregnant hediff_Pregnant = (Hediff_Pregnant)__instance;
						if (hediff_Pregnant.father != null && hediff_Pregnant.father != hediff_Pregnant.pawn)
						{
							if (hediff_Pregnant.father.gender == Gender.Female)
							{
								if (!__result.Contains("(" + "ChildrenMessageSecondMother".Translate() + ":"))
								{
									__result += " (" + "ChildrenMessageSecondMother".Translate() + ": " + hediff_Pregnant.father.Name.ToStringShort + ")";
								}
							}
							else if (hediff_Pregnant.father.gender == Gender.None)
							{
								if (!__result.Contains("(" + "ChildrenMessagePartner".Translate() + ":"))
								{
									__result += " (" + "ChildrenMessagePartner".Translate() + ": " + hediff_Pregnant.father.Name.ToStringShort + ")";
								}
							}
							else if (!__result.Contains("(" + "ChildrenMessageFather".Translate() + ":"))
							{
								__result += " (" + "ChildrenMessageFather".Translate() + ": " + hediff_Pregnant.father.Name.ToStringShort + ")";
							}
						}
						else if (!__result.Contains("(" + "ChildrenMessageNoPartner".Translate() + ""))
						{
							__result += " (" + "ChildrenMessageNoPartner".Translate() + ")";
						}
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x02000032 RID: 50
		[HarmonyPatch(typeof(Pawn_TimetableTracker))]
		[HarmonyPatch("CurrentAssignment", 1)]
		public static class Pawn_TimetableTracker_Patch
		{
			// Token: 0x060000A0 RID: 160 RVA: 0x0000AD08 File Offset: 0x00008F08
			[HarmonyPostfix]
			public static void CurrentAssignment_Post(Pawn_TimetableTracker __instance, ref TimeAssignmentDef __result)
			{
				try
				{
					if (__result == MorePawnUtilities.TimeAssignmentDefOf.School)
					{
						__result = TimeAssignmentDefOf.Work;
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("CurrentAssignment edit in School: error: " + ex.Message);
					}
				}
			}
		}

		// Token: 0x02000033 RID: 51
		[HarmonyPatch(typeof(Pawn_WorkSettings))]
		[HarmonyPatch("WorkGiversInOrderNormal", 1)]
		public static class Pawn_WorkSettings_Patch
		{
			// Token: 0x060000A1 RID: 161 RVA: 0x0000AD58 File Offset: 0x00008F58
			[HarmonyPrefix]
			public static bool WorkGiversInOrderNormal_Pre(Pawn_WorkSettings __instance, ref List<WorkGiver> __result)
			{
				try
				{
					Pawn pawn = null;
					try
					{
						pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
					}
					catch
					{
					}
					if (pawn != null && pawn.RaceProps.Humanlike && ((pawn.timetable != null) ? ChildrenHarmony.Pawn_WorkSettings_Patch.SchoolCurrentAssignment(pawn) : TimeAssignmentDefOf.Anything) == MorePawnUtilities.TimeAssignmentDefOf.School)
					{
						List<KeyValuePair<WorkTypeDef, int>> list = new List<KeyValuePair<WorkTypeDef, int>>();
						List<WorkGiver> list2 = new List<WorkGiver>();
						List<WorkTypeDef> allDefsListForReading = DefDatabase<WorkTypeDef>.AllDefsListForReading;
						for (int i = 0; i < allDefsListForReading.Count; i++)
						{
							WorkTypeDef workTypeDef = allDefsListForReading[i];
							int num = __instance.GetPriority(workTypeDef);
							if (num > 0)
							{
								if (workTypeDef == SchoolDefOf.Teach || workTypeDef == SchoolDefOf.Study)
								{
									num += 3;
								}
								list.Add(new KeyValuePair<WorkTypeDef, int>(workTypeDef, num));
							}
						}
						list = (from wts in list
								orderby wts.Value descending
								select wts).ToList<KeyValuePair<WorkTypeDef, int>>();
						for (int j = 0; j < list.Count; j++)
						{
							WorkTypeDef key = list[j].Key;
							for (int k = 0; k < key.workGiversByPriority.Count; k++)
							{
								WorkGiver worker = key.workGiversByPriority[k].Worker;
								if (!worker.def.emergency)
								{
									list2.Add(worker);
								}
							}
						}
						__result = list2;
						return false;
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Pawn_WorkSettings edit in School: error: " + ex.Message);
					}
				}
				return true;
			}

			// Token: 0x060000A2 RID: 162 RVA: 0x0000AF1C File Offset: 0x0000911C
			public static TimeAssignmentDef SchoolCurrentAssignment(Pawn pawn)
			{
				if (!pawn.IsColonist)
				{
					return TimeAssignmentDefOf.Anything;
				}
				return pawn.timetable.times[GenLocalDate.HourOfDay(pawn)];
			}
		}

		// Token: 0x02000034 RID: 52
		[HarmonyPatch(typeof(FloatMenuMakerMap), "AddUndraftedOrders")]
		public static class FloatMenuMakerMap_Patch
		{
			// Token: 0x060000A3 RID: 163 RVA: 0x0000AF44 File Offset: 0x00009144
			[HarmonyPostfix]
			public static void AddUndraftedOrders_Post(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
			{
				try
				{
					foreach (Thing thing in pawn.Map.thingGrid.ThingsAt(IntVec3.FromVector3(clickPos)))
					{
						if (thing != null)
						{
							if (thing != null && thing.def.defName == "SchoolTable")
							{
								if (pawn.ageTracker.AgeBiologicalYears >= ChildrenController.Instance.GeneratedChildrenLearnMaxAge.Value)
								{
									opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("CannotGenericWork".Translate(SchoolDefOf.Study.verb, thing.LabelShort, thing) + " (" + WorkGiver_Study.PawnToOldSeeOptionsTranslated + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null), pawn, IntVec3.FromVector3(clickPos), "ReservedBy"));
								}
								else if (!MorePawnUtilities.PawnAndroidCheck(pawn) && !MorePawnUtilities.PawnDroidCheck(pawn))
								{
									if (pawn.ageTracker.AgeBiologicalYears < ChildrenController.Instance.GeneratedChildrenLearnMinAge.Value)
									{
										opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("CannotGenericWork".Translate(SchoolDefOf.Study.verb, thing.LabelShort, thing) + " (" + WorkGiver_Study.PawnToYoungSeeOptionsTranslated + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null), pawn, IntVec3.FromVector3(clickPos), "ReservedBy"));
									}
								}
								else if (pawn.ageTracker.AgeBiologicalYears < ChildrenController.Instance.GeneratedChildrenLearnMinAgeAndroid.Value)
								{
									opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("CannotGenericWork".Translate(SchoolDefOf.Study.verb, thing.LabelShort, thing) + " (" + WorkGiver_Study.PawnToYoungSeeOptionsTranslated + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null), pawn, IntVec3.FromVector3(clickPos), "ReservedBy"));
								}
							}
							else if (thing != null && thing.def.defName == "Blackboard" && pawn.ageTracker.AgeBiologicalYears < ChildrenController.Instance.GeneratedChildrenTeachMinAge.Value)
							{
								opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("CannotGenericWork".Translate(SchoolDefOf.Teach.verb, thing.LabelShort, thing) + " (" + WorkGiver_Study.PawnToYoungSeeOptionsTranslated + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null), pawn, IntVec3.FromVector3(clickPos), "ReservedBy"));
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("FloatMenuMakerMap show why failed: error: " + ex.Message);
					}
				}
			}
		}

		// Token: 0x02000035 RID: 53
		[HarmonyPatch(typeof(AreaManager), "AddStartingAreas")]
		public static class AreaManager_AddStartingAreas_Patch
		{
			// Token: 0x060000A4 RID: 164 RVA: 0x0000B2AC File Offset: 0x000094AC
			[HarmonyPostfix]
			public static void AddStartingAreas_Post(AreaManager __instance)
			{
				try
				{
					Traverse.Create(__instance).Field("areas").GetValue<List<Area>>().Add(new Area_School(__instance));
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("AddStartingAreas School: error: " + ex.Message);
					}
				}
			}
		}

		// Token: 0x02000036 RID: 54
		[HarmonyPatch(typeof(AreaAllowedGUI), "DoAreaSelector")]
		public static class AreaAllowedGUI_DoAreaSelector_Patch
		{
			// Token: 0x060000A5 RID: 165 RVA: 0x0000B30C File Offset: 0x0000950C
			[HarmonyPrefix]
			public static bool AreaAllowedGUI_Pre(Rect rect, Pawn p, Area area)
			{
				try
				{
					if (p != null && p.RaceProps.Humanlike && Area_School.GetPawnHourSchoolCache(p) == Area_School.BasicTriStateEnum.True)
					{
						Area_School orCreateSchoolAreaFromMap = Area_School.GetOrCreateSchoolAreaFromMap(p.Map);
						if (orCreateSchoolAreaFromMap == area && orCreateSchoolAreaFromMap.Map == p.MapHeld && p.playerSettings.RespectsAllowedArea)
						{
							ChildrenHarmony.AreaAllowedGUI_DoAreaSelector_Patch.DoAreaSelectorCustomBorder(rect, p, area);
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("DoAreaSelector School: error: " + ex.Message);
					}
				}
				return true;
			}

			// Token: 0x060000A6 RID: 166 RVA: 0x0000B398 File Offset: 0x00009598
			private static void DoAreaSelectorCustomBorder(Rect rect, Pawn p, Area area)
			{
				ChildrenHarmony.AreaAllowedGUI_DoAreaSelector_Patch.DrawBoxCustom(rect, BaseContent.YellowTex, 2);
				string str = "(" + "Children_SchoolTooltipOverride".Translate() + ")";
				TooltipHandler.TipRegion(rect, str);
			}

			// Token: 0x060000A7 RID: 167 RVA: 0x0000B3E4 File Offset: 0x000095E4
			private static void DrawBoxCustom(Rect rect, Texture texture, int thickness = 1)
			{
				Vector2 vector = new Vector2(rect.x, rect.y);
				Vector2 vector2 = new Vector2(rect.x + rect.width, rect.y + rect.height);
				if ((double)vector.x > (double)vector2.x)
				{
					float x = vector.x;
					vector.x = vector2.x;
					vector2.x = x;
				}
				if ((double)vector.y > (double)vector2.y)
				{
					float y = vector.y;
					vector.y = vector2.y;
					vector2.y = y;
				}
				Vector3 vector3 = vector2 - vector;
				GUI.DrawTexture(new Rect(vector.x, vector.y, (float)thickness, vector3.y), texture);
				GUI.DrawTexture(new Rect(vector2.x - (float)thickness, vector.y, (float)thickness, vector3.y), texture);
				GUI.DrawTexture(new Rect(vector.x + (float)thickness, vector.y, vector3.x - (float)(thickness * 2), (float)thickness), texture);
				GUI.DrawTexture(new Rect(vector.x + (float)thickness, vector2.y - (float)thickness, vector3.x - (float)(thickness * 2), (float)thickness), texture);
			}
		}

		// Token: 0x02000037 RID: 55
		[HarmonyPatch(typeof(Pawn_PlayerSettings))]
		[HarmonyPatch("EffectiveAreaRestrictionInPawnCurrentMap", 1)]
		public static class Pawn_PlayerSettings_EffectiveAreaRestrictionInPawnCurrentMap_Patch
		{
			// Token: 0x060000A8 RID: 168 RVA: 0x0000B524 File Offset: 0x00009724
			[HarmonyPrefix]
			public static bool EffectiveAreaRestrictionInPawnCurrentMap_Pre(Pawn_PlayerSettings __instance, ref Area __result)
			{
				try
				{
					Pawn pawn = null;
					KeyValuePair<bool, Pawn> pawn_PlayerSettingsPawnCache = Area_School.GetPawn_PlayerSettingsPawnCache(ref __instance);
					if (pawn_PlayerSettingsPawnCache.Key)
					{
						pawn = pawn_PlayerSettingsPawnCache.Value;
					}
					if (pawn != null && pawn.RaceProps.Humanlike && Area_School.GetPawnHourSchoolCache(pawn) == Area_School.BasicTriStateEnum.True)
					{
						Area_School orCreateSchoolAreaFromMap = Area_School.GetOrCreateSchoolAreaFromMap(pawn.Map);
						if (orCreateSchoolAreaFromMap.Map == pawn.MapHeld && pawn.playerSettings.RespectsAllowedArea)
						{
							__result = orCreateSchoolAreaFromMap;
							return false;
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("EffectiveAreaRestrictionInPawnCurrentMap School: error: " + ex.Message);
					}
				}
				return true;
			}
		}

		// Token: 0x02000038 RID: 56
		[HarmonyPatch(typeof(Pawn_PlayerSettings))]
		[HarmonyPatch("EffectiveAreaRestriction", 1)]
		public static class Pawn_PlayerSettings_EffectiveAreaRestrictionSchool_Patch
		{
			// Token: 0x060000A9 RID: 169 RVA: 0x0000B5CC File Offset: 0x000097CC
			[HarmonyPrefix]
			public static bool EffectiveAreaRestriction_Pre(Pawn_PlayerSettings __instance, ref Area __result)
			{
				try
				{
					if (__instance.RespectsAllowedArea)
					{
						Pawn pawn = null;
						KeyValuePair<bool, Pawn> pawn_PlayerSettingsPawnCache = Area_School.GetPawn_PlayerSettingsPawnCache(ref __instance);
						if (pawn_PlayerSettingsPawnCache.Key)
						{
							pawn = pawn_PlayerSettingsPawnCache.Value;
						}
						if (pawn != null && pawn.RaceProps.Humanlike && Area_School.GetPawnHourSchoolCache(pawn) == Area_School.BasicTriStateEnum.True)
						{
							__result = Area_School.GetOrCreateSchoolAreaFromMap(pawn.Map);
							return false;
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("EffectiveAreaRestriction School: error: " + ex.Message);
					}
				}
				return true;
			}
		}

		// Token: 0x02000039 RID: 57
		[HarmonyPatch(typeof(Pawn_PlayerSettings))]
		[HarmonyPatch("AreaRestriction", 1)]
		public static class Pawn_PlayerSettings_AreaRestrictioSchool_Patch
		{
			// Token: 0x060000AA RID: 170 RVA: 0x0000B65C File Offset: 0x0000985C
			[HarmonyPrefix]
			public static bool AreaRestriction_Pre(Pawn_PlayerSettings __instance, ref Area __result)
			{
				try
				{
					Pawn pawn = null;
					KeyValuePair<bool, Pawn> pawn_PlayerSettingsPawnCache = Area_School.GetPawn_PlayerSettingsPawnCache(ref __instance);
					if (pawn_PlayerSettingsPawnCache.Key)
					{
						pawn = pawn_PlayerSettingsPawnCache.Value;
					}
					if (pawn != null && pawn.RaceProps.Humanlike && Area_School.GetPawnHourSchoolCache(pawn) == Area_School.BasicTriStateEnum.True)
					{
						__result = Area_School.GetOrCreateSchoolAreaFromMap(pawn.Map);
						return false;
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("AreaRestriction School: error: " + ex.Message);
					}
				}
				return true;
			}
		}

		// Token: 0x0200003A RID: 58
		[HarmonyPatch(typeof(RestUtility), "FindBedFor")]
		[HarmonyPatch(new Type[]
		{
			typeof(Pawn),
			typeof(Pawn),
			typeof(bool),
			typeof(bool),
			typeof(bool)
		})]
		public static class RestUtility_FindBedFor_Patch
		{
			// Token: 0x060000AB RID: 171 RVA: 0x0000B6E4 File Offset: 0x000098E4
			[HarmonyPrefix]
			public static bool FindBedFor_Pre(Pawn sleeper, Pawn traveler)
			{
				try
				{
					Area_School.SetPawnHourSchoolCache(sleeper, Area_School.BasicTriStateEnum.Default);
					Area_School.SetPawnHourSchoolCache(traveler, Area_School.BasicTriStateEnum.Default);
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("GetBedSleepingSlotPosFor School: error: " + ex.Message);
					}
				}
				return true;
			}
		}

		// Token: 0x0200003B RID: 59
		[HarmonyPatch(typeof(SkillRecord), "Learn")]
		public static class SkillRecord_Patch
		{
			// Token: 0x060000AC RID: 172 RVA: 0x0000B734 File Offset: 0x00009934
			[HarmonyPrefix]
			public static bool SkillRecord_Learn_Pre(SkillRecord __instance, ref float xp, bool direct = false)
			{
				try
				{
					if (xp > 0f)
					{
						Pawn pawn = null;
						try
						{
							pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
						}
						catch
						{
						}
						if (pawn != null && __instance.def != null)
						{
							RecipeDef recipeDef = null;
							try
							{
								recipeDef = pawn.jobs.curJob.RecipeDef;
							}
							catch
							{
							}
							if (recipeDef != null && recipeDef.defName.StartsWith("LearnAtSchool"))
							{
								Job curJob = pawn.jobs.curJob;
								Building_SchoolTable building_SchoolTable = null;
								Pawn pawn2 = null;
								bool flag = false;
								bool flag2 = false;
								try
								{
									building_SchoolTable = (Building_SchoolTable)pawn.jobs.curJob.targetA.Thing;
								}
								catch
								{
								}
								if (building_SchoolTable != null && curJob != null)
								{
									if (building_SchoolTable.AffectedFacilities != null)
									{
										foreach (Thing thing in building_SchoolTable.AffectedFacilities.LinkedFacilitiesListForReading)
										{
											Building_Blackboard building_Blackboard = (Building_Blackboard)thing;
											building_Blackboard.GetComp<CompFacility>();
											building_Blackboard.GetComp<CompReportWorkSpeed>();
											bool flag3 = false;
											foreach (Pawn pawn3 in building_Blackboard.Map.mapPawns.FreeColonists)
											{
												if (building_Blackboard.InteractionCell == pawn3.Position)
												{
													flag3 = true;
													pawn2 = pawn3;
													break;
												}
											}
											if (flag3 && pawn2.CurJob.targetA == building_Blackboard && pawn2.CurJob.RecipeDef.defName.StartsWith("TeachAtSchool"))
											{
												if (curJob.bill.recipe.defName.Replace("LearnAtSchool", "") == pawn2.CurJob.RecipeDef.defName.Replace("TeachAtSchool", ""))
												{
													flag = true;
													break;
												}
												if (pawn2.CurJob.RecipeDef == SchoolRecipeDefOf.TeachAtSchoolGeneral)
												{
													flag = true;
													flag2 = true;
													break;
												}
											}
										}
									}
									if (!flag)
									{
										return false;
									}
									float num = xp;
									int num2 = 0;
									float num3;
									if (flag2)
									{
										num3 = xp;
									}
									else
									{
										num2 = pawn2.skills.GetSkill(__instance.def).Level - __instance.Level;
										if (num2 <= 0)
										{
											num3 = (float)Math.Round((double)(xp * 0.3f), 2);
											if (ChildrenController.Instance.NotLearningMessageEnabled.Value)
											{
												Messages.Message("ChildrenMessageNotLearningMuch".Translate(pawn2.LabelShort, pawn.LabelShort, __instance.def.label), pawn, MessageTypeDefOf.NegativeEvent, true);
											}
										}
										else if (num2 == 1)
										{
											num3 = xp;
										}
										else
										{
											num3 = (float)Math.Round((double)(xp * ((float)num2 * 0.65f)), 2);
										}
									}
									int level = pawn2.skills.GetSkill(SkillDefOf.Intellectual).Level;
									if (level == 0)
									{
										num3 = (float)Math.Round((double)(num3 * 0.3f), 2);
									}
									else if (level == 1)
									{
										num3 = (float)Math.Round((double)(num3 * 0.7f), 2);
									}
									else if (level == 2)
									{
										num3 = (float)Math.Round((double)(num3 * 0.9f), 2);
									}
									else if (level <= 5)
									{
										num3 = (float)Math.Round((double)(num3 * 1.05f), 2);
									}
									else if (level <= 10)
									{
										num3 = (float)Math.Round((double)(num3 * 1.15f), 2);
									}
									else
									{
										num3 = (float)Math.Round((double)(num3 * 1.25f), 2);
									}
									xp = num3;
									if (Prefs.DevMode)
									{
										CLog.Message(string.Concat(new object[]
										{
											"Pawn SkillRecord_Learn: learning! pawn: ",
											pawn.Name.ToString(),
											" teacher: ",
											pawn2.Name.ToString(),
											" intLevelTeacher: ",
											level,
											" skill: ",
											__instance.def.defName.ToString(),
											" xpPrior: ",
											num.ToString(),
											" xp: ",
											xp.ToString(),
											" job:",
											recipeDef.defName.ToString(),
											" diff:",
											num2.ToString()
										}));
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("SkillRecord_Learn: error: " + ex.Message);
					}
				}
				return true;
			}
		}

		// Token: 0x0200003C RID: 60
		[HarmonyPatch(typeof(Need_Food))]
		[HarmonyPatch("MaxLevel", 1)]
		public static class Need_Food_Patch
		{
			// Token: 0x060000AD RID: 173 RVA: 0x0000BC60 File Offset: 0x00009E60
			[HarmonyPrefix]
			public static bool MaxLevel_Pre(Need_Food __instance, ref float __result)
			{
				try
				{
					Pawn pawn = null;
					try
					{
						pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
					}
					catch
					{
					}
					if (pawn != null)
					{
						float foodMaxFactor = pawn.ageTracker.CurLifeStage.foodMaxFactor;
						bool flag = pawn.ageTracker.AgeBiologicalYears >= 13;
						__result = pawn.BodySize * foodMaxFactor;
						if (flag)
						{
							if (__result < 0.85f)
							{
								__result = 0.85f;
							}
						}
						else if (__result < 0.6f)
						{
							__result = 0.6f;
						}
						return false;
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Need_Food cap MaxLevel: error: " + ex.Message);
					}
				}
				return true;
			}
		}

		// Token: 0x0200003D RID: 61
		[HarmonyPatch(typeof(HediffWithComps))]
		[HarmonyPatch("Visible", 1)]
		public static class HediffWithComps_Preg_Patch
		{
			// Token: 0x060000AE RID: 174 RVA: 0x0000BD24 File Offset: 0x00009F24
			[HarmonyPrefix]
			public static bool Visible_Pre(HediffWithComps __instance, ref bool __result)
			{
				try
				{
					if (__instance.def == HediffDefOf.Pregnant)
					{
						Pawn pawn = null;
						try
						{
							pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
						}
						catch
						{
						}
						if (pawn != null && pawn.health.hediffSet.HasHediff(HediffDefOf.Pregnant, false))
						{
							if ((ChildrenController.Instance.AlwaysShowPregnancyEnabled.Value && pawn.RaceProps.Humanlike) || pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.PregnancyDiscovered, false))
							{
								__result = true;
								return false;
							}
							return true;
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("HediffWithComps Preg Visible: error: " + ex.Message);
					}
				}
				return true;
			}
		}

		// Token: 0x0200003E RID: 62
		[HarmonyPatch(typeof(HediffComp_Discoverable), "CheckDiscovered")]
		public static class HediffComp_Discoverable_CheckDiscovered_Patch
		{
			// Token: 0x060000AF RID: 175 RVA: 0x0000BDF8 File Offset: 0x00009FF8
			[HarmonyPrefix]
			public static bool CheckDiscovered_Pre(HediffComp_Discoverable __instance)
			{
				try
				{
					if (__instance.parent.def == HediffDefOf.Pregnant)
					{
						bool flag;
						try
						{
							flag = Traverse.Create(__instance).Field("discovered").GetValue<bool>();
						}
						catch
						{
							return true;
						}
						if (flag || (!__instance.parent.CurStage.becomeVisible && (!ChildrenController.Instance.AlwaysShowPregnancyEnabled.Value || !__instance.Pawn.RaceProps.Humanlike) && !__instance.Pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.PregnancyDiscovered, false)))
						{
							return false;
						}
						flag = true;
						try
						{
							Traverse.Create(__instance).Field("discovered").SetValue(flag);
						}
						catch
						{
							return true;
						}
						if (!__instance.Props.sendLetterWhenDiscovered || !PawnUtility.ShouldSendNotificationAbout(__instance.Pawn))
						{
							return false;
						}
						if (__instance.Pawn.RaceProps.Humanlike)
						{
							string str = __instance.Props.discoverLetterLabel.NullOrEmpty() ? ("LetterLabelNewDisease".Translate() + ": " + __instance.Def.LabelCap) : string.Format(__instance.Props.discoverLetterLabel, __instance.Pawn.LabelShortCap).CapitalizeFirst();
							string str2;
							if (!__instance.Props.discoverLetterText.NullOrEmpty())
							{
								str2 = __instance.Props.discoverLetterText.Formatted(__instance.Pawn.LabelIndefinite(), __instance.Pawn.Named("PAWN")).AdjustedFor(__instance.Pawn, "PAWN", true).CapitalizeFirst();
							}
							else if (__instance.parent.Part == null)
							{
								str2 = "NewDisease".Translate(__instance.Pawn.Named("PAWN"), __instance.Def.label, __instance.Pawn.LabelDefinite()).AdjustedFor(__instance.Pawn, "PAWN", true).CapitalizeFirst();
							}
							else
							{
								str2 = "NewPartDisease".Translate(__instance.Pawn.Named("PAWN"), __instance.parent.Part.Label, __instance.Pawn.LabelDefinite(), __instance.Def.label).AdjustedFor(__instance.Pawn, "PAWN", true).CapitalizeFirst();
							}
							Find.LetterStack.ReceiveLetter(str, str2, LetterDefOf.PositiveEvent, __instance.Pawn, null, null, null, null);
						}
						else
						{
							string text;
							if (!__instance.Props.discoverLetterText.NullOrEmpty())
							{
								string value = __instance.Pawn.KindLabelIndefinite();
								if (__instance.Pawn.Name.IsValid && !__instance.Pawn.Name.Numerical)
								{
									value = __instance.Pawn.Name.ToString() + " (" + __instance.Pawn.KindLabel + ")";
								}
								text = __instance.Props.discoverLetterText.Formatted(value, __instance.Pawn.Named("PAWN")).AdjustedFor(__instance.Pawn, "PAWN", true).CapitalizeFirst();
							}
							else
							{
								text = ((__instance.parent.Part != null) ? "NewPartDiseaseAnimal".Translate(__instance.Pawn.LabelShort, __instance.parent.Part.Label, __instance.Pawn.LabelDefinite(), __instance.Def.LabelCap, __instance.Pawn.Named("PAWN")).AdjustedFor(__instance.Pawn, "PAWN", true).CapitalizeFirst() : "NewDiseaseAnimal".Translate(__instance.Pawn.LabelShort, __instance.Def.LabelCap, __instance.Pawn.LabelDefinite(), __instance.Pawn.Named("PAWN")).AdjustedFor(__instance.Pawn, "PAWN", true).CapitalizeFirst());
							}
							Messages.Message(text, __instance.Pawn, MessageTypeDefOf.PositiveEvent, true);
						}
						return false;
					}
				}
				catch
				{
				}
				return true;
			}
		}

		// Token: 0x0200003F RID: 63
		[HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal")]
		[HarmonyPatch(new Type[]
		{
			typeof(Vector3),
			typeof(float),
			typeof(bool),
			typeof(Rot4),
			typeof(Rot4),
			typeof(RotDrawMode),
			typeof(bool),
			typeof(bool),
			typeof(bool)
		})]
		public static class PawnRenderer_RenderPawnInternal_Patch
		{
			// Token: 0x060000B0 RID: 176 RVA: 0x0000C304 File Offset: 0x0000A504
			[HarmonyPrefix]
			[HarmonyBefore(new string[]
			{
				"AlienRace",
				"FacialStuff"
			})]
			[HarmonyPriority(600)]
			public static bool PawnRenderer_RenderPawnInternal_Pre(PawnRenderer __instance, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
			{
				try
				{
					if (!ChildrenHarmony.ModBaC_ON && ChildrenController.Instance.PawnSizeEnabled.Value)
					{
						Pawn pawn = null;
						try
						{
							pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
						}
						catch
						{
						}
						if (pawn != null && pawn.ageTracker != null && pawn.ageTracker.CurLifeStage != null)
						{
							float num = pawn.ageTracker.CurLifeStage.bodySizeFactor;
							if (pawn.RaceProps.Humanlike && num != 1f && (!ChildrenHarmony.ModCaP_ON || pawn.kindDef.race.ToString().ToLower() != "human") && (!ChildrenHarmony.ModAlienChildrenCompatibility_ON || !(pawn.kindDef.race.ToString().ToLower() != "human")))
							{
								num = ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.GetBodysizeScaling(num, pawn);
								PawnWoundDrawer pawnWoundDrawer = null;
								try
								{
									pawnWoundDrawer = Traverse.Create(__instance).Field("woundOverlays").GetValue<PawnWoundDrawer>();
								}
								catch
								{
								}
								if (pawnWoundDrawer != null)
								{
									PawnHeadOverlays pawnHeadOverlays = null;
									try
									{
										pawnHeadOverlays = Traverse.Create(__instance).Field("statusOverlays").GetValue<PawnHeadOverlays>();
									}
									catch
									{
									}
									if (pawnHeadOverlays != null)
									{
										ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.RenderPawnInternalScaled(__instance, pawn, num, pawnWoundDrawer, pawnHeadOverlays, rootLoc, angle, renderBody, bodyFacing, headFacing, bodyDrawType, portrait, headStump, invisible);
										return false;
									}
								}
							}
						}
					}
				}
				catch
				{
				}
				return true;
			}

			// Token: 0x060000B1 RID: 177 RVA: 0x0000C4C0 File Offset: 0x0000A6C0
			private static GraphicMeshSet GetModifiedBodyMeshSet(float bodySizeFactor)
			{
				if (!ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeBodySetModified.ContainsKey(bodySizeFactor))
				{
					ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeBodySetModified.Add(bodySizeFactor, new GraphicMeshSet(1.5f * bodySizeFactor));
				}
				return ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeBodySetModified[bodySizeFactor];
			}

			// Token: 0x060000B2 RID: 178 RVA: 0x0000C4F1 File Offset: 0x0000A6F1
			private static GraphicMeshSet GetModifiedHeadMeshSet(float bodySizeFactor)
			{
				if (!ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHeadSetModified.ContainsKey(bodySizeFactor))
				{
					ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHeadSetModified.Add(bodySizeFactor, new GraphicMeshSet(1.5f * bodySizeFactor));
				}
				return ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHeadSetModified[bodySizeFactor];
			}

			// Token: 0x060000B3 RID: 179 RVA: 0x0000C524 File Offset: 0x0000A724
			private static GraphicMeshSet GetModifiedHairMeshSet(float bodySizeFactor, Pawn pawn)
			{
				if (pawn.story.crownType == CrownType.Average)
				{
					if (!ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHairAverageSetModified.ContainsKey(bodySizeFactor))
					{
						ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHairAverageSetModified.Add(bodySizeFactor, new GraphicMeshSet(1.5f * bodySizeFactor));
					}
					return ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHairAverageSetModified[bodySizeFactor];
				}
				if (pawn.story.crownType == CrownType.Narrow)
				{
					if (!ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHairNarrowSetModified.ContainsKey(bodySizeFactor))
					{
						ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHairNarrowSetModified.Add(bodySizeFactor, new GraphicMeshSet(1.3f * bodySizeFactor, 1.5f * bodySizeFactor));
					}
					return ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHairNarrowSetModified[bodySizeFactor];
				}
				if (!ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHairAverageSetModified.ContainsKey(bodySizeFactor))
				{
					ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHairAverageSetModified.Add(bodySizeFactor, new GraphicMeshSet(1.5f * bodySizeFactor));
				}
				return ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.humanlikeHairAverageSetModified[bodySizeFactor];
			}

			// Token: 0x060000B4 RID: 180 RVA: 0x0000C5E4 File Offset: 0x0000A7E4
			private static float GetBodysizeScaling(float bodySizeFactor, Pawn pawn)
			{
				float num = bodySizeFactor;
				float num2 = 1f;
				try
				{
					int curLifeStageIndex = pawn.ageTracker.CurLifeStageIndex;
					int num3 = pawn.RaceProps.lifeStageAges.Count - 1;
					LifeStageAge lifeStageAge = pawn.RaceProps.lifeStageAges[curLifeStageIndex];
					if (num3 == curLifeStageIndex && curLifeStageIndex != 0 && bodySizeFactor != 1f)
					{
						LifeStageAge lifeStageAge2 = pawn.RaceProps.lifeStageAges[curLifeStageIndex - 1];
						num = lifeStageAge2.def.bodySizeFactor + (float)Math.Round((double)((lifeStageAge.def.bodySizeFactor - lifeStageAge2.def.bodySizeFactor) / (lifeStageAge.minAge - lifeStageAge2.minAge) * (pawn.ageTracker.AgeBiologicalYearsFloat - lifeStageAge2.minAge)), 2);
					}
					else if (num3 == curLifeStageIndex)
					{
						num = bodySizeFactor;
					}
					else if (curLifeStageIndex == 0)
					{
						LifeStageAge lifeStageAge3 = pawn.RaceProps.lifeStageAges[curLifeStageIndex + 1];
						num = lifeStageAge.def.bodySizeFactor + (float)Math.Round((double)((lifeStageAge3.def.bodySizeFactor - lifeStageAge.def.bodySizeFactor) / (lifeStageAge3.minAge - lifeStageAge.minAge) * (pawn.ageTracker.AgeBiologicalYearsFloat - lifeStageAge.minAge)), 2);
					}
					else
					{
						LifeStageAge lifeStageAge3 = pawn.RaceProps.lifeStageAges[curLifeStageIndex + 1];
						num = lifeStageAge.def.bodySizeFactor + (float)Math.Round((double)((lifeStageAge3.def.bodySizeFactor - lifeStageAge.def.bodySizeFactor) / (lifeStageAge3.minAge - lifeStageAge.minAge) * (pawn.ageTracker.AgeBiologicalYearsFloat - lifeStageAge.minAge)), 2);
					}
					if (pawn.RaceProps.baseBodySize > 0f)
					{
						num2 = pawn.RaceProps.baseBodySize;
					}
				}
				catch
				{
				}
				return num * num2;
			}

			// Token: 0x060000B5 RID: 181 RVA: 0x0000C7CC File Offset: 0x0000A9CC
			private static void RenderPawnInternalScaled(PawnRenderer __instance, Pawn pawn, float bodySizeFactor, PawnWoundDrawer woundOverlays, PawnHeadOverlays statusOverlays, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
			{
				if (!__instance.graphics.AllResolved)
				{
					__instance.graphics.ResolveAllGraphics();
				}
				Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
				Mesh mesh = null;
				if (renderBody)
				{
					Vector3 loc = rootLoc;
					loc.y += 0.007575758f;
					if (bodyDrawType == RotDrawMode.Dessicated && !pawn.RaceProps.Humanlike && __instance.graphics.dessicatedGraphic != null && !portrait)
					{
						__instance.graphics.dessicatedGraphic.Draw(loc, bodyFacing, pawn, angle);
					}
					else
					{
						if (pawn.RaceProps.Humanlike)
						{
							mesh = ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.GetModifiedBodyMeshSet(bodySizeFactor).MeshAt(bodyFacing);
						}
						else
						{
							mesh = __instance.graphics.nakedGraphic.MeshAt(bodyFacing);
						}
						List<Material> list = __instance.graphics.MatsBodyBaseAt(bodyFacing, bodyDrawType);
						for (int i = 0; i < list.Count; i++)
						{
							Material mat = ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.OverrideMaterialIfNeeded(__instance, list[i], pawn);
							GenDraw.DrawMeshNowOrLater(mesh, loc, quaternion, mat, portrait);
							loc.y += 0.003787879f;
						}
						if (bodyDrawType == RotDrawMode.Fresh)
						{
							Vector3 drawLoc = rootLoc;
							drawLoc.y += 0.01893939f;
							woundOverlays.RenderOverBody(drawLoc, mesh, quaternion, portrait);
						}
					}
				}
				Vector3 vector = rootLoc;
				Vector3 a = rootLoc;
				if (bodyFacing != Rot4.North)
				{
					a.y += 0.02651515f;
					vector.y += 0.02272727f;
				}
				else
				{
					a.y += 0.02272727f;
					vector.y += 0.02651515f;
				}
				if (__instance.graphics.headGraphic != null)
				{
					Vector3 vector2 = quaternion * __instance.BaseHeadOffsetAt(headFacing);
					vector2.z *= bodySizeFactor;
					vector2.x *= bodySizeFactor;
					Material material = __instance.graphics.HeadMatAt(headFacing, bodyDrawType, headStump);
					if (material != null)
					{
						GenDraw.DrawMeshNowOrLater(ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.GetModifiedHeadMeshSet(bodySizeFactor).MeshAt(headFacing), a + vector2, quaternion, material, portrait);
					}
					Vector3 vector3 = rootLoc + vector2;
					vector3.y += 0.03030303f;
					bool flag = false;
					if (!portrait || !Prefs.HatsOnlyOnMap)
					{
						Mesh mesh2 = ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.GetModifiedHairMeshSet(bodySizeFactor, pawn).MeshAt(headFacing);
						List<ApparelGraphicRecord> apparelGraphics = __instance.graphics.apparelGraphics;
						for (int j = 0; j < apparelGraphics.Count; j++)
						{
							if (apparelGraphics[j].sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead)
							{
								if (!apparelGraphics[j].sourceApparel.def.apparel.hatRenderedFrontOfFace)
								{
									flag = true;
									Material mat2 = ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.OverrideMaterialIfNeeded(__instance, apparelGraphics[j].graphic.MatAt(bodyFacing, null), pawn);
									GenDraw.DrawMeshNowOrLater(mesh2, vector3, quaternion, mat2, portrait);
								}
								else
								{
									Material mat3 = ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.OverrideMaterialIfNeeded(__instance, apparelGraphics[j].graphic.MatAt(bodyFacing, null), pawn);
									Vector3 loc2 = rootLoc + vector2;
									loc2.y += ((bodyFacing == Rot4.North) ? 0.003787879f : 0.03409091f);
									GenDraw.DrawMeshNowOrLater(mesh2, loc2, quaternion, mat3, portrait);
								}
							}
						}
					}
					if (!flag && bodyDrawType != RotDrawMode.Dessicated && !headStump)
					{
						Mesh mesh3 = ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.GetModifiedHairMeshSet(bodySizeFactor, pawn).MeshAt(headFacing);
						Material material2 = __instance.graphics.HairMatAt(headFacing);
						Vector3 loc3 = vector3;
						Quaternion quat = quaternion;
						Material mat4 = material2;
						int num = portrait ? 1 : 0;
						GenDraw.DrawMeshNowOrLater(mesh3, loc3, quat, mat4, num != 0);
					}
				}
				if (renderBody)
				{
					for (int k = 0; k < __instance.graphics.apparelGraphics.Count; k++)
					{
						ApparelGraphicRecord apparelGraphicRecord = __instance.graphics.apparelGraphics[k];
						if (apparelGraphicRecord.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Shell)
						{
							Material mat5 = ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.OverrideMaterialIfNeeded(__instance, apparelGraphicRecord.graphic.MatAt(bodyFacing, null), pawn);
							GenDraw.DrawMeshNowOrLater(mesh, vector, quaternion, mat5, portrait);
						}
					}
				}
				if (!portrait && pawn.RaceProps.Animal && pawn.inventory != null && pawn.inventory.innerContainer.Count > 0 && __instance.graphics.packGraphic != null)
				{
					Graphics.DrawMesh(mesh, vector, quaternion, __instance.graphics.packGraphic.MatAt(bodyFacing, null), 0);
				}
				if (portrait)
				{
					return;
				}
				Traverse.Create(__instance).Method("DrawEquipment", new object[]
				{
					rootLoc
				}).GetValue();
				if (pawn.apparel != null)
				{
					List<Apparel> wornApparel = pawn.apparel.WornApparel;
					for (int l = 0; l < wornApparel.Count; l++)
					{
						wornApparel[l].DrawWornExtras();
					}
				}
				Vector3 bodyLoc = rootLoc;
				bodyLoc.y += 0.04166667f;
				statusOverlays.RenderStatusOverlays(bodyLoc, quaternion, ChildrenHarmony.PawnRenderer_RenderPawnInternal_Patch.GetModifiedHeadMeshSet(bodySizeFactor).MeshAt(headFacing));
			}

			// Token: 0x060000B6 RID: 182 RVA: 0x0000CCB7 File Offset: 0x0000AEB7
			private static Material OverrideMaterialIfNeeded(PawnRenderer PRinstance, Material original, Pawn pawn)
			{
				return PRinstance.graphics.flasher.GetDamagedMat(pawn.IsInvisible() ? InvisibilityMatPool.GetInvisibleMat(original) : original);
			}

			// Token: 0x040000BE RID: 190
			private static Dictionary<float, GraphicMeshSet> humanlikeBodySetModified = new Dictionary<float, GraphicMeshSet>();

			// Token: 0x040000BF RID: 191
			private static Dictionary<float, GraphicMeshSet> humanlikeHeadSetModified = new Dictionary<float, GraphicMeshSet>();

			// Token: 0x040000C0 RID: 192
			private static Dictionary<float, GraphicMeshSet> humanlikeHairAverageSetModified = new Dictionary<float, GraphicMeshSet>();

			// Token: 0x040000C1 RID: 193
			private static Dictionary<float, GraphicMeshSet> humanlikeHairNarrowSetModified = new Dictionary<float, GraphicMeshSet>();
		}

		// Token: 0x02000040 RID: 64
		[HarmonyPatch(typeof(RoomRoleWorker_Bedroom), "GetScore")]
		public static class RoomRoleWorker_Bedroom_GetScore_Patch
		{
			// Token: 0x060000B8 RID: 184 RVA: 0x0000CD04 File Offset: 0x0000AF04
			[HarmonyPrefix]
			public static bool GetScoreDone(RoomRoleWorker_Bedroom __instance, ref float __result, Room room)
			{
				try
				{
					int num = 0;
					int num2 = 0;
					bool flag = false;
					List<Pawn> list = new List<Pawn>();
					List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
					for (int i = 0; i < containedAndAdjacentThings.Count; i++)
					{
						Building_Bed building_Bed = containedAndAdjacentThings[i] as Building_Bed;
						if (building_Bed != null && building_Bed.def.building.bed_humanlike && !building_Bed.Medical && !building_Bed.ForPrisoners)
						{
							num++;
							if (i == 0)
							{
								num2++;
								list = building_Bed.CompAssignableToPawn.AssignedPawns.ToList<Pawn>();
							}
							else if (list == null || list.Count == 0)
							{
								list = building_Bed.CompAssignableToPawn.AssignedPawns.ToList<Pawn>();
								num2++;
							}
							else
							{
								List<Pawn> list2 = building_Bed.CompAssignableToPawn.AssignedPawns.ToList<Pawn>();
								if (list2 != null && list2.Count > 0 && list != null && list.Count > 0)
								{
									for (int j = 0; j < list2.Count; j++)
									{
										int num3 = 0;
										if (num3 < list.Count)
										{
											if (PawnRelationDefOf.Kin.Worker.InRelation(list[num3], list2[j]))
											{
												num2++;
												flag = true;
											}
											else if (list2[j].relations.DirectRelationExists(PawnRelationDefOf.Parent, list[num3]))
											{
												num2++;
												flag = true;
											}
											else if (list2[j].relations.DirectRelationExists(PawnRelationDefOf.Fiance, list[num3]))
											{
												num2++;
												flag = true;
											}
											else if (list2[j].relations.DirectRelationExists(PawnRelationDefOf.Spouse, list[num3]))
											{
												num2++;
												flag = true;
											}
											else
											{
												flag = true;
											}
										}
										if (flag)
										{
											break;
										}
									}
								}
								else
								{
									num2++;
								}
							}
						}
					}
					if (num == 1 && num == num2)
					{
						__result = 100000f;
					}
					else if (num > 1 && num == num2)
					{
						__result = (float)num * 100110f;
					}
					else
					{
						__result = 0f;
					}
					return false;
				}
				catch
				{
				}
				return true;
			}
		}

		// Token: 0x02000041 RID: 65
		[HarmonyPatch(typeof(Hediff_Pregnant), "DoBirthSpawn")]
		public static class Hediff_Pregnant_Patch2
		{
			// Token: 0x060000B9 RID: 185 RVA: 0x0000CF48 File Offset: 0x0000B148
			[HarmonyPrefix]
			public static bool Pawn_DoBirthSpawn_Pre(Pawn mother, Pawn father)
			{
				double num = (double)Rand.Value;
				int num2 = 0;
				bool flag = false;
				List<PawnGenerationRequest> list = new List<PawnGenerationRequest>();
				try
				{
					num = (double)Rand.Value;
					num2 = 0;
					if (mother != null && mother.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.PregnancyDiscovered, false))
					{
						mother.health.RemoveHediff(mother.health.hediffSet.GetFirstHediffOfDef(MorePawnUtilities.HediffDefOfMODDED.PregnancyDiscovered, false));
					}
					if (mother != null && mother.RaceProps.Humanlike)
					{
						if (ChildrenHarmony.ModCaP_ON && mother.kindDef.race.ToString().ToLower() == "human")
						{
							if (Prefs.DevMode)
							{
								CLog.Message(string.Concat(new object[]
								{
									"Children: Birth: C&P is active! Deactivating own Birth routines for Humans except age and skills. ",
									mother.Name,
									" - ",
									mother.kindDef.race.ToString()
								}));
							}
							return true;
						}
						if (Prefs.DevMode)
						{
							CLog.Message(string.Concat(new object[]
							{
								"Children: Birth: DoBirthSpawn. Mother: ",
								mother.Name,
								" - ",
								mother.kindDef.race.ToString()
							}));
						}
						try
						{
							PawnKindDef pawnKindDef2;
							if (father != null && ((mother.kindDef.race.ToString().ToLower() == "human" && father.kindDef.race.ToString().ToLower().Contains("lotre_elf")) || (father.kindDef.race.ToString().ToLower() == "human" && mother.kindDef.race.ToString().ToLower().Contains("lotre_elf"))))
							{
								ThingDef thingDef = (from t in DefDatabase<ThingDef>.AllDefsListForReading
													 where t.defName == "LotRE_ElfStandardRaceHalf"
													 select t).FirstOrDefault<ThingDef>();
								PawnKindDef pawnKindDef = (from t in DefDatabase<PawnKindDef>.AllDefsListForReading
														   where t.defName == "LotRE_HalfElfColonist"
														   select t).FirstOrDefault<PawnKindDef>();
								ThingDef thingDef2 = (thingDef == null || thingDef.defName == "") ? ChildrenHarmony.Hediff_Pregnant_Patch2.GetRealThingDefForBrokenPawn(father) : thingDef;
								pawnKindDef2 = ((pawnKindDef == null || pawnKindDef.defName == "") ? ChildrenHarmony.Hediff_Pregnant_Patch2.GetRealPawnKindDefForBrokenPawn(father, out flag) : pawnKindDef);
								num2 = ((thingDef2.race.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(thingDef2.race.litterSizeCurve)));
								if (num2 < 1)
								{
									num2 = 1;
								}
								if (num2 > 6)
								{
									num2 = 6;
								}
								for (int i = 0; i < num2; i++)
								{
									list.Add(new PawnGenerationRequest(pawnKindDef2, mother.Faction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false, false, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null));
								}
								if (Prefs.DevMode)
								{
									CLog.Message("Children: Birth: DoBirthSpawn: R HalfElf set for litter: " + pawnKindDef2.race.ToString() + " Value:" + num.ToString());
								}
							}
							else if (num <= 0.5 && father != null)
							{
								ThingDef thingDef2 = ChildrenHarmony.Hediff_Pregnant_Patch2.GetRealThingDefForBrokenPawn(father);
								pawnKindDef2 = ChildrenHarmony.Hediff_Pregnant_Patch2.GetRealPawnKindDefForBrokenPawn(father, out flag);
								num2 = ((thingDef2.race.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(thingDef2.race.litterSizeCurve)));
								if (num2 < 1)
								{
									num2 = 1;
								}
								if (num2 > 6)
								{
									num2 = 6;
								}
								for (int j = 0; j < num2; j++)
								{
									list.Add(new PawnGenerationRequest(pawnKindDef2, mother.Faction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false, false, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null));
								}
								if (Prefs.DevMode)
								{
									CLog.Message("Children: Birth: DoBirthSpawn: R father set for litter: " + pawnKindDef2.race.ToString() + " Value:" + num.ToString());
								}
							}
							else
							{
								ThingDef thingDef2 = ChildrenHarmony.Hediff_Pregnant_Patch2.GetRealThingDefForBrokenPawn(mother);
								pawnKindDef2 = ChildrenHarmony.Hediff_Pregnant_Patch2.GetRealPawnKindDefForBrokenPawn(mother, out flag);
								num2 = ((mother.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(mother.RaceProps.litterSizeCurve)));
								if (num2 < 1)
								{
									num2 = 1;
								}
								if (num2 > 6)
								{
									num2 = 6;
								}
								for (int k = 0; k < num2; k++)
								{
									list.Add(new PawnGenerationRequest(pawnKindDef2, mother.Faction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false, false, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null));
								}
								if (Prefs.DevMode)
								{
									CLog.Message("Children: Birth: DoBirthSpawn: R mother set for litter: " + pawnKindDef2.race.ToString() + " Value:" + num.ToString());
								}
							}
							if (ChildrenController.Instance.AlwaysShowChildSpeciesDialogEnabled.Value)
							{
								if (Prefs.DevMode)
								{
									CLog.Message("Children: Birth: DoBirthSpawn: Showing species dialog(always)!");
								}
								Find.WindowStack.Add(new ChildSpeciesDialog_Confirm((!flag) ? "" : "ChildrenMessageChooseSpeciesInfo".Translate().ToString(), mother, father, list, pawnKindDef2, new Action(ChildrenHarmony.Hediff_Pregnant_Patch2.OnSpeciesDialogConfirmed), false, "ChildrenMessageChooseSpecies".Translate(mother.Name.ToString())));
								return false;
							}
							if (ChildrenController.Instance.ShowChildSpeciesDialogIfUnclearEnabled.Value && flag)
							{
								if (Prefs.DevMode)
								{
									CLog.Message("Children: Birth: DoBirthSpawn: Showing species dialog(unclear)!");
								}
								Find.WindowStack.Add(new ChildSpeciesDialog_Confirm("ChildrenMessageChooseSpeciesInfo".Translate(), mother, father, list, pawnKindDef2, new Action(ChildrenHarmony.Hediff_Pregnant_Patch2.OnSpeciesDialogConfirmed), false, "ChildrenMessageChooseSpecies".Translate(mother.Name.ToString())));
								return false;
							}
							try
							{
								if (Prefs.DevMode)
								{
									CLog.Message("Children: Birth: DoBirthSpawn: Normal generation without species dialog!");
								}
								if (ChildrenHarmony.Hediff_Pregnant_Patch2.BirthByPawnGenerationRequests(mother, father, list))
								{
									return false;
								}
							}
							catch (Exception ex)
							{
								if (Prefs.DevMode)
								{
									ChdLogging.LogError(string.Concat(new object[]
									{
										"Hediff_Pregnant: DoBirthSpawn: error3: ",
										ex.Message,
										" num:",
										num2
									}));
								}
								return true;
							}
						}
						catch (Exception ex2)
						{
							if (Prefs.DevMode)
							{
								ChdLogging.LogError("Hediff_Pregnant: DoBirthSpawn: error2: " + ex2.Message);
							}
							return true;
						}
					}
				}
				catch (Exception ex3)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Hediff_Pregnant: DoBirthSpawn: error1: " + ex3.Message);
					}
					return true;
				}
				return true;
			}

			// Token: 0x060000BA RID: 186 RVA: 0x0000D6A0 File Offset: 0x0000B8A0
			private static void OnSpeciesDialogConfirmed()
			{
				try
				{
					if (Prefs.DevMode)
					{
						CLog.Message("Children: Birth: DoBirthSpawn: OnSpeciesDialogConfirmed!");
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Hediff_Pregnant: OnSpeciesDialogConfirmed: error: " + ex.Message);
					}
				}
			}

			// Token: 0x060000BB RID: 187 RVA: 0x0000D6F0 File Offset: 0x0000B8F0
			public static bool BirthByPawnGenerationRequests(Pawn mother, Pawn father, List<PawnGenerationRequest> requests)
			{
				bool result;
				try
				{
					Pawn pawn = null;
					bool flag = false;
					bool flag2 = true;
					int num = 0;
					foreach (PawnGenerationRequest request in requests)
					{
						num++;
						if (Prefs.DevMode)
						{
							CLog.Message("Children: Birth: DoBirthSpawn: num:(none) index:" + num);
						}
						pawn = PawnGenerator.GeneratePawn(request);
						if (Prefs.DevMode)
						{
							CLog.Message("Children: Birth: DoBirthSpawn: GeneratePawn done!");
						}
						try
						{
							pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, mother);
							if (father != null && father != mother)
							{
								pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, father);
							}
							flag2 = false;
						}
						catch (Exception ex)
						{
							if (Prefs.DevMode)
							{
								CLog.Message("Hediff_Pregnant: DoBirthSpawn: errorRelation: " + ex.Message);
							}
						}
						try
						{
							flag = PawnUtility.TrySpawnHatchedOrBornPawn(pawn, mother);
						}
						catch (Exception ex2)
						{
							if (Prefs.DevMode)
							{
								CLog.Message("Hediff_Pregnant: DoBirthSpawn: errorSpawn: " + ex2.Message + " -- num:(none)");
							}
							if (!flag && pawn.Spawned)
							{
								flag = true;
								if (Prefs.DevMode)
								{
									CLog.Message("Children: Birth: DoBirthSpawn: TrySpawnHatchedOrBornPawn! Fixed Result to true because pawn was spawned");
								}
							}
						}
						if (Prefs.DevMode)
						{
							CLog.Message("Children: Birth: DoBirthSpawn: TrySpawnHatchedOrBornPawn done! Result: " + flag.ToString());
						}
						if (flag)
						{
							if (pawn.playerSettings != null && mother.playerSettings != null)
							{
								pawn.playerSettings.AreaRestriction = mother.playerSettings.AreaRestriction;
							}
							if (flag2 && pawn.RaceProps.IsFlesh)
							{
								pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, mother);
								if (father != null && father != mother)
								{
									pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, father);
								}
							}
							if (request.KindDef != null && request.KindDef.race != null && pawn.kindDef != null && pawn.kindDef.race != null && request.KindDef.race.defName != pawn.kindDef.race.defName && Prefs.DevMode)
							{
								ChdLogging.LogError(string.Concat(new object[]
								{
									"Hediff_Pregnant: DoBirthSpawn: Race mismatch:  The colonist: ",
									pawn.Name,
									" should have been born with the race: ",
									request.KindDef.race.defName,
									"but was born with the race: ",
									pawn.kindDef.race.defName,
									" (some mod may have influenced this!)"
								}));
							}
						}
						else
						{
							Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
						}
						TaleRecorder.RecordTale(TaleDefOf.GaveBirth, new object[]
						{
							mother,
							pawn
						});
						if (pawn.caller != null)
						{
							pawn.caller.DoCall();
						}
					}
					if (!mother.Spawned)
					{
						result = true;
					}
					else
					{
						FilthMaker.TryMakeFilth(mother.Position, mother.Map, ThingDefOf.Filth_AmnioticFluid, mother.LabelIndefinite(), 5, FilthSourceFlags.None);
						if (mother.caller != null)
						{
							mother.caller.DoCall();
						}
						if (pawn.caller == null)
						{
							result = true;
						}
						else
						{
							pawn.caller.DoCall();
							result = true;
						}
					}
				}
				catch (Exception ex3)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("BirthByPawnGenerationRequests: error: " + ex3.Message);
					}
					result = false;
				}
				return result;
			}

			// Token: 0x060000BC RID: 188 RVA: 0x0000DA78 File Offset: 0x0000BC78
			public static ThingDef GetRealThingDefForBrokenPawn(Pawn pawn)
			{
				bool flag;
				return ChildrenHarmony.Hediff_Pregnant_Patch2.GetRealThingDefForBrokenPawn(pawn, out flag);
			}

			// Token: 0x060000BD RID: 189 RVA: 0x0000DA90 File Offset: 0x0000BC90
			public static ThingDef GetRealThingDefForBrokenPawn(Pawn pawn, out bool speciesUnclear)
			{
				speciesUnclear = false;
				try
				{
					if (pawn.RaceProps.Humanlike && pawn.kindDef != null && pawn.kindDef.race != null && pawn.RaceProps != null && pawn.RaceProps.body != null)
					{
						string text = pawn.kindDef.race.ToString();
						string text2 = (pawn.def.race.AnyPawnKind != null) ? pawn.def.race.AnyPawnKind.defName.ToString() : "";
						string text3 = (pawn.def.race.AnyPawnKind != null) ? pawn.def.race.AnyPawnKind.race.ToString() : "";
						string text4 = pawn.RaceProps.body.ToString();
						if (Prefs.DevMode)
						{
							CLog.Message(string.Concat(new object[]
							{
								"Children: ",
								pawn.Name,
								": r-thing fix: ",
								text,
								" / ",
								text4,
								" KD: ",
								pawn.kindDef.ToString(),
								" AKD: ",
								text2,
								" AKR: ",
								text3
							}));
						}
						if (text.ToLower().StartsWith("human"))
						{
							if (text3 != "" && text3.ToLower() != "human" && pawn.def.race.AnyPawnKind != pawn.kindDef)
							{
								List<ThingDef> allDefsListForReading = DefDatabase<ThingDef>.AllDefsListForReading;
								for (int i = 0; i < allDefsListForReading.Count; i++)
								{
									if (allDefsListForReading[i] != null && allDefsListForReading[i].race != null && allDefsListForReading[i].race.Humanlike && allDefsListForReading[i].defName.ToString().ToLower() == text3.ToLower())
									{
										if (Prefs.DevMode)
										{
											CLog.Message(string.Concat(new object[]
											{
												"Children: ",
												pawn.Name,
												": r-thing1 fix: Human to ",
												allDefsListForReading[i].defName.ToString()
											}));
										}
										return allDefsListForReading[i];
									}
								}
								speciesUnclear = true;
							}
							if (!text4.ToLower().StartsWith("human"))
							{
								speciesUnclear = true;
							}
						}
					}
				}
				catch
				{
				}
				return pawn.def;
			}

			// Token: 0x060000BE RID: 190 RVA: 0x0000DD4C File Offset: 0x0000BF4C
			public static PawnKindDef GetRealPawnKindDefForBrokenPawn(Pawn pawn)
			{
				bool flag;
				return ChildrenHarmony.Hediff_Pregnant_Patch2.GetRealPawnKindDefForBrokenPawn(pawn, out flag);
			}

			// Token: 0x060000BF RID: 191 RVA: 0x0000DD64 File Offset: 0x0000BF64
			public static PawnKindDef GetRealPawnKindDefForBrokenPawn(Pawn pawn, out bool speciesUnclear)
			{
				speciesUnclear = false;
				try
				{
					if (pawn.RaceProps.Humanlike && pawn.kindDef != null && pawn.kindDef.race != null && pawn.RaceProps != null && pawn.RaceProps.body != null)
					{
						string text = pawn.kindDef.race.ToString();
						if (pawn.def.race.AnyPawnKind != null)
						{
							pawn.def.race.AnyPawnKind.defName.ToString();
						}
						string text2 = (pawn.def.race.AnyPawnKind != null) ? pawn.def.race.AnyPawnKind.race.ToString() : "";
						string text3 = pawn.RaceProps.body.ToString();
						if (text.ToLower().StartsWith("human"))
						{
							if (text2 != "" && text2.ToLower() != "human" && pawn.def.race.AnyPawnKind != pawn.kindDef)
							{
								List<PawnKindDef> allDefsListForReading = DefDatabase<PawnKindDef>.AllDefsListForReading;
								for (int i = 0; i < allDefsListForReading.Count; i++)
								{
									if (allDefsListForReading[i] != null && allDefsListForReading[i].race != null && allDefsListForReading[i].race.race != null && allDefsListForReading[i].race.race.Humanlike && allDefsListForReading[i].race.defName.ToString().ToLower() == text2.ToLower())
									{
										if (Prefs.DevMode)
										{
											CLog.Message(string.Concat(new object[]
											{
												"Children: ",
												pawn.Name,
												": r-kind1 fix: Human to ",
												allDefsListForReading[i].race.defName.ToString()
											}));
										}
										return allDefsListForReading[i];
									}
								}
								speciesUnclear = true;
							}
							if (!text3.ToLower().StartsWith("human"))
							{
								speciesUnclear = true;
							}
						}
					}
				}
				catch
				{
				}
				return pawn.kindDef;
			}
		}

		// Token: 0x02000042 RID: 66
		[HarmonyPatch(typeof(Dialog_NamePawn), "Dialog_NamePawn", 3)]
		[HarmonyPatch(new Type[]
		{
			typeof(Pawn)
		})]
		public static class Dialog_NamePawn_PatchLoad
		{
			// Token: 0x060000C0 RID: 192 RVA: 0x0000DFB8 File Offset: 0x0000C1B8
			[HarmonyPostfix]
			public static void ConstructorExtended(ref string ___curName, Pawn pawn)
			{
				try
				{
					if (pawn != null && pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.ChildRenamePending, false))
					{
						NameTriple nameTriple = pawn.Name as NameTriple;
						___curName = nameTriple.First;
						if (Prefs.DevMode)
						{
							CLog.Message("Children: Rename2: Post!");
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Children: Rename2: error: " + ex.Message);
					}
				}
			}
		}

		// Token: 0x02000043 RID: 67
		[HarmonyPatch(typeof(Dialog_NamePawn), "DoWindowContents")]
		public static class Dialog_NamePawn_PatchDL
		{
			// Token: 0x060000C1 RID: 193 RVA: 0x0000E038 File Offset: 0x0000C238
			[HarmonyPrefix]
			public static bool DoWindowContentsExtended(Dialog_NamePawn __instance, Pawn ___pawn, ref string ___curTitle, ref string ___curName, Rect inRect)
			{
				try
				{
					if (___pawn != null && (___pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.ChildRenamePending, false) || (___pawn.RaceProps.Humanlike && ChildrenController.Instance.AlwaysRenameFirstNameEnabled.Value)))
					{
						bool flag = false;
						if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
						{
							flag = true;
							Event.current.Use();
						}
						Text.Font = GameFont.Medium;
						string text = "Set initial first name for: " + Environment.NewLine + ChildrenHarmony.Dialog_NamePawn_PatchDL.CurPawnNameWrite(___pawn, ___curTitle, ___curName).ToString().Replace(" '' ", " ");
						if (___curTitle == "")
						{
							text = text + ", " + ___pawn.story.TitleDefaultCap;
						}
						else if (___curTitle != null)
						{
							text = text + ", " + ___curTitle.CapitalizeFirst();
						}
						Widgets.Label(new Rect(15f, 15f, 500f, 100f), text);
						Text.Font = GameFont.Small;
						string text2 = Widgets.TextField(new Rect(15f, 100f, (float)((double)inRect.width / 2.0 - 20.0), 35f), ___curName);
						if (text2.Length < 16 && CharacterCardUtility.ValidNameRegex.IsMatch(text2))
						{
							___curName = text2;
						}
						if (!Widgets.ButtonText(new Rect((float)((double)inRect.width / 2.0 + 20.0), inRect.height - 35f, (float)((double)inRect.width / 2.0 - 20.0), 35f), "OK", true, true, true) && !flag)
						{
							return false;
						}
						if (string.IsNullOrEmpty(___curName))
						{
							___curName = ((NameTriple)___pawn.Name).First;
						}
						___pawn.Name = ChildrenHarmony.Dialog_NamePawn_PatchDL.CurPawnNameWrite(___pawn, ___curTitle, ___curName);
						if (___pawn.story != null)
						{
							___pawn.story.Title = ___curTitle;
						}
						Find.WindowStack.TryRemove(__instance, true);
						Messages.Message(___pawn.def.race.Animal ? "AnimalGainsName".Translate(___curName) : "PawnGainsName".Translate(___curName, ___pawn.story.Title, ___pawn.Named("PAWN")).AdjustedFor(___pawn, "PAWN", true).Replace("nickname", "name"), ___pawn, MessageTypeDefOf.PositiveEvent, false);
						if (___pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.ChildRenamePending, false))
						{
							___pawn.health.RemoveHediff(___pawn.health.hediffSet.GetFirstHediffOfDef(MorePawnUtilities.HediffDefOfMODDED.ChildRenamePending, false));
						}
						return false;
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Children: Rename: error: " + ex.Message);
					}
				}
				return true;
			}

			// Token: 0x060000C2 RID: 194 RVA: 0x0000E35C File Offset: 0x0000C55C
			private static Name CurPawnName(Pawn pawn, string curTitle, string curName)
			{
				NameTriple nameTriple = pawn.Name as NameTriple;
				if (nameTriple != null)
				{
					return new NameTriple(nameTriple.First, curName, nameTriple.Last);
				}
				if (pawn.Name is NameSingle)
				{
					return new NameSingle(curName, false);
				}
				throw new InvalidOperationException();
			}

			// Token: 0x060000C3 RID: 195 RVA: 0x0000E3A8 File Offset: 0x0000C5A8
			private static Name CurPawnNameWrite(Pawn pawn, string curTitle, string curName)
			{
				NameTriple nameTriple = pawn.Name as NameTriple;
				if (nameTriple != null)
				{
					return new NameTriple(curName, nameTriple.Nick, nameTriple.Last);
				}
				if (pawn.Name is NameSingle)
				{
					return new NameSingle(curName, false);
				}
				throw new InvalidOperationException();
			}
		}

		// Token: 0x02000044 RID: 68
		[HarmonyPatch(typeof(Pawn), "SetFaction")]
		public static class Pawn_SetFaction_Patch
		{
			// Token: 0x060000C4 RID: 196 RVA: 0x0000E3F4 File Offset: 0x0000C5F4
			[HarmonyPostfix]
			public static void SetFactionExtended(Pawn __instance, Faction newFaction, Pawn recruiter = null)
			{
				try
				{
					if (__instance != null && __instance.Spawned && __instance.IsColonist)
					{
						if (Prefs.DevMode)
						{
							CLog.Message("Children: SetFactionExtended: Player Faction Colonist! Name: " + __instance.Name + " InitOtherPawnStuffForPawn!");
						}
						ChildrenController.InitOtherPawnStuffForPawn(__instance, false);
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Children: SetFactionExtended: error: " + ex.Message);
					}
				}
			}
		}

		// Token: 0x02000045 RID: 69
		[HarmonyPatch(typeof(Thing), "Ingested")]
		public static class Thing_Ingested_Patch
		{
			// Token: 0x060000C5 RID: 197 RVA: 0x0000E470 File Offset: 0x0000C670
			[HarmonyPrefix]
			public static bool IngestedExtended(Thing __instance, Pawn ingester, float nutritionWanted)
			{
				try
				{
					if (__instance != null && !__instance.Destroyed && __instance.IngestibleNow && __instance.def != null && ingester != null && __instance.def.defName == "PregnancyTest")
					{
						if (Prefs.DevMode)
						{
							CLog.Message("Children: Ingested: PregnancyTest! Name: " + ingester.Name);
						}
						if (ingester.gender != Gender.Male && ingester.health.hediffSet.HasHediff(HediffDefOf.Pregnant, false))
						{
							ChildrenController.PregnancyInitGiveMoods(ingester);
							if (Prefs.DevMode)
							{
								CLog.Message("Children: Ingested_PregnancyTest: Pawn is Pregnant! Pawn:" + ingester.Name);
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Children: IngestedExtended: error: " + ex.Message);
					}
				}
				return true;
			}
		}

		// Token: 0x02000046 RID: 70
		[HarmonyPatch(typeof(PawnUtility), "TrySpawnHatchedOrBornPawn")]
		public static class PawnUtility_Patch
		{
			// Token: 0x060000C6 RID: 198 RVA: 0x0000E54C File Offset: 0x0000C74C
			[HarmonyPostfix]
			public static void TrySpawnHatchedOrBornPawnExtended(ref bool __result, Pawn pawn, Thing motherOrEgg)
			{
				bool flag = false;
				int num = 15;
				Hediff hediff = null;
				Pawn pawn2 = null;
				Pawn pawn3 = null;
				System.Random random = new System.Random();
				if (Prefs.DevMode)
				{
					CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn v1.5.5");
				}
				if (__result)
				{
					if (Prefs.DevMode)
					{
						CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn: OK");
						CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn: newborn: " + pawn);
					}
					try
					{
						if (!pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.ChildBirthInProgress, false))
						{
							pawn.health.AddHediff(MorePawnUtilities.HediffDefOfMODDED.ChildBirthInProgress, null, null, null);
							if (Prefs.DevMode)
							{
								CLog.Message("Children: Birth in progress added!");
							}
						}
					}
					catch (Exception ex)
					{
						if (Prefs.DevMode)
						{
							ChdLogging.LogError("Children: Birth in progress error: " + ex.Message);
						}
					}
					pawn2 = (motherOrEgg as Pawn);
					if (pawn2 == null && motherOrEgg.ParentHolder != null)
					{
						Pawn_InventoryTracker pawn_InventoryTracker = motherOrEgg.ParentHolder as Pawn_InventoryTracker;
						if (pawn_InventoryTracker != null)
						{
							pawn2 = pawn_InventoryTracker.pawn;
						}
					}
					if (pawn2 != null)
					{
						if (Prefs.DevMode)
						{
							CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn: mother: " + pawn2);
						}
						if (pawn2.health.hediffSet.HasHediff(HediffDefOf.Pregnant, false))
						{
							Hediff_Pregnant hediff_Pregnant = (Hediff_Pregnant)pawn2.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant, false);
							if (hediff_Pregnant != null && hediff_Pregnant.father != pawn2)
							{
								pawn3 = hediff_Pregnant.father;
								if (Prefs.DevMode)
								{
									CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn: hediff father: " + pawn3);
								}
							}
							else if (MorePawnUtilities.GetFatherKin(pawn) != null && MorePawnUtilities.GetFatherKin(pawn) != pawn2)
							{
								pawn3 = MorePawnUtilities.GetFatherKin(pawn);
								if (Prefs.DevMode)
								{
									CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn: relation father: " + pawn3);
								}
							}
							else if (Prefs.DevMode)
							{
								CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn: NO father! WP");
							}
						}
						else if (MorePawnUtilities.GetFatherKin(pawn) != null && MorePawnUtilities.GetFatherKin(pawn) != pawn2)
						{
							pawn3 = MorePawnUtilities.GetFatherKin(pawn);
							if (Prefs.DevMode)
							{
								CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn: relation father: " + pawn3);
							}
						}
						else if (Prefs.DevMode)
						{
							CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn: NO father! NP");
						}
						if (pawn.RaceProps.Humanlike)
						{
							num = ChildrenHarmony.PawnUtility_Patch.GetChildAge(pawn, false);
							if (!pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.ChildRenamePending, false))
							{
								pawn.ageTracker.AgeBiologicalTicks = (long)((double)num * 3600000.0);
								pawn.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
								try
								{
									if (!pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.ChildRenamePending, false))
									{
										pawn.health.AddHediff(MorePawnUtilities.HediffDefOfMODDED.ChildRenamePending, null, null, null);
										if (Prefs.DevMode)
										{
											CLog.Message("Children: Birth: Rename ability added!");
										}
									}
								}
								catch (Exception ex2)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Rename ability error: " + ex2.Message);
									}
								}
								try
								{
									List<Trait> list = new List<Trait>();
									if (pawn3 != null)
									{
										if (Prefs.DevMode)
										{
											CLog.Message("Children: Birth: Both Parents traits!");
										}
									}
									else if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: Only Mother traits!");
									}
									if (ChildrenController.Instance.GeneratedChildrenInheritTraits.Value != ChildrenController.GeneratedChildrenInheritTraitsHandleEnum._RandomTraits)
									{
										if (ChildrenController.Instance.GeneratedChildrenInheritTraits.Value == ChildrenController.GeneratedChildrenInheritTraitsHandleEnum._NoTraits)
										{
											pawn.story.traits.allTraits.Clear();
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Traits cleared");
											}
										}
										else if (ChildrenController.Instance.GeneratedChildrenInheritTraits.Value == ChildrenController.GeneratedChildrenInheritTraitsHandleEnum._InheritedTraits || ChildrenController.Instance.GeneratedChildrenInheritTraits.Value == ChildrenController.GeneratedChildrenInheritTraitsHandleEnum._InheritedAndRandomTraits)
										{
											if (ChildrenController.Instance.GeneratedChildrenInheritTraits.Value == ChildrenController.GeneratedChildrenInheritTraitsHandleEnum._InheritedTraits)
											{
												pawn.story.traits.allTraits.Clear();
												if (Prefs.DevMode)
												{
													CLog.Message("Children: Traits cleared for parent inherit");
												}
											}
											int num2 = 0;
											if (pawn3 != null && pawn3 != pawn2 && pawn3.story.traits.allTraits.Count > 0)
											{
												if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._all)
												{
													num2 = pawn3.story.traits.allTraits.Count;
												}
												else if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._1each || ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._1father)
												{
													num2 = 1;
												}
												else if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._2each || ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._2father)
												{
													num2 = 2;
												}
												else if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._3each || ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._3father)
												{
													num2 = 3;
												}
												if (num2 > 0)
												{
													if (Prefs.DevMode)
													{
														CLog.Message("Children: Father traits: " + num2);
													}
													List<Trait> list2 = new List<Trait>();
													for (int i = 0; i < pawn3.story.traits.allTraits.Count; i++)
													{
														list2.Add(pawn3.story.traits.allTraits[i]);
													}
													int num3 = 0;
													for (int j = 0; j < num2; j++)
													{
														int num4;
														if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._all)
														{
															num4 = j - num3;
														}
														else
														{
															num4 = random.Next(0, list2.Count);
														}
														if (list2.Count > num4 && !pawn.story.traits.HasTrait(list2[num4].def))
														{
															list.Add(new Trait(list2[num4].def, list2[num4].Degree, false));
															if (Prefs.DevMode)
															{
																CLog.Message("Children: Father trait added to list: " + list2[num4].def.defName);
															}
														}
														else if (list2.Count > num4)
														{
															if (Prefs.DevMode)
															{
																CLog.Message("Children: Father trait skipped: " + list2[num4].def.defName);
															}
														}
														else if (list2.Count == 0)
														{
															if (Prefs.DevMode)
															{
																CLog.Message("Children: Father traits none left! ");
															}
														}
														else if (Prefs.DevMode)
														{
															CLog.Message(string.Concat(new object[]
															{
																"Children: Index problem: ",
																num4,
																"/",
																list2.Count
															}));
														}
														if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value != ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._all && list2.Count > num4)
														{
															list2.RemoveAt(num4);
															num3++;
															if (Prefs.DevMode)
															{
																CLog.Message("Children: Trait List Remove");
															}
														}
													}
												}
											}
											num2 = 0;
											if (pawn2.story.traits.allTraits.Count > 0)
											{
												if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._all)
												{
													num2 = pawn2.story.traits.allTraits.Count;
												}
												else if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._1each || ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._1mother)
												{
													num2 = 1;
												}
												else if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._2each || ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._2mother)
												{
													num2 = 2;
												}
												else if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._3each || ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._3mother)
												{
													num2 = 3;
												}
												if (num2 > 0)
												{
													if (Prefs.DevMode)
													{
														CLog.Message("Children: Mother traits: " + num2);
													}
													List<Trait> list2 = new List<Trait>();
													for (int k = 0; k < pawn2.story.traits.allTraits.Count; k++)
													{
														list2.Add(pawn2.story.traits.allTraits[k]);
													}
													int num3 = 0;
													for (int l = 0; l < num2; l++)
													{
														int num4;
														if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value == ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._all)
														{
															num4 = l - num3;
														}
														else
														{
															num4 = random.Next(0, list2.Count);
														}
														if (list2.Count > num4 && !pawn.story.traits.HasTrait(list2[num4].def))
														{
															list.Add(new Trait(list2[num4].def, list2[num4].Degree, false));
															if (Prefs.DevMode)
															{
																CLog.Message("Children: Mother trait added to list: " + list2[num4].def.defName);
															}
														}
														else if (list2.Count > num4)
														{
															if (Prefs.DevMode)
															{
																CLog.Message("Children: Mother trait skipped: " + list2[num4].def.defName);
															}
														}
														else if (list2.Count == 0)
														{
															if (Prefs.DevMode)
															{
																CLog.Message("Children: Mother traits none left! ");
															}
														}
														else if (Prefs.DevMode)
														{
															CLog.Message(string.Concat(new object[]
															{
																"Children: Index problem: ",
																num4,
																"/",
																list2.Count
															}));
														}
														if (ChildrenController.Instance.GeneratedChildrenInheritTraitCount.Value != ChildrenController.GeneratedChildrenInheritTraitCountHandleEnum._all && list2.Count > num4)
														{
															list2.RemoveAt(num4);
															num3++;
															if (Prefs.DevMode)
															{
																CLog.Message("Children: Trait List Remove");
															}
														}
													}
												}
											}
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Parent traits added");
											}
										}
										if (list != null && list.Count > 0)
										{
											foreach (Trait trait in list.InRandomOrder(null))
											{
												if (trait != null && !pawn.story.traits.HasTrait(trait.def))
												{
													pawn.story.traits.GainTrait(trait);
													if (Prefs.DevMode)
													{
														CLog.Message("Children: Trait added: " + trait.def.defName);
													}
												}
												else if (trait != null && Prefs.DevMode)
												{
													CLog.Message("Children: Trait skipped: " + trait.def.defName);
												}
											}
										}
										if (Prefs.DevMode)
										{
											CLog.Message("Children: Birth: Traits set");
										}
									}
									else if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: Traits set random");
									}
									if (ChildrenController.Instance.GeneratedChildrenTraitScenario.Value == ChildrenController.GeneratedChildrenTraitScenarioHandleEnum._Inherit)
									{
										if (pawn3 != null)
										{
											ChildrenHarmony.PawnUtility_Patch.SetScenarioTraits(pawn, pawn2, pawn3);
										}
										else
										{
											ChildrenHarmony.PawnUtility_Patch.SetScenarioTraits(pawn, pawn2, null);
										}
									}
									if (ChildrenController.Instance.GeneratedChildrenTraitMax.Value != ChildrenController.GeneratedChildrenTraitMaxHandleEnum._NoLimit)
									{
										int num5 = 0;
										if (ChildrenController.Instance.GeneratedChildrenTraitMax.Value == ChildrenController.GeneratedChildrenTraitMaxHandleEnum._One)
										{
											num5 = 1;
										}
										else if (ChildrenController.Instance.GeneratedChildrenTraitMax.Value == ChildrenController.GeneratedChildrenTraitMaxHandleEnum._Two)
										{
											num5 = 2;
										}
										else if (ChildrenController.Instance.GeneratedChildrenTraitMax.Value == ChildrenController.GeneratedChildrenTraitMaxHandleEnum._Three)
										{
											num5 = 3;
										}
										else if (ChildrenController.Instance.GeneratedChildrenTraitMax.Value == ChildrenController.GeneratedChildrenTraitMaxHandleEnum._Four)
										{
											num5 = 4;
										}
										else if (ChildrenController.Instance.GeneratedChildrenTraitMax.Value == ChildrenController.GeneratedChildrenTraitMaxHandleEnum._Five)
										{
											num5 = 5;
										}
										if (num5 < pawn.story.traits.allTraits.Count)
										{
											if (Prefs.DevMode)
											{
												CLog.Message(string.Concat(new object[]
												{
													"Children: Birth: Traits: Too many! (has:",
													pawn.story.traits.allTraits.Count,
													" max:",
													num5,
													")"
												}));
											}
											for (int m = 0; m < pawn.story.traits.allTraits.Count - num5; m++)
											{
												int num4 = random.Next(0, pawn.story.traits.allTraits.Count);
												if (pawn.story.traits.allTraits.Count > num4)
												{
													if (Prefs.DevMode)
													{
														CLog.Message("Children: Birth: Traits: Too many! removing:" + pawn.story.traits.allTraits[num4].def.defName);
													}
													pawn.story.traits.allTraits.RemoveAt(num4);
												}
											}
										}
									}
									if (ChildrenController.Instance.GeneratedChildrenTraitScenario.Value == ChildrenController.GeneratedChildrenTraitScenarioHandleEnum._OnTop)
									{
										ChildrenHarmony.PawnUtility_Patch.SetScenarioTraits(pawn, null, null);
									}
									else if (ChildrenController.Instance.GeneratedChildrenTraitScenario.Value == ChildrenController.GeneratedChildrenTraitScenarioHandleEnum._InheritOnTop)
									{
										if (pawn3 != null)
										{
											ChildrenHarmony.PawnUtility_Patch.SetScenarioTraits(pawn, pawn2, pawn3);
										}
										else
										{
											ChildrenHarmony.PawnUtility_Patch.SetScenarioTraits(pawn, pawn2, null);
										}
									}
								}
								catch (Exception ex3)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Traits set error: " + ex3.Message);
									}
								}
								MorePawnUtilities.SetPawnSkillsAndPassions(pawn, pawn2, pawn3, "Birth", false, false);
								try
								{
									if (pawn.health.hediffSet.hediffs != null)
									{
										List<Hediff> list3 = new List<Hediff>();
										for (int n = 0; n < pawn.health.hediffSet.hediffs.Count; n++)
										{
											if (pawn.health.hediffSet.hediffs[n].def.addedPartProps != null && pawn.health.hediffSet.hediffs[n].def.addedPartProps.partEfficiency > 0f)
											{
												list3.Add(pawn.health.hediffSet.hediffs[n]);
											}
										}
										for (int num6 = 0; num6 < list3.Count; num6++)
										{
											pawn.health.RemoveHediff(list3[num6]);
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: implant removed: " + list3[num6]);
											}
										}
									}
								}
								catch (Exception ex4)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Implant purge error: " + ex4.Message);
									}
								}
								try
								{
									if (pawn.RaceProps != null && pawn.RaceProps.body != null)
									{
										for (int num7 = 0; num7 < pawn.RaceProps.body.AllParts.Count; num7++)
										{
											pawn.health.RestorePart(pawn.RaceProps.body.AllParts[num7], null, true);
										}
									}
									if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: Fix Up Prostetics&BodyParts done");
									}
								}
								catch (Exception ex5)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Fix Up Prostetics&BodyParts error: " + ex5.Message);
									}
								}
								try
								{
									if (pawn.health.hediffSet.hediffs != null)
									{
										List<Hediff> list4 = new List<Hediff>();
										List<Hediff> list5 = new List<Hediff>();
										try
										{
											for (int num8 = 0; num8 < pawn.health.hediffSet.hediffs.Count; num8++)
											{
												if (pawn.health.hediffSet.hediffs[num8].def.IsAddiction)
												{
													bool flag2 = true;
													if ((Hediff_Pregnant)pawn2.health.hediffSet.GetFirstHediffOfDef(pawn.health.hediffSet.hediffs[num8].def, false) != null)
													{
														flag2 = false;
														if (Prefs.DevMode)
														{
															CLog.Message("Children: Birth: Mother also has addiction, not removing: " + pawn.health.hediffSet.hediffs[num8]);
														}
													}
													if (flag2)
													{
														list4.Add(pawn.health.hediffSet.hediffs[num8]);
														hediff = pawn.health.hediffSet.hediffs[num8];
													}
												}
												else if (hediff != null && pawn.health.hediffSet.hediffs[num8].def.defName.ToLower().Contains(hediff.def.defName.Split(new char[]
												{
													Convert.ToChar(32)
												})[0].ToLower()))
												{
													if (Prefs.DevMode)
													{
														CLog.Message("Children: Birth: related addiction stuff found: " + pawn.health.hediffSet.hediffs[num8]);
													}
													list4.Add(pawn.health.hediffSet.hediffs[num8]);
												}
												else
												{
													hediff = null;
												}
												if (pawn.health.hediffSet.hediffs[num8].def.defName.ToLower().Contains("tolerance"))
												{
													if (Prefs.DevMode)
													{
														CLog.Message("Children: Birth: tolerance stuff found: " + pawn.health.hediffSet.hediffs[num8]);
													}
													list5.Add(pawn.health.hediffSet.hediffs[num8]);
												}
											}
										}
										catch (Exception ex6)
										{
											if (Prefs.DevMode)
											{
												ChdLogging.LogError("Children: Birth: Remove addictions error1: " + ex6.Message);
											}
										}
										for (int num9 = 0; num9 < list4.Count; num9++)
										{
											pawn.health.RemoveHediff(list4[num9]);
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: addiction removed: " + list4[num9]);
											}
										}
										for (int num10 = 0; num10 < list5.Count; num10++)
										{
											pawn.health.RemoveHediff(list5[num10]);
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: tolerance removed: " + list5[num10]);
											}
										}
									}
								}
								catch (Exception ex7)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Remove addictions error2: " + ex7.Message);
									}
								}
								try
								{
									if (pawn2.IsPrisonerInPrisonCell())
									{
										pawn.guest.SetGuestStatus(pawn2.guest.HostFaction, true);
										if (Prefs.DevMode)
										{
											CLog.Message("Children: Birth: Prisoner!");
										}
									}
								}
								catch (Exception ex8)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Prisoner error: " + ex8.Message);
									}
								}
								try
								{
									if (ChildrenController.Instance.PostpartumEnabled.Value)
									{
										if (pawn2.gender == Gender.Female)
										{
											if (!pawn2.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.Postpartum, false))
											{
												pawn2.health.AddHediff(MorePawnUtilities.HediffDefOfMODDED.Postpartum, null, null, null);
												if (Prefs.DevMode)
												{
													CLog.Message("Children: Birth: m: Postpartum added!");
												}
											}
										}
										else if (pawn3 != null && pawn3.gender == Gender.Female && !pawn3.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.Postpartum, false))
										{
											pawn3.health.AddHediff(MorePawnUtilities.HediffDefOfMODDED.Postpartum, null, null, null);
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: f: Postpartum added!");
											}
										}
									}
								}
								catch (Exception ex9)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Postpartum error: " + ex9.Message);
									}
								}
								try
								{
									if (ChildrenController.Instance.OneChildEnabled.Value)
									{
										if (pawn2.gender == Gender.Female)
										{
											if (!pawn2.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.OneChildOnly, false))
											{
												pawn2.health.AddHediff(MorePawnUtilities.HediffDefOfMODDED.OneChildOnly, null, null, null);
												if (Prefs.DevMode)
												{
													CLog.Message("Children: Birth: m: OneChild added!");
												}
											}
										}
										else if (pawn3 != null && pawn3.gender == Gender.Female && !pawn3.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.OneChildOnly, false))
										{
											pawn3.health.AddHediff(MorePawnUtilities.HediffDefOfMODDED.OneChildOnly, null, null, null);
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: f: OneChild added!");
											}
										}
									}
								}
								catch (Exception ex10)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: OneChild error: " + ex10.Message);
									}
								}
								try
								{
									if (ChildrenHarmony.ModCaP_ON && pawn2.kindDef.race.ToString().ToLower() == "human")
									{
										if (Prefs.DevMode)
										{
											CLog.Message(string.Concat(new object[]
											{
												"Children: Birth: C&P is active! Deactivating own Birth routines for Humans except age and skills. ",
												pawn2.Name,
												" - ",
												pawn2.kindDef.race.ToString()
											}));
										}
										return;
									}
								}
								catch (Exception ex11)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: C&P check error: " + ex11.Message);
									}
								}
								try
								{
									if (ChildrenHarmony.ModBaC_ON && Prefs.DevMode)
									{
										CLog.Message(string.Concat(new object[]
										{
											"Children: Birth: B&C is active! Deactivating scaling. ",
											pawn2.Name,
											" - ",
											pawn2.kindDef.race.ToString()
										}));
									}
								}
								catch (Exception ex12)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: B&C check error: " + ex12.Message);
									}
								}
								try
								{
									NameTriple nameTriple = (NameTriple)pawn2.Name;
									if (pawn3 != null)
									{
										NameTriple nameTriple2 = (NameTriple)pawn3.Name;
										if (ChildrenController.Instance.GeneratedChildrenLastName.Value == ChildrenController.GeneratedChildrenLastNameHandleEnum._NameFather)
										{
											pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, nameTriple2.Last);
										}
										else
										{
											pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, nameTriple.Last);
										}
									}
									else
									{
										pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, nameTriple.Last);
									}
									if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: Name set: " + pawn.Name);
									}
								}
								catch (Exception ex13)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Name error: " + ex13.Message);
									}
								}
								try
								{
									if (!pawn.kindDef.race.ToString().ToLower().Contains("leeani"))
									{
										if (pawn.kindDef.race.ToString().ToLower().Contains("human"))
										{
											double num11 = (double)Rand.Value;
											if (num11 < 0.2)
											{
												pawn.story.bodyType = BodyTypeDefOf.Thin;
											}
											else if (num11 < 0.7 && pawn3 != null)
											{
												pawn.story.bodyType = ((pawn.gender != Gender.Female) ? pawn3.story.bodyType : pawn2.story.bodyType);
											}
											else
											{
												pawn.story.bodyType = ((pawn.gender != Gender.Female) ? BodyTypeDefOf.Male : BodyTypeDefOf.Female);
											}
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: Bodytype set to " + pawn.story.bodyType.ToString());
											}
										}
										else if (pawn.kindDef.race.ToString().ToLower().Contains("lotrd_dwarf"))
										{
											pawn.story.bodyType = ((pawn.gender != Gender.Female) ? BodyTypeDefOf.Male : BodyTypeDefOf.Female);
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: Bodytype set to " + pawn.story.bodyType.ToString() + " (Dwarf)");
											}
										}
										else if (pawn.kindDef.race.ToString().ToLower().Contains("lotre_elf"))
										{
											pawn.story.bodyType = ((pawn.gender != Gender.Female) ? BodyTypeDefOf.Male : BodyTypeDefOf.Female);
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: Bodytype set to " + pawn.story.bodyType.ToString() + " (Elf)");
											}
										}
										else if (pawn.kindDef.race.ToString().ToLower().Contains("lotre_halfelf"))
										{
											pawn.story.bodyType = ((pawn.gender != Gender.Female) ? BodyTypeDefOf.Male : BodyTypeDefOf.Female);
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: Bodytype set to " + pawn.story.bodyType.ToString() + " (HalfElf)");
											}
										}
										else if (pawn.kindDef.race.ToString().ToLower().Contains("lotrh_hobbit"))
										{
											pawn.story.bodyType = ((pawn.gender != Gender.Female) ? BodyTypeDefOf.Male : BodyTypeDefOf.Female);
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: Bodytype set to " + pawn.story.bodyType.ToString() + " (Hobbit)");
											}
										}
										else if (Prefs.DevMode)
										{
											CLog.Message("Children: Birth: Bodytype NOT set (AlienRace)");
										}
									}
									else if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: Bodytype NOT set (Leeani)");
									}
								}
								catch (Exception ex14)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Bodytype error: " + ex14.Message);
									}
								}
								try
								{
									if ((double)Rand.Value <= 0.5 && pawn3 != null)
									{
										pawn.story.crownType = pawn3.story.crownType;
									}
									else
									{
										pawn.story.crownType = pawn2.story.crownType;
									}
									if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: CrownType set");
									}
								}
								catch (Exception ex15)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: CrownType error: " + ex15.Message);
									}
								}
								try
								{
									Color color = ChildrenHarmony.PawnUtility_Patch.MixColors(pawn2.story.hairColor, pawn3 != null, (pawn3 != null) ? pawn3.story.hairColor : pawn2.story.hairColor);
									pawn.story.hairColor.r = color.r;
									pawn.story.hairColor.g = color.g;
									pawn.story.hairColor.b = color.b;
									if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: Hair color set");
									}
								}
								catch (Exception ex16)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Hair color error: " + ex16.Message);
									}
								}
								try
								{
									pawn.story.melanin = pawn2.story.melanin;
									if (pawn3 != null)
									{
										pawn.story.melanin = (pawn.story.melanin + pawn3.story.melanin) / 2f;
										if (Prefs.DevMode)
										{
											CLog.Message(string.Concat(new string[]
											{
												"Children: Birth: M set: ",
												pawn.story.melanin.ToString(),
												" Mother:",
												pawn2.story.melanin.ToString(),
												" Father:",
												pawn3.story.melanin.ToString()
											}));
										}
									}
									else if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: M set: " + pawn.story.melanin.ToString() + " Mother:" + pawn2.story.melanin.ToString());
									}
								}
								catch (Exception ex17)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: M error: " + ex17.Message);
									}
								}
								if (Prefs.DevMode)
								{
									CLog.Message("Children: Birth: R set to: " + pawn.kindDef.race.ToString());
								}
								try
								{
									if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: AlienRace? Special color settings:");
									}
									ThingComp thingComp = null;
									ThingComp thingComp2 = null;
									ThingComp thingComp3 = null;
									bool flag3 = true;
									bool flag4 = true;
									foreach (ThingComp thingComp4 in pawn.GetComps<ThingComp>())
									{
										if (thingComp4.GetType().Namespace.ToLower() == "alienrace" && thingComp4.GetType().Name.ToLower() == "aliencomp")
										{
											thingComp = thingComp4;
											break;
										}
									}
									foreach (ThingComp thingComp5 in pawn2.GetComps<ThingComp>())
									{
										if (thingComp5.GetType().Namespace.ToLower() == "alienrace" && thingComp5.GetType().Name.ToLower() == "aliencomp")
										{
											thingComp2 = thingComp5;
											break;
										}
									}
									if (pawn3 != null)
									{
										foreach (ThingComp thingComp6 in pawn3.GetComps<ThingComp>())
										{
											if (thingComp6.GetType().Namespace.ToLower() == "alienrace" && thingComp6.GetType().Name.ToLower() == "aliencomp")
											{
												thingComp3 = thingComp6;
												break;
											}
										}
									}
									if (thingComp != null)
									{
										FieldInfo field = thingComp.GetType().GetField("skinColor");
										Color color2;
										if (thingComp2 != null)
										{
											color2 = (Color)thingComp2.GetType().GetField("skinColor").GetValue(thingComp2);
										}
										else
										{
											color2 = PawnSkinColors.GetSkinColor(pawn2.story.melanin);
											flag3 = false;
										}
										Color fatherColor;
										if (pawn3 != null && thingComp3 != null)
										{
											fatherColor = (Color)thingComp3.GetType().GetField("skinColor").GetValue(thingComp3);
										}
										else if (pawn3 != null)
										{
											fatherColor = PawnSkinColors.GetSkinColor(pawn3.story.melanin);
											flag4 = false;
										}
										else
										{
											fatherColor = color2;
											flag4 = flag3;
										}
										if (flag3 || flag4)
										{
											Color color3 = (Color)field.GetValue(thingComp);
											Color color4 = ChildrenHarmony.PawnUtility_Patch.MixColors(color2, true, fatherColor);
											color3.r = color4.r;
											color3.g = color4.g;
											color3.b = color4.b;
											field.SetValue(thingComp, color3);
											FieldInfo field2 = thingComp.GetType().GetField("skinColor");
											if (thingComp2 != null)
											{
												color2 = (Color)thingComp2.GetType().GetField("skinColor").GetValue(thingComp2);
											}
											else
											{
												color2 = PawnSkinColors.GetSkinColor(pawn2.story.melanin);
											}
											if (pawn3 != null && thingComp3 != null)
											{
												fatherColor = (Color)thingComp3.GetType().GetField("skinColor").GetValue(thingComp3);
											}
											else if (pawn3 != null)
											{
												fatherColor = PawnSkinColors.GetSkinColor(pawn3.story.melanin);
											}
											else
											{
												fatherColor = color2;
											}
											color3 = (Color)field2.GetValue(thingComp);
											color4 = ChildrenHarmony.PawnUtility_Patch.MixColors(color2, true, fatherColor);
											color3.r = color4.r;
											color3.g = color4.g;
											color3.b = color4.b;
											field2.SetValue(thingComp, color3);
											FieldInfo field3 = thingComp.GetType().GetField("skinColor");
											if (thingComp2 != null)
											{
												color2 = (Color)thingComp2.GetType().GetField("skinColor").GetValue(thingComp2);
											}
											else
											{
												color2 = pawn2.story.hairColor;
											}
											if (pawn3 != null && thingComp3 != null)
											{
												fatherColor = (Color)thingComp3.GetType().GetField("skinColor").GetValue(thingComp3);
											}
											else if (pawn3 != null)
											{
												fatherColor = pawn3.story.hairColor;
											}
											else
											{
												fatherColor = color2;
											}
											color3 = (Color)field3.GetValue(thingComp);
											color4 = ChildrenHarmony.PawnUtility_Patch.MixColors(color2, true, fatherColor);
											color3.r = color4.r;
											color3.g = color4.g;
											color3.b = color4.b;
											field3.SetValue(thingComp, color3);
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: AlienRace detected! Special color settings set!");
											}
										}
										else if (Prefs.DevMode)
										{
											CLog.Message("Children: Birth: AlienRace detected! Special color settings skipped!");
										}
									}
									else if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: NO AlienRace!");
									}
								}
								catch (Exception ex18)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: AlienRace color: " + ex18.Message + ex18.StackTrace);
									}
								}
								try
								{
									if (pawn.kindDef.race.ToString().ToLower().Contains("ferian"))
									{
										if (Prefs.DevMode)
										{
											CLog.Message("Children: Birth: Ferian detected! Special settings:");
										}
										pawn.story.bodyType = BodyTypeDefOf.Thin;
										if (pawn.gender == Gender.Female)
										{
											pawn.story.hairDef = pawn2.story.hairDef;
										}
										else if (pawn.gender == Gender.Male && pawn3 != null)
										{
											pawn.story.hairDef = pawn3.story.hairDef;
										}
										if (Prefs.DevMode)
										{
											CLog.Message("Children: Birth: Ferian detected! Special settings set!");
										}
									}
								}
								catch
								{
								}
								try
								{
									if (ChildrenController.Instance.VampireChildrenAreVampires.Value && ChildrenHarmony.ModRoM_Vampires_ON)
									{
										bool flag5 = false;
										HediffDef VampireCurseHediffDef = HediffDef.Named("ROM_Vampirism");
										if (VampireCurseHediffDef != null && pawn.health.hediffSet.HasHediff(VampireCurseHediffDef, false))
										{
											if (Prefs.DevMode)
											{
												CLog.Message("Birth: Pawn is Vampire by chance (" + VampireCurseHediffDef.defName + "), should not be possible!");
											}
											flag5 = true;
										}
										if (!flag5)
										{
											VampireCurseHediffDef = HediffDef.Named("ROM_Vampirism");
											if (VampireCurseHediffDef != null && pawn2.health.hediffSet.HasHediff(VampireCurseHediffDef, false))
											{
												string tipStringExtra = (from t in pawn2.health.hediffSet.hediffs
																		 where t.def != null && t.def.defName == VampireCurseHediffDef.defName
																		 select t).FirstOrDefault<Hediff>().TipStringExtra;
												if (Prefs.DevMode)
												{
													CLog.Message("Birth: Mother is a vampire (" + VampireCurseHediffDef.defName + "), Vampire curse inherited: " + tipStringExtra);
												}
												if (tipStringExtra.Contains("Gargoyle"))
												{
													pawn.health.AddHediff(HediffDef.Named("ROM_VampirismGargoyle"), null, null, null);
												}
												else if (tipStringExtra.Contains("Lasombra"))
												{
													pawn.health.AddHediff(HediffDef.Named("ROM_VampirismLasombra"), null, null, null);
												}
												else if (tipStringExtra.Contains("Tzimisce"))
												{
													pawn.health.AddHediff(HediffDef.Named("ROM_VampirismTzimisce"), null, null, null);
												}
												else if (tipStringExtra.Contains("Tremere"))
												{
													pawn.health.AddHediff(HediffDef.Named("ROM_VampirismTremere"), null, null, null);
												}
												else if (tipStringExtra.Contains("Pijavica"))
												{
													pawn.health.AddHediff(HediffDef.Named("ROM_VampirismPijavica"), null, null, null);
												}
												else
												{
													pawn.health.AddHediff(VampireCurseHediffDef, null, null, null);
												}
											}
											else if (VampireCurseHediffDef != null && pawn3 != null && pawn3.health.hediffSet.HasHediff(VampireCurseHediffDef, false))
											{
												string tipStringExtra2 = (from t in pawn3.health.hediffSet.hediffs
																		  where t.def != null && t.def.defName == VampireCurseHediffDef.defName
																		  select t).FirstOrDefault<Hediff>().TipStringExtra;
												if (Prefs.DevMode)
												{
													CLog.Message("Birth: Father is a vampire (" + VampireCurseHediffDef.defName + "), Vampire curse inherited: " + tipStringExtra2);
												}
												if (tipStringExtra2.Contains("Gargoyle"))
												{
													pawn.health.AddHediff(HediffDef.Named("ROM_VampirismGargoyle"), null, null, null);
												}
												else if (tipStringExtra2.Contains("Lasombra"))
												{
													pawn.health.AddHediff(HediffDef.Named("ROM_VampirismLasombra"), null, null, null);
												}
												else if (tipStringExtra2.Contains("Tzimisce"))
												{
													pawn.health.AddHediff(HediffDef.Named("ROM_VampirismTzimisce"), null, null, null);
												}
												else if (tipStringExtra2.Contains("Tremere"))
												{
													pawn.health.AddHediff(HediffDef.Named("ROM_VampirismTremere"), null, null, null);
												}
												else if (tipStringExtra2.Contains("Pijavica"))
												{
													pawn.health.AddHediff(HediffDef.Named("ROM_VampirismPijavica"), null, null, null);
												}
												else
												{
													pawn.health.AddHediff(VampireCurseHediffDef, null, null, null);
												}
											}
										}
									}
								}
								catch
								{
								}
								try
								{
									if (ChildrenHarmony.ModRoM_Werewolves_ON)
									{
										TraitDef traitDef = TraitDef.Named("ROM_Werewolf");
										try
										{
											if (traitDef != null && pawn.story != null && pawn.story.traits != null && pawn.story.traits.HasTrait(traitDef))
											{
												Trait trait2 = pawn.story.traits.GetTrait(traitDef);
												pawn.story.traits.allTraits.Remove(trait2);
											}
										}
										catch
										{
										}
										if (ChildrenController.Instance.WerewolfChildrenAreWerewolfs.Value)
										{
											bool flag6 = false;
											if (traitDef != null && pawn2.story != null && pawn2.story.traits != null && pawn2.story.traits.HasTrait(traitDef))
											{
												if (Prefs.DevMode)
												{
													CLog.Message("Birth: Mother is a werewolf (" + traitDef.defName + "), Lycanthropy inherited: ");
												}
												flag6 = true;
											}
											if (traitDef != null && pawn3 != null && pawn3.story != null && pawn3.story.traits != null && pawn3.story.traits.HasTrait(traitDef))
											{
												if (Prefs.DevMode)
												{
													CLog.Message("Birth: Father is a werewolf (" + traitDef.defName + "), Lycanthropy inherited: ");
												}
												flag6 = true;
											}
											if (flag6)
											{
												if ((double)num >= (double)ChildrenController.Instance.WerewolfChildrenMinAgeFirstTransformation.Value)
												{
													Trait trait2 = new Trait(traitDef, -1, false);
													pawn.story.traits.GainTrait(trait2);
												}
												else
												{
													Trait trait2 = new Trait(MorePawnUtilities.TraitDefOfMODDED.ChildrenWerewolfChild, 0, false);
													pawn.story.traits.GainTrait(trait2);
												}
											}
										}
									}
								}
								catch
								{
								}
								try
								{
									string text = "";
									if (ChildrenController.Instance.RandomBackstoryEnabled.Value)
									{
										double num12 = (double)Rand.Value;
										if (num12 <= 0.2)
										{
											text = "";
										}
										else if (num12 <= 0.3)
										{
											text = "GunKid57";
										}
										else if (num12 <= 0.4)
										{
											text = "ComaChild18";
										}
										else if (num12 <= 0.5)
										{
											text = "FrightenedChild6";
										}
										else if (num12 <= 0.6)
										{
											text = "Pyromaniac68";
										}
										else if (num12 <= 0.7)
										{
											text = "MusicLover26";
										}
										else if (num12 <= 0.8)
										{
											text = "CaveChild31";
										}
										else if (num12 <= 0.9)
										{
											text = "Nerd92";
										}
										else if (num12 <= 1.0)
										{
											text = "SpoiledChild32";
										}
									}
									else
									{
										text = "CHDR_ColonyChild";
										try
										{
											string text2 = pawn.kindDef.race.ToString();
											Backstory backstory = null;
											if (Prefs.DevMode)
											{
												CLog.Message(string.Concat(new string[]
												{
													"Children: Birth: FindCustomBackstory: Trying to find custom backstory for: ",
													text2,
													" with name: ",
													text2,
													"_Child_Colony_NewbornCSL"
												}));
											}
											text2 = (text2 + "_Child_Colony_NewbornCSL").ToLower();
											foreach (KeyValuePair<string, Backstory> keyValuePair in BackstoryDatabase.allBackstories)
											{
												if (keyValuePair.Key.ToLower() == text2)
												{
													BackstoryDatabase.TryGetWithIdentifier(keyValuePair.Key, out backstory, true);
													if (backstory == null)
													{
														break;
													}
													text = keyValuePair.Key;
													flag = true;
													if (Prefs.DevMode)
													{
														CLog.Message("Children: Birth: FindCustomBackstory: Found backstory: " + backstory.identifier + " - " + backstory.title);
														break;
													}
													break;
												}
											}
											if (backstory == null && Prefs.DevMode)
											{
												CLog.Message("Children: Birth: FindCustomBackstory: No custom backstory found with name: " + text2);
											}
										}
										catch (Exception ex19)
										{
											if (Prefs.DevMode)
											{
												ChdLogging.LogError("Children: Birth: FindCustomBackstory: error: " + ex19.Message);
											}
										}
									}
									Backstory childhood;
									if (text != "" && BackstoryDatabase.TryGetWithIdentifier(text, out childhood, true))
									{
										pawn.story.childhood = childhood;
										if (Prefs.DevMode)
										{
											CLog.Message("Children: Birth: Backstory set to: " + text);
										}
									}
									else if (!ChildrenController.Instance.RandomBackstoryEnabled.Value)
									{
										pawn.story.childhood = null;
										if (Prefs.DevMode)
										{
											CLog.Message("Children: Birth: Backstory set to nothing");
										}
									}
									else if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: Backstory not changed");
									}
									ChildrenController.NaturalColonyChildCheckAndMod(pawn);
									try
									{
										if (pawn.story.childhood != null && flag)
										{
											if (Prefs.DevMode)
											{
												CLog.Message("Children: Birth: BackstorySkillBonus: Custom backstory loaded");
											}
											Dictionary<SkillDef, int> skillGainsResolved = pawn.story.childhood.skillGainsResolved;
											if (skillGainsResolved != null && skillGainsResolved.Count > 0)
											{
												if (Prefs.DevMode)
												{
													CLog.Message("Children: Birth: BackstorySkillBonus: Custom backstory has skillGains");
												}
												List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
												foreach (KeyValuePair<SkillDef, int> keyValuePair2 in skillGainsResolved)
												{
													SkillRecord skill = pawn.skills.GetSkill(keyValuePair2.Key);
													int num13 = skill.Level;
													if (Prefs.DevMode)
													{
														CLog.Message(string.Concat(new string[]
														{
															"Children: Birth: BackstorySkillBonus: ",
															keyValuePair2.Key.defName,
															" from ",
															num13.ToString(),
															" to ",
															(num13 + keyValuePair2.Value).ToString()
														}));
													}
													num13 += keyValuePair2.Value;
													if (num13 < 0)
													{
														num13 = 0;
													}
													if (!skill.TotallyDisabled)
													{
														skill.Level = num13;
														skill.xpSinceLastLevel = Rand.Range(skill.XpRequiredForLevelUp * 0.1f, skill.XpRequiredForLevelUp * 0.9f);
													}
												}
											}
										}
									}
									catch (Exception ex20)
									{
										if (Prefs.DevMode)
										{
											ChdLogging.LogError("Children: Birth: BackstorySkillBonus: error: " + ex20.Message);
										}
									}
								}
								catch (Exception ex21)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Backstory error: " + ex21.Message);
									}
								}
								try
								{
									pawn.Notify_ColorChanged();
									pawn.skills.Notify_SkillDisablesChanged();
									List<TraitDef> allDefsListForReading2 = DefDatabase<TraitDef>.AllDefsListForReading;
									int num14 = 0;
									while (num14 < allDefsListForReading2.Count)
									{
										if (!pawn.story.traits.HasTrait(allDefsListForReading2[num14]))
										{
											if (Prefs.DevMode)
											{
												CLog.Message(string.Concat(new object[]
												{
													"Children: Birth: Fix Up Skills and Passions: Add Trait: ",
													allDefsListForReading2[num14].defName,
													" - count: ",
													pawn.story.traits.allTraits.Count
												}));
											}
											pawn.story.traits.GainTrait(new Trait(allDefsListForReading2[num14], 0, true));
											for (int num15 = 0; num15 < pawn.story.traits.allTraits.Count; num15++)
											{
												if (pawn.story.traits.allTraits[num15].def.defName == allDefsListForReading2[num14].defName)
												{
													pawn.story.traits.allTraits.RemoveAt(num15);
													break;
												}
											}
											if (Prefs.DevMode)
											{
												CLog.Message(string.Concat(new object[]
												{
													"Children: Birth: Fix Up Skills and Passions: Remove Trait: ",
													allDefsListForReading2[num14].defName,
													" - count: ",
													pawn.story.traits.allTraits.Count
												}));
												break;
											}
											break;
										}
										else
										{
											num14++;
										}
									}
									List<SkillDef> allDefsListForReading3 = DefDatabase<SkillDef>.AllDefsListForReading;
									for (int num16 = 0; num16 < allDefsListForReading3.Count; num16++)
									{
										SkillDef skillDef = allDefsListForReading3[num16];
										SkillRecord skill2 = pawn.skills.GetSkill(skillDef);
										if (skill2.TotallyDisabled)
										{
											skill2.passion = Passion.None;
											skill2.Level = 0;
											skill2.xpSinceLastLevel = 0f;
										}
									}
									if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: Fix Up Skills and Passions done");
									}
								}
								catch (Exception ex22)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Fix Up Skills and Passions error: " + ex22.Message);
									}
								}
								try
								{
									Find.LetterStack.ReceiveLetter("MessageGaveBirth".Translate(pawn2.LabelIndefinite()).CapitalizeFirst(), "MessageGaveBirth".Translate(pawn2.LabelIndefinite()).CapitalizeFirst(), LetterDefOf.PositiveEvent, pawn, null, null, null, null);
									if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: Letter set");
									}
								}
								catch (Exception ex23)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Letter error: " + ex23.Message);
									}
								}
								try
								{
									ChildrenController.NoFertilityMod(pawn);
								}
								catch (Exception ex24)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: FertilityDefault-error: " + ex24.Message);
									}
								}
								try
								{
									if (!pawn2.story.traits.HasTrait(MorePawnUtilities.TraitDefOfMODDED.ChildrenDislikesChildren))
									{
										pawn2.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(MorePawnUtilities.ThoughtDefOfMODDED.ChildrenBornPositive), null);
									}
									else
									{
										pawn2.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(MorePawnUtilities.ThoughtDefOfMODDED.ChildrenBornNegative), null);
									}
									if (pawn3 != null)
									{
										List<PawnRelationDef> list6 = pawn3.GetRelations(pawn2).ToList<PawnRelationDef>();
										if (!list6.Contains(PawnRelationDefOf.ExLover) && !list6.Contains(PawnRelationDefOf.ExSpouse))
										{
											if (!pawn3.story.traits.HasTrait(MorePawnUtilities.TraitDefOfMODDED.ChildrenDislikesChildren))
											{
												pawn3.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(MorePawnUtilities.ThoughtDefOfMODDED.ChildrenBornPositive), pawn2);
											}
											else
											{
												pawn3.needs.mood.thoughts.memories.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(MorePawnUtilities.ThoughtDefOfMODDED.ChildrenBornNegative), pawn2);
											}
										}
									}
									if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: Mood set");
									}
								}
								catch (Exception ex25)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Mood error: " + ex25.Message);
									}
								}
								try
								{
									pawn.Drawer.renderer.graphics.ResolveAllGraphics();
									if (Prefs.DevMode)
									{
										CLog.Message("Children: Birth: Graphics reset");
									}
								}
								catch (Exception ex26)
								{
									if (Prefs.DevMode)
									{
										ChdLogging.LogError("Children: Birth: Graphics reset error: " + ex26.Message);
									}
								}
								ChildrenHarmony.Pawn_AgeTracker_Patch.CheckWorkingPawn(pawn, false);
								pawn.workSettings.EnableAndInitialize();
								if (Prefs.DevMode)
								{
									CLog.Message("Children: Birth: workSettings reinitialized");
								}
							}
							else if (Prefs.DevMode)
							{
								CLog.Message("Children: Birth: Skipped! (Child already set-up)");
							}
						}
					}
					try
					{
						if (pawn.health.hediffSet.HasHediff(MorePawnUtilities.HediffDefOfMODDED.ChildBirthInProgress, false))
						{
							pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(MorePawnUtilities.HediffDefOfMODDED.ChildBirthInProgress, false));
							if (Prefs.DevMode)
							{
								CLog.Message("Children: Birth in progress removed!");
							}
						}
						return;
					}
					catch (Exception ex27)
					{
						if (Prefs.DevMode)
						{
							ChdLogging.LogError("Children: Birth in progress error: " + ex27.Message);
						}
						return;
					}
				}
				if (Prefs.DevMode)
				{
					CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn: NO SPAWN!");
					CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn: " + pawn);
					CLog.Message("Children: Birth: TrySpawnHatchedOrBornPawn: " + motherOrEgg);
				}
			}

			// Token: 0x060000C7 RID: 199 RVA: 0x00011804 File Offset: 0x0000FA04
			public static int GetChildAge(Pawn pawn, bool log)
			{
				int result = ChildrenController.Instance.GeneratedChildrenAge.Value;
				try
				{
					float num;
					if (pawn.kindDef.race.ToString().ToLower() == "human")
					{
						num = -2f;
					}
					else
					{
						switch (ChildrenController.Instance.GeneratedChildrenLifeStage.Value)
						{
							case ChildrenController.GeneratedChildrenLifeStageHandleEnum._Baby:
								num = ChildrenHarmony.PawnUtility_Patch.GetChildRaceAgeByString(pawn, "Baby", log);
								break;
							case ChildrenController.GeneratedChildrenLifeStageHandleEnum._Toddler:
								num = ChildrenHarmony.PawnUtility_Patch.GetChildRaceAgeByString(pawn, "Toddler", log);
								if (num < 0f)
								{
									num = ChildrenHarmony.PawnUtility_Patch.GetChildRaceAgeByString(pawn, "Child", log);
								}
								break;
							case ChildrenController.GeneratedChildrenLifeStageHandleEnum._Child:
								num = ChildrenHarmony.PawnUtility_Patch.GetChildRaceAgeByString(pawn, "Child", log);
								break;
							case ChildrenController.GeneratedChildrenLifeStageHandleEnum._Teenager:
								num = ChildrenHarmony.PawnUtility_Patch.GetChildRaceAgeByString(pawn, "Teenager", log);
								if (num < 0f)
								{
									num = ChildrenHarmony.PawnUtility_Patch.GetChildRaceAgeByString(pawn, "Teen", log);
								}
								break;
							case ChildrenController.GeneratedChildrenLifeStageHandleEnum._Adult:
								num = ChildrenHarmony.PawnUtility_Patch.GetChildRaceAgeByString(pawn, "Adult", log);
								break;
							default:
								num = -2f;
								break;
						}
					}
					if (num >= 0f)
					{
						result = (int)Math.Ceiling((double)num);
						CLog.Message(string.Concat(new object[]
						{
							"Children: ",
							pawn.Name,
							" Alien-AgeFound: ",
							result.ToString()
						}));
					}
					else if (num == -1f)
					{
						CLog.Message(string.Concat(new object[]
						{
							"Children: ",
							pawn.Name,
							" AgeNOTFound: ",
							result.ToString()
						}));
					}
					else
					{
						CLog.Message(string.Concat(new object[]
						{
							"Children: ",
							pawn.Name,
							" AgeOK: ",
							result.ToString()
						}));
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("GetChildAge: error: " + ex.Message);
					}
				}
				return result;
			}

			// Token: 0x060000C8 RID: 200 RVA: 0x000119F0 File Offset: 0x0000FBF0
			public static float GetChildRaceAgeAdult(Pawn pawn, bool log)
			{
				float result = 20f;
				try
				{
					float childRaceAgeByString = ChildrenHarmony.PawnUtility_Patch.GetChildRaceAgeByString(pawn, "Adult", log);
					if (childRaceAgeByString >= 0f)
					{
						result = childRaceAgeByString;
						if (log)
						{
							CLog.Message(string.Concat(new object[]
							{
								"Children: ",
								pawn.Name,
								" AdultAgeFound: ",
								result.ToString()
							}));
						}
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("GetChildRaceAgeAdult: error: " + ex.Message);
					}
				}
				return result;
			}

			// Token: 0x060000C9 RID: 201 RVA: 0x00011A88 File Offset: 0x0000FC88
			public static float GetChildRaceAgeByString(Pawn pawn, string ageName, bool log)
			{
				float num = -1f;
				try
				{
					if (pawn.kindDef != null && pawn.kindDef.RaceProps != null && pawn.kindDef.RaceProps.lifeStageAges != null)
					{
						foreach (LifeStageAge lifeStageAge in pawn.kindDef.RaceProps.lifeStageAges)
						{
							if (lifeStageAge.def.defName.ToLower().Contains(ageName.ToLower()))
							{
								num = lifeStageAge.minAge;
								if (log)
								{
									CLog.Message(string.Concat(new object[]
									{
										"Children: ",
										pawn.Name,
										" AgeFound: ",
										ageName,
										" - ",
										num.ToString()
									}));
									break;
								}
								break;
							}
						}
						if (num == -1f && log)
						{
							CLog.Message(string.Concat(new object[]
							{
								"Children: ",
								pawn.Name,
								"LifeStage: ",
								ageName,
								" not found! Race: ",
								pawn.kindDef.race.defName
							}));
						}
					}
					else if (log)
					{
						CLog.Message("Children: " + pawn.Name + " Race not defined correctly!");
					}
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("GetChildRaceAgeByString: error: " + ex.Message);
					}
				}
				return num;
			}

			// Token: 0x060000CA RID: 202 RVA: 0x00011C34 File Offset: 0x0000FE34
			private static Color MixColors(Color motherColor, bool hasFather, Color fatherColor)
			{
				Color color = default(Color);
				color.r = motherColor.r;
				color.g = motherColor.g;
				color.b = motherColor.b;
				if (hasFather)
				{
					color.r = (float)Math.Round((double)((color.r + fatherColor.r) / 2f), 2);
					color.g = (float)Math.Round((double)((color.g + fatherColor.g) / 2f), 2);
					color.b = (float)Math.Round((double)((color.b + fatherColor.b) / 2f), 2);
				}
				if (color.r > 1f)
				{
					color.r = 1f;
				}
				if (color.g > 1f)
				{
					color.g = 1f;
				}
				if (color.b > 1f)
				{
					color.b = 1f;
				}
				return color;
			}

			// Token: 0x060000CB RID: 203 RVA: 0x00011D28 File Offset: 0x0000FF28
			private static void SetScenarioTraits(Pawn Child, Pawn Mother, Pawn Father)
			{
				try
				{
					Scenario scenario = Find.Scenario;
					if (scenario != null)
					{
						if (Prefs.DevMode)
						{
							CLog.Message("Children: ScenarioTraits");
						}
						using (IEnumerator<ScenPart> enumerator = scenario.AllParts.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ScenPart scenPart = enumerator.Current;
								if (!(scenPart.def.defName == "ForcedHediff") && scenPart.def.defName == "ForcedTrait")
								{
									ScenPart_ForcedTrait scenPart_ForcedTrait = (ScenPart_ForcedTrait)scenPart;
									TraitDef traitDef = (TraitDef)Traverse.Create(scenPart_ForcedTrait).Field("trait").GetValue();
									if (traitDef != null)
									{
										if (Prefs.DevMode)
										{
											CLog.Message("Children: ScenarioTraits: Scenario has forced trait: " + traitDef.ToString());
										}
										if (Mother != null || Father != null)
										{
											if ((Mother != null && Mother.story.traits.HasTrait(traitDef)) || (Father != null && Father.story.traits.HasTrait(traitDef)))
											{
												if (!Child.story.traits.HasTrait(traitDef))
												{
													scenPart_ForcedTrait.Notify_PawnGenerated(Child, Child.Faction.IsPlayer ? PawnGenerationContext.PlayerStarter : PawnGenerationContext.NonPlayer, true);
													if (Child.story.traits.HasTrait(traitDef))
													{
														if (Prefs.DevMode)
														{
															CLog.Message("Children: SetScenarioTraits: ScenarioTraits set! (inherited) " + scenario.ToString() + " Trait: " + traitDef.ToString());
														}
													}
													else if (Prefs.DevMode)
													{
														CLog.Message("Children: SetScenarioTraits: ScenarioTraits chance: trait was not set! (inherited) " + scenario.ToString() + " Trait: " + traitDef.ToString());
													}
												}
												else if (Prefs.DevMode)
												{
													CLog.Message("Children: SetScenarioTraits: ScenarioTraits already has trait! " + scenario.ToString() + " Trait: " + traitDef.ToString());
												}
											}
										}
										else if (!Child.story.traits.HasTrait(traitDef))
										{
											scenPart_ForcedTrait.Notify_PawnGenerated(Child, Child.Faction.IsPlayer ? PawnGenerationContext.PlayerStarter : PawnGenerationContext.NonPlayer, true);
											if (Child.story.traits.HasTrait(traitDef))
											{
												if (Prefs.DevMode)
												{
													CLog.Message("Children: SetScenarioTraits: ScenarioTraits set! " + scenario.ToString() + " Trait: " + traitDef.ToString());
												}
											}
											else if (Prefs.DevMode)
											{
												CLog.Message("Children: SetScenarioTraits: ScenarioTraits chance: trait was not set! " + scenario.ToString() + " Trait: " + traitDef.ToString());
											}
										}
										else if (Prefs.DevMode)
										{
											CLog.Message("Children: SetScenarioTraits: ScenarioTraits already has trait! " + scenario.ToString() + " Trait: " + traitDef.ToString());
										}
									}
								}
							}
							goto IL_29B;
						}
					}
					if (Prefs.DevMode)
					{
						CLog.Message("Children: SetScenarioTraits: No Scenario!");
					}
				IL_29B:;
				}
				catch (Exception ex)
				{
					if (Prefs.DevMode)
					{
						ChdLogging.LogError("Children: SetScenarioTraits: ScenarioTraits set error: " + ex.Message);
					}
				}
			}
		}
	}
}
