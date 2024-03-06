using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameController2 : MonoBehaviour
{
    private List<FighterStats> fighterStats;

    [SerializeField]
    private GameObject battleMenu;

    public Text battleText;
    public GameObject gameOver;
    // Start is called before the first frame update

    void Start()
    {
        this.gameOver.SetActive(false);
        fighterStats = new List<FighterStats>();
        GameObject hero = GameObject.FindGameObjectWithTag("Hero");
        FighterStats currentFighterStats = hero.GetComponent<FighterStats>();
        currentFighterStats.CalculateNextTurn(0);
        fighterStats.Add(currentFighterStats);

        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        FighterStats currentEnemyStats = enemy.GetComponent<FighterStats>();
        currentEnemyStats.CalculateNextTurn(0);
        fighterStats.Add(currentEnemyStats);

        fighterStats.Sort();
        this.battleMenu.SetActive(false);

        NextTurn();
    }

    public void NextTurn()
    {
        battleText.gameObject.SetActive(false);
        FighterStats currentFighterStats = fighterStats[0];
        fighterStats.Remove(currentFighterStats);
        if (!currentFighterStats.GetDead())
        {
            GameObject currentUnit = currentFighterStats.gameObject;
            currentFighterStats.CalculateNextTurn(currentFighterStats.nextActTurn);
            fighterStats.Add(currentFighterStats);
            fighterStats.Sort();
            if (currentUnit.tag == "Hero")
            {
                this.battleMenu.SetActive(true);
            }
            else
            {
                this.battleMenu.SetActive(false);
                string attackType = Random.Range(0, 2) == 1 ? "attack" : "special";
                currentUnit.GetComponent<FighterAction>().SelectAttack(attackType);
            }
        }
        else NextTurn();
    }

    public void QuitToMenu()
    {
        Debug.Log("Quit To Menu");
        SceneManager.LoadScene("MainMenu");
    }

    public void Replay()
    {
        Debug.Log("Replay");
        SceneManager.LoadScene("WorldMap");
    }
}
