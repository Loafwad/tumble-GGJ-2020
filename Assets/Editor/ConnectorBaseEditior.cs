using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[CustomEditor(typeof(ConnectorFixed), true)]
public class ConnectorBaseEditior : Editor
{
	void OnSceneGUI()
	{
		ConnectorBase connector = (ConnectorBase)target;

		for (int i = 0; i < connector.connectorsPositions.Count; ++i)
		{
			Vector3 nextPosition = connector.transform.TransformPoint(connector.connectorsPositions[i]);

			EditorGUI.BeginChangeCheck();
			Vector3 newPosition = Handles.PositionHandle(nextPosition, Quaternion.identity);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(connector, "Connector moved");
				connector.connectorsPositions[i] = newPosition -  connector.transform.position;
			}
		}
	}
}
