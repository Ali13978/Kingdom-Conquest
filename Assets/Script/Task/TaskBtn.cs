using UnityEngine;
using System.Collections;


// 世界地图切入主场景时会显示不正常 ...
public class TaskBtn : MonoBehaviour {
	
	public TaskManager taskmanager = null;
	
	void OnEnable()
	{
		taskmanager.SetUnReadIcon();
	}
}
