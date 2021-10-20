using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arbiter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //If Towers are Destroyed End the Game
        if (Blackboard.instance.redTower.Count == 0)
        {
            Blackboard.instance.blueWins.SetActive(true);
        }
        else if (Blackboard.instance.blueTower.Count == 0)
        {
            Blackboard.instance.redWins.SetActive(true);
        }
        else
        {
            //Run each Units Update if They Exist
            if (Blackboard.instance.blueAttackers.Count > 0)
            {
                BlueUpdate();
            }

            if (Blackboard.instance.redAttackers.Count > 0)
            {
                RedUpdate();
            }
        }

    }

    /// <summary>
    /// Helper Function to Return the Index of the Nearest Object in the List
    /// </summary>
    /// <param name="unit">Current start unit</param>
    /// <param name="destinations">List of possible destinations</param>
    /// <returns>Index of nearest object in list</returns>
    public int nearestIndex(GameObject unit, List<GameObject> destinations)
    {
        float shortestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < destinations.Count; i++)
        {
            //The Woderful Distance Formula
            float distance = Mathf.Sqrt(Mathf.Pow((unit.transform.position.x - destinations[i].transform.position.x), 2)
                + Mathf.Pow((unit.transform.position.y - destinations[i].transform.position.y), 2));

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                index = i;
            }
        }

        return index;
    }

    /// <summary>
    /// Helper Function to Return the Index of the Nearest Object in the List
    /// </summary>
    /// <param name="unit">Current start unit</param>
    /// <param name="destinations">List of possible destinations</param>
    /// <returns>Index of nearest object in list</returns>
    public int nearestIndex(GameObject unit, List<Agent> destinations)
    {
        float shortestDistance = Mathf.Infinity;
        int index = 0;

        for (int i = 0; i < destinations.Count; i++)
        {
            //The Woderful Distance Formula
            float distance = Mathf.Sqrt(Mathf.Pow((unit.transform.position.x - destinations[i].transform.position.x), 2)
                + Mathf.Pow((unit.transform.position.y - destinations[i].transform.position.y), 2));

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                index = i;
            }
        }

        return index;
    }

    /// <summary>
    /// Helper Function to Return the Two Nearest Agents For a Tower
    /// </summary>
    /// <param name="tower"></param>
    /// <param name="attackers"></param>
    /// <returns></returns>
    public List<Agent> nearestAgents(GameObject tower, List<Agent> attackers)
    {
        
        List<Agent> defenders = new List<Agent>();

        float shortestDistance = Mathf.Infinity;
        float secondShortestDistance = Mathf.Infinity;

        int index = 0;
        int index2 = 0;

        for (int i = 0; i < attackers.Count; i++)
        {
            //The Wonderful Distance Formula
            float distance = Mathf.Sqrt(Mathf.Pow((tower.transform.position.x - attackers[i].transform.position.x), 2)
                + Mathf.Pow((tower.transform.position.y - attackers[i].transform.position.y), 2));

            if (distance < secondShortestDistance)
            {
                if (distance < shortestDistance)
                {
                    secondShortestDistance = shortestDistance;
                    shortestDistance = distance;
                    index = i;
                }
                else
                {
                    secondShortestDistance = distance;
                    index2 = i;
                }

            }
        }

        defenders.Add(attackers[index]);
        defenders.Add(attackers[index2]);

        return defenders;
    }

    public void BlueUpdate()
    {

        //If an Attack Unit is Waiting Send It After An Enemy Tower
        foreach (Agent a in Blackboard.instance.blueAttackers)
        {
            if (a.task == Agent.Task.Wait)
            {
                //Find Nearest Tower to Unit
                int tower = nearestIndex(a.gameObject, Blackboard.instance.redTower);

                a.MoveTo(Blackboard.instance.redTower[tower].transform.position);
                a.target = Blackboard.instance.redTower[tower];
                a.targetIndex = tower;
                a.task = Agent.Task.MoveToTower;
            }
        }


        //Attack Enemies that are Nearby
        foreach (Agent a in Blackboard.instance.blueAttackers)
        {
            if (a.task != Agent.Task.AttackEnemy && a.enemiesNearby.Count > 0)
            {
                a.MoveTo(a.enemiesNearby[0].transform.position);
                a.target = a.enemiesNearby[0].gameObject;
                a.targetIndex = 0;
                a.task = Agent.Task.MoveToEnemy;
            }
        }

        //First Priority Should Be Defending Towers
        //If a Tower's Health is Below The Check Limit, Send Two of the Attackers to Defend It
        //Choose by nearest Attackers

        for (int i = 0; i < Blackboard.instance.blueTowerHps.Count; i++)
        {
            if (Blackboard.instance.blueTowerHps[i] < Blackboard.instance.blueTowerDefendHps[i])
            {
                //Find nearest two attackers
                float num = Mathf.Round(Blackboard.instance.blueAttackers.Count / 2);

                List<Agent> defenders = nearestAgents(Blackboard.instance.blueTower[i], Blackboard.instance.blueAttackers);

                foreach (Agent a in defenders)
                {
                    a.MoveTo(Blackboard.instance.blueTower[i].transform.position);
                    a.target = Blackboard.instance.blueTower[i];
                    a.targetIndex = i;
                    a.task = Agent.Task.DefendTower;
                }

                break;

            }
        }
    }

    /// <summary>
    /// Update Red Agents with Instructions. Red will proritize attack enemies over towers.
    /// </summary>
    public void RedUpdate()
    {
        //If there are no enemies to attack, attack the towers
        if (Blackboard.instance.blueAttackers.Count <= 0)
        {
            //Send Each Unit to the Nearest Tower to Attack
            foreach (Agent a in Blackboard.instance.redAttackers)
            {
                if (a.task != Agent.Task.AttackTower)
                {

                    //Find Nearest Tower to Unit
                    int tower = nearestIndex(a.gameObject, Blackboard.instance.blueTower);

                    //Send Unit
                    a.MoveTo(Blackboard.instance.blueTower[tower].transform.position);
                    a.target = Blackboard.instance.blueTower[tower];
                    a.targetIndex = tower;
                    a.task = Agent.Task.MoveToTower;
                }
                
            }
        }
        else
        {
            //Attack Existing Enemies 
            foreach (Agent a in Blackboard.instance.redAttackers)
            {
                if (a.task != Agent.Task.AttackEnemy)
                {
                    int enemy = nearestIndex(a.gameObject, Blackboard.instance.blueAttackers);

                    a.MoveTo(Blackboard.instance.blueAttackers[enemy].transform.position);
                    a.target = Blackboard.instance.blueAttackers[enemy].gameObject;
                    a.targetIndex = enemy;
                    a.task = Agent.Task.MoveToEnemy;
                }
            }

            //First Priority Should Be Defending Towers
            //If a Tower's Health is Below Defend Check, Send 2 of the Attackers to Defend It
            //Choose by nearest Attackers
            for (int i = 0; i < Blackboard.instance.redTowerHps.Count; i++)
            {
                if (Blackboard.instance.redTowerHps[i] < Blackboard.instance.redTowerDefendHps[i]) //&& Blackboard.instance.redTowersBeingAttacked[i])
                {
                    //Find nearest two attackers
                    List<Agent> defenders = nearestAgents(Blackboard.instance.redTower[i], Blackboard.instance.redAttackers);

                    foreach (Agent a in defenders)
                    {
                        a.MoveTo(Blackboard.instance.redTower[i].transform.position);
                        a.target = Blackboard.instance.redTower[i];
                        a.targetIndex = i;
                        a.task = Agent.Task.DefendTower;
                    }

                    break;

                }
            }
        }

    }

}
