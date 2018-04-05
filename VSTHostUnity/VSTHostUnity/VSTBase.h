#pragma once
#include "stdafx.h"
#include <vector>
#include <algorithm>
#include <Objbase.h>
#include <tchar.h>
#include "DebugCPP.h"
#include <memory>
#include "HostGlobals.h"

#define TEMP_PARAM_NAME_SIZE 128

class VstBasicParams
{
public:
	int blocksize;
	int samplerate;
	VstBasicParams();
};

class VSTBase
{
public:
	VSTBase(std::string& pluginPath);
	int getNumParams();
	void setParam(int paramIndex, float p_value);
	float getParam(int index);
	std::string& getParamName(int index);
	void setVstIndex(int p_index);
	bool inline getPluginReady() { return pluginReady; }

protected:

	AEffect* plugin = nullptr;
	void silenceChannel(std::vector<std::vector<float>> channelData);
	void startPlugin();
	void resumePlugin();
	void suspendPlugin();
	bool canPluginDo(char *canDoString);
	int samplerate;
	int blocksize;
	int vstIndex = -1;
	int pluginNumInputs = 0;
	int pluginNumOutputs = 0;
	bool pluginReady = false;
	
private:

	std::vector<std::string> paramNames;
	int loadPlugin(std::string& path);
	int configurePluginCallbacks();
	virtual void initializeIO() = 0;
	char tempParamName[TEMP_PARAM_NAME_SIZE];
};

extern "C"
{
	VstIntPtr hostCallback(AEffect *effect, VstInt32 opcode, VstInt32 index,
		VstIntPtr value, void *ptr, float opt);
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
}
