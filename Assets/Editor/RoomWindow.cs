﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class RoomWindow : EditorWindow {

	GameObject itemObject;
	bool itemSelected = false;

	GameObject roomObject;
	GameObject roomPreview;
	string currentRoomPreview;
	bool roomSelected = false;

	List<GameObject> objectList;
	bool foldList;
	bool foldTexture;
	//For ItemObject;
	RoomObject currentObject;
	Texture currentTexture;
	Editor gameobjectEditor;

	int roomNumber;
	
	enum RoomType{
		Default,
		Hall,
		Corner,
		TRoom,
		Plus,
		BigRoom
	}
	RoomType roomtype;
	[MenuItem ("Window/RoomEditor")]
	static void Init(){
		RoomWindow window = (RoomWindow)EditorWindow.GetWindow(typeof(RoomWindow));
	}

	void OnInspectorUpdate(){
		if(Selection.activeGameObject != null){
			if(Selection.activeGameObject.tag == "Item" && !itemSelected){
				itemObject = Selection.activeGameObject.gameObject;
				itemSelected = true;
				currentObject = itemObject.GetComponent<RoomObject>();
				currentTexture = itemObject.renderer.sharedMaterial.mainTexture;
				this.Repaint();
			}else if(Selection.activeGameObject.tag != "Item"){
				itemSelected = false;
				itemObject = null;
				this.Repaint();
			}
			if(Selection.activeGameObject.tag == "Room" && !roomSelected){
				roomObject = Selection.activeGameObject.gameObject;
				roomSelected = true;
				this.Repaint();
			}else if(Selection.activeGameObject.tag != "Room"){
				roomSelected = false;
				roomObject = null;
				this.Repaint();
			}
		}
	}

	void OnGUI(){
		if(roomSelected){
			EditorGUILayout.HelpBox("Here you can insert RoomContent in a Prefab, These content will" +
				"be spawning within a room",MessageType.Info);
			EditorGUILayout.HelpBox("BE SURE you selected the correct roomtype and check if your Prefab number doesnt already exist!",MessageType.Warning);
			roomNumber = EditorGUILayout.IntField("RoomNumber",roomNumber);
			roomtype = (RoomType)EditorGUILayout.EnumPopup("RoomType", roomtype);
			//if(roomPreview == null){
			//	currentRoomPreview = "Default";
			//}
			if(roomtype.ToString() != currentRoomPreview){
				currentRoomPreview = roomtype.ToString();
				CreateRoom(currentRoomPreview);
			}
			if(GUILayout.Button("Create")){
				CreatePrefab(roomNumber,roomtype);
			}
			DrawPropsWindow();
		}

		foldList = EditorGUILayout.Foldout(foldList,"ItemList");
		if(foldList){
			if(objectList.Count != 0){
				for(int i = 0; i < objectList.Count; i++){
					//objectList[i] = (GameObject)EditorGUILayout.ObjectField("THIS",typeof(GameObject),true);
					if(objectList[i] == null){
						objectList.RemoveAt(i);
						break;
					}
					EditorGUILayout.ObjectField((GameObject)objectList[i],typeof(GameObject),true);
				}
			}
		}
		if(itemSelected){
			EditorGUILayout.HelpBox("You are currently editing a Item Object",MessageType.Info);
			DrawSelectedItemScreen();
		}
		
	}
	void CreatePrefab(int num,RoomType type){
		Debug.Log("Assets/Resources/Prefabs/Rooms/Content/" + type.ToString() + "/" + type.ToString() + num);
		Object prefab = EditorUtility.CreateEmptyPrefab("Assets/Resources/Prefabs/Rooms/Content/" + type.ToString() + "/" + type.ToString() + num +".prefab");
		EditorUtility.ReplacePrefab(Selection.activeGameObject, prefab);
		AssetDatabase.Refresh();
	}
	void DrawPropsWindow(){
		
		GUILayout.Label("Props",EditorStyles.objectFieldThumb);
		EditorGUILayout.BeginHorizontal();
		string[] list1 = new string[] {"Pot", "Pillar", "Chest", "Rock", "Moss", "BrokenPot"};
		string[] list2 = new string[] {"Torch", "Statue", "BrkSword", "Table", "Chair", "Anvil"};
		string[] list3 = new string[] {"Furnace", "Fountain", "Tree", "Bush", "Saw", "Crystal"};
		DrawPropList(list1);
		DrawPropList(list2);
		DrawPropList(list3);
		EditorGUILayout.EndHorizontal();
		GUILayout.Label("",EditorStyles.objectFieldThumb);
		
	}
	void DrawPropList(string[] items){
		EditorGUILayout.BeginVertical();
		for(int i = 0; i < items.Length; i++){
			if(GUILayout.Button(items[i].ToString())){
				CreateItem(items[i].ToString());
			}
		}
		EditorGUILayout.EndVertical();
	}
	void CreateRoom(string name){
		DestroyImmediate(roomPreview);
		GameObject instantiatedItem = Instantiate(Resources.Load("Prefabs/Rooms/Template/"+name),new Vector3(0,0.5f,0),Quaternion.identity) as GameObject;
		instantiatedItem.name = name;
		instantiatedItem.tag = "Room";
		roomPreview = instantiatedItem;
	}
	void CreateItem(string name){
		GameObject instantiatedItem = Instantiate(Resources.Load("Prefabs/RoomItems/"+name),new Vector3(0,1,0),Quaternion.identity) as GameObject;
		instantiatedItem.name = name;
		instantiatedItem.transform.parent = roomObject.transform;
		Object[] selectedobject = new Object[1];
		selectedobject[0] = instantiatedItem;
		Selection.objects = selectedobject;
		objectList.Add(instantiatedItem);
	}
	void DrawSelectedItemScreen(){
		GUILayout.Label("Selected Item",EditorStyles.objectFieldThumb);
		EditorGUILayout.HelpBox("Here you can Edit the Room Item you currently have selected",MessageType.Info);
		foldTexture = EditorGUILayout.Foldout(foldTexture,"Texture/Material");
		if(foldTexture){
			currentTexture = (Texture)EditorGUILayout.ObjectField(currentTexture,typeof(Texture),true);
			currentObject.renderer.sharedMaterial.mainTexture = currentTexture;
		}

		if(itemObject != null){
			if(gameobjectEditor == null){
				gameobjectEditor = Editor.CreateEditor(itemObject);
			}
			gameobjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(200,200),EditorStyles.layerMaskField);
		}

		if(GUILayout.Button("Deselect Item")){
			Selection.objects = new GameObject[0];
		}
		if(GUILayout.Button("Destroy Item")){
			Selection.objects = new GameObject[0];
			DestroyImmediate(currentObject);
		}
		GUILayout.Label("",EditorStyles.objectFieldThumb);
	}
}
	
	/*
	}
	void DrawSelectedItemScreen(){

		showItemSelector = EditorGUILayout.InspectorTitlebar(showItemSelector,selectedItem);
		if(showItemSelector && selectedItem){
			
			EditorGUILayout.HelpBox("Here you can Edit the Room Item you currently have selected",MessageType.Info);
			Color selectedItemColor = selectedItemGameObject.renderer.sharedMaterial.color;
			foldTexture = EditorGUILayout.Foldout(foldTexture,"Texture/Material");
			if(foldTexture){
				selectedItemColor = EditorGUILayout.ColorField(selectedItemColor);
				selectedObjectTexture = selectedItemGameObject.renderer.sharedMaterial.mainTexture;
				selectedItemGameObject.renderer.sharedMaterial.mainTexture = (Texture)EditorGUILayout.ObjectField(selectedObjectTexture,typeof(Texture),true);
			}
		}else{
			EditorGUILayout.HelpBox("No Object with RoomObject found. To edit a RoomObject Select a GameObject with a RoomObject Script attached to it",MessageType.Warning);
		}
		if(gameobj != null){
			if(gameobjectEditor == null){
				gameobjectEditor = Editor.CreateEditor(gameobj);
			}
			gameobjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(200,200),EditorStyles.layerMaskField);
		}
		if(selectedItem != null){
			if(GUILayout.Button("Deselect Item")){
				Selection.objects = new GameObject[0];
			}
			if(GUILayout.Button("Destroy Item")){
				Selection.objects = new GameObject[0];
				DestroyImmediate(selectedItemGameObject);
			}
		}
		
		if(selectedItem == null){
			DrawPropsWindow();
		}

	}

	void DrawPropsWindow(){

		GUILayout.Label("Props",EditorStyles.objectFieldThumb);
		EditorGUILayout.BeginHorizontal();
		DrawItemWindowOne();
		DrawItemWindowTwo();
		DrawItemWindowThree();
		EditorGUILayout.EndHorizontal();
		GUILayout.Label("",EditorStyles.objectFieldThumb);

	}
	void DrawItemWindowOne(){

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos,GUILayout.Width(100),GUILayout.Height(150));
		if(GUILayout.Button("Pot")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("Torch")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("Box")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("Corpse")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("BloodStain")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("Chains")){
			CreateItem("Pot");
		}
		EditorGUILayout.EndScrollView();
	}
	void DrawItemWindowTwo(){
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos,GUILayout.Width(100),GUILayout.Height(150));
		if(GUILayout.Button("Spikes")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("Statue")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("Rubble")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("Chain-Floor")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("GargoyleDead")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("WarriorDead")){
			CreateItem("Pot");
		}
		EditorGUILayout.EndScrollView();

	}
	void DrawItemWindowThree(){
		/*
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos,GUILayout.Width(100),GUILayout.Height(150));
		if(GUILayout.Button("EmptyChest")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("BrokenSword")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("Moss")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("Pillar")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("Wall")){
			CreateItem("Pot");
		}
		if(GUILayout.Button("CrackedFloor")){
			CreateItem("Pot");
		}
		EditorGUILayout.EndScrollView();

	}
	void OnInspectorUpdate(){

		this.Repaint();
		if(Selection.activeGameObject){
			if(Selection.activeGameObject.gameObject != previousObject){
				selectedItemGameObject = null;
				gameobj = null;
				//gameobjectEditor = null;
			}
			previousObject = Selection.activeGameObject.gameObject;
			Debug.Log(Selection.activeGameObject.gameObject);
			selectedItem = Selection.activeGameObject.GetComponent<RoomObject>();
			selectedItemGameObject = Selection.activeGameObject.gameObject;
			gameobjectEditor = null;
		gameobj = selectedItemGameObject;
		}else{
			//gameobjectEditor = null;
			//selectedItem = null;
			//gameobj = null;
		}

	}
	*/
