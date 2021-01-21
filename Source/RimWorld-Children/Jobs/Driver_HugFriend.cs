using System;
using RimWorld;
using Verse;
using Verse.AI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;

namespace RimWorldChildren
{
	public class InteractionWorker_HugFriend : InteractionWorker
	{
		public override float RandomSelectionWeight (Pawn initiator, Pawn recipient)
		{
			if (initiator.relations.OpinionOf(recipient) >= 50 && initiator.needs.mood.CurLevel >= 0.9f && ChildrenUtility.GetAgeStage(initiator) <= AgeStage.Child && ChildrenUtility.GetAgeStage(initiator) != AgeStage.Baby)
			{
				return 0.2f;
			}

			return 0;
		}
	}
}