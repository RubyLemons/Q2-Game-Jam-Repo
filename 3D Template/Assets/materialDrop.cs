using UnityEngine;

public class materialDrop : MonoBehaviour
{
    public GameObject materialPrefab;
    public bool triggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    void Update()
    {
        if (triggered == true)
        {
            Debug.Log("2");
            Drop();
        }
    }


    public void Drop()
    {
        Debug.Log("3");
        //drop materials on death
        int dropAmount = Random.Range(1, 4);
        for (int i = 0; i < dropAmount; i++)
        {
            Debug.Log("4");
            GameObject materialPrefab = Instantiate(Resources.Load<GameObject>("materialDrop"), transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Rigidbody matRb = materialPrefab.GetComponent<Rigidbody>();
            Vector3 forceDir = (Vector3.up + (Random.insideUnitSphere)).normalized;
            matRb.AddForce(forceDir * Random.Range(2f, 5f), ForceMode.Impulse);
        }
    }
}
    // Update is called once per frame
