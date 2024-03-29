using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public GameObject owner;

    [SerializeField]
    private string AnimationName;

    [SerializeField]
    private bool specialAttack;

    [SerializeField]
    private float specialCost;

    [SerializeField]
    private float minAttackMultiplier;

    [SerializeField] 
    private float maxAttackMultiplier;

    [SerializeField]
    private float minDefenseMultiplier;

    [SerializeField]
    private float maxDefenseMultiplier;


    private FighterStats attackerStats;
    private FighterStats targetStats;

    private float damage = 0.0f;
    private float xMagicNewScale;
    private Vector2 magicScale;

    private void Start()
    {
        magicScale = GameObject.Find("HeroMagicFill").GetComponent<RectTransform>().localScale;
    }

    public void Attack(GameObject victim)
    {
        attackerStats = owner.GetComponent<FighterStats>();
        targetStats = victim.GetComponent<FighterStats>();

        if(attackerStats.magic >= specialCost)
        {
            float multiplier = Random.Range(minAttackMultiplier, maxAttackMultiplier);

            if(specialCost > 0)
            {
                attackerStats.updateManaFill(specialCost);
            }

            damage = multiplier * attackerStats.attack;
            if (specialAttack)
            {
                damage = multiplier * attackerStats.magic;
                attackerStats.magic -= specialCost;
            }
        }

        float defenseMultiplier = Random.Range(minDefenseMultiplier, maxDefenseMultiplier);
        damage = Mathf.Max(0, damage - (defenseMultiplier * targetStats.defense));
        owner.GetComponent<Animator>().Play(AnimationName);
        targetStats.ReceiveDamage(damage);
    }

    public void Run()
    {
        GameObject GameControllerObj = GameObject.Find("GameControllerObject");
        attackerStats = owner.GetComponent<FighterStats>();
        float runAbility = 0.25f;
        float randomValue = Random.value;

        if(randomValue <= runAbility)
        {
            Debug.Log("Hero successfully runs away!");
        }
        else
        {
            GameControllerObj.GetComponent<GameController2>().battleText.gameObject.SetActive(true);
            GameControllerObj.GetComponent<GameController2>().battleText.text = "Hero couldn't run away...";
            Debug.Log("Hero couldn't run away...");
            Invoke("ContinueGame", 2);
        }
    }

    void ContinueGame()
    {
        GameObject.Find("GameControllerObject").GetComponent<GameController2>().NextTurn();
    }
}
