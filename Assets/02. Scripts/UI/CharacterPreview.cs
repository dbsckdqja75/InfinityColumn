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

    [SerializeField] CharacterManager characterManager;

    [Space(10)]
    [SerializeField] Camera previewCamera;
    [SerializeField] GameObject previewList;
    [SerializeField] Transform[] pivot;

    GameObject[] previewCharacter = new GameObject[3];

    Vector2 beginPosition;
    Vector2 currentPosition;
    Vector2 dir;

    int characterCount = 0;
    int centerIdx = 0;
    float t = 0;

    void OnEnable() 
    {
        characterCount = characterManager.GetCharacterCount();

        if(characterCount > 0)
        {
            ResetPreview();

            if(previewList && previewCamera)
            {
                previewCamera.gameObject.SetActive(true);
                previewList.SetActive(true);
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
            UpdateInput();
            UpdatePreview();
        }
    }

    void UpdateInput()
    {
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousCharacter();
        }

        if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextCharacter();
        }
    }

    void UpdateDrag()
    {
        currentPosition = Input.mousePosition;
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
        CharacterData data = characterManager.GetCharacterData((CharacterID)centerIdx);

        int price = data.GetPrice();
        bool isUnlock = (price <= 0 || characterManager.GetCharacterUnlockState((CharacterID)centerIdx)); 
        bool isApplied = characterManager.GetCurrentCharacterID().IsEquals((CharacterID)centerIdx);

        nameText.text = data.GetName();

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
        centerIdx = (int)characterManager.GetCurrentCharacterID();
        
        foreach(GameObject character in previewCharacter)
        {
            Destroy(character);
        }

        int nextIdx = (int)Mathf.Repeat(centerIdx+1, characterCount);
        int previousIdx = (int)Mathf.Repeat(centerIdx-1, characterCount);

        previewCharacter[0] = Instantiate(characterManager.GetPrefab((CharacterID)previousIdx), pivot[0]);
        previewCharacter[1] = Instantiate(characterManager.GetPrefab((CharacterID)centerIdx), pivot[1]);
        previewCharacter[2] = Instantiate(characterManager.GetPrefab((CharacterID)nextIdx), pivot[2]);

        previewCharacter[0].transform.SetScale(1);
        previewCharacter[1].transform.SetScale(1);
        previewCharacter[2].transform.SetScale(1);

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
        previewCharacter[0] = pivot[0].transform.GetChild(0).gameObject;
        previewCharacter[1] = pivot[1].transform.GetChild(0).gameObject;
        previewCharacter[2] = pivot[2].transform.GetChild(0).gameObject;
    }

    public void NextCharacter()
    {
        SetCharacterPivot(2,0,1);

        centerIdx = (int)Mathf.Repeat(centerIdx+1, characterCount);
        int nextIdx = (int)Mathf.Repeat(centerIdx+1, characterCount);

        Destroy(previewCharacter[2]);

        previewCharacter[2] = Instantiate(characterManager.GetPrefab((CharacterID)nextIdx), pivot[2]);

        SoundManager.Instance.PlaySound("Hit2");

        UpdateUI();
    }

    public void PreviousCharacter()
    {
        SetCharacterPivot(1,2,0);

        centerIdx = (int)Mathf.Repeat(centerIdx-1, characterCount);
        int previousIdx = (int)Mathf.Repeat(centerIdx-1, characterCount);

        Destroy(previewCharacter[0]);

        previewCharacter[0] = Instantiate(characterManager.GetPrefab((CharacterID)previousIdx), pivot[0]);

        SoundManager.Instance.PlaySound("Hit2");

        UpdateUI();
    }

    public void ApplyCharacter()
    {
        characterManager.ApplyCharacter((CharacterID)centerIdx);

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
        CharacterData data = characterManager.GetCharacterData((CharacterID)centerIdx);
        if(CurrencyManager.Instance.Purchase(CurrencyType.VOXEL_POINT, data.GetPrice()))
        {
            characterManager.RewardCharacter(data.GetID());
        }

        UpdateUI();
    }

    public void BeginDrag()
    {
        beginPosition = Input.mousePosition;

        ResetCharacterPivot();

        isDragging = true;
    }

    public void EndDrag()
    {
        UpdateCharacterList();

        isDragging = false;
    }
}
