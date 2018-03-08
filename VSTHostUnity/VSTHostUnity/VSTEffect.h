#pragma once
#include "VSTBase.h"

class VSTEffect : public VSTBase
{
public:
	VSTEffect(const wchar_t* path, VstBasicParams* p_hostParams);
	~VSTEffect();
	float* processAudio(float* audioThrough, long numFrames, int numChannels);
	int getNumPluginInputs();
	int getNumPluginOutputs();
	void initializeIO();

private:
	float** pluginInputs = NULL;
	float** pluginOutputs = NULL;
	std::string pluginPath;
};
