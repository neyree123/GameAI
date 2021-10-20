using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Agent : MonoBehaviour
{
    public enum Task
    {
        AttackEnemy,
        AttackTower,
        DefendTower,
        MoveToTower,
        MoveToEnemy,
        Wait,
    }

    public Task task;
    public int health = 10;
    private int damage = 2;
    public float speed = 2;
    public float stopRange = 2;
    public float attackRange = 1;
    public GameObject target;
    public int targetIndex;
    public bool inAttackRange;
    public float attackTimer;
    public List<Agent> enemiesNearby;
    public bool isBlue;

    // Start is called before the first frame update
    void Start()
    {
        task = Task.Wait;
        enemiesNearby = new List<Agent>();
    }

    // Update is called once per frame
    void Update()
    {
        //Quick WASD for Testing
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, .1f, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-.1f, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, -.1f, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(.1f, 0, 0);
        }

        //Blue and Red Have Different Targets and Functions
        if (isBlue)
        {
            BlueUpdate();
        }
        else
        {
            RedUpdate();
        }
        
    }

    public void BlueUpdate()
    {
        switch (task)
        {
            case Task.AttackEnemy:
                if (target == null)
                {
                    task = Task.Wait;
                    break;
                }

                //If Enemy Has Moved, Chase It               
                if (!inRange(target.transform.position, attackRange))
                {
                    MoveTo(target.transform.position);
                    task = Task.MoveToEnemy;
                }

                attackTimer += Time.deltaTime;

                //Attack
                if (attackTimer >= 5)
                {
                    damage = Random.Range(1, 4);

                    target.GetComponent<Agent>().health -= damage;

                    //If Enemy Dies
                    if (target.GetComponent<Agent>().health <= 0)
                    {
                        target.GetComponent<Agent>().OnDeath();
                        task = Task.Wait;
                    }

                    attackTimer = 0;
                }
                break;
            case Task.AttackTower:
                //If Tower is Gone
                if (targetIndex > Blackboard.instance.redTower.Count || target == null)
                {
                    task = Task.Wait;
                    break;
                }

                attackTimer += Time.deltaTime;

                //Attack
                if (attackTimer >= 5)
                {
                    damage = Random.Range(1, 4);

                    Blackboard.instance.redTowerHps[targetIndex] -= damage;
                    Blackboard.instance.redTowerHpText[targetIndex].text = Blackboard.instance.redTowerHps[targetIndex].ToString();
                    attackTimer = 0;

                    //If Tower Is Destroyed
                    if (Blackboard.instance.redTowerHps[targetIndex] <= 0)
                    {
                        Blackboard.instance.redTowerHpText[targetIndex].text = "0";
                        Destroy(Blackboard.instance.redTower[targetIndex]);
                        Blackboard.instance.redTower.RemoveAt(targetIndex);
                        Blackboard.instance.redTowerHpText.RemoveAt(targetIndex);
                        Blackboard.instance.redTowerHps.RemoveAt(targetIndex);
                        task = Task.Wait;
                    }
                }
                break;
            case Task.DefendTower:
                if (inRange(target.transform.position, stopRange))
                {
                    transform.DOPause(); //Stop Movement

                    if (enemiesNearby.Count > 0)
                    {
                        //transform.DOPause();
                        attackTimer += Time.deltaTime;

                        //Attack
                        if (attackTimer >= 5)
                        {
                            enemiesNearby[0].health -= damage;
                            attackTimer = 0;

                            //If Enemy Dies
                            if (enemiesNearby[0].health <= 0)
                            {
                                enemiesNearby[0].OnDeath();

                                //Set Next Point to Defend Tower
                                if (enemiesNearby.Count == 0)
                                {
                                    Blackboard.instance.blueTowerDefendHps[targetIndex] -= 25;
                                }

                                task = Task.Wait;
                            }
                        }
                    }
                    else
                    {
                        task = Task.Wait;
                    }


                }
                break;
            case Task.MoveToTower:
                if (inRange(target.transform.position, attackRange))
                {
                    task = Task.AttackTower;
                    transform.DOPause();
                }
                break;
            case Task.MoveToEnemy:
                if (inRange(target.transform.position, attackRange))
                {
                    task = Task.AttackEnemy;
                    transform.DOPause();
                }
                break;
            default:
                break;
        }
    }

    public void RedUpdate()
    {
        switch (task)
        {
            case Task.AttackEnemy:
                if (target == null)
                {
                    task = Task.Wait;
                    break;
                }

                //If Enemy Has Moved, Chase It               
                if (!inRange(target.transform.position, attackRange))
                {
                    MoveTo(target.transform.position);
                    task = Task.MoveToEnemy;
                }

                attackTimer += Time.deltaTime;

                //Attack
                if (attackTimer >= 3)
                {
                    damage = Random.Range(1, 4);

                    target.GetComponent<Agent>().health -= damage;

                    //If Enemy Dies
                    if (target.GetComponent<Agent>().health <= 0)
                    {
                        target.GetComponent<Agent>().OnDeath();
                        task = Task.Wait;
                    }

                    attackTimer = 0;
                }
                break;
            case Task.AttackTower:
                //If Tower is Gone
                if (targetIndex > Blackboard.instance.blueTower.Count || target == null)
                {
                    task = Task.Wait;
                    break;
                }

                attackTimer += Time.deltaTime;

                //Attack
                if (attackTimer >= 3)
                {
                    damage = Random.Range(1, 4);

                    Blackboard.instance.blueTowerHps[targetIndex] -= damage;
                    Blackboard.instance.blueTowerHpText[targetIndex].text = Blackboard.instance.blueTowerHps[targetIndex].ToString();
                    attackTimer = 0;

                    //If Tower Is Destroyed
                    if (Blackboard.instance.blueTowerHps[targetIndex] <= 0)
                    {
                        Blackboard.instance.blueTowerHpText[targetIndex].text = "0";
                        Destroy(Blackboard.instance.blueTower[targetIndex]);
                        Blackboard.instance.blueTower.RemoveAt(targetIndex);
                        Blackboard.instance.blueTowerHpText.RemoveAt(targetIndex);
                        Blackboard.instance.blueTowerHps.RemoveAt(targetIndex);
                        task = Task.Wait;
                    }
                }
                break;
            case Task.DefendTower:
                if (inRange(target.transform.position, stopRange))
                {
                    transform.DOPause(); //Stop Movement

                    if (enemiesNearby.Count > 0)
                    {
                        attackTimer += Time.deltaTime;

                        //Attack
                        if (attackTimer >= 5)
                        {
                            enemiesNearby[0].health -= damage;
                            attackTimer = 0;

                            //If Enemy Dies
                            if (enemiesNearby[0].health <= 0)
                            {
                                enemiesNearby[0].OnDeath();

                                if (enemiesNearby.Count == 0)
                                {
                                    Blackboard.instance.blueTowerDefendHps[targetIndex] -= 25;
                                }

                                task = Task.Wait;
                            }
                        }
                    }
                    else
                    {
                        task = Task.Wait;
                    }

                }

                break;
            case Task.MoveToTower:
                if (inRange(target.transform.position, attackRange))
                {
                    task = Task.AttackTower;
                    transform.DOPause();
                }
                break;
            case Task.MoveToEnemy:
                if (inRange(target.transform.position, attackRange))
                {
                    task = Task.AttackEnemy;
                    transform.DOPause();
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Handles All Variable Changes When Unit Dies
    /// </summary>
    public void OnDeath()
    {

        if (isBlue)
        {
            Blackboard.instance.blueAttackers.Remove(this);
        }
        else
        {
            Blackboard.instance.redAttackers.Remove(this);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Moves to Target Position Over Time
    /// </summary>
    /// <param name="targetPos">Target Position</param>
    public void MoveTo(Vector3 targetPos)
    {
        //target = targetPos;
        transform.DOMove(targetPos, speed).SetSpeedBased(true);
    }

    /// <summary>
    /// Checks if target is in range of the target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool inRange(Vector3 target, float range)
    {
       float distance = Mathf.Sqrt(Mathf.Pow((transform.position.x - target.x), 2)
       + Mathf.Pow((transform.position.y - target.y), 2));

       if (distance <= range) { return true;}

       return false;
    }


    //OnTriggers for Enemy Detection
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Red" && isBlue)
        {
            if (!enemiesNearby.Contains(collision.gameObject.GetComponent<Agent>()))
            {
                enemiesNearby.Add(collision.gameObject.GetComponent<Agent>());
            }
        }
        else if (collision.gameObject.tag == "Blue" && !isBlue)
        {
            if (!enemiesNearby.Contains(collision.gameObject.GetComponent<Agent>()))
            {
                enemiesNearby.Add(collision.gameObject.GetComponent<Agent>());
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Red" && isBlue)
        {
            enemiesNearby.Remove(collision.gameObject.GetComponent<Agent>());
        }
        else if (collision.gameObject.tag == "Blue" && !isBlue)
        {
            enemiesNearby.Remove(collision.gameObject.GetComponent<Agent>());
        }
    }
}
