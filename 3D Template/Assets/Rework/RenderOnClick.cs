using UnityEngine;

public class RenderOnClick1 : MonoBehaviour
{
    public GameObject objectToRender;
    public GameObject objectToRender2;
    public GameObject objectToRender3;
    public GameObject objectsToUnrender;
    public GameObject objectsToUnrender2;
    public GameObject objectsToUnrender3;

    private bool isRendered = false;
   
    public void Onclick()
    { 
        Debug.Log("Kreston is a Bum That disrespects the dead");
        //also kreston is a bum
        //
       isRendered = !isRendered;//ts is so simple but wont work cause u dumb
      
    }
    public void Update()
    {
        if (isRendered == true)
        {
            objectToRender.SetActive(true);
            objectToRender2.SetActive(true);
            objectToRender3.SetActive(true);
        }
        else if (isRendered == false)
        { 
            objectToRender.SetActive(false);
            objectToRender2.SetActive(false);
            objectToRender3.SetActive(false);
        }

        if (isRendered == true)
        {
            objectsToUnrender.SetActive(false);
            objectsToUnrender2.SetActive(false);
            objectsToUnrender3.SetActive(false);
        }
        else if (isRendered == false)
        {
            objectsToUnrender.SetActive(true);
            objectsToUnrender2.SetActive(true);
            objectsToUnrender3.SetActive(true);
        }
    }

}
