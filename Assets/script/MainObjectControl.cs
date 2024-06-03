using UnityEngine;

public class MainObjectControl : MonoBehaviour
{
	[SerializeField]
	GameObject target, ressources;

	[SerializeField]
	float targetSpawnRate, ressourceSpawnRate;

    [SerializeField]
    float targetSpawnRadius, ressourceSpawnRadius;

    [HideInInspector]
    public bool actif;

    float timerTarget;
    float timerRessource;

    private void Update()
    {
        if (!actif) return;

        timerTarget += Time.deltaTime;
        timerRessource += Time.deltaTime;

        float scaleModif = transform.localScale.x;

        if (timerRessource > ressourceSpawnRate)
        {
            timerRessource = 0;
            InstantiateElement(ressources, ressourceSpawnRadius * scaleModif);
        }

        if (timerTarget > targetSpawnRate)
        {
            timerTarget = 0;
            InstantiateElement(target, targetSpawnRadius * scaleModif);
        }
    }

    void InstantiateElement(GameObject element, float radiusDist)
    {
        Vector3 position = transform.position + Random.insideUnitSphere * radiusDist;

        position = new Vector3(position.x, Mathf.Abs(position.y), position.z);

        Instantiate(element, position, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * ressourceSpawnRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * targetSpawnRadius);
    }
}