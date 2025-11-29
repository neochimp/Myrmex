using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public void RestartLevel()
    {   
        // Obtain the current scene from the build hierarchy.
        // Load that scene. 
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene); 
    }

    public void Quit()
    {   
        // You can't actually Application.Quit the editor, has to be built. 
        Debug.LogWarning("Will not work in Unity editor"); 
        Application.Quit(); 
    }
}
