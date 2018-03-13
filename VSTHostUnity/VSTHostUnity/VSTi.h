#pragma once
#include "VSTBase.h"

class VSTi : public VSTBase
{
public:
	VSTi(std::string path, VstBasicParams& p_host);
	~VSTi();
	float* processAudio(long numFrames, int numChannels);
	void midiEvent(int status, int mess1, int mess2, long delaySamps);
	int getNumPluginOutputs();
	void initializeIO();

private:
	std::vector<std::vector<float>> pluginOutputs;
	std::string pluginPath;
	float** pluginOutputsStartPtr;
	VstEvent eventHolder;
	VstEvents* multEventsHolder;
	void setNumInOut();
};
