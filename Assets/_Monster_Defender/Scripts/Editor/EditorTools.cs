using UnityEngine;
using UnityEditor;

namespace ClawbearGames
{
    public class EditorTools : EditorWindow
    {
        [MenuItem("Tools/ClawbearGames/Reset PlayerPrefs")]
        public static void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("*************** PlayerPrefs Was Deleted ***************");
        }


        [MenuItem("Tools/Capture Screenshot To Desktop")]
        public static void CaptureScreenshot_Desktop()
        {
            string path = "C:/Users/TIENNQ/Desktop/icon.png";
            ScreenCapture.CaptureScreenshot(path);
        }


        //[MenuItem("Tools/ClawbearGames/Setup Models")]
        //public static void SetupModels()
        //{
        //    //string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        //    //foreach (string assetPath in assetPaths)
        //    //{
        //    //    System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

        //    //    if (assetType != null)
        //    //    {
        //    //        if (!assetPath.Contains(".prefab") && assetType.ToString().Contains("GameObject"))
        //    //        {
        //    //            ModelImporter importer = ModelImporter.GetAtPath(assetPath) as ModelImporter;
        //    //            importer.materialImportMode = ModelImporterMaterialImportMode.None;
        //    //        }
        //    //    }
        //    //}
        //    //AssetDatabase.Refresh();


            //string towerPaths = "Assets/_Highway_Deliver_Master/Models/Towers/";
            //for (int i = 0; i <= 15; i++)
            //{
            //    string matPath = towerPaths + "Tower_" + i.ToString() + "/" + "Tower_" + i.ToString() + ".mat";
            //    string texturePath = towerPaths + "Tower_" + i.ToString() + "/" + "Tower_" + i.ToString() + ".png";
            //    Material material = new Material(Shader.Find("Clawbear Games/Self Illumin Diffuse"));
            //    AssetDatabase.CreateAsset(material, matPath);
            //    material = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            //    material.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath));
            //}

        //    //foreach (GameObject cha in characterPrefabs)
        //    //{
        //    //    GameObject charTank = cha;
        //    //    GameObject charBody = charTank.transform.GetChild(0).gameObject;
        //    //    GameObject charTurret = charTank.transform.GetChild(1).gameObject;

        //    //    string rootPath = "Assets/_Tankie_Snaker_Attack/Models/Characters/" + cha.name;

        //    //    Material material = AssetDatabase.LoadAssetAtPath<Material>(rootPath + ("/" + cha.name + ".mat"));

        //    //    GameObject tankModel = AssetDatabase.LoadAssetAtPath<GameObject>(rootPath + "/Character_Tank_" + cha.name.Split('_')[1] + ".fbx");
        //    //    charTank.GetComponent<MeshFilter>().sharedMesh = tankModel.GetComponent<MeshFilter>().sharedMesh;
        //    //    charTank.GetComponent<MeshRenderer>().sharedMaterial = material;

        //    //    GameObject bodyModel = AssetDatabase.LoadAssetAtPath<GameObject>(rootPath + "/Character_Body_" + cha.name.Split('_')[1] + ".fbx");
        //    //    charBody.GetComponent<MeshFilter>().sharedMesh = bodyModel.GetComponent<MeshFilter>().sharedMesh;
        //    //    charBody.GetComponent<MeshRenderer>().sharedMaterial = material;

        //    //    GameObject turretModel = AssetDatabase.LoadAssetAtPath<GameObject>(rootPath + "/Character_Turret_" + cha.name.Split('_')[1] + ".fbx");
        //    //    charTurret.GetComponent<MeshFilter>().sharedMesh = turretModel.GetComponent<MeshFilter>().sharedMesh;
        //    //    charTurret.GetComponent<MeshRenderer>().sharedMaterial = material;
        //    //}

        //    //AssetDatabase.Refresh();

                //foreach (GameObject tank in characterPrefabs)
                //{
                //    GameObject charTank = tank;
                //    GameObject tankTurret = charTank.transform.GetChild(0).transform.GetChild(0).gameObject;

                //    string rootPath = "Assets/_Coin_Hoarder/Models/Others/Tanks/" + tank.name;

                //    Material material = AssetDatabase.LoadAssetAtPath<Material>(rootPath + ("/" + tank.name + ".mat"));

                //    GameObject tankModel = AssetDatabase.LoadAssetAtPath<GameObject>(rootPath + "/Tank_" + tank.name.Split('_')[1] + ".fbx").transform.GetChild(0).gameObject;
                //    charTank.GetComponent<MeshFilter>().sharedMesh = tankModel.GetComponent<MeshFilter>().sharedMesh;
                //    charTank.GetComponent<MeshRenderer>().sharedMaterial = material;
                //    tank.GetComponent<MeshCollider>().sharedMesh = tankModel.GetComponent<MeshFilter>().sharedMesh;

                //    GameObject turretModel = AssetDatabase.LoadAssetAtPath<GameObject>(rootPath + "/Tank_" + tank.name.Split('_')[1] + ".fbx").transform.GetChild(1).gameObject;
                //    tankTurret.GetComponent<MeshFilter>().sharedMesh = turretModel.GetComponent<MeshFilter>().sharedMesh;
                //    tankTurret.GetComponent<MeshRenderer>().sharedMaterial = material;
                //}

                //AssetDatabase.Refresh();
        //}
    }
}

