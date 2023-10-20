using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class StartButton : MonoBehaviour
{
    public Blackout blackout;

    public async void StartGame()
    {
        blackout.enabled = true;
        await Task.Delay(200);
        SceneManager.LoadScene("SpiritualWorld");
    }
}
