using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour, Interactable
{
    public string sceneName = "Dungeon";

    public void Interact()
    {
        SceneManager.LoadScene(sceneName);
    }
}
