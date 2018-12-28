using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// スクリプトを作るクラス
/// </summary>
public class ScriptCreator : EditorWindow
{

    //テンプレートとスクリプトの拡張子
    protected const string TEMPLATE_SCRIPT_EXTENSION = ".txt";
    protected const string SCRIPT_EXTENSION = ".cs";

    //テンプレートがあるディレクトリへのパス
    protected const string TEMPLATE_SCRIPT_DIRECTORY_PATH = "Assets/Toast/Editor/ScriptTemplateExpansion/Templates/";

    //置換用タグ
    protected const string PRODUCT_NAME_TAG = "#PRODUCTNAME#";
    protected const string AUTHOR_TAG = "#AUTHOR#";
    protected const string DATA_TAG = "#DATA#";
    protected const string SUMMARY_TAG = "#SUMMARY#";
    protected const string SCRIPT_NAME_TAG = "#SCRIPTNAME#";

}
/// <summary>
/// テンプレートから新しくスクリプトを作るクラス
/// </summary>
public class CustomScriptCreator : ScriptCreator
{
    //メニューのパス
    private const string MENU_PATH = "Assets/Create/";

    //テンプレート名
    private const string TEMPLATE_SCRIPT_STATEMACHINE_NAME = "StateMachineWithMonoBehaviour";
    //private const string TEMPLATE_SCRIPT_STATIC_NAME = "";
    //private const string TEMPLATE_SCRIPT_SINGLETON_NAME = "Singleton";

    //作成する元のテンプレート名
    private static string _templateScriptName = "";

    //新しく作成するスクリプト及びクラス名
    private static string _newScriptName = "";
    //スクリプトの説明文
    private static string _scriptSummary = "";
    //作成者名
    private static string _authorName = "";
    //作成日
    private static string _createdData = "";

    //=================================================================================
    //メニューに表示する項目
    //=================================================================================

    [MenuItem(MENU_PATH + "C# " + TEMPLATE_SCRIPT_STATEMACHINE_NAME, priority = 1)]
    private static void CreateBasicScript()
    {
        ShowWindow(TEMPLATE_SCRIPT_STATEMACHINE_NAME);
    }
    /*
    [MenuItem(MENU_PATH + TEMPLATE_SCRIPT_SINGLETON_NAME)]
    private static void CreateSingletonScript()
    {
        ShowWindow(TEMPLATE_SCRIPT_SINGLETON_NAME);
    }

    [MenuItem(MENU_PATH + TEMPLATE_SCRIPT_STATIC_NAME)]
    private static void CreateConstantsScript()
    {
        ShowWindow(TEMPLATE_SCRIPT_STATIC_NAME);
    }
    */
    //=================================================================================
    //ウィンドウ表示
    //=================================================================================

    private static void ShowWindow(string templateScriptName)
    {

        //各項目を初期化
        _templateScriptName = templateScriptName;
        _newScriptName = templateScriptName;
        _createdData = DateTime.Now.ToString("yyyy.MM.dd");

        //作成者名は既に設定されてある場合は初期化しない
        if (string.IsNullOrEmpty(_authorName))
        {
            _authorName = System.Environment.UserName;
        }

        //ウィンドウ作成
        EditorWindow.GetWindow<CustomScriptCreator>("Create Script");
    }

    //表示するGUI設定
    private void OnGUI()
    {
        //作成日と元テンプレートを表示
        EditorGUILayout.LabelField("Template Script Name : " + _templateScriptName);
        GUILayout.Space(0);
        EditorGUILayout.LabelField("Created Data : " + _createdData);
        GUILayout.Space(10);

        //新しく作成するスクリプト及びクラス名の入力欄
        GUILayout.Label("New Script Name");
        _newScriptName = GUILayout.TextField(_newScriptName);
        GUILayout.Space(10);

        //スクリプトの説明文
        GUILayout.Label("Script Summary");
        _scriptSummary = GUILayout.TextArea(_scriptSummary);
        GUILayout.Space(10);

        //作成者名の入力欄
        GUILayout.Label("Author Name");
        _authorName = GUILayout.TextField(_authorName);
        GUILayout.Space(30);

        //作成ボタン、作成が成功したらウィンドウを閉じる
        if (GUILayout.Button("Create"))
        {
            if (CreateScript())
            {
                this.Close();
            }
        }
    }

    //=================================================================================
    //スクリプト作成
    //=================================================================================

