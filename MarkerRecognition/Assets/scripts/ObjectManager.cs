using UnityEngine;
using System.Collections;

public class ObjectManager : MonoBehaviour {

	void Update()
	{
		if (ImageTrackerService.Instance.ContainsTracker("stones")) {
			ImageTrackerService.Instance.RegisterTrackerEvent("stones", TrackingStatus.FOUND, HandleStonesFound);
			ImageTrackerService.Instance.RegisterTrackerEvent("stones", TrackingStatus.NOT_FOUND, HandleStonesNotFound);
		}
	}

	void HandleStonesFound (Transform obj)
	{
//		Debug.Log ("HandleStones triggered");
		if (!stoneGO) {
			stoneGO = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			stoneGO.transform.parent = obj;
			stoneGO.transform.localScale = Vector3.one * 0.125f;
		}

		stoneGO.gameObject.SetActive (true);
	}

	void HandleStonesNotFound (Transform obj)
	{
//		Debug.Log ("HandleStones not found triggered");
		stoneGO.gameObject.SetActive (false);
	}

	private GameObject stoneGO;
}
