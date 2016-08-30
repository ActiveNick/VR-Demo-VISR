using UnityEngine;
using System.Collections;

public class Aimpoint : MonoBehaviour {

    public float PushBack = 0.2f;
    public GameObject Reticle;

    int previousTouchCount = 0;

	void Update () {
        RaycastHit rayHit;
        if(Physics.Raycast(new Ray(transform.position, transform.forward), out rayHit))
        {
            Reticle.SetActive(true);    
            Reticle.transform.position = rayHit.point + -transform.forward * PushBack;
            Reticle.transform.rotation.SetLookRotation(-transform.forward);

            int touchCount = Input.touchCount;
            if (Input.GetMouseButtonDown(0))
            {
                touchCount = 1;
                previousTouchCount = 0;
            }

            LookReciever reciever = rayHit.collider.GetComponentInParent<LookReciever>();
            if (reciever != null)
             reciever.LookRecieved(rayHit, touchCount > 0 && previousTouchCount == 0);

            previousTouchCount = touchCount;
        }
        else
        {
            Reticle.SetActive(false);
        }
	}
}
