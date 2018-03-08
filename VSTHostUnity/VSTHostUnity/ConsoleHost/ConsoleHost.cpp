// ConsoleHost.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "ConsoleFuncs.h"
#define isConsoleVersion

int main()
{ 
	Debug::Log("Appl starting");
	ConsoleHost consoleHost;
	Debug::Log("Console accessor class created");
	consoleHost.initHost();
	Debug::Log("Host initialised");
	consoleHost.setSampleRate(44100);
	Debug::Log("SR setup");
	consoleHost.setHostBlockSize(1);
	Debug::Log("Blocksize set");
	int effectIndex = consoleHost.loadEffect("Blah");
	Debug::Log("effect loaded");
	float someSound[1];
	someSound[0] = 0.0f;
	consoleHost.processFxAudio(effectIndex, someSound, 1, 1);
	Debug::Log("Audio through");
    return 0;
}
