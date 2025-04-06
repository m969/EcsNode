/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

import "csharp"
import "puerts"
import fgui = CS.FairyGUI
import { $typeof } from "puerts";
import UI_LoginWindow from "./UI_LoginWindow.mjs";

export default class LoginBinder {
	public static bindAll():void {
		fgui.UIObjectFactory.SetPackageItemExtension(UI_LoginWindow.URL, $typeof(UI_LoginWindow));
	}
}