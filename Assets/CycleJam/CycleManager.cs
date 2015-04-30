using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CycleManager : Singleton<CycleManager> 
{
	const float timePerLevel = 46f;

	#region Cycle Memory Digits - [Four numbers 0-9]
	private int firstDigit;
	public int FirstDigit
	{
		get
		{
			return firstDigit;
		}
		set
		{
			if (value >= 0 && value <= 9)
			{
				firstDigit = value;
			}
			else
			{
				Debug.LogError("Assigned value for a digit was not between 0 and 9.\n");
			}
		}
	}

	private int secondDigit;
	public int SecondDigit
	{
		get
		{
			return secondDigit;
		}
		set
		{
			if (value >= 0 && value <= 9)
			{
				secondDigit = value;
			}
			else
			{
				Debug.LogError("Assigned value for a digit was not between 0 and 9.\n");
			}
		}
	}

	private int thirdDigit;
	public int ThirdDigit
	{
		get
		{
			return thirdDigit;
		}
		set
		{
			if (value >= 0 && value <= 9)
			{
				thirdDigit = value;
			}
			else
			{
				Debug.LogError("Assigned value for a digit was not between 0 and 9.\n");
			}
		}
	}

	private int fourthDigit;
	public int FourthDigit
	{
		get
		{
			return fourthDigit;
		}
		set
		{
			if (value >= 0 && value <= 9)
			{
				fourthDigit = value;
			}
			else
			{
				Debug.LogError("Assigned value for a digit was not between 0 and 9.\n");
			}
		}
	}
	#endregion
	#region Digit Helper Methods - GetDigits
	public int GetDigits()
	{
		return (firstDigit * 1000 + secondDigit * 100 + thirdDigit * 10 + fourthDigit);
	}

	public string GetDigitString()
	{
		return ("" + (firstDigit) + (secondDigit) + (thirdDigit) + fourthDigit);
	}
	#endregion

	public bool cycleForever = true;

	public enum CycleState { MainMenu, Paused, Playing, End };
	public static CycleState jamState;

	public static bool hideCursor = false;

	#region Time Variables & TimeIncreasing helper method
	/// <summary>
	/// This is whether or not time is counted at all.
	/// </summary>
	private static bool enableTime = true;
	private static float timeLeft;
	public float TimeLeft
	{
		get
		{
			return timeLeft;
		}
	}

	public void AdjustTimeLeft(float timeAdjustment)
	{
		Debug.Log("Adjusting time left by : " + timeAdjustment + "\n");
		timeLeft += timeAdjustment;
	}
	#endregion

	public List<JamEntry> entriesPlayed;
	public List<JamEntry> entriesNotPlayed;

	/// <summary>
	/// A List of GameObject Singletons that developers have added to the CycleManager's records. These objects are deleted when a new JamEntry is loaded to prevent DontDestroyOnLoad issues.
	/// </summary>
	public List<GameObject> singletonRecords;

	#region GUI Input Collection Variables
	private float first;
	private float second;
	private float third;
	private float fourth;
	#endregion

	#region JamEntry class definition
	public class JamEntry
	{
		public string versionNum;
		public string teamName;
		public string entryName;
		public string identifier;
		public List<string> scenes;

		public JamEntry()
		{
			teamName = "";
			entryName = "";
			identifier = "";
			scenes = new List<string>();
		}
	}
	#endregion

	#region Awake & Start
	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
		timeLeft = timePerLevel;
		jamState = CycleState.Playing;
		singletonRecords = new List<GameObject>();
		
	}
	void Start()
	{
		enableTime = true;
		
		entriesPlayed = new List<JamEntry>();
		entriesNotPlayed = FindJamEntries();
	}

	/// <summary>
	/// Locates all TextAsset in Resources/Jam.
	/// Uses ParseJamEntry to handle the text of each JamEntry TextAsset.
	/// Creates and returns a list of legal JamEntry objects. (Doesn't add incorrect JamEntry objects)
	/// This code is duplicated in CycleJam.cs & CycleManager.cs, one being runtime the other being editor. Any changes should be made to both.
	/// </summary>
	/// <returns></returns>
	List<JamEntry> FindJamEntries()
	{
		TextAsset[] jams = Resources.LoadAll<TextAsset>("Jam");

		List<JamEntry> jamEntries = new List<JamEntry>();

		for (int i = 0; i < jams.Length; i++)
		{
			//Parses the string and turns it into a Jam Entry object.
			JamEntry newEntry = ParseJamEntry(jams[i].text);
			if (newEntry != null)
			{
				jamEntries.Add(newEntry);
			}
		}

		return jamEntries;
	}

	/// <summary>
	/// Parses a string for contents of a valid JamEntry. Will return null on failure.
	/// </summary>
	/// <param name="fullString">A string of a JamEntry.
	/// Line[1] = Version of Editor Tool.
	/// Line[3] = Entry's team name.
	/// Line[5] = Entry name.
	/// Line[7] = Entry Identifier.
	/// Line[9] = The names of the scenes separated by commas.
	/// This code exists in CycleJam.cs & CycleManager.cs, one being a runtime script, the other being an editor script. Be aware of this when editing.</param>
	/// <returns></returns>
	JamEntry ParseJamEntry(string fullString)
	{
		try
		{
			JamEntry entry = new JamEntry();

			char newLineSplit = '\n';
			char commaSplit = ',';

			string[] segments = fullString.Split(newLineSplit);

			entry.versionNum = segments[1];
			entry.teamName = segments[3];
			entry.entryName = segments[5];
			entry.identifier = segments[7];

			string[] sceneSegments = segments[9].Split(commaSplit);

			for (int j = 0; j < sceneSegments.Length; j++)
			{
				entry.scenes.Add(sceneSegments[j]);
			}

			return entry;
		}
		catch
		{
			Debug.LogError("CycleManager:\nImproper Asset in a Resources/Jam folder. CycleManager handled gracefully.\n");
			return null;
		}
	}
	#endregion

	#region [Important Methods] - Update, GetInput, OnGUI
	void Update()
	{
		switch (CycleManager.jamState)
		{
			#region Main Menu
			case CycleState.MainMenu:

				break;
			#endregion

			#region Paused
			case CycleState.Paused:
				//When we pause, ALWAYS reveal & unlock the cursor
				Screen.showCursor = true;
				Screen.lockCursor = false;
				break;
			#endregion

			#region Playing
			case CycleState.Playing:

				//If a dev wants to hide the cursor, set it to hidden every frame. There are cleaner solutions. This one ensures that the cursor is hidden or visible when it needs to be at minimal operational complexity.
				if (hideCursor)
				{
					Screen.showCursor = false;
					Screen.lockCursor = true;
				}
				else
				{
					Screen.showCursor = true;
					Screen.lockCursor = false;
				}

				//Support for the stop clock button.
				if (enableTime)
				{
					timeLeft -= Time.deltaTime;
				}
				break;
			#endregion

			#region End Scene
			case CycleState.End:

				break;
			#endregion
		}

		#region Get Input
		GetInput();
		#endregion

		#region Check next scene
		//Provides support for a Main & Pause menus to advance to next JamEntry.
		if (timeLeft <= 0)
		{
			LoadRandomEntry();
		}
		#endregion
	}

	void GetInput()
	{
		#region Check Paused
		if (Input.GetKeyDown(KeyCode.Escape)) // This does not support controllers currently. This would be ideally updated.
		{
			if (CycleManager.jamState == CycleState.Paused)
			{
				ResumePlay();
			}
			else if(CycleManager.jamState == CycleState.Playing)
			{
				PausePlay();
			}
		}
		#endregion

		#region Stop Clock
		if (Input.GetKeyDown(KeyCode.Equals))
		{
			enableTime = !enableTime;
		}
		#endregion
	}

	void OnGUI()
	{
		switch (CycleManager.jamState)
		{
			#region Main Menu
			case CycleState.MainMenu:
				if (GUI.Button(new Rect(Screen.width / 3, Screen.height - 250, Screen.width / 3, 100), "Begin CycleJam!"))
				{
					entriesNotPlayed = FindJamEntries();
					entriesPlayed.Clear();

					ResumePlay();

					//This is somewhat hacky. We only ever advance scenes through time running out.
					AdvanceToNext();

				}

				DrawQuitButton();

				#region Cycle Forever Button
				string buttonText = "Cycle Forever?\n";

				//Setting text based on which creates more personality.
				if (CycleManager.Instance.cycleForever)
				{
					buttonText += "Definitely";
				}
				else
				{
					buttonText += "Just once please.";
				}
				//Draw the button
				if (GUI.Button(new Rect(30, Screen.height - 170, 150, 40), buttonText))
				{
					//Toggle the boolean.
					CycleManager.Instance.cycleForever = !CycleManager.Instance.cycleForever;
				}
				#endregion

				#region Main Menu Digit Setting
				//A box for this little submenu
				GUI.Box(new Rect(10, Screen.height - 120, 190, 110), "Code Control");

				//A set of horizontal sliders for each. This uses some private floats to hold the values in the meantime.
				first = GUI.HorizontalSlider(new Rect(20, Screen.height - 90, 150, 20), first, 0, 9);
				second = GUI.HorizontalSlider(new Rect(20, Screen.height - 70, 150, 20), second, 0, 9);
				third = GUI.HorizontalSlider(new Rect(20, Screen.height - 50, 150, 20), third, 0, 9);
				fourth = GUI.HorizontalSlider(new Rect(20, Screen.height - 30, 150, 20), fourth, 0, 9);

				//We assign our digit values the integer values. The reason this works is because it keeps assigning the float to 3.4, but once it moves to 3.6, it will round up to 4, creating a integer slider.
				FirstDigit = (int)first;
				SecondDigit = (int)second;
				ThirdDigit = (int)third;
				FourthDigit = (int)fourth;

				//Reassign our new value so the slider snaps to integer values every frame.
				first = (int)first;
				second = (int)second;
				third = (int)third;
				fourth = (int)fourth;

				//Draw labels with the Digits so the user can easily see what values they have selected.
				GUI.Label(new Rect(180, Screen.height - 95, 10, 20), FirstDigit.ToString());
				GUI.Label(new Rect(180, Screen.height - 75, 10, 20), SecondDigit.ToString());
				GUI.Label(new Rect(180, Screen.height - 55, 10, 20), ThirdDigit.ToString());
				GUI.Label(new Rect(180, Screen.height - 35, 10, 20), FourthDigit.ToString());
				#endregion
				break;
			#endregion

			#region Paused
			case CycleState.Paused:

				//Contains no base pause menu currently. Individual devs make a pause menu on top of this.
				GUI.Box(new Rect(-5, -5, Screen.width + 5, Screen.height + 5), "");


				DrawTimeRemaining();

				//Pause menu elements
				DrawNextButton();
				DrawStopClockButton();
				DrawQuitButton();
				break;
			#endregion

			#region Playing
			case CycleState.Playing:

				DrawTimeRemaining();
				break;
			#endregion

			#region End
			case CycleState.End:
				//Need a way to reset to main menu to keep the play loop going.
				if (GUI.Button(new Rect(Screen.width / 3, Screen.height - 60, Screen.width / 3, 40), "Back to Main Menu!"))
				{
					ResumePlay();
					CycleManager.jamState = CycleState.MainMenu;
					Application.LoadLevel(0);
				}

				DrawQuitButton();
				break;
			#endregion
		}

#if UNITY_EDITOR
		//Will only draw in the editor. Will not draw in built versions
		DrawDebugInfo(CycleManager.jamState.ToString() + "\n" + GetDigitString());
#else
			if (Debug.isDebugBuild) 
			{
				DrawDebugInfo(CycleManager.jamState.ToString() + "\n" + GetDigitString());
			}
#endif
	}
	#endregion

	#region Helper Methods - Pause, Resume, Advance to Next JamEntry, & Load Random Entry
	/// <summary>
	/// Handles State Transition from [Playing -> Paused]
	/// Sets Timescale to 0
	/// </summary>
	void PausePlay()
	{
		CycleManager.jamState = CycleState.Paused;
		Time.timeScale = 0.0f;

		//Stub code for supporting a universal AudioBus
		/*if (transform.FindChild("AudioBus") != null)
		{
			AudioSource playerBus = transform.FindChild("AudioBus").audio;
			if (playerBus.isPlaying)
			{
				transform.FindChild("AudioBus").audio.Pause();
				wasPlaying = true;
			}
		}*/
	}

	/// <summary>
	/// Handles State Transition from [Paused -> Playing]
	/// Sets Timescale to 1
	/// </summary>
	void ResumePlay()
	{
		CycleManager.jamState = CycleState.Playing;
		Time.timeScale = 1.0f;

		//Stub code for supporting a universal AudioBus
		/*if (wasPlaying)
		{
			transform.FindChild("AudioBus").audio.Play();
		}*/
	}

	/// <summary>
	/// A single multiplatform approach for quitting the application/returning to the main menu.
	/// </summary>
	public void QuitCycle()
	{
#if UNITY_WEBPLAYER
		enableTime = true;
		ResumePlay();
		CycleManager.jamState = CycleState.MainMenu;
		Application.LoadLevel(0);
		enableTime = true;
#elif UNITY_EDITOR
		enableTime = true;
		ResumePlay();
		CycleManager.jamState = CycleState.MainMenu;
		Application.LoadLevel(0);
		enableTime = true;
#elif UNITY_STANDALONE_WIN
		Application.Quit();
#endif
	}

	/// <summary>
	/// Subtracts all remaining time so upon next frame the game will advance.
	/// </summary>
	public void AdvanceToNext()
	{
		CycleManager.Instance.AdjustTimeLeft(-CycleManager.Instance.TimeLeft);
	}

	/// <summary>
	/// Loads a random JamEntry that has not been played in the current cycle.
	/// Updates the EntriesPlayed List.
	/// </summary>
	void LoadRandomEntry()
	{
		int nextLevel = -1;
		int nextEntry = -1;

		//This is for output information.
		System.Text.StringBuilder sb = new System.Text.StringBuilder();

		//EntriesNotPlayed is reset every loop. If we still have an entries we have not played this loop
		if (entriesNotPlayed.Count > 0)
		{
			//We need to reset the time.
			timeLeft = timePerLevel;

			//Pick one of the unplayed entries at random.
			nextEntry = Random.Range(0, entriesNotPlayed.Count);

			sb.AppendLine("Changing Scene from \t[" + Application.loadedLevel + "]" + Application.loadedLevelName + "\tto\t" + entriesNotPlayed[nextEntry].scenes[0]);

			JamEntry selectedEntry = entriesNotPlayed[nextEntry];

			//Move the selected entry from the 'Not played' to the 'Played' list.
			entriesNotPlayed.RemoveAt(nextEntry);
			entriesPlayed.Add(selectedEntry);

			//Make sure we update the state otherwise it'll stay paused in the new entry.
			CycleManager.jamState = CycleState.Playing;

			//Clear any existing singletons from the previous scene out.
			ClearSingletons();

			//Load the first listed scene in the Selected Entry.
			Application.LoadLevel(selectedEntry.scenes[0]);
		}
		//This case occurs when we have no entries left unplayed this loop.
		else
		{
			//If we want to cycle forever like Groundhog day, just reset the list of unplayed entries.
			if (cycleForever)
			{
				entriesNotPlayed = FindJamEntries();

				//Note, we don't reset EntriesPlayed because that list will constantly grow in length giving a 'play path'
				//This does not support someone playing more entries than the maximum size of a list, but this is an unlikely problem.
			}
			//If we don't want to play forever, we go to the credits.
			else
			{
				//The credits scene should be the last scene. Could also load it by name.
				nextLevel = Application.levelCount - 1;
				sb.AppendLine("Changing to Credits Scene.");

				CycleManager.jamState = CycleState.End;

				//Clear any existing singletons from the previous scene out.
				ClearSingletons();

				Application.LoadLevel(nextLevel);

				//No time limit on the credits scene.
				timeLeft = float.MaxValue;

				//Don't even show the clock on the credits scene.
				enableTime = false;
			}
		}


#if UNITY_EDITOR
		//Will only print out in dev builds or in the editor. Minor computation saving in built versions.
		ConcatOutput(sb);
#else
		if (Debug.isDebugBuild) 
		{
			ConcatOutput(sb);
		}
#endif


	}
	#endregion

	#region Draw Menus and Info Panels
	/// <summary>
	/// Calls AdvanceToNext. Has hard coded location.
	/// </summary>
	void DrawNextButton()
	{
		if (GUI.Button(new Rect(Screen.width - 110, Screen.height - 150, 100, 40), "Next Entry"))
		{
			AdvanceToNext();
		}
	}

	/// <summary>
	/// Toggles enableTime. Has hard coded location & Context dependent button text.
	/// </summary>
	void DrawStopClockButton()
	{
		//Changes the text depending upon the context.
		string buttonText = "Stop Clock";
		if (!enableTime)
		{
			buttonText = "Resume Clock";
		}
		if (GUI.Button(new Rect(Screen.width - 110, Screen.height - 100, 100, 40), buttonText))
		{
			enableTime = !enableTime;
		}
	}

	/// <summary>
	/// Quits dependent on platform. Has little testing. Hard coded location.
	/// </summary>
	void DrawQuitButton()
	{
		if (GUI.Button(new Rect(Screen.width - 110, Screen.height - 50, 100, 40), "Quit"))
		{
			QuitCycle();
		}
	}

	/// <summary>
	/// Draws the remaining time in the top left corner. Draws an indicator for when the clock is stopped.
	/// </summary>
	void DrawTimeRemaining()
	{
		//Draw a GUI box in top right with the remaining time as well as the name of the game.
		GUI.Box(new Rect(Screen.width * .1f - 40, Screen.height * .1f, 80, 50), "Time Left:\n" + (int)timeLeft);

		if (!enableTime)
		{
			GUI.Box(new Rect(Screen.width * .1f - 50, Screen.height * .1f + 60, 100, 25), "Clock Stopped");
		}
	}

	/// <summary>
	/// Will display the current game digit data.
	/// </summary>
	/// <param name="info"></param>
	public void DrawDebugInfo(string info)
	{
		GUI.Box(new Rect(Screen.width * .15f, Screen.height * .02f, Screen.width * .70f, 60), info);
		GUI.Label(new Rect(Screen.width * .15f + 10, Screen.height * .02f + 10, Screen.width * .1f, 30), "Dev Only");
	}
	
	/// <summary>
	/// [UNUSED] Support for a universal pause menu.
	/// </summary>
	/// <param name="windowID"></param>
	void DrawPauseMenu(int windowID)
	{
		if (GUI.Button(new Rect(10, 20, 180, 80), "Resume"))
		{
			ResumePlay();
		}

		if (GUI.Button(new Rect(10, 110, 180, 80), "Settings & Controls"))
		{
			//menuMode = 1;
		}

		if (GUI.Button(new Rect(10, 200, 180, 80), "About"))
		{
			//menuMode = 2;
		}

		if (GUI.Button(new Rect(10, 290, 180, 80), "Quit"))
		{
			QuitCycle();
		}
	}
	#endregion

	/// <summary>
	/// Appends the played and unplayed JamEntries before printing a single verbose log with other provided information.
	/// </summary>
	/// <param name="sb">A stringbuilder that is populated in LoadRandomEntry.</param>
	public void ConcatOutput(System.Text.StringBuilder sb)
	{
		for (int i = 0; i < entriesPlayed.Count; i++)
		{
			sb.AppendLine("Played: " + entriesPlayed[i].entryName);
		}
		sb.AppendLine();
		for (int i = 0; i < entriesNotPlayed.Count; i++)
		{
			sb.AppendLine("Not Played: " + entriesNotPlayed[i].entryName);
		}

		//Debug.Log("CycleManager: " + sb.ToString() + "\n\n\n\n\n\n");
	}

	/// <summary>
	/// For registering a singleton to be cleared out upon Entry change. Will not clear on scene change.
	/// </summary>
	/// <param name="newSingleton">The GameObject reference. Do not pass in an individual component.</param>
	public void RegisterSingleton(GameObject newSingleton)
	{
		if (!singletonRecords.Contains(newSingleton))
		{
			singletonRecords.Add(newSingleton);
		}
		else
		{
			Debug.LogError("CycleManager:\nAttempted to add duplicate singleton to SingletonRecords.\n");
		}
	}

	/// <summary>
	/// Called by LoadRandomEntry upon scene changes. Deletes the singleton gameobjects and then resets the records for the next JamEntry's singletons (if any)
	/// </summary>
	public void ClearSingletons()
	{
		//For each singleton on record
		for (int i = 0; i < singletonRecords.Count; i++)
		{
			//Destroying the .gameObject to make sure someone didn't hand in a script. Need to test this a bit further.
			GameObject.Destroy(singletonRecords[i].gameObject);
		}
		//Then clear the list and then reset capacity.
		singletonRecords.Clear();
		singletonRecords = new List<GameObject>();
	}
}
