using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Vehicles.Car;

public class CarManager : MonoBehaviour
{

    public enum CarMode
    {
        parked,
        player,
        npc

    }

    public CarMode CarStatus;
    private CarUserControl playerControl;
    private CarAIControl npcControl;
    private GameObject m_player = null;
    private bool backWards = false;
    private float backwardsTime = 0f;
    public float maxBackwardsTime = 1.5f;
    public Road lastRoad;
    public bool isRoadA;
    private GameObject target;
    // Start is called before the first frame update
    void Awake()
    {
        //obtenemos los componentes
        playerControl = GetComponent<CarUserControl>();
        npcControl = GetComponent<CarAIControl>();

        target = new GameObject();
        target.transform.parent = transform;
        isRoadA = true;
        CheckControls();
    }


    // Update is called once per frame
    void Update()
    {
        
        if ((CarStatus == CarMode.player) && Input.GetButtonDown("Action"))
        {
            FreeLookCam cam = FindObjectOfType<FreeLookCam>();
            cam.SetTarget(m_player.transform);
            m_player.transform.parent = null;
            m_player.SetActive(true);
            CarStatus = CarMode.parked;
            CheckControls();
            m_player = null;
        }
        if (CarStatus == CarMode.npc)
        {
          
            //if ((transform.position - target.transform.position).magnitude < 4)
            //{
                UpdateNavigation();
            //}
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Road")
        {
            Road newRoad = other.GetComponentInParent<Road>();
            if (lastRoad != newRoad)
            {
                lastRoad = newRoad;
                isRoadA = true;
                if (lastRoad.esCruce)
                {
                    isRoadA = other == lastRoad.roadA;

                }
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Road")
        {
            Road newRoad = other.GetComponentInParent<Road>();
            if(lastRoad != newRoad)
            {
                lastRoad = newRoad;
                isRoadA = true;
                if (lastRoad.esCruce)
                {
                    isRoadA = other == lastRoad.roadA;

                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(CarStatus == CarMode.npc)
        {
            if (collision.gameObject.tag != "Road" && collision.gameObject.tag != "Sidewalk")
            {
                float angle = Vector3.Angle(collision.transform.position - transform.position, -transform.forward);
                angle = (angle + 360) % 360;
                if (angle > 140 && angle < 220)
                {
                    backWards = true;
                    backwardsTime = 0;
                }

            }
        }


    }

    void UpdateNavigation()
    {
        if (backWards)
        {
            if(backwardsTime < maxBackwardsTime)
            {
                backwardsTime += Time.deltaTime;
                Vector3 next = getNextPoint(true);
                target.transform.position = next;
                npcControl.SetTarget(target.transform);
            }
            else
            {
                backWards = false;
                backwardsTime = 0f;
            }

        }
        else
        {
            Vector3 next = getNextPoint();
            target.transform.position = next;
            npcControl.SetTarget(target.transform);
        }

    }

    private Vector3 getNextPoint(bool backwards = false)
    {
        int bw = (backWards) ? -1 : 1;
        BoxCollider road = (isRoadA) ? lastRoad.roadA : lastRoad.roadB;
        Vector3 carFwdOnRoad = lastRoad.transform.InverseTransformVector(transform.forward);
        int carRight;
        if (isRoadA)
        {
            carRight = (carFwdOnRoad.y > 0) ? 1 : -1;
        }
        else
        {
            carRight = (carFwdOnRoad.x > 0) ? 1 : -1;
        }
        carRight *= bw;

        Vector3 CarrilDerecho = road.bounds.center + carRight * road.bounds.extents / 2;
        //Vector3 CarrilDerecho = road.center - carRight * Vector3.Scale(road.size, transform.localScale) / 4;
        //Vector3 localPosition = lastRoad.transform.InverseTransformVector(transform.position);
        Vector3 deltaVector = lastRoad.transform.InverseTransformVector(CarrilDerecho - transform.position);

        float deltaCarrilDerecho =  (isRoadA) ? deltaVector.x : deltaVector.y;
        Vector3 forward = (isRoadA) ? lastRoad.transform.up : lastRoad.transform.right;
        Vector3 right = (isRoadA) ? lastRoad.transform.right : -lastRoad.transform.up;
        Vector3 newPos = transform.position + 15* carRight * forward + carRight * deltaCarrilDerecho * right;

        return newPos;

    }

    void CheckControls() 
    { 
        //activamos los controladores solo si estamos en el modo correcto
        playerControl.enabled = (CarStatus == CarMode.player);
        npcControl.enabled = (CarStatus == CarMode.npc);
        if (npcControl.enabled)
        {
            UpdateNavigation();
        }
    }

    public void StartDriving(GameObject player)
    {
        //hacemos al player hijo nuestro y le desactivamos
        m_player = player;
        player.transform.parent = transform;
        player.SetActive(false);
        FreeLookCam cam = FindObjectOfType<FreeLookCam>();
        cam.SetTarget(transform);
        CarStatus = CarMode.player;
        CheckControls();
    }
}
