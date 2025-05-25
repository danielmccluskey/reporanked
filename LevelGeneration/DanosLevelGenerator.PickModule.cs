using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RepoRanked.LevelGeneration
{
    public partial class DanosLevelGenerator
    {

        public static GameObject PickModule(List<GameObject> _list1, List<GameObject> _list2, List<GameObject> _list3, ref int _index1, ref int _index2, ref int _index3, ref int _loops1, ref int _loops2, ref int _loops3, LevelGenerator instance, System.Random rng )
        {
            GameObject result = null;
            float[] array = new float[3] { instance.ModuleRarity1, instance.ModuleRarity2, instance.ModuleRarity3 };
            if (_list2.Count <= 0)
            {
                array[1] = 0f;
            }
            if (_list3.Count <= 0)
            {
                array[2] = 0f;
            }
            int num = Mathf.Max(_loops1, _loops2, _loops3);
            int num2 = Mathf.Min(_loops1, _loops2, _loops3);
            if (num != num2)
            {
                if (_loops1 == num2 && array[0] > 0f)
                {
                    if (_loops1 != _loops2)
                    {
                        array[1] = 0f;
                    }
                    if (_loops1 != _loops3)
                    {
                        array[2] = 0f;
                    }
                }
                else if (_loops2 == num2 && array[1] > 0f)
                {
                    if (_loops2 != _loops1)
                    {
                        array[0] = 0f;
                    }
                    if (_loops2 != _loops3)
                    {
                        array[2] = 0f;
                    }
                }
                else if (_loops3 == num2 && array[2] > 0f)
                {
                    if (_loops3 != _loops1)
                    {
                        array[0] = 0f;
                    }
                    if (_loops3 != _loops2)
                    {
                        array[1] = 0f;
                    }
                }
            }
            float num3 = -1f;
            int num4 = -1;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > 0f)
                {
                    float num5 = (float)rng.NextDouble() * array[i];
                    if (num5 > num3)
                    {
                        num3 = num5;
                        num4 = i;
                    }
                }
            }
            switch (num4)
            {
                case 0:
                    result = _list1[_index1];
                    _index1++;
                    if (_index1 >= _list1.Count)
                    {
                        _list1.ShuffleWithRng(rng);
                        _index1 = 0;
                        _loops1++;
                    }
                    break;
                case 1:
                    result = _list2[_index2];
                    _index2++;
                    if (_index2 >= _list2.Count)
                    {
                        _list2.ShuffleWithRng(rng);
                        _index2 = 0;
                        _loops2++;
                    }
                    break;
                case 2:
                    result = _list3[_index3];
                    _index3++;
                    if (_index3 >= _list3.Count)
                    {
                        _list3.ShuffleWithRng(rng);
                        _index3 = 0;
                        _loops3++;
                    }
                    break;
            }

            if(result == null)
            {
                //choose a random module from the first list
                if (_list1.Count > 0)
                {
                    result = _list1[rng.Next(_list1.Count)];
                }
                else if (_list2.Count > 0)
                {
                    result = _list2[rng.Next(_list2.Count)];
                }
                else if (_list3.Count > 0)
                {
                    result = _list3[rng.Next(_list3.Count)];
                }

                if (result == null)
                {
                    RepoRanked.Logger.LogError("No modules available to pick from.");
                    return null; // No modules available
                }
            }


            return result;
        }
    }
}
