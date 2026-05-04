using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;

public abstract class MultiCSVImporterBase : EditorWindow
{
    protected abstract string WindowTitle { get; }
    protected abstract string outputFolderPath { get; }
    protected abstract string[] CSVFilePaths { get; }      // 여러 CSV 경로
    protected abstract string KeyColumnName { get; }       // 매칭 키 (예: "unitName")
    protected abstract void CreateSOFromMergedRow(string key, MergedRow row);

    protected int successCount = 0;
    protected List<string> warnings = new List<string>();

    public class MergedRow
    {
        private Dictionary<string, Dictionary<string, string>> sheets = new();

        public void AddSheet(string sheetName, Dictionary<string, string> row)
        {
            sheets[sheetName] = row;
        }

        public bool HasSheet(string sheetName) => sheets.ContainsKey(sheetName);

        public string Get(string sheetName, string columnName, string defaultValue = "")
        {
            if (sheets.TryGetValue(sheetName, out var row) &&
                row.TryGetValue(columnName, out var value))
                return value;
            return defaultValue;
        }
    }

    protected virtual void OnGUI()
    {
        GUILayout.Label(WindowTitle, EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("입력 CSV 파일들:");
        foreach (var path in CSVFilePaths)
            EditorGUILayout.LabelField($"  • {path}");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"저장 위치: {outputFolderPath}");
        EditorGUILayout.LabelField($"매칭 키: {KeyColumnName}");

        EditorGUILayout.Space(10);

        if (GUILayout.Button("임포트 시작", GUILayout.Height(40)))
        {
            ImportAll();
        }
    }

    protected void ImportAll()
    {
        // 1. 모든 CSV 파일 존재 체크
        foreach (var path in CSVFilePaths)
        {
            if (!File.Exists(path))
            {
                EditorUtility.DisplayDialog("에러", $"CSV 파일을 찾을 수 없어요!\n{path}", "확인");
                return;
            }
        }

        if (!Directory.Exists(outputFolderPath))
            Directory.CreateDirectory(outputFolderPath);

        successCount = 0;
        warnings.Clear();

        // 2. 모든 CSV를 읽어서 키별로 모으기
        // key → MergedRow
        var mergedRows = new Dictionary<string, MergedRow>();

        foreach (var path in CSVFilePaths)
        {
            string sheetName = Path.GetFileNameWithoutExtension(path);
            ReadCSVIntoMergedRows(path, sheetName, mergedRows);
        }

        // 3. 키별로 SO 생성
        foreach (var kvp in mergedRows)
        {
            CreateSOFromMergedRow(kvp.Key, kvp.Value);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string msg = $"성공: {successCount}개 처리!\n저장 위치: {outputFolderPath}";
        if (warnings.Count > 0)
            msg += $"\n\n경고: {warnings.Count}개\n" + string.Join("\n", warnings);

        EditorUtility.DisplayDialog("임포트 완료", msg, "확인");
        Debug.Log(msg);
    }

    private void ReadCSVIntoMergedRows(string filePath, string sheetName,
                                       Dictionary<string, MergedRow> mergedRows)
    {
        string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
        if (lines.Length <= 1)
        {
            warnings.Add($"{sheetName}: 데이터가 없습니다.");
            return;
        }

        // 헤더 파싱
        string[] headers = lines[0].Split(',');
        for (int i = 0; i < headers.Length; i++)
            headers[i] = headers[i].Trim();

        // 키 컬럼이 있는지 확인
        int keyIndex = System.Array.IndexOf(headers, KeyColumnName);
        if (keyIndex < 0)
        {
            warnings.Add($"{sheetName}: 키 컬럼 '{KeyColumnName}'을 찾을 수 없습니다.");
            return;
        }

        // 데이터 행 파싱
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] columns = line.Split(',');
            if (columns.Length <= keyIndex)
            {
                warnings.Add($"{sheetName} 줄 {i + 1}: 키 컬럼 부족");
                continue;
            }

            string key = columns[keyIndex].Trim();
            if (string.IsNullOrEmpty(key)) continue;

            // 헤더 → 값 딕셔너리 만들기
            var rowDict = new Dictionary<string, string>();
            for (int c = 0; c < headers.Length && c < columns.Length; c++)
                rowDict[headers[c]] = columns[c].Trim();

            // 키별로 병합
            if (!mergedRows.ContainsKey(key))
                mergedRows[key] = new MergedRow();
            mergedRows[key].AddSheet(sheetName, rowDict);
        }
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
