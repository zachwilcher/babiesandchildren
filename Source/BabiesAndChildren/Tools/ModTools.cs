using Verse;

namespace BabiesAndChildren
{
    public static class ModTools
    {
        public static bool IsModOn(string modName)
        {
            foreach (ModMetaData modMetaData in ModLister.AllInstalledMods)
            {
                if ((modMetaData != null) && (modMetaData.Active) && (modMetaData.Name.ToLower().StartsWith(modName.ToLower())))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsRobot(Pawn pawn)
        {
            if (ChildrenBase.ModAT_ON)
            {
                string defName = pawn.def.defName.ToLower();
                return defName.Contains("robot") || defName.Contains("android");
            }
            else
            {
                return false;
            }
        }


        public static bool HumanFaceRaces(Pawn pawn)
        {
            if (
                pawn.def.defName == "Kurin_Race"
                || pawn.def.defName == "Ratkin"
            )
            {
                return true;
            }
            return false;
        }

        public static Hediff GetVag(Pawn pawn) => pawn.health.hediffSet.hediffs.Find((Hediff hed) => hed.def.defName.ToLower().Contains("vagina"));
        public static Hediff GetPen(Pawn pawn) => pawn.health.hediffSet.hediffs.Find((Hediff hed) => hed.def.defName.ToLower().Contains("penis"));
        public static Hediff GetBre(Pawn pawn) => pawn.health.hediffSet.hediffs.Find((Hediff hed) => hed.def.defName.ToLower().Contains("breasts"));
        public static Hediff GetAnu(Pawn pawn) => pawn.health.hediffSet.hediffs.Find((Hediff hed) => hed.def.defName.ToLower().Contains("anus"));

        public static void ChangeSize(Pawn pawn, float Maxsize, bool Is_SizeInit)
        {
            if (ChildrenBase.ModRJW_ON)
            {
                Hediff bodypart = GetAnu(pawn);
                if (bodypart == null) return;
                float size = Rand.Range(0.01f, Maxsize);
                float cursize = bodypart.Severity;
                if (Is_SizeInit)
                {
                    bodypart.Severity = size;
                }
                else
                {
                    bodypart.Severity = ((cursize > size) ? cursize : size);
                }

                if (pawn.gender == Gender.Male)
                {
                    bodypart = GetPen(pawn);
                    if (bodypart == null) return;
                    size = Rand.Range(0.01f, Maxsize);
                    cursize = bodypart.Severity;
                    if (Is_SizeInit)
                    {
                        bodypart.Severity = size;
                    }
                    else
                    {
                        bodypart.Severity = ((cursize > size) ? cursize : size);
                    }
                }
                else if (pawn.gender == Gender.Female)
                {
                    bodypart = ModTools.GetVag(pawn);
                    if (bodypart == null) return;
                    Maxsize = ((Maxsize < 0.35f) ? 0.35f : Maxsize);
                    size = Rand.Range(0.02f, Maxsize);
                    cursize = bodypart.Severity;
                    if (Is_SizeInit)
                    {
                        bodypart.Severity = size;
                    }
                    else
                    {
                        bodypart.Severity = ((cursize > size) ? cursize : size);
                    }

                    bodypart = GetBre(pawn);
                    if (bodypart == null) return;
                    if (ChildrenUtility.GetAgeStage(pawn) < AgeStage.Teenager && Maxsize > 0.07f) Maxsize = 0.07f;
                    size = Rand.Range(0.01f, Maxsize);
                    cursize = bodypart.Severity;
                    if (Is_SizeInit)
                    {
                        bodypart.Severity = size;
                    }
                    else
                    {
                        bodypart.Severity = ((cursize > size) ? cursize : size);
                    }
                }
            }
        }
    }
}