using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ValuableDirector;

namespace RepoRanked.LevelGeneration
{
    public partial class DanosValuableGeneration
    {
        public void DollarValueSetLogic(ValuableObject valuableInstance)
        {
            if (valuableInstance.dollarValueOverride != 0)
            {
                valuableInstance.dollarValueOriginal = valuableInstance.dollarValueOverride;
                valuableInstance.dollarValueCurrent = valuableInstance.dollarValueOverride;
            }
            else
            {
                valuableInstance.dollarValueOriginal = Mathf.Round(rng.Next((int)valuableInstance.valuePreset.valueMin, (int)valuableInstance.valuePreset.valueMax));
                valuableInstance.dollarValueOriginal = Mathf.Round(valuableInstance.dollarValueOriginal / 100f) * 100f;
                valuableInstance.dollarValueCurrent = valuableInstance.dollarValueOriginal;
            }
            valuableInstance.dollarValueSet = true;

        }

        public static IEnumerator CustomDollarValueSet(ValuableObject instance)
        {
            DollarValueGenerationQueue.Enqueue(instance);

            while (!DollarValueGenerationQueue.IsMyTurn(instance))
                yield return null;

            while (LevelGenerator.Instance.State <= LevelGenerator.LevelState.Valuable)
                yield return null;

            if (!SemiFunc.IsMultiplayer())
            {
                instance.DollarValueSetLogic();
            }
            else if (SemiFunc.IsMasterClient())
            {
                instance.DollarValueSetLogic();
                instance.photonView.RPC("DollarValueSetRPC", RpcTarget.Others, instance.dollarValueCurrent);
            }

            if (SemiFunc.IsMasterClientOrSingleplayer())
            {
                RoundDirector.instance.haulGoalMax += (int)instance.dollarValueCurrent;
            }

            DollarValueGenerationQueue.Done(instance);
        }

    }
}
