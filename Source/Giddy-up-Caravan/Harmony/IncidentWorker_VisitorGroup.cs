﻿using GiddyUpCaravan.Utilities;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace GiddyUpCaravan.Harmony
{
    [HarmonyPatch(typeof(IncidentWorker_VisitorGroup), "TryExecuteWorker")]
    static class IncidentWorker_VisitorGroup_TryExecuteWorker
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            for (var i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                yield return instruction;

                if (i > 0 && instructionsList[i - 1].operand == AccessTools.Method(typeof(IncidentWorker_NeutralGroup), "SpawnPawns")) //Identifier for which IL line to inject to

                {
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 2);//load generated pawns as parameter
                    yield return new CodeInstruction(OpCodes.Ldarg_1);//load incidentparms as parameter
                    yield return new CodeInstruction(OpCodes.Call, typeof(VisitorMountUtility).GetMethod("mountAnimals"));//Injected code                                                                                                                         //yield return new CodeInstruction(OpCodes.Stloc_2);
                }

            }

        }
    }

    //Animals can't be turned into traders so should be stripped from the list
    [HarmonyPatch(typeof(IncidentWorker_VisitorGroup), "TryConvertOnePawnToSmallTrader")]
    static class IncidentWorker_VisitorGroup_TryConvertOnePawnToSmallTrader
    {
        static void Prefix(ref List<Pawn> pawns)
        {
            List<Pawn> animals = new List<Pawn>();
            foreach(Pawn pawn in pawns)
            {
                if (pawn.RaceProps.Animal)
                {
                    animals.Add(pawn);
                }
            }
            pawns = pawns.Except(animals).ToList();
        }
    }


}
