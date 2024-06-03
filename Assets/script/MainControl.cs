using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Events;
using System.Linq;

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
	TextMeshProUGUI textScore, textAmmo, textChrono, textLife, endScoreText;

    [Header("Set up related")]

    [SerializeField]
    ARRaycastManager raycastAR;

    [SerializeField]
    GameObject prefabToSpawn;

    [SerializeField]
    UnityEvent endEvent;

    //------------------------------------------------------
    //private variable

    bool canplay;

    bool objectPlaced;
    GameObject placedObj;

    bool endDoOnce;
    float timer;

    int score;
    int ammo;
    int life;

    public void StartGame()
    {
        canplay = true;
    }

    private void Start()
    {
        GameReset();
    }

    private void Update()
    {
        if (!canplay) return;

        CheckEnd();
        PlaceObject();
        ShootTarget();

        UpdateText();
    }

    //update the visible text
    void UpdateText()
    {
        textChrono.text = (Mathf.Round(timer * 10) / 10).ToString();

        textScore.text = "Score: " + score;
        textAmmo.text = "Ammo: " + ammo;
        textLife.text = "Life: " + life;
        endScoreText.text = "Final Score: \n" + score;
    }

    #region ObjectPlacement
    //place the generator on the the ar plane
    void PlaceObject()
    {
        if (placedObj != null) return;

        List<ARRaycastHit> hit = new();

        if (Input.GetMouseButtonDown(0))
        {
            if (raycastAR.Raycast(Input.mousePosition, hit, TrackableType.PlaneWithinPolygon))
            {
                placedObj = Instantiate(prefabToSpawn, hit[0].pose.position, Quaternion.identity);
            }
        }
    }

    //--------------------------------------------
    //button called
    public void ValidatePlacement()
    {
        if (!placedObj) return;

        placedObj.GetComponent<MainObjectControl>().actif = true;
        placedObj.GetComponent<MainObjectControl>().ResetAll();
        objectPlaced = true;
    }

    public void ScaleModif(float scaleFactor)
    {
        if (scaleFactor < 0 && placedObj.transform.localScale.x < 0.2f) return;

        placedObj.transform.localScale += Vector3.one * scaleFactor;
    }

    public void MoveForward(float dir)
    {
        placedObj.transform.position += new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z) * dir;
    }

    public void MoveRight(float dir)
    {
        placedObj.transform.position += new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z) * dir;
    }

    #endregion

    #region GameReLated
    //Raycast to detect the object
    void ShootTarget()
    {
        if (!objectPlaced || timer <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray rayCam = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(rayCam, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Target") && ammo > 0)
                {
                    score++;
                    ammo--;
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
            placedObj.GetComponent<MainObjectControl>().actif = false;
            endEvent?.Invoke();
        }
        else if (timer > maxTimer) timer -= Time.deltaTime;
    }


    public void GameReset()
    {
        timer = maxTimer;
        score = 0;
        ammo = baseAmmo;
        life = maxLife;
        endDoOnce = false;

        if (placedObj != null)
        {
            placedObj.GetComponent<MainObjectControl>().actif = true;
            placedObj.GetComponent<MainObjectControl>().ResetAll();
        }
    }
    #endregion
}