using Spine.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Spine
{
    public class ConvertMaterial
    {
		public static float defaultScale = 0.01f;
		public static float defaultMix = 0.2f;
        [MenuItem("Assets/ConvertSineModel", false, 1)]
        static void DoConvertSineModel()
        {
            UnityEngine.Object[] arr=Selection.GetFiltered(typeof(UnityEngine.Material), SelectionMode.DeepAssets);
            string pre_path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/";
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(@"Assets\Art\Role\model.prefab");
            Shader shader = Shader.Find("Spine/Skeleton ETC");
            for(var i = 0;i < arr.Length;++i)
            {
                string file_name = pre_path + AssetDatabase.GetAssetPath(arr[i]);
                FileInfo file = new FileInfo(file_name);
                UnityEngine.Material material = arr[i] as UnityEngine.Material;
                if (material.shader.name == "Hidden/InternalErrorShader")
                {
                    material.shader = shader;
                }
                if (material.shader.name == "Spine/Skeleton" || material.shader.name == "Spine/Skeleton ETC")
                {
                    var mainTexture = AssetDatabase.GetAssetPath(material.mainTexture);
                    var alpha = mainTexture.Replace(".png","_a.png");
                    if(File.Exists(pre_path + alpha))
                    {
                        Debug.Log(alpha);
                        material.shader = shader;
                        var at= AssetDatabase.LoadAssetAtPath<Texture2D>(alpha);
                        material.SetTexture("_MainTex_A", at);
                    }
                    var data = mainTexture.Replace(".png", "_SkeletonData.asset");
                    if (!File.Exists(pre_path + data))
                    {
                        FileInfo data_file = new FileInfo(pre_path + data);
                        //var data_file_name = data_file.Name.Substring(0,data_file.Name.Length - 6);
                        //CreateAsset<SkeletonDataAsset>(data_file_name + "_SkeletonData");
                        var atlas = mainTexture.Replace(".png", "_Atlas.asset");
                        AtlasAsset atlasAsset = AssetDatabase.LoadAssetAtPath<AtlasAsset>(atlas);
                        var json = mainTexture.Replace(".png", ".skel.bytes");
                        TextAsset spineJson = AssetDatabase.LoadAssetAtPath<TextAsset>(json);

                        var skelDataAsset = SkeletonDataAsset.CreateInstance<SkeletonDataAsset>();
                        skelDataAsset.atlasAssets = new AtlasAsset[1] { atlasAsset };
                        skelDataAsset.skeletonJSON = spineJson;
                        skelDataAsset.fromAnimation = new string[0];
                        skelDataAsset.toAnimation = new string[0];
                        skelDataAsset.duration = new float[0];
                        skelDataAsset.defaultMix = defaultMix;
                        skelDataAsset.scale = defaultScale;
                        try
                        {

                            AssetDatabase.CreateAsset(skelDataAsset, data);
                            AssetDatabase.SaveAssets();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }

                    }
                    var prefab = mainTexture.Replace(".png", ".prefab");
                    if (!File.Exists(pre_path + prefab))
                    {
                        GameObject prefab_instance = GameObject.Instantiate(model);
                        try
                        {
                            SkeletonAnimation animation = prefab_instance.GetComponent<SkeletonAnimation>();
                            var skeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(data);
                            animation.skeletonDataAsset = skeletonDataAsset;
                            SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
                            Skin skin = skeletonData.DefaultSkin;
                            if (skin == null) skin = skeletonData.Skins.Items[skeletonData.Skins.Items.Length - 1];
                            animation.initialSkinName = skin.Name;
                            animation.AnimationName = skeletonData.Animations.Items[0].Name;
                            UnityEngine.Object n_prefab = PrefabUtility.CreateEmptyPrefab(prefab);
                            PrefabUtility.ReplacePrefab(prefab_instance, n_prefab, ReplacePrefabOptions.ConnectToPrefab);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                            GameObject.DestroyImmediate(prefab_instance);
                        }
                    }
                    Debug.LogFormat("convert {0} ok !", file_name);
                }
            }
            AssetDatabase.SaveAssets();
            Debug.Log("convert all done!");
        }
        static public void CreateSkeletonData()
        {
            CreateAsset<SkeletonDataAsset>("New SkeletonData");
        }

        static private void CreateAsset<T>(String name) where T : ScriptableObject
        {
            var dir = "Assets/";
            var selected = Selection.activeObject;
            if (selected != null)
            {
                var assetDir = AssetDatabase.GetAssetPath(selected.GetInstanceID());
                if (assetDir.Length > 0 && Directory.Exists(assetDir))
                    dir = assetDir + "/";
            }
            ScriptableObject asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, dir + name + ".asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
