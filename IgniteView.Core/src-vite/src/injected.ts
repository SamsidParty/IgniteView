import "./main";
import { getCommandBridge } from "./commandBridge";
import "./localStorage";
import "./sharedContext";
import "./styles";
import "./polyfills";
import "./windowDragging";
import "./reactHelpers";

void getCommandBridge().invoke("igniteview_call_load_event");