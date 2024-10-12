using System;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{

    [Serializable]
    public class PanelOption
    {
        public bool isActive;
        public bool isIgnore;
    }

    [Serializable]
    public class PanelSet
    {
        public string NAME = "Empty";

        public List<PanelOption> options = new List<PanelOption>();
    }

    Panel currentPanel;

    public List<Panel> panelList = new List<Panel>();
    public List<PanelSet> panelSetList = new List<PanelSet>();

    public void SetPanel(string targetName)
    {
        PanelSet panelSet = panelSetList.Find(x => (x.NAME.ToLower() == targetName.ToLower()));
        if (panelSet != null)
        {
            SetPanel(panelSet);
        }
        else
        {
            Debug.LogWarningFormat("[CanvasManager] SetPanel - 해당되는 PanelSet Name ({0}) 이 존재하지 않습니다!", targetName);
        }
    }

    public void SetPanel(int targetIdx)
    {
        if(targetIdx < panelSetList.Count)
        {
            SetPanel(panelSetList[targetIdx]);
        }
        else
        {
            Debug.LogWarningFormat("[CanvasManager] SetPanel - 해당되는 PanelSet Index ({0}) 가 존재하지 않습니다!", targetIdx);
        }
    }

    void SetPanel(PanelSet panelSet)
    {
        for (int i = 0; i < panelList.Count; i++)
        {
            if (panelList[i] != null)
            {
                if (!panelSet.options[i].isIgnore)
                {
                    bool isActive = panelSet.options[i].isActive;
                    panelList[i].SetPanel(isActive);
                    
                    if(isActive)
                    {
                        currentPanel = panelList[i];
                    }
                }
            }
        }
    }

    public bool ClosePanel()
    {
        if(currentPanel)
        {
            if(currentPanel is PanelWithAnimation)
            {
                currentPanel.Close();

                return true;
            }
        }
        return false;
    }
}
