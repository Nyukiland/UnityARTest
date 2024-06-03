using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToPos : MonoBehaviour
{
    [SerializeField]
    float speed;

    public Vector3 posToGo;

    [SerializeField]
    bool isEnnemi;

    private void Update()
    {
        if (isEnnemi)
        {
            transform.LookAt(posToGo);
            posToGo = Camera.main.transform.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, posToGo, speed * Time.deltaTime);
    }
}
