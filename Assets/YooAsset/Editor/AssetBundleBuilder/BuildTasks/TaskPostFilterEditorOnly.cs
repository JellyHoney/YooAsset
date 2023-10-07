namespace YooAsset.Editor
{
    [Task("恢复被修改的EditorOnly")]
    public class TaskPostFilterEditorOnly : IBuildTask
    {
        void IBuildTask.Run(BuildContext context)
        {
            var buildEditorOnlyContext = context.GetContextObject<BuildEditorOnlyContext>();
            buildEditorOnlyContext.RecoveryRelease();
            BuildLogger.Log("恢复EditorOnly完毕！");
        }
    }
}