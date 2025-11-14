using UnityEngine;

public class RenderOnClick : MonoBehaviour
{
    public GameObject objectToRender;
    public GameObject objectsToUnrender1;
    public GameObject objectsToUnrender2;
    public bool isRendered = false;
   
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
        }
        else if (isRendered == false)
        { 
            objectToRender.SetActive(false);
        }

        if (isRendered == true)
        {
            objectsToUnrender1.SetActive(false);
            objectsToUnrender2.SetActive(false);
        }
        else if (isRendered == false)
        {
            objectsToUnrender1.SetActive(true);
            objectsToUnrender2.SetActive(true);
        }
    }

}
