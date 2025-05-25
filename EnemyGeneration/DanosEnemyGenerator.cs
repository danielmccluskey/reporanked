using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RepoRanked.EnemyGeneration
{
    public static class DanosEnemyGenerator
    {
        public static EnemySetup CreateCustomEnemySetup(Dictionary<string, int> enemyList)
        {
            var director = EnemyDirector.instance;
            if (director.enemiesDifficulty1.Count == 0)
            {
                RepoRanked.Logger.LogWarning("No enemies found in enemiesDifficulty1 to use as a template.");
                return null!;
            }

            // Use the first as a template
            EnemySetup template = director.enemiesDifficulty1[0];





            template.name = "RepoRankedCustomEnemy";
            template.spawnObjects = new List<GameObject>();

            //Loop through the dictionary, find the string and add the value as the amount
            foreach (KeyValuePair<string, int> kvp in enemyList)
            {
                GameObject enemy = FindEnemy(kvp.Key, director);
                if (enemy != null)
                {
                    for (int i = 0; i < kvp.Value; i++)
                    {
                        template.spawnObjects.Add(enemy);
                    }
                }
                else
                {
                }
            }


            return template;
        }





        public static GameObject FindEnemy(string enemyName, EnemyDirector instance)
        {
            GameObject enemyObject = null;


            enemyObject = ArrayLoop(enemyName, instance.enemiesDifficulty1);
            if (enemyObject != null)
            {
                return enemyObject;
            }

            enemyObject = ArrayLoop(enemyName, instance.enemiesDifficulty2);
            if (enemyObject != null)
            {
                return enemyObject;
            }

            enemyObject = ArrayLoop(enemyName, instance.enemiesDifficulty3);
            if (enemyObject != null)
            {
                return enemyObject;
            }

            return null;


        }

        public static GameObject ArrayLoop(string enemyName, List<EnemySetup> arrayToLoop)
        {
            List<string> enemyNames = new List<string>();
            //Loop through the array and find the enemy with the given name
            foreach (EnemySetup enemy in arrayToLoop)
            {

                foreach (GameObject enemyObject in enemy.spawnObjects)
                {
                    if (enemyName.Contains("Gnome Director"))
                    {
                        //Try get the EnemyGnomeDirector component
                        EnemyGnomeDirector enemyGnomeDirector = enemyObject.GetComponent<EnemyGnomeDirector>();
                        if (enemyGnomeDirector != null)
                        {
                            //RepoRanked.Logger.LogInfo($"Found enemy: {enemyObject.name}");
                            return enemyObject;
                        }
                    }

                    else if (enemyName.Contains("Bang Director"))
                    {

                        //Try bang director
                        EnemyBangDirector enemyBangDirector = enemyObject.GetComponent<EnemyBangDirector>();
                        if (enemyBangDirector != null)
                        {
                            //RepoRanked.Logger.LogInfo($"Found enemy: {enemyObject.name}");
                            return enemyObject;
                        }
                    }


                    else if (enemyName.Contains("Director"))
                    {
                        //Try it just by enemy.Name
                        if (enemyObject.name.Equals(enemyName, StringComparison.OrdinalIgnoreCase))
                        {
                            //RepoRanked.Logger.LogInfo($"Found enemy: {enemyObject.name}");
                            return enemyObject;
                        }

                    }





                    //Get the enemy parent script component
                    EnemyParent enemyParent = enemyObject.GetComponent<EnemyParent>();
                    if (enemyParent == null)
                    {
                        //RepoRanked.Logger.LogWarning($"EnemyParent component not found on {enemyObject.name}");
                        continue;
                    }
                    //Check if the enemy name matches the given name
                    if (enemyParent != null && enemyParent.name.Equals(enemyName, StringComparison.OrdinalIgnoreCase))
                    {
                        //If the enemy is found, return it
                        //RepoRanked.Logger.LogInfo($"Found enemy: {enemyObject.name}");
                        return enemyObject;
                    }
                    else
                    {
                    }

                    //Add the enemy name to the list
                    enemyNames.Add(enemyParent.name);





                }

            }

            //If the enemy is not found, return null

            //RepoRanked.Logger.LogWarning($"Enemy {enemyName} not found in the array.");


            return null;
        }
    }
}
