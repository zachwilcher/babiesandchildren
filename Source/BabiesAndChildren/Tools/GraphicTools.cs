using RimWorld;
using BabiesAndChildren.Harmony;
using UnityEngine;
using Verse;

namespace BabiesAndChildren
{
    public static class GraphicTools
    {
        internal static void ResolveAgeGraphics(PawnGraphicSet graphics)
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                if (!ChildrenUtility.RaceUsesChildren(graphics.pawn)) return;

                if (graphics.pawn.story.hairDef != null)
                {
                    graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>(graphics.pawn.story.hairDef.texPath, ShaderDatabase.Cutout, Vector2.one, graphics.pawn.story.hairColor);
                }
                if (ChildrenUtility.GetAgeStage(graphics.pawn) > AgeStage.Child) return;                

                // The pawn is a baby
                if (ChildrenUtility.GetAgeStage(graphics.pawn) == AgeStage.Baby)
                {
                    graphics.nakedGraphic = GraphicDatabase.Get<Graphic_Single>("Things/Pawn/Humanlike/Children/Bodies/Newborn", ShaderDatabase.CutoutSkin, Vector2.one, graphics.pawn.story.SkinColor);
                    graphics.rottingGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Bodies/Newborn", ShaderDatabase.CutoutSkin, Vector2.one, PawnGraphicSet.RottingColor);
                    graphics.headGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/null", ShaderDatabase.Cutout, Vector2.one, Color.white);
                    graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/null", ShaderDatabase.Cutout, Vector2.one, Color.white);
                }

                // The pawn is a toddler
                if (ChildrenUtility.GetAgeStage(graphics.pawn) == AgeStage.Toddler)
                {
                    string upright = "";
                    if (ChildrenUtility.ToddlerIsUpright(graphics.pawn))
                    {
                        upright = "Upright";
                    }
                    graphics.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Bodies/Toddler" + upright, ShaderDatabase.CutoutSkin, Vector2.one, graphics.pawn.story.SkinColor);
                    graphics.rottingGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Bodies/Toddler" + upright, ShaderDatabase.CutoutSkin, Vector2.one, PawnGraphicSet.RottingColor);
                    graphics.headGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/null", ShaderDatabase.Cutout, Vector2.one, Color.white);
                    graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/null", ShaderDatabase.Cutout, Vector2.one, Color.white);                    
                }
                // The pawn is a child
                else if (ChildrenUtility.GetAgeStage(graphics.pawn) == AgeStage.Child)
                {
                    if (graphics.pawn.def.defName == "Human")
                    {               
                        graphics.headGraphic = GetChildHeadGraphics(ShaderDatabase.CutoutSkin, graphics.pawn.story.SkinColor);
                    }
                    else if (BnCSettings.human_like_head_enabled && ChildrenUtility.HumanFaceRaces(graphics.pawn))
                    {
                        graphics.headGraphic = GetChildHeadGraphics(ShaderDatabase.CutoutSkin, graphics.pawn.story.SkinColor);
                    }
                    else if (BnCSettings.Rabbie_Child_head_enabled && graphics.pawn.def.defName == "Rabbie")
                    {
                        graphics.headGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Heads/RabbieChild", ShaderDatabase.Cutout, Vector2.one, Color.white);
                    }
                }
            });
        }

        internal static Graphic GetChildHeadGraphics(Shader shader, Color skinColor)
        {
            Graphic_Multi graphic = null;
            string str = "Male_Child";
            string path = "Things/Pawn/Humanlike/Children/Heads/" + str;
            graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, Vector2.one, skinColor) as Graphic_Multi;
            return graphic;
        }

        public static Vector3 ModifyChildYPosOffset(Vector3 pos, Pawn pawn, bool portrait)
        {
            Vector3 newPos = pos;

            // move the draw target down to compensate for child shortness
            if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child && !pawn.InBed())
            {
                if (pawn.def.defName == "Human") newPos.z += BnCSettings.HumanrootlocZ;
                else newPos.z += BnCSettings.AlienrootlocZ;
                //newPos.z -= 0.15f;            
            }
            if (pawn.InBed() && !portrait && pawn.CurrentBed().def.size.z == 1)
            {
                Building_Bed bed = pawn.CurrentBed();
                // Babies and toddlers get drawn further down along the bed
                if (ChildrenUtility.GetAgeStage(pawn) < AgeStage.Child)
                {
                    Vector3 vector = new Vector3(0, 0, 0.5f).RotatedBy(bed.Rotation.AsAngle);
                    newPos -= vector;
                    // ... as do children, but to a lesser extent
                }
                else if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child)
                {
                    if (pawn.def.defName == "Human") newPos.z += BnCSettings.HumanrootlocZ;
                    else newPos.z += BnCSettings.AlienrootlocZ;
                    // Are we in a crib?
                    Vector3 vector = new Vector3(0, 0, 0.2f).RotatedBy(bed.Rotation.AsAngle);
                    newPos -= vector;
                }
                newPos += new Vector3(0, 0, 0.2f);

            }
            return newPos;
        }
    }
}