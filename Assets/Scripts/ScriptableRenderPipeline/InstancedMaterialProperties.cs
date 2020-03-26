using UnityEngine;


public class InstancedMaterialProperties : MonoBehaviour {
	
	static MaterialPropertyBlock propertyBlock;

	static int colorID = Shader.PropertyToID("_Color");
	static int smoothnessID = Shader.PropertyToID("_Smoothness");
	static int metallicID = Shader.PropertyToID("_Metallic");
	static int emissionID = Shader.PropertyToID("_EmissionColor");

	[SerializeField]
	Color color = Color.white;
	[SerializeField, Range(0f, 1f)]
	float metallic = 0;
	[SerializeField, Range(0f, 1f)]
	float smoothness = 0.5f;
	[SerializeField, ColorUsage(false, true)]
	Color emissionColor = Color.black;
	[SerializeField]
	float pulseEmissionFreqency;

	void Awake () {
		OnValidate();
		if (pulseEmissionFreqency <= 0f) {
			enabled = false;
		}
	}
	private void Update() {
		Color originalEmissionColor = emissionColor;
		emissionColor *= 0.5f + 
			0.5f * Mathf.Cos(2f * Mathf.PI * pulseEmissionFreqency * Time.time);
		OnValidate();
		GetComponent<MeshRenderer>().UpdateGIMaterials();
		DynamicGI.SetEmissive(GetComponent<MeshRenderer>(), emissionColor);
		emissionColor = originalEmissionColor;
	}

	void OnValidate () {
		if (propertyBlock == null) {
			propertyBlock = new MaterialPropertyBlock();
		}

		propertyBlock.SetColor(colorID, color);
		propertyBlock.SetFloat(smoothnessID, smoothness);
		propertyBlock.SetFloat(metallicID, metallic);
		propertyBlock.SetColor(emissionID, emissionColor);
		GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
	}
}