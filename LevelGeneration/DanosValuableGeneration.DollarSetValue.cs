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

            if (!valuableInstance.dollarValueSet)
            {

                if (valuableInstance.dollarValueOverride != 0)
                {
                    valuableInstance.dollarValueOriginal = valuableInstance.dollarValueOverride;
                    valuableInstance.dollarValueCurrent = valuableInstance.dollarValueOverride;
                }
                else
                {
                    valuableInstance.dollarValueOriginal = Mathf.Round(rng2.Next((int)valuableInstance.valuePreset.valueMin, (int)valuableInstance.valuePreset.valueMax));
                    valuableInstance.dollarValueOriginal = Mathf.Round(valuableInstance.dollarValueOriginal / 100f) * 100f;
                    valuableInstance.dollarValueCurrent = valuableInstance.dollarValueOriginal;
                }
                valuableInstance.dollarValueSet = true;
                //Debug.Log($"Dollar value set for {valuableInstance.gameObject.name}: {valuableInstance.dollarValueCurrent}");
            }
            else
            {
                //Debug.LogWarning($"Dollar value for {valuableInstance.gameObject.name} is already set. Current value: {valuableInstance.dollarValueCurrent}");
            }

        }

        public static IEnumerator CustomDollarValueSet(ValuableObject instance)
        {



            while (LevelGenerator.Instance.State <= LevelGenerator.LevelState.Valuable)
                yield return null;



            if (SemiFunc.IsMasterClientOrSingleplayer())
            {
                RoundDirector.instance.haulGoalMax += (int)instance.dollarValueCurrent;
            }

        }

    }
}
