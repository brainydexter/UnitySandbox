using UnityEngine;
using System.Collections;

using DemoVuforia;

/// <summary>
/// Sample consumer of image tracker service. Many consumers can latch onto tracker found/not found events
/// </summary>
public class ObjectManager : MonoBehaviour 
{
	#region Mono Methods
	void Update()
	{
		// need to do this in update. Look at ImageTrackerService::Update() for details
		if (ImageTrackerService.Instance.IsTracking("stones")) {

			// registering callbacks for different events for all trackers
			ImageTrackerService.Instance.RegisterTrackerEvent("stones", TrackingStatus.FOUND, HandleStonesFound);
			ImageTrackerService.Instance.RegisterTrackerEvent("stones", TrackingStatus.NOT_FOUND, HandleStonesNotFound);
		}

		if (ImageTrackerService.Instance.IsTracking("chips")) {

			// registering callbacks for different events for all trackers
			ImageTrackerService.Instance.RegisterTrackerEvent("chips", TrackingStatus.FOUND, HandleChipsFound);
			ImageTrackerService.Instance.RegisterTrackerEvent("chips", TrackingStatus.NOT_FOUND, HandleChipsNotFound);
		}

		if (ImageTrackerService.Instance.IsTracking("tarmac")) {

			// registering callbacks for different events for all trackers
			ImageTrackerService.Instance.RegisterTrackerEvent("tarmac", TrackingStatus.FOUND, HandleTarmacFound);
			ImageTrackerService.Instance.RegisterTrackerEvent("tarmac", TrackingStatus.NOT_FOUND, HandleTarmacNotFound);
		}
	}

	#endregion

	#region Stone Callbacks

	/// <summary>
	/// Callback for when stones is found
	/// </summary>
	/// <param name="obj">Object.</param>
	void HandleStonesFound (Transform obj)
	{
		//		Debug.Log ("HandleStones triggered");

		// when stones is found for the first time
		if (!stoneGO) {
			// can use a object pool but that looked like overkill here
			// also can use a fancy asset and trigger animation here or whatever one fancies
			stoneGO = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			stoneGO.transform.parent = obj;
			stoneGO.transform.localScale = Vector3.one * 0.125f;
		}

		stoneGO.gameObject.SetActive (true);
	}

	/// <summary>
	/// Callback when stone tracking is lost
	/// </summary>
	/// <param name="obj">Object.</param>
	void HandleStonesNotFound (Transform obj)
	{
		//		Debug.Log ("HandleStones not found triggered");

		if(stoneGO)
			stoneGO.gameObject.SetActive (false);
	}

	#endregion

	#region Chips Callbacks

	/// <summary>
	/// Callback for when chips is found
	/// </summary>
	/// <param name="obj">Object.</param>
	void HandleChipsFound (Transform obj)
	{
		//		Debug.Log ("HandleStones triggered");

		// when stones is found for the first time
		if (!chipsGO) {
			// can use a object pool but that looked like overkill here
			// also can use a fancy asset and trigger animation here or whatever one fancies
			chipsGO = GameObject.CreatePrimitive (PrimitiveType.Cube);
			chipsGO.transform.parent = obj;
			chipsGO.transform.localScale = Vector3.one * 0.125f;
		}

		chipsGO.gameObject.SetActive (true);
	}

	/// <summary>
	/// Callback when chips tracking is lost
	/// </summary>
	/// <param name="obj">Object.</param>
	void HandleChipsNotFound (Transform obj)
	{
		if(chipsGO)
			chipsGO.gameObject.SetActive (false);
	}

	#endregion

	#region Tarmac Callbacks

	/// <summary>
	/// Callback for when Tarmac is found
	/// </summary>
	/// <param name="obj">Object.</param>
	void HandleTarmacFound (Transform obj)
	{
		// when stones is found for the first time
		if (!tarmacGO) {
			// can use a object pool but that looked like overkill here
			// also can use a fancy asset and trigger animation here or whatever one fancies
			tarmacGO = GameObject.CreatePrimitive (PrimitiveType.Capsule);
			tarmacGO.transform.parent = obj;
			tarmacGO.transform.localScale = Vector3.one * 0.125f;
		}

		tarmacGO.gameObject.SetActive (true);
	}

	/// <summary>
	/// Callback when tarmac tracking is lost
	/// </summary>
	/// <param name="obj">Object.</param>
	void HandleTarmacNotFound (Transform obj)
	{
		if(tarmacGO)
			tarmacGO.gameObject.SetActive (false);
	}

	#endregion

	#region Members

	private GameObject stoneGO;

	private GameObject chipsGO;

	private GameObject tarmacGO;

	#endregion
}