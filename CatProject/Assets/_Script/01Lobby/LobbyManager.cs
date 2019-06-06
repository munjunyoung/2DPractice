using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LobbyManager : MonoBehaviour
{
    //싱글턴
    public static LobbyManager instance = null;
    private PLAYER_TYPE pType;
    public GameObject selectCompeletePanel;
    public Text selectCompeleteText;
    [SerializeField]
    private Camera cam;
    //BackGround
    [Header("BackGround Tilemap")]
    [SerializeField]
    private GameObject GridObject;
    [SerializeField]
    private Tilemap backgroundTilemap;
    [SerializeField, Range(5,60)]
    private int width, height;
    [SerializeField]
    private Room_TileType spriteType;

    private void Awake()
    {
        instance = this;

        selectCompeletePanel.SetActive(false);
        //BackGround 생성
        //CreateBackGround();
        //카메라 배경포지션 센터 설정
        cam.transform.position = new Vector3(width * 0.5f, height * 0.5f, -1f);
    }
    #region UI
    public void ActiveSelectCompletePanel(PLAYER_TYPE _pType)
    {
        selectCompeletePanel.SetActive(true);
        selectCompeleteText.text = "'" + _pType.ToString() + "' START ?";
        pType = _pType;
    }
    
    public void YesButtonInSelectCompleteUI()
    {
        LoadStageScene();
    }

    public void NoButtonInSelectCompleteUI()
    {
        
        selectCompeletePanel.SetActive(false);
    }
    #endregion

    public void LoadStageScene()
    {
        GlobalManager.instance.pType = pType;
        GlobalManager.instance.LoadScene(Scene_Name.S_02StageSelect);
    }

    private void CreateBackGround()
    {
        LoadDataManager loadData = new LoadDataManager();
        for (int i = 0; i < width; i++)
        {
            backgroundTilemap.SetTile(new Vector3Int(i, -1, 0), loadData.tileDataArray[(int)spriteType].terrainRuleTile);
            backgroundTilemap.SetTile(new Vector3Int(i, -2, 0), loadData.tileDataArray[(int)spriteType].terrainRuleTile);
            backgroundTilemap.SetTile(new Vector3Int(i, height, 0), loadData.tileDataArray[(int)spriteType].terrainRuleTile);
            backgroundTilemap.SetTile(new Vector3Int(i, height + 1, 0), loadData.tileDataArray[(int)spriteType].terrainRuleTile);
        }

        ////left, right
        //for (int j = -2; j < height + 2; j++)
        //{
        //    backgroundTilemap.SetTile(new Vector3Int(-1, j, 0), loadData.tileDataArray[(int)spriteType].terrainRuleTile);
        //    backgroundTilemap.SetTile(new Vector3Int(-2, j, 0), loadData.tileDataArray[(int)spriteType].terrainRuleTile);
        //    backgroundTilemap.SetTile(new Vector3Int(width, j, 0), loadData.tileDataArray[(int)spriteType].terrainRuleTile);
        //    backgroundTilemap.SetTile(new Vector3Int(width + 1, j, 0), loadData.tileDataArray[(int)spriteType].terrainRuleTile);
        //}

        GameObject tmpParent = new GameObject("BackGroundParent");
        int count = 0;
        //배경 오브젝트 생성
        foreach (var tmptile in loadData.tileDataArray[(int)spriteType].backGroundTile)
        {
            GameObject backgroundob = new GameObject("BackGround", typeof(SpriteRenderer));
            backgroundob.transform.localPosition = Vector3.zero;
            backgroundob.transform.localRotation = Quaternion.identity;
            backgroundob.GetComponent<SpriteRenderer>().sortingLayerName = "BackGround";
            backgroundob.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
            backgroundob.GetComponent<SpriteRenderer>().sprite = tmptile.sprite;
            backgroundob.GetComponent<SpriteRenderer>().size = new Vector2(width, height);
            backgroundob.GetComponent<SpriteRenderer>().sortingOrder = count;
            count++;
            backgroundob.transform.SetParent(tmpParent.transform);
        }
        tmpParent.transform.SetParent(GridObject.transform);
    }
}
