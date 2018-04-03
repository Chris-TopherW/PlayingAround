#pragma once

//plugins are stored by index with seperate indexes for vsti and vst

#include "PluginHost.h"

#define VSTEXPORT __declspec(dllexport) 
#define isUnityDLL 1

std::shared_ptr<VstHost> vstHost;
bool hostInitialised = false;

extern "C" {
	VSTEXPORT void initHost();
	VSTEXPORT int setSampleRate(long p_sampleRate);
	VSTEXPORT int setHostBlockSize(int p_blocksize);
	VSTEXPORT int loadEffect(const char* path);
	VSTEXPORT int loadInstrument(const char* path);
	VSTEXPORT float* processFxAudio(int vstIndex, float* audioThrough, long numFrames, int numChannels);
	VSTEXPORT float* processInstAudio(int vstIndex, long numFrames, int numChannels);
	VSTEXPORT int getNumPluginInputs(int vstIndex);
	VSTEXPORT int getNumPluginOutputs(int vstIndex);
	VSTEXPORT int getNumParams(int vstIndex);
	VSTEXPORT float getParam(int vstIndex, int paramIndex);
	VSTEXPORT char* getParamName(int vstIndex, int paramIndex);
	VSTEXPORT int setParam(int vstIndex, int paramIndex, float p_value);

	//debugging stuff
	VSTEXPORT int getEffectVectorSize();
	VSTEXPORT int getHostNumRef();
}
