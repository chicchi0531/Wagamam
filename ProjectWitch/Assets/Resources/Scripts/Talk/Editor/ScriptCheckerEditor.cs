using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using ProjectWitch;
using ProjectWitch.Talk.WorkSpace;

using System.IO;
using System.Linq;

//参考：http://baba-s.hatenablog.com/entry/2015/08/09/100000
public static class DirectoryUtils
{
    /// <summary>
    /// <para>指定したディレクトリ内の指定した</para>
    /// <para>いずれかの拡張子を持つファイル名 (パスを含む) を返します</para>
    /// </summary>
    public static string[] GetFiles
    (
        string path,
        params string[] extensions
    )
    {
        return Directory
            .GetFiles(path, "*.*")
            .Where(c => extensions.Any(extension => c.EndsWith(extension)))
            .ToArray()
        ;
    }

    /// <summary>
    /// <para>指定したディレクトリの中から、</para>
    /// <para>指定したいずれかの拡張子を持ち、</para>
    /// <para>サブディレクトリを検索するかどうかを決定する</para>
    /// <para>値を持つファイル名 (パスを含む) を返します</para>
    /// </summary>
    public static string[] GetFiles
    (
        string path,
        SearchOption searchOption,
        params string[] extensions
    )
    {
        return Directory
            .GetFiles(path, "*.*", searchOption)
            .Where(c => extensions.Any(extension => c.EndsWith(extension)))
            .ToArray()
        ;
    }
}

public class ScriptCheckerEditor : EditorWindow {

    float mCompileProgress = 0.0f;
    IEnumerator mCompileProcess = null;
    bool mIsDone = true;

    [MenuItem("TalkScript/Checker")]
    static void CheckAll()
    {
        GetWindow<ScriptCheckerEditor>();
    }

    private void OnEnable()
    {
        EditorApplication.update += OnUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnUpdate;
    }

    private void OnGUI()
    {
        if(GUILayout.Button("すべてのスクリプトのエラーチェック"))
        {
            Debug.Log("コンパイル開始");
            mCompileProcess = Check();
        }
    }

    private void OnUpdate()
    {
        if (mCompileProcess != null)
        { 
            EditorUtility.DisplayProgressBar("Compile Now ...", "すべてのスクリプトのエラーチェックをしています。", mCompileProgress);

            mCompileProcess.MoveNext();
            if(mIsDone)
            {
                mCompileProcess = null;
                EditorUtility.ClearProgressBar();
            }
        }
    }

    public IEnumerator Check()
    {
        mIsDone = false;
        mCompileProgress = 0.0f;

        var scriptList = DirectoryUtils.GetFiles("Assets/Resources/Scenarios", ".txt");

        var progressWait = 1.0f/scriptList.Length;

        var errorFileNum = 0;
        foreach (var s in scriptList)
        {
            ProjectWitch.Talk.Compiler.ScriptCompiler compiler = new ProjectWitch.Talk.Compiler.ScriptCompiler();

            var path = Path.GetFileNameWithoutExtension(s);
            path = "Scenarios/" + path;

            VirtualMachine vm = null;
            IEnumerator t = compiler.CompileScript(path, (result) => vm = result, true);

            while (t.MoveNext()) { yield return null; }

            if (vm == null)
            {
                errorFileNum++;
                Debug.LogWarning(path + " :コンパイルエラーで停止しました。");
            }
            else Debug.Log(path + " :コンパイル成功");


            mCompileProgress += progressWait;

            yield return null;
        }

        if (errorFileNum == 0) EditorUtility.DisplayDialog("result", "すべてのスクリプトが正常にコンパイルされました。", "OK");
        else EditorUtility.DisplayDialog("result", errorFileNum.ToString() + "件のスクリプトがコンパイルできませんでした。\nコンソールを確認してください。", "OK");
        mIsDone = true;

        yield return true;
    }
}
