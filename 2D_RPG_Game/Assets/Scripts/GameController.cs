using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gamestate { FreeRoam, Dialog, Battle }
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    Gamestate state;

    private void Start()
    {
        DialogManager.Instance.OnShowDialog += () =>
        {
            state = Gamestate.Dialog;
        };
        DialogManager.Instance.OnHideDialog += () =>
        {
            if (state == Gamestate.Dialog)
            {
                state = Gamestate.FreeRoam;
            }
        };
    }
    private void Update()
    {
        if (state == Gamestate.FreeRoam)
        {
            playerController.HandleUpdate();
        } else if (state == Gamestate.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        } else if (state == Gamestate.Battle)
        {

        }
    }
}
