using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System;

public class BitmapFont : MonoBehaviour {

	[Header("BMFont")]
	public TextAsset bitmapFontXml;
	public Material bitmapFontMaterial;
	public float glyphHeight;
	public float glyphWidth;
	public float quadHeightScale;

	[Header("REXPaint CP437")]
	public bool useRexPaintFont = false;
	public TextAsset IBMGRAPH;
	public Material rexPaintFontMaterial;
	public float rexPaintTextureSize;
	public int rexPaintGridSize;
	public float rexPaintBleed;
	public float rexPaintQuadHeightScale;

	private float rexPaintGlyphSize;

	[HideInInspector]
	public float textureSize;

	[HideInInspector]
	public bool fontLoaded = false;

	private Dictionary<string, BitmapFontGlyph> glyphs = new Dictionary<string, BitmapFontGlyph>();

	public void Awake(){


		if (!useRexPaintFont) {
			string fontXmlText = bitmapFontXml.text;
			XmlReader reader = XmlReader.Create(new StringReader(fontXmlText));
			while (reader.Read()) {


				if (reader.LocalName == "common") {
					textureSize = float.Parse(reader.GetAttribute("scaleW"));
				}

	
				else if (reader.LocalName == "char") {
					string glyphString = char.ConvertFromUtf32(int.Parse(reader.GetAttribute("id")));

		
					if (!glyphs.ContainsKey(glyphString)) {
						BitmapFontGlyph glyph = new BitmapFontGlyph();
						glyph.glyphString = glyphString;
						glyph.x = float.Parse(reader.GetAttribute("x"));
						glyph.y = float.Parse(reader.GetAttribute("y"));
						glyph.xOffset = float.Parse(reader.GetAttribute("xoffset"));
						glyph.yOffset = float.Parse(reader.GetAttribute("yoffset"));
						glyph.width = float.Parse(reader.GetAttribute("width"));
						glyph.height = float.Parse(reader.GetAttribute("height"));
						glyphs.Add(glyphString, glyph);
						glyph.RecalculateGlyphMetrics(glyphWidth, glyphHeight, textureSize, 0f);
					}
				}
			}
		}


		else {


			Dictionary<int,int> ibmGraphMapping = BitmapFont.GetIBMGRAPHMapping(IBMGRAPH);


			rexPaintGlyphSize = rexPaintTextureSize / (float) rexPaintGridSize;


			textureSize = rexPaintTextureSize;

	
			for (int y = 0; y < rexPaintGridSize; y++) {
				for (int x = 0; x < rexPaintGridSize; x++) {
					
					string glyphString = CP437ToUnicode(Mathf.RoundToInt(y * rexPaintGridSize + x), ibmGraphMapping);

		
					if (!glyphs.ContainsKey(glyphString)) {
						BitmapFontGlyph glyph = new BitmapFontGlyph();
						glyph.glyphString = glyphString;
						glyph.x = x * rexPaintGlyphSize;
						glyph.y = y * rexPaintGlyphSize;
						glyph.xOffset = 0f;
						glyph.yOffset = 0f;
						glyph.width = rexPaintGlyphSize;
						glyph.height = rexPaintGlyphSize;
						glyphs.Add(glyph.glyphString, glyph);
						glyph.RecalculateGlyphMetrics(
							rexPaintGlyphSize,
							rexPaintGlyphSize, 
							textureSize, 
							rexPaintBleed);
					}
				}
			}
		}


		fontLoaded = true;
		Debug.Log("FONT TEXTURE SIZE: " + textureSize);
		Debug.Log(glyphs.Count + " GLYPHS LOADED!");
	}

	public Material GetFontMaterial(){
		return useRexPaintFont ? rexPaintFontMaterial : bitmapFontMaterial;
	}

	public float GetGlyphWidth(){
		return useRexPaintFont ? rexPaintGlyphSize : glyphWidth;
	}

	public float GetGlyphHeight(){
		return useRexPaintFont ? rexPaintGlyphSize : glyphHeight;
	}

	public BitmapFontGlyph GetGlyph(string glyphString){
		BitmapFontGlyph glyph;

	
		if (glyphs.TryGetValue(glyphString, out glyph)) {
			return glyph;
		}

	
		else {
			return glyphs["?"];
		}
	}

	public static Dictionary<int,int> GetIBMGRAPHMapping(TextAsset IBMGRAPH){
		

		Dictionary<int,int> ibmGraphMapping = new Dictionary<int, int>();
		string[] lines = IBMGRAPH.ToString().Split('\n');
		foreach (string line in lines) {


			if (!line.StartsWith("#") && line.Trim().Length > 0) {

	
				string[] tabs = line.Split('	');
				int cp437 = Convert.ToInt32(tabs[1], 16);
				int unicode = Convert.ToInt32(tabs[0], 16);
				ibmGraphMapping.Add(cp437, unicode);
			}
		}

		return ibmGraphMapping;
	}

	public static string CP437ToUnicode(int asciiCode, Dictionary<int,int> ibmGraphMapping){

		string character = "";


		if (asciiCode == 0) {
			character = " ";
		}


		else if (!ibmGraphMapping.ContainsKey(asciiCode)) {
			character = Encoding.GetEncoding(437).GetString(new byte[]{ (byte)asciiCode });
		}


		else {
			character = char.ConvertFromUtf32(ibmGraphMapping[asciiCode]);
		}

		return character;
	}
}
