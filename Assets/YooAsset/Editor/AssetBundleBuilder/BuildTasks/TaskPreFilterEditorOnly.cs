using UnityEditor;
using UnityEngine;

namespace YooAsset.Editor
{
    [Task("过滤EditorOnly并临时修改Asset")]
    public class TaskPreFilterEditorOnly : IBuildTask
    {
        void IBuildTask.Run(BuildContext context)
        {
            var buildMapContext = context.GetContextObject<BuildMapContext>();
            BuildEditorOnlyContext buildEditorOnlyContext = new BuildEditorOnlyContext();
            foreach (var bundleInfo in buildMapContext.Collection)
            {
                foreach (var assetInfo in bundleInfo.AllMainAssets)
                {
                    if (!assetInfo.AssetPath.EndsWith(".prefab"))
                        continue;

                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetInfo.AssetPath);
                    GameObject ins = Object.Instantiate(prefab);
                    bool changed = false;
                    foreach (var trans in ins.GetComponentsInChildren<Transform>(true))
                    {
                        if (trans == null)
                            continue;

                        GameObject go = trans.gameObject;
                        if (!go.CompareTag("EditorOnly"))
                            continue;

                        Object.DestroyImmediate(go);
                        changed = true;
                    }

                    if (changed)
                    {
                        buildEditorOnlyContext.Load(assetInfo.AssetPath);
                        PrefabUtility.SaveAsPrefabAsset(ins, assetInfo.AssetPath);
                    }

                    Object.DestroyImmediate(ins);
                }
            }

            context.SetContextObject(buildEditorOnlyContext);
            BuildLogger.Log("过滤EditorOnly完毕！");
        }
    }
}