    private static bool CreateScript()
    {

        //スクリプト名が空欄の場合は作成失敗
        if (string.IsNullOrEmpty(_newScriptName))
        {
            Debug.Log("スクリプト名が入力されていないため、スクリプトが作成できませんでした");
            return false;
        }

        //現在選択しているファイルのパスを取得、選択されていない場合はスクリプト作成失敗
        string directoryPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(directoryPath))
        {
            Debug.Log("作成場所が選択されていないため、スクリプトが作成できませんでした");
            return false;
        }

        //選択されているファイルに拡張子がある場合(ディレクトリでない場合)は一つ上のディレクトリ内に作成する
        if (!string.IsNullOrEmpty(new System.IO.FileInfo(directoryPath).Extension))
        {
            directoryPath = System.IO.Directory.GetParent(directoryPath).FullName;
        }

        //同名ファイルがあった場合はスクリプト作成失敗にする(上書きしてしまうため)
        string exportPath = directoryPath + "/" + _newScriptName + SCRIPT_EXTENSION;

        if (System.IO.File.Exists(exportPath))
        {
            Debug.Log(exportPath + "が既に存在するため、スクリプトが作成できませんでした");
            return false;
        }

        //テンプレートへのパスを作成しテンプレート読み込み
        string templatePath = TEMPLATE_SCRIPT_DIRECTORY_PATH + _templateScriptName + TEMPLATE_SCRIPT_EXTENSION;
        StreamReader streamReader = new StreamReader(templatePath, Encoding.GetEncoding("Shift_JIS"));
        string scriptText = streamReader.ReadToEnd();

        //各項目を置換
        scriptText = scriptText.Replace(PRODUCT_NAME_TAG, PlayerSettings.productName);
        scriptText = scriptText.Replace(AUTHOR_TAG, _authorName);
        scriptText = scriptText.Replace(DATA_TAG, _createdData);
        scriptText = scriptText.Replace(SUMMARY_TAG, _scriptSummary.Replace("\n", "\n/// ")); //改行するとコメントアウトから外れるので///を追加
        scriptText = scriptText.Replace(SCRIPT_NAME_TAG, _newScriptName);

        //スクリプトを書き出し
        File.WriteAllText(exportPath, scriptText, Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

        return true;
    }

}
public class ScriptTemplateExpansion : ScriptCreator
{
    //テンプレート上部に差し込む上部定型テキスト
    private const string TOP_TEXT =
        "//  " + SCRIPT_NAME_TAG + SCRIPT_EXTENSION + "\n" +
        "//  ProductName " + PRODUCT_NAME_TAG + "\n" +
        "//\n" +
        "//  Created by " + AUTHOR_TAG + " on " + DATA_TAG + "\n";

    //クラス説明のサマリー用定型テキスト
    private const string CLASS_SUMMARY_BETWEEN = "/// " + SUMMARY_TAG + "\n";

    private const string CLASS_SUMMARY =
        "\n/// <summary>\n" +
        CLASS_SUMMARY_BETWEEN +
        "/// </summary>";

    //メニューのパス
    private const string MENU_PATH = "Assets/Create/Template Script";

    //元スクリプトへのパス
    private static string _originalScriptPath = "";

    //テンプレート名
    private static string _templateName = "";

    //=================================================================================
    //ウィンドウ表示
    //=================================================================================

    [MenuItem(MENU_PATH)]
    private static void ShowWindow()
    {

        //現在選択しているディレクトリのパスを取得
        _originalScriptPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(_originalScriptPath))
        {
            Debug.Log("テンプレートの元ファイルが選択されていないため、テンプレートが作成できません");
            return;
        }

        //拡張子を取得し、スクリプトファイルか判定
        string extension = new System.IO.FileInfo(_originalScriptPath).Extension;
        if (extension != SCRIPT_EXTENSION)
        {
            Debug.Log("拡張子が " + SCRIPT_EXTENSION + "のファイルが選択されていないため、テンプレートが作成できません");
            return;
        }

        //作成するテンプレート名に選択したスクリプト名を仮設定
        _templateName = System.IO.Path.GetFileNameWithoutExtension(_originalScriptPath);

