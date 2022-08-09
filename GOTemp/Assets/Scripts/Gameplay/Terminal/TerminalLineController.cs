using System.Collections.Generic;
using UnityEngine;

public class TerminalLineController : MonoBehaviour
{
    [SerializeField]
    private TerminalLine linePrefab = null;
    [SerializeField]
    private Transform lineParent = null;
    [SerializeField]
    private int linesPooling = 20;

    private List<TerminalLine> lines = new List<TerminalLine>();

    private void Awake()
    {
        PoolingLines();
    }

    public void ShowLines(bool show)
    {
        foreach (var singleLine in lines)
        {
            if (singleLine.Drawn)
            {
                singleLine.gameObject.SetActive(show);
            }
        }
    }

    public void RemoveLine(TerminalLine line)
    {

        line.Drawn = false;
        line.gameObject.SetActive(false);
    }

    public TerminalLine DrawLine(Vector3 startPos, Vector3 endPos)
    {
        foreach (var singleLine in lines)
        {
            if (!singleLine.Drawn)
            {
                singleLine.gameObject.SetActive(true);
                singleLine.SetPosition(startPos, endPos);
                singleLine.Drawn = true;
                return singleLine;
            }
        }

        TerminalLine newLine = Instantiate(linePrefab, lineParent);
        newLine.gameObject.SetActive(true);
        newLine.SetPosition(startPos, endPos);
        newLine.Drawn = true;
        lines.Add(newLine);

        return newLine;
    }

    private void PoolingLines()
    {
        lines.Clear();
        for (int i = 0; i < linesPooling; i++)
        {
            TerminalLine newLine = Instantiate(linePrefab, lineParent);
            newLine.gameObject.SetActive(false);
            lines.Add(newLine);
        }
    }
}
