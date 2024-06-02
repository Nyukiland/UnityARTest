using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Events;

public class MainControl : MonoBehaviour
{
    //visible in inspector
    [Header("Game related")]

    [SerializeField]
    float maxTimer;

    [SerializeField]
    int baseAmmo;

    [Header("Canvas related")]

    [SerializeField]
    GameObject playMenu;

	[SerializeField]
	TextMeshProUGUI textScore, textAmmo, textChrono;

    [Header("Set up related")]

    [SerializeField]
    ARRaycastManager raycastAR;

    [SerializeField]
    GameObject prefabToSpawn;

    [SerializeField]
    UnityEvent endEvent;

    //------------------------------------------------------
    //private variable

    bool objectPlaced;

    bool endDoOnce;
    float timer;

    int score;
    int ammo;

    private void Start()
    {
        GameReset();
    }

    private void Update()
    {
        CheckEnd();
        PlaceObject();
        ShootTarget();

        UpdateText();
    }

    void CheckEnd()
    {
        if (!objectPlaced) return;

        if (timer <= 0 && !endDoOnce)
        {
            endDoOnce = true;
            endEvent?.Invoke();
        }
        else if (timer > maxTimer) timer -= Time.deltaTime;
    }

    void PlaceObject()
    {
        if (objectPlaced) return;

        List<ARRaycastHit> hit = new();

        if (Input.GetMouseButtonDown(0))
        {
            if (raycastAR.Raycast(Input.mousePosition, hit, TrackableType.PlaneWithinPolygon))
            {
                Instantiate(prefabToSpawn, hit[0].pose.position, Quaternion.identity);
                objectPlaced = true;
            }
        }
    }  
    
    void ShootTarget()
    {
        if (!objectPlaced || ammo <= 0 || timer <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            ammo--;

            RaycastHit hit;
            Ray rayCam = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(rayCam, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Target"))
                {
                    score++;
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    void UpdateText()
    {
        textChrono.text = (Mathf.Round(timer*10)/10).ToString();

        textScore.text = "Score: " +score;
        textAmmo.text = "Ammo: " + ammo;
    }

    //increment the Ammo based on the detected object
    private void OnTriggerEnter(Collider other)
    {
        if (timer <= 0) return;

        if (other.CompareTag("Ammo"))
        {
            ammo++;
            Destroy(other.gameObject);
        }
    }

    public void GameReset()
    {
        timer = maxTimer;
        score = 0;
        ammo = baseAmmo;
        endDoOnce = false;
    }
}