using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackboard : MonoBehaviour
{
    //Singleton Instance
    public static Blackboard instance;

    //Shared Variables
    public List<GameObject> redTower;
    public List<GameObject> blueTower;
    public List<Text> redTowerHpText;
    public List<Text> blueTowerHpText;
    public List<int> redTowerHps;
    public List<int> blueTowerHps;
    public List<int> redTowerDefendHps;
    public List<int> blueTowerDefendHps;
    public List<Agent> blueAttackers;
    public List<Agent> redAttackers;
    public GameObject redPrefab;
    public GameObject bluePrefab;
    public GameObject blueWins;
    public GameObject redWins;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    //Spawning Methods
    public void SpawnRed()
    {
        float x = Random.Range(-7, 7.1f);

        GameObject temp = Instantiate(redPrefab, new Vector3(x, 2.5f, 0), Quaternion.identity);
        redAttackers.Add(temp.GetComponent<Agent>());
    }

    public void SpawnBlue()
    {
        float x = Random.Range(-7, 7.1f);

        GameObject temp = Instantiate(bluePrefab, new Vector3(x, -2.5f, 0), Quaternion.identity);
        blueAttackers.Add(temp.GetComponent<Agent>());
    }
}
