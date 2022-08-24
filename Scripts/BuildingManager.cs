using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour{

    public static BuildingManager Instance { get; private set; }

    private BuildingTypeSo activeBuildingType;
    private BuildingTypeListSo buildingTypeList;


    public event EventHandler<OnActiveBuildingTypeChangedEventArgs> OnActiveBuildingTypeChanged;

    public class OnActiveBuildingTypeChangedEventArgs: EventArgs
    {
        public BuildingTypeSo activebuildingType;
    }
    private Camera mainCamera;

    [SerializeField] private Building hqBuilding;
    private void Awake()
    {
        Instance = this;
        buildingTypeList = Resources.Load<BuildingTypeListSo>(typeof(BuildingTypeListSo).Name);
    }
    private void Start()
    {
        mainCamera = Camera.main;

        hqBuilding.GetComponent<HealthSystem>().OnDied += HQ_OnDied;

        }

    private void HQ_OnDied(object sender, EventArgs e)
    {
        GameOverUI.Instance.Show();
    }


    // Update is called once per frame
    private void Update(){

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()  )
        {
            if(activeBuildingType != null)
            {
                if (CanSpawnBuilding(activeBuildingType, UtilsClass.GetMouseWorldPosition(), out string errorMessage))
                {
                    if (ResourceManager.Instance.CanAfford(activeBuildingType.constructionResourceCostArray))
                    {
                        ResourceManager.Instance.SpendResources(activeBuildingType.constructionResourceCostArray);
                        Instantiate(activeBuildingType.prefab, UtilsClass.GetMouseWorldPosition(), Quaternion.identity);
                    }
                    else
                    {
                        TooltipUI.Instance.Show("Cannot afford " + activeBuildingType.GetConstructionResourceCostString(),
                            new TooltipUI.TooltipTimer { timer = 2f });
                    }
                }
                else
                {
                    TooltipUI.Instance.Show(errorMessage, new TooltipUI.TooltipTimer {timer = 2f });
                }
                }




        }

           
        

        if (Input.GetKeyDown(KeyCode.T))
        {
            activeBuildingType = buildingTypeList.list[0];
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            activeBuildingType = buildingTypeList.list[1];
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Vector3 enemySpawnPosition = UtilsClass.GetMouseWorldPosition() + UtilsClass.GetRandomDir() * 5f;
            Enemy.Create(enemySpawnPosition);
        }
    }



    public void SetActiveBuildingType(BuildingTypeSo buildingType)
    {
        activeBuildingType = buildingType;

        OnActiveBuildingTypeChanged?.Invoke(this, new OnActiveBuildingTypeChangedEventArgs { activebuildingType = activeBuildingType });
    }


    public BuildingTypeSo GetActiveBuildingType()
    { 
        return activeBuildingType;
    }


    private bool CanSpawnBuilding(BuildingTypeSo buildingType, Vector3 position, out string errorMessage)
    {
        BoxCollider2D boxCollider2D = buildingType.prefab.GetComponent<BoxCollider2D>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(position , new Vector2(2,2), 0);

        collider2DArray.Length = 0;
        if (!isAreaClear) {

            errorMessage = "Area is not clear!";
            return false; }

        collider2DArray = Physics2D.OverlapCircleAll(position, buildingType.minConstructionRadius);
        foreach(Collider2D collider2D in collider2DArray)
        {
            // Colliders inside the construction radius
            BuildingTypeHolder buildingTypeHolder = collider2D.GetComponent<BuildingTypeHolder>();
            if(buildingTypeHolder != null)
            {
                // Has a BuildingTypeHolder
                if(buildingTypeHolder.buildingType == buildingType)
                {
                    errorMessage = "Too close to another building of same type!";
                    // Theres already building
                    return false;
                }
            }
        }

        float maxConstructionRadius = 25;
        collider2DArray = Physics2D.OverlapCircleAll(position, maxConstructionRadius);
        foreach (Collider2D collider2D in collider2DArray)
        {
            // Colliders inside the construction radius
            BuildingTypeHolder buildingTypeHolder = collider2D.GetComponent<BuildingTypeHolder>();
            if (buildingTypeHolder != null)
            {
                errorMessage = "";
                return true;

            }
        }
        errorMessage = "Too far from any other building";
        return false;

    }

    public Building GetHQBuilding()
    {
        return hqBuilding;
    }
}
