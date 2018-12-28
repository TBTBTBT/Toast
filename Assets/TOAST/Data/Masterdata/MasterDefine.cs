using System;
//-----------------------------------------------------------------------------

//マスターデータの定義ファイル
//定義方法 
// IMasterRecordを実装したクラスを作る
// MasterPath属性でjsonのパスを指定(Asset/Resource以下)
//ひな形
//[MasterPath("")]
//public class MstNameRecord : IMasterRecord{public int id { get; set; }}

//-----------------------------------------------------------------------------

[Serializable]
[MasterPath("/Master/mst_unit.json")]
public class MstUnitRecord : IMasterRecord
{
    public int Id { get { return id; } }
    public int id;
    public int atk;
    public int def;
    public int spd;

    //public int allowequipid;
    public string jobname;
}
[Serializable]
[MasterPath("/Master/mst_function.json")]
public class MstFunctionRecord : IMasterRecord
{
    public int Id { get { return id; } }
    public int id;
    public string functionkey;//処理
    public string actionkey;//見た目
    public string name;
    public int waitframe;
    public string imagepath;

}
[Serializable]
[MasterPath("/Master/mst_object.json")]
public class MstObjectRecord : IMasterRecord
{
    public int Id { get { return id; } }
    public int id;
    public string name;
    public string prefabPath;
    public int waitframe;

}