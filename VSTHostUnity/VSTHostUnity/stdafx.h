// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"



#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>

// TODO: reference additional headers your program requires here

#include <aeffect.h>
#include <aeffectx.h>
#include <vstfxstore.h>

#define VSTEXPORT __declspec(dllexport) 

extern "C"
{
	VSTEXPORT float** getOutput();
	VSTEXPORT void setBlockSize(int p_blocksize);
	VSTEXPORT void setNumChannels(int p_numChannels);
	// Main host callback
	VstIntPtr VSTCALLBACK hostCallback(AEffect *effect, VstInt32 opcode, VstInt32 index,
		VstIntPtr value, void *ptr, float opt);

	const wchar_t* GetWC(const char *c);
	VSTEXPORT /*AEffect* */ void loadPlugin(/*char* path*/);
	VSTEXPORT int configurePluginCallbacks(/*AEffect *plugin*/);
	VSTEXPORT void startPlugin(/*AEffect *plugin*/);
	VSTEXPORT void resumePlugin(/*AEffect *plugin*/);
	VSTEXPORT void suspendPlugin(/*AEffect *plugin*/);
	//VstIntPtr VSTCALLBACK hostCallback(AEffect *effect, VstIntPtr opcode, VstIntPtr index,
	//	VstIntPtr value, void *ptr, float opt);
	VSTEXPORT void initializeIO();
	//VSTEXPORT void processAudio(/*AEffect *plugin, */float **inputs, float **outputs,
	//	long numFrames);
	VSTEXPORT float* processAudio(float* in, long numFrames);
	VSTEXPORT void silenceChannel(float **channelData, int numChannels, long numFrames);
	VSTEXPORT void processMidi(/*AEffect *plugin, */VstEvents *events);
	VSTEXPORT char* cDebugDelegate();

	VSTEXPORT void shutdown();
	VSTEXPORT void start();

	void addDebugMess(char* p_message);
}

//debugger
extern "C"
{
	//Create a callback delegate
	typedef void(*FuncCallBack)(const char* message, int color, int size);
	static FuncCallBack callbackInstance = nullptr;
	VSTEXPORT void RegisterDebugCallback(FuncCallBack cb);
}


// Plugin's entry point
typedef AEffect *(*vstPluginFuncPtr)(audioMasterCallback host);
// Plugin's dispatcher function
typedef VstIntPtr(*dispatcherFuncPtr)(AEffect *effect, VstInt32 opCode,
	VstInt32 index, VstInt32 value, void *ptr, float opt);
// Plugin's getParameter() method
typedef float(*getParameterFuncPtr)(AEffect *effect, VstInt32 index);
// Plugin's setParameter() method
typedef void(*setParameterFuncPtr)(AEffect *effect, VstInt32 index, float value);
// Plugin's processEvents() method
typedef VstInt32(*processEventsFuncPtr)(VstEvents *events);
// Plugin's process() method
typedef void(*processFuncPtr)(AEffect *effect, float **inputs,
	float **outputs, VstInt32 sampleFrames);


