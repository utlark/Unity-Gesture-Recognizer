using QDollarGestureRecognizer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QDollar : MonoBehaviour
{
    [Header("QDollar")]
    [Tooltip("¿ffects the speed.")]
    public bool useEarlyAbandoning = true;
    [Tooltip("¿ffects the speed.")]
    public bool useLowerBounding = true;
    [Tooltip("The difference between two point-cloud. The greater the difference, the lower the accuracy..")]
    public float tolerance = 15f;
    public TextAsset[] gesturesXML;

    [Header("Debugging")]
    public bool debug = false;

    private Gesture[] _trainingSet;
    private readonly List<Point> Points = new List<Point>();
    private bool _dataSetLoaded = false;

    private void Awake()
    {
        StartCoroutine(LoadTrainingSet());

        QPointCloudRecognizer.UseEarlyAbandoning = useEarlyAbandoning;
        QPointCloudRecognizer.UseLowerBounding = useLowerBounding;
    }

    public async Task<string> Evaluate(List<Vector2> target)
    {
        if (_dataSetLoaded)
        {
            Points.Clear();
            if (target.Count > 2)
            {
                foreach (var point in target)
                    Points.Add(new Point(point, 1));

                Gesture candidate = new Gesture(Points.ToArray());
                Tuple<string, float> answer = new Tuple<string, float>("0", 0);

                await Task.Run(() => 
                {  
                    answer = QPointCloudRecognizer.Classify(candidate, _trainingSet); 
                });

                if (debug)
                    Debug.Log($"ŒÚ‚ÂÚ: {answer.Item1} –‡ÁÌËˆ‡ {answer.Item2}");

                if (answer.Item2 < tolerance)
                    return answer.Item1;            
            }
        }
        return "0";
    }

    IEnumerator LoadTrainingSet()
    {
        List<Gesture> gestures = new List<Gesture>();
        foreach (TextAsset file in gesturesXML)
        {
            yield return null;
            gestures.Add(GestureIO.ReadGesture(file));
        }
        _trainingSet = gestures.ToArray();
        _dataSetLoaded = true;
    }
}
