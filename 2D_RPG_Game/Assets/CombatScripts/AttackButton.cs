using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//MakeScript
public class AttackButton : MonoBehaviour
{
    public bool physical;
    public GameObject hero;
    // Start is called before the first frame update
    void Start()
    {
        string name = gameObject.name;
        if(gameObject.GetComponent<Button>() != null)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(() => AttachCallBack(name));
        }
        hero = GameObject.FindGameObjectWithTag("Hero");
    }

    public void StartAction()
    {
        string name = gameObject.name;
        gameObject.GetComponent<Button>().onClick.AddListener(() => AttachCallBack(name));
        hero = GameObject.FindGameObjectWithTag("Hero");
    }

    private void AttachCallBack(string btn)
    {
        if (btn.CompareTo("AttackBin") == 0)
        {
            hero.GetComponent<FighterAction>().SelectAttack("attack");
        }else if(btn.CompareTo("FireBin") == 0)
        {
            hero.GetComponent<FighterAction>().SelectAttack("special");
        }
        else
        {
            hero.GetComponent<FighterAction>().SelectAttack("run");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
