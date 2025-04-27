using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public bool isCutscene = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) QuitGame();

        if (Input.GetKeyDown(KeyCode.E) && isCutscene) StartGame("Spaceship");
    }

    public void StartGame(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
