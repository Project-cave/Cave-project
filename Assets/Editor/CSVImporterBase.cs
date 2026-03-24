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
        if (!File.Exists(csvFilePath)) // CSV 파일 경로가 없는 경우
        {
            EditorUtility.DisplayDialog("에러", $"CSV 파일을 찾을 수 없어요!\n경로: {csvFilePath}", "확인");
            return;
        }

        if (!Directory.Exists(outputFolderPath)) // 만들 곳의 경로에 폴더가 없는 경우
        {
            Directory.CreateDirectory(outputFolderPath);
            AssetDatabase.Refresh();
        }

        // UTF-8 인코딩으로 전체 읽기  -  UTF8 -> 한글 깨짐 방지
        string[] lines = File.ReadAllLines(csvFilePath, Encoding.UTF8);

        if (lines.Length <= 1) // 1번 줄은 값이 아님
        {
            EditorUtility.DisplayDialog("경고", "데이터가 없어요! CSV 파일을 확인해주세요.", "확인");
            return;
        }

        successCount = 0;
        failedLines.Clear();

        for (int i = 1; i < lines.Length; i++) // CSV (콤마 기준으로 나눈 파일) -> 한줄한줄 ,기준으로 배열 생성
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] columns = line.Split(',');

            if (columns.Length < RequiredColumnCount)
            {
                failedLines.Add($"줄 {i + 1}: 컬럼 수 부족 ({line})");
                continue;
            }

            ParseAndCreateSO(columns, i); // 자식에서 데이터(columns) 넣고 만들기 -> SaveAsset();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string resultMessage = $" 성공: {successCount}개 생성!\n저장 위치: {outputFolderPath}";
        if (failedLines.Count > 0)
            resultMessage += $"\n\n 실패: {failedLines.Count}개\n" + string.Join("\n", failedLines);

        EditorUtility.DisplayDialog("임포트 완료", resultMessage, "확인");
        Debug.Log(resultMessage);
    }

    protected int successCount = 0;
    protected List<string> failedLines = new List<string>();

    protected int ParseInt(string value, int defaultValue = 0)
    {
        if (int.TryParse(value.Trim(), out int result)) return result;
        Debug.LogWarning($"숫자 변환 실패: '{value}' → 기본값 {defaultValue} 사용");
        return defaultValue;
    }

    protected float ParseFloat(string value, float defaultValue = 0f)
    {
        if (float.TryParse(value.Trim(), out float result)) return result;
        Debug.LogWarning($"실수 변환 실패: '{value}' → 기본값 {defaultValue} 사용");
        return defaultValue;
    }

    protected T ParseEnum<T>(string value, T defaultValue) where T : struct
    {
        if (System.Enum.TryParse(value.Trim(), true, out T result)) return result;
        Debug.LogWarning($"Enum 변환 실패: '{value}' → 기본값 {defaultValue} 사용");
        return defaultValue;
    }

    protected void SaveAsset(ScriptableObject so, string fileName)
    {
        string safeFileName = fileName.Replace(" ", "_");
        string assetPath = $"{outputFolderPath}/{safeFileName}.asset";
        AssetDatabase.DeleteAsset(assetPath);
        AssetDatabase.CreateAsset(so, assetPath);
        successCount++;
    }
}
