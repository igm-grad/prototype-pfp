using UnityEngine;
using System.Collections;

public class CycleJamInitializer : MonoBehaviour 
{
	public CycleManager.CycleState initialState = CycleManager.CycleState.MainMenu;

	void Start ()
	{
		CycleManager.Instance.FirstDigit = 0;
		CycleManager.Instance.SecondDigit = 0;
		CycleManager.Instance.ThirdDigit = 0;
		CycleManager.Instance.FourthDigit = 0;
		CycleManager.jamState = initialState;
	}
}
