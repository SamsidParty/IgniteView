import { getCommandBridge } from "./commandBridge";
import "./localStorage";
import "./main";
import "./polyfills";
import "./reactHelpers";
import "./sharedContext";
import "./styles";
import "./windowDragging";

void getCommandBridge().invoke("igniteview_call_load_event");