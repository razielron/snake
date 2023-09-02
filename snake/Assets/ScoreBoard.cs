using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    Text firstPoints;
    [SerializeField]
    Text secondPoints;
    [SerializeField]
    Text thirdPoints;
    [SerializeField]
    Text forthPoints;
    [SerializeField]
    Text fifthPoints;
    public List<double> ScoreList { get; private set; } = new List<double>();
    public double LastScore { get; private set; }

    public void Back()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void Start()
    {
        // Load scores from PlayerPrefs
        for (int i = 0; i < 5; i++)
        {
            string scoreStr = PlayerPrefs.GetString("TopScore" + i, null);  // Default to null if not found
            if (!string.IsNullOrEmpty(scoreStr))
            {
                double score = double.Parse(scoreStr);
                ScoreList.Add(score);
            }
        }

        // Sort the loaded scores
        ScoreList = ScoreList.OrderByDescending(x => x).ToList();
        // Update ScoreList with last score result
        UpdateScoreList();
        // Update the text fields
        Text[] scoreTexts = { firstPoints, secondPoints, thirdPoints, forthPoints, fifthPoints };

        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if (i < ScoreList.Count)
            {
                // If there is a score for this position, update the text field
                scoreTexts[i].text = ScoreList[i].ToString();
            }
            else
            {
                // If there is no score for this position, set the text to "-"
                scoreTexts[i].text = "-";
            }
        }
    }

    private void UpdateScoreList()
    {
        // Get the last score from PlayerPrefs; it could be null
        string lastScore = PlayerPrefs.GetString("LastScore", null);
        PlayerPrefs.DeleteKey("LastScore");  // Remove the saved score from PlayerPrefs

        // Check if lastScore is not null or empty
        if (!string.IsNullOrEmpty(lastScore))
        {
            // Parse the string to a double
            LastScore = double.Parse(lastScore);
            // Add the LastScore to ScoreList
            ScoreList.Add(LastScore);
            // Sort ScoreList in descending order (highest to lowest)
            ScoreList = ScoreList.OrderByDescending(x => x).ToList();

            // Keep only the top 5 scores
            while (ScoreList.Count > 5)
            {
                ScoreList.RemoveAt(ScoreList.Count - 1);  // Remove the smallest score
            }

            // Save the top 5 scores back to PlayerPrefs
            for (int i = 0; i < ScoreList.Count; i++)
            {
                PlayerPrefs.SetString("TopScore" + i, ScoreList[i].ToString());
            }

            PlayerPrefs.Save();  // Make sure to save the changes
        }
    }
}
