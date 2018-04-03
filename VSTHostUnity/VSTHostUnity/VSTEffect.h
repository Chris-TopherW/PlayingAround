#pragma once
#include "VSTBase.h"

class VSTEffect : public VSTBase
{
public:
	VSTEffect(std::string& path, int blocksize, int samplerate);
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
