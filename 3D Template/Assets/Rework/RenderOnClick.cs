using UnityEngine;

public class RenderOnClick : MonoBehaviour
{
    public GameObject objectToRender;
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
        if (isRendered)
        {
            objectToRender.SetActive(true);
        }
        else
        {
            objectToRender.SetActive(false);
        }
    }

}
