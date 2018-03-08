#pragma once

//plugins are stored by index with seperate indexes for vsti and vst

#include "PluginHost.h"

#define VSTEXPORT __declspec(dllexport) 
#define isUnityDLL 1

std::shared_ptr<VstHost> vstHost;
bool hostInitialised = false;

extern "C" {
	VSTEXPORT void initHost();
	VSTEXPORT void setSampleRate(long p_sampleRate);
	VSTEXPORT void setHostBlockSize(int p_blocksize);
	VSTEXPORT int loadEffect(char* path);
	VSTEXPORT int loadInstrument(char* path);
	VSTEXPORT float* processFxAudio(int vstIndex, float* audioThrough, long numFrames, int numChannels);
	VSTEXPORT float* processInstAudio(int vstIndex, long numFrames, int numChannels);

	//Debugger
	typedef void(*FuncCallBack)(const char* message, int color, int size);
	static FuncCallBack callbackInstance = nullptr;
	VSTEXPORT void RegisterDebugCallback(FuncCallBack cb);
}
