using UnityEngine;
using UnityEditor;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class CycleJam : EditorWindow
{
	[MenuItem("CycleJam/Game Manager %K")]
	static void Init()
	{
		CycleJam window = (CycleJam)EditorWindow.GetWindow(typeof(CycleJam));
	}

	#region VERSION
	//The Version of the current editor. This is pushed into the JamEntries created.
	const float version = 1.05f;
	#endregion
	#region Editor Variables & Field Values
	// Time the JamEntry assets were last checked.
	public System.DateTime lastEntryCheck;

	//The name of team/creator of a new JamEntry
	string creatorName;

	//The name of a new JamEntry
	private string entryName = "";

	private string identifier = "";

	//The text that is written to a TextAsset to store the JamEntries
	private string textAssetContents = "Test text contents";

	//The extra scene for adding more scenes to the List<LevelScene> scenes
	Object spareScene = null;

	#region Foldout Bools
	private bool showJamEntries = false;
	private bool showEntryCreation = false;
	private bool showBuildHandler = false;
	#endregion
	#endregion
	#region Scene and JamEntry Lists
	[SerializeField]
	List<LevelScene> scenes;

	[SerializeField]
	List<JamEntry> jEntries;
	#endregion

	void OnGUI()
	{

		GUILayout.Label("Cycle Jam Game Manager", EditorStyles.largeLabel);
		DrawDivider();

		CheckEntries(true);
		//if (GUILayout.Button("Refresh Entries"))
		//{
		//}
		//else if(System.DateTime.Now.Subtract(lastEntryCheck).Seconds > 60)
		//{
		//	CheckEntries(false);
		//}

		EditorGUILayout.BeginVertical();
		#region Known Jam Entries
		if (showJamEntries)
		{
			showJamEntries = EditorGUILayout.Foldout(showJamEntries, "Hide Jam Entries");
			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Jam Entries Found: " + jEntries.Count, EditorStyles.largeLabel);
			for (int i = 0; i < jEntries.Count; i++)
			{
				EditorGUILayout.HelpBox(jEntries[i].entryName + "\n\tby " + jEntries[i].teamName, MessageType.None);
			}
			EditorGUILayout.EndVertical();
		}
		else
		{
			showJamEntries = EditorGUILayout.Foldout(showJamEntries, "Show Jam Entries");
		}
		#endregion
		#region Jam Entry Creation
		showEntryCreation = EditorGUILayout.Foldout(showEntryCreation, "Jam Entry Creation");
		if (showEntryCreation)
		{
			EditorGUILayout.BeginVertical("box");
			DrawJamCreation();
			EditorGUILayout.EndVertical();
		}
		#endregion
		#region Build Handling
		showBuildHandler = EditorGUILayout.Foldout(showBuildHandler, "Build Handling");
		if (showBuildHandler)
		{
			EditorGUILayout.BeginVertical("box");
			if (GUILayout.Button("Set Scenes Based on Jam Entries"))
			{
				HandleJam();
			}
			EditorGUILayout.EndVertical();
		}
		#endregion
		EditorGUILayout.EndVertical();
	}

	#region Build Pipeline Contents
	/// <summary>
	/// Will find all the Jams, then make sure all of them are in the build and all inputs are set.
	/// </summary>
	void HandleJam()
	{
		StringBuilder sb = new StringBuilder("Build Pipeline Begin");

		//The following code needs to be repurposed.
		//Find all of the JamEntry Resource Objects.
		List<string> sceneList = new List<string>();

		//These are directory contents. It is VERY important to include assetDir. There will be NO clear indication that you left it out in the Build List. the entries will just simply appear invalid.
		string assetDir = "Assets/";
		string sceneDir = "Scenes/";
		string sceneType = ".unity";

		//The name of the scene needs to be Assets/[Identifier]/Scenes/[SceneName][sceneType]

		string cycleJamFolder = "CycleJam/";
		string mainScene = "CycleStart";
		string creditsScene = "CycleCredits";

		//Add the main menu scene.
		sceneList.Add(assetDir + cycleJamFolder + sceneDir + mainScene);

		#region sceneList Population
		sb.AppendLine("  Jam Entry Count : " + jEntries.Count);
		//For every entry
		for (int i = 0; i < jEntries.Count; i++)
		{
			//For every scene in every entry
			for (int j = 0; j < jEntries[i].scenes.Count; j++)
			{
				//If our scene list doesn't contain this scene
				if (!sceneList.Contains(jEntries[i].scenes[j]))
				{
					//Uncomment this to see each scene.
					//Debug.Log("Adding to Scene List: " + assetDir + jEntries[i].identifier + "/" + sceneDir + jEntries[i].scenes[j] + "\n");

					//Add this scene to the overall list
					sceneList.Add(assetDir + jEntries[i].identifier + "/" + sceneDir + jEntries[i].scenes[j]);
				}
				//If the scene already exists, throw an error with the name of the jam entry.
				else
				{
					Debug.LogError("Scene Name Collision. Did not add scene.\nJam Entry: [" + jEntries[i].entryName + " by " + jEntries[i].teamName + "] scene " + jEntries[i].scenes[j] + " already in list.\n\n" + sceneList.ToString());
				}
			}
		}
		
		//We now have every entry's every scene from the appropriate folder (unless the scene existed in more than one entry.)
		#endregion

		//Add the credits scene.
		sceneList.Add(assetDir + cycleJamFolder + sceneDir + creditsScene);

		//We need an array for EditorBuildSettingsScene. Thank you LINQ.
		string[] sceneArray = sceneList.ToArray();

		//Size the array.
		EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[sceneArray.Length];

		//Set the array one element at a time.
		for (int i = 0; i < sceneArray.Length; i++)
		{
			sb.AppendLine("    " + sceneArray[i] + sceneType);

			//The name of the scene needs to be Assets/[Identifier]/Scenes/[SceneName][sceneType]

			EditorBuildSettingsScene sceneToAdd = new EditorBuildSettingsScene(sceneArray[i] + sceneType, true);
			newSettings[i] = sceneToAdd;
		}

		//Use this to determine if the scenes are improperly named or concatenated.
		Debug.Log(sb.ToString());

		//Set our new scene list.
		EditorBuildSettings.scenes = newSettings;
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
			jamEntries.Add(ParseJamEntry(jams[i].text));
		}

		return jamEntries;
	}
	#endregion
	
	#region JamEntry Helper Functions
	/// <summary>
	/// Checks the List of JamEntries to make sure its clean and populated.
	/// </summary>
	public void CheckEntries(bool forceRefresh)
	{
		if (jEntries == null || forceRefresh)
		{
			//Debug.Log("Refreshing JamEntries\n\n");
			lastEntryCheck = System.DateTime.Now;
			jEntries = FindJamEntries();
		}

		if (scenes == null)
		{
			scenes = new List<LevelScene>();
		}
	}

	/// <summary>
	/// Creates the actual JamEntry which stores information about the game.
	/// </summary>
	public void DrawJamCreation()
	{
		//Provide Name
		//Provide Creator
		//Provide Scenes
		//Provide Controls
		creatorName = EditorGUILayout.TextField("Name of Team", creatorName);
		entryName = EditorGUILayout.TextField("Name of Entry", entryName);
		identifier = EditorGUILayout.TextField("Entry Identifier", identifier);

		DrawDivider();

		#region Scene List
		for (int i = 0; i < scenes.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			scenes[i].scene = EditorGUILayout.ObjectField("Scene [" + i + "]", scenes[i].scene, typeof(Object), false);
			EditorGUILayout.BeginHorizontal();

			GUIStyle style = EditorStyles.miniButtonRight;
			style.fixedWidth = 35;

			if (GUILayout.Button("Remove"))
			{
				scenes.RemoveAt(i);
			}
			if (i > 0)
			{
				if (GUILayout.Button("^", style))
				{
					LevelScene temp = scenes[i - 1];
					scenes[i - 1] = scenes[i];
					scenes[i] = temp;
				}
			}
			if (i < scenes.Count - 1)
			{
				if (GUILayout.Button("V", style))
				{
					LevelScene temp = scenes[i + 1];
					scenes[i + 1] = scenes[i];
					scenes[i] = temp;
				}
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndHorizontal();
		}


		if (scenes.Count < 1 || scenes[scenes.Count - 1] != null)
		{
			spareScene = EditorGUILayout.ObjectField("New Scene", spareScene, typeof(Object), false);

			if (spareScene != null)
			{
				LevelScene scene = new LevelScene();
				scene.scene = spareScene;
				scene.LevelName = spareScene.name;
				//Debug.Log("Scene.scene: " + scene.scene + "\nScene.LevelName: " + spareScene.name);

				scenes.Add(scene);
				spareScene = null;
			}
		}
		else
		{

		}
		#endregion

		DrawDivider();

		#region Create JamEntry Button
		if (GUILayout.Button("Create JamEntry Resource!"))
		{
			#region Creator Name & Entry Name Field Checking
			bool jamNameValid = false;
			bool creatorValid = false;
			if (creatorName.Length > 0 && creatorName.Length < 50)
			{
				creatorValid = true;
			}
			else
			{
				Debug.LogError("Creator Name is invalid.\nLength is zero or longer than 50 characters.\n");
			}
			if (entryName.Length > 0 && entryName.Length < 50)
			{
				jamNameValid = true;
			}
			else
			{
				Debug.LogError("Jam Entry Name is invalid.\nLength is zero or longer than 50 characters.\n");
			}
			#endregion

			if (jamNameValid && creatorValid)
			{
				CreateJamEntry();
			}
		}
		#endregion
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

	public static void DrawDivider()
	{
		GUILayout.Space(24f);
	}

	/// <summary>
	/// The Editor tool elements for creating a JamEntry.
	/// </summary>
	void CreateJamEntry()
	{
		#region Create Text Contents
		//Combine the name, entryname and scenelist.
		textAssetContents = "\tJam Entry\n";
		textAssetContents += version + "\n";
		textAssetContents += "\tTeam Name\n" + creatorName + "\n";
		textAssetContents += "\tEntry Name\n" + entryName + "\n";
		textAssetContents += "\tEntry Identifier\n" + identifier + "\n";
		textAssetContents += "\tScenes Needed:\n";
		for (int i = 0; i < scenes.Count; i++)
		{
			textAssetContents += scenes[i].LevelName;
			if (i != scenes.Count - 1)
			{
				textAssetContents += ",";
			}
		}
		#endregion

		Debug.Log("Creating File: " + textAssetContents + "\n\n\n");
		System.IO.File.WriteAllText(Application.dataPath + "\\" + identifier + "/Resources/Jam/" + entryName + ".txt", textAssetContents);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
	#endregion
}

[System.Serializable]
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

#region Scene Selector Object Type
/// <summary>
/// This code is credit to FriesB_DAEStudios, a commentor on StackOverflow.
/// http://answers.unity3d.com/questions/429860/type-of-scene-asset.html
/// </summary>
[System.Serializable]
public class LevelScene
{
	public string LevelName;

	[SerializeField]
	private Object _scene;
	public Object scene
	{
		get { return _scene; }
		set
		{
			//Only set when the value is changed
			if (_scene != value && value != null)
			{
				string name = value.ToString();
				if (name.Contains(" (UnityEngine.SceneAsset)"))
				{
					_scene = value;
					LevelName = name.Substring(0, name.IndexOf(" (UnityEngine.SceneAsset)"));
				}
			}
		}
	}
}
#endregion
