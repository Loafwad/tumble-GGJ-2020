using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static bool dontDestroyOnLoad = false;

	// Check to see if we're about to be destroyed.
	private static bool shuttingDown = false;
	private static object lockObject = new object();
	private static T _instance;

	public static T instance
	{
		get
		{
			if (shuttingDown)
			{
				Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
					"' already destroyed. Returning null.");
				return null;
			}

			lock (lockObject)
			{
				if (_instance == null)
				{
					// Search for existing instance.
					_instance = (T)FindObjectOfType(typeof(T));

					// Create new instance if one doesn't already exist.
					if (_instance == null)
					{
						// Need to create a new GameObject to attach the singleton to.
						var singletonObject = new GameObject();
						_instance = singletonObject.AddComponent<T>();
						singletonObject.name = typeof(T).ToString() + " (Singleton)";

						if (dontDestroyOnLoad)
						{
							DontDestroyOnLoad(singletonObject);
						}
					}
				}

				return _instance;
			}
		}
	}


	private void OnApplicationQuit()
	{
		shuttingDown = true;
	}


	private void OnDestroy()
	{
		shuttingDown = true;
	}
}
