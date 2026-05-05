using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;

public abstract class CSVImporterBase : EditorWindow
{
    protected string csvFilePath = "";
    protected string outputFolderPath = "";

    protected abstract string WindowTitle { get; }
    protected abstract string DefaultCSVPath { get; }
    protected abstract string DefaultOutputPath { get; }
    protected abstract void ParseAndCreateSO(string[] columns, int lineIndex);
    protected abstract int RequiredColumnCount { get; }

    protected int successCount = 0;
    protected List<string> failedLines = new List<string>();

    protected virtual void OnEnable()
    {
        if (string.IsNullOrEmpty(csvFilePath))
            csvFilePath = DefaultCSVPath;
        if (string.IsNullOrEmpty(outputFolderPath))
            outputFolderPath = DefaultOutputPath;
    }

    protected virtual void OnGUI()
    {
        GUILayout.Label(WindowTitle, EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("CSV 파일 경로");
        csvFilePath = EditorGUILayout.TextField(csvFilePath);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("저장할 폴더 경로");
        outputFolderPath = EditorGUILayout.TextField(outputFolderPath);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("임포트 시작", GUILayout.Height(40)))
        {
            ImportCSV();
        }
    }

    protected void ImportCSV()
    {
        if (!File.Exists(csvFilePath))
        {
            EditorUtility.DisplayDialog("에러", $"CSV 파일을 찾을 수 없어요!\n경로: {csvFilePath}", "확인");
            return;
        }

        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
            AssetDatabase.Refresh();
        }

        string[] lines = File.ReadAllLines(csvFilePath, Encoding.UTF8);

        if (lines.Length <= 1)
        {
            EditorUtility.DisplayDialog("경고", "데이터가 없어요!", "확인");
            return;
        }

        successCount = 0;
        failedLines.Clear();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] columns = line.Split(',');

            if (columns.Length < RequiredColumnCount)
            {
                failedLines.Add($"줄 {i + 1}: 컬럼 수 부족 ({line})");
                continue;
            }

            ParseAndCreateSO(columns, i);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string resultMessage = $"성공: {successCount}개 처리!\n저장 위치: {outputFolderPath}";
        if (failedLines.Count > 0)
            resultMessage += $"\n\n실패: {failedLines.Count}개\n" + string.Join("\n", failedLines);

        EditorUtility.DisplayDialog("임포트 완료", resultMessage, "확인");
        Debug.Log(resultMessage);
    }


    //----helper----

    protected int ParseInt(string value, int defaultValue = 0)
    {
        if (int.TryParse(value, out int result)) return result;
        if (!string.IsNullOrEmpty(value))
            Debug.LogWarning($"숫자 변환 실패: '{value}' → {defaultValue} 사용");
        return defaultValue;
    }

    protected float ParseFloat(string value, float defaultValue = 0f)
    {
        if (float.TryParse(value, out float result)) return result;
        if (!string.IsNullOrEmpty(value))
            Debug.LogWarning($"실수 변환 실패: '{value}' → {defaultValue} 사용");
        return defaultValue;
    }

    protected T ParseEnum<T>(string value, T defaultValue) where T : struct
    {
        if (System.Enum.TryParse(value, true, out T result)) return result;
        if (!string.IsNullOrEmpty(value))
            Debug.LogWarning($"Enum 변환 실패: '{value}' → {defaultValue} 사용");
        return defaultValue;
    }

    protected T SaveOrUpdateAsset<T>(string fileName, System.Action<T> applyValues)
        where T : ScriptableObject
    {
        string safeFileName = fileName.Replace(" ", "_");
        string assetPath = $"{outputFolderPath}/{safeFileName}.asset";

        T target = AssetDatabase.LoadAssetAtPath<T>(assetPath);

        if (target == null)
        {
            target = ScriptableObject.CreateInstance<T>();
            applyValues(target);
            AssetDatabase.CreateAsset(target, assetPath);
        }
        else
        {
            applyValues(target);
            EditorUtility.SetDirty(target);
        }

        successCount++;
        return target;
    }
}
