using UnityEngine;

public class ControladorTutorial : MonoBehaviour
{
    [SerializeField] private GameObject cajaTutorial; 

    void Update()
    {
        if (cajaTutorial != null && cajaTutorial.activeSelf)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                cajaTutorial.SetActive(false);
            }
        }
    }
}