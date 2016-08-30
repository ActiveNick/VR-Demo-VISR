using UnityEngine;
using System.Collections;

public enum LookState
{
    None,
    Gaze,
    Action,
    Count
}

public class LookReciever : MonoBehaviour {
    public LookState CurrentState = LookState.None;

    bool lastButtonState = false;

	public void LookRecieved(RaycastHit lookRay, bool actionButton)
    {
        if(lastButtonState != actionButton)
        {
            SendMessage("OnLookStateAction", lookRay, SendMessageOptions.DontRequireReceiver);
        }

        SendMessage("OnLookStateChanged", CurrentState, SendMessageOptions.DontRequireReceiver);
        lastButtonState = actionButton;
    }
}
