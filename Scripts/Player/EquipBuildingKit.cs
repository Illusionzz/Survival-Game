using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipBuildingKit : Equip
{
    public GameObject buildingWindow;
    private BuildingRecipe curRecipe;
    private BuildingPreview curBuildingPreview;

    public float placementUpdateRate = 0.03f;
    public float lastPlacementUpdateTime;
    public float placementMaxDistance = 5.0f;

    public LayerMask placementLayerMask;

    public Vector3 placementPosition;
    private bool canPlace;
    private float curYRot;

    public float rotateSpeed = 180.0f;

    private Camera cam;
    public static EquipBuildingKit instance;

    void Awake()
    {
        instance = this;
        cam = Camera.main;
    }

    void Start()
    {
        buildingWindow = FindObjectOfType<BuildingWindow>(true).gameObject;
    }

    public override void OnAttackInput()
    {
        if (curRecipe == null || curBuildingPreview == null || !canPlace) {
            return;
        }

        Instantiate(curRecipe.spawnPrefab, curBuildingPreview.transform.position, curBuildingPreview.transform.rotation);

        for (int x = 0; x < curRecipe.cost.Length; x++) {
            for (int y = 0; y < curRecipe.cost[x].quantity; y++) {
                Inventory.instance.RemoveItem(curRecipe.cost[x].item);
            }
        }

        curRecipe = null;
        Destroy(curBuildingPreview.gameObject);
        curBuildingPreview = null;
        canPlace = false;
        curYRot = 0;
    }

    public override void OnAltAttackInput()
    {
        if (curBuildingPreview != null) {
            Destroy(curBuildingPreview.gameObject);
        }
                
        buildingWindow.SetActive(true);
        PlayerController.instance.ToggleCursor(true);
    }

    public void SetNewBuildingRecipe(BuildingRecipe recipe) 
    {
        curRecipe = recipe;
        buildingWindow.SetActive(false);
        PlayerController.instance.ToggleCursor(false);

        curBuildingPreview = Instantiate(recipe.previewPrefab).GetComponent<BuildingPreview>();
    }

    void Update()
    {
        if (curRecipe != null && curBuildingPreview != null && Time.time - lastPlacementUpdateTime > placementUpdateRate) {
            lastPlacementUpdateTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, placementMaxDistance, placementLayerMask)) {
                curBuildingPreview.transform.position = hit.point;
                curBuildingPreview.transform.up = hit.normal;
                curBuildingPreview.transform.Rotate(new Vector3(0.0f, curYRot, 0.0f), Space.Self);

                if (!curBuildingPreview.CollidingWithObjects()) {
                    if (!canPlace)
                        curBuildingPreview.CanPlace();
                    
                    canPlace = true;
                }
                else  {
                    if (canPlace)
                        curBuildingPreview.CantPlace();

                    canPlace = false;
                }
            }
        } 

        if (Keyboard.current.rKey.isPressed) {
            curYRot += rotateSpeed * Time.deltaTime;

            if (curYRot > 360.0f)
                curYRot = 0.0f;
        }
    }

    void OnDestroy()
    {
        if (curBuildingPreview != null) {
            Destroy(curBuildingPreview.gameObject);
        }
    }
}
