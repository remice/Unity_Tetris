using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public void ClickButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
