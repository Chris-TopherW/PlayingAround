#pragma once
#include "stdafx.h"
#include <vector>
#include <algorithm>
#include <Objbase.h>
#include <tchar.h>
#include "DebugCPP.h"
#include <memory>
#include "HostGlobals.h"

class VstBasicParams
{
public:
	int blocksize;
	int samplerate;
	VstBasicParams();
	//static std::vector<std::vector<float>> inputsHolder;
	//static std::vector<std::vector<float>> outputsHolder;
};

extern "C"
{
	// Plugin's entry point
	typedef AEffect *(*vstPluginFuncPtr)(audioMasterCallback host);
	// Plugin's dispatcher function
	typedef VstIntPtr(*dispatcherFuncPtr)(AEffect* effect, VstInt32 opcode, VstInt32 index, VstIntPtr value, void* ptr, float opt);
	// Plugin's getParameter() method
	typedef float(*getParameterFuncPtr)(AEffect *effect, VstInt32 index);
	// Plugin's setParameter() method
	typedef void(*setParameterFuncPtr)(AEffect *effect, VstInt32 index, float value);
	// Plugin's processEvents() method
	typedef VstInt32(*processEventsFuncPtr)(VstEvents *events);
	// Plugin's process() method
	typedef void(*processFuncPtr)(AEffect *effect, float **inputs,
		float **outputs, VstInt32 sampleFrames);
}

class VSTBase
{
public:
	VSTBase(const wchar_t* pluginPath);
	~VSTBase();
	int getNumParams();
	void setParam(int paramIndex, float p_value);
	float getParam(int index);
	char* getParamName(int index);
	void setVstIndex(int p_index);

protected:

	AEffect* plugin = NULL;
	void silenceChannel(float **channelData, int numChannels, long numFrames);
	void startPlugin();
	void resumePlugin();
	void suspendPlugin();
	bool canPluginDo(char *canDoString);
	VstBasicParams* hostParams;
	int vstIndex = -1;
	int pluginNumInputs = 0;
	int pluginNumOutputs = 0;
	
private:

	char** paramNames;
	//std::vector<std::string> paramNames;

	void loadPlugin(const wchar_t* path);
	int configurePluginCallbacks();
	virtual void initializeIO() = 0;
	
};

extern "C"
{
	VstIntPtr hostCallback(AEffect *effect, VstInt32 opcode, VstInt32 index,
		VstIntPtr value, void *ptr, float opt);
}
