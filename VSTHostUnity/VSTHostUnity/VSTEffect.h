#pragma once
#include "VSTBase.h"

class VSTEffect : public VSTBase
{
public:
	VSTEffect(std::string& path, VstBasicParams& p_hostParams);
	~VSTEffect();
	float* processAudio(float* audioThrough, long numFrames, int numChannels);
	int getNumPluginInputs();
	int getNumPluginOutputs();
	void initializeIO();

private:
	std::vector<std::vector<float>> pluginInputs;
	std::vector<std::vector<float>> pluginOutputs;
	std::string pluginPath;
	float** pluginInputsStartPtr;
	float** pluginOutputsStartPtr;
};
