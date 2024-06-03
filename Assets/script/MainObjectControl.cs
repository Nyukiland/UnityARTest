using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainObjectControl : MonoBehaviour
{
	[SerializeField]
	GameObject target, ressources;

	[SerializeField]
	Vector2 targetSpawnRate, ressourceSpawnRate;

    [SerializeField]
    float targetSpawnRadius, ressourceSpawnRadius;

    public bool actif;

    float timerTarget, timerRessource;

    float rSpawnRate, tSpawnRate;

    private void Update()
    {
        if (!actif) return;

        timerTarget += Time.deltaTime;
        timerRessource += Time.deltaTime;

        float scaleModif = transform.localScale.x;

        if (timerRessource > rSpawnRate)
        {
            rSpawnRate = Random.Range(ressourceSpawnRate.x, ressourceSpawnRate.y);
            timerRessource = 0;
            InstantiateElement(ressources, ressourceSpawnRadius * scaleModif);
        }

        if (timerTarget > tSpawnRate)
        {
            tSpawnRate = Random.Range(targetSpawnRate.x, targetSpawnRate.y);
            timerTarget = 0;
            InstantiateElement(target, targetSpawnRadius * scaleModif);
        }
    }

    void InstantiateElement(GameObject element, float radiusDist)
    {
        GameObject obj = Instantiate(element, transform.position, Quaternion.identity);

        //change the scale based on the factor given to the generator
        obj.transform.localScale = obj.transform.localScale * transform.localScale.x;

        //set the position in the script
        Vector3 position = transform.position + Random.insideUnitSphere * radiusDist;
        position = new Vector3(position.x, Mathf.Abs(position.y), position.z);
        obj.GetComponent<GoToPos>().posToGo = position;
    }

    public void ResetAll()
    {
        rSpawnRate = Random.Range(ressourceSpawnRate.x, ressourceSpawnRate.y);
        tSpawnRate = Random.Range(targetSpawnRate.x, targetSpawnRate.y);

        List<GoToPos> temp = FindObjectsOfType<GoToPos>().ToList();

        for (int i = temp.Count - 1; i >= 0; i--)
        {
            Destroy(temp[i].gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * ressourceSpawnRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * targetSpawnRadius);
    }
}