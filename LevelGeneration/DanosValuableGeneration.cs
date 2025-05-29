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
    public class DanosValuableGeneration
    {
        public static DanosValuableGeneration? Instance { get; private set; }
        private System.Random rng;
        private DanosValuableGeneration(int seed)
        {
            rng = new System.Random(seed);
        }

        public static void Create(int seed)
        {
            Instance = new DanosValuableGeneration(seed);
        }

        public static void Delete()
        {
            Instance = null;
        }

        public int Range(int min, int max) => rng.Next(min, max);
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

        public IEnumerator SetupHost(ValuableDirector instance)
        {
            float time = SemiFunc.RunGetDifficultyMultiplier();
            if (SemiFunc.RunIsArena())
            {
                time = 0.75f;
            }
            instance.totalMaxAmount = Mathf.RoundToInt(instance.totalMaxAmountCurve.Evaluate(time));
            instance.tinyMaxAmount = Mathf.RoundToInt(instance.tinyMaxAmountCurve.Evaluate(time));
            instance.smallMaxAmount = Mathf.RoundToInt(instance.smallMaxAmountCurve.Evaluate(time));
            instance.mediumMaxAmount = Mathf.RoundToInt(instance.mediumMaxAmountCurve.Evaluate(time));
            instance.bigMaxAmount = Mathf.RoundToInt(instance.bigMaxAmountCurve.Evaluate(time));
            instance.wideMaxAmount = Mathf.RoundToInt(instance.wideMaxAmountCurve.Evaluate(time));
            instance.tallMaxAmount = Mathf.RoundToInt(instance.tallMaxAmountCurve.Evaluate(time));
            instance.veryTallMaxAmount = Mathf.RoundToInt(instance.veryTallMaxAmountCurve.Evaluate(time));
            if (SemiFunc.RunIsArena())
            {
                instance.totalMaxAmount /= 2;
                instance.tinyMaxAmount /= 3;
                instance.smallMaxAmount /= 3;
                instance.mediumMaxAmount /= 3;
                instance.bigMaxAmount /= 3;
                instance.wideMaxAmount /= 2;
                instance.tallMaxAmount /= 2;
                instance.veryTallMaxAmount /= 2;
            }
            foreach (LevelValuables valuablePreset in LevelGenerator.Instance.Level.ValuablePresets)
            {
                instance.tinyValuables.AddRange(valuablePreset.tiny);
                instance.smallValuables.AddRange(valuablePreset.small);
                instance.mediumValuables.AddRange(valuablePreset.medium);
                instance.bigValuables.AddRange(valuablePreset.big);
                instance.wideValuables.AddRange(valuablePreset.wide);
                instance.tallValuables.AddRange(valuablePreset.tall);
                instance.veryTallValuables.AddRange(valuablePreset.veryTall);
            }
            List<ValuableVolume> list = UnityEngine.Object.FindObjectsOfType<ValuableVolume>(includeInactive: false).ToList();
            instance.tinyVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Tiny);
            instance.tinyVolumes.ShuffleWithRng(rng);
            instance.smallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Small);
            instance.smallVolumes.ShuffleWithRng(rng);
            instance.mediumVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Medium);
            instance.mediumVolumes.ShuffleWithRng(rng);
            instance.bigVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Big);
            instance.bigVolumes.ShuffleWithRng(rng);
            instance.wideVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Wide);
            instance.wideVolumes.ShuffleWithRng(rng);
            instance.tallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Tall);
            instance.tallVolumes.ShuffleWithRng(rng);
            instance.veryTallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.VeryTall);
            instance.veryTallVolumes.ShuffleWithRng(rng);
            if (instance.valuableDebug == ValuableDebug.All)
            {
                instance.totalMaxAmount = list.Count;
                instance.tinyMaxAmount = instance.tinyVolumes.Count;
                instance.smallMaxAmount = instance.smallVolumes.Count;
                instance.mediumMaxAmount = instance.mediumVolumes.Count;
                instance.bigMaxAmount = instance.bigVolumes.Count;
                instance.wideMaxAmount = instance.wideVolumes.Count;
                instance.tallMaxAmount = instance.tallVolumes.Count;
                instance.veryTallMaxAmount = instance.veryTallVolumes.Count;
            }
            if (instance.valuableDebug == ValuableDebug.None || LevelGenerator.Instance.Level.ValuablePresets.Count <= 0)
            {
                instance.totalMaxAmount = 0;
                instance.tinyMaxAmount = 0;
                instance.smallMaxAmount = 0;
                instance.mediumMaxAmount = 0;
                instance.bigMaxAmount = 0;
                instance.wideMaxAmount = 0;
                instance.tallMaxAmount = 0;
                instance.veryTallMaxAmount = 0;
            }
            instance.valuableTargetAmount = 0;
            string[] _names = new string[7] { "Tiny", "Small", "Medium", "Big", "Wide", "Tall", "Very Tall" };
            int[] _maxAmount = new int[7] { instance.tinyMaxAmount, instance.smallMaxAmount, instance.mediumMaxAmount, instance.bigMaxAmount, instance.wideMaxAmount, instance.tallMaxAmount, instance.veryTallMaxAmount };
            List<ValuableVolume>[] _volumes = new List<ValuableVolume>[7] { instance.tinyVolumes, instance.smallVolumes, instance.mediumVolumes, instance.bigVolumes, instance.wideVolumes, instance.tallVolumes, instance.veryTallVolumes };
            string[] _path = new string[7] { instance.tinyPath, instance.smallPath, instance.mediumPath, instance.bigPath, instance.widePath, instance.tallPath, instance.veryTallPath };
            int[] _chance = new int[7] { instance.tinyChance, instance.smallChance, instance.mediumChance, instance.bigChance, instance.wideChance, instance.tallChance, instance.veryTallChance };
            List<GameObject>[] _valuables = new List<GameObject>[7] {instance.tinyValuables, instance.smallValuables, instance.mediumValuables, instance.bigValuables, instance.wideValuables, instance.tallValuables, instance.veryTallValuables };
            int[] _volumeIndex = new int[7];
            for (int _i = 0; _i < instance.totalMaxAmount; _i++)
            {
                float num = -1f;
                int num2 = -1;
                for (int i = 0; i < _names.Length; i++)
                {
                    if (_volumeIndex[i] < _maxAmount[i] && _volumeIndex[i] < _volumes[i].Count)
                    {

                        int num3 = Range(0, _chance[i]);
                        if ((float)num3 > num)
                        {
                            num = num3;
                            num2 = i;
                        }
                    }
                }
                if (num2 == -1)
                {
                    break;
                }
                ValuableVolume volume = _volumes[num2][_volumeIndex[num2]];
                GameObject valuable = _valuables[num2][Range(0, _valuables[num2].Count)];
                instance.Spawn(valuable, volume, _path[num2]);
                _volumeIndex[num2]++;
                yield return null;
            }
            if (instance.valuableTargetAmount < instance.totalMaxAmount && (bool)DebugComputerCheck.instance && (!DebugComputerCheck.instance.enabled || !DebugComputerCheck.instance.LevelDebug || !DebugComputerCheck.instance.ModuleOverrideActive || !DebugComputerCheck.instance.ModuleOverride))
            {
                for (int j = 0; j < _names.Length; j++)
                {
                    if (_volumeIndex[j] < _maxAmount[j])
                    {
                        Debug.LogError("Could not spawn enough ''" + _names[j] + "'' valuables!");
                    }
                }
            }
            if (GameManager.instance.gameMode == 1)
            {
                instance.PhotonView.RPC("ValuablesTargetSetRPC", RpcTarget.All, instance.valuableTargetAmount);
            }
            instance.valuableSpawnPlayerReady++;
            while (GameManager.instance.gameMode == 1 && instance.valuableSpawnPlayerReady < PhotonNetwork.CurrentRoom.PlayerCount)
            {
                yield return new WaitForSeconds(0.1f);
            }
            instance.VolumesAndSwitchSetup();
            while (GameManager.instance.gameMode == 1 && instance.switchSetupPlayerReady < PhotonNetwork.CurrentRoom.PlayerCount)
            {
                yield return new WaitForSeconds(0.1f);
            }
            instance.setupComplete = true;
        }
    }
}
