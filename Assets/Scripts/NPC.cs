using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject modelPrfab;

    public NPC()
    {
         Vector3 position = new Vector3(Random.Range(-7.5f, 7.5f), 0, Random.Range(-5.0f, 5.0f));
         Instantiate(modelPrfab, position, Quaternion.identity);
    }
}
