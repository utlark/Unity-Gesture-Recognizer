using UnityEngine;
using TensorFlow;
using System.Threading.Tasks;

[RequireComponent(typeof(TextAsset))]
public class TensorFlowRecognizer : MonoBehaviour
{
	[Header("Components")]
	public TextAsset graphModel;

	[Header("Parameters")]
	[Tooltip("The minimum Color value to set white on texture.")]
	public float whiteBorder = 0.8f;
	[Tooltip("Tolerance for glyph evaluation.")]
	public float tolerance = 0.8f;

	[Header("Debugging")]
	public bool debug = false;

	private const int ImgWidth = 28; // Ширина изображения, на которой была натренирована нейросеть.
	private const int ImgHeight = 28; // Высота изображения, на которой была натренирована нейросеть.
	private readonly float[,,,] InputImg = new float[1, ImgWidth, ImgHeight, 1];

	public async Task<string> Evaluate(Texture2D texture28x28) 
	{
		if (texture28x28 != null)
		{
			for (int i = 0; i < ImgWidth; i++)
				for (int j = 0; j < ImgHeight; j++)
					if (texture28x28.GetPixel(j, i).r < whiteBorder)
						InputImg[0, ImgWidth - i - 1, j, 0] = 1;
					else
						InputImg[0, ImgWidth - i - 1, j, 0] = 0;

			TFGraph graph = new TFGraph();
			graph.Import(graphModel.bytes);
			TFSession session = new TFSession(graph);
			TFSession.Runner runner = session.GetRunner();

			float highestVal = 0; //Вероятность
			int highestInd = -1; //Ответ

			await Task.Run(() =>
					{
						runner.AddInput(graph["conv2d_1_input"][0], InputImg);
						runner.Fetch(graph["dense_2/Softmax"][0]);

						float[,] recurrent_tensor = runner.Run()[0].GetValue() as float[,];

						for (int j = 0; j < 10; j++)
						{
							float confidence = recurrent_tensor[0, j];
							if (highestInd > -1)
							{
								if (recurrent_tensor[0, j] > highestVal)
								{
									highestVal = confidence;
									highestInd = j;
								}
							}
							else
							{
								highestVal = confidence;
								highestInd = j;
							}
						}
					});

			if (debug)
				Debug.Log($"Ответ: {highestInd} Вероятность: {highestVal}");

			if (highestVal >= tolerance)
				return highestInd.ToString();
		}
		return "0";
	}
}
