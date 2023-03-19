using UnityEngine;

public class DissolveController : MonoBehaviour
{
    public Material dissolveMaterial;
    private float dissolveAmount = 0f;
    private bool increasing = true;

    void Update()
    {
        if (increasing)
        {
            dissolveAmount += Time.deltaTime;
            if (dissolveAmount >= 1f)
            {
                dissolveAmount = 1f;
                increasing = false;
            }
        }
        else
        {
            dissolveAmount -= Time.deltaTime;
            if (dissolveAmount <= 0f)
            {
                dissolveAmount = 0f;
                increasing = true;
            }
        }

        dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);
    }
}