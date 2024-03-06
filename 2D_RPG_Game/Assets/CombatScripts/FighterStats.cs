using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FighterStats : MonoBehaviour, IComparable
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject healthFill;

    [SerializeField]
    private GameObject magicFill;

    private GameObject GameControllerObj;

    [Header("Stats")]
    public float health;
    public float magic;
    public float attack;
    public float defense;
    public float range;
    public float speed;
    public float experience;

    private float startHealth;
    private float startMagic;

    [HideInInspector]
    public int nextActTurn;

    private bool dead = false;

    //resize heal and magic bar
    private Transform healthTransform;
    private Transform magicTransform;

    private Vector2 healthScale;
    private Vector2 magicScale;

    private float xNewHealthScale;
    private float xNewMagicScale;

    void Awake()
    {
        healthTransform = healthFill.GetComponent<RectTransform>();
        healthScale = healthFill.transform.localScale;

        magicTransform = magicFill.GetComponent<RectTransform>();
        magicScale = magicFill.transform.localScale;

        startHealth = health;
        startMagic = magic;

        GameControllerObj = GameObject.Find("GameControllerObject");
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        animator.Play("damaged");

        if (health <= 0)
        {
            dead = true;
            if (dead == true)
            {
                Debug.Log("You are dead");
            }
            gameObject.tag = "Dead";
            Destroy(healthFill);
            Destroy(gameObject);
            SceneManager.LoadScene("dungeon");
        }
        else if (damage > 0)
        {
            xNewHealthScale = healthScale.x * (health / startHealth);
            healthFill.transform.localScale = new Vector2(xNewHealthScale, healthScale.y);
        }

        GameControllerObj.GetComponent<GameController2>().battleText.gameObject.SetActive(true);
        GameControllerObj.GetComponent<GameController2>().battleText.text = "Damage Received:" + damage.ToString(); 
        Invoke("ContinueGame", 2);
    }

    public void updateManaFill(float cost)
    {
        if(cost > 1)
        {
            magic -= cost;
            xNewMagicScale = magicScale.x * (magic / startMagic);
            magicFill.transform.localScale = new Vector2(xNewMagicScale, magicScale.y);
        }
    }

    public bool GetDead()
    {
        return dead;
    }

    void ContinueGame()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController2>().NextTurn();
    }

    public void CalculateNextTurn(int currentTurn)
    {
        nextActTurn = currentTurn + Mathf.CeilToInt(100f / speed);
    }

    public int CompareTo(object otherStats)
    {
        Debug.Log("CompareTo Running");
        int nex = nextActTurn.CompareTo(((FighterStats)otherStats).nextActTurn);
        return nex;
    }
}
