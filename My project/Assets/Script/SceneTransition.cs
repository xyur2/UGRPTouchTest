using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    public void ToTouchTest()
    {
        SceneManager.LoadScene("TouchTest");
    }
    public void ToSlideTest()
    {
        SceneManager.LoadScene("SlideTest");
    }
}
