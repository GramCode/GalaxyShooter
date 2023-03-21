using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {

        }
    }

    public void GoBack()
    {
        

        Time.timeScale = 1;
        Destroy(this.gameObject);
        
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);

    }
}
