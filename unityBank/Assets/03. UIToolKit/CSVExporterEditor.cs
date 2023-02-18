using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class CSVExporterEditor : EditorWindow
{
    [MenuItem("Window/WeKlem/Open CSV Exporter")]
    public static void ShowEditor()
    {
        CSVExporterEditor wnd = GetWindow<CSVExporterEditor>();
        wnd.titleContent = new GUIContent("CSV Exporter");
    }

    public void CreateGUI()
    {

        VisualElement root = rootVisualElement;

        // SO 값을 저장해둘 변수.
        SimControllerSO soValue = null;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/03. UIToolKit/CSVExporterEditor.uxml");
        VisualElement treeFromUXML = visualTree.Instantiate();
        root.Add(treeFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/03. UIToolKit/CSVExporterEditor.uss");

        VisualElement VE3 = treeFromUXML.Q<VisualElement>("VE3");

        var SOField = new ObjectField("SimControllerSO 파일을 첨부해주세요");

        // SimControllerSO 오브젝트만 Input 가능하게 제한.
        SOField.objectType = typeof(SimControllerSO);

        // 씬에서 오브젝트 끌어 놓기 끄기.
        SOField.allowSceneObjects = false;


        // 필드에 값 들어오면 SO 데이터 저장.
        SOField.RegisterCallback<ChangeEvent<Object>>((evt) =>
        {
            // Object -> SimControllerSO 캐스팅
            soValue = (SimControllerSO)evt.newValue;
        });



        VE3.Add(SOField);


        // 버튼 가져오기.
        Button exportBtn = treeFromUXML.Q<Button>("Export");

        // 버튼 클릭스 필드의 데이터 export하기.
        exportBtn.clicked += () => { runExportBtn(soValue); };

    }


    void runExportBtn(SimControllerSO so)
    {

        if (so == null)
        {
            Debug.LogError("필드에 SO 파일이 없습니다. 확인해주세요!");

            return;
        }


        CSV_Exporter exporter = new CSV_Exporter();

        exporter.ExportToCSV("my_data", so);
    }

}