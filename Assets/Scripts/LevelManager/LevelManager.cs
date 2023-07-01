using UnityEngine.SceneManagement;

public class LevelManager
{
    public static void NextLevel()
    {
        var index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index + 1);  
    }
}
