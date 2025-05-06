/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

import "csharp"
import "puerts"
import fgui = CS.FairyGUI
import { $typeof } from "puerts";
export default class UI_LoginWindow extends fgui.GComponent {

	public m_nFrame1:fgui.GTextField;
	public static URL:string = "ui://g9o3wgyhpggt0";

	public static createInstance():UI_LoginWindow {
		return (fgui.UIPackage.CreateObject("Login", "LoginWindow")) as UI_LoginWindow;
	}

	protected onConstruct():void {
		this.m_nFrame1 = (this.GetChildAt(0)) as fgui.GTextField;
	}
}