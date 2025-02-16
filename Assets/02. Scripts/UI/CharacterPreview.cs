using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterPreview : MonoBehaviour
{
    bool isDragging = false;

    [SerializeField] TMP_Text nameText;
    [SerializeField] Button buyButton;
    [SerializeField] Button applyButton;
    [SerializeField] LocalizeText applyText;
    [SerializeField] GameObject priceBox;
    [SerializeField] TMP_Text priceText;

    [SerializeField] float lerpSpeed = 15;

    [Space(10)]
    [SerializeField] GameObject fakeShadowPrefab;

    [Space(10)]
    [SerializeField] InputController inputController;
    [SerializeField] CharacterManager characterManager;

    [Space(10)]
    [SerializeField] Camera previewCamera;
    [SerializeField] GameObject previewList;
    [SerializeField] Transform[] pivot;

    PlayerCharacter[] previewCharacter = new PlayerCharacter[3];

    Vector2 beginPosition, currentPosition;
    Vector2 dir;

    CharacterID centerId = CharacterID.BASIC_0;

    float t = 0;

    void OnEnable() 
    {
        int characterCount = characterManager.GetCharacterCount();

        if(characterCount > 0)
        {
            ResetPreview();

            if(previewList && previewCamera)
            {
                previewCamera.gameObject.SetActive(true);
                previewList.SetActive(true);

                ResetCharacterHeadTrack(0);
                ResetCharacterHeadTrack(1);
                ResetCharacterHeadTrack(2);
            }
        }
    }

    void OnDisable() 
    {
        if(previewList && previewCamera)
        {
            previewCamera.gameObject.SetActive(false);
            previewList.SetActive(false);
        }
    }

    void Update()
    {
        if(isDragging)
        {
            UpdateDrag();
            UpdateDragPreview();
        }
        else
        {
            UpdatePreview();
        }
    }

    void UpdateDrag()
    {
        currentPosition = inputController.GetPointerPosition();
        currentPosition.y = 0;
        beginPosition.y = 0;

        dir = (currentPosition - beginPosition).normalized;
    }

    void UpdateDragPreview()
    {
        t = (Vector2.Distance(beginPosition, currentPosition) * 0.003f);

        if(dir.x < 0) // NOTE : 다음 캐릭터
        {
            previewCharacter[0].transform.position = pivot[0].position;
            previewCharacter[1].transform.position = Vector3.Lerp(pivot[1].transform.position, pivot[0].position, t);
            previewCharacter[2].transform.position = Vector3.Lerp(pivot[2].transform.position, pivot[1].position, t);
        }
        else if(dir.x > 0) // NOTE : 이전 캐릭터
        {
            previewCharacter[0].transform.position = Vector3.Lerp(pivot[0].transform.position, pivot[1].position, t);
            previewCharacter[1].transform.position = Vector3.Lerp(pivot[1].transform.position, pivot[2].position, t);
            previewCharacter[2].transform.position = pivot[2].position;
        }
    }

    void UpdatePreview()
    {
        previewCharacter[0].transform.position = Vector3.Lerp(previewCharacter[0].transform.position, pivot[0].position, lerpSpeed * Time.deltaTime);
        previewCharacter[1].transform.position = Vector3.Lerp(previewCharacter[1].transform.position, pivot[1].position, lerpSpeed * Time.deltaTime);
        previewCharacter[2].transform.position = Vector3.Lerp(previewCharacter[2].transform.position, pivot[2].position, lerpSpeed * Time.deltaTime);
    }

    void UpdateCharacterList()
    {
        if(dir.x < 0)
        {
            if(t > 0.5f)
            {
                NextCharacter();
            }
        }
        else if(dir.x > 0)
        {
            if(t > 0.5f)
            {
                PreviousCharacter();
            }
        }
    }

    void UpdateUI()
    {
        CharacterData data = characterManager.GetCharacterData(centerId);

        int price = data.GetPrice();
        bool isUnlock = (price <= 0 || characterManager.GetCharacterUnlockState(centerId)); 
        bool isApplied = characterManager.GetCurrentCharacterID().IsEquals(centerId);

        nameText.text = LocalizationManager.Instance.GetString(data.GetNameLocaleKey());

        applyButton.interactable = !isApplied;
        applyButton.gameObject.SetActive(isUnlock);
        applyText.SetLocaleString(isApplied ? "Applied" : "Apply");

        priceText.text = String.Format("{0} VP", price);
        priceBox.SetActive(!isUnlock);
        buyButton.interactable = CurrencyManager.Instance.CanPurchase(CurrencyType.VOXEL_POINT, price);
        buyButton.gameObject.SetActive(!isUnlock);
    }

    void ResetPreview()
    {
        centerId = characterManager.GetCurrentCharacterID();
        CharacterID nextId = centerId.Next();
        CharacterID previousId = centerId.Previous();

        SpawnCharacter(0, previousId);
        SpawnCharacter(1, centerId);
        SpawnCharacter(2, nextId);

        UpdateUI();
    }

    void SetCharacterPivot(int firstPivot, int secondPivot, int thirdPivot)
    {
        previewCharacter[0].transform.SetParent(pivot[firstPivot]);
        previewCharacter[1].transform.SetParent(pivot[secondPivot]);
        previewCharacter[2].transform.SetParent(pivot[thirdPivot]);

        ResetCharacterPivot();
    }

    void ResetCharacterPivot()
    {
        previewCharacter[0] = pivot[0].transform.GetChild(0).GetComponent<PlayerCharacter>();
        previewCharacter[1] = pivot[1].transform.GetChild(0).GetComponent<PlayerCharacter>();
        previewCharacter[2] = pivot[2].transform.GetChild(0).GetComponent<PlayerCharacter>();
    }

    public void SpawnCharacter(int pivotIdx, CharacterID characterId)
    {
        Destroy(previewCharacter[pivotIdx]?.gameObject);

        CharacterData data = characterManager.GetCharacterData(characterId);

        previewCharacter[pivotIdx] = Instantiate(data.GetPrefab(), pivot[pivotIdx]).GetComponent<PlayerCharacter>();
        previewCharacter[pivotIdx].transform.SetScale(data.GetPreviewScale());

        FakeShadow shadow = Instantiate(fakeShadowPrefab, transform.position, fakeShadowPrefab.transform.rotation, previewCharacter[pivotIdx].transform).GetComponent<FakeShadow>();
        shadow.SetTarget(previewCharacter[pivotIdx].transform);

        SetCharacterLayer(previewCharacter[pivotIdx].gameObject);
    }

    void SetCharacterLayer(GameObject targetObj)
    {
        targetObj.gameObject.layer = LayerMask.NameToLayer("PreviewCharacter");
        foreach(Transform child in targetObj.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("PreviewCharacter");
            if(child.childCount > 0)
            {
                SetCharacterLayer(child.gameObject);
            }
        }
    }

    public void ResetCharacterHeadTrack(int pivotIdx)
    {
        previewCharacter[pivotIdx].SetHeadTrackTarget(previewCamera.transform);
        previewCharacter[pivotIdx].EnableHeadTracking(true);
    }

    public void NextCharacter()
    {
        SetCharacterPivot(2,0,1);

        centerId = centerId.Next();
        CharacterID nextId = centerId.Next();

        SpawnCharacter(2, nextId);
        ResetCharacterHeadTrack(2);

        UpdateUI();

        SoundManager.Instance.PlaySound("Hit2");
    }

    public void PreviousCharacter()
    {
        SetCharacterPivot(1,2,0);

        centerId = centerId.Previous();
        CharacterID previousId = centerId.Previous();

        SpawnCharacter(0, previousId);
        ResetCharacterHeadTrack(0);

        UpdateUI();

        SoundManager.Instance.PlaySound("Hit2");
    }

    public void ApplyCharacter()
    {
        characterManager.ApplyCharacter(centerId);
        previewCharacter[1].EventMotion(1);

        UpdateUI();
    }

    public void BuyCharacter()
    {
        ConfirmPopup.Instance.Confirm("Buy", "BuyConfirmContext", () => 
        { 
            OnBuyCharacter();
        });
    }

    void OnBuyCharacter()
    {
        CharacterData data = characterManager.GetCharacterData(centerId);

        if(CurrencyManager.Instance.Purchase(CurrencyType.VOXEL_POINT, data.GetPrice()))
        {
            characterManager.RewardCharacter(data.GetID());
        }

        UpdateUI();
    }

    public void BeginDrag()
    {
        beginPosition = inputController.GetPointerPosition();

        ResetCharacterPivot();

        isDragging = true;
    }

    public void EndDrag()
    {
        UpdateCharacterList();

        isDragging = false;
    }
}
