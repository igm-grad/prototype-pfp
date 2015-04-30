using UnityEditor;
using UnityEngine;
using System.Linq;
public class BuildUtePrefabs : Editor {

    const string tilePath = "Assets/tiles";
    const string prefabPath = "Assets/Prefabs/Tiles";

    [MenuItem("Utility/Build Tile Prefabs")]
    public static void BuildTilePrefabs()
    {
        // prevent someone from calling this by mistake;
        //return;

        var assets = AssetDatabase.FindAssets("", new[] { tilePath }).Distinct();

        foreach (var guid in assets)
        {
            var asset = (GameObject) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject));

            var tempPrefab = PrefabUtility.CreatePrefab("Assets/__temp__.prefab", asset);

            var container = new GameObject(asset.name);
            var col = container.AddComponent<BoxCollider>();

            var temp = (GameObject)GameObject.Instantiate(tempPrefab);
            temp.transform.localScale = Vector3.one * .125f;
            temp.transform.parent = container.transform;
            var mf = temp.GetComponent<MeshFilter>();

            col.size = new Vector3(mf.sharedMesh.bounds.size.x, mf.sharedMesh.bounds.size.z, mf.sharedMesh.bounds.size.y) * .125f;
            col.center = new Vector3(mf.sharedMesh.bounds.center.x, mf.sharedMesh.bounds.center.z, mf.sharedMesh.bounds.center.y) * .125f;

            PrefabUtility.CreatePrefab(string.Format("{0}/{1}.prefab", prefabPath, container.name), container);

            GameObject.DestroyImmediate(temp);
            GameObject.DestroyImmediate(container);
        }


        UnityEngine.Debug.Log(assets);
    }
}
