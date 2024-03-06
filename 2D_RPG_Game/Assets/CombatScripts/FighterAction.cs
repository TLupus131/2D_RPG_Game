using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAction : MonoBehaviour
{
    private GameObject enemy;
    private GameObject hero;

    [SerializeField]
    private GameObject attackPrefab;

    [SerializeField]
    private GameObject specialPrefab;

    [SerializeField]
    private Sprite faceIcon;

    private GameObject currentAttack;

    void Awake()
    {
        hero = GameObject.FindGameObjectWithTag("Hero");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
    }

    public void SelectAttack(string btn)
    {
        GameObject victim = hero;
        if (tag == "Hero")
        {
            victim = enemy;
        }
        if (btn.CompareTo("attack") == 0)
        {
            Debug.Log("MeleeAttack");
            attackPrefab.GetComponent<AttackScript>().Attack(victim);
        }
        else if (btn.CompareTo("special") == 0)
        {
            Debug.Log("Special");
            specialPrefab.GetComponent<AttackScript>().Attack(victim);
        }
        else
        {
            attackPrefab.GetComponent<AttackScript>().Run();
            Debug.Log("Run");
        }
    }
}