        //ウィンドウ作成
        EditorWindow.GetWindow<ScriptTemplateExpansion>("Create Template");
    }

    //表示するGUI設定
    private void OnGUI()
    {

        //元スクリプトのパスを表示
        EditorGUILayout.LabelField("Original Script : " + _originalScriptPath);
        GUILayout.Space(10);

        //新しく作成するテンプレ名の入力欄
        GUILayout.Label("New Template Name");
        _templateName = GUILayout.TextField(_templateName);
        GUILayout.Space(10);

        //作成ボタン、作成が成功したらウィンドウを閉じる
        if (GUILayout.Button("Create"))
        {
            if (CreateTemplate())
            {
                this.Close();
            }
        }

    }
    
    //=================================================================================
    //スクリプト作成
    //=================================================================================

    private static bool CreateTemplate()
    {

        //テンプレート名が空欄の場合は作成失敗
        if (string.IsNullOrEmpty(_templateName))
        {
            Debug.Log("テンプレート名が入力されていないため、テンプレートが作成できませんでした");
            return false;
        }

        //同名ファイルがあった場合はテンプレート作成失敗にする(上書きしてしまうため)
        string exportPath = TEMPLATE_SCRIPT_DIRECTORY_PATH + "/" + _templateName + TEMPLATE_SCRIPT_EXTENSION;

        if (System.IO.File.Exists(exportPath))
        {
            Debug.Log(exportPath + "が既に存在するため、テンプレートが作成できませんでした");
            return false;
        }

        //元スクリプト読み込み
        StreamReader streamReader = new StreamReader(_originalScriptPath, Encoding.GetEncoding("Shift_JIS"));
        string scriptText = streamReader.ReadToEnd();

        //クラス名をタグ置換
        scriptText = ReplaceClassNameToTag(scriptText);

        //クラス説明のSummaryを置換
        scriptText = ReplaceClassSummaryToTag(scriptText);

        //元スクリプトがテンプレートから作った時などに上部定型テキストがあるかもしれないので、最初のusingより上を削除
        int usingFirstPoint = scriptText.IndexOf("using");
        if (usingFirstPoint > 0)
        {
            scriptText = scriptText.Remove(0, usingFirstPoint - 1);
        }
        else
        {
            scriptText = "\n" + scriptText;
        }

        //上部定型テキスト差し込み
        scriptText = TOP_TEXT + scriptText;

        //スクリプトを書き出し
        File.WriteAllText(exportPath, scriptText, Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

        return true;
    }

    //クラス説明のSummaryを置換
    private static string ReplaceClassSummaryToTag(string scriptText)
    {

        int summaryFirstPoint = scriptText.IndexOf("summary");
        int scriptNameTagFirstPoint = scriptText.IndexOf(SCRIPT_NAME_TAG);

        //クラス説明のSummaryが無い場合は、Summaryの定型を差し込む
        if (summaryFirstPoint == -1 || summaryFirstPoint > scriptNameTagFirstPoint)
        {

            //"class "前の改行位置を調べる
            string classText = "class ";
            int classNameStartPoint = scriptText.IndexOf(classText);

            int lineBreakPoint = scriptText.IndexOf("\n");
            int lineBreakPointBeforeClassName = 0;

            while (lineBreakPoint < classNameStartPoint)
            {
                lineBreakPointBeforeClassName = lineBreakPoint;
                lineBreakPoint = scriptText.IndexOf("\n", lineBreakPointBeforeClassName + 1);
            }

            //"class "前の改行のさらに前にSummaryの定型を差し込む
            scriptText = scriptText.Insert(lineBreakPointBeforeClassName, CLASS_SUMMARY);

        }
        //クラス説明のSummaryがある場合は最初の<summary>の後から</summary>の前の改行まで全て削除し、Summaryの定型の間だけ差し込む
        else
        {
            string summaryStartText = "<summary>";
            int removeStartPoint = scriptText.IndexOf(summaryStartText) + summaryStartText.Length;

            string summaryEndText = "</summary>";
            int summaryEndTextPoint = scriptText.IndexOf(summaryEndText);

            int lineBreakPoint = scriptText.IndexOf("\n");
            int removeEndPoint = 0;

            while (lineBreakPoint < summaryEndTextPoint)
            {
                removeEndPoint = lineBreakPoint;
                lineBreakPoint = scriptText.IndexOf("\n", removeEndPoint + 1);
            }

            scriptText = scriptText.Remove(removeStartPoint, removeEndPoint - removeStartPoint);
            scriptText = scriptText.Insert(removeStartPoint + 1, CLASS_SUMMARY_BETWEEN);
        }

        return scriptText;
    }

    //クラス名をタグ置換
    private static string ReplaceClassNameToTag(string scriptText)
    {

        //全置換すると関係ない所も置換するので、一旦クラス名を削除した後にタグを差し込む
        //クラス名で検索すると他の所が引っかかるかもしれないので"class "を使って検索する
        string classText = "class ";
        int classNameStartPoint = scriptText.IndexOf(classText) + classText.Length;
        string classname = System.IO.Path.GetFileNameWithoutExtension(_originalScriptPath);

        scriptText = scriptText.Remove(classNameStartPoint, classname.Length);
        scriptText = scriptText.Insert(classNameStartPoint, SCRIPT_NAME_TAG);

        return scriptText;
    }
}