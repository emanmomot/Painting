using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GUIManager : MonoBehaviour {

	public static GUIManager singleton;

	public Text guiTextMode;
	public Slider sizeSlider;
	public TexturePainter painter;

	void Awake() {
		singleton = this;
	}

	public void SetBrushMode(int newMode){
		Painter_BrushMode brushMode =newMode==0? Painter_BrushMode.DECAL:Painter_BrushMode.PAINT; //Cant set enums for buttons :(
		string colorText=brushMode==Painter_BrushMode.PAINT?"orange":"purple";	
		guiTextMode.text="<b>Mode:</b><color="+colorText+">"+brushMode.ToString()+"</color>";
		painter.SetBrushMode (brushMode);
	}
	public void UpdateSizeSlider(){
		//painter.SetBrushSize (sizeSlider.value);
	}

	public void ReduceBrushSize(float amt) {
		sizeSlider.value -= amt;
		if (sizeSlider.value < sizeSlider.minValue) {
			sizeSlider.value = sizeSlider.minValue;
		}
	}

	public void IncreaseBrushSize(float amt) {
		sizeSlider.value += amt;
		if (sizeSlider.value > sizeSlider.maxValue) {
			sizeSlider.value = sizeSlider.maxValue;
		}
	}
}
