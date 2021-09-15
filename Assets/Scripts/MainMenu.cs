using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string firstLevel;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject boss;

    private void Awake()
    {
        optionsScreen.SetActive(false);
    }
    private void FixedUpdate()
    {
        boss.transform.position = new Vector3(boss.transform.position.x, Mathf.Sin(Time.time * 2f) * 20f + 20f, 0);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void OpenOptions()
    {
        optionsScreen.SetActive(true);
    }
    public void CloseOptions()
    {
        optionsScreen.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("Quitting"); //simple check here since application doesn't quit in editor
        Application.Quit();
    }
}
