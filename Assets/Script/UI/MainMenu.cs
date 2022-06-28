using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip _buttonPress;
    public void MainMenuBtn()
    {
        AudioManager.Instance.PlaySound(_buttonPress);
        SceneManager.LoadScene(1);
    }
}
