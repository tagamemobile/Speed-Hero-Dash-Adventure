using UnityEngine;
[AddComponentMenu("TAGame/Program")]
public class Program : MonoBehaviour
{
    Tiger tiger;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tiger = new Tiger();
        string logString = tiger.HowToEat();
        Debug.Log(logString);
    }

   
}
