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

    [SerializeField]
    int maxLife;

    [Header("Canvas related")]

    [SerializeField]
    GameObject playMenu;

	[SerializeField]
	TextMeshProUGUI textScore, textAmmo, textChrono, textLife;

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
    GameObject placedObj;

    bool endDoOnce;
    float timer;

    int score;
    int ammo;
    int life;

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

    void UpdateText()
    {
        textChrono.text = (Mathf.Round(timer * 10) / 10).ToString();

        textScore.text = "Score: " + score;
        textAmmo.text = "Ammo: " + ammo;
        textLife.text = "Life: " + life;
    }

    #region ObjectPlacement
    void PlaceObject()
    {
        if (placedObj == null) return;

        List<ARRaycastHit> hit = new();

        if (Input.GetMouseButtonDown(0))
        {
            if (raycastAR.Raycast(Input.mousePosition, hit, TrackableType.PlaneWithinPolygon))
            {
                placedObj = Instantiate(prefabToSpawn, hit[0].pose.position, Quaternion.identity);
            }
        }
    }

    public void ValidatePlacement()
    {
        objectPlaced = true;
    }

    public void ScaleModif(int scaleFactor)
    {
        placedObj.transform.localScale += Vector3.one * scaleFactor;
    }

    public void Move(Vector3 dir)
    {
        placedObj.transform.position += dir;
    }

    #endregion

    #region GameReLated
    void ShootTarget()
    {
        if (!objectPlaced || timer <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            ammo--;

            RaycastHit hit;
            Ray rayCam = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(rayCam, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Target") && ammo > 0)
                {
                    score++;
                    Destroy(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Ammo"))
                {
                    ammo++;
                    Destroy(hit.collider.gameObject);
                }
            }
        }
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
        else if (other.CompareTag("Target"))
        {
            life--;
            Destroy(other.gameObject);
        }
    }

    void CheckEnd()
    {
        if (!objectPlaced) return;

        if ((timer <= 0 || life <= 0) && !endDoOnce)
        {
            endDoOnce = true;
            endEvent?.Invoke();
        }
        else if (timer > maxTimer) timer -= Time.deltaTime;
    }


    public void GameReset()
    {
        timer = maxTimer;
        score = 0;
        ammo = baseAmmo;
        endDoOnce = false;
    }
    #endregion
}