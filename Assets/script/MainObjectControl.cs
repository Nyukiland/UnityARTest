using UnityEngine;

public class MainObjectControl : MonoBehaviour
{
	[SerializeField]
	GameObject target, ressources;

	[SerializeField]
	float targetSpawnRate, ressourceSpawnRate;

    [SerializeField]
    float targetSpawnRadius, ressourceSpawnRadius;

    float timerTarget;
    float timerRessource;

    private void Update()
    {
        timerTarget += Time.deltaTime;
        timerRessource += Time.deltaTime;

        if (timerRessource > ressourceSpawnRate)
        {
            timerRessource = 0;
            InstantiateElement(ressources, ressourceSpawnRadius);
        }

        if (timerTarget > targetSpawnRate)
        {
            timerTarget = 0;
            InstantiateElement(target, targetSpawnRadius);
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
        Gizmos.DrawWireSphere(transform.position, ressourceSpawnRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetSpawnRadius);
    }
}