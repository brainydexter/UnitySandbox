using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Vuforia;

namespace DemoVuforia
{
/// <summary>
/// Tracking status.
/// </summary>
public enum TrackingStatus
{
	/// <summary>
	/// Tracker is found
	/// </summary>
	FOUND,

	/// <summary>
	/// Tracking lost and not found
	/// </summary>
	NOT_FOUND

	/*
	 * Could potentially add more states for transition if the user needs
	 */
}

/// <summary>
/// Image interface tracker service. 
/// Interface allows the implementation to be easily swapped with something else
/// </summary>
public interface IImageTrackerService
{
//	#region Image tracking
//	void RegisterImageTracking ();
//	void UnregisterImageTracking ();
//	#endregion

	#region Event Handling

	/// <summary>
	/// Registers the callback for the given trackable and tracking event type - found/not found
	/// </summary>
	/// <param name="trackableName">Trackable name.</param>
	/// <param name="status">Status.</param>
	/// <param name="eventHandler">Event handler.</param>
	void RegisterTrackerEvent (string trackableName, TrackingStatus status, Action<Transform> eventHandler);

	/// <summary>
	/// Unregisters the callback for the given trackable and tracking event type - found/not found
	/// </summary>
	/// <param name="trackableName">Trackable name.</param>
	/// <param name="status">Status.</param>
	/// <param name="eventHandler">Event handler.</param>
	void UnregisterTrackerEvent (string trackableName, TrackingStatus status, Action<Transform> eventHandler);

	/// <summary>
	/// Determines whether the service is tracking the specified image(trackableName)
	/// </summary>
	/// <returns><c>true</c> if the service is tracking the specified trackableName; otherwise, <c>false</c>.</returns>
	/// <param name="trackableName">Trackable name.</param>
	bool IsTracking(string trackableName);
	#endregion
}

/// <summary>
/// Image tracker service implementation
/// </summary>
public sealed class ImageTrackerService : Singleton<ImageTrackerService>, IImageTrackerService {

	#region Mono Methods

	void Update () {

		// doing init here since trackable behaviors were not showing up at Start().
		// Ideally, this should be done during init time

		if (!inited) {
			var trackableBehaviors = TrackerManager.Instance.GetStateManager ().GetTrackableBehaviours ();

			// adding event handling tracker
			foreach (var item in trackableBehaviors) {
				Debug.Log (item.TrackableName);

				// updating the tracker game object
				item.gameObject.name = item.TrackableName + " Tracker";

				var trackingHandler = new TrackingEventHandler (item.transform);

				// associating tracking handler with the tracker
				item.RegisterTrackableEventHandler (trackingHandler);

				// storing a reference of the tracker with handler
				eventHandlers.Add (item.TrackableName, trackingHandler);
			}

			inited = true;
		}
	}

	#endregion

	#region IImageTrackerService Methods

	/// <summary>
	/// Registers the callback for the given trackable and tracking event type - found/not found
	/// </summary>
	/// <param name="trackableName">Trackable name.</param>
	/// <param name="status">Status.</param>
	/// <param name="eventHandler">Event handler.</param>
	public void RegisterTrackerEvent (string trackableName, TrackingStatus status, Action<Transform> subscriber)
	{
		Debug.Assert (subscriber != null, "[ImageTrackerService]: callback cannot be null");
		TrackingEventHandler trackingHandler = null;

		if (eventHandlers.TryGetValue (trackableName, out trackingHandler)) {
			if(status == TrackingStatus.FOUND)
				trackingHandler.AddTrackingFoundCallback (subscriber);
			else
				trackingHandler.AddTrackingLostCallback (subscriber);
		} else {
			Debug.LogWarning ("[ImageTrackerService]: Registration failed. Not tracking " + trackableName);
		}
	}

	/// <summary>
	/// Unregisters the callback for the given trackable and tracking event type - found/not found
	/// </summary>
	/// <param name="trackableName">Trackable name.</param>
	/// <param name="status">Status.</param>
	/// <param name="eventHandler">Event handler.</param>
	public void UnregisterTrackerEvent (string trackableName, TrackingStatus status, Action<Transform> subscriber)
	{
		Debug.Assert (subscriber != null, "[ImageTrackerService]: callback cannot be null");
		TrackingEventHandler trackingHandler = null;

		if (eventHandlers.TryGetValue (trackableName, out trackingHandler)) {
			if(status == TrackingStatus.FOUND)
				trackingHandler.RemoveTrackingFoundCallback (subscriber);
			else
				trackingHandler.RemoveTrackingLostCallback (subscriber);
		} else {
			Debug.LogWarning ("[ImageTrackerService]: Registration failed. Not tracking " + trackableName);
		}
	}

	/// <summary>
	/// Determines whether the service is tracking the specified image(trackableName)
	/// </summary>
	/// <returns><c>true</c> if the service is tracking the specified trackableName; otherwise, <c>false</c>.</returns>
	/// <param name="trackableName">Trackable name.</param>
	public bool IsTracking(string trackableName){
//		Debug.Assert (eventHandlers.Count != 0, "[ImageTrackerService]: Service is not tracking any trackers");
		return eventHandlers.ContainsKey (trackableName);
	}

	#endregion

	#region members

	/// <summary>
	/// The event handlers mapping for O(1) lookup
	/// </summary>
	private Dictionary<string, TrackingEventHandler> eventHandlers = new Dictionary<string, TrackingEventHandler> ();

	private bool inited = false;
	#endregion
}
}